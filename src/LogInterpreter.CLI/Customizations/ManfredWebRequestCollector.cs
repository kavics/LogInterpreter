using Kavics.LogInterpreter.Abstractions;
using Kavics.LogInterpreter.Abstractions.DefaultImplementations;

namespace LogInterpreter.CLI.Customizations;

internal class ManfredWebRequestCollector : IPipelineItem<LogEntry, LogEntry>
{
    public string Name => this.GetType().Name;

    public IEnumerable<LogEntry> Input { get; set; } = Array.Empty<LogEntry>();

    private Dictionary<string, (int Count, DateTime FirstTime, DateTime LastTime)> _requestEntries = new();
    public Dictionary<string, int> StatusCodes = new();
    public int RequestCount { get; private set; }
    public double AverageTime { get; private set; }
    public int LongCount { get; private set; }
    public int VeryLongCount { get; private set; }
    public double LongestTimeSec { get; private set; }
    public string LongestRequestId { get; private set; }

    public IEnumerator<LogEntry> GetEnumerator()
    {
        foreach (var entry in Input)
        {
            if (entry.Message == "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms")
            {
                var valueRaw = entry.Properties["Elapsed"];
                var value = Convert.ToDouble(valueRaw);
                var valueSec = value / 1000.0;

                if (value > 4.0)
                    LongCount++;
                if (value > 10.0)
                    VeryLongCount++;

                if (valueSec > LongestTimeSec)
                {
                    LongestTimeSec = valueSec;
                    entry.Properties.TryGetValue("RequestId", out var requestId);
                    LongestRequestId = requestId ?? "unknown";
                }

                // Increment status code counter
                if (entry.Properties.TryGetValue("StatusCode", out var statusCode))
                {
                    if (!StatusCodes.ContainsKey(statusCode))
                        StatusCodes[statusCode] = 0;
                    StatusCodes[statusCode]++;
                }

                RequestCount++;
                AverageTime += (value - AverageTime) / RequestCount;
            }

            yield return entry;
        }
    }
}
