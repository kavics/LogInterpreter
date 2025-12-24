using Kavics.LogInterpreter.Abstractions;
using Kavics.LogInterpreter.Abstractions.DefaultImplementations;
using System.Text;

namespace LogInterpreter.CLI.Customizations;

internal class ManfredRentalCollector : IPipelineItem<LogEntry, LogEntry>
{
    private class Rental
    {
        public string Id { get; set; }
        public string Renter { get; set; }
        public string Bicycle { get; set; }
        public bool Finished { get; set; }
        public bool Problematic { get; set; } = true;
        public List<string> Log { get; } = new List<string>();
    }

    public string Name => this.GetType().Name;

    public IEnumerable<LogEntry> Input { get; set; } = Array.Empty<LogEntry>();

    private Dictionary<string, Rental> _rentals = new();

    public IEnumerator<LogEntry> GetEnumerator()
    {
        foreach (var entry in Input)
        {
            if (EntryFilter(entry))
            {
                var formatted = Format(entry, out var rentalId, out var renter, out var bicycle);
                if (rentalId != null)
                {
                    if (!_rentals.TryGetValue(rentalId, out var rental))
                    {
                        rental = new Rental();
                        _rentals.Add(rentalId, rental);
                    }

                    if (rentalId != null)
                        rental.Id = rentalId;
                    if (renter != null)
                        rental.Renter = renter;
                    if (bicycle != null)
                        rental.Bicycle = bicycle;

                    if (formatted.EndsWith("--> Error"))
                    {
                        rental.Finished = true;
                    }
                    if (formatted.EndsWith("--> Finished"))
                    {
                        rental.Problematic = false;
                        rental.Finished = true;
                    }
                    rental.Log.Add(formatted);
                }
            }

            yield return entry;
        }
    }

    private bool EntryFilter(LogEntry e)
    {
        if (e.Message.StartsWith("Updating rental status"))
            return true;
        if (e.Message.StartsWith("Rental {RentalId} started for user {UserEmail}"))
            return true;
        if (e.Message.StartsWith("Rental {RentalId} initiated for user {UserEmail}"))
            return true;
        return false;
    }

    private string Format(LogEntry entry, out string? rental, out string? renter, out string? bicycle)
    {
        entry.Properties.TryGetValue("RentalId", out rental);
        entry.Properties.TryGetValue("UserEmail", out renter);
        entry.Properties.TryGetValue("Bicycle", out bicycle);

        var message = entry.Message
            .Replace("Rental {RentalId} started for user {UserEmail}", "Rental start")
            .Replace("Rental {RentalId} initiated for user {UserEmail}", "Rental start")
            .Replace("Updating rental status: ", "")
            .Replace("Updating rental status after wait for close: ", "")
            .Replace("Updating rental status after wait for open: ", "");

        return $"{entry.Time.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fff}\t{message}".Trim();
    }

    public void WriteToFile(string filePath)
    {
        using var writer = new StreamWriter(filePath, Encoding.UTF8, new FileStreamOptions
        {
            Access = FileAccess.Write,
            Mode = FileMode.OpenOrCreate
        });

        var count = 0;
        var problematicCount = 0;
        var finishedCount = 0;

        foreach (var item in _rentals)
        {
            var rental = item.Value;

            count++;
            if (rental.Problematic)
                problematicCount++;
            if (rental.Finished)
                finishedCount++;

            writer.WriteLine($"#{rental.Id} &{rental.Bicycle} @{rental.Renter} " +
                $"{(rental.Problematic ? "!" : "")} {(rental.Finished ? "" : "RUNNING")}");

            foreach (var line in rental.Log)
            {
                writer.WriteLine(line);
            }
        }
        writer.WriteLine("===================================================");
        writer.WriteLine($"TOTAL RENTALS: {count}");
        writer.WriteLine($"PROBLEMATIC RENTALS: {problematicCount}");
        writer.WriteLine($"FINISHED RENTALS: {finishedCount}");
        writer.WriteLine($"RUNNING RENTALS: {count - finishedCount}");
    }
}
