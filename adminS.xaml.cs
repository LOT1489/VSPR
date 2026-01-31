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
    public partial class adminS : Window
    {
        public adminS()
        {
            InitializeComponent();
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            var autW = new Authorization();
            autW.Show();
            Close();
        }

        private void toprper_Click(object sender, RoutedEventArgs e)
        {
            var autW = new ProsmotrPer();
            autW.Show();
            Close();
        }

        private void dobya_Click(object sender, RoutedEventArgs e)
        {
            var autW = new dobya();
            autW.Show();
            Close();
        }

        private void prosmotrya(object sender, RoutedEventArgs e)
        {
            var autW = new Prosmotrya();
            autW.Show();
            Close();
        }

        private void prosmotrgr(object sender, RoutedEventArgs e)
        {
            var autW = new Prosmotrgr();
            autW.Show();
            Close();
        }

        private void Fillreader(object sender, RoutedEventArgs e)
        {
            var readeradd = new Priemgruza();
            readeradd.Show();
            Close();
        }

        private void Extradition(object sender, RoutedEventArgs e)
        {
            var extr = new Extradition();
            extr.Show();
            Close();
        }
    }
}
