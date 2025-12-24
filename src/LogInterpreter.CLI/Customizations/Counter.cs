using Kavics.LogInterpreter.Abstractions;
using Kavics.LogInterpreter.Abstractions.DefaultImplementations;

namespace LogInterpreter.CLI.Customizations;

internal class Counter : IPipelineItem<LogEntry, LogEntry>
{
    public string Name => this.GetType().Name;

    public int Entries { get; private set; }
    public int NotParsedEntries { get; private set; }
    public int Informations { get; private set; }
    public int Warnings { get; private set; }
    public int Errors { get; private set; }
    public Dictionary<string, int> Categories { get; } = new Dictionary<string, int>();

    public IEnumerable<LogEntry> Input { get; set; } = Array.Empty<LogEntry>();

    public IEnumerator<LogEntry> GetEnumerator()
    {
        foreach (var entry in Input)
        {
            Entries++;
            switch (entry.Level)
            {
                case LogLevel.Information: Informations++; break;
                case LogLevel.Warning: Warnings++; break;
                case LogLevel.Error: Errors++; break;
                case LogLevel.Critical: Errors++; break;
                case LogLevel.NotParsed: NotParsedEntries++; break;
            }

            if (entry.Category != null)
            {
                if (!Categories.ContainsKey(entry.Category))
                    Categories[entry.Category] = 0;
                Categories[entry.Category]++;
            }

            yield return entry;
        }
    }
}
