using Kavics.LogInterpreter.Abstractions.DefaultImplementations;
using System.Collections;

namespace Kavics.LogInterpreter.Abstractions;

public interface IPipelineItem
{
    Pipeline Pipeline { get; set; }
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

public interface  IAggregation
{
    Task WriteAggregation(CancellationToken cancellationToken = default);
}