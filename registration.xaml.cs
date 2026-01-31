using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace SKLAD
{
    public partial class registration : Window
    {
        public registration()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            string connectionString = @"Data Source=localhost;Initial Catalog=books;Integrated Security=True";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT Логин FROM employers";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        UserComboBox.Items.Clear();
                        while (reader.Read())
                        {
                            UserComboBox.Items.Add(reader["Логин"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке сотрудников: " + ex.Message);
            }
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (UserComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите сотрудника для удаления.");
                return;
            }

            string selectedUser = UserComboBox.SelectedItem.ToString();
            string connectionString = @"Data Source=localhost;Initial Catalog=books;Integrated Security=True";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM employers WHERE Логин = @login";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@login", selectedUser);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Сотрудник успешно удален.");
                            LoadUsers();
                        }
                        else
                        {
                            MessageBox.Show("Ошибка при удалении сотрудника.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void registr_Click(object sender, RoutedEventArgs e)
        {
            string login = Login.Text;
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;
            string role = RegisterRole.Text;
            string surname = Surname.Text;
            string name = Name.Text;
            string otch = Otch.Text;
            string hashedPass = HashPassword(password);

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword) || string.IsNullOrWhiteSpace(role) ||
                string.IsNullOrWhiteSpace(surname) || string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            if (!IsComplex(login, password, role))
            {
                MessageBox.Show("Пароль должен содержать не менее 8 символов, включать строчные и заглавные буквы, " +
                    "цифры и специальные символы. Логин и пароль не могут содержать пробелов и символа :");
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают.");
                return;
            }

            if (UserExists(login))
            {
                MessageBox.Show("Пользователь с таким логином уже существует.");
                return;
            }

            SaveEmployeeToDatabase(login, hashedPass, role, surname, name, otch);

            Login.Clear();
            PasswordBox.Clear();
            ConfirmPasswordBox.Clear();
            RegisterRole.Text = "";
            Surname.Clear();
            Name.Clear();
            Otch.Clear();

            MessageBox.Show("Регистрация успешна");
        }

        private void SaveEmployeeToDatabase(string login, string hashedPass, string role, string surname, string name, string otch)
        {
            string connectionString = @"Data Source=localhost;Initial Catalog=books;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO employers (Логин, Пароль, Фамилия, Имя, Отчество, Должность) " +
                                   "VALUES (@Логин, @Пароль, @Фамилия, @Имя, @Отчество, @Должность)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Логин", login);
                        command.Parameters.AddWithValue("@Пароль", hashedPass);
                        command.Parameters.AddWithValue("@Фамилия", surname);
                        command.Parameters.AddWithValue("@Имя", name);
                        command.Parameters.AddWithValue("@Отчество", otch);
                        command.Parameters.AddWithValue("@Должность", role);

                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при сохранении данных: " + ex.Message);
                }
            }
        }

        private bool UserExists(string login)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["BOOKS"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM employers WHERE Логин = @Логин";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Логин", login);
                        int count = (int)command.ExecuteScalar();
                        return count > 0;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при проверке существования пользователя: " + ex.Message);
                    return false;
                }
            }
        }

        private bool IsComplex(string login, string password, string role)
        {
            if (password.Length < 8) { return false; }
            bool hasUpper = false, hasLower = false, hasDigit = false, hasSpecial = false, hasSpace = false;

            foreach (char c in password)
            {
                if (char.IsUpper(c)) { hasUpper = true; }
                if (char.IsLower(c)) { hasLower = true; }
                if (char.IsDigit(c)) { hasDigit = true; }
                if (!char.IsLetterOrDigit(c) && c != ':') { hasSpecial = true; }
                if (char.IsWhiteSpace(c)) { hasSpace = true; }
            }
            foreach (char c in login)
            {
                if (c == ':') { hasSpecial = false; }
                if (char.IsWhiteSpace(c)) { hasSpace = true; }
            }

            foreach (char c in role)
            {
                if (c == ':') { hasSpecial = false; }
                if (char.IsWhiteSpace(c)) { hasSpace = true; }
            }
            return hasUpper && hasLower && hasDigit && hasSpecial && !hasSpace;
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }


        private void exit_Click(object sender, RoutedEventArgs e)
        {
            var autW = new Authorization();
            autW.Show();
            Close();
        }

        private void prosmotr_Click(object sender, RoutedEventArgs e)
        {
            var autW = new ProsmotrSotr();
            autW.Show();
            Close();
        }

    }
}
