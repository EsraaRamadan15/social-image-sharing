using Application.Abstractions;
using Application.Common;
using Identity.Application.Abstractions;
using Identity.Application.Abstractions.Authentication;
using Identity.Application.Abstractions.Repositories;
using Identity.Application.Dtos;
using Identity.Application.Errors;
using Identity.Application.Settings;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Profiles.Application.Abstractions;
using Profiles.Domain.Entities;

namespace Identity.Application.Services
{

    public sealed class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserSessionRepository _userSessionRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IRefreshTokenHasher _refreshTokenHasher;
        private readonly ITokenService _tokenService;
        private readonly IApplicationDbContext _dbContext;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly JwtOptions _jwtOptions;

        public AuthService(
            IUserRepository userRepository,
            IUserSessionRepository userSessionRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IProfileRepository profileRepository,
            IPasswordHasher passwordHasher,
            IRefreshTokenHasher refreshTokenHasher,
            ITokenService tokenService,
            IApplicationDbContext dbContext,
            IDateTimeProvider dateTimeProvider,
            Microsoft.Extensions.Options.IOptions<JwtOptions> jwtOptions)
        {
            _userRepository = userRepository;
            _userSessionRepository = userSessionRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _profileRepository = profileRepository;
            _passwordHasher = passwordHasher;
            _refreshTokenHasher = refreshTokenHasher;
            _tokenService = tokenService;
            _dbContext = dbContext;
            _dateTimeProvider = dateTimeProvider;
            _jwtOptions = jwtOptions.Value;
        }

        public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, string? ipAddress, string? userAgent, CancellationToken cancellationToken = default)
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            var normalizedUserName = request.UserName.Trim();

            var existingUserByEmail = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
            if (existingUserByEmail is not null)
                return Result<AuthResponse>.Failure(AuthErrors.EmailAlreadyExists);

            var existingUserByUserName = await _userRepository.GetByUserNameAsync(normalizedUserName, cancellationToken);
            if (existingUserByUserName is not null)
                return Result<AuthResponse>.Failure(AuthErrors.UserNameAlreadyExists);

            var now = _dateTimeProvider.UtcNow;
            var user = User.Create(
                Guid.NewGuid(),
                normalizedUserName,
                normalizedEmail,
                _passwordHasher.Hash(request.Password),
                UserRole.User,
                now);

            var profile = Profile.Create(
                Guid.NewGuid(),
                user.Id,
                normalizedUserName,
                now);

            var refreshTokenValue = _tokenService.CreateRefreshToken();
            var session = UserSession.Create(
                Guid.NewGuid(),
                user.Id,
                GetDeviceName(userAgent),
                userAgent,
                ipAddress,
                now);

            var refreshToken = RefreshToken.Create(
                Guid.NewGuid(),
                user.Id,
                session.Id,
                _refreshTokenHasher.Hash(refreshTokenValue),
                now.AddDays(_jwtOptions.RefreshTokenExpirationDays),
                now,
                ipAddress);

            await _userRepository.AddAsync(user, cancellationToken);
            await _profileRepository.AddAsync(profile, cancellationToken);
            await _userSessionRepository.AddAsync(session, cancellationToken);
            await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<AuthResponse>.Success(new AuthResponse
            {
                AccessToken = _tokenService.CreateAccessToken(user, session.Id),
                RefreshToken = refreshTokenValue,
                ExpiresAtUtc = now.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes)
            });
        }

        public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, string? ipAddress, string? userAgent, CancellationToken cancellationToken = default)
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            var user = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

            if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
                return Result<AuthResponse>.Failure(AuthErrors.InvalidCredentials);

            if (!user.IsActive)
                return Result<AuthResponse>.Failure(AuthErrors.UserInactive);

            var now = _dateTimeProvider.UtcNow;
            var refreshTokenValue = _tokenService.CreateRefreshToken();
            var session = UserSession.Create(
                Guid.NewGuid(),
                user.Id,
                GetDeviceName(userAgent),
                userAgent,
                ipAddress,
                now);

            var refreshToken = RefreshToken.Create(
                Guid.NewGuid(),
                user.Id,
                session.Id,
                _refreshTokenHasher.Hash(refreshTokenValue),
                now.AddDays(_jwtOptions.RefreshTokenExpirationDays),
                now,
                ipAddress);

            await _userSessionRepository.AddAsync(session, cancellationToken);
            await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<AuthResponse>.Success(new AuthResponse
            {
                AccessToken = _tokenService.CreateAccessToken(user, session.Id),
                RefreshToken = refreshTokenValue,
                ExpiresAtUtc = now.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes)
            });
        }

        public async Task<Result<AuthResponse>> RefreshTokenAsync(string refreshToken, string? ipAddress, CancellationToken cancellationToken = default)
        {
            var refreshTokenHash = _refreshTokenHasher.Hash(refreshToken);
            var existingToken = await _refreshTokenRepository.GetByTokenHashAsync(refreshTokenHash, cancellationToken);
            if (existingToken is null)
                return Result<AuthResponse>.Failure(AuthErrors.InvalidRefreshToken);

            var now = _dateTimeProvider.UtcNow;

            if (existingToken.IsRevoked)
            {
                if (existingToken.ReplacedByToken is not null)
                {
                    await RevokeSessionInternalAsync(existingToken.SessionId, now, cancellationToken);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }

                return Result<AuthResponse>.Failure(AuthErrors.InvalidRefreshToken);
            }

            if (existingToken.IsExpiredAt(now))
                return Result<AuthResponse>.Failure(AuthErrors.ExpiredRefreshToken);

            var session = await _userSessionRepository.GetByIdAsync(existingToken.SessionId, cancellationToken);
            if (session is null || session.IsRevoked)
                return Result<AuthResponse>.Failure(AuthErrors.InvalidRefreshToken);

            var user = await _userRepository.GetByIdAsync(existingToken.UserId, cancellationToken);
            if (user is null || !user.IsActive)
                return Result<AuthResponse>.Failure(AuthErrors.InvalidRefreshToken);

            var newRefreshTokenValue = _tokenService.CreateRefreshToken();
            var newRefreshTokenHash = _refreshTokenHasher.Hash(newRefreshTokenValue);

            existingToken.Revoke(now, newRefreshTokenHash);

            var newRefreshToken = RefreshToken.Create(
                Guid.NewGuid(),
                user.Id,
                existingToken.SessionId,
                newRefreshTokenHash,
                now.AddDays(_jwtOptions.RefreshTokenExpirationDays),
                now,
                ipAddress);

            session.MarkSeen(now, ipAddress);

            await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<AuthResponse>.Success(new AuthResponse
            {
                AccessToken = _tokenService.CreateAccessToken(user, existingToken.SessionId),
                RefreshToken = newRefreshTokenValue,
                ExpiresAtUtc = now.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes)
            });
        }

        public async Task<Result> LogoutAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            var refreshTokenHash = _refreshTokenHasher.Hash(refreshToken);
            var existingToken = await _refreshTokenRepository.GetByTokenHashAsync(refreshTokenHash, cancellationToken);
            if (existingToken is null)
                return Result.Failure(AuthErrors.InvalidRefreshToken);

            if (!existingToken.IsRevoked)
            {
                var now = _dateTimeProvider.UtcNow;
                existingToken.Revoke(now);
                var session = await _userSessionRepository.GetByIdAsync(existingToken.SessionId, cancellationToken);
                session?.Revoke(now);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            return Result.Success();
        }

        public async Task<Result<IReadOnlyList<UserSessionResponse>>> ListSessionsAsync(Guid userId, Guid? currentSessionId, CancellationToken cancellationToken = default)
        {
            var sessions = await _userSessionRepository.ListByUserIdAsync(userId, cancellationToken);

            var response = sessions
                .Select(x => new UserSessionResponse
                {
                    Id = x.Id,
                    DeviceName = x.DeviceName,
                    UserAgent = x.UserAgent,
                    CreatedByIp = x.CreatedByIp,
                    LastSeenIp = x.LastSeenIp,
                    CreatedAtUtc = x.CreatedAtUtc,
                    LastSeenAtUtc = x.LastSeenAtUtc,
                    IsCurrent = currentSessionId == x.Id
                })
                .ToList();

            return Result<IReadOnlyList<UserSessionResponse>>.Success(response);
        }

        public async Task<Result> RevokeSessionAsync(Guid userId, Guid sessionId, CancellationToken cancellationToken = default)
        {
            var session = await _userSessionRepository.GetByIdForUserAsync(sessionId, userId, cancellationToken);
            if (session is null)
                return Result.Failure(AuthErrors.InvalidSession);

            if (!session.IsRevoked)
            {
                await RevokeSessionInternalAsync(session.Id, _dateTimeProvider.UtcNow, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            return Result.Success();
        }

        private async Task RevokeSessionInternalAsync(Guid sessionId, DateTimeOffset now, CancellationToken cancellationToken)
        {
            var session = await _userSessionRepository.GetByIdAsync(sessionId, cancellationToken);
            session?.Revoke(now);

            var activeTokens = await _refreshTokenRepository.ListActiveBySessionIdAsync(sessionId, now, cancellationToken);
            foreach (var activeToken in activeTokens)
            {
                activeToken.Revoke(now);
            }
        }

        private static string GetDeviceName(string? userAgent)
        {
            if (string.IsNullOrWhiteSpace(userAgent))
                return "Unknown device";

            if (userAgent.Contains("Mobile", StringComparison.OrdinalIgnoreCase))
                return "Mobile device";

            if (userAgent.Contains("Windows", StringComparison.OrdinalIgnoreCase))
                return "Windows device";

            if (userAgent.Contains("Mac", StringComparison.OrdinalIgnoreCase))
                return "Mac device";

            if (userAgent.Contains("Linux", StringComparison.OrdinalIgnoreCase))
                return "Linux device";

            return "Unknown device";
        }
    }
}
