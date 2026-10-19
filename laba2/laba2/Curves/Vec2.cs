namespace laba2.Curves;

internal readonly struct Vec2
{
    public double X { get; }
    public double Y { get; }

    public Vec2(double x, double y)
    {
        X = x;
        Y = y;
    }

    public double Length
        => Math.Sqrt(X * X + Y * Y);

    public static Vec2 operator +(Vec2 a, Vec2 b)
        => new(a.X + b.X, a.Y + b.Y);

    public static Vec2 operator -(Vec2 a, Vec2 b)
        => new(a.X - b.X, a.Y - b.Y);

    public static Vec2 operator -(Vec2 a)
        => new(-a.X, -a.Y);

    public static Vec2 operator *(Vec2 a, double k)
        => new(a.X * k, a.Y * k);

    public static Vec2 operator *(double k, Vec2 a)
        => new(a.X * k, a.Y * k);

    public static Vec2 operator /(Vec2 a, double k)
        => new(a.X / k, a.Y / k);

    public PointF ToPointF()
        => new((float)X, (float)Y);

    /// <summary>
    /// Вектор длины <paramref name="v"/>, направленный как <paramref name="direction"/>.
    /// </summary>
    public static Vec2 Align(Vec2 v, Vec2 direction)
    {
        var dirLen = direction.Length;
        if (dirLen < 1e-9)
        {
            return v;
        }

        var len = v.Length < 1e-9
            ? dirLen
            : v.Length;
        return direction * (len / dirLen);
    }
}
