namespace Kavics.LogInterpreter.Abstractions.DefaultImplementations;

public class Filter<T> : IPipelineItem<T, T> //where T : ILogEntry
{
    public Pipeline Pipeline { get; set; } = null!;
    public string Name => this.GetType().Name;
    public IEnumerable<T> Input { get; set; } = Array.Empty<T>();

    [Configurable]
    public Func<T, bool> Function { get; set; } = _ => true;


    public IEnumerator<T> GetEnumerator()
    {
        foreach (var logEntry in Input)
            if (Function(logEntry))
                yield return logEntry;
    }
}