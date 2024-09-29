namespace MySqlDumper
{
    public class Logger
    {
        public static bool RequestConfirmation(string message, bool arrow = true, bool newLine = false, bool showYN = true)
        {
            string answer = "";

            while (answer != "y" && answer != "n")
            {
                DisplayInConsole("REQUEST", ConsoleColor.DarkMagenta, message, false);

                if (showYN)
                {
                    Console.Write(" (y/n)");
                }

                if (arrow)
                {
                    Console.Write(" > ");
                }
                else if (newLine)
                {
                    Console.WriteLine();
                }

                answer = Console.ReadLine();

                if (answer != "y" && answer != "n")
                {
                    ShowError("The answer must be between \"y\" (yes) or \"n\" (no).");
                }
            }

            return answer == "y";
        }

        public static string RequestInfo(string message, bool arrow = true, bool newLine = false)
        {
            DisplayInConsole("REQUEST", ConsoleColor.DarkMagenta, message, false);

            if (arrow)
            {
                Console.Write(" > ");
            }
            else if (newLine)
            {
                Console.WriteLine();
            }

            return Console.ReadLine();
        }

        public static void ShowError(string message, bool newLine = true)
        {
            DisplayInConsole("ERROR", ConsoleColor.Red, message, newLine);
        }

        public static void ShowInfo(string message, bool newLine = true)
        {
            DisplayInConsole("INFO", ConsoleColor.Cyan, message, newLine);
        }

        public static void ShowWarning(string message, bool newLine = true)
        {
            DisplayInConsole("WARNING", ConsoleColor.Yellow, message, newLine);
        }

        public static void ShowSuccess(string message, bool newLine = true)
        {
            DisplayInConsole("SUCCESS", ConsoleColor.Green, message, newLine);
        }

        private static void DisplayInConsole(string caption, ConsoleColor consoleColor, string message, bool newLine)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("[");
            Console.ForegroundColor = consoleColor;
            Console.Write(caption);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("] ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(message);

            if (newLine)
            {
                Console.WriteLine();
            }
        }
    }
}