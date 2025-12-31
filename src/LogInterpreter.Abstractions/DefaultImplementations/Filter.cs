namespace Kavics.LogInterpreter.Abstractions.DefaultImplementations;

public class CategoryFilter : Filter<ILogEntry>
{
    public HashSet<string> IncludeCategoryNames = new();
    public HashSet<string> ExcludeCategoryNames = new();

    [Configurable]
    public string Include
    {
        get => string.Join(",", IncludeCategoryNames);
        set
        {
            var parts = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            IncludeCategoryNames = new HashSet<string>(parts);
        }
    }

    [Configurable]
    public string Exclude
    {
        get => string.Join(",", ExcludeCategoryNames);
        set
        {
            var parts = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            ExcludeCategoryNames = new HashSet<string>(parts);
        }
    }

    public CategoryFilter()
    {
        this.Function = entry =>
        {
            // If there is an include list, only that matters
            if (IncludeCategoryNames.Count > 0)
                return IncludeCategoryNames.Contains(entry.Category);
            
            // If there is no include list, then the exclude list applies
            if (ExcludeCategoryNames.Count > 0)
                return !ExcludeCategoryNames.Contains(entry.Category);
            
            // If neither is specified, everything passes through
            return true;
        };
    }
}


public class Filter<T> : IPipelineItem<T, T> //where T : ILogEntry
{
    public Pipeline Pipeline { get; set; } = null!;
    public string Name => this.GetType().Name;
    public IEnumerable<T> Input { get; set; } = Array.Empty<T>();

    public Func<T, bool> Function { get; set; } = _ => true;

    public virtual IEnumerator<T> GetEnumerator()
    {
        foreach (var logEntry in Input)
            if (Function(logEntry))
                yield return logEntry;
    }
}