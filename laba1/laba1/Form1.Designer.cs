namespace laba1
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private MenuStrip menuStrip;
        private ToolStripMenuItem menuFile;
        private ToolStripMenuItem menuFileClear;
        private ToolStripMenuItem menuFileExit;
        private ToolStripMenuItem menuSegments;
        private ToolStripMenuItem menuSegDda;
        private ToolStripMenuItem menuSegBresenham;
        private ToolStripMenuItem menuSegWu;
        private ToolStripMenuItem menuCurves;
        private ToolStripMenuItem menuCurveCircle;
        private ToolStripMenuItem menuCurveEllipse;
        private ToolStripMenuItem menuCurveHyperbola;
        private ToolStripMenuItem menuCurveParabola;
        private ToolStripMenuItem menuMode;
        private ToolStripMenuItem menuModeDebug;
        private ToolStripMenuItem menuHelp;
        private ToolStripMenuItem menuHelpAbout;

        private ToolStrip tsSegments;
        private ToolStripLabel lblSegments;
        private ToolStripButton btnDda;
        private ToolStripButton btnBresenham;
        private ToolStripButton btnWu;
        private ToolStripSeparator sepSegments;
        private ToolStripButton btnClear;

        private ToolStrip tsCurves;
        private ToolStripLabel lblCurves;
        private ToolStripButton btnCircle;
        private ToolStripButton btnEllipse;
        private ToolStripButton btnHyperbola;
        private ToolStripButton btnParabola;

        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;

        private SplitContainer splitMain;
        private PictureBox pictureBox;

        private Panel debugPanel;
        private SplitContainer splitDebug;
        private Label lblDebugTitle;
        private Label stepInfo;
        private TrackBar stepTrack;
        private Panel gridScrollHost;
        private Panel gridCanvas;
        private DataGridView stepsGrid;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                _canvas?.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            menuStrip = new MenuStrip();
            menuFile = new ToolStripMenuItem();
            menuFileClear = new ToolStripMenuItem();
            menuFileExit = new ToolStripMenuItem();
            menuSegments = new ToolStripMenuItem();
            menuSegDda = new ToolStripMenuItem();
            menuSegBresenham = new ToolStripMenuItem();
            menuSegWu = new ToolStripMenuItem();
            menuCurves = new ToolStripMenuItem();
            menuCurveCircle = new ToolStripMenuItem();
            menuCurveEllipse = new ToolStripMenuItem();
            menuCurveHyperbola = new ToolStripMenuItem();
            menuCurveParabola = new ToolStripMenuItem();
            menuMode = new ToolStripMenuItem();
            menuModeDebug = new ToolStripMenuItem();
            menuHelp = new ToolStripMenuItem();
            menuHelpAbout = new ToolStripMenuItem();

            tsSegments = new ToolStrip();
            lblSegments = new ToolStripLabel();
            btnDda = new ToolStripButton();
            btnBresenham = new ToolStripButton();
            btnWu = new ToolStripButton();
            sepSegments = new ToolStripSeparator();
            btnClear = new ToolStripButton();

            tsCurves = new ToolStrip();
            lblCurves = new ToolStripLabel();
            btnCircle = new ToolStripButton();
            btnEllipse = new ToolStripButton();
            btnHyperbola = new ToolStripButton();
            btnParabola = new ToolStripButton();

            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel();

            splitMain = new SplitContainer();
            pictureBox = new PictureBox();

            debugPanel = new Panel();
            splitDebug = new SplitContainer();
            lblDebugTitle = new Label();
            stepInfo = new Label();
            stepTrack = new TrackBar();
            gridScrollHost = new Panel();
            gridCanvas = new Panel();
            stepsGrid = new DataGridView();

            menuStrip.SuspendLayout();
            tsSegments.SuspendLayout();
            tsCurves.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitMain).BeginInit();
            splitMain.Panel1.SuspendLayout();
            splitMain.Panel2.SuspendLayout();
            splitMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox).BeginInit();
            debugPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitDebug).BeginInit();
            splitDebug.Panel1.SuspendLayout();
            splitDebug.Panel2.SuspendLayout();
            splitDebug.SuspendLayout();
            gridScrollHost.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)stepsGrid).BeginInit();
            SuspendLayout();

            //
            // menuStrip
            //
            menuStrip.Items.AddRange(new ToolStripItem[]
            {
                menuFile, menuSegments, menuCurves, menuMode, menuHelp
            });
            menuStrip.Dock = DockStyle.Top;
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(1100, 24);
            menuStrip.Text = "menuStrip";

            menuFile.DropDownItems.AddRange(new ToolStripItem[] { menuFileClear, menuFileExit });
            menuFile.Name = "menuFile";
            menuFile.Text = "Файл";

            menuFileClear.Name = "menuFileClear";
            menuFileClear.Text = "Очистить";

            menuFileExit.Name = "menuFileExit";
            menuFileExit.Text = "Выход";

            menuSegments.DropDownItems.AddRange(new ToolStripItem[]
            {
                menuSegDda, menuSegBresenham, menuSegWu
            });
            menuSegments.Name = "menuSegments";
            menuSegments.Text = "Отрезки";

            menuSegDda.Name = "menuSegDda";
            menuSegDda.Text = "ЦДА";

            menuSegBresenham.Name = "menuSegBresenham";
            menuSegBresenham.Text = "Брезенхем (целочисленный)";

            menuSegWu.Name = "menuSegWu";
            menuSegWu.Text = "Ву";

            menuCurves.DropDownItems.AddRange(new ToolStripItem[]
            {
                menuCurveCircle, menuCurveEllipse, menuCurveHyperbola, menuCurveParabola
            });
            menuCurves.Name = "menuCurves";
            menuCurves.Text = "Линии второго порядка";

            menuCurveCircle.Name = "menuCurveCircle";
            menuCurveCircle.Text = "Окружность";

            menuCurveEllipse.Name = "menuCurveEllipse";
            menuCurveEllipse.Text = "Эллипс";

            menuCurveHyperbola.Name = "menuCurveHyperbola";
            menuCurveHyperbola.Text = "Гипербола";

            menuCurveParabola.Name = "menuCurveParabola";
            menuCurveParabola.Text = "Парабола";

            menuMode.DropDownItems.AddRange(new ToolStripItem[] { menuModeDebug });
            menuMode.Name = "menuMode";
            menuMode.Text = "Режим";

            menuModeDebug.CheckOnClick = true;
            menuModeDebug.Name = "menuModeDebug";
            menuModeDebug.Text = "Отладочный режим";

            menuHelp.DropDownItems.AddRange(new ToolStripItem[] { menuHelpAbout });
            menuHelp.Name = "menuHelp";
            menuHelp.Text = "Справка";

            menuHelpAbout.Name = "menuHelpAbout";
            menuHelpAbout.Text = "Как пользоваться";

            //
            // tsSegments — панель «Отрезки»
            //
            tsSegments.Dock = DockStyle.Top;
            tsSegments.GripStyle = ToolStripGripStyle.Hidden;
            tsSegments.Items.AddRange(new ToolStripItem[]
            {
                lblSegments, btnDda, btnBresenham, btnWu, sepSegments, btnClear
            });
            tsSegments.Location = new Point(0, 24);
            tsSegments.Name = "tsSegments";
            tsSegments.Size = new Size(1100, 25);
            tsSegments.Text = "Отрезки";

            lblSegments.Name = "lblSegments";
            lblSegments.Text = "Отрезки:";

            btnDda.CheckOnClick = true;
            btnDda.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnDda.Name = "btnDda";
            btnDda.Text = "ЦДА";

            btnBresenham.CheckOnClick = true;
            btnBresenham.Checked = true;
            btnBresenham.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnBresenham.Name = "btnBresenham";
            btnBresenham.Text = "Брезенхем";

            btnWu.CheckOnClick = true;
            btnWu.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnWu.Name = "btnWu";
            btnWu.Text = "Ву";

            sepSegments.Name = "sepSegments";

            btnClear.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnClear.Name = "btnClear";
            btnClear.Text = "Очистить";

            //
            // tsCurves — панель «Линии второго порядка»
            //
            tsCurves.Dock = DockStyle.Top;
            tsCurves.GripStyle = ToolStripGripStyle.Hidden;
            tsCurves.Items.AddRange(new ToolStripItem[]
            {
                lblCurves, btnCircle, btnEllipse, btnHyperbola, btnParabola
            });
            tsCurves.Location = new Point(0, 49);
            tsCurves.Name = "tsCurves";
            tsCurves.Size = new Size(1100, 25);
            tsCurves.Text = "Линии второго порядка";

            lblCurves.Name = "lblCurves";
            lblCurves.Text = "Линии второго порядка:";

            btnCircle.CheckOnClick = true;
            btnCircle.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnCircle.Name = "btnCircle";
            btnCircle.Text = "Окружность";

            btnEllipse.CheckOnClick = true;
            btnEllipse.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnEllipse.Name = "btnEllipse";
            btnEllipse.Text = "Эллипс";

            btnHyperbola.CheckOnClick = true;
            btnHyperbola.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnHyperbola.Name = "btnHyperbola";
            btnHyperbola.Text = "Гипербола";

            btnParabola.CheckOnClick = true;
            btnParabola.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnParabola.Name = "btnParabola";
            btnParabola.Text = "Парабола";

            //
            // statusStrip
            //
            statusStrip.Dock = DockStyle.Bottom;
            statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel });
            statusStrip.Location = new Point(0, 678);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(1100, 22);
            statusStrip.Text = "statusStrip";

            statusLabel.Name = "statusLabel";
            statusLabel.Spring = true;
            statusLabel.Text = "Брезенхем: два щелчка — начало и конец отрезка";
            statusLabel.TextAlign = ContentAlignment.MiddleLeft;

            //
            // splitMain
            //
            splitMain.Dock = DockStyle.Fill;
            splitMain.Location = new Point(0, 74);
            splitMain.Name = "splitMain";
            splitMain.Orientation = Orientation.Vertical;
            splitMain.Panel2Collapsed = true;
            splitMain.Panel2MinSize = 280;
            splitMain.Size = new Size(1100, 604);
            splitMain.SplitterDistance = 720;
            splitMain.TabIndex = 0;

            //
            // pictureBox (холст)
            //
            pictureBox.BackColor = Color.White;
            pictureBox.BorderStyle = BorderStyle.FixedSingle;
            pictureBox.Dock = DockStyle.Fill;
            pictureBox.Location = new Point(0, 0);
            pictureBox.Name = "pictureBox";
            pictureBox.Size = new Size(720, 604);
            pictureBox.TabIndex = 0;
            pictureBox.TabStop = false;

            splitMain.Panel1.Controls.Add(pictureBox);

            //
            // debugPanel
            //
            debugPanel.Controls.Add(splitDebug);
            debugPanel.Dock = DockStyle.Fill;
            debugPanel.Name = "debugPanel";
            debugPanel.Padding = new Padding(6);
            debugPanel.Size = new Size(376, 604);

            //
            // splitDebug — сетка сверху, таблица шагов снизу
            //
            splitDebug.Dock = DockStyle.Fill;
            splitDebug.Location = new Point(6, 6);
            splitDebug.Name = "splitDebug";
            splitDebug.Orientation = Orientation.Horizontal;
            splitDebug.Panel1MinSize = 180;
            splitDebug.Panel2MinSize = 120;
            splitDebug.Size = new Size(364, 592);
            splitDebug.SplitterDistance = 340;
            splitDebug.TabIndex = 0;

            // Panel1 debug: title + info + track + scrollable grid
            lblDebugTitle.Dock = DockStyle.Top;
            lblDebugTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblDebugTitle.Height = 22;
            lblDebugTitle.Name = "lblDebugTitle";
            lblDebugTitle.Text = "Отладочный режим — дискретная сетка";
            lblDebugTitle.TextAlign = ContentAlignment.MiddleLeft;

            stepInfo.Dock = DockStyle.Top;
            stepInfo.Height = 42;
            stepInfo.Name = "stepInfo";
            stepInfo.Text = "Постройте фигуру — здесь появятся шаги алгоритма";

            stepTrack.Dock = DockStyle.Top;
            stepTrack.Height = 45;
            stepTrack.Maximum = 0;
            stepTrack.Name = "stepTrack";
            stepTrack.TickStyle = TickStyle.None;

            gridScrollHost.AutoScroll = true;
            gridScrollHost.BackColor = Color.Gainsboro;
            gridScrollHost.BorderStyle = BorderStyle.FixedSingle;
            gridScrollHost.Controls.Add(gridCanvas);
            gridScrollHost.Dock = DockStyle.Fill;
            gridScrollHost.Name = "gridScrollHost";

            gridCanvas.BackColor = Color.WhiteSmoke;
            gridCanvas.Location = new Point(0, 0);
            gridCanvas.Name = "gridCanvas";
            gridCanvas.Size = new Size(200, 200);

            splitDebug.Panel1.Controls.Add(gridScrollHost);
            splitDebug.Panel1.Controls.Add(stepTrack);
            splitDebug.Panel1.Controls.Add(stepInfo);
            splitDebug.Panel1.Controls.Add(lblDebugTitle);

            //
            // stepsGrid
            //
            stepsGrid.AllowUserToAddRows = false;
            stepsGrid.AllowUserToDeleteRows = false;
            stepsGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            stepsGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            stepsGrid.Dock = DockStyle.Fill;
            stepsGrid.MultiSelect = false;
            stepsGrid.Name = "stepsGrid";
            stepsGrid.ReadOnly = true;
            stepsGrid.RowHeadersVisible = false;
            stepsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            stepsGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "idx",
                HeaderText = "№",
                FillWeight = 15
            });
            stepsGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "desc",
                HeaderText = "Описание",
                FillWeight = 35
            });
            stepsGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "vars",
                HeaderText = "Переменные",
                FillWeight = 50
            });

            splitDebug.Panel2.Controls.Add(stepsGrid);
            splitMain.Panel2.Controls.Add(debugPanel);

            //
            // Form1
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1100, 700);
            // Порядок важен для Dock: Fill → Bottom → Top-панели → MenuStrip
            Controls.Add(splitMain);
            Controls.Add(statusStrip);
            Controls.Add(tsCurves);
            Controls.Add(tsSegments);
            Controls.Add(menuStrip);
            MainMenuStrip = menuStrip;
            MinimumSize = new Size(900, 600);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Лабораторная работа 1 — Растровая графика";

            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            tsSegments.ResumeLayout(false);
            tsSegments.PerformLayout();
            tsCurves.ResumeLayout(false);
            tsCurves.PerformLayout();
            splitMain.Panel1.ResumeLayout(false);
            splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitMain).EndInit();
            splitMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox).EndInit();
            debugPanel.ResumeLayout(false);
            splitDebug.Panel1.ResumeLayout(false);
            splitDebug.Panel1.PerformLayout();
            splitDebug.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitDebug).EndInit();
            splitDebug.ResumeLayout(false);
            gridScrollHost.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)stepsGrid).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
