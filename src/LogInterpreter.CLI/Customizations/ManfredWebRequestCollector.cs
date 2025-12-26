using Kavics.LogInterpreter.Abstractions;
using Kavics.LogInterpreter.Abstractions.DefaultImplementations;
using System.Text;

namespace LogInterpreter.CLI.Customizations;

internal class ManfredWebRequestCollector : IPipelineItem<LogEntry, LogEntry>
{
    public string Name => this.GetType().Name;

    public IEnumerable<LogEntry> Input { get; set; } = Array.Empty<LogEntry>();

    private Dictionary<string, (int Count, DateTime FirstTime, DateTime LastTime)> _requestEntries = new();
    public Dictionary<string, int> StatusCodes = new();
    public Dictionary<string, int> RequestPaths = new();
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

                // Increment request path counter
                if (entry.Properties.TryGetValue("RequestPath", out var requestPath))
                {
                    if (!RequestPaths.ContainsKey(requestPath))
                        RequestPaths[requestPath] = 0;
                    RequestPaths[requestPath]++;
                }

                RequestCount++;
                AverageTime += (value - AverageTime) / RequestCount;
            }

            yield return entry;
        }
    }

    public void WriteToFile(string filePath)
    {
        using var writer = new StreamWriter(filePath, false, Encoding.UTF8);

        writer.WriteLine("WEB REQUEST STATISTICS");
        writer.WriteLine("======================");
        writer.WriteLine();
        writer.WriteLine($"Total Requests:        {RequestCount,8}");
        writer.WriteLine($"Long Requests (>4ms):  {LongCount,8}");
        writer.WriteLine($"Very Long (>10ms):     {VeryLongCount,8}");
        writer.WriteLine($"Average Time:          {AverageTime,8:F2} ms");
        writer.WriteLine($"Longest Time:          {LongestTimeSec,8:F2} sec");
        writer.WriteLine($"Longest Request ID:    {LongestRequestId}");
        writer.WriteLine();

        writer.WriteLine("STATUS CODES");
        writer.WriteLine("------------");
        foreach (var kvp in StatusCodes.OrderBy(x => x.Key))
        {
            writer.WriteLine($"  {kvp.Key,3}: {kvp.Value,8}");
        }
        writer.WriteLine();

        writer.WriteLine("REQUEST PATHS");
        writer.WriteLine("-------------");
        foreach (var kvp in RequestPaths.OrderByDescending(x => x.Value))
        {
            writer.WriteLine($"  {kvp.Value,8}  {kvp.Key}");
        }
    }
}
