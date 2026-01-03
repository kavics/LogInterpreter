using System.ComponentModel;
using Kavics.LogInterpreter.Abstractions;

namespace LogInterpreter.CLI.Customizations;

/// <summary>
/// Reads the first N lines from each file in the pipeline.
/// </summary>
[Description("Reads the first N lines from each file in the pipeline.")]
internal class FirstLineReader : IPipelineItem<string, string>
{
    public Pipeline Pipeline { get; set; } = null!;
    public string Name => this.GetType().Name;

    /// <summary>
    /// Gets or sets the number of lines to read from each file. Defaults to 1 if not specified.
    /// </summary>
    [Configurable]
    [Description("Number of lines to read from each file. Defaults to 1 if not specified.")]
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
