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
            if (debugModeEnabled) 
            {
                Console.WriteLine("Debug Mode");
                TestHashString();
            }
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

        public static void UsersInsertNew() 
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

            dBConnection.RunCommand($"INSERT INTO `users` (`ID`, `Username`, `Email`, `Hash`) VALUES (NULL, '{name}', '{email}', @data)", hash);

            dBConnection.Close();
        }

        public static void UsersConfirmPassword()
        {
            DBConnection dBConnection = new DBConnection();
            dBConnection.Open();

            Console.WriteLine("Get Password\nName: ");
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

                if (debugModeEnabled == true)
                {
                    Console.WriteLine(emailResult);
                }
            }
            dBConnection.Close();

            DBConnection dBConnection2 = new DBConnection();
            dBConnection2.Open();

            byte[] localHash = HashString(plainText, name, emailResult);

            reader = dBConnection2.RunCommand($"SELECT `Hash` FROM `Users` WHERE `Username` = '{name}';");

            byte[] hashResult = null;

            while (reader.Read())
            {
                hashResult = reader.GetFieldValue<byte[]>(0);
            }

            if (HashingService.CompareByteArrays(localHash, hashResult) == true) 
            {
                Console.WriteLine("Login Succesful!\nLoading Ticketsystem...");
                Menu.isEnabled = false;
                dBConnection.Close();
                //TicketsystemMenu.Display();
                return;
            }
            else
            {
                Console.WriteLine("Login failed!\n Either your username or password were wrong, or your account is not registered with us.");
            }

            dBConnection.Close();
        }

        static byte[] HashString(string plainText, string username, string email)
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
    }
}
