namespace Lab2;

public readonly record struct NumberedLine(int Number, string Text);

public sealed class CommandReader
{
    private readonly Logger _logger;

    public CommandReader(Logger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IReadOnlyList<NumberedLine>? Read(string[] args)
    {
        string path = args.Length > 0 && !string.IsNullOrWhiteSpace(args[0])
            ? args[0]
            : "shop-monday.txt";

        if (!Path.IsPathRooted(path) && !File.Exists(path))
        {
            string besideExecutable = Path.Combine(AppContext.BaseDirectory, path);
            if (File.Exists(besideExecutable))
            {
                path = besideExecutable;
            }
        }

        try
        {
            if (!File.Exists(path))
            {
                _logger.Error($"Command file '{path}' does not exist. Program stopped without processing commands.");
                return null;
            }

            string[] lines = File.ReadAllLines(path);
            var numberedLines = new List<NumberedLine>(lines.Length);
            for (int i = 0; i < lines.Length; i++)
            {
                numberedLines.Add(new NumberedLine(i + 1, lines[i]));
            }

            return numberedLines;
        }
        catch (IOException exception)
        {
            _logger.Error($"Could not read command file '{path}': {exception.Message}");
            return null;
        }
        catch (UnauthorizedAccessException exception)
        {
            _logger.Error($"Could not read command file '{path}': {exception.Message}");
            return null;
        }
    }
}
