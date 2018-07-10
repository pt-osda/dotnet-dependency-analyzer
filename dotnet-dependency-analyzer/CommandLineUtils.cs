using System;

namespace DotnetDependencyAnalyzer.NetCore
{
    public class CommandLineUtils
    {
        public static void PrintSuccessMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("\n" + message);
            Console.ResetColor();
        }

        public static void PrintInfoMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("\n" + message);
            Console.ResetColor();
        }

        public static void PrintWarningMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\n" + message);
            Console.ResetColor();
        }

        public static void PrintErrorMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("\n" + message);
            Console.ResetColor();
        }

        public static void PrintLogo()
        {
            string logo = @"
       ____          _                 _     ____                                 _                                _                   _                        
      |  _ \   ___  | |_  _ __    ___ | |_  |  _ \   ___  _ __    ___  _ __    __| |  ___  _ __    ___  _   _     / \    _ __    __ _ | | _   _  ____ ___  _ __ 
      | | | | / _ \ | __|| '_ \  / _ \| __| | | | | / _ \| '_ \  / _ \| '_ \  / _` | / _ \| '_ \  / __|| | | |   / _ \  | '_ \  / _` || || | | ||_  // _ \| '__|
      | |_| || (_) || |_ | | | ||  __/| |_  | |_| ||  __/| |_) ||  __/| | | || (_| ||  __/| | | || (__ | |_| |  / ___ \ | | | || (_| || || |_| | / /|  __/| |   
      |____/  \___/  \__||_| |_| \___| \__| |____/  \___|| .__/  \___||_| |_| \__,_| \___||_| |_| \___| \__, | /_/   \_\|_| |_| \__,_||_| \__, |/___|\___||_|   
                                                       |_|                                            |___/                             |___/                 
            ";
            Console.WriteLine(logo);
        }

        public static void AppendSuccessMessage(int cursorLeft, int cursorTop, string message)
        {
            int currCursorLeft = Console.CursorLeft, currCursorTop = Console.CursorTop;
            Console.SetCursorPosition(cursorLeft, cursorTop);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(message);
            Console.ResetColor();
            Console.SetCursorPosition(currCursorLeft, currCursorTop);
        }
    }
}
