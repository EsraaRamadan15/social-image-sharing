using Identity.Domain.Entities;

namespace Identity.Application.Abstractions
{

    public interface ITokenService
    {
        string CreateAccessToken(User user);
        string CreateRefreshToken();
    }
}
