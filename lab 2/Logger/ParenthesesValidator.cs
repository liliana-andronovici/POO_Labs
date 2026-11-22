namespace Lab2;

/// <summary>Lab 1 validator updated to receive and use the refactored logger.</summary>
public sealed class ParenthesesValidator
{
    private readonly Logger _logger;

    public ParenthesesValidator(Logger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public bool IsValid(string? input)
    {
        if (input is null)
        {
            _logger.Error("Input \"<null>\" is invalid because it is null.");
            _logger.Info("Validation result: invalid.");
            return false;
        }

        int openParentheses = 0;
        bool hasUnmatchedClosingParenthesis = false;

        foreach (char character in input)
        {
            if (character == '(')
            {
                openParentheses++;
                _logger.Trace("A new opening parenthesis was found.");
            }
            else if (character == ')')
            {
                if (openParentheses == 0)
                {
                    hasUnmatchedClosingParenthesis = true;
                    _logger.Error("A closing parenthesis has no opening parenthesis to match it.");
                }
                else
                {
                    openParentheses--;
                    _logger.Trace("A matching pair was found.");
                }
            }
        }

        if (openParentheses > 0)
        {
            _logger.Error($"Input \"{input}\" has {openParentheses} unclosed opening parenthesis/parentheses.");
        }

        bool isValid = openParentheses == 0 && !hasUnmatchedClosingParenthesis;
        _logger.Info($"Validation result: {(isValid ? "valid" : "invalid")}.");
        return isValid;
    }
}
