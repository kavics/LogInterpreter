using Kavics.LogInterpreter.Abstractions;
using Kavics.LogInterpreter.Abstractions.DefaultImplementations;
using System.Text;

namespace LogInterpreter.CLI.Customizations;

internal class ErrorAggregator : IPipelineItem<LogEntry, LogEntry>
{
    public string Name => this.GetType().Name;

    public IEnumerable<LogEntry> Input { get; set; } = Array.Empty<LogEntry>();

    public Dictionary<string, List<DateTime>> Criticals = new();
    public Dictionary<string, List<DateTime>> Errors = new();
    public Dictionary<string, List<DateTime>> Warnings = new();

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

    public void WriteToFile(string filePath, bool withTimes = false)
    {
        using var writer = new StreamWriter(filePath, Encoding.UTF8, new FileStreamOptions
        {
            Access = FileAccess.Write,
            Mode = FileMode.OpenOrCreate
        });

        void PrintMore(Dictionary<string, List<DateTime>> source)
        {
            foreach (var item in source
                         .Where(x => x.Value.Count > 1)
                         .OrderByDescending(x => x.Value.Count))
                writer.WriteLine($"{item.Key} ({item.Value.Count} items)");
        }

        writer.WriteLine("More than one items");
        writer.WriteLine("-------------------");
        writer.WriteLine();
        writer.WriteLine("CRITICAL ERRORS (more than one items)");
        PrintMore(Criticals);
        writer.WriteLine();
        writer.WriteLine("ERRORS (more than one items)");
        PrintMore(Errors);
        writer.WriteLine();
        writer.WriteLine("WARNINGS (more than one items)");
        PrintMore(Warnings);
        writer.WriteLine("========================================================================");

        void PrintOne(Dictionary<string, List<DateTime>> source)
        {
            foreach (var item in source
                         .Where(x => x.Value.Count == 1)
                         .OrderByDescending(x => x.Key))
            {
                writer.WriteLine($"{item.Value.Single().ToUniversalTime():yyyy-MM-dd HH:mm:ss.fff} {item.Key}");
            }
        }

        writer.WriteLine();
        writer.WriteLine("Only one items");
        writer.WriteLine("--------------");
        writer.WriteLine();
        writer.WriteLine("CRITICAL ERRORS");
        PrintOne(Criticals);
        writer.WriteLine();
        writer.WriteLine("ERRORS");
        PrintOne(Errors);
        writer.WriteLine();
        writer.WriteLine("WARNINGS");
        PrintOne(Warnings);
    }
}
