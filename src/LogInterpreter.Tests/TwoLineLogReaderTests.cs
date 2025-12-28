using System.Collections;
using Kavics.LogInterpreter.Abstractions;
using Kavics.LogInterpreter.Abstractions.DefaultImplementations;

namespace LogInterpreter.Tests;

internal class TestLogSource : IPipelineItem<string, string>
{
    public Pipeline Pipeline { get; set; } = null!;
    public string Name => this.GetType().Name;

    public IEnumerable<string> Input { get; set; }

    public IEnumerator<string> GetEnumerator() => Input.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

internal class TestAggregator<T> : IPipelineItem<T, T>
{
    public Pipeline Pipeline { get; set; } = null!;
    public string Name => this.GetType().Name;

    public IEnumerable<T> Input { get; set; }
    public List<T> Output { get; set; } = new List<T>();

    public IEnumerator<T> GetEnumerator()
    {
        foreach (T item in Input)
        {
            Output.Add(item);
            if (item is Array array)
                yield return (T)array.Clone();
            yield return item;
        }
    }
}


[TestClass]
public sealed class TwoLineLogReaderTests
{
    [TestMethod]
    public void TwoLineLogReader_1line_Empty()
    {
        var aggregator = new TestAggregator<string[]>();

        var logLines = new[]
        {
            "",
        };

        var pipeline = new Pipeline()
            .AddItem(new TestLogSource { Input = logLines })
            .AddItem(new TwoLineLogReader())
            .AddItem(aggregator);

        // ACT
        pipeline.Run();

        // ASSERT
        Assert.AreEqual(0, aggregator.Output.Count);
    }
    [TestMethod]
    public void TwoLineLogReader_1line_first()
    {
        var aggregator = new TestAggregator<string[]>();

        var logLines = new[]
        {
            "line0",
        };

        var pipeline = new Pipeline()
            .AddItem(new TestLogSource { Input = logLines })
            .AddItem(new TwoLineLogReader())
            .AddItem(aggregator);

        // ACT
        pipeline.Run();

        // ASSERT
        Assert.AreEqual(1, aggregator.Output.Count);
        Assert.AreEqual("line0", aggregator.Output[0][0]);
        Assert.AreEqual("{}", aggregator.Output[0][1]);
    }
    [TestMethod]
    public void TwoLineLogReader_1line_second()
    {
        var aggregator = new TestAggregator<string[]>();

        var logLines = new[]
        {
            "{ line1: 42}",
        };
        var pipeline = new Pipeline()
            .AddItem(new TestLogSource { Input = logLines })
            .AddItem(new TwoLineLogReader())
            .AddItem(aggregator);

        // ACT
        pipeline.Run();

        // ASSERT
        Assert.AreEqual(1, aggregator.Output.Count);
        Assert.AreEqual(string.Empty, aggregator.Output[0][0]);
        Assert.AreEqual("{ line1: 42}", aggregator.Output[0][1]);
    }

    [TestMethod]
    public void TwoLineLogReader_2lines_RightOrder()
    {
        var aggregator = new TestAggregator<string[]>();

        var logLines = new[]
        {
            "line0",
            "{ line1: 42}",
        };

        var pipeline = new Pipeline()
            .AddItem(new TestLogSource { Input = logLines })
            .AddItem(new TwoLineLogReader())
            .AddItem(aggregator);

        // ACT
        pipeline.Run();

        // ASSERT
        Assert.AreEqual(1, aggregator.Output.Count);
        Assert.AreEqual("line0", aggregator.Output[0][0]);
        Assert.AreEqual("{ line1: 42}", aggregator.Output[0][1]);
    }
    [TestMethod]
    public void TwoLineLogReader_2lines_WrongOrder()
    {
        var aggregator = new TestAggregator<string[]>();

        var logLines = new[]
        {
            "{ line1: 42}",
            "line0",
        };

        var pipeline = new Pipeline()
            .AddItem(new TestLogSource { Input = logLines })
            .AddItem(new TwoLineLogReader())
            .AddItem(aggregator);

        // ACT
        pipeline.Run();

        // ASSERT
        Assert.AreEqual(2, aggregator.Output.Count);
        Assert.AreEqual("", aggregator.Output[0][0]);
        Assert.AreEqual("{ line1: 42}", aggregator.Output[0][1]);
        Assert.AreEqual("line0", aggregator.Output[1][0]);
        Assert.AreEqual("{}", aggregator.Output[1][1]);
    }

    [TestMethod]
    public void TwoLineLogReader_3lines_1()
    {
        var aggregator = new TestAggregator<string[]>();

        var logLines = new[]
        {
            "line0",
            "{ line1: 42}",
            "line2",
        };

        var pipeline = new Pipeline()
            .AddItem(new TestLogSource { Input = logLines })
            .AddItem(new TwoLineLogReader())
            .AddItem(aggregator);

        // ACT
        pipeline.Run();

        // ASSERT
        Assert.AreEqual(2, aggregator.Output.Count);
        Assert.AreEqual("line0", aggregator.Output[0][0]);
        Assert.AreEqual("{ line1: 42}", aggregator.Output[0][1]);
        Assert.AreEqual("line2", aggregator.Output[1][0]);
        Assert.AreEqual("{}", aggregator.Output[1][1]);
    }
    [TestMethod]
    public void TwoLineLogReader_3lines_2()
    {
        var aggregator = new TestAggregator<string[]>();

        var logLines = new[]
        {
            "{ line1: 42}",
            "line2",
            "{ line3: 43}",
        };

        var pipeline = new Pipeline()
            .AddItem(new TestLogSource { Input = logLines })
            .AddItem(new TwoLineLogReader())
            .AddItem(aggregator);

        // ACT
        pipeline.Run();

        // ASSERT
        Assert.AreEqual(2, aggregator.Output.Count);
        Assert.AreEqual("", aggregator.Output[0][0]);
        Assert.AreEqual("{ line1: 42}", aggregator.Output[0][1]);
        Assert.AreEqual("line2", aggregator.Output[1][0]);
        Assert.AreEqual("{ line3: 43}", aggregator.Output[1][1]);
    }

    [TestMethod]
    public void TwoLineLogReader_8RealLines()
    {
        var aggregator = new TestAggregator<string[]>();

        var logLines = new[]
        {
@"2025-04-23 10:34:29.064 +00:00 [VRB] 11467	Index	Pf:222	Op:3502	Start		DocumentPopulator.CommitPopulateNode. Version: V1.0.A, VersionId: 254, Path: /Root/Localization/CameraScreen.xml ",
@"{ SourceContext: ""SenseNet.Diagnostics.SnILoggerTracer"", RequestId: ""0HNC2A7SUIJ3G:00000007"", RequestPath: ""/OData.svc/('Root')/Import"", ConnectionId: ""0HNC2A7SUIJ3G"", Application: ""#### backend"", Repository: """", Version: ""0.9.5.0"", MachineName: ""####repo-test-sensenet-cloud-7bfc9bc8c4-nd89c"", EnvironmentName: ""Production"" }",
@"2025-04-23 10:34:29.074 +00:00 [VRB] 11468	Index	Pf:222				ExecuteDistributedActivity: #299 ",
@"{ SourceContext: ""SenseNet.Diagnostics.SnILoggerTracer"", RequestId: ""0HNC2A7SUIJ3G:00000007"", RequestPath: ""/OData.svc/('Root')/Import"", ConnectionId: ""0HNC2A7SUIJ3G"", Application: ""#### backend"", Repository: """", Version: ""0.9.5.0"", MachineName: ""####repo-test-sensenet-cloud-7bfc9bc8c4-nd89c"", EnvironmentName: ""Production"" }",
@"2025-04-23 10:34:29.085 +00:00 [VRB] 11469	Index	Pf:222	Op:3503	Start		IAQ: EXECUTION: A299. ",
@"{ SourceContext: ""SenseNet.Diagnostics.SnILoggerTracer"", RequestId: ""0HNC2A7SUIJ3G:00000007"", RequestPath: ""/OData.svc/('Root')/Import"", ConnectionId: ""0HNC2A7SUIJ3G"", Application: ""#### backend"", Repository: """", Version: ""0.9.5.0"", MachineName: ""####repo-test-sensenet-cloud-7bfc9bc8c4-nd89c"", EnvironmentName: ""Production"" }",
@"2025-04-23 10:34:29.092 +00:00 [VRB] 11470	Index	Pf:222				LM: AddDocumentActivity: [1241/254], /root/localization/camerascreen.xml. ActivityId:299, ExecutingUnprocessedActivities:False",
@"{ SourceContext: ""SenseNet.Diagnostics.SnILoggerTracer"", RequestId: ""0HNC2A7SUIJ3G:00000007"", RequestPath: ""/OData.svc/('Root')/Import"", ConnectionId: ""0HNC2A7SUIJ3G"", Application: ""#### backend"", Repository: """", Version: ""0.9.5.0"", MachineName: ""####repo-test-sensenet-cloud-7bfc9bc8c4-nd89c"", EnvironmentName: ""Production"" }",
        };
        var pipeline = new Pipeline()
            .AddItem(new TestLogSource{ Input = logLines})
            .AddItem(new TwoLineLogReader())
            .AddItem(aggregator);

        // ACT
        pipeline.Run();

        // ASSERT
        Assert.AreEqual(4, aggregator.Output.Count);
    }

    [TestMethod]
    public void TwoLineLogReader_LongEntry()
    {
        var aggregator = new TestAggregator<string[]>();

        var logLines = new[]
        {
            "line0",
            "{ line1: 42}",
            "line2",
            "line3",
            "line4",
            "line5",
            "line6",
            "{ line7: 42}",
            "line8",
            "{ line9: 42}",
        };

        var pipeline = new Pipeline()
            .AddItem(new TestLogSource { Input = logLines })
            .AddItem(new TwoLineLogReader())
            .AddItem(aggregator);

        // ACT
        pipeline.Run();

        // ASSERT
        Assert.AreEqual(3, aggregator.Output.Count);
        Assert.AreEqual("line0", aggregator.Output[0][0]);
        Assert.AreEqual("{ line1: 42}", aggregator.Output[0][1]);
        Assert.AreEqual("line2\nline3\nline4\nline5\nline6", aggregator.Output[1][0]);
        Assert.AreEqual("{ line7: 42}", aggregator.Output[1][1]);
        Assert.AreEqual("line8", aggregator.Output[2][0]);
        Assert.AreEqual("{ line9: 42}", aggregator.Output[2][1]);
    }

    [TestMethod]
    public void TwoLineLogReader_RealLongEntry()
    {
        var aggregator = new TestAggregator<string[]>();

        var logLines = new[]
        {
@"2025-04-03 03:05:22.157 +02:00 [VRB] 3997	Event	Pf:155				ERROR #1338b0af-1ae8-422b-9331-bc8b1bb1c6de: Invalid resource: /Root/Localization/CustomerSupportScreen.xml..Root element is missing. ",
@"{ SourceContext: ""SenseNet.Diagnostics.SnILoggerTracer"", RequestId: ""0HNBI9DOJ4C5E:00000001"", RequestPath: ""/OData.svc/('Root')/Import"", ConnectionId: ""0HNBI9DOJ4C5E"", Application: ""#### backend"", Repository: """", Version: ""0.9.3.0"", MachineName: ""SNPC016"", EnvironmentName: ""Development"" }",
@"2025-04-03 03:05:22.159 +02:00 [ERR] Timestamp: 2025. 04. 03. 1:05:22",
@"Message: Invalid resource: /Root/Localization/CustomerSupportScreen.xml",
@"Root element is missing.",
@"Category: General",
@"Priority: -1",
@"EventId: 1",
@"Severity: Error",
@"Title: ",
@"Machine: SNPC016",
@"Application Domain: SN",
@"Process Id: 53116",
@"Process Name: ####Backend",
@"Managed Thread Id: 24",
@"Thread Name: .NET TP Worker",
@"Extended Properties: Messages - XmlException: Root element is missing.",
@"  at System.Xml.XmlTextReaderImpl.Throw(Exception e)",
@"  at System.Xml.XmlTextReaderImpl.ThrowWithoutLineInfo(String res)",
@"  at System.Xml.XmlTextReaderImpl.ParseDocumentContent()",
@"  at System.Xml.XmlLoader.Load(XmlDocument doc, XmlReader reader, Boolean preserveWhitespace)",
@"  at System.Xml.XmlDocument.Load(XmlReader reader)",
@"  at System.Xml.XmlDocument.Load(Stream inStream)",
@"  at SenseNet.ContentRepository.i18n.SenseNetResourceManager.ParseAll(IEnumerable`1 nodes)",
@"=====================",
@"",
@"SnTrace - #1338b0af-1ae8-422b-9331-bc8b1bb1c6de ",
@"{ EventId: { Id: 1 }, SourceContext: ""SenseNet.Diagnostics.SnILogger"", RequestId: ""0HNBI9DOJ4C5E:00000001"", RequestPath: ""/OData.svc/('Root')/Import"", ConnectionId: ""0HNBI9DOJ4C5E"", Application: ""#### backend"", Repository: """", Version: ""0.9.3.0"", MachineName: ""SNPC016"", EnvironmentName: ""Development"" }",
@"2025-04-03 03:05:22.164 +02:00 [INF] ResourceManager created: SenseNet.ContentRepository.i18n.SenseNetResourceManager ",
@"{ SourceContext: ""SenseNet.ContentRepository.i18n.SenseNetResourceManager"", RequestId: ""0HNBI9DOJ4C5E:00000001"", RequestPath: ""/OData.svc/('Root')/Import"", ConnectionId: ""0HNBI9DOJ4C5E"", Application: ""#### backend"", Repository: """", Version: ""0.9.3.0"", MachineName: ""SNPC016"", EnvironmentName: ""Development"" }",
        };
        var pipeline = new Pipeline()
            .AddItem(new TestLogSource { Input = logLines })
            .AddItem(new TwoLineLogReader())
            .AddItem(aggregator);

        // ACT
        pipeline.Run();

        // ASSERT
        Assert.AreEqual(3, aggregator.Output.Count);
        Assert.IsTrue(aggregator.Output[0][0].StartsWith(@"2025-04-03 03:05:22.157 +02:00 [VRB]"));
        Assert.IsTrue(aggregator.Output[0][1].StartsWith(@"{ SourceContext: ""SenseNet.Diagnostics.SnILoggerTracer"""));
        Assert.IsTrue(aggregator.Output[1][0].StartsWith(@"2025-04-03 03:05:22.159 +02:00 [ERR]"));
        Assert.IsTrue(aggregator.Output[1][1].StartsWith(@"{ EventId: { Id: 1 }, SourceContext: ""SenseNet.Diagnostics.SnILogger"""));
        Assert.IsTrue(aggregator.Output[2][0].StartsWith(@"2025-04-03 03:05:22.164 +02:00 [INF]"));
        Assert.IsTrue(aggregator.Output[2][1].StartsWith(@"{ SourceContext: ""SenseNet.ContentRepository.i18n.SenseNetResourceManager"""));
    }

    [TestMethod] // After "SenseNet.OData.ODataException..." block there is no line of properties object.
    public void TwoLineLogReader_RealLongEntry_ODataError()
    {
        var aggregator = new TestAggregator<string[]>();

        var logLines = new[]
        {
@"2025-04-23 10:31:52.281 +00:00 [VRB] 2805	Index	Pf:12				WARNING: Creating SnPerFieldAnalyzerWrapper without AnalyzerInfo. Indexing and querying components are incomplete. ",
@"{ SourceContext: ""SenseNet.Diagnostics.SnILoggerTracer"", RequestId: ""0HNC2A7SUIJ2C:00000001"", RequestPath: ""/OData.svc/('Root')/GetClients"", ConnectionId: ""0HNC2A7SUIJ2C"", Application: ""#### backend"", Repository: """", Version: ""0.9.5.0"", MachineName: ""####repo-test-sensenet-cloud-7bfc9bc8c4-nd89c"", EnvironmentName: ""Production"" }",
@"2025-04-23 10:31:52.281 +00:00 [ERR] Object reference not set to an instance of an object. ",
@"{ SourceContext: ""SenseNet.OData.ODataMiddleware"", RequestId: ""0HNC2A7SUIJ2C:00000001"", RequestPath: ""/OData.svc/('Root')/GetClients"", ConnectionId: ""0HNC2A7SUIJ2C"", Application: ""#### backend"", Repository: """", Version: ""0.9.5.0"", MachineName: ""####repo-test-sensenet-cloud-7bfc9bc8c4-nd89c"", EnvironmentName: ""Production"" }",
@"SenseNet.OData.ODataException: Object reference not set to an instance of an object.",
@"---> System.NullReferenceException: Object reference not set to an instance of an object.",
@"  at SenseNet.Search.Lucene29.LuceneSearchManager.GetIndexReaderFrame(Boolean dirty)",
@"  at SenseNet.Search.Lucene29.QueryExecutors.LuceneQueryExecutor.Execute()",
@"  at SenseNet.Search.Lucene29.LucQuery.Execute(IPermissionFilter filter, IQueryContext context)",
@"  at SenseNet.Search.Lucene29.Lucene29LocalQueryEngine.ExecuteQueryAsync(SnQuery query, IPermissionFilter filter, IQueryContext context, CancellationToken cancel)",
@"  at SenseNet.Search.Querying.SnQuery.ExecuteAsync(IQueryContext context, CancellationToken cancel)",
@"  at SenseNet.Search.Querying.SnQuery.QueryAsync(String queryText, IQueryContext context, CancellationToken cancel)",
@"  at SenseNet.Search.ContentQuery.ExecuteAsync(String query, SnQueryContext context, CancellationToken cancel)",
@"  at SenseNet.Search.ContentQuery.ExecuteAsync(CancellationToken cancel)",
@"  at SenseNet.Search.ContentQuery.QueryAsync(String text, QuerySettings settings, CancellationToken cancel, Object[] parameters)",
@"  at SenseNet.ApplicationModel.ApplicationStorage.LoadApps(List`1& appNames, List`1& appList, List`1& scenarioNames)",
@"  at SenseNet.ApplicationModel.ApplicationStorage.get_RootAppNode()",
@"  at SenseNet.ApplicationModel.ApplicationStorage.GetApplicationsInternal(String appName, NodeHead head, String scenarioName, String requestedDevice)",
@"  at SenseNet.ApplicationModel.ApplicationStorage.GetApplicationsInternal(String appName, Content context, String scenarioName, String device)",
@"  at SenseNet.ApplicationModel.ApplicationStorage.GetApplication(String applicationName, Content context, Boolean& existingApplication, String device)",
@"  at SenseNet.ApplicationModel.ActionFramework.GetAction(String name, Content context, String backUri, Object parameters, Func`4 getDefaultAction, Object state)",
@"  at SenseNet.ApplicationModel.ActionFramework.GetAction(String name, Content context, Object parameters, Func`4 getDefaultAction, Object state)",
@"  at SenseNet.OData.DefaultActionResolver.GetAction(Content context, String scenario, String actionName, String backUri, Object parameters, HttpContext httpContext, IConfiguration appConfig)",
@"  at SenseNet.OData.Writers.ODataWriter.WriteGetOperationResultAsync(HttpContext httpContext, ODataRequest odataReq, IConfiguration appConfig)",
@"  at SenseNet.OData.Writers.ODataWriter.WriteContentPropertyAsync(String path, String propertyName, Boolean rawValue, HttpContext httpContext, ODataRequest req, IConfiguration appConfig)",
@"  at SenseNet.OData.ODataMiddleware.ProcessRequestAsync(HttpContext httpContext, ODataRequest odataRequest)",
@"  --- End of inner exception stack trace ---",
@"2025-04-23 10:31:52.294 +00:00 [ERR] HTTP GET /OData.svc/('Root')/GetClients responded 500 in 17.1015 ms ",
@"{ UserId: 1, Username: ""\"", SourceContext: ""Serilog.AspNetCore.RequestLoggingMiddleware"", Application: ""#### backend"", Repository: """", Version: ""0.9.5.0"", RequestId: ""0HNC2A7SUIJ2C:00000001"", ConnectionId: ""0HNC2A7SUIJ2C"", MachineName: ""####repo-test-sensenet-cloud-7bfc9bc8c4-nd89c"", EnvironmentName: ""Production"" }",
@"2025-04-23 10:31:52.354 +00:00 [VRB] 2806	Index	Pf:3				Serialize IndexDocument. VersionId: 67, size: 2723 ",
@"{ SourceContext: ""SenseNet.Diagnostics.SnILoggerTracer"", Application: ""#### backend"", Repository: """", Version: ""0.9.5.0"", MachineName: ""####repo-test-sensenet-cloud-7bfc9bc8c4-nd89c"", EnvironmentName: ""Production"" }",
        };
        var pipeline = new Pipeline()
            .AddItem(new TestLogSource { Input = logLines })
            .AddItem(new TwoLineLogReader())
            .AddItem(aggregator);

        // ACT
        pipeline.Run();

        // ASSERT
        Assert.AreEqual(4, aggregator.Output.Count);
        Assert.IsTrue(aggregator.Output[0][0].StartsWith(@"2025-04-23 10:31:52.281 +00:00 [VRB]"));
        Assert.IsTrue(aggregator.Output[0][1].StartsWith(@"{ SourceContext: ""SenseNet.Diagnostics.SnILoggerTracer"""));
        // special aggregation
        Assert.IsTrue(aggregator.Output[1][0].StartsWith(@"2025-04-23 10:31:52.281 +00:00 [ERR] Object reference not set to an instance of an object."));
        Assert.IsTrue(aggregator.Output[1][0].Contains("SenseNet.OData.ODataException: Object reference "));
        Assert.IsTrue(aggregator.Output[1][0].Contains("---> System.NullReferenceException: Object reference not set "));
        Assert.IsTrue(aggregator.Output[1][0].Contains("at SenseNet.ApplicationModel.ApplicationStorage.get_RootAppNode()"));
        Assert.IsTrue(aggregator.Output[1][0].Contains("--- End of inner exception stack trace ---"));
        Assert.IsTrue(aggregator.Output[1][1].StartsWith(@"{ SourceContext: ""SenseNet.OData.ODataMiddleware"""));

        Assert.IsTrue(aggregator.Output[2][0].StartsWith(@"2025-04-23 10:31:52.294 +00:00 [ERR] HTTP GET /OData.svc/('Root')/GetClients responded 500 in 17.1015 ms"));
        Assert.IsTrue(aggregator.Output[2][1].StartsWith(@"{ UserId: 1, Username: ""\"", SourceContext: ""Serilog.AspNetCore.RequestLoggingMiddleware"""));

        Assert.IsTrue(aggregator.Output[3][0].StartsWith(@"2025-04-23 10:31:52.354 +00:00 [VRB]"));
        Assert.IsTrue(aggregator.Output[3][1].StartsWith(@"{ SourceContext: ""SenseNet.Diagnostics.SnILoggerTracer"""));
    }

    [TestMethod] // After "SenseNet.OData.ODataException..." block there is no line of properties object.
    public void TwoLineLogReader_RealLongEntry_LongEntryAndODataError()
    {
        var aggregator = new TestAggregator<string[]>();

        var logLines = new[]
        {
@"2025-04-26 05:16:22.675 +00:00 [VRB] 32400	Event	Pf:2717				ERROR #fff515ce-2026-4f9c-b7ff-638fe154fb0f: Invalid content query ",
@"{ SourceContext: ""SenseNet.Diagnostics.SnILoggerTracer"", RequestId: ""0HNC2A7SUIKV5:00000001"", RequestPath: ""/odata.svc/('Root')/Users/Login"", ConnectionId: ""0HNC2A7SUIKV5"", Application: ""#### backend"", Repository: """", Version: ""0.9.5.0"", MachineName: ""####repo-test-sensenet-cloud-7bfc9bc8c4-nd89c"", EnvironmentName: ""Production"" }",
@"2025-04-26 05:16:22.677 +00:00 [ERR] Timestamp: 04/26/2025 05:16:22",
@"Message: Invalid content query",
@"Category: General",
@"Priority: -1",
@"EventId: 1",
@"Severity: Error",
@"Title: ",
@"Machine: ####repo-test-sensenet-cloud-7bfc9bc8c4-nd89c",
@"Application Domain: ####repo-test-sensenet-cloud-7bfc9bc8c4-nd89c",
@"Process Id: 1",
@"Process Name: dotnet",
@"Managed Thread Id: 82",
@"Thread Name: .NET TP Worker",
@"Extended Properties: Messages -   : Invalid content query",
@"   at SenseNet.Search.ContentQuery.ExecuteAsync(String query, SnQueryContext context, CancellationToken cancel)",
@"   at SenseNet.Search.ContentQuery.ExecuteAsync(CancellationToken cancel)",
@"   at SenseNet.Search.ContentQuery.QueryAsync(String text, QuerySettings settings, CancellationToken cancel, Object[] parameters)",
@"   at SenseNet.ContentRepository.User.Load(String domain, String name, ExecutionHint hint)",
@"---- Inner Exception:",
@"ParserException: Unexpected '', (query: ""+InTree:""/Root/IMS"" +TypeIs:User +LoginName:"") [Line: 1, Col: 44]",
@"   at SenseNet.Search.Querying.Parser.CqlParser.ParseUnaryTermExp()",
@"   at SenseNet.Search.Querying.Parser.CqlParser.ParseTermExp()",
@"   at SenseNet.Search.Querying.Parser.CqlParser.ParseClause()",
@"   at SenseNet.Search.Querying.Parser.CqlParser.ParseBinaryAnd()",
@"   at SenseNet.Search.Querying.Parser.CqlParser.ParseBinaryOr()",
@"   at SenseNet.Search.Querying.Parser.CqlParser.ParseTopLevelQueryExpList()",
@"   at SenseNet.Search.Querying.Parser.CqlParser.Parse(String queryText, IQueryContext context)",
@"   at SenseNet.Search.Querying.SnQuery.QueryAsync(String queryText, IQueryContext context, CancellationToken cancel)",
@"   at SenseNet.Search.ContentQuery.ExecuteAsync(String query, SnQueryContext context, CancellationToken cancel)",
@"=====================",
@"",
@"SnTrace - #fff515ce-2026-4f9c-b7ff-638fe154fb0f ",
@"{ EventId: { Id: 1 }, SourceContext: ""SenseNet.Diagnostics.SnILogger"", RequestId: ""0HNC2A7SUIKV5:00000001"", RequestPath: ""/odata.svc/('Root')/Users/Login"", ConnectionId: ""0HNC2A7SUIKV5"", Application: ""#### backend"", Repository: """", Version: ""0.9.5.0"", MachineName: ""####repo-test-sensenet-cloud-7bfc9bc8c4-nd89c"", EnvironmentName: ""Production"" }",
@"2025-04-26 05:16:22.688 +00:00 [ERR] Invalid content query ",
@"{ SourceContext: ""SenseNet.OData.ODataMiddleware"", RequestId: ""0HNC2A7SUIKV5:00000001"", RequestPath: ""/odata.svc/('Root')/Users/Login"", ConnectionId: ""0HNC2A7SUIKV5"", Application: ""#### backend"", Repository: """", Version: ""0.9.5.0"", MachineName: ""####repo-test-sensenet-cloud-7bfc9bc8c4-nd89c"", EnvironmentName: ""Production"" }",
@"SenseNet.OData.ODataException: Invalid content query",
@" ---> SenseNet.ContentRepository.Search.InvalidContentQueryException: Invalid content query",
@" ---> SenseNet.Search.Querying.Parser.ParserException: Unexpected '', (query: ""+TypeIs:####User +InTree:""/Root/IMS/Public/####"" +Email:"") [Line: 1, Col: 62]",
@"   at SenseNet.Search.Querying.Parser.CqlParser.ParseUnaryTermExp()",
@"   at SenseNet.Search.Querying.Parser.CqlParser.ParseTermExp()",
@"   at SenseNet.Search.Querying.Parser.CqlParser.ParseClause()",
@"   at SenseNet.Search.Querying.Parser.CqlParser.ParseBinaryAnd()",
@"   at SenseNet.Search.Querying.Parser.CqlParser.ParseBinaryOr()",
@"   at SenseNet.Search.Querying.Parser.CqlParser.ParseTopLevelQueryExpList()",
@"   at SenseNet.Search.Querying.Parser.CqlParser.Parse(String queryText, IQueryContext context)",
@"   at SenseNet.Search.Querying.SnQuery.QueryAsync(String queryText, IQueryContext context, CancellationToken cancel)",
@"   at SenseNet.Search.ContentQuery.ExecuteAsync(String query, SnQueryContext context, CancellationToken cancel)",
@"   --- End of inner exception stack trace ---",
@"   at SenseNet.Search.ContentQuery.ExecuteAsync(String query, SnQueryContext context, CancellationToken cancel)",
@"   at SenseNet.Search.ContentQuery.ExecuteAsync(CancellationToken cancel)",
@"   at SenseNet.BusinessSolutions.Common.BsTools.LoginByNameOrEmailAsync[T](String login, String password, String usersContainerPath, HttpContext httpContext, ILogger logger, CancellationToken cancel) in /src/####Business/BsToolsExperimental/BsTools.cs:line 60",
@"   at ####Business.Controllers.UsersODataController.Login(String user, String password) in /src/####Business/Controllers/UsersODataController.cs:line 574",
@"   at SenseNet.OData.OperationCenter.InvokeAsync(OperationCallingContext context)",
@"   at SenseNet.OData.Writers.ODataWriter.WritePostOperationResultAsync(HttpContext httpContext, ODataRequest odataReq, IConfiguration appConfig)",
@"   at SenseNet.OData.ODataMiddleware.ProcessRequestAsync(HttpContext httpContext, ODataRequest odataRequest)",
@"   --- End of inner exception stack trace ---",
@"2025-04-26 05:16:22.711 +00:00 [ERR] HTTP POST /odata.svc/('Root')/Users/Login responded 500 in 758.9674 ms ",
@"{ UserId: 6, Username: ""BuiltIn\Visitor"", SourceContext: ""Serilog.AspNetCore.RequestLoggingMiddleware"", Application: ""#### backend"", Repository: """", Version: ""0.9.5.0"", RequestId: ""0HNC2A7SUIKV5:00000001"", ConnectionId: ""0HNC2A7SUIKV5"", MachineName: ""####repo-test-sensenet-cloud-7bfc9bc8c4-nd89c"", EnvironmentName: ""Production"" }",
@"2025-04-26 05:16:29.942 +00:00 [INF] Login failed for user ______@____.com ",
@"{ SourceContext: ""####Business.Controllers.UsersODataController"", RequestId: ""0HNC2A7SUIKV5:00000002"", RequestPath: ""/odata.svc/('Root')/Users/Login"", ConnectionId: ""0HNC2A7SUIKV5"", Application: ""#### backend"", Repository: """", Version: ""0.9.5.0"", MachineName: ""####repo-test-sensenet-cloud-7bfc9bc8c4-nd89c"", EnvironmentName: ""Production"" }",
@"",
        };
        var pipeline = new Pipeline()
            .AddItem(new TestLogSource { Input = logLines })
            .AddItem(new TwoLineLogReader())
            .AddItem(aggregator);

        // ACT
        pipeline.Run();

        // ASSERT
        Assert.AreEqual(5, aggregator.Output.Count);
        Assert.IsTrue(aggregator.Output[0][0].StartsWith(@"2025-04-26 05:16:22.675 +00:00 [VRB] 32400"));
        Assert.IsTrue(aggregator.Output[0][1].StartsWith(@"{ SourceContext: ""SenseNet.Diagnostics.SnILoggerTracer"""));

        // long entry
        Assert.IsTrue(aggregator.Output[1][0].StartsWith(@"2025-04-26 05:16:22.677 +00:00 [ERR] Timestamp:"));
        Assert.IsTrue(aggregator.Output[1][0].Contains("Message: Invalid content query"));
        Assert.IsTrue(aggregator.Output[1][0].Contains("Extended Properties: Messages -   : Invalid content query"));
        Assert.IsTrue(aggregator.Output[1][0].Contains("SnTrace - #fff515ce-2026-4f9c-b7ff-638fe154fb0f"));
        Assert.IsTrue(aggregator.Output[1][1].StartsWith(@"{ EventId: { Id: 1 }, SourceContext: ""SenseNet.Diagnostics.SnILogger"""));

        // special aggregation
        Assert.IsTrue(aggregator.Output[2][0].StartsWith(@"2025-04-26 05:16:22.688 +00:00 [ERR] Invalid content query"));
        Assert.IsTrue(aggregator.Output[2][0].Contains("SenseNet.OData.ODataException: Invalid content query"));
        Assert.IsTrue(aggregator.Output[2][0].Contains("---> SenseNet.ContentRepository.Search.InvalidContentQueryException: Invalid content query"));
        Assert.IsTrue(aggregator.Output[2][0].Contains("at SenseNet.Search.Querying.Parser.CqlParser.ParseBinaryOr()"));
        Assert.IsTrue(aggregator.Output[2][0].Contains("--- End of inner exception stack trace ---"));
        Assert.IsTrue(aggregator.Output[2][1].StartsWith(@"{ SourceContext: ""SenseNet.OData.ODataMiddleware"","));

        Assert.IsTrue(aggregator.Output[3][0].StartsWith(@"2025-04-26 05:16:22.711 +00:00 [ERR] HTTP POST /odata.svc/('Root')/"));
        Assert.IsTrue(aggregator.Output[3][1].StartsWith(@"{ UserId: 6, Username: ""BuiltIn\Visitor"", SourceContext: ""Serilog.AspNetCore.RequestLoggingMiddleware"","));

        Assert.IsTrue(aggregator.Output[4][0].StartsWith(@"2025-04-26 05:16:29.942 +00:00 [INF] Login failed for user ______@____.com"));
        Assert.IsTrue(aggregator.Output[4][1].StartsWith(@"{ SourceContext: ""####Business.Controllers.UsersODataController"","));
    }
}
