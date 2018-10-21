namespace ABeko.CLI.Logic.Commands
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    using ABeko.Logic;

    internal static class OpenCommand
    {
        /// <summary>
        /// Gets the engine.
        /// </summary>
        private static BekoEngine Engine
        {
            get
            {
                return Program.Engine;
            }
        }

        /// <summary>
        /// Executes the command with the specified parameters.
        /// </summary>
        /// <param name="Parameters">The parameters.</param>
        internal static void Execute(params string[] Parameters)
        {
            Process Process = null;

            if (Parameters.Length == 2)
            {
                if (Parameters[0] == "-pid")
                {
                    try
                    {
                        Process = Process.GetProcessById(int.Parse(Parameters[1]));
                    }
                    catch (ArgumentException)
                    {
                        Console.WriteLine("[*] Couldn't find process with the specified PID.");
                    }
                }
                else if (Parameters[0] == "-name")
                {
                    Process = Process.GetProcessesByName(Parameters[1]).FirstOrDefault();

                    if (Process == null)
                    {
                        Console.WriteLine("[*] Couldn't find process with the specified name.");
                    }
                }
            }
            else if (Parameters.Length == 0)
            {
                Process = Process.GetCurrentProcess();
            }
            else
            {
                Console.WriteLine("[*] Arguments passed to the open command are invalid.");
            }

            if (Process != null)
            {
                Console.WriteLine("[*] Process '" + Process.ProcessName + "', PID " + Process.Id + " has been selected.");

                if (OpenCommand.Engine.Process != Process)
                {
                    OpenCommand.Engine.SetProcess(Process);
                }
            }
        }
    }
}
