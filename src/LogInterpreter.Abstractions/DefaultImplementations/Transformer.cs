using System.ComponentModel;

namespace Kavics.LogInterpreter.Abstractions.DefaultImplementations;

/// <summary>
/// Transforms entries from one type to another using a custom function.
/// </summary>
/// <typeparam name="Tin">Input entry type.</typeparam>
/// <typeparam name="Tout">Output entry type.</typeparam>
[Description("Transforms entries from one type to another using a custom function.")]
public class Transformer<Tin, Tout> : IPipelineItem<Tin, Tout>
{
    public Pipeline Pipeline { get; set; } = null!;
    public string Name => this.GetType().Name;
    public IEnumerable<Tin> Input { get; set; } = Array.Empty<Tin>();

    /// <summary>
    /// Gets or sets the transformation function to apply to each entry.
    /// </summary>
    [Configurable]
    [Description("The transformation function to apply to each entry.")]
    public Func<Tin, Tout> Function { get; set; } = _ => default!;

    public IEnumerator<Tout> GetEnumerator()
    {
        foreach (var entry in Input)
            yield return Function(entry);
    }
}
