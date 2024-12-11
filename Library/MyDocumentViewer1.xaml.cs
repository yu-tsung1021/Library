using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace Library
{
    public partial class MyDocumentViewer1 : Window
    {
        private List<string> books;
        private List<string> authors;

        public MyDocumentViewer1(List<string> books, List<string> authors)
        {
            InitializeComponent();
            this.books = books;
            this.authors = authors;
            DisplayBookInfo(books, authors);
        }

        private void DisplayBookInfo(List<string> books, List<string> authors)
        {
            BookInfoLabel.Content = "你要預約的書是:\n";
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

            // 儲存預約資料到 txt 檔案
            SaveReservationToFile(name, phone);

            MessageBox.Show($"預約成功！\n名子: {name}\n電話號碼: {phone}\n預約時間: {DateTime.Now}");
            this.Close();
        }

        private void SaveReservationToFile(string name, string phone)
        {
            string directoryPath = @"C:\Users\qwert\OneDrive\文件\library專題\Library";
            string fileName = "預約.txt";
            string filePath = Path.Combine(directoryPath, fileName);

            // 確保目錄存在
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            using (StreamWriter sw = new StreamWriter(filePath, true))
            {
                sw.WriteLine("預約資料：");
                for (int i = 0; i < books.Count; i++)
                {
                    sw.WriteLine($"書本: {books[i]}, 作者: {authors[i]}");
                }
                sw.WriteLine($"名子: {name}");
                sw.WriteLine($"電話號碼: {phone}");
                sw.WriteLine($"預約時間: {DateTime.Now}");
                sw.WriteLine("----------");
            }
        }
    }
}
