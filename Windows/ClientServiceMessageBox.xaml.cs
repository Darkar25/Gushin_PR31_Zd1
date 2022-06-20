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

namespace Salon.Windows
{
    /// <summary>
    /// Логика взаимодействия для CientServiceMessageBox.xaml
    /// </summary>
    public partial class ClientServiceMessageBox : Window
    {
        public clientservice cs { get; set; }
        public string StartText { get; set; }
        public ClientServiceMessageBox(clientservice clientservice)
        {
            cs = clientservice;
            var span = cs.StartTime - DateTime.Now;
            StartText = string.Format("{0}{1}{2}",
                span.TotalDays > 0 ? string.Format("{0:0} дней, ", span.Days) : string.Empty,
                span.TotalHours > 0 ? string.Format("{0:0} часов, ", span.Hours) : string.Empty,
                span.TotalMinutes > 0 ? string.Format("{0:0} минут ", span.Minutes) : string.Empty);
            InitializeComponent();
            if (cs.StartTime >= DateTime.Now.AddHours(-1) && cs.StartTime <= DateTime.Now.AddHours(1))
                starttime.Foreground = Brushes.Red;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
