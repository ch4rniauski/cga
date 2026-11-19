using System;

namespace ParticlesSystems
{
    // Частица из методических указаний, с добавленным вращением в полёте.
    class Partlice
    {
        float[] position = new float[3];
        float _size;
        float _lifeTime;

        float[] Grav = new float[3];
        float[] power = new float[3];
        float attenuation;
        float[] speed = new float[3];
        float LastTime;

        // Ось, текущий угол (градусы) и угловая скорость (градусы в секунду).
        float[] rotationAxis = new float[3];
        float rotationAngle;
        float angularSpeed;

        public Partlice(float x, float y, float z, float size, float lifeTime, float startTime)
        {
            _size = size;
            _lifeTime = lifeTime;

            position[0] = x;
            position[1] = y;
            position[2] = z;

            speed[0] = 0;
            speed[1] = 0;
            speed[2] = 0;

            Grav[0] = 0;
            Grav[1] = -9.8f;
            Grav[2] = 0;

            attenuation = 3.33f;
            power[0] = 0;
            power[1] = 0;
            power[2] = 0;

            rotationAxis[0] = 0;
            rotationAxis[1] = 1;
            rotationAxis[2] = 0;

            LastTime = startTime;
        }

        public void SetPower(float x, float y, float z)
        {
            power[0] = x;
            power[1] = y;
            power[2] = z;
        }

        public void setAttenuation(float newValue)
        {
            attenuation = newValue;
        }

        // Ось (ax, ay, az) и скорость вращения в градусах в секунду.
        public void SetRotation(float ax, float ay, float az, float degreesPerSecond)
        {
            float len = (float)Math.Sqrt(ax * ax + ay * ay + az * az);
            if (len < 1e-4f)
            {
                ax = 0;
                ay = 1;
                az = 0;
                len = 1;
            }
            rotationAxis[0] = ax / len;
            rotationAxis[1] = ay / len;
            rotationAxis[2] = az / len;
            angularSpeed = degreesPerSecond;
        }

        // Отражение скорости по оси. attenuation здесь — доля скорости, которая сохраняется.
        public void InvertSpeed(int os, float attenuation)
        {
            speed[os] *= -1 * attenuation;
        }

        public void UpdatePosition(float timeNow)
        {
            float dTime = timeNow - LastTime;
            _lifeTime -= dTime;
            LastTime = timeNow;

            for (int a = 0; a < 3; a++)
            {
                // Ускорение затухает и для положительной, и для отрицательной проекции.
                if (power[a] > 0)
                    power[a] = Math.Max(0, power[a] - attenuation * dTime);
                else if (power[a] < 0)
                    power[a] = Math.Min(0, power[a] + attenuation * dTime);

                position[a] += speed[a] * dTime + (Grav[a] + power[a]) * dTime * dTime;
                speed[a] += (Grav[a] + power[a]) * dTime;
            }

            rotationAngle = (rotationAngle + angularSpeed * dTime) % 360f;
        }

        public bool isLife()
        {
            return _lifeTime > 0;
        }

        public float GetSize()
        {
            return _size;
        }

        public float GetPositionX() { return position[0]; }
        public float GetPositionY() { return position[1]; }
        public float GetPositionZ() { return position[2]; }

        public float GetSpeed(int axis)
        {
            return speed[axis];
        }

        public void ClampPositionY(float y)
        {
            position[1] = y;
        }

        public float GetRotationAngle()
        {
            return rotationAngle;
        }

        public float[] GetRotationAxis()
        {
            return rotationAxis;
        }
    }
}
