namespace Kavics.LogInterpreter.Abstractions
{
    public class PipelineItemConfigurationDescriptor
    {
        public string Name { get; set; } = null!;
        public Type Type { get; set; } = null!;
        public ConfigurationType EditorType { get; set; } = default;
        public string Description { get; set; } = string.Empty;
    }
}