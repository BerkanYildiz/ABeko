namespace ABeko.CLI.Logic.Commands
{
    using System;

    internal static class ClearCommand
    {
        /// <summary>
        /// Executes the command with the specified parameters.
        /// </summary>
        /// <param name="Parameters">The parameters.</param>
        internal static void Execute(params string[] Parameters)
        {
            Console.Clear();
        }
    }
}
