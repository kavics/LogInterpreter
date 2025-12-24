using Kavics.LogInterpreter.Abstractions;

namespace LogInterpreter.CLI.Customizations;

internal class FirstLineReader : IPipelineItem<string, string>
{
    [Configurable]
    public int? Count { get; set; }

    public FirstLineReader(int? count = null)
    {
        Count = count;
    }

    public string Name => this.GetType().Name;

    public IEnumerable<string> Input { get; set; } = Array.Empty<string>();

    public IEnumerator<string> GetEnumerator()
    {
        foreach (var path in Input)
        {
            using var textReader = new StreamReader(path);
            for (int i = 0; i < (Count ?? 1); i++)
            {
                var line = textReader.ReadLine();
                if (line != null)
                    yield return line;
            }
        }
    }
}
