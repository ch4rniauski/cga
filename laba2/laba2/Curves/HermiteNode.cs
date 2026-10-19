namespace laba2.Curves;

/// <summary>
/// Узел кривой Эрмита: точка и касательные векторы входящего и исходящего сегментов.
/// </summary>
internal sealed class HermiteNode
{
    public Vec2 P { get; set; }
    public Vec2 RIn { get; set; }
    public Vec2 ROut { get; set; }

    public HermiteNode(Vec2 p, Vec2 rIn, Vec2 rOut)
    {
        P = p;
        RIn = rIn;
        ROut = rOut;
    }
}
