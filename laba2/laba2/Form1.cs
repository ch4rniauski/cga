using System.Drawing.Drawing2D;
using laba2.Curves;
using laba2.Enums;

namespace laba2;

public partial class Form1 : Form
{
    private const float HitRadius = 8f;
    private const float PointRadius = 5f;
    private const double TangentScale = 3.0;
    private const int StepsPerSegment = 50;

    private readonly record struct Grip(HandleKind Kind, int Index);

    private readonly List<HermiteNode> hermiteNodes = [];
    private readonly List<Vec2> bezierPoints = [];
    private readonly List<Vec2> bsplinePoints = [];

    private CurveType curveType = CurveType.Bezier;
    private EditorMode mode = EditorMode.Add;
    private bool smoothJoin;
    private bool closedBSpline;
    private bool showControl = true;
    private bool showLabels = true;

    private Grip? dragged;
    private Grip? hovered;
    private bool creatingHermiteNode;
    private Vec2 mouseDownPosition;

    private readonly Font labelFont = new("Segoe UI", 9f);
    private readonly Font hintFont = new("Segoe UI", 11f);
    private readonly Color[] segmentColors = [
        Color.RoyalBlue,
        Color.OrangeRed
    ];

    public Form1()
    {
        InitializeComponent();
        UpdateUi();
    }

    #region Команды меню и панели инструментов

    private void Hermite_Click(object? sender, EventArgs e)
    {
        SetCurveType(CurveType.Hermite);
    }

    private void Bezier_Click(object? sender, EventArgs e)
    {
        SetCurveType(CurveType.Bezier);
    }

    private void BSpline_Click(object? sender, EventArgs e)
    {
        SetCurveType(CurveType.BSpline);
    }

    private void AddMode_Click(object? sender, EventArgs e)
    {
        SetMode(EditorMode.Add);
    }

    private void EditMode_Click(object? sender, EventArgs e)
    {
        SetMode(EditorMode.Edit);
    }

    private void SmoothJoin_Click(object? sender, EventArgs e)
    {
        smoothJoin = !smoothJoin;
        if (smoothJoin)
        {
            ApplySmoothJoin();
        }

        UpdateUi();
    }

    private void ClosedBSpline_Click(object? sender, EventArgs e)
    {
        closedBSpline = !closedBSpline;
        UpdateUi();
    }

    private void ShowControl_Click(object? sender, EventArgs e)
    {
        showControl = !showControl;
        UpdateUi();
    }

    private void ShowLabels_Click(object? sender, EventArgs e)
    {
        showLabels = !showLabels;
        UpdateUi();
    }

    private void Clear_Click(object? sender, EventArgs e)
    {
        switch (curveType)
        {
            case CurveType.Hermite:
            {
                hermiteNodes.Clear();
                break;
            }
            case CurveType.Bezier:
            {
                bezierPoints.Clear();
                break;
            }
            case CurveType.BSpline:
            {
                bsplinePoints.Clear();
                break;
            }
        }

        ResetInteraction();
        UpdateUi();
    }

    private void About_Click(object? sender, EventArgs e)
    {
        MessageBox.Show(this,
            @"Выбор кривой: меню «Кривые» или панель инструментов «Кривые».\n\n" +
            @"Режим «Построение»: щелчок левой кнопкой добавляет точку.\n" +
            @"  • Эрмит: нажмите и потяните мышь, чтобы задать касательный вектор узла.\n" +
            @"  • Безье: каждые 3 новые точки образуют сегмент (2 опорные + концевая).\n" +
            @"  • B-сплайн: каждая точка — вершина опорного многоугольника.\n\n" +
            @"Режим «Корректировка»: перетаскивайте точки и концы касательных,\n" +
            @"правая кнопка мыши удаляет точку.\n\n" +
            @"«Гладкая состыковка»: касательные в точках стыковки сегментов\n" +
            @"сохраняют общее направление (для Безье точки P3, P4, P5 лежат на одной прямой).",
            @"Как пользоваться", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    #endregion

    #region Мышь

    private void Canvas_MouseDown(object? sender, MouseEventArgs e)
    {
        var m = new Vec2(e.X, e.Y);
        var hit = HitTest(m);

        switch (e.Button)
        {
            case MouseButtons.Left when mode == EditorMode.Edit:
                dragged = hit;
                break;
            case MouseButtons.Left:
                AddPoint(m);
                break;
            case MouseButtons.Right when (mode == EditorMode.Edit && hit is { Kind: HandleKind.Point } h):
                DeletePoint(h.Index);
                hovered = null;
                break;
        }

        UpdateUi();
    }

    private void Canvas_MouseMove(object? sender, MouseEventArgs e)
    {
        var m = new Vec2(e.X, e.Y);

        if (dragged is { } d)
        {
            MoveHandle(d, m);
            canvas.Invalidate();
            return;
        }

        if (mode == EditorMode.Edit)
        {
            var hit = HitTest(m);
            if (hit != hovered)
            {
                hovered = hit;
                canvas.Cursor = hit is not null
                    ? Cursors.Hand
                    : Cursors.Default;
                canvas.Invalidate();
            }
        }
    }

    private void Canvas_MouseUp(object? sender, MouseEventArgs e)
    {
        if (creatingHermiteNode && hermiteNodes.Count > 0)
        {
            var m = new Vec2(e.X, e.Y);
            if ((m - mouseDownPosition).Length < 4)
            {
                SetDefaultHermiteTangent(hermiteNodes.Count - 1);
            }
        }

        creatingHermiteNode = false;
        dragged = null;
        canvas.Invalidate();
    }

    private void Canvas_MouseLeave(object? sender, EventArgs e)
    {
        if (hovered is not null)
        {
            hovered = null;
            canvas.Invalidate();
        }
    }

    private void Canvas_Resize(object? sender, EventArgs e)
    {
        canvas.Invalidate();
    }

    #endregion

    #region Редактирование

    private void AddPoint(Vec2 m)
    {
        switch (curveType)
        {
            case CurveType.Hermite:
            {
                if (hermiteNodes.Count > 0)
                {
                    var prev = hermiteNodes[^1];
                    if (prev.ROut.Length < 1e-9)
                    {
                        prev.ROut = prev.RIn = m - prev.P;
                    }
                }

                hermiteNodes.Add(new HermiteNode(m, default, default));
                dragged = new Grip(HandleKind.TangentOut, hermiteNodes.Count - 1);
                creatingHermiteNode = true;
                mouseDownPosition = m;
                break;
            }
            case CurveType.Bezier:
            {
                var i = bezierPoints.Count;
                var p = m;
                if (smoothJoin && i >= 4 && i % 3 == 1)
                {
                    var joint = bezierPoints[i - 1];
                    p = joint + Vec2.Align(m - joint, joint - bezierPoints[i - 2]);
                }

                bezierPoints.Add(p);
                break;
            }
            case CurveType.BSpline:
            {
                bsplinePoints.Add(m);
                break;
            }
        }
    }

    private void SetDefaultHermiteTangent(int index)
    {
        var node = hermiteNodes[index];
        var r = index > 0
            ? node.P - hermiteNodes[index - 1].P
            : default;
        node.RIn = node.ROut = r;
    }

    private void DeletePoint(int index)
    {
        switch (curveType)
        {
            case CurveType.Hermite:
            {
                hermiteNodes.RemoveAt(index);
                break;
            }
            case CurveType.Bezier:
            {
                bezierPoints.RemoveAt(index);
                break;
            }
            case CurveType.BSpline:
            {
                bsplinePoints.RemoveAt(index);
                break;
            }
        }
    }

    private void MoveHandle(Grip h, Vec2 m)
    {
        switch (curveType)
        {
            case CurveType.Hermite:
            {
                MoveHermiteHandle(h, m);
                break;
            }
            case CurveType.Bezier:
            {
                MoveBezierPoint(h.Index, m);
                break;
            }
            case CurveType.BSpline:
            {
                bsplinePoints[h.Index] = m;
                break;
            }
        }
    }

    private void MoveHermiteHandle(Grip h, Vec2 m)
    {
        var node = hermiteNodes[h.Index];
        switch (h.Kind)
        {
            case HandleKind.Point:
            {
                node.P = m;
                break;
            }
            case HandleKind.TangentOut:
            {
                node.ROut = (m - node.P) * TangentScale;
                if (creatingHermiteNode)
                {
                    node.RIn = node.ROut;
                }
                else if (smoothJoin)
                {
                    node.RIn = Vec2.Align(node.RIn, node.ROut);
                }

                break;
            }
            case HandleKind.TangentIn:
            {
                node.RIn = (node.P - m) * TangentScale;
                if (smoothJoin)
                {
                    node.ROut = Vec2.Align(node.ROut, node.RIn);
                }

                break;
            }
        }
    }

    /// <summary>
    /// Концевые вершины (индекс кратен 3) перемещаются вместе с соседними опорными.
    /// При гладкой состыковке опорная вершина по другую сторону стыка
    /// поворачивается так, чтобы три точки лежали на одной прямой.
    /// </summary>
    private void MoveBezierPoint(int i, Vec2 m)
    {
        var count = bezierPoints.Count;
        if (i % 3 == 0)
        {
            var delta = m - bezierPoints[i];
            bezierPoints[i] = m;
            if (i - 1 >= 0)
            {
                bezierPoints[i - 1] += delta;
            }

            if (i + 1 < count)
            {
                bezierPoints[i + 1] += delta;
            }

            return;
        }

        bezierPoints[i] = m;
        if (!smoothJoin)
        {
            return;
        }

        var joint = i % 3 == 1
            ? i - 1
            : i + 1;
        var opposite = i % 3 == 1
            ? i - 2
            : i + 2;
        if (joint <= 0 || opposite < 0 || opposite >= count)
        {
            return;
        }

        var j = bezierPoints[joint];
        bezierPoints[opposite] = j + Vec2.Align(bezierPoints[opposite] - j, j - m);
    }

    private void ApplySmoothJoin()
    {
        foreach (var node in hermiteNodes)
        {
            node.RIn = Vec2.Align(node.RIn, node.ROut);
        }

        for (var j = 3; j + 1 < bezierPoints.Count; j += 3)
        {
            var joint = bezierPoints[j];
            bezierPoints[j + 1] = joint + Vec2.Align(bezierPoints[j + 1] - joint, joint - bezierPoints[j - 1]);
        }
    }

    private Grip? HitTest(Vec2 m)
    {
        Grip? best = null;
        var bestDist = (double)HitRadius;

        switch (curveType)
        {
            case CurveType.Hermite:
            {
                if (showControl)
                {
                    for (var i = 0; i < hermiteNodes.Count; i++)
                    {
                        Check(OutHandle(hermiteNodes[i]), HandleKind.TangentOut, i);
                        if (i > 0)
                        {
                            Check(InHandle(hermiteNodes[i]), HandleKind.TangentIn, i);
                        }
                    }
                }

                // Узлы проверяются последними и при равном расстоянии имеют приоритет.
                for (var i = 0; i < hermiteNodes.Count; i++)
                {
                    var d = (hermiteNodes[i].P - m).Length;
                    if (d <= HitRadius && (best is null || d <= bestDist + 2))
                    {
                        bestDist = d;
                        best = new Grip(HandleKind.Point, i);
                    }
                }

                break;
            }
            case CurveType.Bezier:
            {
                for (var i = 0; i < bezierPoints.Count; i++)
                {
                    Check(bezierPoints[i], HandleKind.Point, i);
                }

                break;
            }
            case CurveType.BSpline:
            {
                for (var i = 0; i < bsplinePoints.Count; i++)
                {
                    Check(bsplinePoints[i], HandleKind.Point, i);
                }

                break;
            }
        }

        return best;

        void Check(Vec2 p, HandleKind kind, int index)
        {
            var d = (p - m).Length;
            if (d <= bestDist)
            {
                bestDist = d;
                best = new Grip(kind, index);
            }
        }
    }

    private static Vec2 OutHandle(HermiteNode n)
    {
        return n.P + n.ROut / TangentScale;
    }

    private static Vec2 InHandle(HermiteNode n)
    {
        return n.P - n.RIn / TangentScale;
    }

    #endregion

    #region Отрисовка

    private void Canvas_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        switch (curveType)
        {
            case CurveType.Hermite:
            {
                DrawHermite(g);
                break;
            }
            case CurveType.Bezier:
            {
                DrawBezier(g);
                break;
            }
            case CurveType.BSpline:
            {
                DrawBSpline(g);
                break;
            }
        }

        if (PointCount() == 0)
        {
            var text = mode == EditorMode.Add
                ? "Щёлкните левой кнопкой мыши, чтобы добавить точку"
                : "Нет точек. Переключитесь в режим «Построение»";
            using var brush = new SolidBrush(Color.Gray);
            var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(text, hintFont, brush, canvas.ClientRectangle, format);
        }
    }

    private void DrawSegments(Graphics g, List<Vec2[]> segments)
    {
        for (var s = 0; s < segments.Count; s++)
        {
            using var pen = new Pen(segmentColors[s % segmentColors.Length], 2.5f);
            var pts = Array.ConvertAll(segments[s], p => p.ToPointF());
            g.DrawLines(pen, pts);
        }
    }

    private void DrawHermite(Graphics g)
    {
        DrawSegments(g, CurveMath.Hermite(hermiteNodes, StepsPerSegment));

        for (var i = 0; i < hermiteNodes.Count; i++)
        {
            var n = hermiteNodes[i];
            if (showControl)
            {
                using var tangentPen = new Pen(Color.Gray, 1f);
                tangentPen.EndCap = LineCap.ArrowAnchor;
                
                var outH = OutHandle(n);
                g.DrawLine(tangentPen, n.P.ToPointF(), outH.ToPointF());
                DrawHandle(g, outH, Color.ForestGreen, IsHovered(HandleKind.TangentOut, i));
                if (i > 0)
                {
                    var inH = InHandle(n);
                    tangentPen.EndCap = LineCap.Flat;
                    tangentPen.DashStyle = DashStyle.Dash;
                    g.DrawLine(tangentPen, n.P.ToPointF(), inH.ToPointF());
                    DrawHandle(g, inH, Color.MediumVioletRed, IsHovered(HandleKind.TangentIn, i));
                }
            }

            DrawPoint(g, n.P, true, IsHovered(HandleKind.Point, i));
            if (showLabels)
            {
                var text = $"P{i + 1}";
                if (showControl)
                {
                    text += $"  R=({n.ROut.X:0},{-n.ROut.Y:0})";
                }

                DrawLabel(g, n.P, text);
            }
        }
    }

    private void DrawBezier(Graphics g)
    {
        if (showControl && bezierPoints.Count > 1)
        {
            using var pen = new Pen(Color.DarkGray, 1f);
            pen.DashStyle = DashStyle.Dash;
            
            g.DrawLines(pen, bezierPoints.Select(p => p.ToPointF()).ToArray());
        }

        DrawSegments(g, CurveMath.Bezier(bezierPoints, StepsPerSegment));

        for (var i = 0; i < bezierPoints.Count; i++)
        {
            var endpoint = i % 3 == 0;
            DrawPoint(g, bezierPoints[i], endpoint, IsHovered(HandleKind.Point, i));
            if (showLabels)
            {
                DrawLabel(g, bezierPoints[i], $"P{i + 1}");
            }
        }
    }

    private void DrawBSpline(Graphics g)
    {
        if (showControl && bsplinePoints.Count > 1)
        {
            using var pen = new Pen(Color.DarkGray, 1f);
            pen.DashStyle = DashStyle.Dash;
            
            var pts = bsplinePoints.Select(p => p.ToPointF()).ToArray();
            if (closedBSpline && pts.Length >= 3)
            {
                g.DrawPolygon(pen, pts);
            }
            else
            {
                g.DrawLines(pen, pts);
            }
        }

        DrawSegments(g, CurveMath.BSpline(bsplinePoints, closedBSpline, StepsPerSegment));

        for (var i = 0; i < bsplinePoints.Count; i++)
        {
            DrawPoint(g, bsplinePoints[i], false, IsHovered(HandleKind.Point, i));
            if (showLabels)
            {
                DrawLabel(g, bsplinePoints[i], $"P{i + 1}");
            }
        }
    }

    private static void DrawPoint(Graphics g, Vec2 p, bool square, bool highlighted)
    {
        var r = highlighted
            ? PointRadius + 2
            : PointRadius;
        var rect = new RectangleF((float)p.X - r, (float)p.Y - r, 2 * r, 2 * r);
        
        using var fill = new SolidBrush(highlighted
            ? Color.Gold
            : square
                ? Color.Black
                : Color.White);
        
        using var border = new Pen(Color.Black, 1.5f);
        
        if (square)
        {
            g.FillRectangle(fill, rect);
            g.DrawRectangle(border, rect.X, rect.Y, rect.Width, rect.Height);
        }
        else
        {
            g.FillEllipse(fill, rect);
            g.DrawEllipse(border, rect);
        }
    }

    private static void DrawHandle(Graphics g, Vec2 p, Color color, bool highlighted)
    {
        var r = highlighted
            ? 6f
            : 4f;
        
        using var brush = new SolidBrush(highlighted
            ? Color.Gold
            : color);
        
        g.FillEllipse(brush, (float)p.X - r, (float)p.Y - r, 2 * r, 2 * r);
    }

    private void DrawLabel(Graphics g, Vec2 p, string text)
    {
        g.DrawString(text, labelFont, Brushes.DimGray, (float)p.X + 8, (float)p.Y - 20);
    }

    private bool IsHovered(HandleKind kind, int index)
    {
        return (dragged ?? hovered) is { } h && h.Kind == kind && h.Index == index;
    }

    #endregion

    #region Состояние интерфейса

    private void SetCurveType(CurveType type)
    {
        curveType = type;
        ResetInteraction();
        UpdateUi();
    }

    private void SetMode(EditorMode newMode)
    {
        mode = newMode;
        ResetInteraction();
        UpdateUi();
    }

    private void ResetInteraction()
    {
        dragged = null;
        hovered = null;
        creatingHermiteNode = false;
    }

    private int PointCount()
    {
        return curveType switch
        {
            CurveType.Hermite => hermiteNodes.Count,
            CurveType.Bezier => bezierPoints.Count,
            _ => bsplinePoints.Count,
        };
    }

    private void UpdateUi()
    {
        hermiteMenuItem.Checked = hermiteButton.Checked = curveType == CurveType.Hermite;
        bezierMenuItem.Checked = bezierButton.Checked = curveType == CurveType.Bezier;
        bsplineMenuItem.Checked = bsplineButton.Checked = curveType == CurveType.BSpline;

        addModeMenuItem.Checked = addModeButton.Checked = mode == EditorMode.Add;
        editModeMenuItem.Checked = editModeButton.Checked = mode == EditorMode.Edit;

        var joinable = curveType != CurveType.BSpline;
        smoothJoinMenuItem.Checked = smoothJoinButton.Checked = smoothJoin;
        smoothJoinMenuItem.Enabled = smoothJoinButton.Enabled = joinable;

        closedBSplineMenuItem.Checked = closedBSplineButton.Checked = closedBSpline;
        closedBSplineMenuItem.Enabled = closedBSplineButton.Enabled = curveType == CurveType.BSpline;

        showControlMenuItem.Checked = showControl;
        showLabelsMenuItem.Checked = showLabels;

        canvas.Cursor = mode == EditorMode.Add
            ? Cursors.Cross
            : Cursors.Default;

        var curveName = curveType switch
        {
            CurveType.Hermite => "форма Эрмита",
            CurveType.Bezier => "форма Безье",
            _ => closedBSpline
                ? "замкнутый B-сплайн"
                : "B-сплайн",
        };
        statusCurveLabel.Text = $@"Кривая: {curveName}";
        statusModeLabel.Text = mode == EditorMode.Add
            ? "Режим: построение"
            : "Режим: корректировка";

        canvas.Invalidate();
    }

    #endregion
}
