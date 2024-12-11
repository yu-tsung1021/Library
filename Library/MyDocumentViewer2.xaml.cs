using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace Library
{
    public partial class MyDocumentViewer2 : Window
    {
        private List<string> books;
        private List<string> authors;
        private MainWindow mainWindow;

        public MyDocumentViewer2(List<string> books, List<string> authors, MainWindow mainWindow)
        {
            InitializeComponent();
            this.books = books;
            this.authors = authors;
            this.mainWindow = mainWindow;
            DisplayBookInfo(books, authors);
        }

        private void DisplayBookInfo(List<string> books, List<string> authors)
        {
            BookInfoLabel.Content = "你要借閱的書是:\n";
            for (int i = 0; i < books.Count; i++)
            {
                BookInfoLabel.Content += $"書本: {books[i]}, 作者: {authors[i]}\n";
            }
            BookInfoLabel.Content += "請留下名子和電話號碼。";
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text;
            string phone = PhoneTextBox.Text;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("請填寫名子和電話號碼。");
                return;
            }

            if (!Regex.IsMatch(phone, @"^\d+$"))
            {
                MessageBox.Show("電話號碼必須是數字。");
                return;
            }

            // 儲存書本資料和名子電話號碼到 txt 檔案
            SaveBorrowingToFile(name, phone);

            // 更新書庫布林值
            List<int> selectedIndices = new List<int>();
            foreach (string bookTitle in books)
            {
                selectedIndices.Add(mainWindow.GetBooks().IndexOf(bookTitle));
            }
            DateTime returnDate = DateTime.Now.AddDays(14); // 計算還書時間
            mainWindow.UpdateBookStatus(selectedIndices, returnDate);

            MessageBox.Show($"借閱成功！\n名子: {name}\n電話號碼: {phone}\n借書時間: {DateTime.Now}\n還書時間: {DateTime.Now.AddDays(14)}");
            this.Close();
        }

        private void SaveBorrowingToFile(string name, string phone)
        {
            string fileName = "借書.txt";
            using (StreamWriter sw = new StreamWriter(fileName, true))
            {
                sw.WriteLine("借書資料：");
                for (int i = 0; i < books.Count; i++)
                {
                    sw.WriteLine($"書本: {books[i]}, 作者: {authors[i]}");
                }
                sw.WriteLine($"名子: {name}");
                sw.WriteLine($"電話號碼: {phone}");
                sw.WriteLine($"提交時間: {DateTime.Now}");
                sw.WriteLine($"還書時間: {DateTime.Now.AddDays(14)}");// 紀錄提交時間
                sw.WriteLine("----------");
            }
        }
    }
}


