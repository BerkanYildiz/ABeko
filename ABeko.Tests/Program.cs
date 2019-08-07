namespace ABeko.Tests
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using ABeko.Logic;
    using ABeko.Logic.Engines.Memory.Handlers;
    using ABeko.Logic.Handlers;
    using ABeko.Logic.Native.Enums;
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
                Process         = Process.GetProcessById(16800),
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
                Engine.Disposed += (Sender, Args) =>
                {
                    Console.WriteLine("[*] The engine has been disposed.");
                };

                using (Engine)
                {
                    var Scanner = Engine.ScannerEngine;
                    var Memory  = Engine.MemoryEngine;

                    // Sig-scanning

                    var UWorldSig = new Signature("UWorldSig", Signature: "48-8B-1D-00-00-00-00-48-85-DB-74-3B-41", new SignatureMask("XXX????XXXXXX", AnythingMask: '?', SpecifiedMask: 'X'));
                    Scanner.Signatures.Add(UWorldSig);

                    try
                    {
                        var Regions = Memory.GetMemoryRegions(Region => Region.State == MemoryPageState.MEM_COMMIT);

                        if (Scanner.TrySearchFor(UWorldSig, Regions, out var Result))
                        {
                            if (!Result.IsErrored)
                            {
                                if (Result.IsFound)
                                {
                                    Console.WriteLine("[*] Found the specified signature at 0x" + Result.Address.ToString("X").PadLeft(16, '0') + ".");
                                }
                                else
                                {
                                    Console.WriteLine("[*] Failed to find the '" + Result.Signature.Name + "' signature.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("[*] Sig-scan failed and is errored.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("[*] Failed to sigscan with unknown error.");
                        }
                    }
                    catch (Exception Exception)
                    {
                        Console.WriteLine("[*] " + Exception.GetType().Name + ", "  + Exception.Message);
                    }

                    Console.ReadKey(false);
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
