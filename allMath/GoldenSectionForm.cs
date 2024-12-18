using System;
using System.Windows.Forms;
using ZedGraph;
using org.mariuszgromada.math.mxparser;
using System.Drawing;

public partial class GoldenSectionForm : Form
{
    private ZedGraphControl graphControl;

    public GoldenSectionForm()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        this.Text = "Метод Золотого Сечения";
        this.Size = new System.Drawing.Size(800, 600);

        CreateUI();
        CreateGraph();
    }

    private void CreateUI()
    {
        System.Windows.Forms.Label labelA = new System.Windows.Forms.Label { Text = "Введите a:", Location = new Point(10, 10), AutoSize = true };
        TextBox inputA = new TextBox { Location = new Point(80, 10), Width = 100 };

        System.Windows.Forms.Label labelB = new System.Windows.Forms.Label { Text = "Введите b:", Location = new Point(10, 40), AutoSize = true };
        TextBox inputB = new TextBox { Location = new Point(80, 40), Width = 100 };

        System.Windows.Forms.Label labelE = new System.Windows.Forms.Label { Text = "Введите точность e:", Location = new Point(10, 70), AutoSize = true };
        TextBox inputE = new TextBox { Location = new Point(150, 70), Width = 100 };

        System.Windows.Forms.Label labelF = new System.Windows.Forms.Label { Text = "Введите функцию f(x):", Location = new Point(10, 100), AutoSize = true };
        TextBox inputF = new TextBox { Location = new Point(150, 100), Width = 200 };

        Button calculateButton = new Button { Text = "Рассчитать", Location = new Point(10, 130) };
        calculateButton.Click += (sender, e) => Calculate(inputA, inputB, inputE, inputF);

        Button clearButton = new Button { Text = "Очистить", Location = new Point(120, 130) };
        clearButton.Click += (sender, e) => ClearInputs(inputA, inputB, inputE, inputF);

        graphControl = new ZedGraphControl
        {
            Location = new Point(10, 170),
            Size = new Size(600, 300)
        };

        Controls.Add(labelA);
        Controls.Add(inputA);
        Controls.Add(labelB);
        Controls.Add(inputB);
        Controls.Add(labelE);
        Controls.Add(inputE);
        Controls.Add(labelF);
        Controls.Add(inputF);
        Controls.Add(calculateButton);
        Controls.Add(clearButton);
        Controls.Add(graphControl);
    }

    private void Calculate(TextBox aBox, TextBox bBox, TextBox eBox, TextBox fBox)
    {
        try
        {
            double a = double.Parse(aBox.Text);
            double b = double.Parse(bBox.Text);
            double e = double.Parse(eBox.Text);
            string function = fBox.Text;

            if (a >= b)
            {
                MessageBox.Show("Ошибка: значение a должно быть меньше b.");
                return;
            }

            (double min, double max) = GoldenSectionMethod(a, b, e, function);

            UpdateGraph(a, b, function, min, max);

            MessageBox.Show($"Минимум: {min}, Максимум: {max}");
        }
        catch (FormatException)
        {
            MessageBox.Show("Ошибка ввода данных: Проверьте правильность введенных значений.");
        }
        catch (Exception ex)
        {
            MessageBox.Show("Ошибка: " + ex.Message);
        }
    }

    private (double, double) GoldenSectionMethod(double a, double b, double epsilon, string function)
    {
        double phi = (1 + Math.Sqrt(5)) / 2;
        double invPhi = 1 / phi;

        double x1 = b - (b - a) * invPhi;
        double x2 = a + (b - a) * invPhi;
        double f1 = EvaluateFunction(x1, function);
        double f2 = EvaluateFunction(x2, function);

        while ((b - a) > epsilon)
        {
            if (f1 < f2)
            {
                b = x2;
                x2 = x1;
                f2 = f1;
                x1 = b - (b - a) * invPhi;
                f1 = EvaluateFunction(x1, function);
            }
            else
            {
                a = x1;
                x1 = x2;
                f1 = f2;
                x2 = a + (b - a) * invPhi;
                f2 = EvaluateFunction(x2, function);
            }
        }

        return ((a + b) / 2, Math.Max(f1, f2));
    }

    private double EvaluateFunction(double x, string function)
    {
        try
        {
            Expression expression = new Expression(function);
            expression.defineArgument("x", x);

            double result = expression.calculate();

            if (double.IsNaN(result) || double.IsInfinity(result))
            {
                throw new Exception($"Функция вернула недопустимое значение (NaN/Infinity) при x = {x}");
            }

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception($"Ошибка вычисления функции: {ex.Message}");
        }
    }

    private void ClearInputs(params TextBox[] inputs)
    {
        foreach (var input in inputs)
        {
            input.Clear();
        }
    }

    private void CreateGraph()
    {
        GraphPane pane = graphControl.GraphPane;

        pane.Title.Text = "График функции";
        pane.XAxis.Title.Text = "X";
        pane.YAxis.Title.Text = "f(X)";

        pane.XAxis.Scale.Min = -10;
        pane.XAxis.Scale.Max = 10;
        pane.YAxis.Scale.Min = -10;
        pane.YAxis.Scale.Max = 10;

        graphControl.AxisChange();
    }

    private void UpdateGraph(double a, double b, string function, double min, double max)
    {
        GraphPane pane = graphControl.GraphPane;
        pane.CurveList.Clear();

        PointPairList list = new PointPairList();

        for (double x = a; x <= b; x += 0.01)
        {
            double y = EvaluateFunction(x, function);
            list.Add(x, y);
        }

        LineItem myCurve = pane.AddCurve("f(x)", list, Color.Blue, SymbolType.None);

        PointPairList minList = new PointPairList { { min, EvaluateFunction(min, function) } };
        LineItem minPoint = pane.AddCurve("Минимум", minList, Color.Red, SymbolType.Circle);
        minPoint.Symbol.Fill = new Fill(Color.Red);

        PointPairList maxList = new PointPairList { { max, EvaluateFunction(max, function) } };
        LineItem maxPoint = pane.AddCurve("Максимум", maxList, Color.Green, SymbolType.Circle);
        maxPoint.Symbol.Fill = new Fill(Color.Green);

        graphControl.AxisChange();
        graphControl.Invalidate();
    }
}
