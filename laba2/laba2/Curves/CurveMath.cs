namespace laba2.Curves;

/// <summary>
/// Построение кривых в матричной форме P(t) = T · M · G, где T = [t³ t² t 1].
/// </summary>
internal static class CurveMath
{
    /// <summary>
    /// Матрица Эрмита: обращение матрицы граничных условий x(0), x(1), x'(0), x'(1).
    /// </summary>
    private static readonly Matrix HermiteMatrix = new Matrix(new double[,]
    {
        { 0, 0, 0, 1 },
        { 1, 1, 1, 1 },
        { 0, 0, 1, 0 },
        { 3, 2, 1, 0 },
    }).Inverse();

    /// <summary>
    /// Переход от вершин Безье к эрмитовой геометрии: [P1 P4 R1 R4] = K · [P1 P2 P3 P4],
    /// где R1 = 3(P2 − P1), R4 = 3(P4 − P3).
    /// </summary>
    private static readonly Matrix BezierToHermite = new(new double[,]
    {
        { 1, 0, 0, 0 },
        { 0, 0, 0, 1 },
        { -3, 3, 0, 0 },
        { 0, 0, -3, 3 },
    });

    private static readonly Matrix BezierMatrix = HermiteMatrix * BezierToHermite;

    private static readonly Matrix BSplineMatrix = (1.0 / 6.0) * new Matrix(new double[,]
    {
        { -1, 3, -3, 1 },
        { 3, -6, 3, 0 },
        { -3, 0, 3, 0 },
        { 1, 4, 1, 0 },
    });

    /// <summary>
    /// Матрица геометрии 4x2: в первом столбце x-составляющие, во втором — y.
    /// </summary>
    private static Matrix Geometry(Vec2 g0, Vec2 g1, Vec2 g2, Vec2 g3)
    {
        return new Matrix(new[,]
        {
            { g0.X, g0.Y },
            { g1.X, g1.Y },
            { g2.X, g2.Y },
            { g3.X, g3.Y },
        });
    }

    private static Vec2[] EvaluateSegment(Matrix basis, Matrix geometry, int steps)
    {
        var coefficients = basis * geometry;
        var result = new Vec2[steps + 1];
        for (var i = 0; i <= steps; i++)
        {
            var t = (double)i / steps;
            var p = Matrix.RowVector(t * t * t, t * t, t, 1) * coefficients;
            result[i] = new Vec2(p[0, 0], p[0, 1]);
        }

        return result;
    }

    public static List<Vec2[]> Hermite(IReadOnlyList<HermiteNode> nodes, int steps)
    {
        var segments = new List<Vec2[]>();
        for (var i = 0; i + 1 < nodes.Count; i++)
        {
            var a = nodes[i];
            var b = nodes[i + 1];
            var g = Geometry(a.P, b.P, a.ROut, b.RIn);
            segments.Add(EvaluateSegment(HermiteMatrix, g, steps));
        }

        return segments;
    }

    /// <summary>
    /// Составная кривая Безье: соседние сегменты делят концевую вершину (P4 = P1 следующего).
    /// </summary>
    public static List<Vec2[]> Bezier(IReadOnlyList<Vec2> points, int steps)
    {
        var segments = new List<Vec2[]>();
        for (var i = 0; i + 3 < points.Count; i += 3)
        {
            var g = Geometry(points[i], points[i + 1], points[i + 2], points[i + 3]);
            segments.Add(EvaluateSegment(BezierMatrix, g, steps));
        }

        return segments;
    }

    /// <summary>
    /// Кубический B-сплайн. Для открытой кривой добавляются фиктивные вершины
    /// P0 = 2P1 − P2 и Pn+1 = 2Pn − Pn−1, чтобы кривая начиналась и заканчивалась в крайних точках.
    /// Для замкнутой кривой набор дополняется вершинами Pn+1 = P0, Pn+2 = P1, Pn+3 = P2.
    /// </summary>
    public static List<Vec2[]> BSpline(IReadOnlyList<Vec2> points, bool closed, int steps)
    {
        var segments = new List<Vec2[]>();
        var n = points.Count;

        if (closed && n >= 3)
        {
            for (var i = 0; i < n; i++)
            {
                var g = Geometry(points[i], points[(i + 1) % n], points[(i + 2) % n], points[(i + 3) % n]);
                segments.Add(EvaluateSegment(BSplineMatrix, g, steps));
            }

            return segments;
        }

        if (n < 2)
        {
            return segments;
        }

        var ext = new List<Vec2>(n + 2) { 2 * points[0] - points[1] };
        ext.AddRange(points);
        ext.Add(2 * points[n - 1] - points[n - 2]);

        for (var i = 0; i + 3 < ext.Count; i++)
        {
            var g = Geometry(ext[i], ext[i + 1], ext[i + 2], ext[i + 3]);
            segments.Add(EvaluateSegment(BSplineMatrix, g, steps));
        }

        return segments;
    }
}
