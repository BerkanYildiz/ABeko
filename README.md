# ABeko

ABeko is a repository with libraries and CLI/GUI applications for reading memory, doing scans and dumping processes.

# CLI - Example

![ABeko.CLI](https://i.imgur.com/bmSYREJ.png)

# GUI - Example (WIP)

![ABeko.GUI](https://i.imgur.com/Nkgvcgd.png)

# Library - Example
```csharp
namespace ABeko.Tests
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
                Engine.Disposed += (Sender, Args) =>
                {
                    Console.WriteLine("[*] The engine has been disposed.");
                };

                using (Engine)
                {
                    var Scanner = Engine.ScannerEngine;
                    var Memory  = Engine.MemoryEngine;
                    var Address = 0xFFFFFFLu;

                    // Sig-scanning

                    Scanner.Signatures.Add(new Signature("UWorldSig", Signature: "00-00-00", new SignatureMask("XXX", AnythingMask: '*', SpecifiedMask: 'X')));

                    try
                    {
                        if (Scanner.TrySearchFor("UWorldSig", From: 0x0000007FFFFFFFFF, To: 0x0000007FFFFFFFFF + 32, out var Result))
                        {
                            Console.WriteLine("[*] Found the specified signature at 0x" + Result.Address.ToString("X").PadLeft(16, '0') + ".");
                        }
                        else
                        {
                            Console.WriteLine("[*] Failed to find the '" + Result.Signature.Name + "' signature.");
                        }
                    }
                    catch (Exception Exception)
                    {
                        Console.WriteLine("[*] " + Exception.GetType().Name + ", "  + Exception.Message);
                    }
                }
            }
            else
            {
                Console.WriteLine("[*] The engine failed to initialize.");
            }

            Console.ReadKey(false);
        }
    }
}

```
