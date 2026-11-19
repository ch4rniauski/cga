#define NOMINMAX
#include <windows.h>
#include <cstdio>
#include <cmath>
#include <cctype>
#include <cwchar>
#include <string>
#include <GL/freeglut.h>
#include <glm/glm.hpp>
#include <glm/gtc/type_ptr.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include "CameraMotionHelper.h"
#include "ModelDescription.h"
#include "Text3D.h"

using namespace glm;

#define ESCAPE '\033'

//начальное разрешение окна
#define WINDOW_WIDTH 1100
#define WINDOW_HEIGHT 720

//фамилия латиницей (задание 6)
static const char* SURNAME = "Charniauski";

constexpr float PI = 3.14159265f;

//режимы отображения – пункты задания
enum Mode
{
	MODE_EXAMPLE_LINE = 1,
	MODE_EXAMPLE_SQUARE,
	MODE_FLAT_ARROW,
	MODE_EXTRUDE_COPY,
	MODE_EXTRUDE_EDGES,
	MODE_SOLID,
	MODE_ANIMATION,
	MODE_COUNT = MODE_ANIMATION
};

static const wchar_t* modeTitles[] =
{
	L"",
	L"Пример 1: линия из точек GL_POINTS",
	L"Пример 2: квадрат GL_POLYGON с интерполяцией цвета",
	L"Примитив варианта 9: двусторонняя стрелка",
	L"Выдавливание: копия основания перенесена вдоль оси Z",
	L"Выдавливание: соединение рёбер оснований",
	L"Объёмная фигура с освещением",
	L"Анимация и фамилия",
};

//состояние анимаций варианта 9
struct AnimationState
{
	bool scaleEnabled = true;	//анимация 1 – изменение размера вдоль оси Z
	bool rotateEnabled = true;	//анимация 2 – вращение вокруг своей оси Y
	bool paused = false;
	float phase = 0.0f;			//фаза синуса для изменения размера
	float angle = 0.0f;			//угол поворота вокруг своей оси Y

	//коэффициент масштаба по Z: sz = 1 + 0,7·sin(φ), от 0,3 до 1,7
	float scaleZ() const { return scaleEnabled ? 1.0f + 0.7f * sinf(phase) : 1.0f; }

	void reset()
	{
		phase = 0.0f;
		angle = 0.0f;
	}

	void update(bool force = false)
	{
		if (paused && !force) return;
		if (scaleEnabled) phase = fmodf(phase + 0.035f, 2.0f * PI);
		if (rotateEnabled) angle = fmodf(angle + 1.2f, 360.0f);
	}
};

CameraMotionHelper cameraMotionHelper;
AnimationState animation;
Text3D text3D;

int mode = MODE_EXAMPLE_LINE;
bool showAxes = true;
int windowWidth = WINDOW_WIDTH;
int windowHeight = WINDOW_HEIGHT;
GLuint hudFontBase = 0;

//позиция источника света в мировых координатах
const GLfloat lightPos[] = { 3.5f, 3.2f, 3.5f, 1.0f };

//позиции фигуры и надписи в сцене анимации
const vec3 figurePos(0.6f, -1.1f, 0.0f);
const vec3 labelPos(0.6f, 1.5f, 0.0f);

void printExtList(const char* extensions)
{
	//выводим только первые расширения, чтобы сведения о драйвере остались на экране
	const int shownLimit = 12;
	char name[1024];
	int i = 0, j = 0, count = 0;

	for (i = 0; extensions[i] != '\0'; i++)
		if (!isspace((unsigned char)extensions[i]) && (i == 0 || isspace((unsigned char)extensions[i - 1])))
			count++;

	printf("Supported extensions (%d): \n", count);
	int printed = 0;
	for (i = 0, j = 0; extensions[i] != '\0' && printed < shownLimit; i++)
	{
		if (!isspace((unsigned char)extensions[i]))
			name[j++] = extensions[i];
		else if (j > 0)
		{
			name[j] = '\0';
			printf("\t%s\n", name);
			printed++;
			j = 0;
		}
	}
	if (j > 0 && printed < shownLimit)
	{
		name[j] = '\0';
		printf("\t%s\n", name);
		printed++;
	}
	if (count > printed)
		printf("\t... и ещё %d\n", count - printed);
}

// ---------------------------------------------------------------------------
// Экранный текст (подписи режимов и параметры кадра)
// ---------------------------------------------------------------------------

//растровый шрифт Windows: дисплейные списки для символов 0..0x45F (латиница и кириллица)
void createHudFont()
{
	HDC hdc = wglGetCurrentDC();
	HFONT font = CreateFontW(-15, 0, 0, 0, FW_SEMIBOLD, FALSE, FALSE, FALSE, DEFAULT_CHARSET,
		OUT_TT_PRECIS, CLIP_DEFAULT_PRECIS, ANTIALIASED_QUALITY, FIXED_PITCH | FF_MODERN, L"Consolas");
	HGDIOBJ oldFont = SelectObject(hdc, font);
	hudFontBase = glGenLists(0x460);
	if (!wglUseFontBitmapsW(hdc, 0, 0x460, hudFontBase))
		wglUseFontBitmapsW(hdc, 0, 0x460, hudFontBase);
	SelectObject(hdc, oldFont);
	DeleteObject(font);
}

void hudText(int x, int y, const std::wstring& text)
{
	glRasterPos2i(x, y);
	glListBase(hudFontBase);
	glCallLists((GLsizei)text.size(), GL_UNSIGNED_SHORT, text.c_str());
}

void hudPanel(int x, int y, int w, int h, float alpha)
{
	glEnable(GL_BLEND);
	glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
	glColor4f(0.05f, 0.06f, 0.10f, alpha);
	glBegin(GL_QUADS);
	glVertex2i(x, y); glVertex2i(x + w, y); glVertex2i(x + w, y + h); glVertex2i(x, y + h);
	glEnd();
	//акцентная полоска слева
	glColor4f(0.95f, 0.30f, 0.30f, 0.9f);
	glBegin(GL_QUADS);
	glVertex2i(x, y); glVertex2i(x + 4, y); glVertex2i(x + 4, y + h); glVertex2i(x, y + h);
	glEnd();
	glDisable(GL_BLEND);
}

void begin2DOverlay()
{
	glDisable(GL_LIGHTING);
	glDisable(GL_DEPTH_TEST);
	glMatrixMode(GL_PROJECTION);
	glLoadIdentity();
	gluOrtho2D(0, windowWidth, 0, windowHeight);
	glMatrixMode(GL_MODELVIEW);
	glLoadIdentity();
}

void drawHud()
{
	begin2DOverlay();
	wchar_t buf[256];
	const int top = windowHeight;

	//заголовок режима
	const bool animMode = (mode == MODE_ANIMATION);
	const int panelH = animMode ? 86 : 50;
	hudPanel(12, top - 12 - panelH, 560, panelH, 0.78f);
	glColor3f(1.0f, 0.55f, 0.50f);
	swprintf(buf, 256, L"Лабораторная работа 3 · Вариант 9 · режим %d из %d", mode, (int)MODE_COUNT);
	hudText(26, top - 34, buf);
	glColor3f(0.93f, 0.94f, 0.97f);
	hudText(26, top - 54, modeTitles[mode]);
	if (animMode)
	{
		swprintf(buf, 256, L"[A] Анимация 1: изменение размера вдоль оси Z : %ls",
			animation.scaleEnabled ? L"вкл" : L"выкл");
		hudText(26, top - 74, buf);
		swprintf(buf, 256, L"[R] Анимация 2: вращение вокруг своей оси Y   : %ls",
			animation.rotateEnabled ? L"вкл" : L"выкл");
		hudText(26, top - 92, buf);
	}

	//параметры кадра
	if (mode >= MODE_EXTRUDE_COPY)
	{
		int lines = animMode ? 4 : 2;
		int h = 14 + lines * 19;
		hudPanel(12, 44, 330, h, 0.78f);
		glColor3f(0.80f, 0.86f, 0.95f);
		int y = 44 + h - 22;
		if (animMode)
		{
			swprintf(buf, 256, L"φ       = %6.3f рад", animation.phase);
			hudText(26, y, buf); y -= 19;
			swprintf(buf, 256, L"sz      = %6.3f", animation.scaleZ());
			hudText(26, y, buf); y -= 19;
			swprintf(buf, 256, L"угол Y  = %6.1f°", animation.angle);
			hudText(26, y, buf); y -= 19;
		}
		swprintf(buf, 256, L"камера  = %.0f° / %.0f°, d = %.1f",
			cameraMotionHelper.getRotationY(), cameraMotionHelper.getRotationXZ(),
			cameraMotionHelper.getDistance());
		hudText(26, y, buf); y -= 19;
		if (!animMode)
		{
			swprintf(buf, 256, L"модель  = %d треугольников, %d вершин",
				model::TriangleCount, model::VertexCount);
			hudText(26, y, buf);
		}
	}

	//подсказка по управлению
	hudPanel(0, 0, windowWidth, 30, 0.85f);
	glColor3f(0.70f, 0.74f, 0.82f);
	hudText(16, 10, L"1-7 режим · ЛКМ поворот · колесо приближение · A/R анимации · Space пауза · X оси · Esc выход");
}

// ---------------------------------------------------------------------------
// Освещение, материалы, вспомогательные объекты
// ---------------------------------------------------------------------------

//метод настройки источников освещения
void setupLight()
{
	//общая фоновая освещённость
	GLfloat ambientLight[] = { 0.30f, 0.30f, 0.32f, 1.0f };
	glLightModelfv(GL_LIGHT_MODEL_AMBIENT, ambientLight);
	//модель освещения Фонга: фоновая, рассеянная и зеркальная составляющие
	GLfloat ambientLight0[] = { 0.12f, 0.12f, 0.14f, 1.0f };
	GLfloat diffuseLight0[] = { 1.0f, 0.97f, 0.92f, 1.0f };
	GLfloat specularLight0[] = { 1.0f, 1.0f, 1.0f, 1.0f };
	glLightfv(GL_LIGHT0, GL_AMBIENT, ambientLight0);
	glLightfv(GL_LIGHT0, GL_DIFFUSE, diffuseLight0);
	glLightfv(GL_LIGHT0, GL_SPECULAR, specularLight0);
	glEnable(GL_LIGHT0);
}

void init()
{
	glEnable(GL_DEPTH_TEST);
	//приведение нормалей к единичной длине – нужно при масштабировании модели
	glEnable(GL_NORMALIZE);
	glShadeModel(GL_SMOOTH);
	glDisable(GL_COLOR_MATERIAL);
	glHint(GL_LINE_SMOOTH_HINT, GL_NICEST);
	setupLight();
	createHudFont();
	text3D.create(L"Georgia", 0.3f);
}

void setMaterial(const GLfloat* ambient, const GLfloat* diffuse, const GLfloat* specular, GLfloat shininess)
{
	GLfloat emission[] = { 0.0f, 0.0f, 0.0f, 1.0f };
	glMaterialfv(GL_FRONT_AND_BACK, GL_EMISSION, emission);
	glMaterialfv(GL_FRONT_AND_BACK, GL_AMBIENT, ambient);
	glMaterialfv(GL_FRONT_AND_BACK, GL_DIFFUSE, diffuse);
	glMaterialfv(GL_FRONT_AND_BACK, GL_SPECULAR, specular);
	glMaterialf(GL_FRONT_AND_BACK, GL_SHININESS, shininess);
}

//материал фигуры – «рубин»
void rubyMaterial()
{
	GLfloat ambient[] = { 0.25f, 0.03f, 0.04f, 1.0f };
	GLfloat diffuse[] = { 0.78f, 0.10f, 0.13f, 1.0f };
	GLfloat specular[] = { 0.73f, 0.63f, 0.63f, 1.0f };
	setMaterial(ambient, diffuse, specular, 128 * 0.6f);
}

//материал надписи – «золото»
void goldMaterial()
{
	GLfloat ambient[] = { 0.247f, 0.1995f, 0.0745f, 1.0f };
	GLfloat diffuse[] = { 0.75164f, 0.60648f, 0.22648f, 1.0f };
	GLfloat specular[] = { 0.628281f, 0.555802f, 0.366065f, 1.0f };
	setMaterial(ambient, diffuse, specular, 128 * 0.4f);
}

//градиентный фон сцены
void drawBackground()
{
	begin2DOverlay();
	glBegin(GL_QUADS);
	glColor3f(0.02f, 0.02f, 0.05f);
	glVertex2i(0, 0); glVertex2i(windowWidth, 0);
	glColor3f(0.13f, 0.14f, 0.22f);
	glVertex2i(windowWidth, windowHeight); glVertex2i(0, windowHeight);
	glEnd();
}

void axisLabel(GLfloat x, GLfloat y, GLfloat z, const char* text)
{
	glRasterPos3f(x, y, z);
	glutBitmapString(GLUT_BITMAP_HELVETICA_18, (const unsigned char*)text);
}

//метод прорисовки координатных осей, параметр задаёт размер осей
void axis(GLfloat size, GLfloat alpha = 1.0f)
{
	glDisable(GL_LIGHTING);
	glEnable(GL_BLEND);
	glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
	glLineWidth(1.5f);
	glBegin(GL_LINES);
	glColor4f(1.0f, 0.35f, 0.35f, alpha);
	glVertex3f(-size, 0, 0); glVertex3f(size, 0, 0);
	glColor4f(0.35f, 1.0f, 0.45f, alpha);
	glVertex3f(0, -size, 0); glVertex3f(0, size, 0);
	glColor4f(0.40f, 0.60f, 1.0f, alpha);
	glVertex3f(0, 0, -size); glVertex3f(0, 0, size);
	glEnd();

	glColor4f(1.0f, 0.35f, 0.35f, alpha);
	axisLabel(-size, 0, 0, "-X"); axisLabel(size, 0, 0, "+X");
	glColor4f(0.35f, 1.0f, 0.45f, alpha);
	axisLabel(0, -size, 0, "-Y"); axisLabel(0, size, 0, "+Y");
	glColor4f(0.40f, 0.60f, 1.0f, alpha);
	axisLabel(0, 0, -size, "-Z"); axisLabel(0, 0, size, "+Z");
	glDisable(GL_BLEND);
}

//сетка в плоскости XZ под сценой
void floorGrid(GLfloat size, GLfloat y)
{
	glDisable(GL_LIGHTING);
	glEnable(GL_BLEND);
	glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
	glLineWidth(1.0f);
	glColor4f(0.55f, 0.60f, 0.80f, 0.18f);
	glBegin(GL_LINES);
	for (GLfloat t = -size; t <= size + 0.001f; t += 1.0f)
	{
		glVertex3f(t, y, -size); glVertex3f(t, y, size);
		glVertex3f(-size, y, t); glVertex3f(size, y, t);
	}
	glEnd();
	glDisable(GL_BLEND);
}

//метод прорисовки модели «лампочки»
void renderLamp()
{
	glEnable(GL_LIGHTING);
	GLfloat emissionColor[] = { 1.0f, 0.95f, 0.80f, 1.0f };
	GLfloat black[] = { 0.0f, 0.0f, 0.0f, 1.0f };
	glMaterialfv(GL_FRONT, GL_AMBIENT, black);
	glMaterialfv(GL_FRONT, GL_DIFFUSE, black);
	glMaterialfv(GL_FRONT, GL_SPECULAR, black);
	glMaterialfv(GL_FRONT, GL_EMISSION, emissionColor);
	glutSolidSphere(0.18f, 24, 24);
	glMaterialfv(GL_FRONT, GL_EMISSION, black);
}

// ---------------------------------------------------------------------------
// Геометрия стрелки
// ---------------------------------------------------------------------------

void arrowCap(GLfloat z)
{
	glBegin(GL_TRIANGLES);
	for (const auto& t : model::capTriangles)
		for (int k = 0; k < 3; k++)
			glVertex3f(model::outline[t[k]][0], model::outline[t[k]][1], z);
	glEnd();
}

void arrowOutline(GLfloat z)
{
	glBegin(GL_LINE_LOOP);
	for (int i = 0; i < model::OutlineCount; i++)
		glVertex3f(model::outline[i][0], model::outline[i][1], z);
	glEnd();
}

//рёбра, соединяющие основания
void arrowConnectingEdges(GLfloat h)
{
	glBegin(GL_LINES);
	for (int i = 0; i < model::OutlineCount; i++)
	{
		glVertex3f(model::outline[i][0], model::outline[i][1], -h);
		glVertex3f(model::outline[i][0], model::outline[i][1], h);
	}
	glEnd();
}

void arrowVertices(GLfloat z)
{
	glBegin(GL_POINTS);
	for (int i = 0; i < model::OutlineCount; i++)
		glVertex3f(model::outline[i][0], model::outline[i][1], z);
	glEnd();
}

//метод отрисовки объёмной модели по массивам вершин и нормалей
void renderModel()
{
	glEnable(GL_LIGHTING);
	rubyMaterial();

	//смещаем грани в глубину, чтобы рёбра поверх них не мерцали
	glEnable(GL_POLYGON_OFFSET_FILL);
	glPolygonOffset(1.0f, 1.0f);

	glEnableClientState(GL_NORMAL_ARRAY);
	glNormalPointer(GL_FLOAT, 0, model::normalArr.data());
	glEnableClientState(GL_VERTEX_ARRAY);
	glVertexPointer(3, GL_FLOAT, 0, model::vertexArr.data());
	glDrawArrays(GL_TRIANGLES, 0, model::VertexCount);
	glDisableClientState(GL_NORMAL_ARRAY);
	glDisableClientState(GL_VERTEX_ARRAY);

	glDisable(GL_POLYGON_OFFSET_FILL);

	//контур рёбер поверх граней
	glDisable(GL_LIGHTING);
	glEnable(GL_LINE_SMOOTH);
	glColor3f(1.0f, 0.78f, 0.72f);
	glLineWidth(1.5f);
	const GLfloat h = model::Depth / 2.0f;
	arrowOutline(h);
	arrowOutline(-h);
	arrowConnectingEdges(h);
	glDisable(GL_LINE_SMOOTH);
}

// ---------------------------------------------------------------------------
// Режимы отображения
// ---------------------------------------------------------------------------

//пример 1 из методических указаний
void drawExampleLine()
{
	glDisable(GL_DEPTH_TEST);
	glClearColor(1.0f, 1.0f, 1.0f, 0.0f);
	glClear(GL_COLOR_BUFFER_BIT);
	glMatrixMode(GL_PROJECTION);
	glLoadIdentity();
	gluOrtho2D(0.0, 640.0, 0.0, 480.0);
	glMatrixMode(GL_MODELVIEW);
	glLoadIdentity();

	glColor3f(1.0f, 0.0f, 0.0f);
	glPointSize(2.0f);
	// цвет зелёный
	glColor3ub(0, 255, 0);
	// рисуем линию
	for (int i = 100; i < 300; i++)
	{
		glBegin(GL_POINTS);
		glVertex2i(i, i); // x = i, y = i
		glEnd();
	}
	glPointSize(1.0f);
}

//пример 2 из методических указаний
void drawExampleSquare()
{
	glDisable(GL_DEPTH_TEST);
	glClearColor(1.0f, 1.0f, 1.0f, 0.0f);
	glClear(GL_COLOR_BUFFER_BIT);
	glMatrixMode(GL_PROJECTION);
	glLoadIdentity();
	gluOrtho2D(0.0, 1.0, 0.0, 1.0);
	glMatrixMode(GL_MODELVIEW);
	glLoadIdentity();

	glBegin(GL_POLYGON);
	glColor3f(1.0f, 0.0f, 0.0f);
	glVertex3f(0.25f, 0.25f, 0.0f);
	glVertex3f(0.75f, 0.25f, 0.0f);
	glColor3f(0.0f, 1.0f, 0.0f);
	glVertex3f(0.75f, 0.75f, 0.0f);
	glVertex3f(0.25f, 0.75f, 0.0f);
	glEnd();
}

//задание 2 – плоская двусторонняя стрелка
void drawFlatArrow()
{
	drawBackground();
	glMatrixMode(GL_PROJECTION);
	glLoadIdentity();
	//область проецирования с сохранением пропорций окна
	double aspect = (double)windowWidth / (double)windowHeight;
	gluOrtho2D(-3.0 * aspect, 3.0 * aspect, -3.0, 3.0);
	glMatrixMode(GL_MODELVIEW);
	glLoadIdentity();

	//заливка: невыпуклый контур, поэтому выводим треугольники разбиения
	glBegin(GL_TRIANGLES);
	for (const auto& t : model::capTriangles)
		for (int k = 0; k < 3; k++)
		{
			const GLfloat* p = model::outline[t[k]];
			//лёгкий вертикальный градиент цвета
			GLfloat g = (p[1] + 1.0f) / 2.0f;
			glColor3f(0.70f + 0.25f * g, 0.08f + 0.20f * g, 0.12f + 0.15f * g);
			glVertex2f(p[0], p[1]);
		}
	glEnd();

	glEnable(GL_LINE_SMOOTH);
	glLineWidth(3.0f);
	glColor3f(1.0f, 0.85f, 0.80f);
	arrowOutline(0.0f);
	glDisable(GL_LINE_SMOOTH);

	glPointSize(7.0f);
	glColor3f(1.0f, 1.0f, 1.0f);
	arrowVertices(0.0f);
	glPointSize(1.0f);
}

void beginScene3D()
{
	drawBackground();
	glEnable(GL_DEPTH_TEST);
	glClear(GL_DEPTH_BUFFER_BIT);

	glMatrixMode(GL_PROJECTION);
	glLoadIdentity();
	gluPerspective(45.0, (double)windowWidth / (double)windowHeight, 0.5, 200.0);

	glMatrixMode(GL_MODELVIEW);
	glLoadIdentity();
	//положение наблюдателя на сфере радиуса d вокруг центра сцены
	float yaw = radians(cameraMotionHelper.getRotationY());
	float pitch = radians(cameraMotionHelper.getRotationXZ());
	float d = cameraMotionHelper.getDistance();
	gluLookAt(d * cosf(pitch) * sinf(yaw), d * sinf(pitch), d * cosf(pitch) * cosf(yaw),
		0.0, 0.0, 0.0,
		0.0, 1.0, 0.0);

	//позиция задаётся после gluLookAt – источник неподвижен относительно сцены
	glLightfv(GL_LIGHT0, GL_POSITION, lightPos);
}

void drawLampAndAxes(GLfloat axisSize)
{
	floorGrid(6.0f, -2.6f);
	if (showAxes) axis(axisSize);
	glPushMatrix();
	glTranslatef(lightPos[0], lightPos[1], lightPos[2]);
	renderLamp();
	glPopMatrix();
}

//задание 3, шаг 1 – плоская копия переносится вдоль оси Z
void drawExtrudeCopy()
{
	beginScene3D();
	glDisable(GL_LIGHTING);
	floorGrid(6.0f, -2.6f);
	if (showAxes) axis(4.0f);

	const GLfloat h = model::Depth / 2.0f;
	//исходная фигура
	glColor3f(0.45f, 0.10f, 0.14f);
	arrowCap(-h);
	//копия, перенесённая вдоль оси Z
	glColor3f(0.90f, 0.25f, 0.28f);
	arrowCap(h);

	glLineWidth(2.0f);
	glColor3f(1.0f, 0.85f, 0.80f);
	arrowOutline(-h);
	arrowOutline(h);

	//вектор переноса вдоль оси Z
	glLineWidth(3.0f);
	glColor3f(1.0f, 0.85f, 0.25f);
	glBegin(GL_LINES);
	glVertex3f(0.0f, 1.4f, -h);
	glVertex3f(0.0f, 1.4f, h + 0.3f);
	glEnd();
	glPushMatrix();
	glTranslatef(0.0f, 1.4f, h + 0.3f);
	glutSolidCone(0.08, 0.3, 16, 2);
	glPopMatrix();
	glLineWidth(1.0f);
}

//задание 3, шаг 2 – соединение рёбер оснований (каркас)
void drawExtrudeEdges()
{
	beginScene3D();
	glDisable(GL_LIGHTING);
	floorGrid(6.0f, -2.6f);
	if (showAxes) axis(4.0f);

	const GLfloat h = model::Depth / 2.0f;
	glEnable(GL_LINE_SMOOTH);
	glLineWidth(2.0f);
	glColor3f(1.0f, 0.55f, 0.55f);
	arrowOutline(-h);
	arrowOutline(h);
	//новые рёбра – боковые грани призмы
	glColor3f(1.0f, 0.85f, 0.25f);
	arrowConnectingEdges(h);
	glDisable(GL_LINE_SMOOTH);

	glPointSize(6.0f);
	glColor3f(1.0f, 1.0f, 1.0f);
	arrowVertices(-h);
	arrowVertices(h);
	glPointSize(1.0f);
	glLineWidth(1.0f);
}

//задание 4 – объёмная фигура с освещением
void drawSolid()
{
	beginScene3D();
	drawLampAndAxes(4.5f);
	renderModel();
}

//задания 5–6 – анимация фигуры и надписи
void drawAnimated()
{
	beginScene3D();
	drawLampAndAxes(5.0f);

	const float sz = animation.scaleZ();

	//фигура: перенос в позицию, поворот вокруг своей оси Y, масштаб вдоль своей оси Z
	glPushMatrix();
	glTranslatef(figurePos.x, figurePos.y, figurePos.z);
	glRotatef(animation.angle, 0.0f, 1.0f, 0.0f);
	if (showAxes) axis(2.6f, 0.55f);
	glScalef(1.0f, 1.0f, sz);
	renderModel();
	glPopMatrix();

	//надпись: те же преобразования вокруг её собственного центра
	glPushMatrix();
	glTranslatef(labelPos.x, labelPos.y, labelPos.z);
	glRotatef(animation.angle, 0.0f, 1.0f, 0.0f);
	glScalef(0.8f, 0.8f, 0.8f * sz);
	glEnable(GL_LIGHTING);
	goldMaterial();
	glLightModeli(GL_LIGHT_MODEL_TWO_SIDE, GL_TRUE);
	text3D.drawCentered(SURNAME);
	glLightModeli(GL_LIGHT_MODEL_TWO_SIDE, GL_FALSE);
	glPopMatrix();
}

// ---------------------------------------------------------------------------
// Обработчики GLUT
// ---------------------------------------------------------------------------

void display()
{
	glClearColor(0.0f, 0.0f, 0.0f, 0.0f);
	glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
	glViewport(0, 0, windowWidth, windowHeight);

	switch (mode)
	{
	case MODE_EXAMPLE_LINE:   drawExampleLine(); break;
	case MODE_EXAMPLE_SQUARE: drawExampleSquare(); break;
	case MODE_FLAT_ARROW:     drawFlatArrow(); break;
	case MODE_EXTRUDE_COPY:   drawExtrudeCopy(); break;
	case MODE_EXTRUDE_EDGES:  drawExtrudeEdges(); break;
	case MODE_SOLID:          drawSolid(); break;
	case MODE_ANIMATION:      drawAnimated(); break;
	}

	drawHud();
	glutSwapBuffers();
}

void reshape(GLint w, GLint h)
{
	windowWidth = w > 0 ? w : 1;
	windowHeight = h > 0 ? h : 1;
	glViewport(0, 0, windowWidth, windowHeight);
	glutPostRedisplay();
}

void setMode(int newMode)
{
	mode = newMode;
	char title[256];
	WideCharToMultiByte(CP_UTF8, 0, modeTitles[mode], -1, title, sizeof(title), nullptr, nullptr);
	printf("Режим %d: %s\n", mode, title);
}

void keyboardFunc(unsigned char key, int x, int y)
{
	switch (key)
	{
	case ESCAPE:
		exit(0);
	case ' ':
		animation.paused = !animation.paused;
		break;
	case '0':
		animation.reset();
		break;
	//покадровый просмотр анимации на паузе
	case '.':
		if (animation.paused) animation.update(true);
		break;
	//латинская раскладка и те же клавиши в русской (cp1251)
	case 'a': case 'A': case 0xF4: case 0xD4:
		animation.scaleEnabled = !animation.scaleEnabled;
		break;
	case 'r': case 'R': case 0xEA: case 0xCA:
		animation.rotateEnabled = !animation.rotateEnabled;
		break;
	case 'x': case 'X': case 0xF7: case 0xD7:
		showAxes = !showAxes;
		break;
	case '+': case '=':
		cameraMotionHelper.decreaseDistance();
		break;
	case '-': case '_':
		cameraMotionHelper.increaseDistance();
		break;
	default:
		if (key >= '1' && key <= '0' + MODE_COUNT) setMode(key - '0');
		break;
	}
	glutPostRedisplay();
}

//метод получения событий таймера, используется для анимации
void updateAnimation(int value)
{
	if (mode == MODE_ANIMATION) animation.update();
	glutPostRedisplay();
	glutTimerFunc(16, updateAnimation, 0);
}

void mouseFunc(int button, int state, int x, int y)
{
	if (button == GLUT_LEFT_BUTTON)
	{
		if (state == GLUT_DOWN) cameraMotionHelper.startMove(x, y);
		else cameraMotionHelper.endMove();
	}
}

void mouseMotionFunc(int x, int y)
{
	cameraMotionHelper.move(x, y);
}

void mouseWheelFunc(int button, int dir, int x, int y)
{
	if (dir < 0) cameraMotionHelper.increaseDistance();
	else cameraMotionHelper.decreaseDistance();
}

int main(int argc, char** argv)
{
	SetConsoleOutputCP(CP_UTF8);

	glutInit(&argc, argv);
	glutInitDisplayMode(GLUT_DOUBLE | GLUT_RGB | GLUT_DEPTH);
	glutInitWindowSize(WINDOW_WIDTH, WINDOW_HEIGHT);
	glutInitWindowPosition(80, 60);
	glutCreateWindow("Lab 3 - OpenGL");
	SetWindowTextW(WindowFromDC(wglGetCurrentDC()),
		L"ЛР №3 «OpenGL» — вариант 9: двусторонняя стрелка");

	const char* vendor = (const char*)glGetString(GL_VENDOR);
	const char* renderer = (const char*)glGetString(GL_RENDERER);
	const char* version = (const char*)glGetString(GL_VERSION);
	const char* extensions = (const char*)glGetString(GL_EXTENSIONS);

	printf("Лабораторная работа 3. OpenGL. Вариант 9 – двусторонняя стрелка\n");
	printf(" Vendor: %s\n Renderer: %s\n Version: %s\n", vendor, renderer, version);
	printExtList(extensions);

	//построение призмы и расчёт нормалей для вершин модели
	model::buildPrism();
	model::calcModelNormals(model::vertexArr.data(), model::normalArr.data(), model::VertexCount);
	init();

	printf("\nУправление: 1-7 – режим, ЛКМ – поворот, колесо – приближение,\n"
		"A/R – анимации, Space – пауза, . – шаг на паузе, 0 – сброс анимации,\n"
		"X – оси, Esc – выход\n");
	setMode(mode);

	glutDisplayFunc(display);
	glutReshapeFunc(reshape);
	glutKeyboardFunc(keyboardFunc);
	glutMouseWheelFunc(mouseWheelFunc);
	glutTimerFunc(16, updateAnimation, 0);
	glutMouseFunc(mouseFunc);
	glutMotionFunc(mouseMotionFunc);
	glutMainLoop();
	return 0;
}
