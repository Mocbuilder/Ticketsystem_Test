using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Hashing_Test
{
    public class DBMenu
    {
        public static bool isEnabled = true;

        public static void LoadMenu()
        {
            // Dictionary to hold menu options and corresponding actions
            var menuOptions = new Dictionary<string, Action> { };

            menuOptions.Add("1. New Ticket", UserActions.SubmitNewTicket);
            menuOptions.Add("2. Manage Tickets", Exit);
            menuOptions.Add("3. Change my Password", Exit);
            menuOptions.Add("4. Change my Email", Exit);
            if (Program.currentSession.isAdmin == true)
            {
                menuOptions.Add("5. New User", Exit);
                menuOptions.Add("6. Check AdminStatus of User", CheckAdminOfUser);
                menuOptions.Add("7. Change AdminStatus of User", ChangeAdminOfUser);
                menuOptions.Add("8. Change Name of User", Exit);
                menuOptions.Add("9. Change Email of User", Exit);
                menuOptions.Add("10. Delete User", Exit);
                menuOptions.Add("11. Manage Tickets (Admin)", Exit);
            }
            menuOptions.Add("12. Exit", Exit);

            bool exit = false;

            while (!exit)
            {
                foreach (string name in menuOptions.Keys)
                {
                    Console.WriteLine(name);
                }

                Console.Write("Select an option: ");
                string choice = Console.ReadLine();

                if (menuOptions.ContainsKey(choice))
                {
                    menuOptions[choice].Invoke();
                    if (choice == "4")
                    {
                        exit = true;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid option, please try again.");
                }
            }

            Console.WriteLine("Goodbye!");
        }

        static void ChangeAdminOfUser()
        {
            DBConnection dBConnection = new DBConnection();
            dBConnection.Open();

            Console.WriteLine("Change AdminStatus\nName: ");
            string name = Console.ReadLine();

            Console.WriteLine("Change to:");
            string changeTo = Console.ReadLine();
            int newStatus;
            if (changeTo == "true")
            {
                newStatus = 1;
            }
            else if (changeTo == "false")
            {
                newStatus = 0;
            }
            else
            {
                Console.WriteLine("Invalid Input, action stopped.");
                return;
            }

            var reader = dBConnection.RunCommand($"SELECT `isAdmin` FROM `Users` WHERE `Username` = '{name}';");

            int currentAdminStatus = 2;
            while (reader.Read())
            {
                for (int i = 0; i < reader.GetColumnSchema().Count; i++)
                {
                    currentAdminStatus += Convert.ToInt32(reader.GetValue(i));
                }

                if (Program.debugModeEnabled == true)
                {
                    switch (currentAdminStatus)
                    {
                        case 0: Console.WriteLine("Current AdminStatus: false"); break;
                        case 1: Console.WriteLine("Current AdminStatus: true"); break;
                        default: Console.WriteLine("Current AdminStatus: Couldnt determine."); break;
                    }
                }
            }
            dBConnection.Close();

            DBConnection dBConnection2 = new DBConnection();
            dBConnection2.Open();

            dBConnection2.RunCommand($"UPDATE Users SET isAdmin = {newStatus} WHERE Username = '{name}';");

            if (currentAdminStatus == 1)
            {
                Console.WriteLine($"Changed AdminStatus = true to AdminStatus = false for User {name}");
            }
            else if (currentAdminStatus == 0)
            {
                Console.WriteLine($"Changed AdminStatus = false to AdminStatus = true for User {name}");
            }
            dBConnection2.Close();
        }

        static void CheckAdminOfUser()
        {
            DBConnection dBConnection = new DBConnection();
            dBConnection.Open();

            Console.WriteLine("Check AdminStatus\nName: ");
            string name = Console.ReadLine();

            var reader = dBConnection.RunCommand($"SELECT `isAdmin` FROM `Users` WHERE `Username` = '{name}';");

            int currentAdminStatus = 2;
            while (reader.Read())
            {
                for (int i = 0; i < reader.GetColumnSchema().Count; i++)
                {
                    currentAdminStatus += Convert.ToInt32(reader.GetValue(i));
                }

                switch (currentAdminStatus)
                {
                    case 0: Console.WriteLine("Current AdminStatus: false"); break;
                    case 1: Console.WriteLine("Current AdminStatus: true"); break;
                    default: Console.WriteLine("Current AdminStatus: Couldnt determine."); break;
                }

            }
            dBConnection.Close();
        }

        static void Exit()
        {
            isEnabled = false;
            Console.WriteLine("Logout complete");
        }
    }
}
