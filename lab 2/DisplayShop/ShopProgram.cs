using System.Globalization;

namespace Lab2;

public sealed class ShopProgram
{
    private readonly Logger _logger;
    private readonly List<DisplayManager> _managers = [];
    private readonly List<Display> _displays = [];

    public ShopProgram(Logger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void Run(IReadOnlyList<NumberedLine> lines)
    {
        foreach (NumberedLine line in lines)
        {
            if (!ShopCommandParser.TryParse(line, out ShopCommand? command, out string parseError))
            {
                _logger.Error($"Line {line.Number}: {parseError}");
                continue;
            }
            if (command is null)
            {
                continue;
            }

            try
            {
                Execute(command);
            }
            catch (ArgumentException exception)
            {
                _logger.Error($"Line {line.Number}: {exception.Message}");
            }
            catch (InvalidOperationException exception)
            {
                _logger.Error($"Line {line.Number}: {exception.Message}");
            }
        }
    }

    private void Execute(ShopCommand command)
    {
        IReadOnlyList<string> args = command.Arguments;
        switch (command.Name)
        {
            case "MANAGER":
                if (FindManager(args[0]) is not null)
                {
                    throw new InvalidOperationException($"Manager '{args[0]}' already exists.");
                }
                _managers.Add(new DisplayManager(args[0]));
                _logger.Trace($"Created manager '{args[0]}'.");
                break;

            case "DISPLAY":
                if (FindDisplay(args[0]) is not null)
                {
                    throw new InvalidOperationException($"Display model '{args[0]}' already exists.");
                }
                var newDisplay = new Display(
                    args[0],
                    double.Parse(args[1], CultureInfo.InvariantCulture),
                    double.Parse(args[2], CultureInfo.InvariantCulture),
                    double.Parse(args[3], CultureInfo.InvariantCulture));
                _displays.Add(newDisplay);
                _logger.Trace($"Created display '{newDisplay}'.");
                break;

            case "ASSIGN":
            {
                Display assignedDisplay = RequireDisplay(args[0]);
                DisplayManager manager = RequireManager(args[1]);
                if (!manager.AssignDisplay(assignedDisplay))
                {
                    throw new InvalidOperationException($"Display '{args[0]}' is already assigned to manager '{args[1]}'.");
                }
                _logger.Trace($"Assigned display '{args[0]}' to manager '{args[1]}'.");
                break;
            }

            case "COMPARE":
            {
                Display first = RequireDisplay(args[0]);
                Display second = RequireDisplay(args[1]);
                _logger.Info(first.CompareWithMonitor(second));
                break;
            }

            case "ASSIST":
            {
                DisplayManager manager = RequireManager(args[0]);
                IReadOnlyList<string> comparisons = manager.Assist();
                if (comparisons.Count == 0)
                {
                    _logger.Warn($"Manager '{manager.Name}' has fewer than two displays to compare.");
                }
                else
                {
                    foreach (string comparison in comparisons)
                    {
                        _logger.Info($"Manager '{manager.Name}': {comparison}");
                    }
                }
                break;
            }

            case "BUY":
            {
                Display display = RequireDisplay(args[0]);
                DisplayManager manager = RequireManager(args[1]);
                Display? purchasedDisplay = manager.BuyDisplay(display);
                if (purchasedDisplay is null)
                {
                    throw new InvalidOperationException($"Manager '{manager.Name}' does not have display '{display.Model}'.");
                }
                _logger.Info($"Manager '{manager.Name}' sold display '{display.Model}'.");
                if (manager.Displays.Count == 0)
                {
                    _logger.Warn($"Manager '{manager.Name}' has no displays left after the purchase.");
                }
                break;
            }
        }
    }

    private Display? FindDisplay(string model)
    {
        foreach (Display display in _displays)
        {
            if (string.Equals(display.Model, model, StringComparison.Ordinal))
            {
                return display;
            }
        }
        return null;
    }

    private DisplayManager? FindManager(string name)
    {
        foreach (DisplayManager manager in _managers)
        {
            if (string.Equals(manager.Name, name, StringComparison.Ordinal))
            {
                return manager;
            }
        }
        return null;
    }

    private Display RequireDisplay(string model) =>
        FindDisplay(model) ?? throw new InvalidOperationException($"Display '{model}' was not found.");

    private DisplayManager RequireManager(string name) =>
        FindManager(name) ?? throw new InvalidOperationException($"Manager '{name}' was not found.");
}
