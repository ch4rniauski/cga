using System;
using System.Windows.Forms;

namespace ParticlesSystems
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            Console.Title = path;
            Console.WriteLine("Лабораторная работа №4. Моделирование явлений с помощью частиц в OpenGL");
            Console.WriteLine("Вариант 9 — листопад в заданном объёме");
            Console.WriteLine("Исполняемый файл: " + path);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
