namespace Lab2;

public sealed class Logger
{
    private static LogLevel _minimumLevel = LogLevel.INFO;
    private readonly Formatter _formatter;
    private readonly ConsoleSink _sink;

    public string Name { get; }
    public static LogLevel MinimumLevel => _minimumLevel;

    /// <summary>Composition: this logger creates and owns its formatter and sink.</summary>
    public Logger(string name)
        : this(name, new Formatter(), new ConsoleSink())
    {
    }

    /// <summary>Aggregation: the caller creates and shares the formatter and sink.</summary>
    public Logger(string name, Formatter formatter, ConsoleSink sink)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("A logger name is required.", nameof(name));
        }

        Name = name;
        _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
        _sink = sink ?? throw new ArgumentNullException(nameof(sink));
    }

    public static void SetLevel(LogLevel minimumLevel)
    {
        if (!Enum.IsDefined(minimumLevel))
        {
            throw new ArgumentOutOfRangeException(nameof(minimumLevel));
        }

        _minimumLevel = minimumLevel;
    }

    public void Log(LogLevel level, string message)
    {
        if (level < _minimumLevel)
        {
            return;
        }

        string line = _formatter.Format(Name, level, message ?? string.Empty);
        _sink.Write(line);
    }

    public void Trace(string message) => Log(LogLevel.TRACE, message);
    public void Info(string message) => Log(LogLevel.INFO, message);
    public void Warn(string message) => Log(LogLevel.WARN, message);
    public void Error(string message) => Log(LogLevel.ERROR, message);
}
