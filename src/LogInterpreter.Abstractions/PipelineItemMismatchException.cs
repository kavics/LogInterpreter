namespace Kavics.LogInterpreter.Abstractions;

public class PipelineItemMismatchException : Exception
{
    public PipelineItemMismatchException(string message) : base(message) { }
}
