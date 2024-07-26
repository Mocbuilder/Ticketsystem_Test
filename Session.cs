using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Hashing_Test
{
    public class Session
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool isAdmin { get; set; }

        public Session(string name)
        {
            ID = 0;

            Name = name;

            Email = "";

            isAdmin = false;
        }

        public void GetSessionValues(string name)
        {
            DBConnection dBConnection = new DBConnection();
            dBConnection.Open();

            var reader = dBConnection.RunCommand($"SELECT `ID` FROM `Users` WHERE `Username` = '{name}';");

            while (reader.Read())
            {
                for (int i = 0; i < reader.GetColumnSchema().Count; i++)
                {
                    ID += Convert.ToInt32(reader.GetValue(i));
                }
            }
            dBConnection.Close();


            DBConnection dBConnection2 = new DBConnection();
            dBConnection2.Open();

            var reader2 = dBConnection2.RunCommand($"SELECT `isAdmin` FROM `Users` WHERE `Username` = '{name}';");

            int isAdminResult = 2;
            while (reader2.Read())
            {
                for (int i = 0; i < reader2.GetColumnSchema().Count; i++)
                {
                    isAdminResult = Convert.ToInt32(reader2.GetValue(i));
                }
            }
            dBConnection2.Close();

            switch (isAdminResult)
            {
                case 0: isAdmin = false; break;
                case 1: isAdmin = true; break;
                case 2: isAdmin = false; Console.WriteLine("The DB didnt respond, now defaulted to false."); break;
                default: isAdmin = false; Console.WriteLine("Couldnt determine your AdminStatus, now defaulted to false."); break;
            }

            DBConnection dBConnection3 = new DBConnection();
            dBConnection3.Open();

            var reader3 = dBConnection3.RunCommand($"SELECT `isAdmin` FROM `Users` WHERE `Username` = '{name}';");

            while (reader3.Read())
            {
                for (int i = 0; i < reader3.GetColumnSchema().Count; i++)
                {
                    Email = reader3.GetValue(i).ToString();
                }
            }
            dBConnection3.Close();
        }
    }
}
