using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Kavics.LogInterpreter.Abstractions.DefaultImplementations;

/// <summary>
/// Reads log files where each entry consists of two lines: header and JSON properties.
/// This class is obsolete. Use <see cref="TwoLineLogReader"/> instead.
/// </summary>
[Description("Reads log files where each entry consists of two lines: header and JSON properties. (Obsolete)")]
[Obsolete("Use TwoLineLogReader instead.")]
public class TwoLineLogFileReader : IPipelineItem<string, string[]>
{
    public Pipeline Pipeline { get; set; } = null!;
    public string Name => this.GetType().Name;
    public IEnumerable<string> Input { get; set; } = Array.Empty<string>();

    public IEnumerator<string[]> GetEnumerator()
    {
        foreach (var path in Input)
        {
            var fileInfo = new FileInfo(path);
            Pipeline.AddToCounter<int>("TotalLogFiles", 1);
            Pipeline.AddToCounter<long>("TotalLogFileSizeBytes", fileInfo.Length);

            using var textReader = new StreamReader(path);
            string? line;
            string? currentHeader = null;
            List<string> propertyLines = new();
            Regex headerRegex = new(@"^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3}");

            while ((line = textReader.ReadLine()) != null)
            {
                if (headerRegex.IsMatch(line))
                {
                    if (currentHeader != null)
                    {
                        // Ez egy új logbejegyzés, a régit lezárjuk
                        string[] output = ArrangeLines(currentHeader, propertyLines);
                        yield return output;
                    }

                    currentHeader = line;
                    propertyLines.Clear();
                }
                else
                {
                    propertyLines.Add(line);
                }
            }

            // Az utolsó bejegyzés hozzáadása
            if (currentHeader != null)
            {
                string[] output = ArrangeLines(currentHeader, propertyLines);
                yield return output;
            }
        }
    }

    private string[] ArrangeLines(string headerLine, List<string> propertyLines)
    {
        if (propertyLines.Count == 0)
            return [headerLine, "{}"];
        if (propertyLines.Count == 1)
            return [headerLine, propertyLines[0]];

        var jsonLine = propertyLines.FirstOrDefault(l => l.StartsWith("{"));
        if ((jsonLine == null))
            jsonLine = "{}";

        var newHeader = new List<string> { headerLine };
        newHeader.AddRange(propertyLines.Where(l => l != jsonLine));

        return [string.Join(Environment.NewLine, newHeader), jsonLine];
    }
}