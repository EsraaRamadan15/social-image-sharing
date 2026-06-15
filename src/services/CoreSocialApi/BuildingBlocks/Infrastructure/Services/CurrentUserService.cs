using Application.Abstractions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;


namespace Infrastructure.Services
{

    public sealed class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId
        {
            get
            {
                var userIdValue = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                return Guid.TryParse(userIdValue, out var userId) ? userId : null;
            }
        }

        public string? UserName =>
            _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);

        public bool IsAuthenticated =>
            _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
    }
}
