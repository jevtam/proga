using System;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;

public partial class NewtonMethodForm : Form
{
    private ZedGraphControl graphControl;

    public NewtonMethodForm()
    {
        CreateUI();
        CreateGraph();
    }

    private void CreateUI()
    {
        System.Windows.Forms.Label labelA = new System.Windows.Forms.Label { Text = "Введите a:", Location = new Point(10, 10), AutoSize = true };
        TextBox inputA = new TextBox { Location = new Point(80, 10), Width = 100 };

        System.Windows.Forms.Label labelB = new System.Windows.Forms.Label { Text = "Введите b:", Location = new Point(10, 40), AutoSize = true };
        TextBox inputB = new TextBox { Location = new Point(80, 40), Width = 100 };

        System.Windows.Forms.Label labelE = new System.Windows.Forms.Label { Text = "Введите точность e:", Location = new Point(10, 70) };
        TextBox inputE = new TextBox { Location = new Point(150, 70), Width = 100 };

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

            double result = NewtonMethod(a, b, e, function);

            UpdateGraph(a, b, function, result);

            MessageBox.Show($"Точка минимума: {result}");
        }
        catch (Exception ex)
        {
            MessageBox.Show("Ошибка ввода данных: " + ex.Message);
        }
    }

    private double NewtonMethod(double a, double b, double epsilon, string function)
    {
        double x = (a + b) / 2;
        double fx, f1x, f2x;

        do
        {
            fx = Math.Pow(x - 2, 2);
            f1x = 2 * (x - 2);       
            f2x = 2;                 

            x = x - f1x / f2x;
        }
        while (Math.Abs(f1x) > epsilon);

        return x;
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
            double y = Math.Pow(x - 2, 2);
            list.Add(x, y);
        }

        LineItem myCurve = pane.AddCurve("f(x)", list, Color.Blue, SymbolType.None);

        PointPairList minList = new PointPairList();
        minList.Add(minimum, Math.Pow(minimum - 2, 2));

        LineItem minPoint = pane.AddCurve("Точка минимума", minList, Color.Red, SymbolType.Circle);
        minPoint.Symbol.Fill = new Fill(Color.Red);

        graphControl.AxisChange();
        graphControl.Invalidate();
    }
}
