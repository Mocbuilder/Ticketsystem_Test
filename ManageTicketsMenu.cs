using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hashing_Test
{
    public class ManageTicketsMenu
    {
        static bool exit = false;
        public static void LoadMenu()
        {

            var menuOptions = new Dictionary<int, DBMenuOption> { };

            DBMenuOption myTickets = new DBMenuOption("My Tickets", ManageTickets);
            menuOptions.Add(1, myTickets);

            DBMenuOption deleteTicket = new DBMenuOption("Delete Ticket", DeleteTicket);
            menuOptions.Add(2, deleteTicket);

            DBMenuOption exitOption = new DBMenuOption("Exit", Exit);
            menuOptions.Add(3, exitOption);

            if (Program.currentSession.isAdmin == true)
            {
                DBMenuOption allTickets = new DBMenuOption("All Tickets", AllTickets);
                menuOptions.Add(4, allTickets);

                DBMenuOption deleteTicketAdmin = new DBMenuOption("Delete Ticket (Admin)", DeleteTicketAdmin);
                menuOptions.Add(5, deleteTicketAdmin);

                DBMenuOption makeComment = new DBMenuOption("Make Comment on Ticket", MakeComment);
                menuOptions.Add(6, makeComment);

                DBMenuOption appendComment = new DBMenuOption("Append a Comment on Ticket", AppendComment);
                menuOptions.Add(7, appendComment);

                DBMenuOption changeStatus = new DBMenuOption("Change Status of Ticket", ChangeStatus);
                menuOptions.Add(8, changeStatus);

                DBMenuOption archive = new DBMenuOption("Archive of removed Tickets", Archive);
                menuOptions.Add(9, archive);
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

        static void ManageTickets()
        {
            DBConnection dBConnection = new DBConnection();
            dBConnection.Open();

            Console.WriteLine("MANAGE TICKETS");

            var reader = dBConnection.RunCommand($"SELECT `Title`, `Description`, `Comment`, `Status` FROM `tickets` WHERE `Username` = '{Program.currentSession.Name}' AND `Status` != 3;");

            var table = new Table();

            table.AddColumn("Title");
            table.AddColumn("Description");
            table.AddColumn("Comment");
            table.AddColumn("Status");

            while (reader.Read())
            {
                AddRowWithSeparator(table, reader.GetValue(0).ToString(),
                    reader.GetValue(1).ToString(),
                    reader.GetValue(2).ToString(),
                    Program.IntToStatus((int)reader.GetValue(3)));

            }

            table.Border(TableBorder.Minimal);

            AnsiConsole.Write(table);

            dBConnection.Close();
        }

        public static void DeleteTicket()
        {
            DBConnection dBConnection = new DBConnection();
            dBConnection.Open();

            Console.WriteLine("DELETE TICKET\nTitle: ");
            string title = Console.ReadLine();

            var reader = dBConnection.RunCommand($"UPDATE `tickets` SET `Status` = 3 WHERE `Title` = '{title}' AND `Username` = '{Program.currentSession.Name}';");

            Console.WriteLine("Ticket deleted succesfully");

            dBConnection.Close();
        }

        static void Exit()
        {
            exit = true;
            Console.WriteLine("Logout complete");
            DBMenu.LoadMenu();
        }

        public static void AllTickets()
        {
            DBConnection dBConnection = new DBConnection();
            dBConnection.Open();

            Console.WriteLine("ALL TICKETS");

            var reader = dBConnection.RunCommand($"SELECT `Title`, `Description`, `Comment`, `Status` FROM `tickets` AND `Status` != 3;");

            var table = new Table();

            table.AddColumn("Title");
            table.AddColumn("Description");
            table.AddColumn("Comment");
            table.AddColumn("Status");

            while (reader.Read())
            {
                AddRowWithSeparator(table, reader.GetValue(0).ToString(),
                    reader.GetValue(1).ToString(),
                    reader.GetValue(2).ToString(),
                    Program.IntToStatus((int)reader.GetValue(3)));

            }

            table.Border(TableBorder.Minimal);

            AnsiConsole.Write(table);

            dBConnection.Close();
        }

        public static void DeleteTicketAdmin()
        {
            DBConnection dBConnection = new DBConnection();
            dBConnection.Open();

            Console.WriteLine("DELETE TICKET\nTitle: ");
            string title = Console.ReadLine();

            Console.WriteLine("IF YOU WANT TO ONLY REMOVE THE TICKET, PLEASE CHANGE IT'S STATUS!\nTHIS WILL PERMANENTLY REMOVE THE TICKET!\nConfirm permanent deletion (y/n): ");
            string confirm = Console.ReadLine().ToLower();

            if (confirm != "y" && confirm != "yes" || confirm == null)
            {
                Console.WriteLine("ACTION STOPPED\nTICKET WAS NOT DELETED, RETURNING...");
                Exit();
            }
            
            var reader = dBConnection.RunCommand($"DELETE FROM tickets WHERE `tickets`.`Title` = '{title}';");

            Console.WriteLine("Ticket deleted succesfully");

            dBConnection.Close();
        }

        public static void MakeComment()
        {
                DBConnection dBConnection = new DBConnection();
                dBConnection.Open();

                Console.WriteLine("MAKE COMMENT\nTitle: ");
                string title = Console.ReadLine();

                Console.WriteLine("New Comment (Will replace any previous comments): ");
                string comment = Console.ReadLine();

                var reader = dBConnection.RunCommand($"UPDATE `tickets` SET `Comment` = {comment} WHERE `Title` = '{title}';");

                Console.WriteLine("Comment updated succesfully");

                dBConnection.Close();
        }

        public static void AppendComment()
        {
            DBConnection dBConnection = new DBConnection();
            dBConnection.Open();

            Console.WriteLine("APPEND COMMENT\nTitle: ");
            string title = Console.ReadLine();

            Console.WriteLine("New Comment (Will be added to previous comments): ");
            string comment = Console.ReadLine();

            var reader = dBConnection.RunCommand($"UPDATE `tickets` SET `Comment` = CONCAT(`Comment`, ' ', '{comment}') WHERE `Title` = '{title}';");

            Console.WriteLine("Comment appended succesfully");

            dBConnection.Close();
        }

        public static void ChangeStatus()
        {
            DBConnection dBConnection = new DBConnection();
            dBConnection.Open();

            Console.WriteLine("CHANGE STATUS\nTitle: ");
            string title = Console.ReadLine();

            Console.WriteLine("New Status (Submitted = 0; WIP = 1; Closed = 2; Removed = 3): ");
            int newStatusInt = Convert.ToInt32(Console.ReadLine());

            var reader = dBConnection.RunCommand($"UPDATE `tickets` SET `Status` = {newStatusInt} WHERE `Title` = '{title}';");

            Console.WriteLine("Status updated succesfully");

            dBConnection.Close();
        }

        public static void Archive()
        {
            DBConnection dBConnection = new DBConnection();
            dBConnection.Open();

            Console.WriteLine("ARCHIVE OF REMOVED TICKETS");

            var reader = dBConnection.RunCommand($"SELECT `Title`, `Description`, `Comment`, `Status` FROM `tickets` AND `Status` = 3;");

            var table = new Table();

            table.AddColumn("Title");
            table.AddColumn("Description");
            table.AddColumn("Comment");
            table.AddColumn("Status");

            while (reader.Read())
            {
                AddRowWithSeparator(table, reader.GetValue(0).ToString(),
                    reader.GetValue(1).ToString(),
                    reader.GetValue(2).ToString(),
                    Program.IntToStatus((int)reader.GetValue(3)));

            }

            table.Border(TableBorder.Minimal);

            AnsiConsole.Write(table);

            dBConnection.Close();
        }

        public static void AddRowWithSeparator(Table table, string title, string description, string comment, string status)
        {
            table.AddRow(title, description, comment, status);

            table.AddEmptyRow();
        }

    }
}
