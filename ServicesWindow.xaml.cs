using System.Collections.Generic;
using System.Windows;
using System.Linq;
using static Salon.DB;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Salon.Windows;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Syncfusion.Windows.Controls.Navigation;

namespace Salon
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class ServicesWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<service> Services { get; set; }
        public int Total => DataBase.service.Count();
        public bool IsAdmin => DB.IsAdmin;
        public event PropertyChangedEventHandler PropertyChanged;
        bool menuopenfired = false;
        bool? srt = null;

        public ServicesWindow()
        {
            InitializeComponent();
            Reload();
        }

        public void Reload() {
            var s = DataBase.service.AsQueryable();
            if (slidr.RangeStart != slidr.Minimum || slidr.RangeEnd != slidr.Maximum)
                s = s.Where(x => (x.Discount.HasValue ? x.Discount.Value : 0d) >= (double)slidr.RangeStart && (x.Discount.HasValue ? x.Discount.Value : 0d) < (double)slidr.RangeEnd);
            if (!string.IsNullOrEmpty(name.Text))
                s = s.Where(x => x.Title.Contains(name.Text));
            if (!string.IsNullOrEmpty(desc.Text))
                s = s.Where(x => x.Description.Contains(desc.Text));
            if(srt.HasValue)
            {
                if (srt.Value)
                    s = s.OrderByDescending(x => x.Cost);
                else
                    s = s.OrderBy(x => x.Cost);
            }
            Services = new ObservableCollection<service>(s.ToArray());
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Services)));
        }

        private void SfRangeSlider_RangeChanged(object sender, Syncfusion.Windows.Controls.Input.RangeChangedEventArgs e) => Reload();

        private void name_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) => Reload();

        private void SfRadialMenu_Opened(object sender, RoutedEventArgs e)
        {
            if(!menuopenfired)
            {
                menuopenfired = true;
                return;
            }
            if (!IsAdmin)
            {
                var pass = new AdminPasswordPrompt();
                if (!pass.ShowDialog().Value)
                {
                    menu.Close();
                }
                else if (!EnableAdminMode(pass.Password))
                {
                    menu.Close();
                    MessageBox.Show("Вы ввели неправильный пароль!", "Неверный пароль!");
                }
                else
                {
                    Reload();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAdmin)));
                }
            }
        }

        private void ButtonAdv_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены что хотите удалить эту услугу?", "Подтверждение действия", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                var s = (sender as ButtonAdv).DataContext as service;
                if (s.clientservice.Count > 0)
                {
                    MessageBox.Show("Невозможно удалить эту услугу. На эту услугу есть записи.", "Ошибка при удалении", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                service restore = null;
                try
                {
                    DataBase.servicephoto.RemoveRange(s.servicephoto);
                    restore = DataBase.service.Remove(s);
                    DataBase.SaveChanges();
                    Reload();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Total)));
                } catch(Exception) {
                    if (!(restore is null)) DataBase.Entry(restore).Reload();
                    MessageBox.Show("Во время удаления записи произошло неизвестная ошибка.", "Ошибка при удалении", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }

        private void SfRadialMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var a = new ServiceEditWindow(new service()) { Title = "Добавление услуги" };
                var b = a.ShowDialog();
                if (b.HasValue && b.Value)
                {
                    DataBase.service.Add(a.Service);
                    DataBase.SaveChanges();
                    Reload();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Total)));
                }
            } catch(Exception)
            {
                MessageBox.Show("Во время добавления услуги произошла неизвестная ошибка.", "Ошибка при добавлении", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonAdv_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                var a = new ServiceEditWindow((sender as ButtonAdv).DataContext as service);
                var b = a.ShowDialog();
                if (b.HasValue && b.Value)
                {
                    DataBase.SaveChanges();
                    Reload();
                }
                else DataBase.Entry(a.Service).Reload();
            } catch(Exception)
            {
                MessageBox.Show("Во время изменения услуги произошла неизвестная ошибка.", "Ошибка при изменении", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SfRadialMenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                var a = new ClientServiceWindow();
                var b = a.ShowDialog();
                if (b.HasValue && b.Value)
                {
                    DataBase.clientservice.Add(a.Result);
                    DataBase.SaveChanges();
                }
            } catch(Exception)
            {
                MessageBox.Show("Во время записи клиента на услугу произошла неизвестная ошибка.", "Ошибка при добавлении", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SfRadialMenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            new NearestClientsWindow().ShowDialog();
        }

        private void sort_Click(object sender, RoutedEventArgs e)
        {
            if(srt.HasValue)
            {
                if(srt.Value)
                {
                    srt = null;
                    sort.SmallIcon = new BitmapImage(new Uri("pack://application:,,,/Salon;component/images/no_sort.png"));
                } else
                {
                    sort.SmallIcon = new BitmapImage(new Uri("pack://application:,,,/Salon;component/images/sort.png"));
                    srt = true;
                }
            } else
            {
                sort.SmallIcon = new BitmapImage(new Uri("pack://application:,,,/Salon;component/images/sort_asc.png"));
                srt = false;
            }
            Reload();
        }
    }
}
