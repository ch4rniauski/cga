using System;

namespace ParticlesSystems
{
    // Взрыв из методички и тот же взрыв, собранный в конус заданного направления.
    class Explosion
    {
        float[] position = new float[3];
        float _power;
        int MAX_PARTICLES = 1000;
        int _particles_now;
        bool isStart;
        Partlice[] PartliceArray;

        bool isDisplayList;
        int DisplayListNom;
        readonly Random rnd = new Random();

        public bool Directed;
        public bool RotateParticles;
        public float ConeAngle = 18f;
        public float[] Direction = new float[3];

        public float[] Position
        {
            get { return position; }
        }

        public Explosion(float x, float y, float z, float power, int particleCount, bool directed)
        {
            position[0] = x;
            position[1] = y;
            position[2] = z;
            _power = power;

            if (particleCount > MAX_PARTICLES)
                particleCount = MAX_PARTICLES;
            if (particleCount < 1)
                particleCount = 1;

            _particles_now = particleCount;
            PartliceArray = new Partlice[particleCount];

            Directed = directed;
            if (directed)
            {
                // Произвольное направление: вверх и по диагонали между осями X и Z.
                float dx = 1f, dy = 1.1f, dz = 0.7f;
                float len = (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
                Direction[0] = dx / len;
                Direction[1] = dy / len;
                Direction[2] = dz / len;
            }
            else
            {
                Direction[0] = 0;
                Direction[1] = 1;
                Direction[2] = 0;
            }
        }

        public void SetNewPosition(float x, float y, float z)
        {
            position[0] = x;
            position[1] = y;
            position[2] = z;
        }

        public void SetNewPower(float newPower)
        {
            _power = newPower;
        }

        void CreateDisplayList()
        {
            DisplayListNom = Gl.glGenLists(1);
            Gl.glNewList(DisplayListNom, Gl.GL_COMPILE);
            Gl.glBegin(Gl.GL_TRIANGLES);
            // Нормаль нужна, чтобы при вращении треугольника менялась освещённость.
            Gl.glNormal3f(0.577f, -0.577f, 0.577f);
            Gl.glVertex3d(0, 0, 0);
            Gl.glVertex3d(0.02f, 0.02f, 0);
            Gl.glVertex3d(0.02f, 0, -0.02f);
            Gl.glEnd();
            Gl.glEndList();
            isDisplayList = true;
        }

        public void Boooom(float timeStart)
        {
            if (!isDisplayList)
                CreateDisplayList();

            for (int ax = 0; ax < _particles_now; ax++)
            {
                PartliceArray[ax] = new Partlice(position[0], position[1], position[2], 5.0f, 10, timeStart);

                float powerRnd = rnd.Next((int)_power / 20, (int)_power);
                PartliceArray[ax].setAttenuation(_power * (Directed ? 1.4f : 2.0f));

                if (Directed)
                {
                    // Случайное направление внутри конуса: к оси взрыва добавляется
                    // отклонение не больше tg(половина угла раствора).
                    float spread = (float)Math.Tan(ConeAngle * Math.PI / 180.0);
                    float dx, dy, dz, len;
                    do
                    {
                        dx = Rand(-1f, 1f);
                        dy = Rand(-1f, 1f);
                        dz = Rand(-1f, 1f);
                        len = dx * dx + dy * dy + dz * dz;
                    }
                    while (len > 1f || len < 1e-4f);

                    float k = spread * (float)Math.Sqrt(rnd.NextDouble()) / (float)Math.Sqrt(len);
                    dx = Direction[0] + dx * k;
                    dy = Direction[1] + dy * k;
                    dz = Direction[2] + dz * k;
                    len = (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);

                    // У направленного взрыва разброс мощности меньше: 60–100 %.
                    float m = _power * Rand(0.6f, 1.0f) / len;
                    PartliceArray[ax].SetPower(dx * m, dy * m, dz * m);
                }
                else
                {
                    int directionX = rnd.Next(1, 3) == 2 ? -1 : 1;
                    int directionY = rnd.Next(1, 3) == 2 ? -1 : 1;
                    int directionZ = rnd.Next(1, 3) == 2 ? -1 : 1;

                    PartliceArray[ax].SetPower(
                        powerRnd * (rnd.Next(100, 1000) / 1000.0f) * directionX,
                        powerRnd * (rnd.Next(100, 1000) / 1000.0f) * directionY,
                        powerRnd * (rnd.Next(100, 1000) / 1000.0f) * directionZ);
                }

                PartliceArray[ax].SetRotation(Rand(-1f, 1f), Rand(-1f, 1f), Rand(-1f, 1f), Rand(180f, 720f));
            }

            isStart = true;
        }

        public void Calculate(float time)
        {
            if (!isStart)
                return;

            for (int ax = 0; ax < _particles_now; ax++)
            {
                Partlice p = PartliceArray[ax];
                if (p == null || !p.isLife())
                    continue;

                p.UpdatePosition(time);

                // Сначала отскок, потом рисуем — частица не оказывается под сеткой.
                if (p.GetPositionY() < 0 && p.GetSpeed(1) < 0)
                {
                    p.ClampPositionY(0);
                    p.InvertSpeed(1, 0.6f);
                }

                Gl.glPushMatrix();
                float size = p.GetSize();
                Gl.glTranslatef(p.GetPositionX(), p.GetPositionY(), p.GetPositionZ());
                if (RotateParticles)
                {
                    float[] axis = p.GetRotationAxis();
                    Gl.glRotatef(p.GetRotationAngle(), axis[0], axis[1], axis[2]);
                }
                Gl.glScalef(size, size, size);
                Gl.glCallList(DisplayListNom);
                Gl.glPopMatrix();
            }
        }

        public void DrawDirectionMarker()
        {
            const float len = 5.5f;
            Gl.glDisable(Gl.GL_LIGHTING);
            Gl.glColor3d(0.82, 0.14, 0.12);
            Gl.glLineWidth(2.2f);
            Gl.glBegin(Gl.GL_LINES);
            Gl.glVertex3d(position[0], position[1], position[2]);
            Gl.glVertex3d(
                position[0] + Direction[0] * len,
                position[1] + Direction[1] * len,
                position[2] + Direction[2] * len);
            Gl.glEnd();
            Gl.glLineWidth(1f);
            Gl.glEnable(Gl.GL_LIGHTING);
        }

        float Rand(float a, float b)
        {
            return a + (float)rnd.NextDouble() * (b - a);
        }
    }
}
