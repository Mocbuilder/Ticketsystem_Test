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
using Spectre.Console.Rendering;

namespace Hashing_Test
{
    public class DBMenu
    {
        static bool exit = false;
        public static void LoadMenu()
        {

            var menuOptions = new Dictionary<int, DBMenuOption> { };

            DBMenuOption newTicket = new DBMenuOption("New Ticket", SubmitNewTicket);
            menuOptions.Add(1, newTicket);

            DBMenuOption manageTickets = new DBMenuOption("Manage Tickets", ManageTicketsMenu.LoadMenu);
            menuOptions.Add(2, manageTickets);

            DBMenuOption changeEmail = new DBMenuOption("Change my Email", ChangeEmail);
            menuOptions.Add(3, changeEmail);

            DBMenuOption changePassword = new DBMenuOption("Change my Password", ChangePassword);
            menuOptions.Add(4, changePassword);

            DBMenuOption exitOption = new DBMenuOption("Exit", Exit);
            menuOptions.Add(5, exitOption);

            if (Program.currentSession.isAdmin == true)
            {
                DBMenuOption newUser = new DBMenuOption("New User", UsersInsertNew);
                menuOptions.Add(6, newUser);

                DBMenuOption checkAdminStatus = new DBMenuOption("Check AdminStatus of User", CheckAdminOfUser);
                menuOptions.Add(7, checkAdminStatus);

                DBMenuOption changeAdminStatus = new DBMenuOption("Change AdminStatus of User", ChangeAdminOfUser);
                menuOptions.Add(8, changeAdminStatus);

                DBMenuOption changeName = new DBMenuOption("Change Name of User", ChangeName);
                menuOptions.Add(9, changeName);

                DBMenuOption changeEmailAdmin = new DBMenuOption("Change Email of User", Exit);
                menuOptions.Add(10, changeEmailAdmin);

                DBMenuOption deleteUser = new DBMenuOption("Delete User", Exit);
                menuOptions.Add(11, deleteUser);
            }



            while (!exit)
            {
                foreach (int index in menuOptions.Keys)
                {
                    Console.WriteLine(Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);
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

        public static void ChangeEmail()
        {
            Console.WriteLine("CHANGE EMAIL\nNew Email: ");
            string newEmail = Console.ReadLine();

            DBConnection dBConnection2 = new DBConnection();
            dBConnection2.Open();

            dBConnection2.RunCommand($"UPDATE Users SET Email = '{newEmail}' WHERE Username = '{Program.currentSession.Name}';");

            Console.WriteLine($"Succesfully changed {Program.currentSession.Email} to {newEmail} for User {Program.currentSession.Name}");

            dBConnection2.Close();
        }

        public static void ChangePassword()
        {
            DBConnection dBConnection = new DBConnection();
            dBConnection.Open();

            Console.WriteLine("CHANGE Password\nNew Password: ");
            string plainText = Console.ReadLine();
            byte[] hash = Program.HashString(plainText, Program.currentSession.Name, Program.currentSession.Email);

            DBConnection dBConnection2 = new DBConnection();
            dBConnection2.Open();

            dBConnection2.RunCommand($"UPDATE Users SET Password = @data WHERE Username = '{Program.currentSession.Name}';", Hash);

            Console.WriteLine($"Succesfully changed password for User {Program.currentSession.Name}");

            dBConnection2.Close();
        }
        static void Exit()
        {
            Console.WriteLine("Logout complete");
            exit = true;
            Menu.DisplayMenu();
        }

        public static void UsersInsertNew()
        {
            Console.WriteLine("NEW USER\nName: ");
            string name = Console.ReadLine();

            Console.WriteLine("Email: ");
            string email = Console.ReadLine();

            Console.WriteLine("Password: ");
            string plainText = Console.ReadLine();
            byte[] hash = Program.HashString(plainText, name, email);

            DBConnection dBConnection = new DBConnection();
            dBConnection.Open();

            dBConnection.RunCommandArray($"INSERT INTO `users` (`ID`, `Username`, `Email`, `Hash`) VALUES (NULL, '{name}', '{email}', @data)", hash);

            dBConnection.Close();
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

        public static void ChangeName()
        {
            DBConnection dBConnection = new DBConnection();
            dBConnection.Open();

            Console.WriteLine("CHANGE USERNAME\nName: ");
            string name = Console.ReadLine();

            Console.WriteLine("Change to:");
            string changeTo = Console.ReadLine();
            
            var reader = dBConnection.RunCommand($"UPDATE `Users` SET `Username` = '{changeTo}' WHERE `Username` = '{name}';");

            dBConnection.Close();
        }

        public static void ChangeEmailAdmin()
        {
            DBConnection dBConnection = new DBConnection();
            dBConnection.Open();

            Console.WriteLine("CHANGE EMAIL\nName: ");
            string name = Console.ReadLine();

            Console.WriteLine("Change EMAIL to:");
            string changeTo = Console.ReadLine();

            var reader = dBConnection.RunCommand($"UPDATE `Users` SET `Email` = '{changeTo}' WHERE `Username` = '{name}';");

            dBConnection.Close();
        }

        public static void DeleteUser()
        {
            DBConnection dBConnection = new DBConnection();
            dBConnection.Open();

            Console.WriteLine("DELETE USER\nName: ");
            string name = Console.ReadLine();

            Console.WriteLine("THIS WILL PERMANENTLY REMOVE THE USER!\nConfirm permanent deletion (y/n): ");
            string confirm = Console.ReadLine().ToLower();

            if (confirm != "y" && confirm != "yes" || confirm == null)
            {
                Console.WriteLine("ACTION STOPPED\nUSER WAS NOT DELETED, RETURNING...");
                Exit();
            }

            var reader = dBConnection.RunCommand($"DELETE FROM users WHERE `Users`.`Username` = '{name}';");

            Console.WriteLine("User deleted succesfully");

            dBConnection.Close();
        }
    }
}
