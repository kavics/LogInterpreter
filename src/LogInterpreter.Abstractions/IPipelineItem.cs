using System.Collections;

namespace LogInterpreter.Abstractions;

public interface IPipelineItem<TIn, out TOut> : IEnumerable<TOut>
{
    IEnumerable<TIn> Input { get; set; }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
