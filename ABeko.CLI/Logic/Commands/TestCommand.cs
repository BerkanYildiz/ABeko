namespace ABeko.CLI.Logic.Commands
{
    using System;

    using ABeko.Logic;
    using ABeko.Logic.Native.Enums;

    internal static class TestCommand
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
            Console.WriteLine();

            var ScanEngine      = Engine.ScannerEngine;
            var MemoryEngine    = Engine.MemoryEngine;
            var RequestsHandler = Engine.Configuration.RequestsHandler;
            RequestsHandler.TryGetSystemInfo(out var SystemInfo);
            var MemoryRegions   = MemoryEngine.GetMemoryRegions(Region => Region.Protect == MemoryPagePermissions.PAGE_READWRITE && Region.State == MemoryPageState.MEM_COMMIT);

            Console.WriteLine("[*] Page Size      : 0x" + SystemInfo.PageSize.ToString("X"));
            Console.WriteLine("[*] Processors     : " + SystemInfo.NumberOfProcessors + " threads");
            Console.WriteLine("[*] User-mode      : ");
            Console.WriteLine("[*] -> Minimum     : 0x" + SystemInfo.MinimumApplicationAddress.ToString("X").PadLeft(16, '0'));
            Console.WriteLine("[*] -> Maximum     : 0x" + SystemInfo.MaximumApplicationAddress.ToString("X").PadLeft(16, '0'));
            Console.WriteLine("[*] Architecture   : " + SystemInfo.ProcessorArchitecture);
            Console.WriteLine("[*] IntPtr Size    : " + IntPtr.Size + " bytes");
            Console.WriteLine("[*] Memory Regions : " + MemoryRegions.Count);

            Console.WriteLine();
        }
    }
}
