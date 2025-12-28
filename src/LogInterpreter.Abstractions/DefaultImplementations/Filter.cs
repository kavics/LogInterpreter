namespace Kavics.LogInterpreter.Abstractions.DefaultImplementations;

public class Filter<T> : IPipelineItem<T, T> //where T : ILogEntry
{
    public Pipeline Pipeline { get; set; } = null!;
    public string Name => this.GetType().Name;

    [Configurable]
    public Func<T, bool> FilterFunction { get; set; }

    public Filter(Func<T, bool> filter)
    {
        this.FilterFunction = filter;
    }


    public IEnumerable<T> Input { get; set; } = Array.Empty<T>();

    public IEnumerator<T> GetEnumerator()
    {
        foreach (var logEntry in Input)
            if (FilterFunction(logEntry))
                yield return logEntry;
    }
}