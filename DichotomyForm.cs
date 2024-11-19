using System;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;
using NCalc;

public partial class DichotomyForm : Form
{
    private ZedGraphControl graphControl;

    public DichotomyForm()
    {
        CreateUI();
        CreateGraph();
    }

    private void CreateUI()
    {
        System.Windows.Forms.Label labelA = new System.Windows.Forms.Label { Text = "Введите a:", Location = new Point(10, 10), AutoSize = true };
        TextBox inputA = new TextBox { Location = new Point(80, 10) };

        System.Windows.Forms.Label labelB = new System.Windows.Forms.Label { Text = "Введите b:", Location = new Point(10, 40), AutoSize = true };
        TextBox inputB = new TextBox { Location = new Point(80, 40) };

        System.Windows.Forms.Label labelE = new System.Windows.Forms.Label { Text = "Введите точность e:", Location = new Point(10, 70) };
        TextBox inputE = new TextBox { Location = new Point(150, 70) };

        System.Windows.Forms.Label labelF = new System.Windows.Forms.Label { Text = "Введите функцию f(x):", Location = new Point(10, 100) };
        TextBox inputF = new TextBox { Location = new Point(150, 100), Width = 200 };

        Button calculateButton = new Button { Text = "Рассчитать", Location = new Point(10, 130) };
        calculateButton.Click += (sender, e) => Calculate(inputA, inputB, inputE, inputF);

        Button clearButton = new Button { Text = "Очистить", Location = new Point(120, 130) };
        clearButton.Click += (sender, e) => ClearInputs(inputA, inputB, inputE, inputF);

        graphControl = new ZedGraphControl
        {
            Location = new Point(10, 170),
            Size = new Size(400, 300)
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

            if (double.IsNaN(result))
            {
                MessageBox.Show("Не удалось найти минимум. Проверьте корректность функции и интервала.");
                return;
            }

            UpdateGraph(a, b, function, result);
            MessageBox.Show($"Точка минимума: {result}");
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
        while ((b - a) > epsilon)
        {
            double mid = (a + b) / 2;

            double f_a = EvaluateFunction(a, function);
            double f_b = EvaluateFunction(b, function);
            double f_mid = EvaluateFunction(mid, function);

            if (double.IsNaN(f_a) || double.IsNaN(f_b) || double.IsNaN(f_mid))
            {
                MessageBox.Show("Ошибка вычислений: проверьте введённую функцию.");
                return double.NaN;
            }

            if (f_mid < f_a && f_mid < f_b)
            {
                a = a;
                b = b;
            }
            else if (f_a < f_b)
            {
                b = mid;
            }
            else
            {
                a = mid;
            }
        }

        return (a + b) / 2;
    }



    private double EvaluateFunction(double x, string function)
    {
        try
        {
            Expression expression = new Expression(function);
            expression.Parameters["x"] = x;

            expression.EvaluateFunction += (name, args) =>
            {
                if (name == "ln")
                {
                    double arg = Convert.ToDouble(args.Parameters[0]);
                    args.Result = Math.Log(arg);
                }
            };

            object result = expression.Evaluate();

            if (result == null || double.IsNaN(Convert.ToDouble(result)))
            {
                Console.WriteLine($"EvaluateFunction: На входе x = {x}, результат NaN");
                return double.NaN;
            }

            double finalResult = Convert.ToDouble(result);
            Console.WriteLine($"EvaluateFunction: На входе x = {x}, результат = {finalResult}");
            return finalResult;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"EvaluateFunction: Ошибка при x = {x}, сообщение: {ex.Message}");
            return double.NaN;
        }
    }


    private bool IsDiscontinuous(string function, double x)
    {
        if (function.Contains("tan") && (Math.Abs(x % Math.PI - Math.PI / 2) < 0.0001))
        {
            return true;
        }
        return false;
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

    private void UpdateGraph(double a, double b, string function, double minimum)
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

        PointPairList minList = new PointPairList();
        minList.Add(minimum, EvaluateFunction(minimum, function));

        LineItem minPoint = pane.AddCurve("Точка минимума", minList, Color.Red, SymbolType.Circle);
        minPoint.Symbol.Fill = new Fill(Color.Red);

        graphControl.AxisChange();
        graphControl.Invalidate();
    }
}