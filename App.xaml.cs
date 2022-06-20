﻿using Syncfusion.Licensing;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace Salon
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App() {
            typeof(FusionLicenseProvider).GetField("isLicensed", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, true);
        }
    }
}
