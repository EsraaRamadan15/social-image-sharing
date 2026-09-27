using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Profiles.Application.Abstractions;
using Profiles.Infrastructure.Repositories;
using SharedInfrastructure.Persistence;

namespace Profiles.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddProfilesInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddModelConfigurationsFromAssembly(typeof(DependencyInjection).Assembly);
            services.AddScoped<IProfileRepository, ProfileRepository>();

            return services;
        }
    }
}
