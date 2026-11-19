#pragma once

#include <windows.h>
#include <cstring>
#include <GL/gl.h>

// Объёмный текст: для каждого символа wglUseFontOutlines создаёт дисплейный список
// из многоугольников с нормалями, выдавленных вдоль отрицательного направления оси Z
class Text3D
{
private:
	static constexpr int FirstChar = 32;
	static constexpr int CharCount = 96;

	GLuint base = 0;
	GLfloat extrusion = 0.0f;
	GLYPHMETRICSFLOAT metrics[CharCount] = {};

public:
	bool create(const wchar_t* faceName, GLfloat depth)
	{
		extrusion = depth;
		HDC hdc = wglGetCurrentDC();
		HFONT font = CreateFontW(-64, 0, 0, 0, FW_BOLD, FALSE, FALSE, FALSE, DEFAULT_CHARSET,
			OUT_TT_PRECIS, CLIP_DEFAULT_PRECIS, ANTIALIASED_QUALITY, DEFAULT_PITCH | FF_ROMAN, faceName);
		HGDIOBJ oldFont = SelectObject(hdc, font);

		base = glGenLists(CharCount);
		BOOL ok = wglUseFontOutlinesW(hdc, FirstChar, CharCount, base, 0.0f, extrusion,
			WGL_FONT_POLYGONS, metrics);

		SelectObject(hdc, oldFont);
		DeleteObject(font);
		return ok == TRUE;
	}

	GLfloat width(const char* text) const
	{
		GLfloat w = 0.0f;
		for (const char* c = text; *c; c++)
		{
			int i = (unsigned char)*c - FirstChar;
			if (i >= 0 && i < CharCount) w += metrics[i].gmfCellIncX;
		}
		return w;
	}

	// вывод строки так, чтобы центр надписи совпадал с началом координат
	void drawCentered(const char* text) const
	{
		glPushMatrix();
		glTranslatef(-width(text) / 2.0f, -0.36f, extrusion / 2.0f);
		glListBase(base - FirstChar);
		glCallLists((GLsizei)strlen(text), GL_UNSIGNED_BYTE, text);
		glPopMatrix();
	}

	void destroy()
	{
		if (base) glDeleteLists(base, CharCount);
		base = 0;
	}
};
