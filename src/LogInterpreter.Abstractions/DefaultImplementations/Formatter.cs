namespace Kavics.LogInterpreter.Abstractions.DefaultImplementations;

public class Formatter<T> : IPipelineItem<T, string> where T : ILogEntry
{
    [Configurable]
    public Func<T, string> FormatterFunction { get; set; }

    public Formatter(Func<T, string> formatter)
    {
        this.FormatterFunction = formatter;
    }

    public string Name => this.GetType().Name;

    public IEnumerable<T> Input { get; set; } = Array.Empty<T>();

    public IEnumerator<string> GetEnumerator()
    {
        foreach (var logEntry in Input)
            yield return FormatterFunction(logEntry);
    }
}
