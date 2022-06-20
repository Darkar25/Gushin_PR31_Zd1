using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using static Salon.DB;

namespace Salon.Windows
{
    /// <summary>
    /// Логика взаимодействия для ClientServiceWindow.xaml
    /// </summary>
    public partial class ClientServiceWindow : Window
    {
        public ObservableCollection<service> Services { get; set; }
        public ObservableCollection<client> Clients { get; set; }
        public clientservice Result { get; set; } = new clientservice();
        public ClientServiceWindow()
        {
            Services = new ObservableCollection<service>(DataBase.service.ToList());
            Clients = new ObservableCollection<client>(DataBase.client.ToList());
            InitializeComponent();
        }

        private void ButtonAdv_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void ButtonAdv_Click_1(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
