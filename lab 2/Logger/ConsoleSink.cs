namespace Lab2;

/// <summary>The console output endpoint. This is the only class that writes to the console.</summary>
public sealed class ConsoleSink
{
    public void Write(string line)
    {
        Console.WriteLine(line);
    }
}
