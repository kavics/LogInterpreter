namespace LogInterpreter.Abstractions.DefaultImplementations;

/// <summary>
/// Reads log files from a specified directory or a single file and emits their full paths.
/// If <paramref name="firstFileName"/> is specified, it enumerates the files from that point on.
/// </summary>
/// <param name="path">Full path of the log source directory or one log file.</param>
/// <param name="firstFileName">Optional. Skips files before the given name.
///   The order depends on the operating system.
///   Only makes sense if <paramref name="path"/> points to a directory.</param>
public class LogSource(string path, string? firstFileName = null) : IPipelineItem<int, string>
{
    public IEnumerable<int> Input { get; set; } = Array.Empty<int>();
    public IEnumerator<string> GetEnumerator()
    {
        if(Directory.Exists(path))
        {
            if (firstFileName == null)
            {
                foreach (var file in Directory.GetFiles(path))
                    yield return file;
            }
            else
            {
                var trigger = firstFileName.ToLowerInvariant();
                var triggerAchieved = false;
                foreach (var file in Directory.GetFiles(path))
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
        else if(File.Exists(path))
        {
            yield return path;
        }
        else
        {
            throw new FileNotFoundException($"The specified path '{path}' does not exist or is not a directory/file.");
        }
    }
}
