using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

public partial class SortForm : Form
{
    private DataGridView dataGridView;
    private DataGridView resultGridView;
    private NumericUpDown arraySizeInput;
    private NumericUpDown rangeStartInput;
    private NumericUpDown rangeEndInput;
    private ProgressBar progressBar;
    private CheckBox bubbleSortCheckBox;
    private CheckBox insertionSortCheckBox;
    private CheckBox quickSortCheckBox;
    private CheckBox shakerSortCheckBox;
    private CheckBox bogoSortCheckBox;
    private ListBox resultBox;
    private Button generateButton;
    private Button sortButton;

    public SortForm()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        this.Text = "Сортировка данных";
        this.Size = new System.Drawing.Size(800, 600);

        Label arraySizeLabel = new Label { Text = "Размер массива:", Location = new System.Drawing.Point(10, 10), AutoSize = true };
        arraySizeInput = new NumericUpDown { Location = new System.Drawing.Point(120, 10), Minimum = 1, Maximum = 1000, Value = 10 };

        Label rangeStartLabel = new Label { Text = "Начало диапазона:", Location = new System.Drawing.Point(10, 40), AutoSize = true };
        rangeStartInput = new NumericUpDown { Location = new System.Drawing.Point(120, 40), Minimum = -1000, Maximum = 1000, Value = 0 };

        Label rangeEndLabel = new Label { Text = "Конец диапазона:", Location = new System.Drawing.Point(10, 70), AutoSize = true };
        rangeEndInput = new NumericUpDown { Location = new System.Drawing.Point(120, 70), Minimum = -1000, Maximum = 1000, Value = 100 };

        generateButton = new Button { Text = "Сгенерировать", Location = new System.Drawing.Point(10, 100) };
        generateButton.Click += GenerateButton_Click;

        sortButton = new Button { Text = "Сортировать", Location = new System.Drawing.Point(120, 100) };
        sortButton.Click += SortButton_Click;

        dataGridView = new DataGridView
        {
            Location = new System.Drawing.Point(10, 140),
            Size = new System.Drawing.Size(360, 200),
            ColumnCount = 1,
            AllowUserToAddRows = false
        };
        dataGridView.Columns[0].Name = "Исходные данные";
        dataGridView.RowHeadersVisible = false;

        resultGridView = new DataGridView
        {
            Location = new System.Drawing.Point(400, 140),
            Size = new System.Drawing.Size(360, 200),
            ColumnCount = 1,
            AllowUserToAddRows = false
        };
        resultGridView.Columns[0].Name = "Результаты";
        resultGridView.RowHeadersVisible = false;

        bubbleSortCheckBox = new CheckBox { Text = "Пузырьковая сортировка", Location = new System.Drawing.Point(10, 360), AutoSize = true };
        insertionSortCheckBox = new CheckBox { Text = "Сортировка вставками", Location = new System.Drawing.Point(10, 390), AutoSize = true };
        quickSortCheckBox = new CheckBox { Text = "Быстрая сортировка", Location = new System.Drawing.Point(10, 420), AutoSize = true };
        shakerSortCheckBox = new CheckBox { Text = "Шейкерная сортировка", Location = new System.Drawing.Point(10, 450), AutoSize = true };
        bogoSortCheckBox = new CheckBox { Text = "BOGO сортировка", Location = new System.Drawing.Point(10, 480), AutoSize = true };

        progressBar = new ProgressBar { Location = new System.Drawing.Point(10, 510), Size = new System.Drawing.Size(750, 20) };

        resultBox = new ListBox { Location = new System.Drawing.Point(10, 540), Size = new System.Drawing.Size(750, 60) };

        this.Controls.Add(arraySizeLabel);
        this.Controls.Add(arraySizeInput);
        this.Controls.Add(rangeStartLabel);
        this.Controls.Add(rangeStartInput);
        this.Controls.Add(rangeEndLabel);
        this.Controls.Add(rangeEndInput);
        this.Controls.Add(generateButton);
        this.Controls.Add(sortButton);
        this.Controls.Add(dataGridView);
        this.Controls.Add(resultGridView);
        this.Controls.Add(bubbleSortCheckBox);
        this.Controls.Add(insertionSortCheckBox);
        this.Controls.Add(quickSortCheckBox);
        this.Controls.Add(shakerSortCheckBox);
        this.Controls.Add(bogoSortCheckBox);
        this.Controls.Add(progressBar);
        this.Controls.Add(resultBox);
    }

    private void GenerateButton_Click(object sender, EventArgs e)
    {
        try
        {
            var random = new Random();
            dataGridView.Rows.Clear(); // Очистка таблицы

            int arraySize = (int)arraySizeInput.Value;
            int rangeStart = (int)rangeStartInput.Value;
            int rangeEnd = (int)rangeEndInput.Value;

            if (rangeStart > rangeEnd)
            {
                MessageBox.Show("Начало диапазона не может быть больше конца диапазона.");
                return;
            }

            Debug.WriteLine($"Генерация данных: Размер массива = {arraySize}, Диапазон = [{rangeStart}, {rangeEnd}]");

            for (int i = 0; i < arraySize; i++)
            {
                int value = random.Next(rangeStart, rangeEnd + 1);
                Debug.WriteLine($"Добавлено значение: {value}");

                // Добавляем только корректные строки
                var row = new DataGridViewRow();
                row.CreateCells(dataGridView, value);
                dataGridView.Rows.Add(row);
            }

        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Ошибка генерации данных: {ex.Message}");
            MessageBox.Show("Ошибка генерации данных.");
        }
    }

    private void SortButton_Click(object sender, EventArgs e)
    {
        try
        {
            if (!ValidateInput())
            {
                MessageBox.Show("Некорректные данные в таблице.");
                return;
            }

            List<int> data = GetDataFromGrid();

            if (data.Count == 0)
            {
                MessageBox.Show("Таблица пуста. Добавьте данные для сортировки.");
                return;
            }

            Debug.WriteLine("Исходные данные: " + string.Join(", ", data));

            resultBox.Items.Clear();
            resultGridView.Rows.Clear();

            bool anySortSelected = bubbleSortCheckBox.Checked ||
                                   insertionSortCheckBox.Checked ||
                                   quickSortCheckBox.Checked ||
                                   shakerSortCheckBox.Checked ||
                                   bogoSortCheckBox.Checked;

            if (!anySortSelected)
            {
                MessageBox.Show("Выберите хотя бы один метод сортировки.");
                return;
            }

            if (bubbleSortCheckBox.Checked)
                RunSort("Пузырьковая сортировка", BubbleSort, new List<int>(data));

            if (insertionSortCheckBox.Checked)
                RunSort("Сортировка вставками", InsertionSort, new List<int>(data));

            if (quickSortCheckBox.Checked)
                RunSort("Быстрая сортировка", QuickSort, new List<int>(data));

            if (shakerSortCheckBox.Checked)
                RunSort("Шейкерная сортировка", ShakerSort, new List<int>(data));

            if (bogoSortCheckBox.Checked && data.Count > 5)
            {
                MessageBox.Show("BOGO сортировка поддерживает максимум 5 элементов из-за низкой производительности.");
                return;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Ошибка сортировки: {ex.Message}");
            MessageBox.Show("Ошибка выполнения сортировки.");
        }
    }

    private bool ValidateInput()
    {
        foreach (DataGridViewRow row in dataGridView.Rows)
        {
            if (row.IsNewRow) continue; // Игнорируем строку добавления
            if (row.Cells[0].Value == null || !int.TryParse(row.Cells[0].Value.ToString(), out _))
            {
                Debug.WriteLine("Ошибка валидации: пустая или некорректная ячейка");
                return false;
            }
        }
        return true;
    }

    private List<int> GetDataFromGrid()
    {
        var data = new List<int>();
        foreach (DataGridViewRow row in dataGridView.Rows)
        {
            if (row.Cells[0].Value != null)
            {
                int value = int.Parse(row.Cells[0].Value.ToString());
                data.Add(value);
            }
        }
        return data;
    }

    private void RunSort(string name, Func<List<int>, List<int>> sortMethod, List<int> data)
    {
        var stopwatch = Stopwatch.StartNew();
        List<int> sortedData = sortMethod(data);
        stopwatch.Stop();

        Debug.WriteLine($"{name}: Время выполнения = {stopwatch.ElapsedMilliseconds} мс");
        resultBox.Items.Add($"{name}: {stopwatch.ElapsedMilliseconds} мс");

        resultGridView.Rows.Clear();
        foreach (var value in sortedData)
        {
            resultGridView.Rows.Add(value);
        }
    }

    private List<int> BubbleSort(List<int> data)
    {
        int iterations = 0;
        for (int i = 0; i < data.Count - 1; i++)
        {
            for (int j = 0; j < data.Count - i - 1; j++)
            {
                iterations++;
                if (data[j] > data[j + 1])
                    (data[j], data[j + 1]) = (data[j + 1], data[j]);
            }
        }
        resultBox.Items.Add($"Пузырьковая сортировка: {iterations} итераций");
        return data;
    }

    private List<int> InsertionSort(List<int> data)
    {
        int iterations = 0;
        for (int i = 1; i < data.Count; i++)
        {
            int key = data[i];
            int j = i - 1;
            while (j >= 0 && data[j] > key)
            {
                iterations++;
                data[j + 1] = data[j];
                j--;
            }
            data[j + 1] = key;
        }
        resultBox.Items.Add($"Сортировка вставками: {iterations} итераций");
        return data;
    }

    private List<int> QuickSort(List<int> data)
    {
        int iterations = 0;
        QuickSortRecursive(data, 0, data.Count - 1, ref iterations);
        resultBox.Items.Add($"Быстрая сортировка: {iterations} итераций");
        return data;
    }

    private void QuickSortRecursive(List<int> data, int low, int high, ref int iterations)
    {
        if (low < high)
        {
            int pivot = Partition(data, low, high, ref iterations);
            QuickSortRecursive(data, low, pivot - 1, ref iterations);
            QuickSortRecursive(data, pivot + 1, high, ref iterations);
        }
    }

    private int Partition(List<int> data, int low, int high, ref int iterations)
    {
        int pivot = data[high];
        int i = low - 1;
        for (int j = low; j < high; j++)
        {
            iterations++;
            if (data[j] <= pivot)
            {
                i++;
                (data[i], data[j]) = (data[j], data[i]);
            }
        }
        (data[i + 1], data[high]) = (data[high], data[i + 1]);
        return i + 1;
    }

    private List<int> ShakerSort(List<int> data)
    {
        int left = 0, right = data.Count - 1, iterations = 0;
        while (left < right)
        {
            for (int i = left; i < right; i++)
            {
                iterations++;
                if (data[i] > data[i + 1])
                    (data[i], data[i + 1]) = (data[i + 1], data[i]);
            }
            right--;
            for (int i = right; i > left; i--)
            {
                iterations++;
                if (data[i] < data[i - 1])
                    (data[i], data[i - 1]) = (data[i - 1], data[i]);
            }
            left++;
        }
        resultBox.Items.Add($"Шейкерная сортировка: {iterations} итераций");
        return data;
    }
    private List<int> BogoSort(List<int> data)
    {
        if (data.Count > 5)
        {
            resultBox.Items.Add("BOGO сортировка не поддерживает массивы больше 5 элементов.");
            MessageBox.Show("BOGO сортировка поддерживает максимум 5 элементов из-за низкой производительности.");
            return data;
        }

        var random = new Random();
        int iterations = 0;
        int maxIterations = 100000; // Ограничение на количество итераций

        if (IsSorted(data)) // Проверка до начала сортировки
        {
            resultBox.Items.Add($"BOGO сортировка завершена без изменений: массив уже отсортирован.");
            return data;
        }

        while (!IsSorted(data))
        {
            iterations++;
            if (iterations > maxIterations) // Прерывание при превышении лимита
            {
                resultBox.Items.Add($"BOGO сортировка прервана: превышен лимит в {maxIterations} итераций.");
                MessageBox.Show("BOGO сортировка прервана: превышен лимит итераций.");
                return data;
            }

            // Перетасовка данных
            data = data.OrderBy(x => random.Next()).ToList();
        }

        resultBox.Items.Add($"BOGO сортировка: {iterations} итераций");
        return data;
    }

    private bool IsSorted(List<int> data)
    {
        for (int i = 0; i < data.Count - 1; i++)
        {
            if (data[i] > data[i + 1])
                return false;
        }
        return true;
    }
}