using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
using static Salon.DB;

namespace Salon.Windows
{
    /// <summary>
    /// Логика взаимодействия для ServiceEditWindow.xaml
    /// </summary>
    public partial class ServiceEditWindow : Window
    {
        public service Service { get; set; }
        public ServiceEditWindow(service service)
        {
            Service = service;
            InitializeComponent();
        }

        private void name_TextChanged(object sender, TextChangedEventArgs e)
        {
            namelayout.HasError = DataBase.service.Any(x => x.Title == name.Text && x.ID != Service.ID);
        }

        private void ButtonAdv_Click(object sender, RoutedEventArgs e)
        {
            var f = new OpenFileDialog() { Filter = "Image file|*.jpeg;*.png;*.jpg", Title = "Выберите изображение" };
            if(f.ShowDialog().Value)
            {
                var nf = Path.Combine("Услуги салона красоты", Guid.NewGuid() + Path.GetExtension(f.FileName));
                File.Copy(f.FileName, nf);
                var p = new servicephoto() { PhotoPath = nf };
                Service.servicephoto.Add(p);
            }
        }

        private void ButtonAdv_Click_1(object sender, RoutedEventArgs e)
        {
            Service.servicephoto.Remove(photos.SelectedItem as servicephoto);
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var f = new OpenFileDialog() { Filter = "Image file|*.jpeg;*.png;*.jpg", Title = "Выберите изображение" };
            if (f.ShowDialog().Value)
            {
                var nf = Path.Combine("Услуги салона красоты", Guid.NewGuid() + Path.GetExtension(f.FileName));
                File.Copy(f.FileName, nf);
                Service.MainImagePath = nf;
            }
        }

        private void ButtonAdv_Click_2(object sender, RoutedEventArgs e)
        {
            if (!namelayout.HasError) {
                DialogResult = true;
                Close();
            }
        }

        private void ButtonAdv_Click_3(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
