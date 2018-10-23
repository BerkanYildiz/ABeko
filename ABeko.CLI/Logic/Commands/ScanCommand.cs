namespace ABeko.CLI.Logic.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Security.Policy;
    using System.Text;

    using ABeko.Logic;
    using ABeko.Logic.Engines.Scanner.Events;
    using ABeko.Logic.Native;
    using ABeko.Logic.Native.Enums;
    using ABeko.Logic.Types;

    internal static class ScanCommand
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

            var From      = 0Lu;
            var To        = 0Lu;
            var Align     = 0u;
            var Type      = "None";
            var Value     = (byte[]) null;
            var ValueS    = string.Empty;
            var Signature = (Signature) null;

            //   Usage  : <command> -from <address> -to <address> -type <value type> <value>
            // Examples : scan -from 0x400000 -to 0x500000 -type string MZ
            // Examples : scan -from 0x7ffcad880000 -to 0x7ffcad887000 -type string RichB
            // Examples : scan -all -type string RichB
            // Examples : scan -all -type float 56,43612

            if (Parameters.Length < 2)
            {
                Console.WriteLine("[*] Invalid number of arguments.");
            }
            else
            {
                ValueS = Parameters[Parameters.Length - 1];

                if (ValueS.EndsWith("\""))
                {
                    for (int i = 0; i < Parameters.Length - 1; i++)
                    {
                        if (Parameters[i].StartsWith("\""))
                        {
                            for (int X = i; X < Parameters.Length - 1; X++)
                            {
                                ValueS = Parameters[X] + ValueS;
                            }
                            
                            break;
                        }
                    }
                }

                if (ValueS.StartsWith("\"") && ValueS.EndsWith("\""))
                {
                    ValueS = ValueS.Substring(1);
                    ValueS = ValueS.Substring(0, ValueS.Length - 1);
                }

                for (int i = 0; i < Parameters.Length - 1; i++)
                {
                    var Flag      = Parameters[i];
                    var FlagValue = Parameters[i + 1];

                    switch (Flag)
                    {
                        case "-from" when FlagValue.StartsWith("0x"):
                        {
                            FlagValue = FlagValue.Replace("0x", "");

                            if (!ulong.TryParse(FlagValue, NumberStyles.HexNumber, new NumberFormatInfo(), out From))
                            {
                                Console.WriteLine("[*] The -from flag value is not a valid hexadecimal number.");
                            }

                            break;
                        }

                        case "-from":
                        {
                            if (!ulong.TryParse(FlagValue, out From))
                            {
                                Console.WriteLine("[*] The -from flag value is not a valid number.");
                            }

                            break;
                        }

                        case "-to" when FlagValue.StartsWith("0x"):
                        {
                            FlagValue = FlagValue.Replace("0x", "");

                            if (!ulong.TryParse(FlagValue, NumberStyles.HexNumber, new NumberFormatInfo(), out To))
                            {
                                Console.WriteLine("[*] The -to flag value is not a valid hexadecimal number.");
                            }

                            break;
                        }

                        case "-to":
                        {
                            if (!ulong.TryParse(FlagValue, out To))
                            {
                                Console.WriteLine("[*] The -to flag value is not a valid number.");
                            }

                            break;
                        }

                        case "-type":
                        {
                            Type = FlagValue;
                            break;
                        } 
                    }
                }

                if (From > 0 && To > 0)
                {
                    if (From >= To)
                    {
                        Console.WriteLine("[*] Invalid arguments, -from value is inferior or equal to -to value.");
                        return;
                    }

                    if (To > 0x7FFFFFFFFFFF)
                    {
                        Console.WriteLine("[*] Invalid arguments, -to value is superior to the maximum address for the user-space virtual memory.");
                        return;
                    }
                }

                if (Type == "None")
                {
                    Console.WriteLine("[*] Invalid arguments, the -type flag was not specified.");
                    return;
                }

                if (string.IsNullOrEmpty(Type))
                {
                    Console.WriteLine("[*] Invalid arguments, the -type flag value is empty.");
                    return;
                }

                switch (Type)
                {
                    case "byte" when ValueS.StartsWith("0x"):
                    {
                        Type  = "byte";
                        Align = 1;

                        if (byte.TryParse(ValueS.Replace("0x", ""), NumberStyles.HexNumber, new NumberFormatInfo(), out var Result))
                        {
                            Value = new byte[1] { Result };
                        }

                        break;
                    }

                    case "byte":
                    {
                        Type  = "byte";
                        Align = 1;

                        if (byte.TryParse(ValueS, out var Result))
                        {
                            Value = new byte[1] { Result };
                        }

                        break;
                    }

                    case "int" when ValueS.StartsWith("0x"):
                    case "integer" when ValueS.StartsWith("0x"):
                    {
                        Type  = "int";
                        Align = 4;

                        if (int.TryParse(ValueS.Replace("0x", ""), NumberStyles.HexNumber, new NumberFormatInfo(), out var Result))
                        {
                            Value = BitConverter.GetBytes(Result);
                        }

                        break;
                    }

                    case "int":
                    case "integer":
                    {
                        Type  = "int";
                        Align = 4;

                        if (int.TryParse(ValueS, out var Result))
                        {
                            Value = BitConverter.GetBytes(Result);
                        }

                        break;
                    }

                    case "uint" when ValueS.StartsWith("0x"):
                    case "unsignedint" when ValueS.StartsWith("0x"):
                    case "unsignedinteger" when ValueS.StartsWith("0x"):
                    {
                        Type  = "uint";
                        Align = 4;

                        if (uint.TryParse(ValueS.Replace("0x", ""), NumberStyles.HexNumber, new NumberFormatInfo(), out var Result))
                        {
                            Value = BitConverter.GetBytes(Result);
                        }

                        break;
                    }

                    case "uint":
                    case "unsignedint":
                    case "unsignedinteger":
                    {
                        Type  = "uint";
                        Align = 4;

                        if (uint.TryParse(ValueS, out var Result))
                        {
                            Value = BitConverter.GetBytes(Result);
                        }

                        break;
                    }

                    case "long" when ValueS.StartsWith("0x"):
                    {
                        Type  = "long";
                        Align = 8;

                        if (long.TryParse(ValueS.Replace("0x", ""), NumberStyles.HexNumber, new NumberFormatInfo(), out var Result))
                        {
                            Value = BitConverter.GetBytes(Result);
                        }

                        break;
                    }

                    case "long":
                    {
                        Type  = "long";
                        Align = 8;

                        if (long.TryParse(ValueS, out var Result))
                        {
                            Value = BitConverter.GetBytes(Result);
                        }

                        break;
                    }

                    case "ulong" when ValueS.StartsWith("0x"):
                    case "unsignedlong" when ValueS.StartsWith("0x"):
                    {
                        Type  = "ulong";
                        Align = 8;

                        if (ulong.TryParse(ValueS.Replace("0x", ""), NumberStyles.HexNumber, new NumberFormatInfo(), out var Result))
                        {
                            Value = BitConverter.GetBytes(Result);
                        }

                        break;
                    }

                    case "ulong":
                    case "unsignedlong":
                    {
                        Type  = "ulong";
                        Align = 8;

                        if (ulong.TryParse(ValueS, out var Result))
                        {
                            Value = BitConverter.GetBytes(Result);
                        }

                        break;
                    }

                    case "float"  when ValueS.StartsWith("0x"):
                    case "single" when ValueS.StartsWith("0x"):
                    {
                        Type  = "float";
                        Align = 4;

                        if (float.TryParse(ValueS.Replace("0x", ""), NumberStyles.HexNumber, new NumberFormatInfo(), out var Result))
                        {
                            Value = BitConverter.GetBytes(Result);
                        }

                        break;
                    }

                    case "float":
                    case "single":
                    {
                        Type  = "float";
                        Align = 4;

                        if (float.TryParse(ValueS, out var Result))
                        {
                            Value = BitConverter.GetBytes(Result);
                        }

                        break;
                    }

                    case "utf7":
                    case "utf-7":
                    {
                        Type  = "utf-7";

                        if (!string.IsNullOrEmpty(ValueS))
                        {
                            Value = Encoding.UTF7.GetBytes(ValueS);
                            Align = (uint) Value.Length;
                        }

                        break;
                    }

                    case "string":
                    case "utf8":
                    case "utf-8":
                    {
                        Type  = "string";

                        if (!string.IsNullOrEmpty(ValueS))
                        {
                            Value = Encoding.UTF8.GetBytes(ValueS);
                            Align = (uint) Value.Length;
                        }

                        break;
                    }

                    case "unicode":
                    case "utf16":
                    case "utf-16":
                    {
                        Type  = "unicode";

                        if (!string.IsNullOrEmpty(ValueS))
                        {
                            Value = Encoding.Unicode.GetBytes(ValueS);
                            Align = (uint) Value.Length;
                        }

                        break;
                    }

                    case "ascii":
                    {
                        Type  = "ascii";

                        if (!string.IsNullOrEmpty(ValueS))
                        {
                            Value = Encoding.ASCII.GetBytes(ValueS);
                            Align = (uint) Value.Length;
                        }

                        break;
                    }

                    case "custom":
                    case "bytes":
                    {
                        Type  = "custom";
                        Align = 0;

                        break;
                    }

                    default:
                    {
                        Type  = "None";
                        Align = 0;

                        Console.WriteLine("[*] Invalid arguments, the -type flag value is not supported.");
                        return;
                    }
                }

                if (Type == "custom")
                {
                    /// <summary>
                    /// Trims the specified hexadecimal string.
                    /// </summary>
                    /// <param name="Hex">The hexadecimal string.</param>
                    string TrimHex(string Hex)
                    {
                        return Hex.Replace("-", "").Replace("0x", "").Replace(",", "").Trim();
                    }

                    ValueS = TrimHex(ValueS);

                    if (!(string.IsNullOrEmpty(ValueS) || ValueS.Length % 2 != 0))
                    {
                        try
                        {
                            Signature = new Signature("CLI", ValueS, new SignatureMask(new string('X', ValueS.Length / 2)));
                        }
                        catch (Exception Exception)
                        {
                            Console.WriteLine("[*] Invalid arguments, failed to build the signature. (" + Exception.GetType().Name + ")");
                            Console.WriteLine("[*] -> " + Exception.Message);
                            Console.WriteLine("[*] -> " + ValueS);
                            Console.WriteLine("[*] -> " + ValueS.Length);
                        }

                        if (Signature == null)
                        {
                            return;
                        }

                        Align = Signature.Length;
                        Value = Signature.SigBytes;
                    }
                    else
                    {
                        // ..
                    }
                }

                if (Value == null)
                {
                    Console.WriteLine("[*] Invalid arguments, the given value is either invalid, malformatted, or not supported.");
                    return;
                }

                // Prints the infos about the scan..

                Console.WriteLine("[*] ------------------------------------------------- ");
                Console.WriteLine("[*] -> Scan      : " + Engine.Configuration.Process.ProcessName);

                if (From > 0 && To > 0)
                {
                    Console.WriteLine("[*] -> Range     : 0x" + From.ToString("X").PadLeft(16, '0') + " -> 0x" + To.ToString("X").PadLeft(16, '0'));
                }
                else
                {
                    Console.WriteLine("[*] -> Range     : Every memory regions");
                }

                Console.WriteLine("[*] -> Type      : " + Type);
                Console.WriteLine("[*] -> Size      : " + Align + " " + (Align > 1 ? "bytes" : "byte"));
                Console.WriteLine("[*] -> Alignment : " + Align);
                Console.WriteLine("[*] -> Value     : " + ValueS);

                if (Signature != null)
                {
                    Console.WriteLine("[*] -> Signature : Yes");
                }
                
                Console.WriteLine("[*] ------------------------------------------------- ");
                Console.WriteLine("[*] -> Execute ? (Y/n) [Y]");

                var Input = Console.ReadKey(true);

                if (!(Input.Key == ConsoleKey.Y || Input.Key == ConsoleKey.Enter))
                {
                    return;
                }

                var Scanner = Engine.ScannerEngine;
                var Memory  = Engine.MemoryEngine;

                var Regions = (List<MemoryBasicInformation>) null;

                if (From > 0 && To > 0)
                {
                    Regions = Memory.GetMemoryRegions(From, To, Region => Region.Protect == MemoryPagePermissions.PAGE_READWRITE && Region.State == MemoryPageState.MEM_COMMIT);
                }
                else
                {
                    Regions = Memory.GetMemoryRegions(Region => Region.Protect == MemoryPagePermissions.PAGE_READWRITE && Region.State == MemoryPageState.MEM_COMMIT);
                }

                try
                {
                    Console.WriteLine();

                    if (Type == "custom" && Signature != null)
                    {
                        if (Scanner.TrySearchFor(Signature, Regions, out var Result))
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("[*] Found the searched signature at 0x" + Result.Address.ToString("X").PadLeft(16, '0') + ".");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.WriteLine("[*] Couldn't find the searched value" + (Result.IsErrored ? " and experienced error(s)." : "."));
                            Console.ResetColor();
                        }
                    }
                    else
                    {
                        if (Scanner.TrySearchFor(Value, Regions, out List<ulong> Results))
                        {
                            Console.ForegroundColor = ConsoleColor.Green;

                            foreach (var Result in Results)
                            {
                                Console.WriteLine("[*] Found the searched value at 0x" + Result.ToString("X").PadLeft(16, '0') + ".");
                            }

                            Console.ResetColor();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.WriteLine("[*] Couldn't find the searched value.");
                            Console.ResetColor();
                        }
                    }
                }
                catch (Exception Exception)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[*] The scan engine threw an " + Exception.GetType().Name + ".");
                    Console.WriteLine("[*] It says : \"" + Exception.Message + "\".");
                    Console.ResetColor();
                }
            }

            Console.WriteLine();
        }
    }
}
