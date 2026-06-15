using Identity.Application.Abstractions.Authentication;
using System.Security.Cryptography;
using System.Text;

namespace SharedInfrastructure.Authentication
{
    public sealed class Sha256RefreshTokenHasher : IRefreshTokenHasher
    {
        public string Hash(string token)
        {
            ArgumentException.ThrowIfNullOrEmpty(token);

            var bytes = Encoding.UTF8.GetBytes(token);
            var hash = SHA256.HashData(bytes);
            return Convert.ToHexString(hash).ToLowerInvariant();
        }
    }
}
