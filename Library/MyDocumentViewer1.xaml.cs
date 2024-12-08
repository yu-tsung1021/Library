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
            BookInfoLabel.Content = "你要預約借閱的書是:\n";
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
            SaveReservationToFile(name, phone);

            MessageBox.Show($"預約成功！\n名子: {name}\n電話號碼: {phone}");
            this.Close();
        }

        private void SaveReservationToFile(string name, string phone)
        {
            string fileName = "預約借書.txt";
            using (StreamWriter sw = new StreamWriter(fileName, true))
            {
                sw.WriteLine("預約借書資料：");
                for (int i = 0; i < books.Count; i++)
                {
                    sw.WriteLine($"書本: {books[i]}, 作者: {authors[i]}");
                }
                sw.WriteLine($"名子: {name}");
                sw.WriteLine($"電話號碼: {phone}");
                sw.WriteLine("----------");
            }
        }
    }
}


