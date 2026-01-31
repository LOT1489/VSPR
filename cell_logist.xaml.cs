using System;
using System.Collections.Generic;
using System.Data.Common;
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
    public partial class cell_logist : Window
    {
        public cell_logist()
        {
            InitializeComponent();
        }

        private void exit(object sender, RoutedEventArgs e)
        {
            var autW = new logist();
            autW.Show();
            Close();
        }

        private void prosmotr(object sender, RoutedEventArgs e)
        {
            var autW = new Prosmotrya11();
            autW.Show();
            Close();
        }

        private void fillcell(object sender, RoutedEventArgs e)
        {
            var autW = new Fill_cell();
            autW.Show();
            Close();
        }

        private void history(object sender, RoutedEventArgs e)
        {
            var autW = new history();
            autW.Show();
            Close();
        }
    }
}
