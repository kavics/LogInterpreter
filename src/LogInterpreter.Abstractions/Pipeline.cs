using System.Collections;

namespace Kavics.LogInterpreter.Abstractions;

public class Pipeline
{
    protected IEnumerable? _endpoint;
    public List<IPipelineItem> Items { get; } = new();

    public List<IAggregation> Aggregations { get; } = new();

    public TimeSpan ProcessingTime { get; set; } = TimeSpan.Zero;

    public Pipeline AddItem<TIn, TOut>(IPipelineItem<TIn, TOut> item)
    {
        item.Pipeline = this;
        if (item is IAggregation aggregation)
        {
            Aggregations.Add(aggregation);
        }

        if (_endpoint == null)
        {
            _endpoint = item;
            Items.Add(item);
            return this;
        }

        if (_endpoint is IEnumerable<TIn> inputItem)
        {
            item.Input = inputItem;
            _endpoint = item;
            Items.Add(item);
            return this;
        }

        return this;
    }

    public TextWriter GetConsole()
    {
        return Console.Out;
    }

    public void Run()
    {
        if (_endpoint == null)
            return;

        var timer = System.Diagnostics.Stopwatch.StartNew();

        foreach (var item in _endpoint)
        {
            // do nothing
        }

        timer.Stop();
        ProcessingTime = timer.Elapsed;

        foreach (var aggregation in Aggregations)
        {
            aggregation.WriteAggregation().GetAwaiter().GetResult();
        }

    }

    public Dictionary<string, object> Counters = new();

    internal void AddToCounter<T>(string key, T value)
    {
        if (Counters.ContainsKey(key))
        {
            Counters[key] = (dynamic)Counters[key] + value;
        }
        else
        {
            Counters[key] = value;
        }
    }
}
