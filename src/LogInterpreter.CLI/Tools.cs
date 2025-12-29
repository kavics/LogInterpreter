using Kavics.LogInterpreter.Abstractions;
using Kavics.LogInterpreter.Abstractions.DefaultImplementations;

namespace LogInterpreter.CLI
{
    internal class Tools
    {
        public void Rewrite(string source, string target)
        {
            new Pipeline()
                .AddItem(new LogSource(source))
                .AddItem(new OneLineLogFileReader())
                .AddItem(new CompactJsonLogParser())
                .AddItem(new Formatter<LogEntry>(entry => entry.LineId > 0
                    ? $"{entry.Time.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fff} [{entry.LineId}] {entry.Category}\t{entry.Status}\t {entry.Message}"
                    : $"{entry.Time.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fff} {ReplaceTemplates(entry.Message, entry.Properties)}"))
                .AddItem(new FileWriter(target))
                .Run();
        }

        public string ReplaceTemplates(string message, IDictionary<string, string> properties)
        {
            if (string.IsNullOrEmpty(message)) return message;
            return System.Text.RegularExpressions.Regex.Replace(message, "\\{([^}]+)\\}", match =>
            {
                var key = match.Groups[1].Value;
                if (properties != null && properties.TryGetValue(key, out var value))
                    return value;
                return "_unrecognized_";
            });
        }
    }
}
