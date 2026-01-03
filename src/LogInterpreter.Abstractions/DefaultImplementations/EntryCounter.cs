using System.ComponentModel;
using System.Text;

namespace Kavics.LogInterpreter.Abstractions.DefaultImplementations;

/// <summary>
/// Counts log entries by level and category and produces statistical summaries.
/// </summary>
[Description("Counts log entries by level and category and produces statistical summaries.")]
public class EntryCounter : IPipelineItem<LogEntry, LogEntry>, IAggregation
{
    public Pipeline Pipeline { get; set; } = null!;
    public string Name => this.GetType().Name;
    public IEnumerable<LogEntry> Input { get; set; } = Array.Empty<LogEntry>();

    /// <summary>
    /// Gets or sets the file path to write aggregation results to.
    /// </summary>
    [Configurable(ConfigurationType.Path)]
    [Description("Gets or sets the file path to write aggregation results to.")]
    public string? AggregationFileName { get; set; }

    public int Entries { get; private set; }
    public int NotParsedEntries { get; private set; }
    public int Informations { get; private set; }
    public int Warnings { get; private set; }
    public int Errors { get; private set; }
    public Dictionary<string, int> Categories { get; } = new Dictionary<string, int>();


    public DateTime FirstEntryTime { get; private set; } = DateTime.MinValue;
    public DateTime LastEntryTime { get; private set; } = DateTime.MinValue;

    public IEnumerator<LogEntry> GetEnumerator()
    {
        foreach (var entry in Input)
        {
            Entries++;
            if(FirstEntryTime == DateTime.MinValue)
                FirstEntryTime = entry.Time;
            LastEntryTime = entry.Time;

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

    public async Task WriteAggregation(CancellationToken cancellationToken = default)
    {
        var summary = new System.Text.StringBuilder();
        summary.AppendLine("ENTRIES SUMMARY");
        summary.AppendLine("===========================================================");
        summary.AppendLine($"Entries:           {Entries,8}");
        summary.AppendLine($"NotParsed:         {NotParsedEntries,8}");
        summary.AppendLine("LEVELS");
        summary.AppendLine($"  Informations:    {Informations,8}");
        summary.AppendLine($"  Warnings:        {Warnings,8}");
        summary.AppendLine($"  Errors:          {Errors,8}");
        summary.AppendLine("CATEGORIES");
        foreach (var category in Categories)
            summary.AppendLine($"  {category.Key,-16} {category.Value,8}");
        if(Pipeline.Counters.Count > 0)
        {
            summary.AppendLine("COUNTERS:");
            foreach (var counter in Pipeline.Counters)
            {
                if (counter.Key == "TotalLogFileSizeBytes")
                {
                    summary.AppendLine($"  {counter.Key + ":",-30} {FormatFileSize(counter.Value)}");
                }
                else
                {
                    summary.AppendLine($"  {counter.Key + ":",-30} {counter.Value}");
                }
            }
            summary.AppendLine("First entry time:  " + (FirstEntryTime == DateTime.MinValue ? "N/A" : FirstEntryTime.ToString("yyyy-MM-dd HH:mm:ss.fff")));
            summary.AppendLine("Last entry time:   " + (LastEntryTime == DateTime.MinValue ? "N/A" : LastEntryTime.ToString("yyyy-MM-dd HH:mm:ss.fff")));
        }
        summary.AppendLine($"Processing time:    {Pipeline.ProcessingTime}.");
        summary.AppendLine($"Velocity:           {(Entries + NotParsedEntries) / Pipeline.ProcessingTime.TotalSeconds:0} EPS (Entry per second).");
        summary.Append("===========================================================");

        var console = Pipeline.GetConsole();
        console.WriteLine(summary.ToString());

        if (AggregationFileName != null)
        {
            using var writer = new StreamWriter(AggregationFileName, Encoding.UTF8, new FileStreamOptions
            {
                Access = FileAccess.Write,
                Mode = FileMode.OpenOrCreate
            });

            console.Write("Writing Analysis-summary file...");
            await writer.WriteAsync(summary, cancellationToken);
            console.WriteLine("ok.");
        }
        console.WriteLine();
    }

    private object FormatFileSize(object value)
    {
        if (value is not long sizeInBytes)
            return value;
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = sizeInBytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
}
