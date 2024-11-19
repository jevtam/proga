using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

public partial class SortForm : Form
{
    private DataGridView dataGridView;
    private CheckBox bubbleSortCheckBox;
    private CheckBox insertionSortCheckBox;
    private CheckBox shakerSortCheckBox;
    private CheckBox quickSortCheckBox;
    private CheckBox bogoSortCheckBox;
    private ListBox resultBox;
    private MenuStrip menuStrip;

    public SortForm()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        this.Text = "Сортировка данных";
        this.Size = new System.Drawing.Size(800, 600);

        dataGridView = new DataGridView
        {
            Location = new System.Drawing.Point(10, 40),
            Size = new System.Drawing.Size(500, 300),
            ColumnCount = 1,
            AllowUserToAddRows = false
        };
        dataGridView.Columns[0].Name = "Значения";

        bubbleSortCheckBox = new CheckBox { Text = "Пузырьковая сортировка", Location = new System.Drawing.Point(520, 40) };
        insertionSortCheckBox = new CheckBox { Text = "Сортировка вставками", Location = new System.Drawing.Point(520, 70) };
        shakerSortCheckBox = new CheckBox { Text = "Шейкерная сортировка", Location = new System.Drawing.Point(520, 100) };
        quickSortCheckBox = new CheckBox { Text = "Быстрая сортировка", Location = new System.Drawing.Point(520, 130) };
        bogoSortCheckBox = new CheckBox { Text = "BOGO сортировка", Location = new System.Drawing.Point(520, 160) };

        resultBox = new ListBox
        {
            Location = new System.Drawing.Point(10, 350),
            Size = new System.Drawing.Size(500, 200)
        };

        menuStrip = new MenuStrip();
        var calculateMenuItem = new ToolStripMenuItem("Рассчитать");
        var clearMenuItem = new ToolStripMenuItem("Очистить");
        var generateMenuItem = new ToolStripMenuItem("Сгенерировать");

        calculateMenuItem.Click += CalculateMenuItem_Click;
        clearMenuItem.Click += ClearMenuItem_Click;
        generateMenuItem.Click += GenerateMenuItem_Click;

        menuStrip.Items.Add(calculateMenuItem);
        menuStrip.Items.Add(clearMenuItem);
        menuStrip.Items.Add(generateMenuItem);

        this.Controls.Add(dataGridView);
        this.Controls.Add(bubbleSortCheckBox);
        this.Controls.Add(insertionSortCheckBox);
        this.Controls.Add(shakerSortCheckBox);
        this.Controls.Add(quickSortCheckBox);
        this.Controls.Add(bogoSortCheckBox);
        this.Controls.Add(resultBox);
        this.Controls.Add(menuStrip);
        this.MainMenuStrip = menuStrip;
    }

    private async void CalculateMenuItem_Click(object sender, EventArgs e)
    {
        if (!ValidateInput())
        {
            MessageBox.Show("Некорректные данные в таблице.");
            return;
        }

        List<int> data = GetDataFromGrid();
        resultBox.Items.Clear();

        if (bubbleSortCheckBox.Checked)
            await RunSortAsync("Пузырьковая сортировка", BubbleSort, new List<int>(data));

        if (insertionSortCheckBox.Checked)
            await RunSortAsync("Сортировка вставками", InsertionSort, new List<int>(data));

        if (shakerSortCheckBox.Checked)
            await RunSortAsync("Шейкерная сортировка", ShakerSort, new List<int>(data));

        if (quickSortCheckBox.Checked)
            await RunSortAsync("Быстрая сортировка", QuickSort, new List<int>(data));

        if (bogoSortCheckBox.Checked)
            await RunSortAsync("BOGO сортировка", BogoSort, new List<int>(data));
    }

    private void ClearMenuItem_Click(object sender, EventArgs e)
    {
        dataGridView.Rows.Clear();
        resultBox.Items.Clear();
    }

    private void GenerateMenuItem_Click(object sender, EventArgs e)
    {
        var random = new Random();
        dataGridView.Rows.Clear();
        for (int i = 0; i < 10; i++)
        {
            dataGridView.Rows.Add(random.Next(0, 100));
        }
    }

    private async Task RunSortAsync(string name, Func<List<int>, List<int>> sortMethod, List<int> data)
    {
        var stopwatch = Stopwatch.StartNew();
        await Task.Run(() => sortMethod(data));
        stopwatch.Stop();

        resultBox.Items.Add($"{name}: {stopwatch.ElapsedMilliseconds} мс");
    }

    private bool ValidateInput()
    {
        foreach (DataGridViewRow row in dataGridView.Rows)
        {
            if (row.Cells[0].Value == null || !int.TryParse(row.Cells[0].Value.ToString(), out _))
                return false;
        }
        return true;
    }

    private List<int> GetDataFromGrid()
    {
        var data = new List<int>();
        foreach (DataGridViewRow row in dataGridView.Rows)
        {
            if (row.Cells[0].Value != null)
                data.Add(int.Parse(row.Cells[0].Value.ToString()));
        }
        return data;
    }

    private List<int> BubbleSort(List<int> data)
    {
        for (int i = 0; i < data.Count - 1; i++)
            for (int j = 0; j < data.Count - i - 1; j++)
                if (data[j] > data[j + 1])
                    (data[j], data[j + 1]) = (data[j + 1], data[j]);
        return data;
    }

    private List<int> InsertionSort(List<int> data)
    {
        for (int i = 1; i < data.Count; i++)
        {
            int key = data[i];
            int j = i - 1;
            while (j >= 0 && data[j] > key)
            {
                data[j + 1] = data[j];
                j--;
            }
            data[j + 1] = key;
        }
        return data;
    }

    private List<int> ShakerSort(List<int> data)
    {
        int left = 0, right = data.Count - 1;
        while (left < right)
        {
            for (int i = left; i < right; i++)
                if (data[i] > data[i + 1])
                    (data[i], data[i + 1]) = (data[i + 1], data[i]);
            right--;
            for (int i = right; i > left; i--)
                if (data[i] < data[i - 1])
                    (data[i], data[i - 1]) = (data[i - 1], data[i]);
            left++;
        }
        return data;
    }

    private List<int> QuickSort(List<int> data)
    {
        if (data.Count <= 1) return data;
        var pivot = data[data.Count / 2];
        var left = data.Where(x => x < pivot).ToList();
        var right = data.Where(x => x > pivot).ToList();
        return QuickSort(left).Concat(new List<int> { pivot }).Concat(QuickSort(right)).ToList();
    }

    private List<int> BogoSort(List<int> data)
    {
        var random = new Random();
        while (!IsSorted(data))
            data = data.OrderBy(x => random.Next()).ToList();
        return data;
    }

    private bool IsSorted(List<int> data)
    {
        for (int i = 0; i < data.Count - 1; i++)
            if (data[i] > data[i + 1]) return false;
        return true;
    }
}