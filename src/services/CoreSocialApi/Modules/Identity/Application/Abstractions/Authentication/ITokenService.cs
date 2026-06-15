using Identity.Domain.Entities;

namespace Identity.Application.Abstractions.Authentication
{

    public interface ITokenService
    {
        string CreateAccessToken(User user);
        string CreateRefreshToken();
    }
}
