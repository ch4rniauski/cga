namespace ParticlesSystems
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelSide = new System.Windows.Forms.Panel();
            this.lblMark = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblScene = new System.Windows.Forms.Label();
            this.comboScene = new System.Windows.Forms.ComboBox();
            this.lblCamera = new System.Windows.Forms.Label();
            this.comboCamera = new System.Windows.Forms.ComboBox();
            this.lblMode = new System.Windows.Forms.Label();
            this.comboMode = new System.Windows.Forms.ComboBox();
            this.chkSpin = new System.Windows.Forms.CheckBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.lblHint = new System.Windows.Forms.Label();
            this.AnT = new ParticlesSystems.GlControl();
            this.RenderTimer = new System.Windows.Forms.Timer(this.components);
            this.panelSide.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelSide
            // 
            this.panelSide.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(40)))), ((int)(((byte)(34)))));
            this.panelSide.Controls.Add(this.lblMark);
            this.panelSide.Controls.Add(this.lblTitle);
            this.panelSide.Controls.Add(this.lblScene);
            this.panelSide.Controls.Add(this.comboScene);
            this.panelSide.Controls.Add(this.lblCamera);
            this.panelSide.Controls.Add(this.comboCamera);
            this.panelSide.Controls.Add(this.lblMode);
            this.panelSide.Controls.Add(this.comboMode);
            this.panelSide.Controls.Add(this.chkSpin);
            this.panelSide.Controls.Add(this.btnStart);
            this.panelSide.Controls.Add(this.lblHint);
            this.panelSide.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelSide.Location = new System.Drawing.Point(0, 0);
            this.panelSide.Name = "panelSide";
            this.panelSide.Size = new System.Drawing.Size(248, 640);
            this.panelSide.TabIndex = 0;
            // 
            // lblMark
            // 
            this.lblMark.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(112)))), ((int)(((byte)(48)))));
            this.lblMark.Location = new System.Drawing.Point(0, 0);
            this.lblMark.Name = "lblMark";
            this.lblMark.Size = new System.Drawing.Size(248, 6);
            this.lblMark.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(230)))), ((int)(((byte)(214)))));
            this.lblTitle.Location = new System.Drawing.Point(18, 22);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(110, 30);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Листопад";
            // 
            // lblScene
            // 
            this.lblScene.AutoSize = true;
            this.lblScene.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(176)))), ((int)(((byte)(156)))));
            this.lblScene.Location = new System.Drawing.Point(20, 78);
            this.lblScene.Name = "lblScene";
            this.lblScene.Size = new System.Drawing.Size(41, 15);
            this.lblScene.TabIndex = 2;
            this.lblScene.Text = "Сцена";
            // 
            // comboScene
            // 
            this.comboScene.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(56)))), ((int)(((byte)(48)))));
            this.comboScene.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboScene.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboScene.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(230)))), ((int)(((byte)(214)))));
            this.comboScene.Items.AddRange(new object[] {
            "Обычный взрыв",
            "Направленный взрыв",
            "Листопад"});
            this.comboScene.Location = new System.Drawing.Point(20, 98);
            this.comboScene.Name = "comboScene";
            this.comboScene.Size = new System.Drawing.Size(208, 23);
            this.comboScene.TabIndex = 3;
            this.comboScene.SelectedIndexChanged += new System.EventHandler(this.comboScene_SelectedIndexChanged);
            // 
            // lblCamera
            // 
            this.lblCamera.AutoSize = true;
            this.lblCamera.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(176)))), ((int)(((byte)(156)))));
            this.lblCamera.Location = new System.Drawing.Point(20, 136);
            this.lblCamera.Name = "lblCamera";
            this.lblCamera.Size = new System.Drawing.Size(48, 15);
            this.lblCamera.TabIndex = 4;
            this.lblCamera.Text = "Камера";
            // 
            // comboCamera
            // 
            this.comboCamera.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(56)))), ((int)(((byte)(48)))));
            this.comboCamera.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboCamera.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboCamera.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(230)))), ((int)(((byte)(214)))));
            this.comboCamera.Items.AddRange(new object[] {
            "1 — горизонт",
            "2 — сверху",
            "3 — наклон",
            "4 — сбоку",
            "5 — вдоль"});
            this.comboCamera.Location = new System.Drawing.Point(20, 156);
            this.comboCamera.Name = "comboCamera";
            this.comboCamera.Size = new System.Drawing.Size(208, 23);
            this.comboCamera.TabIndex = 5;
            // 
            // lblMode
            // 
            this.lblMode.AutoSize = true;
            this.lblMode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(176)))), ((int)(((byte)(156)))));
            this.lblMode.Location = new System.Drawing.Point(20, 194);
            this.lblMode.Name = "lblMode";
            this.lblMode.Size = new System.Drawing.Size(30, 15);
            this.lblMode.TabIndex = 6;
            this.lblMode.Text = "Фон";
            // 
            // comboMode
            // 
            this.comboMode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(56)))), ((int)(((byte)(48)))));
            this.comboMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboMode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboMode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(230)))), ((int)(((byte)(214)))));
            this.comboMode.Items.AddRange(new object[] {
            "Светлый",
            "Тёмный"});
            this.comboMode.Location = new System.Drawing.Point(20, 214);
            this.comboMode.Name = "comboMode";
            this.comboMode.Size = new System.Drawing.Size(208, 23);
            this.comboMode.TabIndex = 7;
            // 
            // chkSpin
            // 
            this.chkSpin.AutoSize = true;
            this.chkSpin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(230)))), ((int)(((byte)(214)))));
            this.chkSpin.Location = new System.Drawing.Point(20, 258);
            this.chkSpin.Name = "chkSpin";
            this.chkSpin.Size = new System.Drawing.Size(118, 19);
            this.chkSpin.TabIndex = 8;
            this.chkSpin.Text = "Крутить осколки";
            this.chkSpin.UseVisualStyleBackColor = false;
            // 
            // btnStart
            // 
            this.btnStart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(112)))), ((int)(((byte)(48)))));
            this.btnStart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStart.FlatAppearance.BorderSize = 0;
            this.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStart.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.btnStart.ForeColor = System.Drawing.Color.White;
            this.btnStart.Location = new System.Drawing.Point(20, 296);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(208, 38);
            this.btnStart.TabIndex = 9;
            this.btnStart.Text = "Запустить";
            this.btnStart.UseVisualStyleBackColor = false;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // lblHint
            // 
            this.lblHint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lblHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(160)))), ((int)(((byte)(144)))));
            this.lblHint.Location = new System.Drawing.Point(20, 352);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(208, 268);
            this.lblHint.TabIndex = 10;
            // 
            // AnT
            // 
            this.AnT.BackColor = System.Drawing.Color.Black;
            this.AnT.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AnT.Location = new System.Drawing.Point(248, 0);
            this.AnT.Name = "AnT";
            this.AnT.Size = new System.Drawing.Size(732, 640);
            this.AnT.TabIndex = 1;
            // 
            // RenderTimer
            // 
            this.RenderTimer.Interval = 30;
            this.RenderTimer.Tick += new System.EventHandler(this.RenderTimer_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(47)))), ((int)(((byte)(40)))), ((int)(((byte)(34)))));
            this.ClientSize = new System.Drawing.Size(980, 640);
            this.Controls.Add(this.AnT);
            this.Controls.Add(this.panelSide);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(860, 560);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ЛР №4 · листопад";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panelSide.ResumeLayout(false);
            this.panelSide.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel panelSide;
        private System.Windows.Forms.Label lblMark;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblScene;
        private System.Windows.Forms.ComboBox comboScene;
        private System.Windows.Forms.Label lblCamera;
        private System.Windows.Forms.ComboBox comboCamera;
        private System.Windows.Forms.Label lblMode;
        private System.Windows.Forms.ComboBox comboMode;
        private System.Windows.Forms.CheckBox chkSpin;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label lblHint;
        private GlControl AnT;
        private System.Windows.Forms.Timer RenderTimer;
    }
}
