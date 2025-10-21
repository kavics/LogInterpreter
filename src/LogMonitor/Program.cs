using System.Text.Json;

string baseUrl = args.Length > 0 ? args[0].TrimEnd('/') : throw new ArgumentException("URL argument required.");
string apiKey = "hTzgPCY4V0UsPDkL3rdDQMse7DVOj8oPER58Smt2eLwJzfIW7Mesmr8ZWKin8Kl63Otu5NAZddLyyaPxaPc"; // localhost
//string apiKey = "7A6Qshz4UwLvFXvBLP2TLyJNUnJT6vetjPjAiDijJy2XFZlE3613MTMGpLEsgbCmG1w0fh6Oe3MKJ6vM0ah"; // manfredrepo-test
HttpClient client = new();
client.DefaultRequestHeaders.Add("apiKey", apiKey);

Console.CancelKeyPress += async (sender, e) =>
{
    e.Cancel = true;
    await Shutdown();
};

Timer? pollTimer = null;

try
{
    await CallEndpoint("/log/start");

    pollTimer = new Timer(async _ => await PollRecent(), null, TimeSpan.Zero, TimeSpan.FromSeconds(2.5));

    Console.WriteLine("Polling... Press Ctrl+C to exit.");
    await Task.Delay(Timeout.Infinite);
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Hiba: {ex.Message}");
}
finally
{
    if (pollTimer != null)
    {
        await pollTimer.DisposeAsync();
    }

    await Shutdown();
}

async Task Shutdown()
{
    await CallEndpoint("/log/stop");
    Environment.Exit(0);
}

async Task CallEndpoint(string path)
{
    var response = await client.PostAsync($"{baseUrl}{path}", null);
    response.EnsureSuccessStatusCode();
}

async Task PollRecent()
{
    try
    {
        var response = await client.GetAsync($"{baseUrl}/log/recent");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var events = JsonSerializer.Deserialize<List<LogEventDto>>(json, options);
        if (events == null) return;

        foreach (var logEvent in events)
        {
            if (logEvent.Message.StartsWith("HTTP \"GET\" \"/log/recent\""))
                continue;
            //if (!logEvent.Message.StartsWith("[IoT ") && !logEvent.Message.StartsWith("Updating rental status"))
            //    continue;
            Console.WriteLine($"{logEvent.Timestamp:yyyy-MM-dd HH:mm:ss}\t{logEvent.Message}");
        }
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"[PollRecent] Hiba: {ex.Message}");
    }
}

// Egyszerű LogEvent DTO a JSON feldolgozáshoz
record LogEventDto
{
    public DateTime Timestamp { get; init; }
    public string Message { get; init; } = "";
}
