using System.ComponentModel;

namespace Kavics.LogInterpreter.Abstractions.DefaultImplementations;

/// <summary>
/// Formats log entries into string representation using a custom function.
/// </summary>
/// <typeparam name="T">The type of log entries to format.</typeparam>
[Description("Formats log entries into string representation using a custom function.")]
public class Formatter<T> : IPipelineItem<T, string> where T : ILogEntry
{
    /// <summary>
    /// Gets the name of the formatter.
    /// </summary>
    public string Name => this.GetType().Name;

    /// <summary>
    /// Gets or sets the pipeline associated with the formatter.
    /// </summary>
    public Pipeline Pipeline { get; set; } = null!;

    /// <summary>
    /// Gets or sets the input log entries to be formatted.
    /// </summary>
    public IEnumerable<T> Input { get; set; } = Array.Empty<T>();

    /// <summary>
    /// Gets or sets the function that converts a log entry to a string.
    /// </summary>
    [Configurable]
    [Description("The function that converts a log entry to a string.")]
    public Func<T, string> Function { get; set; } = _ => string.Empty;

    /// <summary>
    /// Converts the input log entries to their string representations.
    /// </summary>
    /// <returns>
    /// An enumerator that allows foreach-ing over the formatted log entries.
    /// </returns>
    public IEnumerator<string> GetEnumerator()
    {
        foreach (var logEntry in Input)
            yield return Function(logEntry);
    }
}
