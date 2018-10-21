namespace ABeko.CLI
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using ABeko.CLI.Logic;
    using ABeko.Logic;
    using ABeko.Logic.Engines.Memory.Handlers;

    internal static class Program
    {
        /// <summary>
        /// Gets the core engine.
        /// </summary>
        internal static BekoEngine Engine
        {
            get;
            private set;
        }

        /// <summary>
        /// Defines the entry point of this application.
        /// </summary>
        private static async Task Main()
        {
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
                Console.Write("[*] > ");

                using (Engine)
                {
                    while (true)
                    {
                        var Command = Console.ReadLine();

                        if (!string.IsNullOrEmpty(Command))
                        {
                            if (Command == "dispose" || Command == "exit" || Command == "quit")
                            {
                                break;
                            }

                            CliCommands.TryExecute(Command.Split(' '));
                        }

                        Console.Write("[*] > ");
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
