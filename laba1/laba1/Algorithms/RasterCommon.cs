namespace laba1.Algorithms;

/// <summary>Пиксель с интенсивностью (для алгоритма Ву).</summary>
public readonly record struct PlotPoint(int X, int Y, float Intensity = 1f);

/// <summary>Один шаг алгоритма для отладочного режима.</summary>
public sealed class AlgorithmStep
{
    public int Index { get; init; }
    public string Description { get; init; } = "";
    public IReadOnlyList<PlotPoint> Points { get; init; } = [];
    public IReadOnlyDictionary<string, string> Variables { get; init; }
        = new Dictionary<string, string>();
}

/// <summary>Результат работы растрового алгоритма.</summary>
public sealed class RasterResult
{
    public List<PlotPoint> Pixels { get; } = [];
    public List<AlgorithmStep> Steps { get; } = [];

    public void AddPixel(int x, int y, float intensity = 1f)
        => Pixels.Add(new PlotPoint(x, y, intensity));

    public void AddStep(string description, IEnumerable<PlotPoint>? points = null,
        IReadOnlyDictionary<string, string>? vars = null)
    {
        Steps.Add(new AlgorithmStep
        {
            Index = Steps.Count,
            Description = description,
            Points = points?.ToList() ?? [],
            Variables = vars ?? new Dictionary<string, string>()
        });
    }
}
