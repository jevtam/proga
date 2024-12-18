using System;
using System.Windows.Forms;
using ZedGraph;
using org.mariuszgromada.math.mxparser;
using System.Drawing;

public partial class NewtonMethodForm : Form
{
    private ZedGraphControl graphControl;

    public NewtonMethodForm()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        this.Text = "Метод Ньютона";
        this.Size = new System.Drawing.Size(800, 600);

        CreateUI();
        CreateGraph();
    }

    private void CreateUI()
    {
        System.Windows.Forms.Label labelX0 = new System.Windows.Forms.Label { Text = "Начальное приближение x0:", Location = new Point(10, 10), AutoSize = true };
        TextBox inputX0 = new TextBox { Location = new Point(200, 10), Width = 100 };

        System.Windows.Forms.Label labelE = new System.Windows.Forms.Label { Text = "Точность e:", Location = new Point(10, 40), AutoSize = true };
        TextBox inputE = new TextBox { Location = new Point(200, 40), Width = 100 };

        System.Windows.Forms.Label labelF = new System.Windows.Forms.Label { Text = "Функция f(x):", Location = new Point(10, 70), AutoSize = true };
        TextBox inputF = new TextBox { Location = new Point(200, 70), Width = 200 };

        System.Windows.Forms.Label labelDF = new System.Windows.Forms.Label { Text = "Производная f'(x):", Location = new Point(10, 100), AutoSize = true };
        TextBox inputDF = new TextBox { Location = new Point(200, 100), Width = 200 };

        Button calculateButton = new Button { Text = "Рассчитать", Location = new Point(10, 130) };
        calculateButton.Click += (sender, e) => Calculate(inputX0, inputE, inputF, inputDF);

        Button clearButton = new Button { Text = "Очистить", Location = new Point(120, 130) };
        clearButton.Click += (sender, e) => ClearInputs(inputX0, inputE, inputF, inputDF);

        graphControl = new ZedGraphControl
        {
            Location = new Point(10, 170),
            Size = new Size(600, 300)
        };

        Controls.Add(labelX0);
        Controls.Add(inputX0);
        Controls.Add(labelE);
        Controls.Add(inputE);
        Controls.Add(labelF);
        Controls.Add(inputF);
        Controls.Add(labelDF);
        Controls.Add(inputDF);
        Controls.Add(calculateButton);
        Controls.Add(clearButton);
        Controls.Add(graphControl);
    }

    private void Calculate(TextBox x0Box, TextBox eBox, TextBox fBox, TextBox dfBox)
    {
        try
        {
            double x0 = double.Parse(x0Box.Text);
            double epsilon = double.Parse(eBox.Text);
            string function = fBox.Text;
            string derivative = dfBox.Text;

            (double root, bool isMin) = NewtonMethod(x0, epsilon, function, derivative);

            UpdateGraph(x0, function, root);

            string type = isMin ? "минимума" : "максимума";
            MessageBox.Show($"Точка {type}: {root}");
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

    private (double, bool) NewtonMethod(double x0, double epsilon, string function, string derivative)
    {
        double x = x0;
        Expression fExpr = new Expression(function);
        Expression dfExpr = new Expression(derivative);

        while (true)
        {
            fExpr.defineArgument("x", x);
            dfExpr.defineArgument("x", x);

            double fx = fExpr.calculate();
            double dfx = dfExpr.calculate();

            if (Math.Abs(fx) < epsilon)
                break;

            if (Math.Abs(dfx) < epsilon)
                throw new Exception("Производная близка к нулю. Метод не может быть применен.");

            x = x - fx / dfx;
        }

        double secondDerivative = EvaluateSecondDerivative(x, derivative);
        bool isMin = secondDerivative > 0;
        return (x, isMin);
    }

    private double EvaluateSecondDerivative(double x, string derivative)
    {
        string secondDerivative = $"({derivative})'";
        Expression ddfExpr = new Expression(secondDerivative);
        ddfExpr.defineArgument("x", x);

        double result = ddfExpr.calculate();
        if (double.IsNaN(result) || double.IsInfinity(result))
            throw new Exception("Ошибка вычисления второй производной.");

        return result;
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

    private void UpdateGraph(double x0, string function, double root)
    {
        GraphPane pane = graphControl.GraphPane;
        pane.CurveList.Clear();

        PointPairList list = new PointPairList();

        for (double x = x0 - 10; x <= x0 + 10; x += 0.01)
        {
            list.Add(x, EvaluateFunction(x, function));
        }

        LineItem myCurve = pane.AddCurve("f(x)", list, Color.Blue, SymbolType.None);

        PointPairList rootList = new PointPairList { { root, EvaluateFunction(root, function) } };
        LineItem rootPoint = pane.AddCurve("Точка", rootList, Color.Red, SymbolType.Circle);
        rootPoint.Symbol.Fill = new Fill(Color.Red);

        graphControl.AxisChange();
        graphControl.Invalidate();
    }

    private double EvaluateFunction(double x, string function)
    {
        try
        {
            Expression expression = new Expression(function);
            expression.defineArgument("x", x);

            double result = expression.calculate();

            if (double.IsNaN(result) || double.IsInfinity(result))
                throw new Exception($"Функция вернула недопустимое значение при x = {x}");

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception($"Ошибка вычисления функции: {ex.Message}");
        }
    }
}
