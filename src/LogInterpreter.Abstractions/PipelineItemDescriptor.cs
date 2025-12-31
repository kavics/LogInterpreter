using System.Diagnostics;

namespace Kavics.LogInterpreter.Abstractions
{
    [DebuggerDisplay("PipelineItem: {Name}: {Input.Name} -> {Output.Name}")]
    public class PipelineItemDescriptor
    {
        public string Name { get; set; } = null!;
        public Type Type { get; set; } = null!;
        public Type Input { get; set; } = null!;
        public Type Output { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        public List<PipelineItemConfigurationDescriptor> Configurations { get; set; } = new();
    }
}
