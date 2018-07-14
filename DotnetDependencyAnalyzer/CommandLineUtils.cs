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
    }
}
