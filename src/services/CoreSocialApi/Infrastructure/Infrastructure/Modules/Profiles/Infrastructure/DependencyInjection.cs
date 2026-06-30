using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Profiles.Application.Abstractions;
using SharedInfrastructure.Repositories.Profiles;

namespace Profiles.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddProfilesInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<IProfileRepository, ProfileRepository>();

            return services;
        }
    }
}
