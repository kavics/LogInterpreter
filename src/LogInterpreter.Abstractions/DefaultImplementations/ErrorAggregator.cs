using System.ComponentModel;
using System.Text;

namespace Kavics.LogInterpreter.Abstractions.DefaultImplementations;

/// <summary>
/// Collects and aggregates error, warning, and critical log entries.
/// </summary>
[Description("Collects and aggregates error, warning, and critical log entries.")]
public class ErrorAggregator : IPipelineItem<LogEntry, LogEntry>, IAggregation
{
    public Pipeline Pipeline { get; set; } = null!;
    public string Name => this.GetType().Name;
    public IEnumerable<LogEntry> Input { get; set; } = Array.Empty<LogEntry>();

    /// <summary>
    /// Gets or sets the file path to write error aggregation results to.
    /// </summary>
    [Configurable(ConfigurationType.Path)]
    [Description("Gets or sets the file path to write error aggregation results to.")]
    public string? AggregationFileName { get; set; }

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

    public Task WriteAggregation(CancellationToken cancellationToken = default)
    {
        var console = Pipeline.GetConsole();
        WriteAggregationSummary(console);

        if (AggregationFileName != null)
        {
            console.Write("Writing detailed error information to file...");
            WriteToFile(AggregationFileName);
            console.WriteLine("ok.");
        }
        else
        {
            console.WriteLine("WARNING: No aggregation file name provided, writing detailed error information skipped.");
        }
        console.WriteLine();
        return Task.CompletedTask;
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

        WriteAggregationSummary(writer);
        writer.WriteLine("========================================================================");

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
    void WriteAggregationSummary(TextWriter writer)
    {
        // Get counts by level
        var criticalCount = Criticals.Sum(x => x.Value.Count);
        var errorCount = Errors.Sum(x => x.Value.Count);
        var warningCount = Warnings.Sum(x => x.Value.Count);
        // Write summary to console
        writer.WriteLine("ERROR AGGREGATION SUMMARY");
        writer.WriteLine("-------------------------");
        writer.WriteLine($"CRITICAL ERRORS: {criticalCount}");
        writer.WriteLine($"ERRORS:          {errorCount}");
        writer.WriteLine($"WARNINGS:        {warningCount}");
        writer.WriteLine("-------------------------");
    }

}
