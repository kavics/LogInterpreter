using System.ComponentModel;

namespace Kavics.LogInterpreter.Abstractions.DefaultImplementations;

/// <summary>
/// Filters log entries by category name using include/exclude lists.
/// </summary>
[Description("Filters log entries by category name using include/exclude lists.")]
public class CategoryFilter : Filter<ILogEntry>
{
    public HashSet<string> IncludeCategoryNames = new();
    public HashSet<string> ExcludeCategoryNames = new();

    /// <summary>
    /// Comma-separated list of category names to include (only these will pass).
    /// </summary>
    [Configurable]
    [Description("Comma-separated list of category names to include (only these will pass).")]
    public string Include
    {
        get => string.Join(",", IncludeCategoryNames);
        set
        {
            var parts = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            IncludeCategoryNames = new HashSet<string>(parts);
        }
    }

    /// <summary>
    /// Comma-separated list of category names to exclude (all others will pass).
    /// </summary>
    [Configurable]
    [Description("Comma-separated list of category names to exclude (all others will pass).")]
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

/// <summary>
/// Filters entries based on a custom predicate function.
/// </summary>
/// <typeparam name="T">The type of entries to filter.</typeparam>
[Description("Filters entries based on a custom predicate function.")]
public class Filter<T> : IPipelineItem<T, T> //where T : ILogEntry
{
    public Pipeline Pipeline { get; set; } = null!;
    public string Name => this.GetType().Name;
    public IEnumerable<T> Input { get; set; } = Array.Empty<T>();

    /// <summary>
    /// Gets or sets the predicate function that determines if an entry should pass through.
    /// </summary>
    [Configurable]
    [Description("The predicate function that determines if an entry should pass through.")]
    public Func<T, bool> Function { get; set; } = _ => true;

    public virtual IEnumerator<T> GetEnumerator()
    {
        foreach (var logEntry in Input)
            if (Function(logEntry))
                yield return logEntry;
    }
}