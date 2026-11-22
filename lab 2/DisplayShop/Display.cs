using System.Globalization;

namespace Lab2;

public sealed class Display
{
    public string Model { get; }
    public double Width { get; }
    public double Height { get; }
    public double Ppi { get; }

    public Display(string model, double width, double height, double ppi)
    {
        if (string.IsNullOrWhiteSpace(model))
        {
            throw new ArgumentException("A display model is required.", nameof(model));
        }
        if (!double.IsFinite(width) || width <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(width), "Width must be a positive finite number.");
        }
        if (!double.IsFinite(height) || height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(height), "Height must be a positive finite number.");
        }
        if (!double.IsFinite(ppi) || ppi <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(ppi), "PPI must be a positive finite number.");
        }

        Model = model;
        Width = width;
        Height = height;
        Ppi = ppi;
    }

    public int CompareSize(Display other)
    {
        ArgumentNullException.ThrowIfNull(other);
        return DiagonalInches().CompareTo(other.DiagonalInches());
    }

    public int CompareSharpness(Display other)
    {
        ArgumentNullException.ThrowIfNull(other);
        return Ppi.CompareTo(other.Ppi);
    }

    /// <summary>Compares diagonal size and pixel density and returns a human-readable verdict.</summary>
    public string CompareWithMonitor(Display other)
    {
        ArgumentNullException.ThrowIfNull(other);
        int sizeComparison = CompareSize(other);
        int sharpnessComparison = CompareSharpness(other);
        string thisDiagonal = DiagonalInches().ToString("0.00", CultureInfo.InvariantCulture);
        string otherDiagonal = other.DiagonalInches().ToString("0.00", CultureInfo.InvariantCulture);

        string sizeVerdict = sizeComparison switch
        {
            > 0 => $"{Model} ({thisDiagonal}\") is bigger than {other.Model} ({otherDiagonal}\").",
            < 0 => $"{other.Model} ({otherDiagonal}\") is bigger than {Model} ({thisDiagonal}\").",
            _ => $"{Model} and {other.Model} have the same diagonal ({thisDiagonal}\")."
        };

        string sharpnessVerdict = sharpnessComparison switch
        {
            > 0 => $"{Model} has higher pixel density ({Ppi.ToString("0.##", CultureInfo.InvariantCulture)} PPI vs {other.Ppi.ToString("0.##", CultureInfo.InvariantCulture)} PPI).",
            < 0 => $"{other.Model} has higher pixel density ({other.Ppi.ToString("0.##", CultureInfo.InvariantCulture)} PPI vs {Ppi.ToString("0.##", CultureInfo.InvariantCulture)} PPI).",
            _ => $"Both have the same pixel density ({Ppi.ToString("0.##", CultureInfo.InvariantCulture)} PPI)."
        };

        return $"{sizeVerdict} {sharpnessVerdict}";
    }

    public double DiagonalInches() => Math.Sqrt(Width * Width + Height * Height) / Ppi;

    public override string ToString() =>
        $"{Model} ({Width.ToString("0.##", CultureInfo.InvariantCulture)} x {Height.ToString("0.##", CultureInfo.InvariantCulture)} px, {Ppi.ToString("0.##", CultureInfo.InvariantCulture)} PPI, {DiagonalInches().ToString("0.##", CultureInfo.InvariantCulture)} in)";
}
