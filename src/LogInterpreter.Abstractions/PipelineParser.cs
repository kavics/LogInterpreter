using System.Reflection;

namespace Kavics.LogInterpreter.Abstractions;

internal class PipelineParser
{
    public Pipeline Parse(string definition)
    {
        if (string.IsNullOrWhiteSpace(definition))
            return new Pipeline();

        var pipeline = new Pipeline();
        var lines = definition.Split(new[] { Environment.NewLine, "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

        PipelineItemDescriptor? currentDescriptor = null;
        object? currentItem = null;
        var pendingConfigurations = new Dictionary<string, string>();

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            
            // Check if this is a configuration line (starts with spaces)
            if (line.StartsWith("    ") || line.StartsWith("\t"))
            {
                // Parse configuration: "PropertyName=Value"
                var parts = trimmedLine.Split('=', 2);
                if (parts.Length == 2)
                {
                    pendingConfigurations[parts[0].Trim()] = parts[1].Trim();
                }
            }
            else
            {
                // This is a new pipeline item
                // First, apply any pending configurations to the previous item
                if (currentItem != null && currentDescriptor != null)
                {
                    ApplyConfigurations(currentItem, currentDescriptor, pendingConfigurations);
                    AddItemToPipeline(pipeline, currentItem);
                }

                // Reset for the new item
                pendingConfigurations.Clear();
                currentDescriptor = Pipeline.AvailableItems.FirstOrDefault(d => d.Name == trimmedLine);

                if (currentDescriptor != null)
                {
                    // Create instance of the pipeline item
                    currentItem = Activator.CreateInstance(currentDescriptor.Type);
                }
                else
                {
                    currentItem = null;
                }
            }
        }

        // Apply configurations to the last item
        if (currentItem != null && currentDescriptor != null)
        {
            ApplyConfigurations(currentItem, currentDescriptor, pendingConfigurations);
            AddItemToPipeline(pipeline, currentItem);
        }

        return pipeline;
    }

    private void ApplyConfigurations(object item, PipelineItemDescriptor descriptor, Dictionary<string, string> configurations)
    {
        foreach (var config in configurations)
        {
            var configDescriptor = descriptor.Configurations.FirstOrDefault(c => c.Name == config.Key);
            if (configDescriptor == null)
                continue;

            var property = item.GetType().GetProperty(config.Key);
            if (property == null)
                continue;

            try
            {
                var convertedValue = ConvertValue(config.Value, property.PropertyType);
                property.SetValue(item, convertedValue);
            }
            catch
            {
                // Ignore conversion errors
            }
        }
    }

    private object? ConvertValue(string value, Type targetType)
    {
        if (targetType == typeof(string))
            return value;

        if (targetType == typeof(int))
            return int.Parse(value);

        if (targetType == typeof(long))
            return long.Parse(value);

        if (targetType == typeof(bool))
            return bool.Parse(value);

        if (targetType == typeof(double))
            return double.Parse(value);

        if (targetType == typeof(decimal))
            return decimal.Parse(value);

        if (targetType == typeof(DateTime))
            return DateTime.Parse(value);

        if (targetType == typeof(TimeSpan))
            return TimeSpan.Parse(value);

        if (targetType.IsEnum)
            return Enum.Parse(targetType, value);

        // Try generic conversion
        return Convert.ChangeType(value, targetType);
    }

    private void AddItemToPipeline(Pipeline pipeline, object item)
    {
        // Use reflection to call the generic AddItem method
        var itemType = item.GetType();
        var interfaces = itemType.GetInterfaces()
            .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPipelineItem<,>))
            .ToList();

        if (interfaces.Count == 0)
            return;

        var pipelineItemInterface = interfaces[0];
        var genericArgs = pipelineItemInterface.GetGenericArguments();

        var addItemMethod = typeof(Pipeline)
            .GetMethod("AddItem")
            ?.MakeGenericMethod(genericArgs);

        addItemMethod?.Invoke(pipeline, new[] { item });
    }
}
