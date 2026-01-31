using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;


namespace SKLAD
{
    public partial class dobya : Window
    {
        public dobya()
        {
            InitializeComponent();
            Loadbooks();
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            var adm = new adminS();
            adm.Show();
            Close();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            string bookName = name.Text.Trim();
            string authorName = author.Text.Trim();
            string storageLocation = location.Text.Trim();

            if (!string.IsNullOrEmpty(bookName) && !string.IsNullOrEmpty(authorName) && !string.IsNullOrEmpty(storageLocation))
            {
                try
                {
                    string connectionString = @"Data Source=localhost;Initial Catalog=books;Integrated Security=True";

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "INSERT INTO books (Наименование, Автор, Статус, [Место хранения]) VALUES (@name, @autor, 'Не выдана', @location)";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@name", bookName);
                            command.Parameters.AddWithValue("@autor", authorName);
                            command.Parameters.AddWithValue("@location", storageLocation);
                            command.ExecuteNonQuery();
                        }
                    }
                    MessageBox.Show("Книга успешно добавлена!");
                    name.Clear();
                    author.Clear();
                    location.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Заполните поля до конца");
            }
        }

        private void Loadbooks()
        {
            string connectionString = @"Data Source=localhost;Initial Catalog=books;Integrated Security=True";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT Наименование FROM books";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        books.Items.Clear();
                        while (reader.Read())
                        {
                            books.Items.Add(reader["Наименование"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке списка книг: " + ex.Message);
            }
        }

        private void Deletebook(object sender, RoutedEventArgs e)
        {
            if (books.SelectedItem == null)
            {
                MessageBox.Show("Выберите книгу для удаления.");
                return;
            }

            string selectedbooks = books.SelectedItem.ToString();
            string connectionString = @"Data Source=localhost;Initial Catalog=books;Integrated Security=True";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM books WHERE Наименование = @name";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", selectedbooks);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Книга удалена из каталога.");
                            Loadbooks();
                        }
                        else
                        {
                            MessageBox.Show("Ошибка при удалении.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
    }
}
