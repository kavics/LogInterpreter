using System.Text;

namespace LogInterpreter.Abstractions.DefaultImplementations
{
    public class FileWriter(string path) : IPipelineItem<string, string>
    {
        public string Name => this.GetType().Name;

        public IEnumerable<string> Input { get; set; } = Array.Empty<string>();

        public IEnumerator<string> GetEnumerator()
        {
            using var fileStream = new System.IO.FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
            using var writer = new StreamWriter(fileStream, Encoding.UTF8);
            foreach (var line in Input)
            {
                writer.WriteLine(line);
                yield return line;
            }
        }
    }
}
