using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kavics.LogInterpreter.Abstractions.DefaultImplementations;

public class OneLineLogFileReader : IPipelineItem<string, string>
{
    public Pipeline Pipeline { get; set; } = null!;
    public string Name => this.GetType().Name;

    public IEnumerable<string> Input { get; set; } = Array.Empty<string>();

    public IEnumerator<string> GetEnumerator()
    {
        foreach (var path in Input)
        {
            var fileInfo = new FileInfo(path);
            Pipeline.AddToCounter<int>("TotalLogFiles", 1);
            Pipeline.AddToCounter<long>("TotalLogFileSizeBytes", fileInfo.Length);

            using var textReader = new StreamReader(path);
            string? line;
            while ((line = textReader.ReadLine()) != null)
                yield return line;
        }
    }
}