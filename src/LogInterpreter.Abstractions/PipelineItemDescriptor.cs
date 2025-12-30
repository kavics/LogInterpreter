using System.Diagnostics;

namespace Kavics.LogInterpreter.Abstractions
{
    [DebuggerDisplay("PipelineItem: {Name}")]
    public class PipelineItemDescriptor
    {
        public string Name { get; set; } = null!;
        public Type Type { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        public List<PipelineItemConfigurationDescriptor> Configurations { get; set; } = new();
    }
}
