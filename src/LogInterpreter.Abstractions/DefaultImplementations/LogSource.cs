namespace LogInterpreter.Abstractions.DefaultImplementations;

public class LogSource(string path) : IPipelineItem<int, string>
{
    public IEnumerable<int> Input { get; set; } = Array.Empty<int>();
    public IEnumerator<string> GetEnumerator()
    {
        if(Directory.Exists(path))
        {
            foreach (var file in Directory.GetFiles(path))
                yield return file;
        }
        else if(File.Exists(path))
        {
            yield return path;
        }
        else
        {
            throw new FileNotFoundException($"The specified path '{path}' does not exist or is not a directory/file.");
        }
    }
}
