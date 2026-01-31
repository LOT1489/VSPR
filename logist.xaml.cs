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

namespace SKLAD
{
    public partial class logist : Window
    {
        public logist()
        {
            InitializeComponent();
        }

        private void exit(object sender, RoutedEventArgs e)
        {
            var autW = new Authorization();
            autW.Show();
            Close();
        }
        private void togruz(object sender, RoutedEventArgs e)
        {
            var autW = new Gruz();
            autW.Show();
            Close();
        }

        private void tocell(object sender, RoutedEventArgs e)
        {
            var autW = new cell_logist();
            autW.Show();
            Close();
        }

    }
}
