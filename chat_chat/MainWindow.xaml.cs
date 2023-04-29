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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace chat_chat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static bool IsServer = false;

        public MainWindow()
        {
            InitializeComponent();

        }

        private void craete_Click(object sender, RoutedEventArgs e)
        {
            IsServer = true;
            windwow windwow = new windwow(name.Text, ip_con.Text);
            windwow.Show();
            this.Close();
        }

        private void connect_Click(object sender, RoutedEventArgs e)
        {
            IsServer = false;
            windwow windwow = new windwow(name.Text, ip_con.Text);
            windwow.Show();

            this.Close();
        }

    }
}
