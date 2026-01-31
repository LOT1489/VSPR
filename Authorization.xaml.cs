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
using System.Windows.Threading;
using System.IO;
using System.Security.Cryptography;
using System.Data.SqlClient;
/*
 (*|*)
(#####)
(#####)
 _| |_
*/
namespace SKLAD
{
    public static class UserSession
    {
        public static string CurrentUserLogin { get; set; }
        public static string CurrentUserRole { get; set; }
    }
    public partial class Authorization : Window
    {
        private int loginAttempts = 0;
        private const int MaxLoginAttempts = 3;
        private const int LockoutTimeInSeconds = 10;
        private DispatcherTimer lockoutTimer;

        public Authorization()
        {
            InitializeComponent();
            LockoutTimer();
        }
        private void LockoutTimer()
        {
            lockoutTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(LockoutTimeInSeconds)
            };
            lockoutTimer.Tick += UnlockLogin;
        }
        private void authorization_click(object sender, RoutedEventArgs e)
        {

            if (lockoutTimer.IsEnabled)
            {
                ShowError($"Попробуйте снова через {LockoutTimeInSeconds} секунд.");
                return;
            }

            string login = Login.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                ShowError("Пожалуйста, заполните все поля.");
                return;
            }

            string check = CheckPass(login, password);

            if (login == "Admin" && password == "Admin")
            {
                var admin = new registration();
                admin.Show();
                Close();
            }
         
            if (check != "false")
            {
                UserSession.CurrentUserLogin = login;
                MessageBox.Show("Логин пользователя: " + UserSession.CurrentUserLogin);
                UserSession.CurrentUserRole = check;

                if (check == "ГлавныйБиблиотекарь") // Proverka1!
                {
                    var admW = new adminS();
                    admW.Show();
                    Close();
                }
                else if (check == "Библиотекарь") // Proverka2!
                {
                    var logW = new logist();
                    logW.Show();
                    Close();
                }
                
            }
            else
            {
                loginAttempts++;
                if (loginAttempts >= MaxLoginAttempts)
                {

                    ShowError($"Аккаунт заблокирован на {LockoutTimeInSeconds} секунд. Повторите попытку позже");
                    if (!lockoutTimer.IsEnabled)
                    {
                        lockoutTimer.Start();
                    }

                }
                else
                {
                    ShowError($"Неверные логин или пароль. Осталось попыток: {MaxLoginAttempts - loginAttempts}");
                }
            }
        }

        private void ShowError(string message)
        {
            ErrorText.Text = message;
            ErrorText.Visibility = Visibility.Visible;
        }

        private string CheckPass(string login, string password)
        {
            string connectionString = @"Data Source=localhost;Initial Catalog=books;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Пароль, Должность FROM employers WHERE Логин = @login";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@login", login);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string savedHashedPassword = reader["Пароль"].ToString();
                            string role = reader["Должность"].ToString();

                            string hashedPassword = HashPassword(password);

                            if (hashedPassword == savedHashedPassword)
                            {
                                return role.Trim();
                            }
                        }
                    }
                }
            }

            return "false";
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] saltedPassword = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(saltedPassword);
                return Convert.ToBase64String(hashBytes);
            }
        }

        private void UnlockLogin(object sender, EventArgs e)
        {
            lockoutTimer.Stop();
            loginAttempts = 0;
            ShowError("Вы можете попытаться войти снова.");
        }
        private void back_click(object sender, RoutedEventArgs e)
        {
            var mainW = new MainWindow();
            mainW.Show();
            Close();
        }
    }
}