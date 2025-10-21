using System.Globalization;

namespace LogInterpreter.Abstractions.DefaultImplementations;

public class TwoLineLogParser : IPipelineItem<string[], LogEntry>
{
    public string Name => this.GetType().Name;

    private LogEntry _notRecognized = new LogEntry { Message = "not recognized entry" };
    private LogEntry _notRecognizedDate = new LogEntry { Message = "not recognized date" };

    private LogEntry Parse(string?[]? lines)
    {
        if(lines == null)
            return _notRecognized;
        if (lines.Length == 0)
            return _notRecognized;
        if (lines[0] == null)
            return _notRecognized;

        // 2025-03-30 00:08:06.279 +00:00 [VRB] 326	ContentOperation	Pf:54	Op:76	End	00:00:00.333804	NODE.SAVE Id: 1158, VersionId: 171, Version: V1.0.A, Name: DatabaseUsage.cache, ParentPath: /Root/System/Cache 
        // { SourceContext: "SenseNet.Diagnostics.SnILoggerTracer", Application: "Manfred backend", Repository: "", Version: "1.0.0", MachineName: "manfredrepo-test-sensenet-cloud-5756d9f84f-66g8p", EnvironmentName: "Production" }
        var pLevel = lines[0]!.IndexOf('[', StringComparison.Ordinal);
        if (pLevel < 0)
        {
            /*
               2025-03-28 02:15:59.036 +00:00 [ERR] Timestamp: 03/28/2025 02:15:59
               Message: Invalid resource: /Root/Localization/Erros.xml
               Root element is missing.
               Category: General
               Priority: -1
               EventId: 1
               Severity: Error
               Title: 
               Machine: manfredrepo-test-sensenet-cloud-76fcb4769c-5z7dz
               Application Domain: manfredrepo-test-sensenet-cloud-76fcb4769c-5z7dz
               Process Id: 1
               Process Name: dotnet
               Managed Thread Id: 161
               Thread Name: .NET TP Worker
               Extended Properties: Messages - XmlException: Root element is missing.
                  at System.Xml.XmlTextReaderImpl.Throw(Exception e)
                  at System.Xml.XmlTextReaderImpl.ThrowWithoutLineInfo(String res)
                  at System.Xml.XmlTextReaderImpl.ParseDocumentContent()
                  at System.Xml.XmlLoader.Load(XmlDocument doc, XmlReader reader, Boolean preserveWhitespace)
                  at System.Xml.XmlDocument.Load(XmlReader reader)
                  at System.Xml.XmlDocument.Load(Stream inStream)
                  at SenseNet.ContentRepository.i18n.SenseNetResourceManager.ParseAll(IEnumerable`1 nodes)
               =====================
               
               SnTrace - #970ab9e3-8619-4cb5-b511-2244c20d3ab5 
               { EventId: { Id: 1 }, SourceContext: "SenseNet.Diagnostics.SnILogger", RequestId: "0HNBDJNQVUD1U:0000000A", RequestPath: "/OData.svc/('Root')/Import", ConnectionId: "0HNBDJNQVUD1U", Application: "Manfred backend", Repository: "", Version: "1.0.0", MachineName: "manfredrepo-test-sensenet-cloud-76fcb4769c-5z7dz", EnvironmentName: "Production" }
 
             
SenseNet.OData.ODataException: Unexpected character encountered while parsing value: k. Path '', line 0, position 0.
 ---> Newtonsoft.Json.JsonReaderException: Unexpected character encountered while parsing value: k. Path '', line 0, position 0.
   at Newtonsoft.Json.JsonTextReader.ParseValue()
   at Newtonsoft.Json.JsonReader.ReadForType(JsonContract contract, Boolean hasConverter)
   at Newtonsoft.Json.Serialization.JsonSerializerInternalReader.Deserialize(JsonReader reader, Type objectType, Boolean checkAdditionalContent)
   at Newtonsoft.Json.JsonSerializer.DeserializeInternal(JsonReader reader, Type objectType)
   at SenseNet.OData.ODataMiddleware.ReadToJson(String models)
   at SenseNet.OData.ODataMiddleware.ReadToJsonAsync(HttpContext context)
   at SenseNet.OData.DefaultActionResolver.GetMethodBasedAction(String name, Content content, Object state)
   at SenseNet.ApplicationModel.ActionFactory.CreateAction(String actionType, Application application, Content context, String backUri, Object parameters, Func`4 getDefaultAction, Object state)
   at SenseNet.OData.Writers.ODataWriter.WritePostOperationResultAsync(HttpContext httpContext, ODataRequest odataReq, IConfiguration appConfig)
   at SenseNet.OData.ODataMiddleware.ProcessRequestAsync(HttpContext httpContext, ODataRequest odataRequest)
   --- End of inner exception stack trace ---
2025-04-16 14:35:01.064 +00:00 [ERR] HTTP POST /odata.svc/('Root')/Users/ForgotPassword responded 500 in 11.7826 ms 
{ UserId: 6, Username: "BuiltIn\Visitor", SourceContext: "Serilog.AspNetCore.RequestLoggingMiddleware", Application: "Manfred backend", Repository: "", Version: "0.9.4.0", RequestId: "0HNBSREHLP1TJ:00000005", ConnectionId: "0HNBSREHLP1TJ", MachineName: "manfredrepo-test-sensenet-cloud-5b796955f-xwsct", EnvironmentName: "Production" }
             */
            return _notRecognized;
        }

        var span = lines[0].AsSpan();

        if (!DateTime.TryParse(span.Slice(0, pLevel - 1),
                CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal,
                out var dateTime))
            return _notRecognizedDate;

        var entry = new LogEntry
        {
            Time = dateTime,
            Level = ParseLevel(span.Slice(pLevel, 5)),
            Message = ParseMessage(span, pLevel + 6),
            Raw = lines!
        };

        if (lines[0]!.Contains(Environment.NewLine))
            ParseMultilineTraceEntry(entry, entry.Raw[0].Substring(pLevel + 6));
        else
            ParseSnTraceEntry(entry, entry.Raw[0].Substring(pLevel + 6));

        ParseProperties(entry, lines[1]);

        entry.Message ??= string.Empty;

        return entry;
    }

    private LogLevel ParseLevel(ReadOnlySpan<char> levelString)
    {
        var slice = levelString.Slice(1, 3);
        switch (slice)
        {
            case "DBG": return LogLevel.Debug;
            case "VRB": return LogLevel.Trace;
            case "INF": return LogLevel.Information;
            case "WRN": return LogLevel.Warning;
            case "ERR": return LogLevel.Error;
            case "CRT": return LogLevel.Critical;
            case "FAT": return LogLevel.Critical;
            default: return LogLevel.NotParsed;
        }
    }

    private string ParseMessage(ReadOnlySpan<char> input, int startPos)
    {
        // Megkeressük az első újsor karaktert (\r vagy \n)
        int newLineIndex = input.IndexOfAny('\r', '\n');

        // Ha nincs sortörés, az egész input az első sor
        int lineEnd = newLineIndex >= 0 ? newLineIndex : input.Length;

        // Ha a startPos túl van a sor végén, üres span-t adunk vissza
        if (startPos >= lineEnd)
            return string.Empty;

        return input.Slice(startPos, lineEnd - startPos).ToString();
    }

    private enum Field
    {
        /// <summary>Value = 0</summary>
        LineId = 0,
        /// <summary>Value = 1</summary>
        Category,
        /// <summary>Value = 2</summary>
        ProgramFlowId,
        /// <summary>Value = 3</summary>
        OpId,
        /// <summary>Value = 4</summary>
        Status,
        /// <summary>Value = 5</summary>
        Duration,
        /// <summary>Value = 6</summary>
        Message
    }


    private void ParseMultilineTraceEntry(LogEntry entry, string data)
    {
        var src = data.Split(Environment.NewLine);
        var messageLine = src.Where(s => s.StartsWith("Message:")).FirstOrDefault();
        if (messageLine != null)
            entry.Message = messageLine.Substring(9);
        //TODO: Extended Properties:??
    }

    public static void ParseSnTraceEntry(LogEntry entry, string? line)
    {
        if (string.IsNullOrEmpty(line))
            return;
        if (line.StartsWith("--") || line.StartsWith("MaxPdiff:", StringComparison.OrdinalIgnoreCase))
            return;

        var data = line.Split('\t');
        if (data.Length < (int)Field.Message)
            return;

        entry.LineId = ParseLineId(data[(int) Field.LineId]);
        entry.Category = data[(int) Field.Category];
        entry.ProgramFlowId = ParseProgramFlow(data[(int) Field.ProgramFlowId]);
        entry.OpId = ParseOperationId(data[(int) Field.OpId]);
        entry.Status = data[(int) Field.Status];
        if(entry.Status == "ERROR")
            entry.Level = LogLevel.Error;
        entry.Duration = ParseDuration(data[(int) Field.Duration]);
        entry.Message = string.Join("\t", data.Skip((int) Field.Message));
    }
    private static int ParseLineId(string src)
    {
        if (string.IsNullOrEmpty(src))
            return 0;
        return int.TryParse(src.StartsWith(">") ? src.AsSpan(1) : src.AsSpan(), out var result)
            ? result : 0;
    }
    private static long ParseProgramFlow(string src)
    {
        if (src.Length == 0)
            return 0L;
        return long.TryParse(src.StartsWith("Pf:") ? src.AsSpan(3) : src.AsSpan(), out var result)
            ? result : 0L;
    }
    private static int ParseOperationId(string src)
    {
        if(src.Length==0)
            return 0;
        return int.TryParse(src.StartsWith("Op:") ? src.AsSpan(3) : src.AsSpan(), out var result)
            ? result : 0;
    }
    private static TimeSpan ParseDuration(string src)
    {
        return src.Length == 0 ? default :
            TimeSpan.TryParse(src, CultureInfo.InvariantCulture, out var result)
                ? result : default;
    }

    private void ParseProperties(LogEntry entry, string? line)
    {
        if (line == null)
            return;
        try
        {
            var properties = LightweightJsonParser.ParseFlatObject(line.AsSpan().Trim());
            entry.Properties = properties;
        }
        catch(Exception e)
        {
            entry.Properties = new Dictionary<string, string>
            {
                { "CannotParsePropoerties", e.Message}
            };

        }
    }

    public IEnumerable<string[]> Input { get; set; } = new List<string[]>();

    public IEnumerator<LogEntry> GetEnumerator()
    {
        foreach (var lines in Input)
        {
            yield return Parse(lines);
        }
    }
}


public static class LightweightJsonParser
{
    public static Dictionary<string, string> ParseFlatObject(ReadOnlySpan<char> input)
    {
        var result = new Dictionary<string, string>();

        input = input.Trim();

        if (input.StartsWith("{".AsSpan())) input = input.Slice(1);
        if (input.EndsWith("{".AsSpan())) input = input.Slice(0, input.Length - 1);

        int pos = 0;

        while (pos < input.Length)
        {
            // Kulcs olvasása a ':'-ig
            int colonIndex = input.Slice(pos).IndexOf(':');
            if (colonIndex == -1) break;

            ReadOnlySpan<char> keySpan = input.Slice(pos, colonIndex).Trim();
            pos += colonIndex + 1;

            // Érték olvasása a következő ','-ig vagy a végéig
            int commaIndex = input.Slice(pos).IndexOf(',');
            ReadOnlySpan<char> valueSpan;
            if (commaIndex == -1)
            {
                valueSpan = input.Slice(pos).Trim();
                pos = input.Length;
            }
            else
            {
                valueSpan = input.Slice(pos, commaIndex).Trim();
                pos += commaIndex + 1;
            }

            // Idézőjelek eltávolítása kulcsról és értékről, ha vannak
            string key = RemoveSurroundingQuotes(keySpan);
            string value = RemoveSurroundingQuotes(valueSpan);

            result[key] = value;
        }

        return result;
    }

    private static string RemoveSurroundingQuotes(ReadOnlySpan<char> span)
    {
        if (span.Length >= 2 && span[0] == '"' && span[^1] == '"')
            return span.Slice(1, span.Length - 2).ToString();
        else
            return span.ToString();
    }
}
