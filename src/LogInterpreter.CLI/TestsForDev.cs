using Kavics.LogInterpreter.Abstractions;
using Kavics.LogInterpreter.Abstractions.DefaultImplementations;
using LogInterpreter.CLI.Customizations;
using Newtonsoft.Json.Linq;
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

    internal static void CompactJsonParser_Manfred_Prod_FilesAndDates()
    {
        new Pipeline()
            .AddItem(new LogSource(@"D:\__temp\logs\manfredrepo-prod-all"))
            //.AddItem(new LogSource(@"D:\__temp\logs\manfredrepo-prod", "log-20250625_124309.txt"))
            .AddItem(new ConsoleWriter())
            .AddItem(new FirstLineReader())
            .AddItem(new Transformer<string, string>(line => $"    {(line.Length > 60 ? line.Substring(0, 60) : line)}"))
            .AddItem(new ConsoleWriter())
            .Run();
    }

    internal static void CompactJsonParser_Manfred_LiveTest2025_07_12()
    {
        var counter = new Counter();
        var errorAggregator = new ErrorAggregator();
        var collector = new ManfredUnfinishedRentalCollector();
        var pattern2 = @"Rental (\d+) initiated for user (\S+)";

        new Pipeline()
            .AddItem(new LogSource(@"D:\__temp\logs\manfredrepo-prod\log-20250710_145840.txt"))
            .AddItem(new OneLineLogFileReader())
            .AddItem(new CompactJsonLogParser())
            .AddItem(counter)
            .AddItem(errorAggregator)
            .AddItem(new Filter<LogEntry>(e =>
            {
                if (e.Message.StartsWith("Updating rental status"))
                    return true;
                if (e.Message.StartsWith("Rental {RentalId} started for user {UserEmail}"))
                    return true;
                if (e.Message.StartsWith("Rental {RentalId} initiated for user {UserEmail}"))
                    return true;
                return false;
            }))
            .AddItem(new Formatter<LogEntry>(entry =>
            {
                if (!entry.Properties.TryGetValue("UserEmail", out var user))
                    user = "unknown";
                if (!entry.Properties.TryGetValue("RentalId", out var rental))
                    rental = "";

                var message = entry.Message
                    .Replace("Rental {RentalId} started for user {UserEmail}", "Rental start")
                    .Replace("Updating rental status: ", "")
                    .Replace("Updating rental status after wait for close: ", "");
                return $"{entry.Time.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fff} #{rental} @{user,-32} {message}";
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
        errorAggregator.WriteToFile(@"D:\__temp\logs\analízis\Manfred_LiveTest2027_07_12-ERRORS.txt");
        Console.WriteLine("Ok");

    }

    internal static void CompactJsonParser_Manfred_2025_10_26()
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        var counter = new Counter();
        var errorAggregator = new ErrorAggregator();
        var rentalCollector = new ManfredRentalCollector();
        var collector = new ManfredUnfinishedRentalCollector();
        var webRequestCollector = new ManfredWebRequestCollector();
        var pattern2 = @"Rental (\d+) initiated for user (\S+)";

        new Pipeline()
            .AddItem(new LogSource(@"D:\__temp\logs\manfredrepo-prod-all", @"log-20250625_124309.txt"))
//            .AddItem(new LogSource(@"D:\__temp\logs\manfredrepo-prod-all", @"log-20251103_034508.txt"))
            .AddItem(new ConsoleWriter())
            .AddItem(new OneLineLogFileReader())
            .AddItem(new CompactJsonLogParser())
            .AddItem(counter)
            .AddItem(webRequestCollector)
            .AddItem(errorAggregator)
            .AddItem(rentalCollector)
            //.AddItem(new Filter<LogEntry>(e =>
            //{
            //    if (e.Message.StartsWith("Updating rental status"))
            //        return true;
            //    if (e.Message.StartsWith("Rental {RentalId} started for user {UserEmail}"))
            //        return true;
            //    if (e.Message.StartsWith("Rental {RentalId} initiated for user {UserEmail}"))
            //        return true;
            //    return false;
            //}))
            //.AddItem(new Formatter<LogEntry>(entry =>
            //{
            //    if (!entry.Properties.TryGetValue("RentalId", out var rental))
            //        rental = "";
            //    if (!entry.Properties.TryGetValue("UserEmail", out var user))
            //        user = "unknown";
            //    if (!entry.Properties.TryGetValue("Bicycle", out var bicycle))
            //        user = "----";

            //    var message = entry.Message
            //        .Replace("Rental {RentalId} started for user {UserEmail}", "Rental start")
            //        .Replace("Updating rental status: ", "")
            //        .Replace("Updating rental status after wait for close: ", "");
            //    return $"{entry.Time.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fff} #{rental} &{bicycle} @{user,-32} {message}";
            //}))
            //.AddItem(new ConsoleWriter())
            //.AddItem(new FileWriter(@"D:\__temp\logs\analízis\Manfred_2025_10_26\Rentals.txt"))
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

        Console.WriteLine("WEB REQUESTS:");
        Console.WriteLine($"  count:           {webRequestCollector.RequestCount,8}");
        Console.WriteLine($"  long count:      {webRequestCollector.LongCount,8}");
        Console.WriteLine($"  very long count: {webRequestCollector.VeryLongCount,8}");
        Console.WriteLine($"  average time:       {webRequestCollector.AverageTime:F2} ms");
        Console.WriteLine($"  longest time:       {webRequestCollector.LongestTimeSec:F2} sec");
        Console.WriteLine($"  longest key:        {webRequestCollector.LongestRequestId:F2}");

        Console.Write("Writing error-aggregation file... ");
        errorAggregator.WriteToFile(@"D:\__temp\logs\analízis\Manfred_2025_10_26\ERRORS.txt");
        Console.WriteLine("Ok");

        Console.Write("Writing rental-collection file... ");
        rentalCollector.WriteToFile(@"D:\__temp\logs\analízis\Manfred_2025_10_26\Rentals.txt");
        Console.WriteLine("Ok");

        stopwatch.Stop();
        Console.WriteLine($"Processing time {stopwatch.Elapsed}.");

        Console.WriteLine("Ok");

    }

    internal static void CompactJsonParser_Manfred_SecurityQueueError()
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        var counter = new Counter();
        var errorAggregator = new ErrorAggregator();

        var fileName = @"D:\dev\tfs\Manfred\backend\src\ManfredBackend\App_Data\Logs\Test10";

        new Pipeline()
            .AddItem(new LogSource(fileName))
            .AddItem(new ConsoleWriter())
            .AddItem(new OneLineLogFileReader())
            .AddItem(new CompactJsonLogParser())
            .AddItem(counter)
            .AddItem(errorAggregator)
            .AddItem(new Filter<LogEntry>(e =>
            {// "Current request count" "Request limit reached"
                if (e.Category == "SecurityQueue")
                    return true;
                if (e.Category == "System")
                {
                    if (e.Message.StartsWith("RequestSupervisor:"))
                        return true;
                }
                return false;
            }))
            .AddItem(new Formatter<LogEntry>(entry =>
            {
                var t = entry.Time.ToUniversalTime();
                return $"{t.Date:yyyy-MM-dd}\t{t.Hour}\t{t.Minute}\t{t:ss.fff}\t{entry.Duration.TotalSeconds,-8}\t{entry.Message}";
            }))
            .AddItem(new FileWriter($"{fileName}\\data\\filtered.log"))
            .Run();

        new Pipeline()
            .AddItem(new LogSource(fileName))
            .AddItem(new ConsoleWriter())
            .AddItem(new OneLineLogFileReader())
            .AddItem(new CompactJsonLogParser())
            .AddItem(new Filter<LogEntry>(e =>
            {
                if (e.Category == "System")
                {
                    if (e.Message.StartsWith("RequestSupervisor: ThreadPool:"))
                        return true;
                }
                return false;
            }))
            .AddItem(new Formatter<LogEntry>(entry =>
            {
                var t = entry.Time.ToUniversalTime();
                return $"{t.Date:yyyy-MM-dd}\t{t.Hour}\t{t.Minute}\t{t:ss.fff}\t{entry.Message}";
            }))
            .AddItem(new FileWriter($"{fileName}\\data\\threads.log"))
            .Run();

        new Pipeline()
            .AddItem(new LogSource(fileName))
            .AddItem(new ConsoleWriter())
            .AddItem(new OneLineLogFileReader())
            .AddItem(new CompactJsonLogParser())
            .AddItem(new Filter<LogEntry>(e =>
            {
                if (e.Category == "System")
                {
                    if (e.Message.StartsWith("RequestSupervisor: Request count for User_Registration:"))
                        return true;
                }
                if(e.Message == "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms")
                {
                    // "RequestMethod":"POST","RequestPath":"/odata.svc/('Root')/Users/Registration"
                    if (e.Properties.TryGetValue("RequestPath", out var path) &&
                        path == "/odata.svc/('Root')/Users/Registration")
                        return true;
                }
                return false;
            }))
            .AddItem(new Formatter<LogEntry>(entry =>
            {
                var t = entry.Time.ToUniversalTime();
                var requestId = entry.Properties.ContainsKey("RequestId") ? entry.Properties["RequestId"] : "-------------:--------";
                var elapsed = entry.Properties.ContainsKey("Elapsed") ? entry.Properties["Elapsed"] : "---.------";
                return $"{t.Date:yyyy-MM-dd}\t{t.Hour}\t{t.Minute}\t{t:ss.fff}\t{requestId}\t{elapsed}\t{entry.Message}";
            }))
            .AddItem(new FileWriter($"{fileName}\\data\\registration-requests.log"))
            .Run();

        new Pipeline()
            .AddItem(new LogSource(fileName))
            .AddItem(new ConsoleWriter())
            .AddItem(new OneLineLogFileReader())
            .AddItem(new CompactJsonLogParser())
            .AddItem(new Filter<LogEntry>(e =>
            {
                if (e.Category == "System")
                {
                    if (e.Message.StartsWith("RequestSupervisor: ThreadPool:"))
                        return true;
                }
                return false;
            }))
            .AddItem(new Formatter<LogEntry>(entry =>
            {
                // Parse: "RequestSupervisor: ThreadPool: 26, 11, 163936 | 9, 3, 4, 0"
                // Output: "26\t11\t9\t3\t4\t0\t163936"
                var message = entry.Message;

                // Extract the numbers part after "ThreadPool: "
                var prefix = "RequestSupervisor: ThreadPool: ";
                if (!message.StartsWith(prefix))
                    return message;

                var numbersPart = message.Substring(prefix.Length);

                var numbers = numbersPart
                    .Split(new[] { '|', ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .ToList();

                if (numbers.Count >= 3)
                {
                    var thirdElement = numbers[2];
                    numbers.RemoveAt(2);
                    numbers.Add(thirdElement);
                }

                return string.Join("\t", numbers);
            }))
            .AddItem(new FileWriter($"{fileName}\\data\\threads_table.log"))
            .Run();

        Console.WriteLine("====================================================");
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
        errorAggregator.WriteToFile($"{fileName}\\data\\errors.log");
        Console.WriteLine("Ok");

        stopwatch.Stop();
        Console.WriteLine($"Processing time {stopwatch.Elapsed}.");

        Console.WriteLine("Ok");

    }

    internal static void CompactJsonParser_Manfred_Prod_Analysis_2025_12_24()
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        var counter = new Counter();
        var errorAggregator = new ErrorAggregator();
        var rentalCollector = new ManfredRentalCollector();
        var webRequestCollector = new ManfredWebRequestCollector();

        new Pipeline()
            .AddItem(new LogSource(@"D:\__temp\logs\manfredrepo-prod-all", @"log-20250625_124309.txt"))
            .AddItem(new ConsoleWriter())
            .AddItem(new OneLineLogFileReader())
            .AddItem(new CompactJsonLogParser())
            .AddItem(counter)
            .AddItem(webRequestCollector)
            .AddItem(errorAggregator)
            .AddItem(rentalCollector)
            //.AddItem(new Filter<LogEntry>(e =>
            //{
            //    if (e.Message.StartsWith("Updating rental status"))
            //        return true;
            //    if (e.Message.StartsWith("Rental {RentalId} started for user {UserEmail}"))
            //        return true;
            //    if (e.Message.StartsWith("Rental {RentalId} initiated for user {UserEmail}"))
            //        return true;
            //    return false;
            //}))
            //.AddItem(new Formatter<LogEntry>(entry =>
            //{
            //    if (!entry.Properties.TryGetValue("RentalId", out var rental))
            //        rental = "";
            //    if (!entry.Properties.TryGetValue("UserEmail", out var user))
            //        user = "unknown";
            //    if (!entry.Properties.TryGetValue("Bicycle", out var bicycle))
            //        user = "----";

            //    var message = entry.Message
            //        .Replace("Rental {RentalId} started for user {UserEmail}", "Rental start")
            //        .Replace("Updating rental status: ", "")
            //        .Replace("Updating rental status after wait for close: ", "");
            //    return $"{entry.Time.ToUniversalTime():yyyy-MM-dd HH:mm:ss.fff} #{rental} &{bicycle} @{user,-32} {message}";
            //}))
            //.AddItem(new ConsoleWriter())
            //.AddItem(new FileWriter(@"D:\__temp\logs\analízis\Manfred_2025_10_26\Rentals.txt"))
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

        Console.WriteLine("WEB REQUESTS:");
        Console.WriteLine($"  count:           {webRequestCollector.RequestCount,8}");
        Console.WriteLine($"  long count:      {webRequestCollector.LongCount,8}");
        Console.WriteLine($"  very long count: {webRequestCollector.VeryLongCount,8}");
        Console.WriteLine($"  average time:       {webRequestCollector.AverageTime:F2} ms");
        Console.WriteLine($"  longest time:       {webRequestCollector.LongestTimeSec:F2} sec");
        Console.WriteLine($"  longest key:        {webRequestCollector.LongestRequestId:F2}");
        Console.WriteLine($"  Status codes:");
        foreach (var kvp in webRequestCollector.StatusCodes.OrderBy(kvp => kvp.Key))
            Console.WriteLine($"    {kvp.Key}: {kvp.Value}");

        Console.Write("Writing error-aggregation file... ");
        errorAggregator.WriteToFile(@"D:\__temp\logs\analízis\Manfred_2025_12_24\ERRORS.txt");
        Console.WriteLine("Ok");

        Console.Write("Writing rental-collection file... ");
        rentalCollector.WriteToFile(@"D:\__temp\logs\analízis\Manfred_2025_12_24\Rentals.txt");
        Console.WriteLine("Ok");

        stopwatch.Stop();
        Console.WriteLine($"Processing time {stopwatch.Elapsed}.");

        Console.WriteLine("Ok");

    }
}