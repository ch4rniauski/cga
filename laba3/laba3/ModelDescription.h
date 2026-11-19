#pragma once

#include <vector>
#include <GL/freeglut.h>
#include <glm/glm.hpp>
#include <glm/gtc/type_ptr.hpp>

// Вариант 9 – двусторонняя стрелка
namespace model
{
	// количество вершин контура
	constexpr int OutlineCount = 10;
	// толщина объёмной фигуры вдоль оси Z
	constexpr GLfloat Depth = 1.0f;

	// контур стрелки (обход против часовой стрелки)
	const GLfloat outline[OutlineCount][2] =
	{
		{ -2.0f,  0.0f },	// 0 – острие левого наконечника
		{ -1.1f, -1.0f },	// 1
		{ -1.1f, -0.4f },	// 2
		{  1.1f, -0.4f },	// 3
		{  1.1f, -1.0f },	// 4
		{  2.0f,  0.0f },	// 5 – острие правого наконечника
		{  1.1f,  1.0f },	// 6
		{  1.1f,  0.4f },	// 7
		{ -1.1f,  0.4f },	// 8
		{ -1.1f,  1.0f },	// 9
	};

	// фигура невыпуклая, поэтому основание разбито на треугольники вручную:
	// левый наконечник, древко (2 треугольника), правый наконечник
	constexpr int CapTriangleCount = 4;
	const int capTriangles[CapTriangleCount][3] =
	{
		{ 0, 1, 9 },
		{ 2, 3, 7 },
		{ 2, 7, 8 },
		{ 5, 6, 4 },
	};

	// 2 основания по 4 треугольника + 10 боковых граней по 2 треугольника
	constexpr int TriangleCount = CapTriangleCount * 2 + OutlineCount * 2;
	constexpr int VertexCount = TriangleCount * 3;

	//массив треугольников модели
	inline std::vector<GLfloat> vertexArr;
	// значение нормалей для каждой вершины будут
	//рассчитываться в начале работы программы
	inline std::vector<GLfloat> normalArr;

	inline void buildPrism()
	{
		const GLfloat h = Depth / 2.0f;
		vertexArr.clear();
		vertexArr.reserve(VertexCount * 3);

		auto vertex = [](int i, GLfloat z)
		{
			vertexArr.push_back(outline[i % OutlineCount][0]);
			vertexArr.push_back(outline[i % OutlineCount][1]);
			vertexArr.push_back(z);
		};

		// переднее основание (z = +h), обход против часовой стрелки
		for (const auto& t : capTriangles)
		{
			vertex(t[0], h); vertex(t[1], h); vertex(t[2], h);
		}
		// заднее основание (z = -h) – копия, перенесённая вдоль Z, с обратным обходом
		for (const auto& t : capTriangles)
		{
			vertex(t[0], -h); vertex(t[2], -h); vertex(t[1], -h);
		}
		// боковые грани: соединяем соответствующие рёбра оснований
		for (int i = 0; i < OutlineCount; i++)
		{
			vertex(i, -h); vertex(i + 1, -h); vertex(i + 1, h);
			vertex(i, -h); vertex(i + 1, h); vertex(i, h);
		}
		normalArr.assign(vertexArr.size(), 0.0f);
	}

	//метод для расчета нормалей для каждой вершины каждого треугольника модели
	//!!! т.к. мы используем алгоритм прорисовки GL_TRIANGLES,
	//!!! то нормали мы расчитываем независимо для каждой вершины
	inline void calcModelNormals(const GLfloat* v, GLfloat* n, GLuint vertexCount)
	{
		for (GLuint i = 0; i + 2 < vertexCount; i += 3)
		{
			glm::vec3 a(v[i * 3 + 0], v[i * 3 + 1], v[i * 3 + 2]);
			glm::vec3 b(v[i * 3 + 3], v[i * 3 + 4], v[i * 3 + 5]);
			glm::vec3 c(v[i * 3 + 6], v[i * 3 + 7], v[i * 3 + 8]);
			glm::vec3 norm = glm::normalize(glm::cross(b - a, c - a));
			//в каждом треугольнике нормали сориентированы одинаково
			for (int k = 0; k < 3; k++)
				memcpy_s(&n[(i + k) * 3], sizeof(GLfloat) * 3, glm::value_ptr(norm), sizeof(GLfloat) * 3);
		}
	}
}
