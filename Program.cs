using MySqlConnector;
using System.Formats.Tar;
using System.Security.Cryptography;
using System.Text;
    
namespace Hashing_Test
{
    public class Program
    {
        public static bool debugModeEnabled = false; //toggles debug functions, default is false
        public static Session currentSession = new Session("");
        static void Main(string[] args)
        {
            //if (debugModeEnabled) 
            //{
            //  Console.WriteLine("Debug Mode");
            //  TestHashString();
            //}
            Menu.DisplayMenu();
        }

        public static void UsersSelectAll()
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

        

        public static void UsersConfirmPassword()
        {
            DBConnection dBConnection = new DBConnection();
            dBConnection.Open();

            Console.WriteLine("LOGIN\nName: ");
            string name = Console.ReadLine();

            Console.WriteLine("Password: ");
            string plainText = Console.ReadLine();
            var reader = dBConnection.RunCommand($"SELECT `Email` FROM `Users` WHERE `Username` = '{name}';");
            
            string emailResult = "";
            while (reader.Read())
            {
                for (int i = 0; i < reader.GetColumnSchema().Count; i++)
                {
                    emailResult += reader.GetValue(i).ToString(); //+ "\t"
                }
            }
            dBConnection.Close();

            DBConnection dBConnection2 = new DBConnection();
            dBConnection2.Open();

            byte[] localHash = HashString(plainText, name, emailResult);

            var reader2 = dBConnection2.RunCommand($"SELECT `Hash` FROM `Users` WHERE `Username` = '{name}';");

            byte[] hashResult = null;

            while (reader2.Read())
            {
                hashResult = reader2.GetFieldValue<byte[]>(0);
            }

            if (HashingService.CompareByteArrays(localHash, hashResult) == true) 
            {
                currentSession.Name = name;
                currentSession.GetSessionValues(name);
                Console.WriteLine("Login Succesful!\nLoading Ticketsystem...");
                Menu.isEnabled = false;
                dBConnection.Close();
                DBMenu.LoadMenu();
                return;
            }
            else
            {
                Console.WriteLine("Login failed!\n Either your username or password were wrong, or your account is not registered with us.");
            }

            dBConnection2.Close();
        }

        public static byte[] HashString(string plainText, string username, string email)
        {
            byte[] plainTextHash = HashingService.ToByte(plainText);
            byte[] saltHash = HashingService.ToByte(username, email);

            byte[] saltedHash = HashingService.GenerateSaltedHash(plainTextHash, saltHash);

            return saltedHash;
        }

        static void TestHashString()
        {
            Console.WriteLine("Add User\nName: ");
            string name = Console.ReadLine();

            Console.WriteLine("Email: ");
            string email = Console.ReadLine();

            Console.WriteLine("Password: ");
            string plainText = Console.ReadLine();

            byte[] hash1 = HashString(plainText, name, email);
            byte[] hash2 = HashString(plainText, name, email);

            if(HashingService.CompareByteArrays(hash1, hash2) == true)
            {
                Console.WriteLine("Its ok");
            }
            else
            {
                Console.WriteLine("Its not ok");
            }
        }

        public static string IntToStatus(int toAdmin)
        {
            switch (toAdmin)
            {
                case 0: return "Submitted";
                case 1: return "Work in progress";
                case 2: return "Closed";
                case 3: return "Removed";
                default: Console.WriteLine("WorkStatus value is corrupted. Now defaulted to Submitted"); return "Submitted";
            }
        }
    }
}
