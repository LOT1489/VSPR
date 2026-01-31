using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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
    public partial class Prosmotrya11 : Window
    {
        public Prosmotrya11()
        {
            InitializeComponent();
            LoadCellsData();
        }

        private void exit(object sender, RoutedEventArgs e)
        {
            var autW = new cell_logist();
            autW.Show();
            Close();
        }

        private void LoadCellsData()
        {
            try
            {
string connectionString = @"Data Source=localhost;Initial Catalog=books;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM books";
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                    {
                        DataTable books = new DataTable();
                        adapter.Fill(books);
                        dataya.ItemsSource = books.DefaultView;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

    }
}
