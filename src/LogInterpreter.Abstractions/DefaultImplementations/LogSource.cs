namespace LogInterpreter.Abstractions.DefaultImplementations;

/// <summary>
/// Reads log files from a specified directory or a single file and emits their full paths.
/// If <paramref name="firstFileName"/> is specified, it enumerates the files from that point on.
/// </summary>
/// <param name="path">Full path of the log source directory or one log file.</param>
/// <param name="firstFileName">Optional. Skips files before the given name.
///   The order depends on the operating system.
///   Only makes sense if <paramref name="path"/> points to a directory.</param>
public class LogSource : IPipelineItem<int, string>
{
    public string Name => this.GetType().Name;

    public IEnumerable<int> Input { get; set; } = Array.Empty<int>();

    public string LogPath { get; set; }
    public string? FirstFileName { get; set; }
    public LogSource(string path, string? firstFileName = null)
    {
        LogPath = path;
        FirstFileName = firstFileName;
    }

    public IEnumerator<string> GetEnumerator()
    {
        if(Directory.Exists(LogPath))
        {
            if (FirstFileName == null)
            {
                foreach (var file in Directory.GetFiles(LogPath))
                    yield return file;
            }
            else
            {
                var trigger = FirstFileName.ToLowerInvariant();
                var triggerAchieved = false;
                foreach (var file in Directory.GetFiles(LogPath))
                {
                    if (!triggerAchieved && Path.GetFileName(file).ToLowerInvariant() == trigger)
                    {
                        triggerAchieved = true;
                        yield return file;
                    }
                    else if (triggerAchieved)
                        yield return file;
                }
            }
        }
        else if(File.Exists(LogPath))
        {
            yield return LogPath;
        }
        else
        {
            throw new FileNotFoundException($"The specified path '{LogPath}' does not exist or is not a directory/file.");
        }
    }
}
