namespace LogInterpreter.Abstractions.DefaultImplementations;

public class Formatter<T>(Func<T, string> formatter) : IPipelineItem<T, string> where T : ILogEntry
{
    public string Name => this.GetType().Name;

    public IEnumerable<T> Input { get; set; } = Array.Empty<T>();

    public IEnumerator<string> GetEnumerator()
    {
        foreach (var logEntry in Input)
            yield return formatter(logEntry);
    }
}
