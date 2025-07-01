using LogInterpreter.Abstractions;
using LogInterpreter.Abstractions.DefaultImplementations;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace LogInterpreter.CLI;

internal static class TestsForDev
{
    public static void TwoLineParser()
    {
        var counter = new Counter();
        var errorAggregator = new ErrorAggregator();

        new Pipeline()
            .AddItem(new LogSource(@"D:\__temp\logs\manfredrepo-test"))
            .AddItem(new TwoLineLogFileReader())
            .AddItem(new TwoLineLogParser())
            .AddItem(counter)
            .AddItem(errorAggregator)
            .AddItem(new Filter<LogEntry>(e => e.Level <= LogLevel.Warning))
            //.AddItem(new Filter<BuiltInEntry>(e => e.Category != "Index"))
            //.AddItem(new Filter<LogEntry>(e => e.Category == "Event" || e.Message.StartsWith("Timestamp")))
            .AddItem(new Formatter<LogEntry>(entry => $"{entry.Time:yyyy-MM-dd HH:mm:ss.fff} {entry.Level, -16} {entry.Category, -12} {entry.Message}"))
            .AddItem(new ConsoleWriter())
            .Run();
        Console.WriteLine("===========================================================");
        Console.WriteLine($"Entries:           {counter.Entries,8}");
        Console.WriteLine($"NotParsed:         {counter.NotParsedEntries,8}");
        Console.WriteLine("LEVELS");
        Console.WriteLine($"  Informations:    {counter.Informations,8}");
        Console.WriteLine($"  Warnings:        {counter.Warnings,8}");
        Console.WriteLine($"  Errors:          {counter.Errors,8}");
        Console.WriteLine("CATEGORIES");
        foreach (var category in counter.Categories)
            Console.WriteLine($"  {category.Key,-16} {category.Value,8}");

        Console.Write("Writing error-aggregation file... ");
        errorAggregator.WriteToFile(@"D:\__temp\logs\manfredrepo-test-ERRORS.txt");
        Console.WriteLine("Ok");

    }

    internal static void TwoLineParserIoT()
    {
        var counter = new Counter();
        var errorAggregator = new ErrorAggregator();

        new Pipeline()
            .AddItem(new LogSource(@"D:\__temp\logs\analízis\log-20250613_091610.txt"))
            .AddItem(new TwoLineLogFileReader())
            .AddItem(new TwoLineLogParser())
            .AddItem(counter)
            .AddItem(errorAggregator)
            .AddItem(new Filter<LogEntry>(e => e.Message.StartsWith("[IoT ")))
            .AddItem(new Formatter<LogEntry>(entry => $"{entry.Time:yyyy-MM-dd HH:mm:ss.fff} {entry.Message}"))
            .AddItem(new ConsoleWriter())
            .Run();
        Console.WriteLine("===========================================================");
        Console.WriteLine($"Entries:           {counter.Entries,8}");
        Console.WriteLine($"NotParsed:         {counter.NotParsedEntries,8}");
        Console.WriteLine("LEVELS");
        Console.WriteLine($"  Informations:    {counter.Informations,8}");
        Console.WriteLine($"  Warnings:        {counter.Warnings,8}");
        Console.WriteLine($"  Errors:          {counter.Errors,8}");
        Console.WriteLine("CATEGORIES");
        foreach (var category in counter.Categories)
            Console.WriteLine($"  {category.Key,-16} {category.Value,8}");

        Console.Write("Writing error-aggregation file... ");
        errorAggregator.WriteToFile(@"D:\__temp\logs\manfredrepo-test-ERRORS.txt");
        Console.WriteLine("Ok");
    }

    internal static void TwoLineParser_Manfred_Experimental()
    {
        var counter = new Counter();
        var errorAggregator = new ErrorAggregator();

        var pattern1 = @"Rental (\d+) started for user (\S+)";
        var pattern2 = @"Rental (\d+) initiated for user (\S+)";
        var flespiCommandPattern = @"^\[IoT drv (\d+)\]\s*POST https://flespi\.io/gw/devices/(\d+)/commands-queue\s*";
        var flespiCommandResponsePattern = @"^\[IoT drv (\d+)\]\s*response: \{.*\}\s*$";
        new Pipeline()
            .AddItem(new LogSource(@"D:\__temp\logs\analízis\test-20250618_142218.txt"))
            .AddItem(new TwoLineLogFileReader())
            .AddItem(new TwoLineLogParser())
            .AddItem(counter)
            .AddItem(errorAggregator)
            .AddItem(new Filter<LogEntry>(e =>
            {
                if (e.Message.StartsWith("[IoT SRV 0]"))
                    return false;
                if (e.Message.StartsWith("[IoT SRV") && e.Message.Contains("Removed DeviceAction."))
                    return false;
                if (e.Message.StartsWith("[IoT SRV") && e.Message.Contains("Update DeviceAction."))
                    return false;
                if (e.Message.StartsWith("[IoT drv") && e.Message.Contains("GET https://device.manfred.mobi/api/telemetry/"))
                    return false;
                if (Regex.IsMatch(e.Message, flespiCommandPattern))
                    return false;
                if (e.Message.StartsWith("[IoT drv") && e.Message.Contains("]   waiting"))
                    return false;
                if (e.Message.StartsWith("[IoT "))
                    return true;
                if (e.Message.StartsWith("Updating rental status"))
                    return true;
                if (Regex.IsMatch(e.Message, pattern1))
                    return true;
                if (Regex.IsMatch(e.Message, pattern2))
                    return true;
                return false;
            }))
            .AddItem(new Transformer<LogEntry, LogEntry>(entry =>
            {
                // [IoT drv 1204]   response: {"result":[{...,"name":"setting.unlock.set","device_id":6435807,...}]}
                if (!Regex.IsMatch(entry.Message, flespiCommandResponsePattern))
                    return entry;

                string pattern = @"^\[IoT drv (\d+)\]\s*response:\s+(.*)$";
                var match = Regex.Match(entry.Message, pattern);

                if (match.Success)
                {
                    var contentId = int.Parse(match.Groups[1].Value);
                    var json = match.Groups[2].Value;
                    var deserialized = Newtonsoft.Json.Linq.JToken.Parse(json);
                    var name = deserialized["result"]?[0]?["name"]?.ToString() ?? "Unknown";
                    var deviceId = deserialized["result"]?[0]?["device_id"]?.ToString() ?? null;
                    entry.Message = $"[IoT drv {contentId}]   Command: {name}, deviceId: {deviceId}";
                }
                return entry;
            }))
            .AddItem(new Formatter<LogEntry>(entry =>
            {
                if (entry.Properties.ContainsKey("RentalId"))
                    return $"{entry.Time.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fff} #{entry.Properties["RentalId"]} {entry.Message}";
                return $"{entry.Time.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fff} {entry.Message}";
            }))
            .AddItem(new ConsoleWriter())
            .Run();
        Console.WriteLine("===========================================================");
        Console.WriteLine($"Entries:           {counter.Entries,8}");
        Console.WriteLine($"NotParsed:         {counter.NotParsedEntries,8}");
        Console.WriteLine("LEVELS");
        Console.WriteLine($"  Informations:    {counter.Informations,8}");
        Console.WriteLine($"  Warnings:        {counter.Warnings,8}");
        Console.WriteLine($"  Errors:          {counter.Errors,8}");
        Console.WriteLine("CATEGORIES");
        foreach (var category in counter.Categories)
            Console.WriteLine($"  {category.Key,-16} {category.Value,8}");

        Console.Write("Writing error-aggregation file... ");
        errorAggregator.WriteToFile(@"D:\__temp\logs\analízis\test-20250618_142218-ERRORS.txt");
        Console.WriteLine("Ok");
    }

    internal static void TwoLineParser_Manfred_RentalAndIoT()
    {
        var collector = new ManfredUnfinishedRentalCollector();

        var pattern1 = @"Rental (\d+) started for user (\S+)";
        var pattern2 = @"Rental (\d+) initiated for user (\S+)";
        var flespiCommandPattern = @"^\[IoT drv (\d+)\]\s*POST https://flespi\.io/gw/devices/(\d+)/commands-queue\s*";
        var flespiCommandResponsePattern = @"^\[IoT drv (\d+)\]\s*response: \{.*\}\s*$";
        new Pipeline()
            .AddItem(new LogSource(@"D:\__temp\logs\analízis\test-20250618_142218.txt"))
            .AddItem(new TwoLineLogFileReader())
            .AddItem(new TwoLineLogParser())
            .AddItem(new Filter<LogEntry>(e =>
            {
                if (e.Message.StartsWith("[IoT SRV 0]"))
                    return false;
                if (e.Message.StartsWith("[IoT SRV") && e.Message.Contains("Removed DeviceAction."))
                    return false;
                if (e.Message.StartsWith("[IoT SRV") && e.Message.Contains("Update DeviceAction."))
                    return false;
                if (e.Message.StartsWith("[IoT drv") && e.Message.Contains("GET https://device.manfred.mobi/api/telemetry/"))
                    return false;
                if (Regex.IsMatch(e.Message, flespiCommandPattern))
                    return false;
                if (e.Message.StartsWith("[IoT drv") && e.Message.Contains("]   waiting"))
                    return false;
                if (e.Message.StartsWith("[IoT "))
                    return true;
                if (e.Message.StartsWith("Updating rental status"))
                    return true;
                if (Regex.IsMatch(e.Message, pattern1))
                    return true;
                if (Regex.IsMatch(e.Message, pattern2))
                    return true;
                return false;
            }))
            .AddItem(new Transformer<LogEntry, LogEntry>(entry =>
            {
                // [IoT drv 1204]   response: {"result":[{...,"name":"setting.unlock.set","device_id":6435807,...}]}
                if (!Regex.IsMatch(entry.Message, flespiCommandResponsePattern))
                    return entry;

                string pattern = @"^\[IoT drv (\d+)\]\s*response:\s+(.*)$";
                var match = Regex.Match(entry.Message, pattern);

                if (match.Success)
                {
                    var contentId = int.Parse(match.Groups[1].Value);
                    var json = match.Groups[2].Value;
                    var deserialized = Newtonsoft.Json.Linq.JToken.Parse(json);
                    var name = deserialized["result"]?[0]?["name"]?.ToString() ?? "Unknown";
                    var deviceId = deserialized["result"]?[0]?["device_id"]?.ToString() ?? null;
                    entry.Message = $"[IoT drv {contentId}]   Command: {name}, deviceId: {deviceId}";
                }
                return entry;
            }))
            .AddItem(new Formatter<LogEntry>(entry =>
            {
                if (entry.Properties.ContainsKey("RentalId"))
                    return $"{entry.Time.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fff} #{entry.Properties["RentalId"]} {entry.Message}";
                return $"{entry.Time.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fff} {entry.Message}";
            }))
            .AddItem(new FileWriter(@"D:\__temp\logs\analízis\test-20250618_142218-RentalsAndIoT.txt"))
            .AddItem(new Filter<string>(s =>
                s.Contains(" initiated for user ") ||
                s.Contains(" started for user") ||
                s.Contains("WaitForPayment --> Finished") ||
                s.Contains("WaitForPayment --> Error")))
            .AddItem(new Transformer<string, string>(s =>
            {
                if (s.Contains("WaitForPayment --> Finished"))
                {
                    var p = s.IndexOf("Updating rental status");
                    return s.Substring(0, p) + "Finished";
                }
                if (s.Contains("WaitForPayment --> Error"))
                {
                    var p = s.IndexOf("Updating rental status");
                    return s.Substring(0, p) + "Error";
                }
                return s;
            }))
            .AddItem(new ConsoleWriter())
            .AddItem(collector)
            .Run();
        Console.WriteLine("===========================================================");
        Console.WriteLine($"UNFINISHED RENTALS");
        Console.WriteLine($"RentalId       User");
        Console.WriteLine($"-------------  --------------------");
        foreach (var kvp in collector.GetUnfinishedRentals())
        {
            Console.WriteLine($"{kvp.Key,10} {kvp.Value}");
        }
        Console.WriteLine($"===========================================================");
        Console.WriteLine("Ok");
    }

    internal static void CompactJsonParser_Manfred_RentalAndIoT()
    {
        var collector = new ManfredUnfinishedRentalCollector();

        var pattern1 = @"Rental (\d+) started for user (\S+)";
        var pattern2 = @"Rental (\d+) initiated for user (\S+)";
        var flespiCommandPattern = @"^\[IoT drv (\d+)\]\s*POST https://flespi\.io/gw/devices/(\d+)/commands-queue\s*";
        var flespiCommandResponsePattern = @"^\[IoT drv (\d+)\]\s*response: \{.*\}\s*$";
        new Pipeline()
            .AddItem(new LogSource(@"D:\__temp\logs\analízis\log-local-compactjson-20250620_163228.txt"))
            .AddItem(new OneLineLogFileReader())
            .AddItem(new CompactJsonLogParser())
            .AddItem(new Filter<LogEntry>(e =>
            {
                if (e.Message.StartsWith("[IoT SRV 0]"))
                    return false;
                if (e.Message.StartsWith("[IoT SRV") && e.Message.Contains("Removed DeviceAction."))
                    return false;
                if (e.Message.StartsWith("[IoT SRV") && e.Message.Contains("Update DeviceAction."))
                    return false;
                if (e.Message.StartsWith("[IoT drv") && e.Message.Contains("GET https://device.manfred.mobi/api/telemetry/"))
                    return false;
                if (Regex.IsMatch(e.Message, flespiCommandPattern))
                    return false;
                if (e.Message.StartsWith("[IoT drv") && e.Message.Contains("]   waiting"))
                    return false;
                if (e.Message.StartsWith("[IoT "))
                    return true;
                if (e.Message.StartsWith("Updating rental status"))
                    return true;
                if (Regex.IsMatch(e.Message, pattern1))
                    return true;
                if (Regex.IsMatch(e.Message, pattern2))
                    return true;
                return false;
            }))
            .AddItem(new Transformer<LogEntry, LogEntry>(entry =>
            {
                // [IoT drv 1204]   response: {"result":[{...,"name":"setting.unlock.set","device_id":6435807,...}]}
                if (!Regex.IsMatch(entry.Message, flespiCommandResponsePattern))
                    return entry;

                string pattern = @"^\[IoT drv (\d+)\]\s*response:\s+(.*)$";
                var match = Regex.Match(entry.Message, pattern);

                if (match.Success)
                {
                    var contentId = int.Parse(match.Groups[1].Value);
                    var json = match.Groups[2].Value;
                    var deserialized = Newtonsoft.Json.Linq.JToken.Parse(json);
                    var name = deserialized["result"]?[0]?["name"]?.ToString() ?? "Unknown";
                    var deviceId = deserialized["result"]?[0]?["device_id"]?.ToString() ?? null;
                    entry.Message = $"[IoT drv {contentId}]   Command: {name}, deviceId: {deviceId}";
                }
                return entry;
            }))
            .AddItem(new Formatter<LogEntry>(entry =>
            {
                if (entry.Properties.ContainsKey("RentalId"))
                    return $"{entry.Time.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fff} #{entry.Properties["RentalId"]} {entry.Message}";
                return $"{entry.Time.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fff} {entry.Message}";
            }))
            .AddItem(new FileWriter(@"D:\__temp\logs\analízis\log-local-compactjson-20250620_163228-RentalsAndIoT.txt"))
            .AddItem(new Filter<string>(s =>
                s.Contains(" initiated for user ") ||
                s.Contains(" started for user") ||
                s.Contains("WaitForPayment --> Finished") ||
                s.Contains("WaitForPayment --> Error")))
            .AddItem(new Transformer<string, string>(s =>
            {
                if (s.Contains("WaitForPayment --> Finished"))
                {
                    var p = s.IndexOf("Updating rental status");
                    return s.Substring(0, p) + "Finished";
                }
                if (s.Contains("WaitForPayment --> Error"))
                {
                    var p = s.IndexOf("Updating rental status");
                    return s.Substring(0, p) + "Error";
                }
                return s;
            }))
            .AddItem(new ConsoleWriter())
            .AddItem(collector)
            .Run();
        Console.WriteLine("===========================================================");
        Console.WriteLine($"UNFINISHED RENTALS");
        Console.WriteLine($"RentalId       User");
        Console.WriteLine($"-------------  --------------------");
        foreach (var kvp in collector.GetUnfinishedRentals())
        {
            Console.WriteLine($"{kvp.Key,10} {kvp.Value}");
        }
        Console.WriteLine($"===========================================================");
        Console.WriteLine("Ok");
    }
}

/// <summary>
/// Experimental class
/// USAGE
/// string template = "*Id:{id},*Name:{name},*";
/// string line = "Akármi Id: 42, Version: 3, Name: Name1, stb";
/// var result = TemplateParser.ParseFromTemplate(line, template);
/// foreach (var kvp in result)
///     Console.WriteLine($"{kvp.Key} = {kvp.Value}");
/// </summary>
internal static class TemplateParser
{
    public static Dictionary<string, string> ParseFromTemplate(string line, string template)
    {
        var result = new Dictionary<string, string>();

        // Regex sablon építése a template alapján
        var regexPattern = new System.Text.StringBuilder();
        int i = 0;
        while (i < template.Length)
        {
            if (template[i] == '*')
            {
                regexPattern.Append(".*?");
                i++;
            }
            else if (template[i] == '{')
            {
                int end = template.IndexOf('}', i);
                if (end == -1) throw new FormatException("Hiányzó '}' a template-ben.");

                string name = template.Substring(i + 1, end - i - 1);
                regexPattern.Append($@"(?<{name}>.*?)");
                i = end + 1;
            }
            else
            {
                // Fix szöveg → escape-eljük
                regexPattern.Append(Regex.Escape(template[i].ToString()));
                i++;
            }
        }

        var regex = new Regex($"^{regexPattern}$");
        var match = regex.Match(line);

        if (!match.Success) return result;

        foreach (var name in regex.GetGroupNames())
        {
            if (int.TryParse(name, out _)) continue;
            result[name] = match.Groups[name].Value;
        }

        return result;
    }
}

internal class Counter : IPipelineItem<LogEntry, LogEntry>
{
    public int Entries { get; private set; }
    public int NotParsedEntries { get; private set; }
    public int Informations { get; private set; }
    public int Warnings { get; private set; }
    public int Errors { get; private set; }
    public Dictionary<string, int> Categories { get; } = new Dictionary<string, int>();

    public IEnumerable<LogEntry> Input { get; set; } = Array.Empty<LogEntry>();

    public IEnumerator<LogEntry> GetEnumerator()
    {
        foreach (var entry in Input)
        {
            Entries++;
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
                if(!Categories.ContainsKey(entry.Category))
                    Categories[entry.Category] = 0;
                Categories[entry.Category]++;
            }

            yield return entry;
        }
    }
}

internal class ErrorAggregator : IPipelineItem<LogEntry, LogEntry>
{
    public IEnumerable<LogEntry> Input { get; set; } = Array.Empty<LogEntry>();

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

            if(target != null)
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
}


internal class ManfredUnfinishedRentalCollector : IPipelineItem<string, string>
{
    public IEnumerable<string> Input { get; set; } = Array.Empty<string>();

    Dictionary<string, string> _started = new ();
    HashSet<string> _finished = new ();

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
}