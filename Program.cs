using MySqlConnector;
using System.Security.Cryptography;
using System.Text;
    
namespace Hashing_Test
{
    public class Program
    {
        public static bool debugModeEnabled = true; //toggles debug functions, default is false

        static void Main(string[] args)
        {
            Menu.DisplayMenu();
        }

        public static void SelectAllFromUsers()
        {
            DBConnection dBConnection = new DBConnection();
            dBConnection.Open();

            var reader = dBConnection.RunCommand("Select * FROM Users");

            while (reader.Read())
            {
                string result = "";
                for (int i = 0; i < reader.GetColumnSchema().Count; i++)
                {
                    result += reader.GetValue(i).ToString() + "\t";
                }

                Console.WriteLine(result);
            }

            dBConnection.Close();
        }

        public static void InsertIntoUsers() 
        {
            Console.WriteLine("Add User\nName: ");
            string name = Console.ReadLine();

            Console.WriteLine("Email: ");
            string email = Console.ReadLine();

            Console.WriteLine("Password: ");
            string plainText = Console.ReadLine();
            byte[] hash = HashString(plainText, name, email);

            DBConnection dBConnection = new DBConnection();
            dBConnection.Open();

            var reader = dBConnection.RunCommand($"NSERT INTO `users` (`ID`, `Username`, `Email`, `Hash`) VALUES (NULL, '{name}', '{email}', '{hash}')");

            while (reader.Read())
            {
                string result = "";
                for (int i = 0; i < reader.GetColumnSchema().Count; i++)
                {
                    result += reader.GetValue(i).ToString() + "\t";
                }

                Console.WriteLine(result);
            }
        }

        static byte[] HashString(string plainText, string username, string email)
        {
            byte[] plainTextHash = HashingService.ToByte(plainText);
            byte[] saltHash = HashingService.ToByte(username, email);

            byte[] saltedHash = HashingService.GenerateSaltedHash(plainTextHash, saltHash);

            return saltedHash;
        }
    }

    public class DBConnection
    {
        MySqlConnection connection {  get; set; }

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
    }
}
