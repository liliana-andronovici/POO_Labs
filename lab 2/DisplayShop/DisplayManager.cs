namespace Lab2;

public sealed class DisplayManager
{
    private readonly List<Display> _displays = [];

    public string Name { get; }
    public IReadOnlyList<Display> Displays => _displays.AsReadOnly();

    public DisplayManager(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("A manager name is required.", nameof(name));
        }

        Name = name;
    }

    public bool AssignDisplay(Display display)
    {
        ArgumentNullException.ThrowIfNull(display);
        if (_displays.Contains(display))
        {
            return false;
        }

        _displays.Add(display);
        return true;
    }

    public IReadOnlyList<string> Assist()
    {
        var comparisons = new List<string>();
        for (int i = 0; i + 1 < _displays.Count; i++)
        {
            comparisons.Add(_displays[i].CompareWithMonitor(_displays[i + 1]));
        }

        return comparisons;
    }

    /// <summary>Removes and returns this manager's reference; other managers are unaffected.</summary>
    public Display? BuyDisplay(Display display)
    {
        ArgumentNullException.ThrowIfNull(display);
        return _displays.Remove(display) ? display : null;
    }
}
