using System.Globalization;

namespace Lab2;

public sealed record ShopCommand(string Name, IReadOnlyList<string> Arguments, int LineNumber);

public static class ShopCommandParser
{
    public static bool TryParse(NumberedLine line, out ShopCommand? command, out string error)
    {
        command = null;
        error = string.Empty;
        string trimmed = line.Text.Trim();
        if (trimmed.Length == 0 || trimmed.StartsWith('#'))
        {
            return true;
        }

        string[] parts = trimmed.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        string name = parts[0].ToUpperInvariant();
        string[] arguments = parts.Skip(1).ToArray();
        int expectedArguments = name switch
        {
            "MANAGER" => 1,
            "DISPLAY" => 4,
            "ASSIGN" => 2,
            "COMPARE" => 2,
            "ASSIST" => 1,
            "BUY" => 2,
            _ => -1
        };

        if (expectedArguments < 0)
        {
            error = $"Unknown command '{parts[0]}'.";
            return false;
        }
        if (arguments.Length != expectedArguments)
        {
            error = $"{parts[0]} expects {expectedArguments} argument(s), but received {arguments.Length}.";
            return false;
        }
        if (arguments.Any(string.IsNullOrWhiteSpace))
        {
            error = "Arguments cannot be empty.";
            return false;
        }

        if (name == "DISPLAY")
        {
            for (int i = 1; i <= 3; i++)
            {
                if (!double.TryParse(arguments[i], NumberStyles.Float, CultureInfo.InvariantCulture, out double value)
                    || !double.IsFinite(value) || value <= 0)
                {
                    string field = i switch { 1 => "width", 2 => "height", _ => "PPI" };
                    error = $"DISPLAY {field} must be a positive finite number; received '{arguments[i]}'.";
                    return false;
                }
            }
        }

        command = new ShopCommand(name, arguments, line.Number);
        return true;
    }
}
