using System.ComponentModel;
using System.Reflection;

namespace Kavics.LogInterpreter.Abstractions;

internal class PipelineItemScanner
{
    public List<PipelineItemDescriptor> Discover()
    {
        var descriptors = new List<PipelineItemDescriptor>();

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var assembly in assemblies)
        {
            try
            {
                var types = assembly.GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && typeof(IPipelineItem).IsAssignableFrom(t));

                foreach (var type in types)
                {
                    var descriptionAttribute = type.GetCustomAttribute<DescriptionAttribute>();
                    
                    var descriptor = new PipelineItemDescriptor
                    {
                        Type = type,
                        Name = type.Name,
                        Description = descriptionAttribute?.Description ?? string.Empty,
                        Configurations = GetConfigurableProperties(type)
                    };

                    descriptors.Add(descriptor);
                }
            }
            catch (ReflectionTypeLoadException)
            {
                // Skip assemblies that cannot be loaded
                continue;
            }
        }

        return descriptors;
    }

    private static List<PipelineItemConfigurationDescriptor> GetConfigurableProperties(Type type)
    {
        var configurations = new List<PipelineItemConfigurationDescriptor>();

        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetCustomAttribute<ConfigurableAttribute>() != null);

        foreach (var property in properties)
        {
            var configurableAttribute = property.GetCustomAttribute<ConfigurableAttribute>()!;
            var propertyDescriptionAttribute = property.GetCustomAttribute<DescriptionAttribute>();

            var configuration = new PipelineItemConfigurationDescriptor
            {
                Name = property.Name,
                Type = property.PropertyType,
                EditorType = configurableAttribute.Type,
                Description = propertyDescriptionAttribute?.Description ?? string.Empty
            };

            configurations.Add(configuration);
        }

        return configurations;
    }
}
