using System;
using System.Windows.Forms;
using ZedGraph;
using NCalc;
using System.Drawing;

public partial class CoordinateDescentForm : Form
{
    private ZedGraphControl graphControl;

    public CoordinateDescentForm()
    {
        CreateUI();
        CreateGraph();
    }

    private void CreateUI()
    {
        System.Windows.Forms.Label labelX = new System.Windows.Forms.Label { Text = "Начальное значение x:", Location = new Point(10, 10), AutoSize = true };
        TextBox inputX = new TextBox { Location = new Point(150, 10) };

        System.Windows.Forms.Label labelY = new System.Windows.Forms.Label { Text = "Начальное значение y:", Location = new Point(10, 40), AutoSize = true };
        TextBox inputY = new TextBox { Location = new Point(150, 40) };

        System.Windows.Forms.Label labelEpsilon = new System.Windows.Forms.Label { Text = "Точность e:", Location = new Point(10, 70), AutoSize = true };
        TextBox inputEpsilon = new TextBox { Location = new Point(150, 70) };

        System.Windows.Forms.Label labelFunction = new System.Windows.Forms.Label { Text = "Функция f(x, y):", Location = new Point(10, 100), AutoSize = true };
        TextBox inputFunction = new TextBox { Location = new Point(150, 100), Width = 200 };

        Button calculateButton = new Button { Text = "Рассчитать", Location = new Point(10, 130) };
        calculateButton.Click += (sender, e) => Calculate(inputX, inputY, inputEpsilon, inputFunction);

        Button clearButton = new Button { Text = "Очистить", Location = new Point(120, 130) };
        clearButton.Click += (sender, e) => ClearInputs(inputX, inputY, inputEpsilon, inputFunction);

        graphControl = new ZedGraphControl
        {
            Location = new Point(10, 170),
            Size = new Size(400, 300)
        };

        Controls.Add(labelX);
        Controls.Add(inputX);
        Controls.Add(labelY);
        Controls.Add(inputY);
        Controls.Add(labelEpsilon);
        Controls.Add(inputEpsilon);
        Controls.Add(labelFunction);
        Controls.Add(inputFunction);
        Controls.Add(calculateButton);
        Controls.Add(clearButton);
        Controls.Add(graphControl);
    }

    private void Calculate(TextBox xBox, TextBox yBox, TextBox epsilonBox, TextBox functionBox)
    {
        try
        {
            double x = double.Parse(xBox.Text);
            double y = double.Parse(yBox.Text);
            double epsilon = double.Parse(epsilonBox.Text);
            string function = functionBox.Text;

            (double minX, double minY) = CoordinateDescent(x, y, epsilon, function);

            UpdateGraph(x, y, function, minX, minY);

            MessageBox.Show($"Точка минимума: ({minX}, {minY})");
        }
        catch (Exception ex)
        {
            MessageBox.Show("Ошибка ввода данных: " + ex.Message);
        }
    }

    private (double, double) CoordinateDescent(double x, double y, double epsilon, string function)
    {
        double previousX, previousY;
        do
        {
            previousX = x;
            previousY = y;

            x = MinimizeAlongCoordinate(x, y, epsilon, function, "x");

            y = MinimizeAlongCoordinate(x, y, epsilon, function, "y");

        } while (Math.Abs(x - previousX) > epsilon || Math.Abs(y - previousY) > epsilon);

        return (x, y);
    }

    private double MinimizeAlongCoordinate(double x, double y, double epsilon, string function, string variable)
    {
        double a = -10, b = 10;

        while (Math.Abs(b - a) > epsilon)
        {
            double mid1 = a + (b - a) / 3;
            double mid2 = b - (b - a) / 3;

            double f1 = EvaluateFunction(mid1, y, function, variable);
            double f2 = EvaluateFunction(mid2, y, function, variable);

            if (f1 < f2)
                b = mid2;
            else
                a = mid1;
        }

        return (a + b) / 2;
    }

    private double EvaluateFunction(double x, double y, string function, string variable)
    {
        try
        {
            Expression expression = new Expression(function);

            expression.Parameters["x"] = variable == "x" ? x : x;
            expression.Parameters["y"] = variable == "y" ? y : y;

            return Convert.ToDouble(expression.Evaluate());
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка вычисления функции: {ex.Message}");
            return double.NaN;
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

    private void UpdateGraph(double x, double y, string function, double minX, double minY)
    {
        GraphPane pane = graphControl.GraphPane;
        pane.CurveList.Clear();

        PointPairList list = new PointPairList();

        for (double i = -10; i <= 10; i += 0.5)
        {
            for (double j = -10; j <= 10; j += 0.5)
            {
                double z = EvaluateFunction(i, j, function, "");
                list.Add(i, j, z);
            }
        }

        LineItem myCurve = pane.AddCurve("f(x, y)", list, Color.Blue, SymbolType.None);

        PointPairList minList = new PointPairList { { minX, minY } };
        LineItem minPoint = pane.AddCurve("Точка минимума", minList, Color.Red, SymbolType.Circle);
        minPoint.Symbol.Fill = new Fill(Color.Red);

        graphControl.AxisChange();
        graphControl.Invalidate();
    }
}