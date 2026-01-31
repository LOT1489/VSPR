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
    public partial class Gruz : Window
    {
        public Gruz()
        {
            InitializeComponent();
        }

        private void exit(object sender, RoutedEventArgs e)
        {
            var autW = new logist();
            autW.Show();
            Close();
        }

        private void prosmotrgr(object sender, RoutedEventArgs e)
        {
            var autW = new Prosmotrgr11();
            autW.Show();
            Close();
        }

        private void togruz(object sender, RoutedEventArgs e)
        {
            var autW = new Dbreader();
            autW.Show();
            Close();
        }

    }
}
