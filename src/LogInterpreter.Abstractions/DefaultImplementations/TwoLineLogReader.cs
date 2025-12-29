using System.Globalization;

namespace Kavics.LogInterpreter.Abstractions.DefaultImplementations;

public class TwoLineLogReader : IPipelineItem<string, string[]>
{
    public Pipeline Pipeline { get; set; } = null!;
    public string Name => this.GetType().Name;
    public IEnumerable<string> Input { get; set; } = Array.Empty<string>();

    public IEnumerator<string[]> GetEnumerator()
    {
        var lineBuffer = new List<string>();
        var twoLine = new string[2];

        var isOdataMiddlewareError = false;
        foreach (var line in Input)
        {
            //UNDONE: REMOVE Special handling for ODataMiddleware errors that are multiline JSON objects
            if (line.StartsWith("{ SourceContext: \"SenseNet.OData.ODataMiddleware\","))
            {
                isOdataMiddlewareError = true;
                twoLine[1] = line;
                continue;
            }

            if (isOdataMiddlewareError)
            {
                // read lines to buffer while the current line starts with timestamp.
                if (IsTimestamp(line))
                {
                    twoLine[0] = string.Join("\n", lineBuffer);
                    isOdataMiddlewareError = false;
                    lineBuffer.Clear();
                    lineBuffer.Add(line); // start line of the next entry
                    yield return twoLine.ToArray();
                }
                else
                {
                    lineBuffer.Add(line);
                }
            }
            else
            {
                // read lines to buffer while the current line starts a json object.
                if (line.Length > 0 && line[0] == '{')
                {
                    twoLine[0] = string.Join("\n", lineBuffer);
                    twoLine[1] = line;
                    lineBuffer.Clear();
                    yield return twoLine.ToArray();
                }
                else
                {
                    lineBuffer.Add(line);
                }
            }

        }

        // Output remaining lines as a truncated entry
        if (lineBuffer.Count > 0)
        {
            // Do not emit last entry if it is empty
            if (lineBuffer.All(x=>x.Length == 0))
                yield break;

            twoLine[0] = string.Join("\n", lineBuffer);
            twoLine[1] = "{}";
            yield return twoLine.ToArray();
        }
    }

    private bool IsTimestamp(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        ReadOnlySpan<char> span = input.AsSpan();

        var bracketIndex = span.IndexOf('[');
        if (bracketIndex == -1)
            return false;

        ReadOnlySpan<char> dateSpan = span.Slice(0, bracketIndex).TrimEnd();

        return DateTimeOffset.TryParse(dateSpan, CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
    }
}