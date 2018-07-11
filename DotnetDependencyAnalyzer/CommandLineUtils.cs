using System;

namespace DotnetDependencyAnalyzer
{
    /// <summary>
    /// Command Line helper methods.
    /// </summary>
    public class CommandLineUtils
    {
        /// <summary>
        /// Prints a success message in command line.
        /// </summary>
        /// <param name="message">Message to be shown.</param>
        public static void PrintSuccessMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("\n" + message);
            Console.ResetColor();
        }

        /// <summary>
        /// Prints an info message in command line.
        /// </summary>
        /// <param name="message">Message to be shown.</param>
        public static void PrintInfoMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("\n" + message);
            Console.ResetColor();
        }

        /// <summary>
        /// Prints a warning message in command line.
        /// </summary>
        /// <param name="message">Message to be shown.</param>
        public static void PrintWarningMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\n" + message);
            Console.ResetColor();
        }

        /// <summary>
        /// Prints an error message in command line.
        /// </summary>
        /// <param name="message">Message to be shown.</param>
        public static void PrintErrorMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("\n" + message);
            Console.ResetColor();
        }

        /// <summary>
        /// Prints the logo of the application.
        /// </summary>
        public static void PrintLogo()
        {
            string logo = @"

  ____                            _                            _                _                    
 |  _ \  ___ _ __   ___ _ __   __| | ___ _ __   ___ _   _     / \   _ __   __ _| |_   _ _______ _ __ 
 | | | |/ _ | '_ \ / _ | '_ \ / _` |/ _ | '_ \ / __| | | |   / _ \ | '_ \ / _` | | | | |_  / _ | '__|
 | |_| |  __| |_) |  __| | | | (_| |  __| | | | (__| |_| |  / ___ \| | | | (_| | | |_| |/ |  __| |   
 |____/ \___| .__/ \___|_| |_|\__,_|\___|_| |_|\___|\__, | /_/   \_|_| |_|\__,_|_|\__, /___\___|_|   
            |_|                                     |___/                         |___/              

 
";
            Console.WriteLine(logo);
        }

        /// <summary>
        /// Writes text in a speficied location on the console.
        /// </summary>
        /// <param name="cursorLeft">Line number to write the message.</param>
        /// <param name="cursorTop">Column number to write the message.</param>
        /// <param name="message">Message to be shown.</param>
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
