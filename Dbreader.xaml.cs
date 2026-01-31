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

namespace SKLAD
{
    public partial class Dbreader : Window
    {
        public Dbreader()
        {
            InitializeComponent();
        }
        private void Exit(object sender, RoutedEventArgs e)
        {
            var autW = new Gruz();
            autW.Show();
            Close();
        }

        private void AddCGruz_Click(object sender, RoutedEventArgs e)
        {
            string surname = Surname.Text.Trim();
            string name = FirstName.Text.Trim();
            string lastname = LastName.Text.Trim();
            string phone = PhoneNumber.Text.Trim();

            if (!string.IsNullOrEmpty(surname) && !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(lastname) && !string.IsNullOrEmpty(phone))
            {
                try
                {
                    string connectionString = @"Data Source=localhost;Initial Catalog=books;Integrated Security=True";

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "INSERT INTO recipient ([Фамилия], [Имя], [Отчество], [Номер телефона]) " + "VALUES (@surname, @name, @lastname, @phone)";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@surname", surname);
                            command.Parameters.AddWithValue("@name", name);
                            command.Parameters.AddWithValue("@lastname", lastname);
                            command.Parameters.AddWithValue("@phone", phone);
                            command.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Читатель успешно добавлен.");
                    Surname.Clear();
                    FirstName.Clear();
                    LastName.Clear();
                    PhoneNumber.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при добавлении читателя: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Заполните все поля.");
            }
        }
    }
}
