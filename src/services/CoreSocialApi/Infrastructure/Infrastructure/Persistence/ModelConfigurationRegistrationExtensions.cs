using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace SharedInfrastructure.Persistence
{
    public static class ModelConfigurationRegistrationExtensions
    {
        public static IServiceCollection AddModelConfigurationsFromAssembly(
            this IServiceCollection services,
            Assembly assembly)
        {
            services.AddSingleton<IModelConfigurationAssembly>(
                new ModelConfigurationAssembly(assembly));

            return services;
        }
    }
}
