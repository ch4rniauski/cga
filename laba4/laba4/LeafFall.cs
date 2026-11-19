using System;

namespace ParticlesSystems
{
    // Вариант 9: листопад внутри заданного объёма.
    // Лист крутится вокруг своей оси и падает не по прямой, а по волнистой траектории.
    class LeafFall
    {
        const int Count = 320;
        const float Half = 6f;
        const float Height = 11f;

        public float[] Center = new float[3];

        struct Leaf
        {
            public float AnchorX, AnchorZ;
            public float AmpX, AmpZ;
            public float Omega, Phase;
            public float Fall;
            public float Y;
            public float Angle, Spin;
            public float Ax, Ay, Az;
            public float Heading;
            public float Size;
            public float R, G, B;
            public float LandX, LandZ;
            public float Rest;
            public bool Settled;
        }

        Leaf[] _leaves = new Leaf[Count];
        readonly Random rnd = new Random();
        float _lastTime = -1f;
        bool _listReady;
        int _list;

        public LeafFall(float centerX, float centerZ)
        {
            Center[0] = centerX;
            Center[1] = 0;
            Center[2] = centerZ;
            for (int i = 0; i < _leaves.Length; i++)
                Spawn(ref _leaves[i], true);
        }

        public void Reset()
        {
            _lastTime = -1f;
            for (int i = 0; i < _leaves.Length; i++)
                Spawn(ref _leaves[i], true);
        }

        public void Calculate(float time)
        {
            float dTime = _lastTime < 0 ? 0 : time - _lastTime;
            if (dTime < 0)
                dTime = 0;
            _lastTime = time;

            if (!_listReady)
                CreateLeafList();

            for (int i = 0; i < _leaves.Length; i++)
            {
                Leaf s = _leaves[i];
                if (!s.Settled)
                {
                    // Вертикальная скорость то быстрее, то медленнее — падение не равномерное.
                    float wobble = 1f + 0.42f * (float)Math.Sin(time * 2.1 + s.Phase);
                    s.Y -= s.Fall * wobble * dTime;
                    s.Angle = (s.Angle + s.Spin * dTime) % 360f;

                    float x, z;
                    Flutter(s, time, out x, out z);
                    if (s.Y <= 0f)
                    {
                        s.Y = 0f;
                        s.Settled = true;
                        s.LandX = x;
                        s.LandZ = z;
                        s.Rest = Rand(1.4f, 3.4f);
                    }
                }
                else
                {
                    s.Rest -= dTime;
                    if (s.Rest <= 0f)
                        Spawn(ref s, false);
                }
                _leaves[i] = s;
            }

            DrawLeaves(time);
        }

        public void DrawVolume(bool lightBackground)
        {
            float x0 = Center[0] - Half;
            float x1 = Center[0] + Half;
            float z0 = Center[2] - Half;
            float z1 = Center[2] + Half;
            float y1 = Height;

            if (lightBackground)
                Gl.glColor3d(0.42, 0.30, 0.16);
            else
                Gl.glColor3d(0.86, 0.72, 0.48);

            Gl.glBegin(Gl.GL_LINES);

            // Рёбра параллелепипеда — границы заданного объёма.
            Edge(x0, 0, z0, x1, 0, z0);
            Edge(x1, 0, z0, x1, 0, z1);
            Edge(x1, 0, z1, x0, 0, z1);
            Edge(x0, 0, z1, x0, 0, z0);

            Edge(x0, y1, z0, x1, y1, z0);
            Edge(x1, y1, z0, x1, y1, z1);
            Edge(x1, y1, z1, x0, y1, z1);
            Edge(x0, y1, z1, x0, y1, z0);

            Edge(x0, 0, z0, x0, y1, z0);
            Edge(x1, 0, z0, x1, y1, z0);
            Edge(x1, 0, z1, x1, y1, z1);
            Edge(x0, 0, z1, x0, y1, z1);

            // Сетка на дне объёма.
            for (float x = x0; x <= x1 + 0.01f; x += 1f)
            {
                Gl.glVertex3d(x, 0, z0);
                Gl.glVertex3d(x, 0, z1);
            }
            for (float z = z0; z <= z1 + 0.01f; z += 1f)
            {
                Gl.glVertex3d(x0, 0, z);
                Gl.glVertex3d(x1, 0, z);
            }

            Gl.glEnd();
        }

        static void Edge(float x1, float y1, float z1, float x2, float y2, float z2)
        {
            Gl.glVertex3d(x1, y1, z1);
            Gl.glVertex3d(x2, y2, z2);
        }

        void DrawLeaves(float time)
        {
            foreach (Leaf s in _leaves)
            {
                float x, z;
                if (s.Settled)
                {
                    x = s.LandX;
                    z = s.LandZ;
                }
                else
                {
                    Flutter(s, time, out x, out z);
                }

                Gl.glColor3f(s.R, s.G, s.B);
                Gl.glPushMatrix();
                Gl.glTranslatef(Center[0] + x, s.Y + 0.02f, Center[2] + z);
                if (s.Settled)
                {
                    Gl.glRotatef(s.Heading, 0, 1, 0);
                    Gl.glRotatef(78f, 1, 0, 0);
                }
                else
                {
                    Gl.glRotatef(s.Angle, s.Ax, s.Ay, s.Az);
                }
                Gl.glScalef(s.Size, s.Size, s.Size);
                Gl.glCallList(_list);
                Gl.glPopMatrix();
            }
        }

        // Сумма двух синусов с разной частотой: лист рыскает вбок, а не летит по прямой.
        void Flutter(Leaf s, float time, out float x, out float z)
        {
            float sx = (float)Math.Sin(s.Omega * time + s.Phase);
            float sz = (float)Math.Sin(s.Omega * 1.7 * time + s.Phase * 1.3);
            float extra = (float)Math.Sin(s.Omega * 0.47 * time);
            x = s.AnchorX + s.AmpX * sx + 0.28f * s.AmpX * extra;
            z = s.AnchorZ + s.AmpZ * sz;

            float limit = Half - 1.15f;
            if (x > limit) x = limit;
            if (x < -limit) x = -limit;
            if (z > limit) z = limit;
            if (z < -limit) z = -limit;
        }

        void Spawn(ref Leaf s, bool anywhere)
        {
            s.AmpX = Rand(0.45f, 1.35f);
            s.AmpZ = Rand(0.45f, 1.35f);
            float margin = Math.Max(s.AmpX, s.AmpZ) + 0.4f;
            s.AnchorX = Rand(-Half + margin, Half - margin);
            s.AnchorZ = Rand(-Half + margin, Half - margin);
            s.Omega = Rand(0.9f, 2.4f);
            s.Phase = Rand(0f, (float)(Math.PI * 2));
            s.Fall = Rand(1.05f, 2.5f);
            s.Y = anywhere ? Rand(0.4f, Height - 1.2f) : Height - 0.8f;
            s.Angle = Rand(0f, 360f);
            s.Spin = Rand(90f, 320f);
            s.Heading = Rand(0f, 360f);
            s.Size = Rand(1.15f, 1.9f);
            s.Settled = false;
            s.Rest = 0;

            float ax = Rand(-1f, 1f);
            float ay = Rand(-1f, 1f);
            float az = Rand(-1f, 1f);
            float len = (float)Math.Sqrt(ax * ax + ay * ay + az * az);
            if (len < 1e-3f)
            {
                ax = 0; ay = 1; az = 0; len = 1;
            }
            s.Ax = ax / len;
            s.Ay = ay / len;
            s.Az = az / len;

            switch (rnd.Next(5))
            {
                case 0: s.R = 0.78f; s.G = 0.22f; s.B = 0.08f; break;
                case 1: s.R = 0.90f; s.G = 0.48f; s.B = 0.10f; break;
                case 2: s.R = 0.86f; s.G = 0.74f; s.B = 0.18f; break;
                case 3: s.R = 0.42f; s.G = 0.55f; s.B = 0.16f; break;
                default: s.R = 0.48f; s.G = 0.28f; s.B = 0.12f; break;
            }
        }

        void CreateLeafList()
        {
            _list = Gl.glGenLists(1);
            Gl.glNewList(_list, Gl.GL_COMPILE);
            DrawLeafSide(1f);
            DrawLeafSide(-1f);
            Gl.glEndList();
            _listReady = true;
        }

        static void DrawLeafSide(float normalY)
        {
            Gl.glBegin(Gl.GL_TRIANGLES);
            Gl.glNormal3f(0f, normalY, 0.15f);
            // Пластинка листа: вытянутый контур, чтобы вращение было видно по форме.
            Gl.glVertex3f(0f, 0f, 0.42f);
            Gl.glVertex3f(0.16f * normalY, 0f, 0.08f);
            Gl.glVertex3f(0f, 0f, -0.06f);

            Gl.glVertex3f(0f, 0f, 0.42f);
            Gl.glVertex3f(0f, 0f, -0.06f);
            Gl.glVertex3f(-0.16f * normalY, 0f, 0.08f);

            Gl.glVertex3f(0.16f * normalY, 0f, 0.08f);
            Gl.glVertex3f(0.07f * normalY, 0f, -0.28f);
            Gl.glVertex3f(0f, 0f, -0.06f);

            Gl.glVertex3f(-0.16f * normalY, 0f, 0.08f);
            Gl.glVertex3f(0f, 0f, -0.06f);
            Gl.glVertex3f(-0.07f * normalY, 0f, -0.28f);

            Gl.glVertex3f(0.07f * normalY, 0f, -0.28f);
            Gl.glVertex3f(0f, 0f, -0.40f);
            Gl.glVertex3f(0f, 0f, -0.06f);

            Gl.glVertex3f(-0.07f * normalY, 0f, -0.28f);
            Gl.glVertex3f(0f, 0f, -0.06f);
            Gl.glVertex3f(0f, 0f, -0.40f);
            Gl.glEnd();
        }

        float Rand(float a, float b)
        {
            return a + (float)rnd.NextDouble() * (b - a);
        }
    }
}
