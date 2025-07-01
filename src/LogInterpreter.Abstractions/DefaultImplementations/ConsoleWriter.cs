namespace LogInterpreter.Abstractions.DefaultImplementations
{
    public class ConsoleWriter : IPipelineItem<string, string>
    {
        public IEnumerable<string> Input { get; set; } = Array.Empty<string>();
        public IEnumerator<string> GetEnumerator()
        {
            foreach (var line in Input)
            {
                Console.WriteLine(line);
                yield return line;
            }
        }
    }
}
