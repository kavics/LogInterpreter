namespace Kavics.LogInterpreter.Abstractions;

public enum ConfigurationType { Default, Path }

[AttributeUsage(AttributeTargets.Property)]
public class ConfigurableAttribute : Attribute
{
    public ConfigurationType Type { get; }

    public ConfigurableAttribute()
    {
        Type = ConfigurationType.Default;
    }

    public ConfigurableAttribute(ConfigurationType type)
    {
        Type = type;
    }
}
