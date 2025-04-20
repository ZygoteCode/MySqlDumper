namespace MySqlDumper
{
    public class Program
    {
        public static void Main()
        {
            Console.Title = "MySqlDumper V1 | Made by https://github.com/GabryB03/";
            Logger.ShowInfo("Welcome to MySqlDumper V1, made by https://github.com/GabryB03/");

            string ip = Logger.RequestInfo("Please, insert the server host / IP address");
            string database = Logger.RequestInfo("Please, insert the database name to dump");
            string user = Logger.RequestInfo("Please, insert the username (you can leave this empty if no user is allowed)");
            string password = Logger.RequestInfo("Please, insert the password (you can leave this empty if no password is allowed)");
            string port = Logger.RequestInfo("Please, insert the port (you can leave this empty for default 3306 port)");

            if (port == "")
            {
                port = "3306";
            }

            if (!Logger.RequestConfirmation("Are you sure you want to dump the database? The dump will be saved in the \"database.sql\" file."))
            {
                Logger.ShowWarning("You refused to dump the database. Please, press ENTER to exit from the program.");
                Console.ReadLine();
                return;
            }

            Logger.ShowWarning("Dumping the database, please wait a while.");

            try
            {
                SqlDumper sqlDumper = new SqlDumper(ip, database, user, password, port);
                sqlDumper.DumpDatabase("database.sql");
                Logger.ShowSuccess("The database has been succesfully dumped! Press ENTER to exit from the program.");
            }
            catch (Exception ex)
            {
                string completeReport = "\r\nException Message: " + ex.Message + "\r\nException Source: " + ex.Source + "\r\nException StackTrace: " + ex.StackTrace + "\r\nException Method: " + ex.TargetSite.Name + "\r\nException Class: " + ex.TargetSite.DeclaringType.Name;
                Logger.ShowError($"An error occured while trying to dump the database. Please, report those info in GitHub opening an issue:\r\n\r\n{completeReport}");
            }

            Console.ReadLine();
        }
    }
}