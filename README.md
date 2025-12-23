# LogInterpreter.Abstractions

**LogInterpreter.Abstractions** is a .NET 8 library that provides fundamental interfaces and default implementations for processing log files using a pipeline-based architecture.

## Overview

The library aims to provide a flexible and extensible framework for reading, processing, and writing log files in various formats. The pipeline-based approach enables chaining and reusing different processing steps.

Built on top of `IEnumerable<T>`, the pipeline architecture leverages lazy evaluation and streaming to efficiently process log files of any size. This design allows the library to handle massive datasets without loading entire files into memory, making it suitable for processing gigabyte-sized log files while maintaining a minimal memory footprint. Data flows through the pipeline element by element, enabling real-time processing and output generation even for extremely large log collections.

## Main Components

### Interfaces

#### `IPipelineItem<TIn, TOut>`
The fundamental interface for pipeline elements, enabling chained processing.

```csharp
public interface IPipelineItem<TIn, out TOut> : IEnumerable<TOut>, IPipelineItem
{
    string Name { get; }
    IEnumerable<TIn> Input { get; set; }
}
```

#### `ILogEntry`
Unified representation of log entries. Supports:
- Timestamp (`Time`)
- Log level (`Level`)
- Message (`Message`)
- Raw data (`Raw`)
- Properties (`Properties`)
- SnTrace-specific fields (LineId, Category, ProgramFlowId, OpId, Status, Duration)

### Enumerations

#### `LogLevel`
Log level definitions:
- `NotParsed` - Not parsed
- `Critical` - Critical
- `Error` - Error
- `Warning` - Warning
- `Information` - Information
- `Trace` - Trace
- `Debug` - Debug

#### `ConfigurationType`
Configuration types:
- `Default` - Default
- `Path` - File path

### Attributes

#### `ConfigurableAttribute`
Used to mark properties that can be configured in pipeline elements.

```csharp
[Configurable]
public string PropertyName { get; set; }

[Configurable(ConfigurationType.Path)]
public string FilePath { get; set; }
```

## Default Implementations

Pre-built components available in the `DefaultImplementations` namespace:

### Readers
- **`LogSource`** - Default implementation of log source
- **`OneLineLogFileReader`** - Reading single-line log files
- **`TwoLineLogFileReader`** - Reading two-line log format
- **`TwoLineLogReader`** - Two-line log processor

### Parsers
- **`CompactJsonLogParser`** - Processing compact JSON format logs
- **`TwoLineLogParser`** - Processing two-line format logs

### Processors
- **`Filter`** - Filtering log entries
- **`Transformer`** - Transforming log entries
- **`Formatter`** - Formatting log entries

### Writers
- **`ConsoleWriter`** - Writing to console
- **`FileWriter`** - Writing to file

### Pipeline
- **`Pipeline`** - Connecting and executing pipeline elements

### Model
- **`LogEntry`** - Default implementation of `ILogEntry`

## Usage

### Creating a Pipeline

```csharp
var pipeline = new Pipeline()
    .AddItem(new LogSource { /* configuration */ })
    .AddItem(new TwoLineLogParser())
    .AddItem(new Filter { /* filtering conditions */ })
    .AddItem(new Transformer { /* transformations */ })
    .AddItem(new Formatter { /* formatting */ })
    .AddItem(new ConsoleWriter());

pipeline.Run();
```

### Creating a Custom Pipeline Element

```csharp
public class MyCustomProcessor : IPipelineItem<ILogEntry, ILogEntry>
{
    public string Name => "MyCustomProcessor";
    public IEnumerable<ILogEntry> Input { get; set; }

    [Configurable]
    public string CustomProperty { get; set; }

    public IEnumerator<ILogEntry> GetEnumerator()
    {
        foreach (var entry in Input)
        {
            // Custom processing
            yield return entry;
        }
    }
}
```

## Requirements

- .NET 8.0 or later

## License

This project is available under the repository's license terms.