using LogInterpreter.Abstractions;
using LogInterpreter.Abstractions.DefaultImplementations;

namespace LogInterpreter.WinForms;

/// <summary>
/// Pipeline elem, amely összegyûjti a LogEntry objektumokat egy listába
/// </summary>
internal class LogEntryCollector : IPipelineItem<LogEntry, LogEntry>
{
    public List<ILogEntry> CollectedEntries { get; } = new List<ILogEntry>();
    
    public IEnumerable<LogEntry> Input { get; set; } = Array.Empty<LogEntry>();

    public IEnumerator<LogEntry> GetEnumerator()
    {
        foreach (var entry in Input)
        {
            CollectedEntries.Add(entry);
            yield return entry;
        }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Clear()
    {
        CollectedEntries.Clear();
    }
}
