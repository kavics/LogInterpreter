namespace Kavics.LogInterpreter.Abstractions.DefaultImplementations;

public class Transformer<Tin, Tout> : IPipelineItem<Tin, Tout>
{
    public Pipeline Pipeline { get; set; } = null!;
    public string Name => this.GetType().Name;
    public IEnumerable<Tin> Input { get; set; } = Array.Empty<Tin>();

    [Configurable]
    public Func<Tin, Tout> Function { get; set; } = _ => default!;

    public IEnumerator<Tout> GetEnumerator()
    {
        foreach (var entry in Input)
            yield return Function(entry);
    }
}
