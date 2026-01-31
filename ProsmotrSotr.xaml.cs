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
    public partial class ProsmotrSotr : Window
    {
        public ProsmotrSotr()
        {
            InitializeComponent();
            LoadCellsData();
        }

        private void exit_click(object sender, RoutedEventArgs e)
        {
            var autW = new registration();
            autW.Show();
            Close();
        }

        private void LoadCellsData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection("Data Source=localhost;Initial Catalog=books;Integrated Security=True"))
                {
                    connection.Open();
                    string query = "SELECT * FROM employers";
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                    {
                        DataTable cellsTable = new DataTable();
                        adapter.Fill(cellsTable);
                        datasotr.ItemsSource = cellsTable.DefaultView;
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
