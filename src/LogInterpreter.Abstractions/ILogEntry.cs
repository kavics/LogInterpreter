namespace LogInterpreter.Abstractions;

public interface ILogEntry
{
    DateTime Time { get; set; } // 2025-03-30 00:00:04.871 +00:00
    LogLevel Level { get; set; } // [VRB]
    string Message { get; set; }
    string[] Raw { get; set; }

    IDictionary<string, string> Properties { get; set; }

    /* =========================================== SnTrace */

    int LineId { get; set; }
    string Category { get; set; }
    long ProgramFlowId { get; set; }
    int OpId { get; set; }
    string Status { get; set; }
    TimeSpan Duration { get; set; }
}
