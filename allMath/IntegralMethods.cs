using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;
using Flee.PublicTypes;

public partial class IntegralMethodsForm : Form
{
    private ZedGraphControl graphControl;
    private NumericUpDown numericA, numericB, numericPrecision;
    private TextBox textBoxFunction;
    private RadioButton rectangleMethod, trapezoidMethod, simpsonMethod;

    public IntegralMethodsForm()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        this.Text = "Вычисление интеграла";
        this.Size = new Size(700, 600);

        // UI Elements
        System.Windows.Forms.Label labelA = new System.Windows.Forms.Label { Text = "Граница a:", Location = new Point(10, 10), AutoSize = true };
        numericA = new NumericUpDown { Location = new Point(100, 10), Minimum = -1000, Maximum = 1000 };

        System.Windows.Forms.Label labelB = new System.Windows.Forms.Label { Text = "Граница b:", Location = new Point(10, 40), AutoSize = true };
        numericB = new NumericUpDown { Location = new Point(100, 40), Minimum = -1000, Maximum = 1000 };

        System.Windows.Forms.Label labelPrecision = new System.Windows.Forms.Label { Text = "Точность n:", Location = new Point(10, 70), AutoSize = true };
        numericPrecision = new NumericUpDown { Location = new Point(100, 70), Minimum = 1, Maximum = 10000, Value = 100 };

        System.Windows.Forms.Label labelFunction = new System.Windows.Forms.Label { Text = "Функция f(x):", Location = new Point(10, 100), AutoSize = true };
        textBoxFunction = new TextBox { Location = new Point(100, 100), Width = 300 };

        rectangleMethod = new RadioButton { Text = "Метод прямоугольников", Location = new Point(10, 130), AutoSize = true };
        trapezoidMethod = new RadioButton { Text = "Метод трапеций", Location = new Point(10, 160), AutoSize = true };
        simpsonMethod = new RadioButton { Text = "Метод Симпсона", Location = new Point(10, 190), AutoSize = true };

        Button calculateButton = new Button { Text = "Рассчитать", Location = new Point(10, 230) };
        calculateButton.Click += async (s, e) => await CalculateIntegralAsync();

        Button clearButton = new Button { Text = "Очистить", Location = new Point(100, 230) };
        clearButton.Click += (s, e) => ClearInputs();

        graphControl = new ZedGraphControl { Location = new Point(10, 270), Size = new Size(660, 250) };

        Controls.Add(labelA);
        Controls.Add(numericA);
        Controls.Add(labelB);
        Controls.Add(numericB);
        Controls.Add(labelPrecision);
        Controls.Add(numericPrecision);
        Controls.Add(labelFunction);
        Controls.Add(textBoxFunction);
        Controls.Add(rectangleMethod);
        Controls.Add(trapezoidMethod);
        Controls.Add(simpsonMethod);
        Controls.Add(calculateButton);
        Controls.Add(clearButton);
        Controls.Add(graphControl);

        CreateGraph();
    }

    private async Task CalculateIntegralAsync()
    {
        try
        {
            double a = (double)numericA.Value;
            double b = (double)numericB.Value;

            if (a == b) throw new Exception("Границы интегрирования не могут быть равны.");

            int n = (int)numericPrecision.Value;
            string functionText = textBoxFunction.Text;

            if (string.IsNullOrWhiteSpace(functionText))
                throw new Exception("Функция не может быть пустой.");

            double result = 0;
            if (rectangleMethod.Checked)
                result = await Task.Run(() => RectangleMethod(a, b, n, functionText));
            else if (trapezoidMethod.Checked)
                result = await Task.Run(() => TrapezoidMethod(a, b, n, functionText));
            else if (simpsonMethod.Checked)
                result = await Task.Run(() => SimpsonMethod(a, b, n, functionText));
            else
                throw new Exception("Выберите метод интегрирования.");

            MessageBox.Show($"Результат интегрирования: {result:F4}");
            UpdateGraph(functionText, a, b);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка: {ex.Message}");
        }
    }

    private double EvaluateFunction(double x, string functionText)
    {
        var context = new ExpressionContext();
        context.Variables["x"] = x;
        IDynamicExpression e = context.CompileDynamic(functionText);
        return Convert.ToDouble(e.Evaluate());
    }

    private double RectangleMethod(double a, double b, int n, string functionText)
    {
        double h = (b - a) / n;
        double sum = 0;
        for (int i = 0; i < n; i++)
        {
            double x = a + i * h + h / 2;
            sum += EvaluateFunction(x, functionText);
        }
        return sum * h;
    }

    private double TrapezoidMethod(double a, double b, int n, string functionText)
    {
        double h = (b - a) / n;
        double sum = (EvaluateFunction(a, functionText) + EvaluateFunction(b, functionText)) / 2;
        for (int i = 1; i < n; i++)
        {
            double x = a + i * h;
            sum += EvaluateFunction(x, functionText);
        }
        return sum * h;
    }

    private double SimpsonMethod(double a, double b, int n, string functionText)
    {
        if (n % 2 != 0) n++; // n должно быть четным
        double h = (b - a) / n;
        double sum = EvaluateFunction(a, functionText) + EvaluateFunction(b, functionText);
        for (int i = 1; i < n; i++)
        {
            double x = a + i * h;
            sum += (i % 2 == 0 ? 2 : 4) * EvaluateFunction(x, functionText);
        }
        return sum * h / 3;
    }

    private void ClearInputs()
    {
        numericA.Value = 0;
        numericB.Value = 0;
        numericPrecision.Value = 100;
        textBoxFunction.Clear();
        graphControl.GraphPane.CurveList.Clear();
        graphControl.Invalidate();
    }

    private void CreateGraph()
    {
        GraphPane pane = graphControl.GraphPane;
        pane.Title.Text = "График функции";
        pane.XAxis.Title.Text = "X";
        pane.YAxis.Title.Text = "Y";
        graphControl.AxisChange();
    }

    private void UpdateGraph(string functionText, double a, double b)
    {
        GraphPane pane = graphControl.GraphPane;
        pane.CurveList.Clear();

        PointPairList list = new PointPairList();
        for (double x = a; x <= b; x += 0.1)
        {
            list.Add(x, EvaluateFunction(x, functionText));
        }

        pane.AddCurve("f(x)", list, Color.Blue, SymbolType.None);
        graphControl.AxisChange();
        graphControl.Invalidate();
    }
}