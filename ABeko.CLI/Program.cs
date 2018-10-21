namespace ABeko.CLI
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using ABeko.Logic;
    using ABeko.Logic.Engines.Memory.Handlers;
    using ABeko.Logic.Types;

    internal static class Program
    {
        /// <summary>
        /// Defines the entry point of this application.
        /// </summary>
        private static async Task Main()
        {
            var Engine          = (BekoEngine) null;
            var EngineConfig    = new BekoConfig
            {
                 Process        = Process.GetCurrentProcess(),
                 MemoryHandler  = new NativeMemoryHandler()
            };

            // ..

            try
            {
                Engine          = BekoEngine.FromConfiguration(EngineConfig);
            }
            catch (Exception Exception)
            {
                Console.WriteLine("[*] " + Exception.Message.Split('\n')[0]);
            }

            // ..

            if (Engine != null)
            {
                using (Engine)
                {
                    var Scanner = Engine.ScannerEngine;
                    var Memory  = Engine.MemoryEngine;
                    var Command = Console.ReadLine();

                    while (true)
                    {
                        switch (Command)
                        {

                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("[*] The engine failed to initialize.");
            }

            await Task.Delay(500);
        }
    }
}
