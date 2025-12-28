using Kavics.LogInterpreter.Abstractions;
using Kavics.LogInterpreter.Abstractions.DefaultImplementations;

namespace LogInterpreter.CLI.Customizations;

internal class FirstLineReader : IPipelineItem<string, string>
{
    public Pipeline Pipeline { get; set; } = null!;
    public string Name => this.GetType().Name;

    [Configurable]
    public int? Count { get; set; }

    public FirstLineReader(int? count = null)
    {
        Count = count;
    }

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
