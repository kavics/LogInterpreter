using Kavics.LogInterpreter.Abstractions;
using Kavics.LogInterpreter.Abstractions.DefaultImplementations;
using System.Reflection;

namespace LogInterpreter.Tests;

[TestClass]
public class PipelineParserTests
{
    [TestMethod]
    public void PipelineParser_SimplePipeline()
    {
        var pipelineDefinition = @"
LogSource
    LogPath = /var/log/app.log
OneLineLogFileReader
CompactJsonLogParser
ErrorAggregator
    AggregationFileName = /var/log/aggregation.txt
";
        var pipeline = Pipeline.Parse(pipelineDefinition);
        Assert.IsNotNull(pipeline);
        Assert.AreEqual(4, pipeline.Items.Count);
        Assert.IsInstanceOfType(pipeline.Items[0], typeof(LogSource));
        var logSource = (LogSource)pipeline.Items[0];
        Assert.AreEqual("/var/log/app.log", logSource.LogPath);
        Assert.AreEqual(null, logSource.FirstFileName);
        Assert.IsInstanceOfType(pipeline.Items[1], typeof(OneLineLogFileReader));
        Assert.IsInstanceOfType(pipeline.Items[2], typeof(CompactJsonLogParser));
        Assert.IsInstanceOfType(pipeline.Items[3], typeof(ErrorAggregator));
        var errorAggregator = (ErrorAggregator)pipeline.Items[3];
        Assert.AreEqual("/var/log/aggregation.txt", errorAggregator.AggregationFileName);
    }
    [TestMethod]
    public void PipelineParser_SimplePipeline_ToString()
    {
        var definition = @"LogSource
    LogPath=/var/log/app.log
OneLineLogFileReader
CompactJsonLogParser
ErrorAggregator
    AggregationFileName=/var/log/aggregation.txt";

        var pipeline = Pipeline.Parse(definition);
        var parsed = pipeline.ToString();
        Assert.AreEqual(definition, parsed);
    }
    [TestMethod]
    public void PipelineParser_AllButNoFunctions_ToString()
    {
        // Filter, Formatter, Transformer are not included.

        var definition1 = @"LogSource
    LogPath=/var/log/app.log
    FirstFileName=/var/log/app-2025-12-01.txt
OneLineLogFileReader
ConsoleWriter
FileWriter
    FilePath=/var/log/output.txt
CompactJsonLogParser
ErrorAggregator
    AggregationFileName=/var/log/aggregation.txt
EntryCounter
    AggregationFileName=/var/log/summary.txt";

        var pipeline1 = Pipeline.Parse(definition1);
        var parsed1 = pipeline1.ToString();
        Assert.AreEqual(definition1, parsed1);

        var definition2 = @"LogSource
    LogPath=/var/log/app.log
    FirstFileName=/var/log/app-2025-12-01.txt
ConsoleWriter
FileWriter
    FilePath=/var/log/output.txt
TwoLineLogReader
TwoLineLogParser
ErrorAggregator
    AggregationFileName=/var/log/aggregation.txt
EntryCounter
    AggregationFileName=/var/log/summary.txt";

        var pipeline2 = Pipeline.Parse(definition2);
        var parsed2 = pipeline2.ToString();
        Assert.AreEqual(definition2, parsed2);
    }
    [TestMethod]
    public void PipelineParser_CategoryFilter_ToString()
    {
        var definition = @"LogSource
    LogPath=/var/log/app.log
OneLineLogFileReader
CompactJsonLogParser
CategoryFilter
    Include=Error,Warning
    Exclude=";

        var pipeline = Pipeline.Parse(definition);
        var filter = pipeline.Items[3] as CategoryFilter;
        Assert.IsNotNull(filter);
        CollectionAssert.AreEquivalent(
            new[] { "Error", "Warning" }, filter.IncludeCategoryNames.ToArray());
        var parsed = pipeline.ToString();
        Assert.AreEqual(definition, parsed);
    }

    [TestMethod]
    public void PipelineParser_Error()
    {
        var definition = @"LogSource
    LogPath=/var/log/app.log
    FirstFileName=/var/log/app-2025-12-01.txt
TwoLineLogReader
ConsoleWriter
FileWriter
    FilePath=/var/log/output.txt
TwoLineLogParser
ErrorAggregator
    AggregationFileName=/var/log/aggregation.txt
EntryCounter
    AggregationFileName=/var/log/summary.txt";

        try
        {
            var _ = Pipeline.Parse(definition);
            Assert.Fail("Expected exception not thrown.");
        }
        catch (TargetInvocationException ex) when (ex.InnerException is PipelineItemMismatchException)
        {
            var innerEx = (PipelineItemMismatchException)ex.InnerException;
            StringAssert.Contains(innerEx.Message, "Pipeline item type mismatch");
            StringAssert.Contains(innerEx.Message, "TwoLineLogReader");
            StringAssert.Contains(innerEx.Message, "ConsoleWriter");
        }
        catch (Exception)
        {
            Assert.Fail("Expected TargetInvocationException with inner PipelineItemMismatchException.");
        }
    }
}
