//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Salon
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;

    public partial class servicephoto : INotifyPropertyChanged
    {
        public int ID { get; set; }
        public int ServiceID { get; set; }
        string _img;

        public event PropertyChangedEventHandler PropertyChanged;

        public string PhotoPath
        {
            get => Path.Combine("pack://siteoforigin:,,,/", _img ?? ""); set
            {
                _img = System.Net.WebUtility.UrlDecode(value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PhotoPath)));
            }
        }

        public virtual service service { get; set; }
    }
}
