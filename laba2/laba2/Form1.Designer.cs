namespace laba2;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components is not null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        menuStrip = new MenuStrip();
        fileMenu = new ToolStripMenuItem();
        clearMenuItem = new ToolStripMenuItem();
        curvesMenu = new ToolStripMenuItem();
        hermiteMenuItem = new ToolStripMenuItem();
        bezierMenuItem = new ToolStripMenuItem();
        bsplineMenuItem = new ToolStripMenuItem();
        curvesSeparator = new ToolStripSeparator();
        closedBSplineMenuItem = new ToolStripMenuItem();
        modeMenu = new ToolStripMenuItem();
        addModeMenuItem = new ToolStripMenuItem();
        editModeMenuItem = new ToolStripMenuItem();
        modeSeparator = new ToolStripSeparator();
        smoothJoinMenuItem = new ToolStripMenuItem();
        viewMenu = new ToolStripMenuItem();
        showControlMenuItem = new ToolStripMenuItem();
        showLabelsMenuItem = new ToolStripMenuItem();
        helpMenu = new ToolStripMenuItem();
        aboutMenuItem = new ToolStripMenuItem();
        curvesToolStrip = new ToolStrip();
        curvesToolLabel = new ToolStripLabel();
        hermiteButton = new ToolStripButton();
        bezierButton = new ToolStripButton();
        bsplineButton = new ToolStripButton();
        toolSeparator1 = new ToolStripSeparator();
        modeToolLabel = new ToolStripLabel();
        addModeButton = new ToolStripButton();
        editModeButton = new ToolStripButton();
        smoothJoinButton = new ToolStripButton();
        toolSeparator2 = new ToolStripSeparator();
        closedBSplineButton = new ToolStripButton();
        clearButton = new ToolStripButton();
        statusStrip = new StatusStrip();
        statusCurveLabel = new ToolStripStatusLabel();
        statusModeLabel = new ToolStripStatusLabel();
        canvas = new PictureBox();
        menuStrip.SuspendLayout();
        curvesToolStrip.SuspendLayout();
        statusStrip.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)canvas).BeginInit();
        SuspendLayout();
        // 
        // menuStrip
        // 
        menuStrip.ImageScalingSize = new Size(20, 20);
        menuStrip.Items.AddRange(new ToolStripItem[] { fileMenu, curvesMenu, modeMenu, viewMenu, helpMenu });
        menuStrip.Location = new Point(0, 0);
        menuStrip.Name = "menuStrip";
        menuStrip.Size = new Size(1000, 28);
        menuStrip.TabIndex = 0;
        menuStrip.Text = "menuStrip";
        // 
        // fileMenu
        // 
        fileMenu.DropDownItems.AddRange(new ToolStripItem[] { clearMenuItem });
        fileMenu.Name = "fileMenu";
        fileMenu.Size = new Size(59, 24);
        fileMenu.Text = "&Файл";
        // 
        // clearMenuItem
        // 
        clearMenuItem.Name = "clearMenuItem";
        clearMenuItem.Size = new Size(160, 26);
        clearMenuItem.Text = "Очистить";
        clearMenuItem.Click += Clear_Click;
        // 
        // curvesMenu
        // 
        curvesMenu.DropDownItems.AddRange(new ToolStripItem[] { hermiteMenuItem, bezierMenuItem, bsplineMenuItem, curvesSeparator, closedBSplineMenuItem });
        curvesMenu.Name = "curvesMenu";
        curvesMenu.Size = new Size(73, 24);
        curvesMenu.Text = "&Кривые";
        // 
        // hermiteMenuItem
        // 
        hermiteMenuItem.Name = "hermiteMenuItem";
        hermiteMenuItem.Size = new Size(240, 26);
        hermiteMenuItem.Text = "Форма Эрмита";
        hermiteMenuItem.Click += Hermite_Click;
        // 
        // bezierMenuItem
        // 
        bezierMenuItem.Name = "bezierMenuItem";
        bezierMenuItem.Size = new Size(240, 26);
        bezierMenuItem.Text = "Форма Безье";
        bezierMenuItem.Click += Bezier_Click;
        // 
        // bsplineMenuItem
        // 
        bsplineMenuItem.Name = "bsplineMenuItem";
        bsplineMenuItem.Size = new Size(240, 26);
        bsplineMenuItem.Text = "B-сплайн";
        bsplineMenuItem.Click += BSpline_Click;
        // 
        // curvesSeparator
        // 
        curvesSeparator.Name = "curvesSeparator";
        curvesSeparator.Size = new Size(237, 6);
        // 
        // closedBSplineMenuItem
        // 
        closedBSplineMenuItem.Name = "closedBSplineMenuItem";
        closedBSplineMenuItem.Size = new Size(240, 26);
        closedBSplineMenuItem.Text = "Замкнутый B-сплайн";
        closedBSplineMenuItem.Click += ClosedBSpline_Click;
        // 
        // modeMenu
        // 
        modeMenu.DropDownItems.AddRange(new ToolStripItem[] { addModeMenuItem, editModeMenuItem, modeSeparator, smoothJoinMenuItem });
        modeMenu.Name = "modeMenu";
        modeMenu.Size = new Size(70, 24);
        modeMenu.Text = "&Режим";
        // 
        // addModeMenuItem
        // 
        addModeMenuItem.Name = "addModeMenuItem";
        addModeMenuItem.Size = new Size(300, 26);
        addModeMenuItem.Text = "Построение точек";
        addModeMenuItem.Click += AddMode_Click;
        // 
        // editModeMenuItem
        // 
        editModeMenuItem.Name = "editModeMenuItem";
        editModeMenuItem.Size = new Size(300, 26);
        editModeMenuItem.Text = "Корректировка опорных точек";
        editModeMenuItem.Click += EditMode_Click;
        // 
        // modeSeparator
        // 
        modeSeparator.Name = "modeSeparator";
        modeSeparator.Size = new Size(297, 6);
        // 
        // smoothJoinMenuItem
        // 
        smoothJoinMenuItem.Name = "smoothJoinMenuItem";
        smoothJoinMenuItem.Size = new Size(300, 26);
        smoothJoinMenuItem.Text = "Гладкая состыковка сегментов";
        smoothJoinMenuItem.Click += SmoothJoin_Click;
        // 
        // viewMenu
        // 
        viewMenu.DropDownItems.AddRange(new ToolStripItem[] { showControlMenuItem, showLabelsMenuItem });
        viewMenu.Name = "viewMenu";
        viewMenu.Size = new Size(49, 24);
        viewMenu.Text = "&Вид";
        // 
        // showControlMenuItem
        // 
        showControlMenuItem.Checked = true;
        showControlMenuItem.CheckState = CheckState.Checked;
        showControlMenuItem.Name = "showControlMenuItem";
        showControlMenuItem.Size = new Size(330, 26);
        showControlMenuItem.Text = "Опорный многоугольник и касательные";
        showControlMenuItem.Click += ShowControl_Click;
        // 
        // showLabelsMenuItem
        // 
        showLabelsMenuItem.Checked = true;
        showLabelsMenuItem.CheckState = CheckState.Checked;
        showLabelsMenuItem.Name = "showLabelsMenuItem";
        showLabelsMenuItem.Size = new Size(330, 26);
        showLabelsMenuItem.Text = "Подписи точек";
        showLabelsMenuItem.Click += ShowLabels_Click;
        // 
        // helpMenu
        // 
        helpMenu.DropDownItems.AddRange(new ToolStripItem[] { aboutMenuItem });
        helpMenu.Name = "helpMenu";
        helpMenu.Size = new Size(81, 24);
        helpMenu.Text = "&Справка";
        // 
        // aboutMenuItem
        // 
        aboutMenuItem.Name = "aboutMenuItem";
        aboutMenuItem.Size = new Size(200, 26);
        aboutMenuItem.Text = "Как пользоваться";
        aboutMenuItem.Click += About_Click;
        // 
        // curvesToolStrip
        // 
        curvesToolStrip.ImageScalingSize = new Size(20, 20);
        curvesToolStrip.Items.AddRange(new ToolStripItem[] { curvesToolLabel, hermiteButton, bezierButton, bsplineButton, toolSeparator1, modeToolLabel, addModeButton, editModeButton, smoothJoinButton, toolSeparator2, closedBSplineButton, clearButton });
        curvesToolStrip.Location = new Point(0, 28);
        curvesToolStrip.Name = "curvesToolStrip";
        curvesToolStrip.Size = new Size(1000, 27);
        curvesToolStrip.TabIndex = 1;
        curvesToolStrip.Text = "Кривые";
        // 
        // curvesToolLabel
        // 
        curvesToolLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        curvesToolLabel.Name = "curvesToolLabel";
        curvesToolLabel.Size = new Size(64, 24);
        curvesToolLabel.Text = "Кривые:";
        // 
        // hermiteButton
        // 
        hermiteButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
        hermiteButton.Name = "hermiteButton";
        hermiteButton.Size = new Size(59, 24);
        hermiteButton.Text = "Эрмит";
        hermiteButton.ToolTipText = "Форма Эрмита";
        hermiteButton.Click += Hermite_Click;
        // 
        // bezierButton
        // 
        bezierButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
        bezierButton.Name = "bezierButton";
        bezierButton.Size = new Size(54, 24);
        bezierButton.Text = "Безье";
        bezierButton.ToolTipText = "Форма Безье";
        bezierButton.Click += Bezier_Click;
        // 
        // bsplineButton
        // 
        bsplineButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
        bsplineButton.Name = "bsplineButton";
        bsplineButton.Size = new Size(77, 24);
        bsplineButton.Text = "B-сплайн";
        bsplineButton.ToolTipText = "B-сплайн";
        bsplineButton.Click += BSpline_Click;
        // 
        // toolSeparator1
        // 
        toolSeparator1.Name = "toolSeparator1";
        toolSeparator1.Size = new Size(6, 27);
        // 
        // modeToolLabel
        // 
        modeToolLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        modeToolLabel.Name = "modeToolLabel";
        modeToolLabel.Size = new Size(58, 24);
        modeToolLabel.Text = "Режим:";
        // 
        // addModeButton
        // 
        addModeButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
        addModeButton.Name = "addModeButton";
        addModeButton.Size = new Size(94, 24);
        addModeButton.Text = "Построение";
        addModeButton.ToolTipText = "Построение точек";
        addModeButton.Click += AddMode_Click;
        // 
        // editModeButton
        // 
        editModeButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
        editModeButton.Name = "editModeButton";
        editModeButton.Size = new Size(111, 24);
        editModeButton.Text = "Корректировка";
        editModeButton.ToolTipText = "Корректировка опорных точек";
        editModeButton.Click += EditMode_Click;
        // 
        // smoothJoinButton
        // 
        smoothJoinButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
        smoothJoinButton.Name = "smoothJoinButton";
        smoothJoinButton.Size = new Size(155, 24);
        smoothJoinButton.Text = "Гладкая состыковка";
        smoothJoinButton.ToolTipText = "Гладкая состыковка сегментов";
        smoothJoinButton.Click += SmoothJoin_Click;
        // 
        // toolSeparator2
        // 
        toolSeparator2.Name = "toolSeparator2";
        toolSeparator2.Size = new Size(6, 27);
        // 
        // closedBSplineButton
        // 
        closedBSplineButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
        closedBSplineButton.Name = "closedBSplineButton";
        closedBSplineButton.Size = new Size(87, 24);
        closedBSplineButton.Text = "Замкнутый";
        closedBSplineButton.ToolTipText = "Замкнутый B-сплайн";
        closedBSplineButton.Click += ClosedBSpline_Click;
        // 
        // clearButton
        // 
        clearButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
        clearButton.Name = "clearButton";
        clearButton.Size = new Size(77, 24);
        clearButton.Text = "Очистить";
        clearButton.ToolTipText = "Удалить все точки текущей кривой";
        clearButton.Click += Clear_Click;
        // 
        // statusStrip
        // 
        statusStrip.ImageScalingSize = new Size(20, 20);
        statusStrip.Items.AddRange(new ToolStripItem[] { statusCurveLabel, statusModeLabel });
        statusStrip.Location = new Point(0, 624);
        statusStrip.Name = "statusStrip";
        statusStrip.Size = new Size(1000, 26);
        statusStrip.TabIndex = 2;
        statusStrip.Text = "statusStrip";
        // 
        // statusCurveLabel
        // 
        statusCurveLabel.BorderSides = ToolStripStatusLabelBorderSides.Right;
        statusCurveLabel.Name = "statusCurveLabel";
        statusCurveLabel.Size = new Size(120, 20);
        statusCurveLabel.Text = "Кривая: Безье";
        // 
        // statusModeLabel
        // 
        statusModeLabel.Name = "statusModeLabel";
        statusModeLabel.Size = new Size(150, 20);
        statusModeLabel.Spring = true;
        statusModeLabel.Text = "Режим: построение";
        statusModeLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // canvas
        // 
        canvas.BackColor = Color.White;
        canvas.Dock = DockStyle.Fill;
        canvas.Location = new Point(0, 55);
        canvas.Name = "canvas";
        canvas.Size = new Size(1000, 569);
        canvas.TabIndex = 3;
        canvas.TabStop = false;
        canvas.Paint += Canvas_Paint;
        canvas.MouseDown += Canvas_MouseDown;
        canvas.MouseLeave += Canvas_MouseLeave;
        canvas.MouseMove += Canvas_MouseMove;
        canvas.MouseUp += Canvas_MouseUp;
        canvas.Resize += Canvas_Resize;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1000, 650);
        Controls.Add(canvas);
        Controls.Add(statusStrip);
        Controls.Add(curvesToolStrip);
        Controls.Add(menuStrip);
        MainMenuStrip = menuStrip;
        MinimumSize = new Size(700, 450);
        Name = "Form1";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Лабораторная работа 2 — Интерполяция и аппроксимация кривых";
        menuStrip.ResumeLayout(false);
        menuStrip.PerformLayout();
        curvesToolStrip.ResumeLayout(false);
        curvesToolStrip.PerformLayout();
        statusStrip.ResumeLayout(false);
        statusStrip.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)canvas).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private MenuStrip menuStrip;
    private ToolStripMenuItem fileMenu;
    private ToolStripMenuItem clearMenuItem;
    private ToolStripMenuItem curvesMenu;
    private ToolStripMenuItem hermiteMenuItem;
    private ToolStripMenuItem bezierMenuItem;
    private ToolStripMenuItem bsplineMenuItem;
    private ToolStripSeparator curvesSeparator;
    private ToolStripMenuItem closedBSplineMenuItem;
    private ToolStripMenuItem modeMenu;
    private ToolStripMenuItem addModeMenuItem;
    private ToolStripMenuItem editModeMenuItem;
    private ToolStripSeparator modeSeparator;
    private ToolStripMenuItem smoothJoinMenuItem;
    private ToolStripMenuItem viewMenu;
    private ToolStripMenuItem showControlMenuItem;
    private ToolStripMenuItem showLabelsMenuItem;
    private ToolStripMenuItem helpMenu;
    private ToolStripMenuItem aboutMenuItem;
    private ToolStrip curvesToolStrip;
    private ToolStripLabel curvesToolLabel;
    private ToolStripButton hermiteButton;
    private ToolStripButton bezierButton;
    private ToolStripButton bsplineButton;
    private ToolStripSeparator toolSeparator1;
    private ToolStripLabel modeToolLabel;
    private ToolStripButton addModeButton;
    private ToolStripButton editModeButton;
    private ToolStripButton smoothJoinButton;
    private ToolStripSeparator toolSeparator2;
    private ToolStripButton closedBSplineButton;
    private ToolStripButton clearButton;
    private StatusStrip statusStrip;
    private ToolStripStatusLabel statusCurveLabel;
    private ToolStripStatusLabel statusModeLabel;
    private PictureBox canvas;
}
