using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Posts.Application.Authorization;

namespace Posts.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPostsApplication(this IServiceCollection services)
        {
            services.AddScoped<IAuthorizationHandler, PostOwnerOrAdminHandler>();

            return services;
        }
    }
}
