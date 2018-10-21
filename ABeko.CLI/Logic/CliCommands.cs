namespace ABeko.CLI.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ABeko.CLI.Logic.Commands;

    internal static class CliCommands
    {
        /// <summary>
        /// Gets the available CLI commands.
        /// </summary>
        private static Dictionary<string, Action<string[]>> AvailableCommands
        {
            get;
        }

        /// <summary>
        /// Initializes the <see cref="CliCommands"/> class.
        /// </summary>
        static CliCommands()
        {
            CliCommands.AvailableCommands = new Dictionary<string, Action<string[]>>();
            CliCommands.AvailableCommands.Add("help", HelpCommand.Execute);
            CliCommands.AvailableCommands.Add("clear", ClearCommand.Execute);
            CliCommands.AvailableCommands.Add("open", OpenCommand.Execute);
            CliCommands.AvailableCommands.Add("close", CloseCommand.Execute);
            CliCommands.AvailableCommands.Add("scan", ScanCommand.Execute);
        }

        /// <summary>
        /// Tries to execute the specified command.
        /// </summary>
        /// <param name="Parameters">The parameters.</param>
        internal static bool TryExecute(params string[] Parameters)
        {
            if (Parameters == null || Parameters.Length == 0)
            {
                throw new ArgumentNullException(nameof(Parameters));
            }

            var Command = Parameters[0];

            if (CliCommands.AvailableCommands.ContainsKey(Command))
            {
                CliCommands.AvailableCommands[Command].Invoke(Parameters.Skip(1).ToArray());
                return true;
            }
            else
            {
                Console.WriteLine("[*] Specified command does not exist.");
            }

            return false;
        }
    }
}
