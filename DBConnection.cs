using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;

namespace Hashing_Test
{
    public class DBConnection
    {
        MySqlConnection connection { get; set; }

        public void Open()
        {
            connection = new MySqlConnection("Server=localhost;User ID=root;Password=;Database=ticketsystem_test");

            connection.Open();
        }

        public void Close()
        {
            connection.Close();
        }

        public MySqlDataReader RunCommand(string query)
        {
            var command = new MySqlCommand(query, connection);
            return command.ExecuteReader();
        }

        public void RunCommand(string query, byte[] parameter)
        {
            using (var command = new MySqlCommand(query, connection))
            {

                command.Parameters.Add($"@data", MySqlDbType.Blob).Value = parameter;

                command.ExecuteNonQuery();
            }
        }
    }
}
