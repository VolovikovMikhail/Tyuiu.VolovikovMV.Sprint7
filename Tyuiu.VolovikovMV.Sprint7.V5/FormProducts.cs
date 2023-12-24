using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Tyuiu.VolovikovMV.Sprint7.V5.Lib;

namespace Tyuiu.VolovikovMV.Sprint7.V5
{
    public partial class FormProducts : Form
    {
        public BindingList<Product> productList;
        private BindingSource bindingSource;
        private SortOrder currentSortOrder = SortOrder.Ascending;

        public FormProducts()
        {
            InitializeComponent();

            // Инициализируем список и источник данных
            productList = new BindingList<Product>();
            bindingSource = new BindingSource(productList, null);

            // Привязываем источник данных к DataGridView
            dataGridView_ProductsInfo_VMV.DataSource = bindingSource;

            dataGridView_ProductsInfo_VMV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            textBox_Search_VMV.TextChanged += textBox_Search_VMV_TextChanged;
        }

        private void button_AddProduct_VMV_Click(object sender, EventArgs e)
        {
            try
            {
                int ID = Convert.ToInt32(textBox_ID_VMV.Text);
                string category = textBox_Category_VMV.Text;
                string name = textBox_NameOfProduct.Text;
                string quality = textBox_QualitiyProduct_VMV.Text;
                int price = Convert.ToInt32(textBox_Price_VMV.Text);

                // Создаем объект продукта
                Product newProduct = new Product(ID, category, name, quality, price);

                // Добавляем продукт в список
                productList.Add(newProduct);

                // Очищаем текстовые поля после добавления
                textBox_ID_VMV.Clear();
                textBox_Category_VMV.Clear();
                textBox_NameOfProduct.Clear();
                textBox_QualitiyProduct_VMV.Clear();
                textBox_Price_VMV.Clear();
            }
            catch
            {
                MessageBox.Show("Введены неверные данные", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button_OpenFile_VMV_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                openFileDialog.Title = "Выберите CSV файл для открытия";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;

                    // Читаем все строки из файла
                    string[] lines = File.ReadAllLines(filePath);

                    // Парсим заголовки столбцов
                    string[] headers = lines[0].Split(',');

                    // Очищаем старые данные в BindingList
                    productList.Clear();

                    // Заполняем BindingList данными из файла
                    for (int i = 1; i < lines.Length; i++)
                    {
                        string[] values = lines[i].Split(',');
                        productList.Add(new Product
                        {
                            ID = Convert.ToInt32(values[0]),
                            Category = values[1],
                            Name = values[2],
                            Quality = values[3],
                            Price = Convert.ToInt32(values[4])
                        });
                    }

                    MessageBox.Show("Файл успешно открыт!", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при открытии файла: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public class Product
        {
            public int ID { get; set; }
            public string Category { get; set; }
            public string Name { get; set; }
            public string Quality { get; set; }
            public int Price { get; set; }

            // Конструктор по умолчанию
            public Product()
            {

            }

            public Product(int id, string category, string name, string quality, int price)
            {
                ID = id;
                Category = category;
                Name = name;
                Quality = quality;
                Price = price;
            }
        }

        private void button_SaveFile_VMV_Click(object sender, EventArgs e)
        {
            try
            {
                string directoryPath = $@"{Directory.GetCurrentDirectory()}";
                string fileName = "КаталогБазы.csv";
                string path = Path.Combine(directoryPath, fileName);


                // Создаем строку для записи заголовков столбцов
                string header = string.Join(",", dataGridView_ProductsInfo_VMV.Columns.Cast<DataGridViewColumn>().Select(column => column.HeaderText));

                // Создаем строки для каждой записи данных
                List<string> lines = new List<string>();
                foreach (DataGridViewRow row in dataGridView_ProductsInfo_VMV.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        string line = string.Join(",", row.Cells.Cast<DataGridViewCell>().Select(cell => cell.Value.ToString()));
                        lines.Add(line);
                    }
                }

                // Соединяем заголовок и строки вместе
                string content = header + Environment.NewLine + string.Join(Environment.NewLine, lines);

                // Записываем содержимое в файл
                File.WriteAllText(path, content);

                DialogResult dialogResult = MessageBox.Show("Файл " + path + " сохранен успешно!\nОткрыть его в блокноте?", "Сообщение", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (dialogResult == DialogResult.Yes)
                {
                    System.Diagnostics.Process txt = new System.Diagnostics.Process();
                    txt.StartInfo.FileName = "notepad.exe";
                    txt.StartInfo.Arguments = path;
                    txt.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении файла: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void SortDataGridViewByColumn(string columnName)
        {
            if (currentSortOrder == SortOrder.Ascending)
            {
                bindingSource.DataSource = new BindingList<Product>(productList.OrderBy(x => typeof(Product).GetProperty(columnName).GetValue(x)).ToList());
                currentSortOrder = SortOrder.Descending;
            }
            else
            {
                bindingSource.DataSource = new BindingList<Product>(productList.OrderByDescending(x => typeof(Product).GetProperty(columnName).GetValue(x)).ToList());
                currentSortOrder = SortOrder.Ascending;
            }
        }

        private void button_Sort_VMV_Click(object sender, EventArgs e)
        {
            try
            {
                string sortName = textBox_SortInput_VMV.Text;
                SortDataGridViewByColumn(sortName);
            }

            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при вводе данных: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button_DeleteProduct_Click(object sender, EventArgs e)
        {
            if (dataGridView_ProductsInfo_VMV.SelectedRows.Count > 0)
            {
                // Получить выбранную строку
                DataGridViewRow selectedRow = dataGridView_ProductsInfo_VMV.SelectedRows[0];

                // Получить объект продукта из выбранной строки
                Product selectedProduct = (Product)selectedRow.DataBoundItem;

                // Удалить продукт из списка
                productList.Remove(selectedProduct);
            }
            else
            {
                MessageBox.Show("Выберите строку для удаления", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SearchInDataGridView(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                // Если поле поиска пусто, отобразить все данные
                dataGridView_ProductsInfo_VMV.DataSource = bindingSource;
            }
            else
            {
                // Иначе выполните поиск и отобразить результаты
                var searchResults = productList.Where(product =>
                    product.ID.ToString().Contains(searchText) ||
                    product.Category.Contains(searchText) ||
                    product.Name.Contains(searchText) ||
                    product.Quality.Contains(searchText) ||
                    product.Price.ToString().Contains(searchText)
                ).ToList();

                dataGridView_ProductsInfo_VMV.DataSource = new BindingList<Product>(searchResults);
            }
        }

        private void textBox_Search_VMV_TextChanged(object sender, EventArgs e)
        {
            SearchInDataGridView(textBox_Search_VMV.Text);
        }

        public int[] GetPrices(Product[] products)
        {
            // Проверяем, есть ли хотя бы один товар в массиве
            if (products.Any())
            {
                // Инициализируем массив для хранения стоимостей
                int[] prices = new int[products.Length];

                // Индекс для заполнения массива
                int index = 0;

                // Проходим по всем товарам в массиве
                foreach (var product in products)
                {
                    // Добавляем стоимость товара в массив
                    prices[index] = product.Price;

                    // Увеличиваем индекс
                    index++;
                }

                return prices;
            }

            // Если массив товаров пуст, возвращаем пустой массив стоимостей
            return new int[0];
        }
        private void button_MaxCost_VMV_Click(object sender, EventArgs e)
        {
            DataService ds = new DataService();
            try
            {
                int[] prices = GetPrices(productList.ToArray());
                int maxCost = ds.FindMaxPrice(prices);
                textBox_MaxCost_VMV.Text = maxCost.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при поиске максимальной цены: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
