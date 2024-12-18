using System;
using System.Windows.Forms;
using ZedGraph;
using org.mariuszgromada.math.mxparser;
using System.Drawing;

public partial class CoordinateDescentForm : Form
{
    private ZedGraphControl graphControl;

    public CoordinateDescentForm()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        this.Text = "Метод Покоординатного Спуска";
        this.Size = new System.Drawing.Size(800, 600);

        CreateUI();
        CreateGraph();
    }

    private void CreateUI()
    {
        System.Windows.Forms.Label labelX0 = new System.Windows.Forms.Label { Text = "Начальное значение x:", Location = new Point(10, 10), AutoSize = true };
        TextBox inputX0 = new TextBox { Location = new Point(200, 10), Width = 100 };

        System.Windows.Forms.Label labelY0 = new System.Windows.Forms.Label { Text = "Начальное значение y:", Location = new Point(10, 40), AutoSize = true };
        TextBox inputY0 = new TextBox { Location = new Point(200, 40), Width = 100 };

        System.Windows.Forms.Label labelEpsilon = new System.Windows.Forms.Label { Text = "Точность e:", Location = new Point(10, 70), AutoSize = true };
        TextBox inputEpsilon = new TextBox { Location = new Point(200, 70), Width = 100 };

        System.Windows.Forms.Label labelFunction = new System.Windows.Forms.Label { Text = "Функция f(x, y):", Location = new Point(10, 100), AutoSize = true };
        TextBox inputFunction = new TextBox { Location = new Point(200, 100), Width = 200 };

        Button calculateButton = new Button { Text = "Рассчитать", Location = new Point(10, 130) };
        calculateButton.Click += (sender, e) => Calculate(inputX0, inputY0, inputEpsilon, inputFunction);

        Button clearButton = new Button { Text = "Очистить", Location = new Point(120, 130) };
        clearButton.Click += (sender, e) => ClearInputs(inputX0, inputY0, inputEpsilon, inputFunction);

        graphControl = new ZedGraphControl
        {
            Location = new Point(10, 170),
            Size = new Size(600, 300)
        };

        Controls.Add(labelX0);
        Controls.Add(inputX0);
        Controls.Add(labelY0);
        Controls.Add(inputY0);
        Controls.Add(labelEpsilon);
        Controls.Add(inputEpsilon);
        Controls.Add(labelFunction);
        Controls.Add(inputFunction);
        Controls.Add(calculateButton);
        Controls.Add(clearButton);
        Controls.Add(graphControl);
    }

    private void Calculate(TextBox x0Box, TextBox y0Box, TextBox epsilonBox, TextBox functionBox)
    {
        try
        {
            double x0 = double.Parse(x0Box.Text);
            double y0 = double.Parse(y0Box.Text);
            double epsilon = double.Parse(epsilonBox.Text);
            string function = functionBox.Text;

            (double minX, double minY, bool isMin) = CoordinateDescent(x0, y0, epsilon, function);

            UpdateGraph(x0, y0, function, minX, minY);

            string type = isMin ? "минимума" : "максимума";
            MessageBox.Show($"Точка локального {type}: ({minX}, {minY})");
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

    private (double, double, bool) CoordinateDescent(double x0, double y0, double epsilon, string function)
    {
        double x = x0, y = y0;
        bool isMin = true;
        double previousValue, currentValue = EvaluateFunction(x, y, function);

        do
        {
            previousValue = currentValue;

            x = MinimizeOrMaximizeAlongAxis(x, y, epsilon, function, "x", ref isMin);
            y = MinimizeOrMaximizeAlongAxis(x, y, epsilon, function, "y", ref isMin);

            currentValue = EvaluateFunction(x, y, function);
        } while (Math.Abs(currentValue - previousValue) > epsilon);

        return (x, y, isMin);
    }

    private double MinimizeOrMaximizeAlongAxis(double x, double y, double epsilon, string function, string axis, ref bool isMin)
    {
        double a = -10, b = 10;

        while ((b - a) > epsilon)
        {
            double mid1 = a + (b - a) / 3;
            double mid2 = b - (b - a) / 3;

            double f1 = EvaluateFunction(axis == "x" ? mid1 : x, axis == "y" ? mid1 : y, function);
            double f2 = EvaluateFunction(axis == "x" ? mid2 : x, axis == "y" ? mid2 : y, function);

            if (f1 < f2)
            {
                b = mid2;
                isMin = true;
            }
            else
            {
                a = mid1;
                isMin = false;
            }
        }

        return (a + b) / 2;
    }

    private double EvaluateFunction(double x, double y, string function)
    {
        try
        {
            Argument xArg = new Argument("x", x);
            Argument yArg = new Argument("y", y);
            Expression expression = new Expression(function, xArg, yArg);

            double result = expression.calculate();

            if (double.IsNaN(result) || double.IsInfinity(result))
                throw new Exception("Функция вернула недопустимое значение.");

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception("Ошибка вычисления функции: " + ex.Message);
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
        pane.YAxis.Title.Text = "Y";

        pane.XAxis.Scale.Min = -10;
        pane.XAxis.Scale.Max = 10;
        pane.YAxis.Scale.Min = -10;
        pane.YAxis.Scale.Max = 10;

        graphControl.AxisChange();
    }

    private void UpdateGraph(double x0, double y0, string function, double minX, double minY)
    {
        GraphPane pane = graphControl.GraphPane;
        pane.CurveList.Clear();

        PointPairList list = new PointPairList();

        for (double x = x0 - 10; x <= x0 + 10; x += 0.5)
        {
            for (double y = y0 - 10; y <= y0 + 10; y += 0.5)
            {
                list.Add(x, EvaluateFunction(x, y, function));
            }
        }

        LineItem myCurve = pane.AddCurve("f(x, y)", list, Color.Blue, SymbolType.None);

        PointPairList rootList = new PointPairList { { minX, EvaluateFunction(minX, minY, function) } };
        LineItem rootPoint = pane.AddCurve("Точка", rootList, Color.Red, SymbolType.Circle);
        rootPoint.Symbol.Fill = new Fill(Color.Red);

        graphControl.AxisChange();
        graphControl.Invalidate();
    }
}
