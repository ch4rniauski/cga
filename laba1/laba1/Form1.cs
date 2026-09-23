using laba1.Algorithms;

namespace laba1;

public partial class Form1 : Form
{
    private const int GridCellSize = 16;
    private const int GridPadding = 28;

    private DrawTool _tool = DrawTool.LineBresenham;
    private bool _debugMode;
    private Point? _firstPoint;
    private Bitmap _canvas = null!;
    private RasterResult? _lastResult;
    private readonly List<(RasterResult result, Color color)> _layers = new();
    private bool _syncingStepSelection;

    // Кеш границ сетки для текущего шага
    private int _gridMinX, _gridMaxX, _gridMinY, _gridMaxY;
    private Dictionary<(int X, int Y), float> _litCells = new();

    public Form1()
    {
        InitializeComponent();
        KeyPreview = true;
        WireEvents();
        ResetCanvas();
        UpdateToolUi();
        statusLabel.Text = ToolHint();
    }

    private void WireEvents()
    {
        menuFileClear.Click += (_, _) => ClearAll();
        menuFileExit.Click += (_, _) => Close();

        menuSegDda.Click += (_, _) => SetTool(DrawTool.LineDda);
        menuSegBresenham.Click += (_, _) => SetTool(DrawTool.LineBresenham);
        menuSegWu.Click += (_, _) => SetTool(DrawTool.LineWu);

        menuCurveCircle.Click += (_, _) => SetTool(DrawTool.Circle);
        menuCurveEllipse.Click += (_, _) => SetTool(DrawTool.Ellipse);
        menuCurveHyperbola.Click += (_, _) => SetTool(DrawTool.Hyperbola);
        menuCurveParabola.Click += (_, _) => SetTool(DrawTool.Parabola);

        menuModeDebug.CheckedChanged += (_, _) =>
        {
            _debugMode = menuModeDebug.Checked;
            splitMain.Panel2Collapsed = !_debugMode;
            if (_debugMode && splitMain.Width > 0)
            {
                // Правая панель ~38% ширины, но не меньше минимума
                int desired = Math.Max(splitMain.Panel2MinSize + splitMain.SplitterWidth,
                    (int)(splitMain.Width * 0.38));
                splitMain.SplitterDistance = Math.Clamp(
                    splitMain.Width - desired,
                    splitMain.Panel1MinSize,
                    Math.Max(splitMain.Panel1MinSize, splitMain.Width - splitMain.Panel2MinSize - splitMain.SplitterWidth));
            }
            statusLabel.Text = _debugMode
                ? "Отладочный режим: постройте фигуру — шаги появятся справа (сетка с прокруткой)"
                : ToolHint();
        };

        menuHelpAbout.Click += (_, _) => ShowHelp();

        btnDda.Click += (_, _) => SetTool(DrawTool.LineDda);
        btnBresenham.Click += (_, _) => SetTool(DrawTool.LineBresenham);
        btnWu.Click += (_, _) => SetTool(DrawTool.LineWu);
        btnClear.Click += (_, _) => ClearAll();

        btnCircle.Click += (_, _) => SetTool(DrawTool.Circle);
        btnEllipse.Click += (_, _) => SetTool(DrawTool.Ellipse);
        btnHyperbola.Click += (_, _) => SetTool(DrawTool.Hyperbola);
        btnParabola.Click += (_, _) => SetTool(DrawTool.Parabola);

        pictureBox.MouseClick += PictureBox_MouseClick;
        pictureBox.Paint += PictureBox_Paint;
        pictureBox.Resize += (_, _) =>
        {
            if (pictureBox.Width > 0 && pictureBox.Height > 0)
                RebuildCanvasBitmap();
        };

        stepTrack.ValueChanged += (_, _) =>
        {
            if (!_syncingStepSelection)
                UpdateStepView(stepTrack.Value);
        };

        stepsGrid.SelectionChanged += (_, _) =>
        {
            if (_syncingStepSelection) return;
            if (stepsGrid.CurrentRow is { Index: >= 0 } row)
            {
                int idx = Math.Min(stepTrack.Maximum, row.Index);
                if (stepTrack.Value != idx)
                    stepTrack.Value = idx;
            }
        };

        gridCanvas.Paint += GridCanvas_Paint;
    }

    private void SetTool(DrawTool tool)
    {
        _tool = tool;
        _firstPoint = null;
        UpdateToolUi();
        statusLabel.Text = ToolHint();
        pictureBox.Invalidate();
    }

    private void UpdateToolUi()
    {
        btnDda.Checked = _tool == DrawTool.LineDda;
        btnBresenham.Checked = _tool == DrawTool.LineBresenham;
        btnWu.Checked = _tool == DrawTool.LineWu;
        btnCircle.Checked = _tool == DrawTool.Circle;
        btnEllipse.Checked = _tool == DrawTool.Ellipse;
        btnHyperbola.Checked = _tool == DrawTool.Hyperbola;
        btnParabola.Checked = _tool == DrawTool.Parabola;
    }

    private string ToolHint() => _tool switch
    {
        DrawTool.LineDda => "ЦДА: два щелчка — начало и конец отрезка",
        DrawTool.LineBresenham => "Брезенхем: два щелчка — начало и конец отрезка",
        DrawTool.LineWu => "Ву: два щелчка — начало и конец отрезка",
        DrawTool.Circle => "Окружность: 1) центр  2) точка на окружности",
        DrawTool.Ellipse => "Эллипс: 1) центр  2) угол ограничивающего прямоугольника",
        DrawTool.Hyperbola => "Гипербола: 1) центр  2) точка (Δx→a, Δy→b)",
        DrawTool.Parabola => "Парабола: 1) вершина  2) точка на ветви (задаёт p и длину)",
        _ => "Выберите инструмент"
    };

    private void ShowHelp()
    {
        MessageBox.Show(
            "1. Отрезки — ЦДА / Брезенхем / Ву (меню или панель «Отрезки»).\n" +
            "2. Кривые — окружность, эллипс, гипербола, парабола (панель «Линии второго порядка»).\n" +
            "3. Режим → «Отладочный режим» — сетка (с прокруткой) и таблица шагов.\n\n" +
            "Построение — щелчками мыши по холсту (обычно две точки).\n" +
            "В отладке разделитель между сеткой и таблицей можно двигать.",
            "Справка", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void ResetCanvas()
    {
        _layers.Clear();
        RebuildCanvasBitmap();
    }

    private void RebuildCanvasBitmap()
    {
        int w = Math.Max(1, pictureBox.ClientSize.Width);
        int h = Math.Max(1, pictureBox.ClientSize.Height);
        var old = _canvas;
        _canvas = new Bitmap(w, h);
        using (var g = Graphics.FromImage(_canvas))
            g.Clear(Color.White);
        old?.Dispose();
        pictureBox.Image = _canvas;
        foreach (var (result, color) in _layers)
            DrawPixelsOnly(result, color);
        pictureBox.Invalidate();
    }

    private void ClearAll()
    {
        _layers.Clear();
        _lastResult = null;
        _firstPoint = null;
        _litCells.Clear();
        RebuildCanvasBitmap();
        stepsGrid.Rows.Clear();
        stepTrack.Maximum = 0;
        stepInfo.Text = "Постройте фигуру — здесь появятся шаги алгоритма";
        gridCanvas.Size = new Size(200, 200);
        gridCanvas.Invalidate();
        statusLabel.Text = "Холст очищен. " + ToolHint();
    }

    private void PictureBox_MouseClick(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left) return;
        var p = e.Location;

        if (_firstPoint is null)
        {
            _firstPoint = p;
            statusLabel.Text = $"Первая точка: ({p.X}, {p.Y}). Укажите вторую.";
            pictureBox.Invalidate();
            return;
        }

        CommitShape(_firstPoint.Value, p);
        _firstPoint = null;
        pictureBox.Invalidate();
    }

    private void CommitShape(Point a, Point b)
    {
        RasterResult result = _tool switch
        {
            DrawTool.LineDda => LineAlgorithms.Dda(a.X, a.Y, b.X, b.Y),
            DrawTool.LineBresenham => LineAlgorithms.Bresenham(a.X, a.Y, b.X, b.Y),
            DrawTool.LineWu => LineAlgorithms.Wu(a.X, a.Y, b.X, b.Y),
            DrawTool.Circle => CircleFromPoints(a, b),
            DrawTool.Ellipse => EllipseFromPoints(a, b),
            DrawTool.Hyperbola => HyperbolaFromPoints(a, b),
            DrawTool.Parabola => ParabolaFromPoints(a, b),
            _ => new RasterResult()
        };

        _lastResult = result;
        _layers.Add((result, Color.Black));
        DrawResult(result, Color.Black);
        statusLabel.Text = $"{_tool}: построено, пикселей={result.Pixels.Count}, шагов={result.Steps.Count}";

        if (_debugMode)
            LoadDebug(result);
    }

    private static RasterResult CircleFromPoints(Point c, Point p)
    {
        int r = (int)Math.Round(Math.Sqrt((p.X - c.X) * (p.X - c.X) + (p.Y - c.Y) * (p.Y - c.Y)));
        return CurveAlgorithms.Circle(c.X, c.Y, r);
    }

    private static RasterResult EllipseFromPoints(Point c, Point corner)
    {
        int aa = Math.Abs(corner.X - c.X);
        int bb = Math.Abs(corner.Y - c.Y);
        return CurveAlgorithms.Ellipse(c.X, c.Y, Math.Max(1, aa), Math.Max(1, bb));
    }

    private static RasterResult HyperbolaFromPoints(Point c, Point p)
    {
        int a = Math.Max(1, Math.Abs(p.X - c.X));
        int b = Math.Max(1, Math.Abs(p.Y - c.Y));
        if (b == 0) b = Math.Max(1, a / 2);
        int extent = Math.Max(a + 50, a * 2);
        return CurveAlgorithms.Hyperbola(c.X, c.Y, a, b, extent);
    }

    private static RasterResult ParabolaFromPoints(Point vertex, Point p)
    {
        int dx = Math.Max(1, Math.Abs(p.X - vertex.X));
        int dy = Math.Abs(p.Y - vertex.Y);
        int param = dy == 0 ? Math.Max(1, dx / 4) : Math.Max(1, (dy * dy) / (2 * dx));
        int extent = Math.Max(dx, 40);
        return CurveAlgorithms.Parabola(vertex.X, vertex.Y, param, extent);
    }

    private void DrawResult(RasterResult result, Color color)
    {
        DrawPixelsOnly(result, color);
        pictureBox.Invalidate();
    }

    private void DrawPixelsOnly(RasterResult result, Color color)
    {
        foreach (var px in result.Pixels)
        {
            if (px.X < 0 || px.Y < 0 || px.X >= _canvas.Width || px.Y >= _canvas.Height)
                continue;

            if (px.Intensity >= 0.999f)
            {
                _canvas.SetPixel(px.X, px.Y, color);
            }
            else
            {
                var bg = _canvas.GetPixel(px.X, px.Y);
                float t = Math.Clamp(px.Intensity, 0f, 1f);
                var blended = Color.FromArgb(
                    (int)(color.R * t + bg.R * (1 - t)),
                    (int)(color.G * t + bg.G * (1 - t)),
                    (int)(color.B * t + bg.B * (1 - t)));
                _canvas.SetPixel(px.X, px.Y, blended);
            }
        }
    }

    private void PictureBox_Paint(object? sender, PaintEventArgs e)
    {
        if (_firstPoint is { } fp)
        {
            using var pen = new Pen(Color.DodgerBlue, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot };
            e.Graphics.DrawEllipse(pen, fp.X - 3, fp.Y - 3, 6, 6);
        }
    }

    private void LoadDebug(RasterResult result)
    {
        stepsGrid.Rows.Clear();
        foreach (var s in result.Steps)
        {
            string vars = string.Join(", ", s.Variables.Select(kv => $"{kv.Key}={kv.Value}"));
            stepsGrid.Rows.Add(s.Index, s.Description, vars);
        }

        stepTrack.Minimum = 0;
        stepTrack.Maximum = Math.Max(0, result.Steps.Count - 1);
        stepTrack.Value = 0;
        UpdateStepView(0);
    }

    private void UpdateStepView(int index)
    {
        if (_lastResult is null || _lastResult.Steps.Count == 0)
        {
            stepInfo.Text = "Нет шагов";
            _litCells.Clear();
            gridCanvas.Invalidate();
            return;
        }

        index = Math.Clamp(index, 0, _lastResult.Steps.Count - 1);
        var step = _lastResult.Steps[index];
        string vars = string.Join("  ", step.Variables.Select(kv => $"{kv.Key}={kv.Value}"));
        stepInfo.Text = $"Шаг {index}/{_lastResult.Steps.Count - 1}: {step.Description}\n{vars}";

        RebuildLitCells(index);
        LayoutGridCanvas();
        ScrollToCurrentStep(index);
        gridCanvas.Invalidate();

        _syncingStepSelection = true;
        try
        {
            if (index < stepsGrid.Rows.Count)
            {
                stepsGrid.ClearSelection();
                stepsGrid.Rows[index].Selected = true;
                if (stepsGrid.CanFocus)
                    stepsGrid.FirstDisplayedScrollingRowIndex = Math.Max(0, index - 2);
            }
        }
        finally
        {
            _syncingStepSelection = false;
        }
    }

    private void RebuildLitCells(int upToStep)
    {
        _litCells = new Dictionary<(int X, int Y), float>();
        if (_lastResult is null) return;

        for (int i = 0; i <= upToStep && i < _lastResult.Steps.Count; i++)
        {
            foreach (var p in _lastResult.Steps[i].Points)
                _litCells[(p.X, p.Y)] = p.Intensity;
        }

        if (_litCells.Count == 0)
        {
            _gridMinX = _gridMaxX = _gridMinY = _gridMaxY = 0;
            return;
        }

        _gridMinX = _litCells.Keys.Min(k => k.X) - 1;
        _gridMaxX = _litCells.Keys.Max(k => k.X) + 1;
        _gridMinY = _litCells.Keys.Min(k => k.Y) - 1;
        _gridMaxY = _litCells.Keys.Max(k => k.Y) + 1;
    }

    /// <summary>
    /// Фиксированный размер ячейки + AutoScroll: длинные линии не сжимаются
    /// и не уходят под таблицу — их можно прокрутить.
    /// </summary>
    private void LayoutGridCanvas()
    {
        int cellsX = Math.Max(1, _gridMaxX - _gridMinX + 1);
        int cellsY = Math.Max(1, _gridMaxY - _gridMinY + 1);

        int width = GridPadding + cellsX * GridCellSize + GridPadding;
        int height = GridPadding + cellsY * GridCellSize + GridPadding;

        // Не меньше области просмотра — чтобы короткие сетки выглядели нормально
        width = Math.Max(width, gridScrollHost.ClientSize.Width);
        height = Math.Max(height, gridScrollHost.ClientSize.Height);

        if (gridCanvas.Width != width || gridCanvas.Height != height)
            gridCanvas.Size = new Size(width, height);
    }

    private void ScrollToCurrentStep(int index)
    {
        if (_lastResult is null || index < 0 || index >= _lastResult.Steps.Count)
            return;

        var pts = _lastResult.Steps[index].Points;
        if (pts.Count == 0) return;

        int avgX = (int)pts.Average(p => p.X);
        int avgY = (int)pts.Average(p => p.Y);

        int px = GridPadding + (avgX - _gridMinX) * GridCellSize + GridCellSize / 2;
        int py = GridPadding + (avgY - _gridMinY) * GridCellSize + GridCellSize / 2;

        int viewW = Math.Max(1, gridScrollHost.ClientSize.Width);
        int viewH = Math.Max(1, gridScrollHost.ClientSize.Height);
        int targetX = Math.Max(0, px - viewW / 2);
        int targetY = Math.Max(0, py - viewH / 2);

        // AutoScrollPosition при чтении даёт отрицательные значения; при записи — положительные
        gridScrollHost.AutoScrollPosition = new Point(targetX, targetY);
    }

    private void GridCanvas_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.Clear(Color.WhiteSmoke);

        if (_litCells.Count == 0)
        {
            using var font = new Font("Segoe UI", 9);
            g.DrawString("Нет данных для отображения", font, Brushes.Gray, 12, 12);
            return;
        }

        using var gridPen = new Pen(Color.LightGray);
        using var axisFont = new Font("Consolas", 7);
        using var brush = new SolidBrush(Color.DimGray);

        for (int y = _gridMinY; y <= _gridMaxY; y++)
        for (int x = _gridMinX; x <= _gridMaxX; x++)
        {
            float px = GridPadding + (x - _gridMinX) * GridCellSize;
            float py = GridPadding + (y - _gridMinY) * GridCellSize;
            var rect = new RectangleF(px, py, GridCellSize, GridCellSize);
            g.DrawRectangle(gridPen, px, py, GridCellSize, GridCellSize);

            if (_litCells.TryGetValue((x, y), out float inten))
            {
                int a = (int)(Math.Clamp(inten, 0.2f, 1f) * 255);
                using var fill = new SolidBrush(Color.FromArgb(a, Color.DarkBlue));
                g.FillRectangle(fill, rect);
            }
        }

        int idx = stepTrack.Value;
        if (_lastResult is not null && idx >= 0 && idx < _lastResult.Steps.Count)
        {
            using var hi = new Pen(Color.OrangeRed, 2);
            foreach (var p in _lastResult.Steps[idx].Points)
            {
                float px = GridPadding + (p.X - _gridMinX) * GridCellSize;
                float py = GridPadding + (p.Y - _gridMinY) * GridCellSize;
                g.DrawRectangle(hi, px, py, GridCellSize, GridCellSize);
            }
        }

        g.DrawString(
            $"Ячейка {GridCellSize}px  |  X:[{_gridMinX}..{_gridMaxX}]  Y:[{_gridMinY}..{_gridMaxY}]  |  прокрутка: мышь/колёсико",
            axisFont, brush, 4, 4);
    }
}
