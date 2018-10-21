namespace ABeko.CLI.Logic.Commands
{
    using System;
    using System.Diagnostics;
    using System.Linq;

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

        }
    }
}
