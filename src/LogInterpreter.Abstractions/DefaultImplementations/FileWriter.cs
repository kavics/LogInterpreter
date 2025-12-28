using System.Text;

namespace Kavics.LogInterpreter.Abstractions.DefaultImplementations;

public class FileWriter : IPipelineItem<string, string>
{
    public Pipeline Pipeline { get; set; } = null!;
    public string Name => this.GetType().Name;

    [Configurable(ConfigurationType.Path)]
    public string FilePath { get; set; }

    public FileWriter(string path)
    {
        FilePath = path;
    }


    public IEnumerable<string> Input { get; set; } = Array.Empty<string>();

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
