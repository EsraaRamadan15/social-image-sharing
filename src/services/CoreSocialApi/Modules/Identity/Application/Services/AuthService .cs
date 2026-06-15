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
            _refreshTokenRepository = refreshTokenRepository;
            _profileRepository = profileRepository;
            _passwordHasher = passwordHasher;
            _refreshTokenHasher = refreshTokenHasher;
            _tokenService = tokenService;
            _dbContext = dbContext;
            _dateTimeProvider = dateTimeProvider;
            _jwtOptions = jwtOptions.Value;
        }

        public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, string? ipAddress, CancellationToken cancellationToken = default)
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
            var refreshToken = RefreshToken.Create(
                Guid.NewGuid(),
                user.Id,
                _refreshTokenHasher.Hash(refreshTokenValue),
                now.AddDays(_jwtOptions.RefreshTokenExpirationDays),
                now,
                ipAddress);

            await _userRepository.AddAsync(user, cancellationToken);
            await _profileRepository.AddAsync(profile, cancellationToken);
            await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<AuthResponse>.Success(new AuthResponse
            {
                AccessToken = _tokenService.CreateAccessToken(user),
                RefreshToken = refreshTokenValue,
                ExpiresAtUtc = now.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes)
            });
        }

        public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, string? ipAddress, CancellationToken cancellationToken = default)
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            var user = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

            if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
                return Result<AuthResponse>.Failure(AuthErrors.InvalidCredentials);

            if (!user.IsActive)
                return Result<AuthResponse>.Failure(AuthErrors.UserInactive);

            var now = _dateTimeProvider.UtcNow;
            var refreshTokenValue = _tokenService.CreateRefreshToken();

            var refreshToken = RefreshToken.Create(
                Guid.NewGuid(),
                user.Id,
                _refreshTokenHasher.Hash(refreshTokenValue),
                now.AddDays(_jwtOptions.RefreshTokenExpirationDays),
                now,
                ipAddress);

            await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<AuthResponse>.Success(new AuthResponse
            {
                AccessToken = _tokenService.CreateAccessToken(user),
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
                return Result<AuthResponse>.Failure(AuthErrors.InvalidRefreshToken);

            if (existingToken.IsExpiredAt(now))
                return Result<AuthResponse>.Failure(AuthErrors.ExpiredRefreshToken);

            var user = await _userRepository.GetByIdAsync(existingToken.UserId, cancellationToken);
            if (user is null || !user.IsActive)
                return Result<AuthResponse>.Failure(AuthErrors.InvalidRefreshToken);

            var newRefreshTokenValue = _tokenService.CreateRefreshToken();
            var newRefreshTokenHash = _refreshTokenHasher.Hash(newRefreshTokenValue);

            existingToken.Revoke(now, newRefreshTokenHash);

            var newRefreshToken = RefreshToken.Create(
                Guid.NewGuid(),
                user.Id,
                newRefreshTokenHash,
                now.AddDays(_jwtOptions.RefreshTokenExpirationDays),
                now,
                ipAddress);

            await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<AuthResponse>.Success(new AuthResponse
            {
                AccessToken = _tokenService.CreateAccessToken(user),
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
                existingToken.Revoke(_dateTimeProvider.UtcNow);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            return Result.Success();
        }
    }
}
