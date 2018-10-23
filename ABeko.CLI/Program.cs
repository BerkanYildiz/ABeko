namespace ABeko.CLI
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using ABeko.CLI.Logic;
    using ABeko.CLI.Logic.Commands;
    using ABeko.Logic;
    using ABeko.Logic.Engines.Memory.Handlers;
    using ABeko.Logic.Handlers;

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
        /// <param name="Arguments">The CLI arguments.</param>
        private static async Task Main(string[] Arguments)
        {
            var EngineConfig    = new BekoConfig
            {
                Process         = Process.GetCurrentProcess(),
                MemoryHandler   = new NativeMemoryHandler(),
                RequestsHandler = new NativeRequestsHandler()
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
                if (Arguments.Length == 0)
                {
                    HelpCommand.Execute(null);
                }

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
