using System.Text;

namespace Lab2;

/// <summary>Builds log lines. It never writes to a console or another output device.</summary>
public sealed class Formatter
{
    private static readonly Dictionary<LogLevel, string[]> RandomMessages = new()
    {
        [LogLevel.TRACE] = ["Checking a processing step", "A processing step was reached"],
        [LogLevel.INFO] = ["Operation completed", "Input was checked", "Here is the result"],
        [LogLevel.WARN] = ["An unexpected situation was found", "Please check this input"],
        [LogLevel.ERROR] = ["The operation failed", "The input is invalid"]
    };

    public string FormatString { get; }

    public Formatter(string formatString = "[%t] [%l] %n: %m")
    {
        FormatString = formatString ?? throw new ArgumentNullException(nameof(formatString));
    }

    public string Format(string loggerName, LogLevel level, string message)
    {
        ArgumentNullException.ThrowIfNull(loggerName);
        ArgumentNullException.ThrowIfNull(message);

        string time = DateTime.Now.ToString("HH:mm:ss");
        string? randomMessage = null;
        var result = new StringBuilder(FormatString.Length + message.Length + loggerName.Length + 16);

        for (int i = 0; i < FormatString.Length; i++)
        {
            if (FormatString[i] != '%' || i == FormatString.Length - 1)
            {
                result.Append(FormatString[i]);
                continue;
            }

            switch (FormatString[i + 1])
            {
                case 't': result.Append(time); i++; break;
                case 'l': result.Append(level); i++; break;
                case 'n': result.Append(loggerName); i++; break;
                case 'm': result.Append(message); i++; break;
                case 'r':
                    randomMessage ??= GetRandomMessage(level);
                    result.Append(randomMessage);
                    i++;
                    break;
                default:
                    // Preserve unknown tokens exactly as written, for example %x.
                    result.Append('%');
                    break;
            }
        }

        // The message is appended as data while expanding %m. It is not scanned again:
        // a message containing "%n" therefore prints those two characters literally.
        return result.ToString();
    }

    private static string GetRandomMessage(LogLevel level)
    {
        if (!RandomMessages.TryGetValue(level, out string[]? messages) || messages.Length == 0)
        {
            return "Log message";
        }

        return messages[Random.Shared.Next(messages.Length)];
    }
}
