namespace laba1.Algorithms;

/// <summary>Алгоритмы растеризации отрезков: ЦДА, Брезенхем, Ву.</summary>
public static class LineAlgorithms
{
    /// <summary>Цифровой дифференциальный анализатор (ЦДА).</summary>
    public static RasterResult Dda(int x1, int y1, int x2, int y2)
    {
        var result = new RasterResult();
        var length = Math.Max(Math.Abs(x2 - x1), Math.Abs(y2 - y1));
        if (length == 0)
        {
            result.AddPixel(x1, y1);
            result.AddStep("Начальная/конечная точка совпадают",
                [new PlotPoint(x1, y1)],
                Dict(("Длина", "0"), ("x", x1.ToString()), ("y", y1.ToString())));
            return result;
        }

        var dx = (double)(x2 - x1) / length;
        var dy = (double)(y2 - y1) / length;
        var x = x1 + 0.5 * Math.Sign(dx);
        var y = y1 + 0.5 * Math.Sign(dy);

        result.AddStep("Инициализация",
            [new PlotPoint((int)x, (int)y)],
            Dict(("Длина", length.ToString()), ("dx", F(dx)), ("dy", F(dy)),
                ("x", F(x)), ("y", F(y))));
        result.AddPixel((int)x, (int)y);

        for (var i = 1; i <= length; i++)
        {
            x += dx;
            y += dy;
            var px = (int)x;
            var py = (int)y;
            result.AddPixel(px, py);
            result.AddStep($"Шаг {i}",
                [new PlotPoint(px, py)],
                Dict(("i", i.ToString()), ("x", F(x)), ("y", F(y)),
                    ("Plot", $"({px}, {py})")));
        }

        return result;
    }

    /// <summary>Целочисленный алгоритм Брезенхема (все октанты).</summary>
    public static RasterResult Bresenham(int x1, int y1, int x2, int y2)
    {
        var result = new RasterResult();
        var dx = Math.Abs(x2 - x1);
        var dy = Math.Abs(y2 - y1);
        var sx = x1 < x2 ? 1 : -1;
        var sy = y1 < y2 ? 1 : -1;
        var err = dx - dy;
        int x = x1, y = y1;

        result.AddStep("Инициализация",
            [new PlotPoint(x, y)],
            Dict(("dx", dx.ToString()), ("dy", dy.ToString()),
                ("sx", sx.ToString()), ("sy", sy.ToString()),
                ("e", err.ToString()), ("x", x.ToString()), ("y", y.ToString())));
        result.AddPixel(x, y);

        var i = 0;
        while (x != x2 || y != y2)
        {
            var e2 = 2 * err;
            var correction = "";
            if (e2 > -dy)
            {
                err -= dy;
                x += sx;
                correction += "x+=sx; ";
            }
            if (e2 < dx)
            {
                err += dx;
                y += sy;
                correction += "y+=sy; ";
            }

            i++;
            result.AddPixel(x, y);
            result.AddStep($"Шаг {i}",
                [new PlotPoint(x, y)],
                Dict(("i", i.ToString()), ("e", err.ToString()), ("2e", e2.ToString()),
                    ("коррекция", correction.Trim()),
                    ("Plot", $"({x}, {y})")));
        }

        return result;
    }

    /// <summary>Алгоритм Ву (сглаживание ступенчатости).</summary>
    public static RasterResult Wu(int x0, int y0, int x1, int y1)
    {
        var result = new RasterResult();
        var steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);

        if (steep)
        {
            (x0, y0) = (y0, x0);
            (x1, y1) = (y1, x1);
        }

        if (x0 > x1)
        {
            (x0, x1) = (x1, x0);
            (y0, y1) = (y1, y0);
        }

        var dx = x1 - x0;
        var dy = y1 - y0;
        var gradient = dx == 0 ? 1f : (float)dy / dx;

        // Первый конец
        var xEnd = x0;
        var yEnd = y0 + gradient * (xEnd - x0);
        var xGap = RfPart(x0 + 0.5f);
        var xPx1 = xEnd;
        var yPx1 = IPart(yEnd);

        var i1 = RfPart(yEnd) * xGap;
        var i2 = FPart(yEnd) * xGap;
        Plot(xPx1, yPx1, i1);
        Plot(xPx1, yPx1 + 1, i2);

        var firstPts = steep
            ? new[] { new PlotPoint(yPx1, xPx1, i1), new PlotPoint(yPx1 + 1, xPx1, i2) }
            : new[] { new PlotPoint(xPx1, yPx1, i1), new PlotPoint(xPx1, yPx1 + 1, i2) };

        result.AddStep("Начальная точка", firstPts,
            Dict(("steep", steep.ToString()), ("gradient", F(gradient)),
                ("интенсивности", $"{F(i1)}, {F(i2)}")));

        var intery = yEnd + gradient;

        // Второй конец
        xEnd = x1;
        yEnd = y1 + gradient * (xEnd - x1);
        xGap = FPart(x1 + 0.5f);
        var xPx2 = xEnd;
        var yPx2 = IPart(yEnd);

        // Основной цикл
        var step = 0;
        for (var x = xPx1 + 1; x <= xPx2 - 1; x++)
        {
            var c1 = RfPart(intery);
            var c2 = FPart(intery);
            var y = IPart(intery);
            Plot(x, y, c1);
            Plot(x, y + 1, c2);

            var pts = steep
                ? new[] { new PlotPoint(y, x, c1), new PlotPoint(y + 1, x, c2) }
                : new[] { new PlotPoint(x, y, c1), new PlotPoint(x, y + 1, c2) };

            step++;
            result.AddStep($"Шаг {step}", pts,
                Dict(("x", x.ToString()), ("y", y.ToString()),
                    ("intery", F(intery)),
                    ("I1", F(c1)), ("I2", F(c2))));
            intery += gradient;
        }

        i1 = RfPart(yEnd) * xGap;
        i2 = FPart(yEnd) * xGap;
        Plot(xPx2, yPx2, i1);
        Plot(xPx2, yPx2 + 1, i2);

        var lastPts = steep
            ? new[] { new PlotPoint(yPx2, xPx2, i1), new PlotPoint(yPx2 + 1, xPx2, i2) }
            : new[] { new PlotPoint(xPx2, yPx2, i1), new PlotPoint(xPx2, yPx2 + 1, i2) };

        result.AddStep("Конечная точка", lastPts,
            Dict(("интенсивности", $"{F(i1)}, {F(i2)}")));

        return result;

        void Plot(int x, int y, float c)
        {
            if (steep)
                result.AddPixel(y, x, c);
            else
                result.AddPixel(x, y, c);
        }
    }

    private static int IPart(float x) => (int)x;
    private static float FPart(float x) => x - (float)Math.Floor(x);
    private static float RfPart(float x) => 1f - FPart(x);

    private static string F(double v) => v.ToString("0.###");

    private static Dictionary<string, string> Dict(params (string k, string v)[] pairs)
    {
        var d = new Dictionary<string, string>();
        foreach (var (k, v) in pairs) d[k] = v;
        return d;
    }
}
