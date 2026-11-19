using System;
using System.Runtime.InteropServices;

namespace ParticlesSystems
{
    // Обёртка над opengl32.dll с теми же именами, что у Tao.OpenGl.Gl.
    static class Gl
    {
        public const int GL_TRIANGLES = 0x0004;
        public const int GL_LINES = 0x0001;
        public const int GL_COMPILE = 0x1300;

        public const int GL_DEPTH_BUFFER_BIT = 0x00000100;
        public const int GL_COLOR_BUFFER_BIT = 0x00004000;

        public const int GL_PROJECTION = 0x1701;
        public const int GL_MODELVIEW = 0x1700;

        public const int GL_DEPTH_TEST = 0x0B71;
        public const int GL_LIGHTING = 0x0B50;
        public const int GL_LIGHT0 = 0x4000;
        public const int GL_COLOR_MATERIAL = 0x0B57;
        public const int GL_NORMALIZE = 0x0BA1;
        public const int GL_FRONT_AND_BACK = 0x0408;
        public const int GL_AMBIENT_AND_DIFFUSE = 0x1602;

        public const int GL_VENDOR = 0x1F00;
        public const int GL_RENDERER = 0x1F01;
        public const int GL_VERSION = 0x1F02;

        [DllImport("opengl32.dll")] public static extern void glBegin(int mode);
        [DllImport("opengl32.dll")] public static extern void glEnd();
        [DllImport("opengl32.dll")] public static extern void glVertex3d(double x, double y, double z);
        [DllImport("opengl32.dll")] public static extern void glVertex3f(float x, float y, float z);
        [DllImport("opengl32.dll")] public static extern void glNormal3f(float x, float y, float z);
        [DllImport("opengl32.dll")] public static extern void glColor3d(double r, double g, double b);
        [DllImport("opengl32.dll")] public static extern void glColor3f(float r, float g, float b);

        [DllImport("opengl32.dll")] public static extern int glGenLists(int range);
        [DllImport("opengl32.dll")] public static extern void glNewList(int list, int mode);
        [DllImport("opengl32.dll")] public static extern void glEndList();
        [DllImport("opengl32.dll")] public static extern void glCallList(int list);

        [DllImport("opengl32.dll")] public static extern void glPushMatrix();
        [DllImport("opengl32.dll")] public static extern void glPopMatrix();
        [DllImport("opengl32.dll")] public static extern void glMatrixMode(int mode);
        [DllImport("opengl32.dll")] public static extern void glLoadIdentity();
        [DllImport("opengl32.dll")] public static extern void glTranslatef(float x, float y, float z);
        [DllImport("opengl32.dll")] public static extern void glRotatef(float angle, float x, float y, float z);
        [DllImport("opengl32.dll")] public static extern void glScalef(float x, float y, float z);

        [DllImport("opengl32.dll")] public static extern void glClear(int mask);
        [DllImport("opengl32.dll")] public static extern void glClearColor(float r, float g, float b, float a);
        [DllImport("opengl32.dll")] public static extern void glViewport(int x, int y, int width, int height);
        [DllImport("opengl32.dll")] public static extern void glFlush();

        [DllImport("opengl32.dll")] public static extern void glEnable(int cap);
        [DllImport("opengl32.dll")] public static extern void glDisable(int cap);
        [DllImport("opengl32.dll")] public static extern void glLineWidth(float width);
        [DllImport("opengl32.dll")] public static extern void glColorMaterial(int face, int mode);

        [DllImport("opengl32.dll", EntryPoint = "glGetString")]
        static extern IntPtr glGetStringRaw(int name);

        public static string glGetString(int name)
        {
            IntPtr ptr = glGetStringRaw(name);
            if (ptr == IntPtr.Zero)
                return "";
            return Marshal.PtrToStringAnsi(ptr) ?? "";
        }
    }

    // Обёртка над glu32.dll с именами Tao.OpenGl.Glu.
    static class Glu
    {
        [DllImport("glu32.dll")]
        public static extern void gluPerspective(double fovy, double aspect, double zNear, double zFar);

        [DllImport("glu32.dll")]
        public static extern void gluLookAt(
            double eyeX, double eyeY, double eyeZ,
            double centerX, double centerY, double centerZ,
            double upX, double upY, double upZ);
    }
}
