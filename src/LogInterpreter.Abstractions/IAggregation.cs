namespace Kavics.LogInterpreter.Abstractions;

public interface IAggregation
{
    Task WriteAggregation(CancellationToken cancellationToken = default);
}