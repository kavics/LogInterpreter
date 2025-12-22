using System.Collections;

namespace Kavics.LogInterpreter.Abstractions;

public interface IPipelineItem
{
    string Name { get; }
}

public interface IPipelineItem<TIn, out TOut> : IEnumerable<TOut>, IPipelineItem
{
    IEnumerable<TIn> Input { get; set; }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
