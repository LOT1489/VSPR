using System;
using System.Collections.Generic;
using System.Data.OleDb;
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

    public partial class ProsmotrPer : Window
    {
        public ProsmotrPer()
        {
            InitializeComponent();
        }

        private void exit_click(object sender, RoutedEventArgs e)
        {
            var autW = new adminS();
            autW.Show();
            Close();
        }

        private void ProsmotrV(object sender, RoutedEventArgs e)
        {
            var autW = new ProsmotrV();
            autW.Show();
            Close();
        }

        private void ProsmotrHistory(object sender, RoutedEventArgs e)
        {
            var autW = new ProsmotrHistory();
            autW.Show();
            Close();
        }
    }
}
