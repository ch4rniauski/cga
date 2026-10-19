using System.Globalization;
using System.Text;

namespace laba2;

/// <summary>
/// Прямоугольная матрица вещественных чисел с базовыми операциями.
/// </summary>
internal sealed class Matrix
{
    private readonly double[,] data;

    private int Rows { get; }
    private int Cols { get; }

    private Matrix(int rows, int cols)
    {
        if (rows <= 0 || cols <= 0)
        {
            throw new ArgumentException("Размеры матрицы должны быть положительными.");
        }

        Rows = rows;
        Cols = cols;
        data = new double[rows, cols];
    }

    public Matrix(double[,] values) : this(values.GetLength(0), values.GetLength(1))
    {
        Array.Copy(values, data, values.Length);
    }

    public double this[int row, int col]
    {
        get => data[row, col];
        set => data[row, col] = value;
    }

    private static Matrix Identity(int n)
    {
        var m = new Matrix(n, n);
        for (var i = 0; i < n; i++)
        {
            m[i, i] = 1.0;
        }

        return m;
    }

    public static Matrix RowVector(params double[] values)
    {
        var m = new Matrix(1, values.Length);
        for (var j = 0; j < values.Length; j++)
        {
            m[0, j] = values[j];
        }

        return m;
    }

    public static Matrix operator +(Matrix a, Matrix b)
    {
        CheckSameSize(a, b);
        var r = new Matrix(a.Rows, a.Cols);
        for (var i = 0; i < a.Rows; i++)
        {
            for (var j = 0; j < a.Cols; j++)
            {
                r[i, j] = a[i, j] + b[i, j];
            }
        }

        return r;
    }

    public static Matrix operator -(Matrix a, Matrix b)
    {
        CheckSameSize(a, b);
        var r = new Matrix(a.Rows, a.Cols);
        for (var i = 0; i < a.Rows; i++)
        {
            for (var j = 0; j < a.Cols; j++)
            {
                r[i, j] = a[i, j] - b[i, j];
            }
        }

        return r;
    }

    public static Matrix operator *(Matrix a, Matrix b)
    {
        if (a.Cols != b.Rows)
        {
            throw new InvalidOperationException(
                $"Нельзя умножить матрицу {a.Rows}x{a.Cols} на матрицу {b.Rows}x{b.Cols}.");
        }

        var r = new Matrix(a.Rows, b.Cols);
        for (var i = 0; i < a.Rows; i++)
        {
            for (var j = 0; j < b.Cols; j++)
            {
                var sum = 0.0;
                for (var k = 0; k < a.Cols; k++)
                {
                    sum += a[i, k] * b[k, j];
                }

                r[i, j] = sum;
            }
        }

        return r;
    }

    public static Matrix operator *(double k, Matrix a)
    {
        var r = new Matrix(a.Rows, a.Cols);
        for (var i = 0; i < a.Rows; i++)
        {
            for (var j = 0; j < a.Cols; j++)
            {
                r[i, j] = k * a[i, j];
            }
        }

        return r;
    }

    public static Matrix operator *(Matrix a, double k)
    {
        return k * a;
    }

    /// <summary>
    /// Обратная матрица методом Гаусса–Жордана.
    /// </summary>
    public Matrix Inverse()
    {
        CheckSquare();
        var a = (double[,])data.Clone();
        var inv = Identity(Rows).data;

        for (var col = 0; col < Rows; col++)
        {
            var pivot = FindPivot(a, col, Rows);
            if (Math.Abs(a[pivot, col]) < 1e-12)
            {
                throw new InvalidOperationException("Матрица вырождена, обратной не существует.");
            }

            SwapRows(a, pivot, col, Rows);
            SwapRows(inv, pivot, col, Rows);

            var p = a[col, col];
            for (var j = 0; j < Rows; j++)
            {
                a[col, j] /= p;
                inv[col, j] /= p;
            }

            for (var i = 0; i < Rows; i++)
            {
                if (i == col)
                {
                    continue;
                }

                var f = a[i, col];
                if (f == 0)
                {
                    continue;
                }

                for (var j = 0; j < Rows; j++)
                {
                    a[i, j] -= f * a[col, j];
                    inv[i, j] -= f * inv[col, j];
                }
            }
        }

        return new Matrix(inv);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (var i = 0; i < Rows; i++)
        {
            for (var j = 0; j < Cols; j++)
            {
                sb.Append(data[i, j].ToString("0.###", CultureInfo.InvariantCulture).PadLeft(8));
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }

    private static int FindPivot(double[,] a, int col, int n)
    {
        var pivot = col;
        for (var i = col + 1; i < n; i++)
        {
            if (Math.Abs(a[i, col]) > Math.Abs(a[pivot, col]))
            {
                pivot = i;
            }
        }

        return pivot;
    }

    private static void SwapRows(double[,] a, int r1, int r2, int n)
    {
        if (r1 == r2)
        {
            return;
        }

        for (var j = 0; j < n; j++)
        {
            (a[r1, j], a[r2, j]) = (a[r2, j], a[r1, j]);
        }
    }

    private void CheckSquare()
    {
        if (Rows != Cols)
        {
            throw new InvalidOperationException("Операция определена только для квадратных матриц.");
        }
    }

    private static void CheckSameSize(Matrix a, Matrix b)
    {
        if (a.Rows != b.Rows || a.Cols != b.Cols)
        {
            throw new InvalidOperationException("Размеры матриц не совпадают.");
        }
    }
}
