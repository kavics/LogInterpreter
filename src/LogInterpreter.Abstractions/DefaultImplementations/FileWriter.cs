using System.Text;

namespace LogInterpreter.Abstractions.DefaultImplementations
{
    public class FileWriter : IPipelineItem<string, string>
    {
        [Configurable(ConfigurationType.Path)]
        public string FilePath { get; set; }

        public FileWriter(string path)
        {
            FilePath = path;
        }

        public string Name => this.GetType().Name;

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
}
