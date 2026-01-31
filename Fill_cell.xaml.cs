using System;
using System.Collections.Generic;
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
using System.Data.OleDb;
using System.Data.Common;
using System.Data.SqlClient;

namespace SKLAD
{
    public partial class Fill_cell : Window
    {
        public Fill_cell()
        {
            InitializeComponent();
            LoadBooks();
            LoadRecipients();
            LoadExtraditedBooks();
        }

        public class Book
        {
            public string Name { get; set; }
        }

        public class Recipient
        {
            public int ID { get; set; }
            public string Name { get; set; }  // ФИО
        }

        private void LoadBooks()
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
                        bookS.Items.Clear();  // Очищаем ComboBox перед загрузкой новых данных

                        while (reader.Read())
                        {
                            // Создаем объект Book для каждой строки
                            var book = new Book
                            {
                                Name = reader["Наименование"].ToString()
                            };

                            // Добавляем объект в ComboBox
                            bookS.Items.Add(book);
                        }

                        // Указываем, какое свойство объекта Book будет отображаться
                        bookS.DisplayMemberPath = "Name";  // Отображаем свойство Name для каждого объекта Book
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке книг: " + ex.Message);
            }
        }

        private void LoadRecipients()
        {
            string connectionString = @"Data Source=localhost;Initial Catalog=books;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT ID, Фамилия, Имя, Отчество FROM recipient"; // Замените на вашу таблицу и поля

                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    List<Recipient> recipients = new List<Recipient>();
                    while (reader.Read())
                    {
                        recipients.Add(new Recipient
                        {
                            ID = Convert.ToInt32(reader["ID"]),
                            Name = $"{reader["Фамилия"]} {reader["Имя"]} {reader["Отчество"]}" // Формируем ФИО
                        });
                    }

                    // Устанавливаем ItemsSource для ComboBox
                    readerS.ItemsSource = recipients;
                    readerS.DisplayMemberPath = "Name";  // Отображаем ФИО
                    readerS.SelectedValuePath = "ID";  // Сохраняем только ID
                }
            }
        }

        private void Extr(object sender, RoutedEventArgs e)
        {
            // ПРАВИЛЬНО получаем название книги
            if (bookS.SelectedItem == null)
            {
                MessageBox.Show("Выберите книгу.");
                return;
            }
            string bookName = (bookS.SelectedItem as Book)?.Name;

            // Проверяем, что книга выбрана
            if (string.IsNullOrEmpty(bookName))
            {
                MessageBox.Show("Выберите книгу.");
                return;
            }

            // Показываем что реально выбралось, для проверки
            MessageBox.Show("Искомая книга: " + bookName);

            // Получаем ID получателя из ComboBox
            if (readerS.SelectedValue == null)
            {
                MessageBox.Show("Выберите получателя.");
                return;
            }

            int recipientId = (int)readerS.SelectedValue;

            // Проверяем дату
            if (string.IsNullOrEmpty(date.Text) || !DateTime.TryParse(date.Text, out DateTime issueDate))
            {
                MessageBox.Show("Введите корректную дату выдачи.");
                return;
            }

            // Проверяем логин ответственного
            if (string.IsNullOrEmpty(UserSession.CurrentUserLogin))
            {
                MessageBox.Show("Не удалось получить логин ответственного.");
                return;
            }

            string connectionString = @"Data Source=localhost;Initial Catalog=books;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Проверяем, существует ли книга
                string checkBookQuery = "SELECT COUNT(*) FROM books WHERE Наименование = @bookName";
                using (SqlCommand checkBookCommand = new SqlCommand(checkBookQuery, connection))
                {
                    checkBookCommand.Parameters.AddWithValue("@bookName", bookName);

                    int count = (int)checkBookCommand.ExecuteScalar();

                    if (count == 0)
                    {
                        MessageBox.Show("Книга не найдена в базе данных.");
                        return;
                    }
                }

                // Вставляем данные в таблицу extradition
                string insertQuery = "INSERT INTO extradition (Книга, Ответственный, Получатель, [Дата выдачи]) " +
                                     "VALUES (@book, @responsible, @recipientId, @issueDate)";

                using (SqlCommand command = new SqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@book", bookName);
                    command.Parameters.AddWithValue("@responsible", UserSession.CurrentUserLogin);
                    command.Parameters.AddWithValue("@recipientId", recipientId);
                    command.Parameters.AddWithValue("@issueDate", issueDate);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Выдача книги успешна!");

                        string updateBookQuery = "UPDATE books SET Статус = 'Выдана' WHERE Наименование = @bookName";
                        using (SqlCommand updateBookCommand = new SqlCommand(updateBookQuery, connection))
                        {
                            updateBookCommand.Parameters.AddWithValue("@bookName", bookName);
                            updateBookCommand.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при выдаче книги.");
                    }
                }
            }
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            var exit = new cell_logist();
            exit.Show();
            Close();
        }

        private void LoadExtraditedBooks()
        {
            string connectionString = @"Data Source=localhost;Initial Catalog=books;Integrated Security=True";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT DISTINCT Книга FROM extradition WHERE [Дата возврата] IS NULL";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        vozvrat.Items.Clear();  // Очищаем ComboBox перед загрузкой новых данных

                        while (reader.Read())
                        {
                            var book = new Book
                            {
                                Name = reader["Книга"].ToString()
                            };

                            vozvrat.Items.Add(book);
                        }

                        vozvrat.DisplayMemberPath = "Name";  // Отображаем свойство Name для каждой книги
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке книг для возврата: " + ex.Message);
            }
        }

        private void Voz(object sender, RoutedEventArgs e)
        {
            // Получаем выбранную книгу из ComboBox
            if (vozvrat.SelectedItem == null)
            {
                MessageBox.Show("Выберите книгу для возврата.");
                return;
            }

            string bookName = (vozvrat.SelectedItem as Book)?.Name;

            // Проверяем, что книга выбрана
            if (string.IsNullOrEmpty(bookName))
            {
                MessageBox.Show("Выберите книгу для возврата.");
                return;
            }

            // Получаем дату из TextBox
            if (string.IsNullOrEmpty(data.Text) || !DateTime.TryParse(data.Text, out DateTime returnDate))
            {
                MessageBox.Show("Введите корректную дату возврата.");
                return;
            }

            string connectionString = @"Data Source=localhost;Initial Catalog=books;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Проверяем, существует ли запись о выдаче книги
                string checkExtraditionQuery = "SELECT COUNT(*) FROM extradition WHERE Книга = @bookName AND [Дата возврата] IS NULL";
                using (SqlCommand checkExtraditionCommand = new SqlCommand(checkExtraditionQuery, connection))
                {
                    checkExtraditionCommand.Parameters.AddWithValue("@bookName", bookName);

                    int count = (int)checkExtraditionCommand.ExecuteScalar();

                    if (count == 0)
                    {
                        MessageBox.Show("Для этой книги нет записи о выдаче или она уже возвращена.");
                        return;
                    }
                }

                // Обновляем дату возврата в таблице extradition
                string updateExtraditionQuery = "UPDATE extradition SET [Дата возврата] = @returnDate WHERE Книга = @bookName AND [Дата возврата] IS NULL";
                using (SqlCommand updateExtraditionCommand = new SqlCommand(updateExtraditionQuery, connection))
                {
                    updateExtraditionCommand.Parameters.AddWithValue("@returnDate", returnDate);
                    updateExtraditionCommand.Parameters.AddWithValue("@bookName", bookName);

                    int rowsAffected = updateExtraditionCommand.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        // Обновляем статус книги в таблице books
                        string updateBookQuery = "UPDATE books SET Статус = 'Не выдана' WHERE Наименование = @bookName";
                        using (SqlCommand updateBookCommand = new SqlCommand(updateBookQuery, connection))
                        {
                            updateBookCommand.Parameters.AddWithValue("@bookName", bookName);
                            updateBookCommand.ExecuteNonQuery();
                        }

                        MessageBox.Show("Книга возвращена успешно!");
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при возврате книги.");
                    }
                }
            }
        }
    }
}