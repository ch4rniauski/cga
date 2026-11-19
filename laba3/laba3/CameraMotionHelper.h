#pragma once

//чувствительность мыши при повороте сцены
#define MOUSE_SENSITIVITY 0.4f
//чувствительность колеса мыши
#define WHEEL_SENSITIVITY 0.6f

//описание объекта-помощника для поворота сцены с помощью курсора мыши
class CameraMotionHelper
{
private:
	//переменная хранит состояние левой кнопки мыши
	bool leftButtonDown;
	//текущая и предыдущая позиции курсора (x и y)
	int currentPosition[2];
	int lastPosition[2];

	//углы поворота сцены, вычисленные из позиции курсора
	float rotationY;
	float rotationXZ;

	//расстояние от наблюдателя до центра сцены
	float distance;

public:
	float getRotationY() const { return rotationY; }
	float getRotationXZ() const { return rotationXZ; }
	float getDistance() const { return distance; }

	//нажата левая кнопка мыши – начинаем поворот сцены
	void startMove(int x, int y)
	{
		leftButtonDown = true;
		lastPosition[0] = currentPosition[0] = x;
		lastPosition[1] = currentPosition[1] = y;
	}

	//движение курсора мыши с нажатой левой кнопкой
	void move(int x, int y)
	{
		if (!leftButtonDown) return;

		lastPosition[0] = currentPosition[0];
		lastPosition[1] = currentPosition[1];
		currentPosition[0] = x;
		currentPosition[1] = y;

		rotationY -= (currentPosition[0] - lastPosition[0]) * MOUSE_SENSITIVITY;
		rotationXZ += (currentPosition[1] - lastPosition[1]) * MOUSE_SENSITIVITY;
		if (rotationXZ > 85.0f) rotationXZ = 85.0f;
		if (rotationXZ < -85.0f) rotationXZ = -85.0f;
	}

	void endMove() { leftButtonDown = false; }

	//изменение дистанции от наблюдателя до наблюдаемой точки (колесо мыши)
	void increaseDistance() { if (distance < 40.0f) distance += WHEEL_SENSITIVITY; }
	void decreaseDistance() { if (distance > 3.0f) distance -= WHEEL_SENSITIVITY; }

	CameraMotionHelper()
	{
		leftButtonDown = false;
		lastPosition[0] = lastPosition[1] = 0;
		currentPosition[0] = currentPosition[1] = 0;
		rotationY = 32.0f;
		rotationXZ = 22.0f;
		distance = 11.0f;
	}
};
