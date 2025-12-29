namespace Kavics.LogInterpreter.Abstractions.DefaultImplementations;

public class Formatter<T> : IPipelineItem<T, string> where T : ILogEntry
{
    public string Name => this.GetType().Name;
    public Pipeline Pipeline { get; set; } = null!;
    public IEnumerable<T> Input { get; set; } = Array.Empty<T>();

    [Configurable]
    public Func<T, string> Function { get; set; } = _ => string.Empty;

    public IEnumerator<string> GetEnumerator()
    {
        foreach (var logEntry in Input)
            yield return Function(logEntry);
    }
}
