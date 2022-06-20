using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Salon
{
    static class DB
    {
        static salonEntities _DB;
        public static salonEntities DataBase => _DB is null ? _DB = new salonEntities() : _DB;
        public static bool IsAdmin { get; set; }
        public static bool EnableAdminMode(string password) {
            try
            {
                string login = enclogin.AESDecrypt(password);
                string pass = encpass.AESDecrypt(password);
                _DB = new salonEntities($"metadata=res://*/SalonModel.csdl|res://*/SalonModel.ssdl|res://*/SalonModel.msl;provider=MySql.Data.MySqlClient;provider connection string=\"server=play.firetype.ru;user id={login};database=salon;password={pass}\"");
                return IsAdmin = true;
            } catch(Exception) { }
            return false;
        }

        public static void DisableAdminMode() {
            IsAdmin = false;
            _DB = new salonEntities();
        }

        public static byte[] enclogin = new byte[] { 26, 74, 93, 95, 4, 211, 153, 237, 157, 197, 217, 60, 195, 80, 117, 200, 142, 141, 161, 17, 98, 103, 176, 100, 91, 193, 20, 117, 119, 16, 31, 111 };
        public static byte[] encpass = new byte[] { 10, 240, 33, 69, 151, 148, 203, 159, 84, 34, 244, 163, 197, 47, 5, 176, 46, 237, 53, 200, 152, 68, 246, 6, 105, 105, 37, 41, 252, 3, 159, 156, 175, 177, 218, 119, 222, 115, 200, 99, 85, 88, 54, 174, 20, 226, 97, 137 };
    }
}
