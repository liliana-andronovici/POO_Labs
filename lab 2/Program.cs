namespace Lab2;

public static class Program
{
    public static int Main(string[] args)
    {
        // Aggregation: all shop loggers receive these same two externally-created objects.
        var sharedFormatter = new Formatter("[%t] [%l] %n: %m");
        var sharedSink = new ConsoleSink();
        Logger.SetLevel(LogLevel.TRACE);

        var shopLogger = new Logger("DisplayShop", sharedFormatter, sharedSink);
        var readerLogger = new Logger("CommandReader", sharedFormatter, sharedSink);
        var reader = new CommandReader(readerLogger);
        IReadOnlyList<NumberedLine>? lines = reader.Read(args);
        if (lines is null)
        {
            return 1;
        }

        DemonstrateFormatterAndParenthesesValidator(sharedFormatter, sharedSink);
        DemonstrateSharedDisplayReference(new Logger("SharingDemo", sharedFormatter, sharedSink));
        new ShopProgram(shopLogger).Run(lines);
        return 0;
    }

    private static void DemonstrateFormatterAndParenthesesValidator(Formatter sharedFormatter, ConsoleSink sink)
    {
        var levelFirstFormatter = new Formatter("[%l] %n: %m");
        var timeFirstFormatter = new Formatter("%t | %m");

        sink.Write(levelFirstFormatter.Format("FormatterDemo", LogLevel.INFO, "First format example."));
        sink.Write(timeFirstFormatter.Format("FormatterDemo", LogLevel.INFO, "Second format example."));
        sink.Write(levelFirstFormatter.Format("FormatterDemo", LogLevel.INFO, "Literal %n remains message text."));

        var validatorLogger = new Logger("Parentheses", sharedFormatter, sink);
        var validator = new ParenthesesValidator(validatorLogger);
        (string Input, bool Expected)[] cases =
        [
            ("()", true),
            (")((()))", false),
            ("(", false),
            ("(())((()())())", true)
        ];

        foreach ((string input, bool expected) in cases)
        {
            bool actual = validator.IsValid(input);
            validatorLogger.Info($"Parentheses test '{input}': expected {expected}, got {actual}.");
        }
    }

    private static void DemonstrateSharedDisplayReference(Logger logger)
    {
        var firstManager = new DisplayManager("Demo-Bob");
        var secondManager = new DisplayManager("Demo-Alice");
        var sharedDisplay = new Display("Dell-U2723QE", 3840, 2160, 163.0);
        var secondDisplay = new Display("LG-27GP850", 2560, 1440, 108.8);
        var thirdDisplay = new Display("Samsung-Odyssey-G5", 2560, 1440, 93.0);

        logger.Trace("Hard-coded display example: " + sharedDisplay.CompareWithMonitor(secondDisplay));
        firstManager.AssignDisplay(sharedDisplay);
        firstManager.AssignDisplay(secondDisplay);
        firstManager.AssignDisplay(thirdDisplay);
        secondManager.AssignDisplay(sharedDisplay);
        logger.Trace("Assigned the same Dell-U2723QE object to both Demo-Bob and Demo-Alice.");
        foreach (string comparison in firstManager.Assist())
        {
            logger.Trace($"Demo-Bob assists: {comparison}");
        }

        Display? bought = firstManager.BuyDisplay(sharedDisplay);
        logger.Info($"Demo-Bob's purchase returned the same object: {ReferenceEquals(sharedDisplay, bought)}.");
        logger.Trace($"After the purchase, Demo-Bob has {firstManager.Displays.Count} display(s); Demo-Alice still has {secondManager.Displays.Count} reference(s) to {secondManager.Displays[0].Model}.");
    }
}
