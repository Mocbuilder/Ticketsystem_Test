using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hashing_Test
{
    public class Menu
    {
        public static void DisplayMenu()
        {
            List<string> options = new List<string>();
            options.Add("SELECT * FROM Users");
            options.Add("INSERT INTO `users` (`ID`, `Username`, `Email`, `Hash`) VALUES (NULL, '', '', '')");
            options.Add("Get Password of user");

            while (true)
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
                        Program.SelectAllFromUsers(); break;
                    case 2:
                        Program.InsertIntoUsers(); break;
                }
            }
        }
    }
}
