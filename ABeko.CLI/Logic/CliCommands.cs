namespace ABeko.CLI.Logic
{
    using System;
    using System.Collections.Generic;

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
            CliCommands.AvailableCommands.Add("scan", ScanCommand.Execute);
        }

        /// <summary>
        /// Tries to execute the specified command.
        /// </summary>
        /// <param name="Parameters">The parameters.</param>
        /// <returns></returns>
        static bool TryExecute(params string[] Parameters)
        {

        }
    }
}
