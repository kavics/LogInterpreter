namespace LogInterpreter.Abstractions.DefaultImplementations
{
    public class Transformer<Tin, Tout> : IPipelineItem<Tin, Tout>
    {
        [Configurable]
        public Func<Tin, Tout> TransformerFunction { get; set; }

        public Transformer(Func<Tin, Tout> transformer)
        {
            TransformerFunction = transformer;
        }

        public string Name => this.GetType().Name;

        public IEnumerable<Tin> Input { get; set; } = Array.Empty<Tin>();
        public IEnumerator<Tout> GetEnumerator()
        {
            foreach (var entry in Input)
                yield return TransformerFunction(entry);
        }
    }
}
