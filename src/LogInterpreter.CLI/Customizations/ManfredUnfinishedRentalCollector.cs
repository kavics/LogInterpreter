using Kavics.LogInterpreter.Abstractions;
using System.Text;
using System.Text.RegularExpressions;

namespace LogInterpreter.CLI.Customizations;

internal class ManfredUnfinishedRentalCollector : IPipelineItem<string, string>
{
    public Pipeline Pipeline { get; set; } = null!;
    public string Name => this.GetType().Name;

    public IEnumerable<string> Input { get; set; } = Array.Empty<string>();

    Dictionary<string, string> _started = new();
    HashSet<string> _finished = new();

    IEnumerator<string> IEnumerable<string>.GetEnumerator()
    {
        var startRegex = new Regex(@"Rental (\d+) (initiated|started) for user (\S+)");
        var finishRegex = new Regex(@"#(\d+)\s+Finished");

        foreach (var line in Input)
        {
            var startMatch = startRegex.Match(line);
            if (startMatch.Success)
            {
                var rentalId = startMatch.Groups[1].Value;
                var user = startMatch.Groups[3].Value;
                _started[rentalId] = user;
                continue;
            }

            var finishMatch = finishRegex.Match(line);
            if (finishMatch.Success)
            {
                var rentalId = finishMatch.Groups[1].Value;
                _finished.Add(rentalId);
            }

            yield return line;
        }
    }

    public IEnumerable<KeyValuePair<string, string>> GetUnfinishedRentals()
    {
        foreach (var kvp in _started.Where(kvp => !_finished.Contains(kvp.Key)))
            yield return kvp;
    }

    internal void WriteToFile(string v)
    {
        using var writer = new StreamWriter(v, Encoding.UTF8, new FileStreamOptions
        {
            Access = FileAccess.Write,
            Mode = FileMode.OpenOrCreate
        });
        writer.WriteLine("Unfinished Rentals");
        writer.WriteLine("------------------");
        writer.WriteLine();
        writer.WriteLine($"{"RentalId",-10} User");
        writer.WriteLine($"{"--------",-10} --------------------");
        foreach (var kvp in GetUnfinishedRentals())
        {
            writer.WriteLine($"{kvp.Key,10} {kvp.Value}");
        }
    }
}
