namespace LogInterpreter.Abstractions.DefaultImplementations
{
    public class Transformer<Tin, Tout>(Func<Tin, Tout> transformer) : IPipelineItem<Tin, Tout>
    {
        public IEnumerable<Tin> Input { get; set; } = Array.Empty<Tin>();
        public IEnumerator<Tout> GetEnumerator()
        {
            foreach (var entry in Input)
                yield return transformer(entry);
        }
    }
}
