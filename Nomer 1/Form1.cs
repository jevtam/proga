using System;
using System.Windows.Forms;

namespace DiskThicknessCalculator
{
    public class MainForm : Form
    {
        private TextBox textBoxDensity;
        private TextBox textBoxRadius;
        private TextBox textBoxMass;
        private Button calculateButton;
        private Label labelResult;
        private Label labelDensity;
        private Label labelRadius;
        private Label labelMass;

        public MainForm()
        {
            this.Text = "Расчет толщины диска";
            this.Size = new System.Drawing.Size(400, 300);

            labelDensity = new Label { Location = new System.Drawing.Point(20, 0), Text = "Плотность (ρ):", Width = 150 };
            labelRadius = new Label { Location = new System.Drawing.Point(20, 40), Text = "Радиус (r):", Width = 150 };
            labelMass = new Label { Location = new System.Drawing.Point(20, 80), Text = "Масса (m):", Width = 150 };

            textBoxDensity = new TextBox { Location = new System.Drawing.Point(20, 20), Width = 150 };
            textBoxRadius = new TextBox { Location = new System.Drawing.Point(20, 60), Width = 150 };
            textBoxMass = new TextBox { Location = new System.Drawing.Point(20, 100), Width = 150 };

            calculateButton = new Button { Location = new System.Drawing.Point(20, 140), Text = "Рассчитать" };
            calculateButton.Click += CalculateButton_Click;

            labelResult = new Label { Location = new System.Drawing.Point(20, 180), Width = 350, Height = 50 };

            this.Controls.Add(labelDensity);
            this.Controls.Add(labelRadius);
            this.Controls.Add(labelMass);
            this.Controls.Add(textBoxDensity);
            this.Controls.Add(textBoxRadius);
            this.Controls.Add(textBoxMass);
            this.Controls.Add(calculateButton);
            this.Controls.Add(labelResult);
        }

        private void CalculateButton_Click(object sender, EventArgs e)
        {
            double density, radius, mass;

            if (double.TryParse(textBoxDensity.Text, out density) &&
                double.TryParse(textBoxRadius.Text, out radius) &&
                double.TryParse(textBoxMass.Text, out mass))
            {
                double thickness = mass / (density * Math.PI * Math.Pow(radius, 2));

                labelResult.Text = $"Толщина диска: {thickness:F3} м";
            }
            else
            {
                labelResult.Text = "Ошибка ввода. Проверьте значения.";
            }
        }
    }
}
