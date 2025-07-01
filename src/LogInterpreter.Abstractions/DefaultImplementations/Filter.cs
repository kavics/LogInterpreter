namespace LogInterpreter.Abstractions.DefaultImplementations;

public class Filter<T>(Func<T, bool> filter) : IPipelineItem<T, T> //where T : ILogEntry
{
    public IEnumerable<T> Input { get; set; } = Array.Empty<T>();

    public IEnumerator<T> GetEnumerator()
    {
        foreach (var logEntry in Input)
            if (filter(logEntry))
                yield return logEntry;
    }
}