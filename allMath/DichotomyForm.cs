using System;
using System.Windows.Forms;
using ZedGraph;
using org.mariuszgromada.math.mxparser;
using System.Drawing;

public partial class DichotomyForm : Form
{
    private ZedGraphControl graphControl;

    public DichotomyForm()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        this.Text = "Метод Дихотомии";
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

            double result = BisectionMethod(a, b, e, function);

            UpdateGraph(a, b, function, result);

            MessageBox.Show($"Результат: {result}");
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

    private double BisectionMethod(double a, double b, double epsilon, string function)
    {
        double f_a = EvaluateFunction(a, function);
        double f_b = EvaluateFunction(b, function);

        if (double.IsNaN(f_a) || double.IsNaN(f_b))
        {
            throw new Exception("Функция содержит недопустимые значения (NaN) на краях интервала.");
        }

        if (f_a == f_b && f_a != 0)
        {
            throw new Exception("Функция не пересекает ось X на данном интервале.");
        }

        if (f_a * f_b > 0)
        {
            throw new Exception("На указанном интервале нет корня. Убедитесь, что функция меняет знак.");
        }

        while ((b - a) / 2 > epsilon)
        {
            double mid = (a + b) / 2;
            double f_mid = EvaluateFunction(mid, function);

            if (double.IsNaN(f_mid))
            {
                throw new Exception($"Функция вернула NaN при x = {mid}. Проверьте функцию.");
            }

            if (Math.Abs(f_mid) < epsilon)
            {
                return mid;
            }

            if (f_a * f_mid < 0)
            {
                b = mid;
                f_b = f_mid;
            }
            else
            {
                a = mid;
                f_a = f_mid;
            }
        }

        return (a + b) / 2;
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

    private void UpdateGraph(double a, double b, string function, double result)
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

        PointPairList rootList = new PointPairList { { result, 0 } };
        LineItem rootPoint = pane.AddCurve("Результат", rootList, Color.Red, SymbolType.Circle);
        rootPoint.Symbol.Fill = new Fill(Color.Red);

        graphControl.AxisChange();
        graphControl.Invalidate();
    }
}
