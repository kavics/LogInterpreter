namespace LogInterpreter.Abstractions.DefaultImplementations;

public class LogEntry : ILogEntry
{
    public DateTime Time { get; set; }
    public LogLevel Level { get; set; }
    public string Message { get; set; } = string.Empty;
    public string[] Raw { get; set; } = new string[0];
    public IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
    public int LineId { get; set; }
    public string Category { get; set; } = string.Empty;
    public long ProgramFlowId { get; set; }
    public int OpId { get; set; }
    public string Status { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
}