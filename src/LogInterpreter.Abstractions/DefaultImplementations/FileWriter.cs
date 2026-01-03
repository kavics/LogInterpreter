using System.ComponentModel;
using System.Text;

namespace Kavics.LogInterpreter.Abstractions.DefaultImplementations;

/// <summary>
/// Writes log entries to a text file.
/// </summary>
[Description("Writes log entries to a text file.")]
public class FileWriter : IPipelineItem<string, string>
{
    public Pipeline Pipeline { get; set; } = null!;
    public string Name => this.GetType().Name;
    public IEnumerable<string> Input { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Gets or sets the full path to the output file.
    /// </summary>
    [Configurable(ConfigurationType.Path)]
    [Description("The full path to the output file.")]
    public string FilePath { get; set; }

    public IEnumerator<string> GetEnumerator()
    {
        var directory = Path.GetDirectoryName(this.FilePath);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        using var fileStream = new System.IO.FileStream(FilePath, FileMode.Create, FileAccess.Write);
        using var writer = new StreamWriter(fileStream, Encoding.UTF8);
        foreach (var line in Input)
        {
            writer.WriteLine(line);
            yield return line;
        }
    }
}
