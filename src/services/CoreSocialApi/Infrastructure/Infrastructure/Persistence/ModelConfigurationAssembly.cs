using System.Reflection;

namespace SharedInfrastructure.Persistence
{
    public interface IModelConfigurationAssembly
    {
        Assembly Assembly { get; }
    }

    public sealed class ModelConfigurationAssembly : IModelConfigurationAssembly
    {
        public ModelConfigurationAssembly(Assembly assembly)
        {
            Assembly = assembly;
        }

        public Assembly Assembly { get; }
    }
}
