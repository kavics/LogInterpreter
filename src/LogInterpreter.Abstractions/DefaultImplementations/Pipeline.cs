using System.Collections;

namespace Kavics.LogInterpreter.Abstractions.DefaultImplementations;

public class Pipeline
{
    protected IEnumerable? _endpoint;
    public List<IPipelineItem> Items { get; } = new();

    public Pipeline AddItem<TIn, TOut>(IPipelineItem<TIn, TOut> item)
    {
        if (_endpoint == null)
        {
            _endpoint = item;
            Items.Add(item);
            return this;
        }

        if (_endpoint is IEnumerable<TIn> inputItem)
        {
            item.Input = (IEnumerable<TIn>)inputItem;
            _endpoint = item;
            Items.Add(item);
            return this;
        }

        return this;
    }

    public void Run()
    {
        if (_endpoint == null)
            return;

        foreach (var item in _endpoint)
        {
            // do nothing
        }
    }
}
