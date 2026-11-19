using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ParticlesSystems
{
    // Элемент формы, который держит контекст OpenGL. В методичке это AnT (SimpleOpenGlControl).
    class GlControl : UserControl
    {
        IntPtr _hdc;
        IntPtr _hrc;

        public GlControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            UpdateStyles();
        }

        public void InitializeContexts()
        {
            if (_hrc != IntPtr.Zero)
                return;

            _hdc = GetDC(Handle);
            var pfd = new PixelFormatDescriptor();
            pfd.nSize = (short)Marshal.SizeOf(typeof(PixelFormatDescriptor));
            pfd.nVersion = 1;
            pfd.dwFlags = 0x25; // PFD_DOUBLEBUFFER | PFD_DRAW_TO_WINDOW | PFD_SUPPORT_OPENGL
            pfd.iPixelType = 0;
            pfd.cColorBits = 32;
            pfd.cDepthBits = 24;

            int format = ChoosePixelFormat(_hdc, ref pfd);
            if (format == 0 || !SetPixelFormat(_hdc, format, ref pfd))
                throw new InvalidOperationException("Не удалось выбрать формат пикселей OpenGL.");

            _hrc = wglCreateContext(_hdc);
            if (_hrc == IntPtr.Zero || !wglMakeCurrent(_hdc, _hrc))
                throw new InvalidOperationException("Не удалось создать контекст OpenGL.");
        }

        public void MakeCurrent()
        {
            if (_hdc != IntPtr.Zero && _hrc != IntPtr.Zero)
                wglMakeCurrent(_hdc, _hrc);
        }

        public void SwapBuffers()
        {
            if (_hdc != IntPtr.Zero)
                GdiSwapBuffers(_hdc);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        protected override void OnPaint(PaintEventArgs e)
        {
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (_hrc != IntPtr.Zero)
            {
                wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
                wglDeleteContext(_hrc);
                _hrc = IntPtr.Zero;
            }
            if (_hdc != IntPtr.Zero)
            {
                ReleaseDC(Handle, _hdc);
                _hdc = IntPtr.Zero;
            }
            base.OnHandleDestroyed(e);
        }

        [StructLayout(LayoutKind.Sequential)]
        struct PixelFormatDescriptor
        {
            public short nSize;
            public short nVersion;
            public int dwFlags;
            public byte iPixelType;
            public byte cColorBits;
            public byte cRedBits;
            public byte cRedShift;
            public byte cGreenBits;
            public byte cGreenShift;
            public byte cBlueBits;
            public byte cBlueShift;
            public byte cAlphaBits;
            public byte cAlphaShift;
            public byte cAccumBits;
            public byte cAccumRedBits;
            public byte cAccumGreenBits;
            public byte cAccumBlueBits;
            public byte cAccumAlphaBits;
            public byte cDepthBits;
            public byte cStencilBits;
            public byte cAuxBuffers;
            public byte iLayerType;
            public byte bReserved;
            public int dwLayerMask;
            public int dwVisibleMask;
            public int dwDamageMask;
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll", EntryPoint = "ChoosePixelFormat")]
        static extern int ChoosePixelFormat(IntPtr hdc, ref PixelFormatDescriptor pfd);

        [DllImport("gdi32.dll", EntryPoint = "SetPixelFormat")]
        static extern bool SetPixelFormat(IntPtr hdc, int format, ref PixelFormatDescriptor pfd);

        [DllImport("gdi32.dll", EntryPoint = "SwapBuffers")]
        static extern bool GdiSwapBuffers(IntPtr hdc);

        [DllImport("opengl32.dll")]
        static extern IntPtr wglCreateContext(IntPtr hdc);

        [DllImport("opengl32.dll")]
        static extern bool wglMakeCurrent(IntPtr hdc, IntPtr hrc);

        [DllImport("opengl32.dll")]
        static extern bool wglDeleteContext(IntPtr hrc);
    }
}
