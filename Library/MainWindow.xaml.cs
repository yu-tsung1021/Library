﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
                            inLibrary.Add(data[2].Trim() == "0"); // 0 代表在書庫中，1 代表不在書庫中
                        }
                    }
                }

                /* 顯示讀取的書本資料
                 ResultTextBlock.Text = "讀取的書本資料:\n";
                 for (int i = 0; i < book.Count; i++)
                 {
                     ResultTextBlock.Text += $"書本: {book[i]}, 作者: {name[i]}, 在書庫: {(inLibrary[i] ? "是" : "否")}\n";
                 }*/
            }
            catch (Exception ex)
            {
                MessageBox.Show($"無法打開檔案！錯誤: {ex.Message}");
            }
        }

        public List<string> GetBooks()
        {
            return book;
        }

        public void UpdateBookStatus(List<int> selectedIndices)
        {
            foreach (int index in selectedIndices)
            {
                inLibrary[index] = !inLibrary[index];
            }
            UpdateBooksStackPanel();
            SaveBooksToCsv();
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

            // 清空之前的結果
            BooksStackPanel.Children.Clear();
            ResultTextBlock.Text = "";

            if (foundInBook)
            {
                ResultTextBlock.Text = "找到書本:";
                foreach (var index in foundBookIndices)
                {
                    AddBookToStackPanel(book[index], name[index], inLibrary[index]);
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
                    ResultTextBlock.Text = "找到書本:";
                    HashSet<string> displayedBooks = new HashSet<string>(); // 用來儲存已顯示的書名
                    foreach (var index in foundAuthorIndices)
                    {
                        if (!displayedBooks.Contains(book[index]))
                        {
                            AddBookToStackPanel(book[index], name[index], inLibrary[index]);
                            displayedBooks.Add(book[index]);
                        }
                    }
                }
            }

            if (!foundInBook && !foundInAuthor)
            {
                ResultTextBlock.Text = "沒有找到該書本或作者。";
            }
        }

        private void AddBookToStackPanel(string bookTitle, string author, bool isInLibrary)
        {
            var sp = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(3),
                Background = Brushes.Transparent, // 設為透明
                Height = 35,
            };

            var cb = new CheckBox
            {
                Content = "借書",
                FontFamily = new FontFamily("微軟正黑體"),
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.LightGreen, // 設為淺綠色
                Width = 60,
                Margin = new Thickness(5),
                VerticalContentAlignment = VerticalAlignment.Center,
            };

            var lb_book = new Label
            {
                Content = $"書本: {bookTitle}",
                FontFamily = new FontFamily("微軟正黑體"),
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.LightGreen, // 設為淺綠色
                Width = 200,
                VerticalContentAlignment = VerticalAlignment.Center,
            };

            var lb_author = new Label
            {
                Content = $"作者: {author}",
                FontFamily = new FontFamily("微軟正黑體"),
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.LightGreen, // 設為淺綠色
                Width = 200,
                VerticalContentAlignment = VerticalAlignment.Center,
            };

            var lb_inLibrary = new Label
            {
                Content = $"在書庫: {(isInLibrary ? "是" : "否")}",
                FontFamily = new FontFamily("微軟正黑體"),
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.LightGreen, // 設為淺綠色
                Width = 100,
                VerticalContentAlignment = VerticalAlignment.Center,
            };

            sp.Children.Add(cb);
            sp.Children.Add(lb_book);
            sp.Children.Add(lb_author);
            sp.Children.Add(lb_inLibrary);

            BooksStackPanel.Children.Add(sp);
        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            List<string> selectedBooks = new List<string>();
            List<string> selectedAuthors = new List<string>();
            bool allNotInLibrary = true;

            foreach (StackPanel sp in BooksStackPanel.Children)
            {
                CheckBox cb = sp.Children[0] as CheckBox;
                Label lb_book = sp.Children[1] as Label;
                Label lb_author = sp.Children[2] as Label;
                Label lb_inLibrary = sp.Children[3] as Label;

                if (cb.IsChecked == true)
                {
                    if (lb_inLibrary.Content.ToString().Contains("是"))
                    {
                        allNotInLibrary = false;
                        break;
                    }
                    selectedBooks.Add(lb_book.Content.ToString().Replace("書本: ", ""));
                    selectedAuthors.Add(lb_author.Content.ToString().Replace("作者: ", ""));
                }
            }

            if (!allNotInLibrary)
            {
                MessageBox.Show("書本在書庫中，請使用借書功能。");
                return;
            }

            if (selectedBooks.Count > 0)
            {
                MyDocumentViewer1 viewer = new MyDocumentViewer1(selectedBooks, selectedAuthors);
                viewer.ShowDialog();
            }
            else
            {
                MessageBox.Show("請選擇至少一本書進行預約。");
            }
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            List<string> selectedBooks = new List<string>();
            List<string> selectedAuthors = new List<string>();
            List<int> selectedIndices = new List<int>();
            bool allInLibrary = true;

            foreach (StackPanel sp in BooksStackPanel.Children)
            {
                CheckBox cb = sp.Children[0] as CheckBox;
                Label lb_book = sp.Children[1] as Label;
                Label lb_author = sp.Children[2] as Label;
                Label lb_inLibrary = sp.Children[3] as Label;

                if (cb.IsChecked == true)
                {
                    if (lb_inLibrary.Content.ToString().Contains("否"))
                    {
                        allInLibrary = false;
                        break;
                    }
                    selectedBooks.Add(lb_book.Content.ToString().Replace("書本: ", ""));
                    selectedAuthors.Add(lb_author.Content.ToString().Replace("作者: ", ""));
                    selectedIndices.Add(book.IndexOf(lb_book.Content.ToString().Replace("書本: ", "")));
                }
            }

            if (!allInLibrary)
            {
                MessageBox.Show("只有書本在書庫中時才可以借書。");
                return;
            }

            if (selectedBooks.Count > 0)
            {
                MyDocumentViewer2 viewer = new MyDocumentViewer2(selectedBooks, selectedAuthors, this);
                viewer.ShowDialog();
            }
            else
            {
                MessageBox.Show("請選擇至少一本書進行借書。");
            }
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> selectedBooks = new List<string>();
            List<string> selectedAuthors = new List<string>();
            List<int> selectedIndices = new List<int>();
            bool allBorrowed = true;

            foreach (StackPanel sp in BooksStackPanel.Children)
            {
                CheckBox cb = sp.Children[0] as CheckBox;
                Label lb_book = sp.Children[1] as Label;
                Label lb_author = sp.Children[2] as Label;
                Label lb_inLibrary = sp.Children[3] as Label;

                if (cb.IsChecked == true)
                {
                    if (lb_inLibrary.Content.ToString().Contains("是"))
                    {
                        allBorrowed = false;
                        break;
                    }
                    selectedBooks.Add(lb_book.Content.ToString().Replace("書本: ", ""));
                    selectedAuthors.Add(lb_author.Content.ToString().Replace("作者: ", ""));
                    selectedIndices.Add(book.IndexOf(lb_book.Content.ToString().Replace("書本: ", "")));
                }
            }

            if (!allBorrowed)
            {
                MessageBox.Show("只有書本在被借閱狀態時才可以還書。");
                return;
            }

            if (selectedBooks.Count > 0)
            {
                // 更新書庫布林值
                foreach (int index in selectedIndices)
                {
                    inLibrary[index] = !inLibrary[index];
                }

                // 更新顯示
                UpdateBooksStackPanel();

                // 將變更回寫到 CSV 檔案中
                SaveBooksToCsv();

                MessageBox.Show("還書成功！");
            }
            else
            {
                MessageBox.Show("請選擇至少一本書進行還書。");
            }
        }

        private void UpdateBooksStackPanel()
        {
            foreach (StackPanel sp in BooksStackPanel.Children)
            {
                Label lb_book = sp.Children[1] as Label;
                Label lb_inLibrary = sp.Children[3] as Label;

                int index = book.IndexOf(lb_book.Content.ToString().Replace("書本: ", ""));
                lb_inLibrary.Content = $"在書庫: {(inLibrary[index] ? "是" : "否")}";
            }
        }

        private void SaveBooksToCsv()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    for (int i = 0; i < book.Count; i++)
                    {
                        sw.WriteLine($"{book[i]},{name[i]},{(inLibrary[i] ? "0" : "1")}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"無法寫入檔案！錯誤: {ex.Message}");
            }
        }
    }
}


