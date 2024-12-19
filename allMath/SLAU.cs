using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Data;
using System.Linq;

public partial class SLAUForm : Form
{
    private DataGridView matrixA;
    private DataGridView vectorB;
    private DataGridView vectorX;
    private MenuStrip menuStrip;
    private ToolStripMenuItem calculateMenu;
    private ToolStripMenuItem clearMenu;
    private ToolStripMenuItem exportMenu;
    private ToolStripMenuItem importMenu;

    public SLAUForm()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        this.Text = "Решение СЛАУ";
        this.Size = new Size(800, 600);

        matrixA = new DataGridView
        {
            Location = new Point(10, 30),
            Size = new Size(350, 200),
            ColumnCount = 3,
            RowCount = 3,
            AllowUserToAddRows = false
        };

        vectorB = new DataGridView
        {
            Location = new Point(370, 30),
            Size = new Size(100, 200),
            ColumnCount = 1,
            RowCount = 3,
            AllowUserToAddRows = false
        };

        vectorX = new DataGridView
        {
            Location = new Point(500, 30),
            Size = new Size(100, 200),
            ColumnCount = 1,
            RowCount = 3,
            AllowUserToAddRows = false,
            ReadOnly = true
        };

        menuStrip = new MenuStrip();

        calculateMenu = new ToolStripMenuItem("Рассчитать");
        calculateMenu.Click += async (s, e) => await CalculateAsync();

        clearMenu = new ToolStripMenuItem("Очистить");
        clearMenu.Click += (s, e) => ClearData();

        exportMenu = new ToolStripMenuItem("Экспортировать");
        exportMenu.Click += (s, e) => ExportData();

        importMenu = new ToolStripMenuItem("Загрузить");
        importMenu.Click += (s, e) => ImportData();

        menuStrip.Items.Add(calculateMenu);
        menuStrip.Items.Add(clearMenu);
        menuStrip.Items.Add(exportMenu);
        menuStrip.Items.Add(importMenu);

        this.Controls.Add(matrixA);
        this.Controls.Add(vectorB);
        this.Controls.Add(vectorX);
        this.Controls.Add(menuStrip);
        this.MainMenuStrip = menuStrip;
    }

    private async Task CalculateAsync()
    {
        try
        {
            var a = GetMatrixFromDataGridView(matrixA);
            var b = GetVectorFromDataGridView(vectorB);

            if (a.Length != b.Length * b.Length)
            {
                MessageBox.Show("Матрица A и вектор B имеют несовместимые размеры.");
                return;
            }

            double[] x = await Task.Run(() => SolveUsingGauss(a, b));
            DisplayResult(x);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка расчета: {ex.Message}");
        }
    }

    private double[,] GetMatrixFromDataGridView(DataGridView dgv)
    {
        int rows = dgv.RowCount;
        int cols = dgv.ColumnCount;
        double[,] matrix = new double[rows, cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                matrix[i, j] = Convert.ToDouble(dgv[j, i].Value);
            }
        }
        return matrix;
    }

    private double[] GetVectorFromDataGridView(DataGridView dgv)
    {
        int rows = dgv.RowCount;
        double[] vector = new double[rows];
        for (int i = 0; i < rows; i++)
        {
            vector[i] = Convert.ToDouble(dgv[0, i].Value);
        }
        return vector;
    }

    private double[] SolveUsingGauss(double[,] a, double[] b)
    {
        int n = b.Length;
        double[] x = new double[n];

        for (int k = 0; k < n; k++)
        {
            for (int i = k + 1; i < n; i++)
            {
                double factor = a[i, k] / a[k, k];
                for (int j = k; j < n; j++)
                {
                    a[i, j] -= factor * a[k, j];
                }
                b[i] -= factor * b[k];
            }
        }

        for (int i = n - 1; i >= 0; i--)
        {
            x[i] = b[i];
            for (int j = i + 1; j < n; j++)
            {
                x[i] -= a[i, j] * x[j];
            }
            x[i] /= a[i, i];
        }

        return x;
    }

    private void DisplayResult(double[] x)
    {
        vectorX.RowCount = x.Length;
        for (int i = 0; i < x.Length; i++)
        {
            vectorX[0, i].Value = x[i];
        }
    }

    private void ClearData()
    {
        matrixA.Rows.Clear();
        vectorB.Rows.Clear();
        vectorX.Rows.Clear();
    }

    private void ExportData()
    {
        MessageBox.Show("Экспорт данных пока не реализован.");
    }

    private void ImportData()
    {
        MessageBox.Show("Импорт данных пока не реализован.");
    }
}
