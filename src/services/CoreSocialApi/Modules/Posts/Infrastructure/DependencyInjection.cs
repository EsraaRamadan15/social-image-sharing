using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Posts.Application;
using SharedInfrastructure.Persistence;

namespace Posts.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPostsInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddModelConfigurationsFromAssembly(typeof(DependencyInjection).Assembly);
            services.AddPostsApplication();

            return services;
        }
    }
}
