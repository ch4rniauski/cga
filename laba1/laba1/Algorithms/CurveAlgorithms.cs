namespace laba1.Algorithms;

/// <summary>Алгоритмы растеризации линий второго порядка (Брезенхем / mid-point).</summary>
public static class CurveAlgorithms
{
    /// <summary>Окружность по алгоритму Брезенхема (все октанты через симметрию).</summary>
    public static RasterResult Circle(int xc, int yc, int r)
    {
        var result = new RasterResult();
        r = Math.Abs(r);

        var x = 0;
        var y = r;
        var delta = 2 - 2 * r;

        PlotCirclePoints(result, xc, yc, x, y);
        result.AddStep("Инициализация", SymPoints(xc, yc, x, y),
            Dict(("R", r.ToString()), ("x", "0"), ("y", r.ToString()), ("Δi", delta.ToString())));

        var step = 0;
        while (y > 0)
        {
            string pixel;
            int pred;

            switch (delta)
            {
                case < 0:
                {
                    // Случай A: диагональ внутри — H или D
                    pred = 2 * (delta + y) - 1;
                    x++;
                    if (pred <= 0)
                    {
                        delta += 2 * x + 1;
                        pixel = "H";
                    }
                    else
                    {
                        y--;
                        delta += 2 * (x - y + 1);
                        pixel = "D";
                    }

                    break;
                }
                case > 0:
                {
                    // Случай B: диагональ снаружи — V или D
                    pred = 2 * (delta - x) - 1;
                    if (pred <= 0)
                    {
                        x++;
                        y--;
                        delta += 2 * (x - y + 1);
                        pixel = "D";
                    }
                    else
                    {
                        y--;
                        delta += -2 * y + 1;
                        pixel = "V";
                    }

                    break;
                }
                default:
                    // Случай C
                    pred = 0;
                    x++;
                    y--;
                    delta += 2 * (x - y + 1);
                    pixel = "D";
                    break;
            }

            step++;
            PlotCirclePoints(result, xc, yc, x, y);
            result.AddStep($"Шаг {step} ({pixel})", SymPoints(xc, yc, x, y),
                Dict(("пиксель", pixel), ("δ", pred.ToString()),
                    ("x", x.ToString()), ("y", y.ToString()), ("Δi", delta.ToString())));
        }

        return result;
    }

    /// <summary>Эллипс по mid-point алгоритму Брезенхема.</summary>
    public static RasterResult Ellipse(int xc, int yc, int a, int b)
    {
        var result = new RasterResult();
        a = Math.Abs(a);
        b = Math.Abs(b);
        if (a == 0 && b == 0)
        {
            result.AddPixel(xc, yc);
            result.AddStep("Точка", [new PlotPoint(xc, yc)], Dict(("a", "0"), ("b", "0")));
            return result;
        }

        var aa = (long)a * a;
        var bb = (long)b * b;
        long x = 0;
        long y = b;
        var delta = aa + bb - 2 * aa * b;

        PlotEllipsePoints(result, xc, yc, (int)x, (int)y);
        result.AddStep("Инициализация (регион 1)", SymEllipse(xc, yc, (int)x, (int)y),
            Dict(("a", a.ToString()), ("b", b.ToString()),
                ("x", "0"), ("y", y.ToString()), ("Δ", delta.ToString())));

        var step = 0;

        // Регион 1
        while (bb * x <= aa * y)
        {
            string pixel;
            switch (delta)
            {
                case < 0:
                {
                    var sigma = 2 * (delta + aa * y) - 1;
                    x++;
                    if (sigma <= 0)
                    {
                        delta += bb * (2 * x + 1);
                        pixel = "H";
                    }
                    else
                    {
                        y--;
                        delta += bb * (2 * x + 1) + aa * (1 - 2 * y);
                        pixel = "D";
                    }

                    break;
                }
                case > 0:
                {
                    var sigma = 2 * (delta - bb * x) - 1;
                    if (sigma <= 0)
                    {
                        x++;
                        y--;
                        delta += bb * (2 * x + 1) + aa * (1 - 2 * y);
                        pixel = "D";
                    }
                    else
                    {
                        y--;
                        delta += aa * (1 - 2 * y);
                        pixel = "V";
                    }

                    break;
                }
                default:
                    x++;
                    y--;
                    delta += bb * (2 * x + 1) + aa * (1 - 2 * y);
                    pixel = "D";
                    break;
            }

            step++;
            PlotEllipsePoints(result, xc, yc, (int)x, (int)y);
            result.AddStep($"Шаг {step} R1 ({pixel})", SymEllipse(xc, yc, (int)x, (int)y),
                Dict(("пиксель", pixel), ("x", x.ToString()), ("y", y.ToString()), ("Δ", delta.ToString())));

            if (y < 0)
            {
                break;
            }
        }

        // Регион 2
        while (y > 0)
        {
            string pixel;
            if (delta < 0)
            {
                var sigma = 2 * (delta + aa * y) - 1;
                if (sigma <= 0)
                {
                    x++;
                    y--;
                    delta += bb * (2 * x + 1) + aa * (1 - 2 * y);
                    pixel = "D";
                }
                else
                {
                    y--;
                    delta += aa * (1 - 2 * y);
                    pixel = "V";
                }
            }
            else
            {
                y--;
                delta += aa * (1 - 2 * y);
                pixel = "V";
            }

            step++;
            PlotEllipsePoints(result, xc, yc, (int)x, (int)y);
            result.AddStep($"Шаг {step} R2 ({pixel})", SymEllipse(xc, yc, (int)x, (int)y),
                Dict(("пиксель", pixel), ("x", x.ToString()), ("y", y.ToString()), ("Δ", delta.ToString())));
        }

        return result;
    }

    /// <summary>
    /// Гипербола x²/a² − y²/b² = 1.
    /// Целочисленный mid-point с симметрией по четырём ветвям.
    /// </summary>
    public static RasterResult Hyperbola(int xc, int yc, int a, int b, int extent)
    {
        var result = new RasterResult();
        a = Math.Max(1, Math.Abs(a));
        b = Math.Max(1, Math.Abs(b));
        extent = Math.Max(a + 2, Math.Abs(extent));

        var aa = (long)a * a;
        var bb = (long)b * b;

        // Старт в вершине (a, 0)
        long x = a;
        long y = 0;
        // d = b²(x+1)² − a²(y+½)² − a²b², умножено для целых:
        // при (a,0): d = b²(2a+1) − a²/4 ≈ используем 4d
        var d = 4 * bb * (2 * a + 1) - aa; // 4*(b²(2a+1) - a²/4)

        PlotHyperbola(result, xc, yc, (int)x, (int)y);
        result.AddStep("Инициализация", SymHyperbola(xc, yc, (int)x, (int)y),
            Dict(("a", a.ToString()), ("b", b.ToString()), ("x", x.ToString()), ("y", "0"), ("d", d.ToString())));

        var step = 0;

        // Регион 1: |наклон| > 1 — растёт y
        while (aa * (2 * y + 1) < 2 * bb * x && x <= extent)
        {
            if (d < 0)
            {
                // Диагональный шаг
                x++;
                y++;
                d += 4 * bb * (2 * x) + 4 * aa * (1 - 2 * y);
            }
            else
            {
                y++;
                d += 4 * aa * (1 - 2 * y);
            }

            step++;
            PlotHyperbola(result, xc, yc, (int)x, (int)y);
            result.AddStep($"Шаг {step} R1", SymHyperbola(xc, yc, (int)x, (int)y),
                Dict(("x", x.ToString()), ("y", y.ToString()), ("d", d.ToString())));
        }

        // Регион 2: |наклон| ≤ 1 — растёт x
        while (x <= extent)
        {
            x++;
            if (d > 0)
            {
                y++;
                d += 4 * bb * (2 * x) + 4 * aa * (1 - 2 * y);
            }
            else
            {
                d += 4 * bb * (2 * x);
            }

            step++;
            PlotHyperbola(result, xc, yc, (int)x, (int)y);
            result.AddStep($"Шаг {step} R2", SymHyperbola(xc, yc, (int)x, (int)y),
                Dict(("x", x.ToString()), ("y", y.ToString()), ("d", d.ToString())));
        }

        return result;
    }

    /// <summary>Парабола y² = 2px (открыта вправо) + симметрия.</summary>
    public static RasterResult Parabola(int xc, int yc, int p, int extent)
    {
        var result = new RasterResult();
        p = Math.Max(1, Math.Abs(p));
        extent = Math.Max(1, Math.Abs(extent));

        var x = 0;
        var y = 0;
        // f(x,y) = y² − 2px; mid-point для региона 1 (движение по y): d = 1 − p
        var d = 1 - p;

        PlotParabola(result, xc, yc, x, y);
        result.AddStep("Инициализация", SymParabola(xc, yc, x, y),
            Dict(("p", p.ToString()), ("x", "0"), ("y", "0"), ("d", d.ToString())));

        var step = 0;

        // Регион 1: пока y < p (крутой участок)
        while (y < p && x <= extent)
        {
            if (d < 0)
            {
                d += 2 * y + 3;
                y++;
            }
            else
            {
                d += 2 * y + 3 - 2 * p;
                y++;
                x++;
            }

            step++;
            PlotParabola(result, xc, yc, x, y);
            result.AddStep($"Шаг {step} R1", SymParabola(xc, yc, x, y),
                Dict(("x", x.ToString()), ("y", y.ToString()), ("d", d.ToString())));
        }

        // Регион 2: пологий участок
        d = (y + 1) * (y + 1) - 2 * p * (x + 1);
        while (x < extent)
        {
            if (d > 0)
            {
                d += -2 * p;
                x++;
            }
            else
            {
                d += 2 * y + 2 - 2 * p;
                x++;
                y++;
            }

            step++;
            PlotParabola(result, xc, yc, x, y);
            result.AddStep($"Шаг {step} R2", SymParabola(xc, yc, x, y),
                Dict(("x", x.ToString()), ("y", y.ToString()), ("d", d.ToString())));
        }

        return result;
    }

    private static void PlotCirclePoints(RasterResult r, int xc, int yc, int x, int y)
    {
        foreach (var p in SymPoints(xc, yc, x, y))
        {
            r.AddPixel(p.X, p.Y);
        }
    }

    private static void PlotEllipsePoints(RasterResult r, int xc, int yc, int x, int y)
    {
        foreach (var p in SymEllipse(xc, yc, x, y))
        {
            r.AddPixel(p.X, p.Y);
        }
    }

    private static void PlotHyperbola(RasterResult r, int xc, int yc, int x, int y)
    {
        foreach (var p in SymHyperbola(xc, yc, x, y))
        {
            r.AddPixel(p.X, p.Y);
        }
    }

    private static void PlotParabola(RasterResult r, int xc, int yc, int x, int y)
    {
        foreach (var p in SymParabola(xc, yc, x, y))
        {
            r.AddPixel(p.X, p.Y);
        }
    }

    private static List<PlotPoint> SymPoints(int xc, int yc, int x, int y) =>
        new List<PlotPoint>
        {
            new(xc + x, yc + y), new(xc - x, yc + y),
            new(xc + x, yc - y), new(xc - x, yc - y),
            new(xc + y, yc + x), new(xc - y, yc + x),
            new(xc + y, yc - x), new(xc - y, yc - x)
        }.Distinct().ToList();

    private static List<PlotPoint> SymEllipse(int xc, int yc, int x, int y) =>
        new List<PlotPoint>
        {
            new(xc + x, yc + y), new(xc - x, yc + y),
            new(xc + x, yc - y), new(xc - x, yc - y)
        }.Distinct().ToList();

    private static List<PlotPoint> SymHyperbola(int xc, int yc, int x, int y) =>
        new List<PlotPoint>
        {
            new(xc + x, yc + y), new(xc + x, yc - y),
            new(xc - x, yc + y), new(xc - x, yc - y)
        }.Distinct().ToList();

    private static List<PlotPoint> SymParabola(int xc, int yc, int x, int y) =>
        new List<PlotPoint>
        {
            new(xc + x, yc + y),
            new(xc + x, yc - y)
        }.Distinct().ToList();

    private static Dictionary<string, string> Dict(params (string k, string v)[] pairs)
    {
        var d = new Dictionary<string, string>();
        foreach (var (k, v) in pairs)
        {
            d[k] = v;
        }
        return d;
    }
}
