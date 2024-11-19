using System;
using System.Windows.Forms;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        this.Text = "Главная форма";
        this.Size = new System.Drawing.Size(800, 600);

        CreateMenu();
    }

    private void CreateMenu()
    {
        MenuStrip menuStrip = new MenuStrip { Dock = DockStyle.Left }; 

        ToolStripMenuItem method1 = new ToolStripMenuItem("Метод Дихотомии");
        method1.Click += Method1_Click;

        ToolStripMenuItem method2 = new ToolStripMenuItem("Метод Ньютона");
        method2.Click += Method2_Click;

        ToolStripMenuItem method3 = new ToolStripMenuItem("Метод Золотого Сечения");
        method3.Click += Method3_Click;

        ToolStripMenuItem method4 = new ToolStripMenuItem("Метод покоординатного спуска");
        method4.Click += Method4_Click;

        ToolStripMenuItem sortingMethods = new ToolStripMenuItem("Сортировка данных");
        sortingMethods.Click += SortingMethods_Click;

        menuStrip.Items.Add(method1);
        menuStrip.Items.Add(method2);
        menuStrip.Items.Add(method3);
        menuStrip.Items.Add(method4);
        menuStrip.Items.Add(sortingMethods);

        this.Controls.Add(menuStrip);
        this.MainMenuStrip = menuStrip;
    }

    private void Method1_Click(object sender, EventArgs e)
    {
        new DichotomyForm().Show();
    }

    private void Method2_Click(object sender, EventArgs e)
    {
        new NewtonMethodForm().Show();
    }

    private void Method3_Click(object sender, EventArgs e)
    {
        new GoldenSectionForm().Show();
    }

    private void Method4_Click(object sender, EventArgs e)
    {
        new CoordinateDescentForm().Show();
    }

    private void SortingMethods_Click(object sender, EventArgs e)
    {
        new SortForm().Show();
    }
}
