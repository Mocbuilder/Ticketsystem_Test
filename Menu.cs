using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Hashing_Test
{
    public class Menu
    {
        public static bool isEnabled = true;
        public static void DisplayMenu()
        {
            List<string> options = new List<string>();
            options.Add("SELECT * FROM Users");
            options.Add("INSERT INTO `users` (`ID`, `Username`, `Email`, `Hash`) VALUES (NULL, '', '', '')");
            options.Add("Get Password of user");
            options.Add("Exit");

            while (isEnabled)
            {
                int i = 1;
                foreach (string item in options)
                {
                    Console.WriteLine(i + ". " + item);
                    i++;
                }

                Console.WriteLine("Choose Option: ");
                string input = Console.ReadLine();

                switch (Convert.ToInt32(input))
                {
                    case 1:
                        Program.UsersSelectAll(); break;
                    case 2:
                        Program.UsersInsertNew(); break;
                    case 3:
                        Program.UsersConfirmPassword(); break;
                    case 4:
                        Environment.Exit(0); break;
                }
            }
        }
    }
}
