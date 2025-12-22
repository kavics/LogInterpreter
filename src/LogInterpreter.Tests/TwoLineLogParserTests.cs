using Kavics.LogInterpreter.Abstractions;
using Kavics.LogInterpreter.Abstractions.DefaultImplementations;

namespace LogInterpreter.Tests;

[TestClass]
public class TwoLineLogParserTests
{
    [TestMethod]
    public void TwoLineLogParser_NotRecognized()
    {
        var aggregator = new TestAggregator<LogEntry>();

        var logLines = new[]
        {
            "line0",
        };

        var pipeline = new Pipeline()
            .AddItem(new TestLogSource { Input = logLines })
            .AddItem(new TwoLineLogReader())
            .AddItem(new TwoLineLogParser())
            .AddItem(aggregator);

        // ACT
        pipeline.Run();

        // ASSERT
        Assert.AreEqual(1, aggregator.Output.Count);
        Assert.AreEqual("not recognized entry", aggregator.Output[0].Message);
    }

    [TestMethod]
    public void TwoLineLogParser_MinimalEntry()
    {
        var aggregator = new TestAggregator<LogEntry>();

        var logLines = new[]
        {
            "2025-05-05 04:03:02.987 +00:00 [...] line0",
        };

        var pipeline = new Pipeline()
            .AddItem(new TestLogSource { Input = logLines })
            .AddItem(new TwoLineLogReader())
            .AddItem(new TwoLineLogParser())
            .AddItem(aggregator);

        // ACT
        pipeline.Run();

        // ASSERT
        Assert.AreEqual(1, aggregator.Output.Count);
        var entry = aggregator.Output[0];
        Assert.AreEqual(DateTime.Parse("2025-05-05 04:03:02.987 +00:00"), entry.Time);
        Assert.AreEqual(LogLevel.NotParsed, entry.Level);
        Assert.AreEqual("line0", entry.Message);
        Assert.IsNotNull(entry.Properties);
        Assert.AreEqual(0, entry.Properties.Count);
    }

    [TestMethod]
    public void TwoLineLogParser_TimeLevelMessageProperties()
    {
        var aggregator = new TestAggregator<LogEntry>();

        var logLines = new[]
        {
            @"2025-05-05 04:03:02.987 +00:00 [...] line0",
            @"{ ""a"": 42}",
        };

        var pipeline = new Pipeline()
            .AddItem(new TestLogSource { Input = logLines })
            .AddItem(new TwoLineLogReader())
            .AddItem(new TwoLineLogParser())
            .AddItem(aggregator);

        // ACT
        pipeline.Run();

        // ASSERT
        Assert.AreEqual(1, aggregator.Output.Count);
        var entry = aggregator.Output[0];
        Assert.AreEqual(DateTime.Parse("2025-05-05 04:03:02.987 +00:00"), entry.Time);
        Assert.AreEqual(LogLevel.NotParsed, entry.Level);
        Assert.AreEqual("line0", entry.Message);
        Assert.IsNotNull(entry.Properties);
        Assert.AreEqual(1, entry.Properties.Count);
        Assert.AreEqual("a", entry.Properties.Keys.First());
        Assert.AreEqual("42", entry.Properties["a"]);
    }

    [TestMethod]
    public void TwoLineLogParser_Properties_ComplexString()
    {
        var aggregator = new TestAggregator<LogEntry>();

        var logLines = new[]
        {
            @"2025-06-19 03:49:55.659 +02:00 [DBG] Message (19.05918271, 47.46739892)",
            @"{ SourceContext: ""Services.LocationService"", UserId: 1342, Username: ""Public\user1"", Location: ""(19.05918271, 47.46739892)"", RequestId: ""0HNDEQDUMEMEJ:00000001"", EnvironmentName: ""Development"" }",
        };

        var pipeline = new Pipeline()
            .AddItem(new TestLogSource { Input = logLines })
            .AddItem(new TwoLineLogReader())
            .AddItem(new TwoLineLogParser())
            .AddItem(aggregator);

        // ACT
        pipeline.Run();

        // ASSERT
        Assert.AreEqual(1, aggregator.Output.Count);
        var entry = aggregator.Output[0];
        Assert.AreEqual(DateTime.Parse("2025-06-19 03:49:55.659 +02:00"), entry.Time);
        Assert.AreEqual(LogLevel.Debug, entry.Level);
        Assert.AreEqual("Message (19.05918271, 47.46739892)", entry.Message);
        Assert.IsNotNull(entry.Properties);
        var properties = entry.Properties.ToArray();
        Assert.AreEqual(6, properties.Length);
        Assert.AreEqual("SourceContext", properties[0].Key);
        Assert.AreEqual("Services.LocationService", properties[0].Value);
        Assert.AreEqual("UserId", properties[1].Key);
        Assert.AreEqual("1342", properties[1].Value);
        Assert.AreEqual("Username", properties[2].Key);
        Assert.AreEqual("Public\\user1", properties[2].Value);
        Assert.AreEqual("Location", properties[3].Key);
        Assert.AreEqual("(19.05918271, 47.46739892)", properties[3].Value);
        Assert.AreEqual("RequestId", properties[4].Key);
        Assert.AreEqual("0HNDEQDUMEMEJ:00000001", properties[4].Value);
        Assert.AreEqual("EnvironmentName", properties[5].Key);
        Assert.AreEqual("Development", properties[5].Value);
    }
}