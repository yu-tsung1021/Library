using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace Library
{
    public partial class MainWindow : Window
    {
        private string filePath;

        public MainWindow()
        {
            InitializeComponent();
            SelectFile();
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

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string query = QueryTextBox.Text;

            if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(query))
            {
                MessageBox.Show("請選擇檔案並輸入查詢內容。");
                return;
            }

            List<string> book = new List<string>();
            List<string> name = new List<string>();
            List<string> s = new List<string>();

            try
            {
                using (StreamReader myfile = new StreamReader(filePath))
                {
                    string f;
                    while ((f = myfile.ReadLine()) != null)
                    {
                        string[] data = f.Split(',');
                        if (data.Length >= 3)
                        {
                            book.Add(data[0].Trim());
                            name.Add(data[1].Trim());
                            s.Add(data[2].Trim());
                        }
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("無法打開檔案！");
                return;
            }

            bool foundInBook = false;
            bool foundInAuthor = false;
            string lowerQuery = query.ToLower().Trim(); // 查詢字串轉小寫並去除空白

            // 查詢書本名稱是否包含查詢字串
            List<string> foundBooks = new List<string>(); // 用來儲存符合條件的書名
            for (int j = 0; j < book.Count; j++)
            {
                if (book[j].ToLower().Contains(lowerQuery)) // 子串匹配
                {
                    foundBooks.Add(book[j]);
                    foundInBook = true;
                }
            }

            // 顯示結果
            ResultTextBlock.Text = "";

            if (foundInBook)
            {
                ResultTextBlock.Text += "找到書本:\n";
                foreach (var bookTitle in foundBooks)
                {
                    for (int j = 0; j < book.Count; j++)
                    {
                        if (book[j] == bookTitle)
                        {
                            ResultTextBlock.Text += $"書本: {book[j]}, 作者: {name[j]}\n";
                        }
                    }
                }
            }

            if (!foundInBook)
            {
                List<string> foundAuthorsBooks = new List<string>(); // 儲存作者的書籍
                for (int j = 0; j < name.Count; j++)
                {
                    if (name[j].ToLower().Contains(lowerQuery))
                    {
                        foundAuthorsBooks.Add(name[j]);
                        foundInAuthor = true;
                    }
                }

                if (foundInAuthor)
                {
                    ResultTextBlock.Text += $"作者: {query} 的書本如下:\n";
                    for (int j = 0; j < foundAuthorsBooks.Count; j++)
                    {
                        for (int k = 0; k < name.Count; k++)
                        {
                            if (name[k] == foundAuthorsBooks[j])
                            {
                                ResultTextBlock.Text += $"書本: {book[k]}\n";
                            }
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













