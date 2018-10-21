namespace ABeko.CLI.Logic.Commands
{
    using System;

    using ABeko.Logic;

    internal static class CloseCommand
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
            if (CloseCommand.Engine.Configuration.Process != null)
            {
                CloseCommand.Engine.RemoveProcess();

                if (CloseCommand.Engine.Configuration.Process == null)
                {
                    Console.WriteLine("[*] Process has been closed.");
                }
                else
                {
                    Console.WriteLine("[*] Failed to close the process.");
                }
            }
            else
            {
                Console.WriteLine("[*] No processes are opened, to be closed.");
            }
        }
    }
}
