using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace Library
{
    public partial class MainWindow : Window
    {
        private string filePath;
        private List<string> book = new List<string>();
        private List<string> name = new List<string>();
        private List<bool> inLibrary = new List<bool>();

        public MainWindow()
        {
            InitializeComponent();
            SelectFile();
            LoadBooks();
        }

        private void SelectFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "選擇書本檔案";
            openFileDialog.Filter = "CSV文件|*.csv|文字檔案|*.txt|所有文件|*.*";

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                filePath = openFileDialog.FileName;
                MessageBox.Show($"已選擇檔案: {filePath}");
            }
            else
            {
                MessageBox.Show("請選擇檔案。");
                Application.Current.Shutdown();
            }
        }

        private void LoadBooks()
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            try
            {
                using (StreamReader myfile = new StreamReader(filePath, Encoding.UTF8))
                {
                    string f;
                    while ((f = myfile.ReadLine()) != null)
                    {
                        string[] data = f.Split(',');
                        if (data.Length >= 3)
                        {
                            book.Add(data[0].Trim());
                            name.Add(data[1].Trim());
                            inLibrary.Add(data[2].Trim() == "1");
                        }
                    }
                }

                // 顯示讀取的書本資料
                // ResultTextBlock.Text = "讀取的書本資料:\n";
                // for (int i = 0; i < book.Count; i++)
                // {
                //     ResultTextBlock.Text += $"書本: {book[i]}, 作者: {name[i]}, 在書庫: {(inLibrary[i] ? "是" : "否")}\n";
                // }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"無法打開檔案！錯誤: {ex.Message}");
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string query = QueryTextBox.Text;

            if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(query))
            {
                MessageBox.Show("請選擇檔案並輸入查詢內容。");
                return;
            }

            bool foundInBook = false;
            bool foundInAuthor = false;
            string lowerQuery = query.ToLower().Trim(); // 查詢字串轉小寫並去除空白

            // 查詢書本名稱是否包含查詢字串
            List<int> foundBookIndices = new List<int>(); // 用來儲存符合條件的書名索引
            for (int j = 0; j < book.Count; j++)
            {
                if (book[j].ToLower().Contains(lowerQuery)) // 子串匹配
                {
                    foundBookIndices.Add(j);
                    foundInBook = true;
                }
            }

            // 顯示結果
            ResultTextBlock.Text = "";

            if (foundInBook)
            {
                ResultTextBlock.Text += "找到書本:\n";
                foreach (var index in foundBookIndices)
                {
                    ResultTextBlock.Text += $"書本: {book[index]}, 作者: {name[index]}, 在書庫: {(inLibrary[index] ? "是" : "否")}\n";
                }
            }

            if (!foundInBook)
            {
                List<int> foundAuthorIndices = new List<int>(); // 儲存作者的書籍索引
                for (int j = 0; j < name.Count; j++)
                {
                    if (name[j].ToLower().Contains(lowerQuery))
                    {
                        foundAuthorIndices.Add(j);
                        foundInAuthor = true;
                    }
                }

                if (foundInAuthor)
                {
                    ResultTextBlock.Text += $"作者: {query} 的書本如下:\n";
                    HashSet<string> displayedBooks = new HashSet<string>(); // 用來儲存已顯示的書名
                    foreach (var index in foundAuthorIndices)
                    {
                        if (!displayedBooks.Contains(book[index]))
                        {
                            ResultTextBlock.Text += $"書本: {book[index]}, 在書庫: {(inLibrary[index] ? "是" : "否")}\n";
                            displayedBooks.Add(book[index]);
                        }
                    }
                }
            }

            if (!foundInBook && !foundInAuthor)
            {
                ResultTextBlock.Text += "沒有找到該書本或作者。\n";
            }
        }
    }
}





























