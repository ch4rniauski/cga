using System;
using System.Windows.Forms;

namespace ParticlesSystems
{
    public partial class Form1 : Form
    {
        enum SceneKind
        {
            Explosion,
            DirectedExplosion,
            LeafFall
        }

        float global_time;
        readonly float[,] camera_date = new float[3, 7];

        // Обычный взрыв из примера и направленный взрыв (конус 18°).
        readonly Explosion BOOOOM_1 = new Explosion(1, 10, 1, 300, 500, false);
        readonly Explosion BOOOOM_2 = new Explosion(1, 10, 1, 55, 900, true);
        readonly LeafFall leaves = new LeafFall(2.5f, 2.5f);

        bool _glReady;
        bool _loading = true;

        public Form1()
        {
            InitializeComponent();
        }

        SceneKind Scene
        {
            get
            {
                int index = comboScene.SelectedIndex;
                if (index < 0)
                    index = 0;
                return (SceneKind)index;
            }
        }

        void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                AnT.InitializeContexts();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "OpenGL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }

            AnT.MakeCurrent();
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glEnable(Gl.GL_LIGHT0);
            Gl.glEnable(Gl.GL_COLOR_MATERIAL);
            Gl.glEnable(Gl.GL_NORMALIZE);
            Gl.glColorMaterial(Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT_AND_DIFFUSE);

            // Камеры 1–3 — как в методических указаниях.
            camera_date[0, 0] = -3; camera_date[0, 1] = 0; camera_date[0, 2] = -20;
            camera_date[0, 3] = 0; camera_date[0, 4] = 1; camera_date[0, 5] = 0; camera_date[0, 6] = 0;

            camera_date[1, 0] = -3; camera_date[1, 1] = 2; camera_date[1, 2] = -20;
            camera_date[1, 3] = 30; camera_date[1, 4] = 1; camera_date[1, 5] = 0; camera_date[1, 6] = 0;

            camera_date[2, 0] = -3; camera_date[2, 1] = 2; camera_date[2, 2] = -20;
            camera_date[2, 3] = 30; camera_date[2, 4] = 1; camera_date[2, 5] = 1; camera_date[2, 6] = 0;

            comboScene.SelectedIndex = 0;
            comboCamera.SelectedIndex = 1;
            comboMode.SelectedIndex = 0;

            Console.WriteLine("Vendor: " + Gl.glGetString(Gl.GL_VENDOR));
            Console.WriteLine("Renderer: " + Gl.glGetString(Gl.GL_RENDERER));
            Console.WriteLine("Version: " + Gl.glGetString(Gl.GL_VERSION));

            _loading = false;
            _glReady = true;
            ApplySceneChrome();
            LogScene();

            RenderTimer.Start();
            Draw();
        }

        void RenderTimer_Tick(object sender, EventArgs e)
        {
            global_time += RenderTimer.Interval / 1000f;
            Draw();
        }

        void Draw()
        {
            if (!_glReady || AnT.Width < 2 || AnT.Height < 2)
                return;

            AnT.MakeCurrent();

            bool lightBackground = comboMode.SelectedIndex == 0;
            if (lightBackground)
                Gl.glClearColor(0.96f, 0.95f, 0.92f, 1);
            else
                Gl.glClearColor(0.07f, 0.08f, 0.09f, 1);

            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            Gl.glViewport(0, 0, AnT.Width, AnT.Height);

            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluPerspective(45, (float)AnT.Width / Math.Max(1, AnT.Height), 0.1, 200);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();

            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glEnable(Gl.GL_LIGHT0);

            if (lightBackground)
                Gl.glColor3d(0.12, 0.12, 0.12);
            else
                Gl.glColor3d(0.92, 0.92, 0.9);

            Gl.glPushMatrix();
            int camera = comboCamera.SelectedIndex;
            if (camera < 0)
                camera = 0;
            if (camera < 3)
            {
                Gl.glTranslatef(camera_date[camera, 0], camera_date[camera, 1], camera_date[camera, 2]);
                Gl.glRotatef(camera_date[camera, 3], camera_date[camera, 4], camera_date[camera, 5], camera_date[camera, 6]);
            }
            else
            {
                SetupExtraCamera(camera);
            }

            Gl.glPushMatrix();
            switch (Scene)
            {
                case SceneKind.Explosion:
                    Gl.glDisable(Gl.GL_LIGHTING);
                    DrawMatrix(5, 0);
                    Gl.glEnable(Gl.GL_LIGHTING);
                    BOOOOM_1.RotateParticles = chkSpin.Checked;
                    BOOOOM_1.Calculate(global_time);
                    break;
                case SceneKind.DirectedExplosion:
                    Gl.glDisable(Gl.GL_LIGHTING);
                    DrawMatrix(30, -8);
                    Gl.glEnable(Gl.GL_LIGHTING);
                    BOOOOM_2.DrawDirectionMarker();
                    if (lightBackground)
                        Gl.glColor3d(0.12, 0.12, 0.12);
                    else
                        Gl.glColor3d(0.92, 0.92, 0.9);
                    BOOOOM_2.RotateParticles = chkSpin.Checked;
                    BOOOOM_2.Calculate(global_time);
                    break;
                case SceneKind.LeafFall:
                    Gl.glDisable(Gl.GL_LIGHTING);
                    leaves.DrawVolume(lightBackground);
                    Gl.glEnable(Gl.GL_LIGHTING);
                    leaves.Calculate(global_time);
                    break;
            }
            Gl.glPopMatrix();
            Gl.glPopMatrix();

            Gl.glFlush();
            AnT.SwapBuffers();
        }

        // Камеры 4 и 5 показывают направленность взрыва: сбоку и вдоль оси.
        void SetupExtraCamera(int camera)
        {
            if (Scene == SceneKind.LeafFall)
            {
                float cx = leaves.Center[0];
                float cz = leaves.Center[2];
                if (camera == 3)
                    Glu.gluLookAt(cx + 18, 6.2, cz + 3, cx, 4.2, cz, 0, 1, 0);
                else
                    Glu.gluLookAt(cx - 3, 15.5, cz + 16, cx, 2.2, cz, 0, 1, 0);
                return;
            }

            float[] epicenter = Scene == SceneKind.DirectedExplosion ? BOOOOM_2.Position : BOOOOM_1.Position;
            float[] direction = BOOOOM_2.Direction;
            float hLen = (float)Math.Sqrt(direction[0] * direction[0] + direction[2] * direction[2]);
            if (hLen < 1e-4f)
                hLen = 1f;
            float hx = direction[0] / hLen;
            float hz = direction[2] / hLen;
            float px = -hz;
            float pz = hx;
            float tx = epicenter[0] + hx * 8f;
            float ty = 3f;
            float tz = epicenter[2] + hz * 8f;

            if (camera == 3)
                Glu.gluLookAt(tx + px * 26, 4, tz + pz * 26, tx, ty, tz, 0, 1, 0);
            else
                Glu.gluLookAt(
                    epicenter[0] - hx * 9, 6, epicenter[2] - hz * 9,
                    epicenter[0] + hx * 10, 3, epicenter[2] + hz * 10,
                    0, 1, 0);
        }

        static void DrawMatrix(int x, float offset)
        {
            float quadSize = 1;
            Gl.glBegin(Gl.GL_LINES);
            for (int ax = 0; ax < x + 1; ax++)
            {
                Gl.glVertex3d(offset + quadSize * ax, 0, offset);
                Gl.glVertex3d(offset + quadSize * ax, 0, offset + quadSize * x);
            }
            for (int bx = 0; bx < x + 1; bx++)
            {
                Gl.glVertex3d(offset, 0, offset + quadSize * bx);
                Gl.glVertex3d(offset + quadSize * x, 0, offset + quadSize * bx);
            }
            Gl.glEnd();
        }

        void btnStart_Click(object sender, EventArgs e)
        {
            if (!_glReady)
                return;

            AnT.MakeCurrent();
            if (Scene == SceneKind.LeafFall)
            {
                leaves.Reset();
                Console.WriteLine("Листопад заново: t = " + global_time.ToString("0.00") + " с");
                return;
            }

            TriggerBurst();
        }

        void TriggerBurst()
        {
            Random rnd = new Random();
            Explosion boom = Scene == SceneKind.DirectedExplosion ? BOOOOM_2 : BOOOOM_1;
            boom.SetNewPosition(rnd.Next(1, 5), rnd.Next(1, 5), rnd.Next(1, 5));
            boom.SetNewPower(rnd.Next(20, 80));
            boom.Boooom(global_time);
            Console.WriteLine("Взрыв: t = " + global_time.ToString("0.00") + " с");
        }

        void comboScene_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading)
                return;
            ApplySceneChrome();
            LogScene();
        }

        void ApplySceneChrome()
        {
            bool leavesScene = Scene == SceneKind.LeafFall;
            btnStart.Text = leavesScene ? "Сыпать заново" : "Запустить";
            chkSpin.Enabled = !leavesScene;

            switch (Scene)
            {
                case SceneKind.Explosion:
                    lblHint.Text = "500 частиц из одной точки, мощность 20–80. От земли отскакивают и теряют 40 % скорости. Сетка 5×5. Флажок крутит осколки в полёте.";
                    break;
                case SceneKind.DirectedExplosion:
                    lblHint.Text = "900 частиц в конусе 18° вдоль (0,61; 0,67; 0,43). Сетка 30×30. Камеры 4 и 5 — сбоку и вдоль струи. Красная черта показывает ось.";
                    break;
                default:
                    lblHint.Text = "Листья падают внутри параллелепипеда, крутятся и рыскают вбок по двум синусам — траектория не прямая. У дна ложатся, потом появляются снова.";
                    break;
            }
        }

        void LogScene()
        {
            string name;
            switch (Scene)
            {
                case SceneKind.Explosion: name = "обычный взрыв"; break;
                case SceneKind.DirectedExplosion: name = "направленный взрыв"; break;
                default: name = "листопад (вариант 9)"; break;
            }
            Console.WriteLine("Сцена: " + name);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            var area = Screen.PrimaryScreen.WorkingArea;
            if (area.Width >= Width + 300)
                Location = new System.Drawing.Point(area.Right - Width - 16, area.Top + 28);
        }
    }
}
