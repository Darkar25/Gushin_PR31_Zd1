using Syncfusion.UI.Xaml.Scheduler;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using static Salon.DB;

namespace Salon.Windows
{
    /// <summary>
    /// Логика взаимодействия для NearestClientsWindow.xaml
    /// </summary>
    public partial class NearestClientsWindow : Window
    {
        DateRange SelectedRange { get; set; }
        public NearestClientsWindow()
        {
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ru-RU");
            InitializeComponent();
            Task.Run(() => {
                while(true)
                {
                    Thread.Sleep(30000);
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                    {
                        var a = DataBase.clientservice
                        .Where(x => x.StartTime >= SelectedRange.StartDate && x.StartTime < DbFunctions.AddDays(SelectedRange.EndDate, 1))
                        .Select(x => new ScheduleAppointment()
                        {
                            StartTime = x.StartTime,
                            EndTime = DbFunctions.AddSeconds(x.StartTime, x.service.DurationInSeconds).Value,
                            Subject = x.service.Title,
                            Id = x
                        }).ToArray();
                        var c = Schedule.ItemsSource as ScheduleAppointmentCollection;
                        foreach (var b in a.Where(x => !c.Any(y => y.Id as clientservice == x.Id as clientservice)))
                        {
                            if (b.StartTime >= DateTime.Now.AddHours(-1))
                                b.Reminders = new ObservableCollection<SchedulerReminder>()
                                {
                                    new SchedulerReminder()
                                    {
                                        ReminderTimeInterval = TimeSpan.FromHours(1)
                                    }
                                };
                            c.Add(b);
                        }
                        var d = c.Where(x => !a.Any(y => y.Id as clientservice == x.Id as clientservice)).ToArray();
                        foreach (var b in d)
                        {
                            c.Remove(b);
                        }
                    }));
                }
            });
        }

        private void Schedule_QueryAppointments(object sender, QueryAppointmentsEventArgs e)
        {
            Schedule.ShowBusyIndicator = true;
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                var arr = new ScheduleAppointmentCollection();
                foreach (var a in DataBase.clientservice
                    .Where(x => x.StartTime >= e.VisibleDateRange.StartDate && x.StartTime < DbFunctions.AddDays(e.VisibleDateRange.EndDate, 1))
                    .Select(x => new ScheduleAppointment()
                    {
                        StartTime = x.StartTime,
                        EndTime = DbFunctions.AddSeconds(x.StartTime, x.service.DurationInSeconds).Value,
                        Subject = x.service.Title,
                        Id = x
                    }))
                {
                    if(a.StartTime >= DateTime.Now.AddHours(-1))
                        a.Reminders = new ObservableCollection<SchedulerReminder>()
                        {
                            new SchedulerReminder()
                            {
                                ReminderTimeInterval = TimeSpan.FromHours(1)
                            }
                        };
                    arr.Add(a);
                }
                Schedule.ItemsSource = arr;
                SelectedRange = e.VisibleDateRange;
                Schedule.ShowBusyIndicator = false;
            }));
        }

        private void Schedule_CellTapped(object sender, CellTappedEventArgs e)
        {
            if (!(e.Appointment is null)) {
                var a = e.Appointment.Id as clientservice;
                /*var span = a.StartTime - DateTime.Now;
                var s = string.Format("{0}{1}{2}",
                    span.TotalDays > 0 ? string.Format("{0:0} дней, ", span.Days) : string.Empty,
                    span.TotalHours > 0 ? string.Format("{0:0} часов, ", span.Hours) : string.Empty,
                    span.TotalMinutes > 0 ? string.Format("{0:0} минут ", span.Minutes) : string.Empty);
                MessageBox.Show($"Название услуги: {a.service.Title}\nФИО Клиента: {a.client.LastName} {a.client.FirstName} {a.client.Patronymic}\nEmail: {a.client.Email}\nТелефон: {a.client.Phone}\nДата и время записи: {a.StartTime}\nДо начала: {s}", "Информация о записи");*/
                new ClientServiceMessageBox(a).ShowDialog();
            }
        }

        private void Schedule_ReminderAlertOpening(object sender, ReminderAlertOpeningEventArgs e)
        {
            foreach(var a in e.Reminders)
                a.Appointment.Foreground = Brushes.Red;
        }
    }
}
