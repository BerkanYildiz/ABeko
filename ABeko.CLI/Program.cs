namespace ABeko.CLI
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using ABeko.Logic;

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
                 MemoryHandler  = null
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
                Console.WriteLine("[*] Engine initialized.");

                using (Engine)
                {
                    var Memory  = Engine.MemoryEngine;

                    if (Memory != null)
                    {
                        Console.WriteLine("[*] MemoryEngine initialized.");
                    }
                    else
                    {
                        Console.WriteLine("[*] MemoryEngine failed to initialize.");
                    }
                }

                Console.WriteLine("[*] Engine disposed.");
            }
            else
            {
                Console.WriteLine("[*] Engine failed to initialize.");
            }

            Console.ReadKey(false);
        }
    }
}
