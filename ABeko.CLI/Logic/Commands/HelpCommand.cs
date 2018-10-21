namespace ABeko.CLI.Logic.Commands
{
    using System;
    using System.Reflection;

    internal static class HelpCommand
    {
        /// <summary>
        /// Executes the command with the specified parameters.
        /// </summary>
        /// <param name="Parameters">The parameters.</param>
        internal static void Execute(params string[] Parameters)
        {
            Console.WriteLine("[*] Usage : " + Assembly.GetExecutingAssembly().GetName().Name + " <flags>");
            Console.WriteLine("[*] ");
            Console.WriteLine("[*] Available commands :");
            Console.WriteLine("[*] ");
            Console.WriteLine("[*] -> help       " + " |   " + "Shows this CLI documentation, and available commands.");
            Console.WriteLine("[*] -> clear      " + " |   " + "Clears the current console output.");
            Console.WriteLine("[*] -> open       " + " |   " + "Opens the specified process. (If no flag specified, opens current process)");
            Console.WriteLine("[*]    -> -pid    " + " |   " + "(flag) PID of the process to open.");
            Console.WriteLine("[*]    -> -name   " + " |   " + "(flag) Name of the process to open (without the extension) .");
            Console.WriteLine("[*] -> scan       " + " |   " + "Executes a signature scan on the currently selected process.");
            Console.WriteLine("[*] -> dump       " + " |   " + "Dumps the virtual memory of the currently selected process.");
            Console.WriteLine("[*] -> close      " + " |   " + "Closes the currently opened and selected process.");
            Console.WriteLine("");
        }
    }
}
