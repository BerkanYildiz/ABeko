namespace ABeko.CLI.Logic.Commands
{
    using System;
    using System.Diagnostics;

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
                    var Offset    = 0;
                    var Processes = Process.GetProcessesByName(Parameters[1]);

                    if (Processes.Length == 0)
                    {
                        Console.WriteLine("[*] Couldn't find process with the specified name.");
                    }

                    if (Processes.Length == 1)
                    {
                        Process = Processes[0];
                    }

                    if (Processes.Length > 1)
                    {
                        Console.WriteLine("[*] More than one process has the specified name, please choose the correct one : ");

                        foreach (var Proc in Processes)
                        {
                            Console.WriteLine("[*] - Type " + Offset++ + " for process with PID " + Proc.Id.ToString().PadRight(5) + " - " + Proc.ProcessName + " | " + Proc.MainWindowTitle);
                        }

                        Console.Write("[*] > ");

                        if (int.TryParse(Console.ReadLine(), out var SelectedOffset))
                        {
                            if (SelectedOffset >= 0 && SelectedOffset < Processes.Length)
                            {
                                Process = Processes[SelectedOffset];
                            }
                            else
                            {
                                Console.WriteLine("[*] Input is invalid, index is out of range.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("[*] Input not considered as a valid number, aborting.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("[*] Invalid flags, the available are :");
                    Console.WriteLine("[*] ");
                    Console.WriteLine("[*] Flags : ");
                    Console.WriteLine("[*] -pid <PID>   |   Opens process with the specified PID.");
                    Console.WriteLine("[*] -name <Name> |   Opens process with the specified name.");
                    Console.WriteLine("[*] ");
                    Console.WriteLine("[*] Examples : ");
                    Console.WriteLine("[*] > open -pid 16752");
                    Console.WriteLine("[*] > open -name DiscordPtb");
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

                if (OpenCommand.Engine.Configuration.Process != Process)
                {
                    OpenCommand.Engine.SetProcess(Process);
                }
                else
                {
                    Console.WriteLine("[*] Selected process is already open.");
                }
            }
        }
    }
}
