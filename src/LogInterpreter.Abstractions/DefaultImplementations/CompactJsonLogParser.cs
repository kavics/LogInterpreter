
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace Kavics.LogInterpreter.Abstractions.DefaultImplementations;

public class CompactJsonLogParser : IPipelineItem<string, LogEntry>
{
    public Pipeline Pipeline { get; set; } = null!;
    public string Name => this.GetType().Name;

    public IEnumerable<string> Input { get; set; } = Array.Empty<string>();

    public IEnumerator<LogEntry> GetEnumerator()
    {
        foreach (var line in Input)
        {
            var logEntry = Parse(line);
            if(logEntry != null)
                yield return logEntry;
        }
    }

    private LogEntry? Parse(string line)
    {
        try
        {
            var json = JObject.Parse(line);

            var entry = new LogEntry
            {
                Raw = [line],
                Time = json["@t"]?.Value<DateTime?>() ?? DateTime.MinValue,
                Message = json["@mt"]?.ToString() ?? "",
                Level = ParseLogLevel(json["@l"]?.ToString())
            };

            foreach (var prop in json)
            {
                var key = prop.Key;
                if (key is "@t" or "@mt" or "@l")
                    continue;
                entry.Properties[key] = prop.Value?.ToString() ?? "";
            }

            if (Char.IsDigit(entry.Message[0]))
            {
                TwoLineLogParser.ParseSnTraceEntry(entry, entry.Message);
            }

            return entry;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hibás log sor: {ex.Message}");
            return null;
        }

    }
    
    LogLevel ParseLogLevel(string? raw)
    {
        if(raw != null)
        {
            if (Enum.TryParse<LogLevel>(raw, true, out var level))
                return level;
            if (string.Compare(raw, "Verbose") == 0)
                return LogLevel.Trace;
        }
        return LogLevel.Information;
    }
}
