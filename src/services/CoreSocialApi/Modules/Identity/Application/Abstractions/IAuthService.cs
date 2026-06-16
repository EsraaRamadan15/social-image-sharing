using Application.Common;
using Identity.Application.Dtos;

namespace Identity.Application.Abstractions
{
    public interface IAuthService
    {
        Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, string? ipAddress, string? userAgent, CancellationToken cancellationToken = default);

        Task<Result<AuthResponse>> LoginAsync(LoginRequest request, string? ipAddress, string? userAgent, CancellationToken cancellationToken = default);

        Task<Result<AuthResponse>> RefreshTokenAsync(string refreshToken, string? ipAddress, CancellationToken cancellationToken = default);

        Task<Result> LogoutAsync(string refreshToken, CancellationToken cancellationToken = default);

        Task<Result<IReadOnlyList<UserSessionResponse>>> ListSessionsAsync(Guid userId, Guid? currentSessionId, CancellationToken cancellationToken = default);

        Task<Result> RevokeSessionAsync(Guid userId, Guid sessionId, CancellationToken cancellationToken = default);
    }
}
