using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Hashing_Test
{
    public class DBMenu
    {
        public static bool isEnabled = true;

        public static void LoadMenu()
        {

            var menuOptions = new Dictionary<int, DBMenuOption> { };

            DBMenuOption newTicket = new DBMenuOption("New Ticket", SubmitNewTicket);
            menuOptions.Add(1, newTicket);

            DBMenuOption manageTickets = new DBMenuOption("Manage Tickets", ManageTickets);
            menuOptions.Add(2, manageTickets);

            DBMenuOption changePassword = new DBMenuOption("Change my Password", Exit);
            menuOptions.Add(3, changePassword);

            DBMenuOption changeEmail = new DBMenuOption("Change my Email", Exit);
            menuOptions.Add(4, changeEmail);

            DBMenuOption exitOption = new DBMenuOption("Exit", Exit);
            menuOptions.Add(5, exitOption);

            if (Program.currentSession.isAdmin == true)
            {
                DBMenuOption newUser = new DBMenuOption("New User", Exit);
                menuOptions.Add(6, newUser);

                DBMenuOption checkAdminStatus = new DBMenuOption("Check AdminStatus of User", CheckAdminOfUser);
                menuOptions.Add(7, checkAdminStatus);

                DBMenuOption changeAdminStatus = new DBMenuOption("Change AdminStatus of User", ChangeAdminOfUser);
                menuOptions.Add(8, changeAdminStatus);

                DBMenuOption changeName = new DBMenuOption("Change Name of User", Exit);
                menuOptions.Add(9, changeName);

                DBMenuOption changeEmailAdmin = new DBMenuOption("Change Email of User", Exit);
                menuOptions.Add(10, changeEmailAdmin);

                DBMenuOption deleteUser = new DBMenuOption("Delete User", Exit);
                menuOptions.Add(11, deleteUser);

                DBMenuOption manageTicketsAdmin = new DBMenuOption("Manage Tickets (Admin)", Exit);
                menuOptions.Add(12, manageTicketsAdmin);
            }

            bool exit = false;

            while (!exit)
            {
                foreach (int index in menuOptions.Keys)
                {
                    Console.WriteLine(index + "." + menuOptions[index].Name);
                }

                Console.Write("Select an option: ");
                int choice = Convert.ToInt32(Console.ReadLine());

                if (menuOptions.ContainsKey(choice))
                {
                    menuOptions[choice].Function.Invoke();
                    if (choice == 5)
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

            Console.WriteLine("CHANGE ADMINSTATUS\nName: ");
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
                Console.WriteLine($"Changed AdminStatus = true to AdminStatus = {changeTo} for User {name}");
            }
            else if (currentAdminStatus == 0)
            {
                Console.WriteLine($"Changed AdminStatus = false to AdminStatus = {changeTo} for User {name}");
            }
            dBConnection2.Close();
        }

        static void CheckAdminOfUser()
        {
            DBConnection dBConnection = new DBConnection();
            dBConnection.Open();

            Console.WriteLine("CHECK ADMINSTATUS\nName: ");
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

        static void SubmitNewTicket()
        {
            DBConnection dBConnection = new DBConnection();
            dBConnection.Open();

            Console.WriteLine("NEW TICKET\nTitle: ");
            string title = Console.ReadLine();

            Console.WriteLine("Description: ");
            string description = Console.ReadLine();

            DateTime today = DateTime.Now;
            string formattedDate = today.ToString("yyyy-MM-dd");

            dBConnection.RunCommandDate($"INSERT INTO `tickets` (`ID`, `Date`, `Username`, `Title`, `Description`, `Comment`, `Status`) VALUES (NULL, @data, '{Program.currentSession.Name}', '{title}', '{description}', NULL, 0)", formattedDate);

            Console.WriteLine("Ticket was submited");

            dBConnection.Close();
        }

        static void ManageTickets()
        {
            DBConnection dBConnection = new DBConnection();
            dBConnection.Open();

            Console.WriteLine("MANAGE TICKETS");

            var reader = dBConnection.RunCommand($"SELECT `Title`, `Description`, `Comment`, `Status` FROM `tickets` WHERE `Username` = '{Program.currentSession.Name}'");

            while (reader.Read())
            {
                string result = "";
                for (int i = 0; i < reader.GetColumnSchema().Count; i++)
                {
                    //maybe as class ? idk
                    result += reader.GetValue(i).ToString() + "\t";
                }

                Console.WriteLine(result);


                var table = new Table();

                // Add some columns
                table.AddColumn("Title");
                table.AddColumn("Description");
                table.AddColumn("Comment");
                table.AddColumn("Status");

                // Add some rows
                table.AddRow("Baz", "[green]Qux[/]");
                table.AddRow(new Markup("[blue]Corgi[/]"), new Panel("Waldo"));

                // Render the table to the console
                AnsiConsole.Write(table);
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
