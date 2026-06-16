using Identity.Domain.Entities;

namespace Identity.Application.Abstractions.Authentication
{

    public interface ITokenService
    {
        string CreateAccessToken(User user, Guid sessionId);
        string CreateRefreshToken();
    }
}
