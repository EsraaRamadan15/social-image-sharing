using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Posts.Application;

namespace Posts.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPostsInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddPostsApplication();

            return services;
        }
    }
}
