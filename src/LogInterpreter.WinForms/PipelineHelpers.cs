using LogInterpreter.Abstractions;
using LogInterpreter.Abstractions.DefaultImplementations;

namespace LogInterpreter.WinForms;

/// <summary>
/// Pipeline elem, amely számolja a különbözõ log szintû bejegyzéseket és kategóriákat
/// </summary>
internal class Counter : IPipelineItem<LogEntry, LogEntry>
{
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

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

/// <summary>
/// Pipeline elem, amely aggregálja a hibákat és figyelmeztetéseket
/// </summary>
internal class ErrorAggregator : IPipelineItem<LogEntry, LogEntry>
{
    public IEnumerable<LogEntry> Input { get; set; } = Array.Empty<LogEntry>();

    public Dictionary<string, List<DateTime>> Criticals { get; } = new();
    public Dictionary<string, List<DateTime>> Errors { get; } = new();
    public Dictionary<string, List<DateTime>> Warnings { get; } = new();

    public IEnumerator<LogEntry> GetEnumerator()
    {
        foreach (var entry in Input)
        {
            Dictionary<string, List<DateTime>>? target = null;
            switch (entry.Level)
            {
                case LogLevel.Critical: target = Criticals; break;
                case LogLevel.Error: target = Errors; break;
                case LogLevel.Warning: target = Warnings; break;
            }

            if (target != null)
            {
                if (!target.TryGetValue(entry.Message, out var times))
                {
                    times = new List<DateTime>();
                    target.Add(entry.Message, times);
                }
                times.Add(entry.Time);
            }

            yield return entry;
        }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
