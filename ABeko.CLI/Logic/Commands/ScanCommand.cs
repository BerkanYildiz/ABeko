namespace ABeko.CLI.Logic.Commands
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    using ABeko.Logic;
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
            var Type      = "None";
            var Align     = 0u;
            var Value     = (byte[]) null;
            var ValueS    = string.Empty;
            var Signature = (Signature) null;

            if (Parameters.Length < 7)
            {
                //   Usage  : <command> -from <address> -to <address> -type <value type> <value>
                // Examples : scan -from 0x400000 -to 0x400008 -type string MZ
                // Examples : scan -from 0x7ffcad880000 -to 0x7ffcad887000 -type string RichB

                Console.WriteLine("[*] Invalid number of arguments.");
            }
            else
            {
                ValueS            = Parameters[Parameters.Length - 1];

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
                            if (!ulong.TryParse(FlagValue, out From))
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

                    if (!Parameters.Contains("-from"))
                    {
                        From = 0x400000;
                    }

                    if (!Parameters.Contains("-to"))
                    {
                        From = 0x7FFFFFFFFFF;
                        //   = 0x7FFFFFFFFFFF
                    }
                }
                
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
                            Signature = new Signature("CLI", ValueS, new SignatureMask(new string('X', ValueS.Length)));
                        }
                        catch (Exception Exception)
                        {
                            Console.WriteLine("[*] Invalid arguments, failed to build the signature. (" + Exception.GetType().Name + ")");
                        }

                        if (Signature == null)
                        {
                            return;
                        }

                        Align = Signature.Length;
                        Value = Signature.SigBytes;
                    }
                }

                if (Value == null)
                {
                    Console.WriteLine("[*] Invalid arguments, the given value is either invalid, malformatted, or not supported.");
                    return;
                }

                // Prints the infos about the scan..

                Console.WriteLine("[*] ------------------------------------------------- ");
                Console.WriteLine("[*] Executing the scan with the specified parameters :");
                Console.WriteLine("[*] -> From      : 0x" + From.ToString("X").PadLeft(16, '0'));
                Console.WriteLine("[*] -> To        : 0x" + To.ToString("X").PadLeft(16, '0'));
                Console.WriteLine("[*] -> Type      : " + Type);
                Console.WriteLine("[*] -> Size      : " + Align + " byte" + (Align > 1 ? "s" : ""));
                Console.WriteLine("[*] -> Alignment : " + Align);
                Console.WriteLine("[*] -> Value     : " + ValueS);
                Console.WriteLine();
                Console.WriteLine("[*] -> Execute ? (Y/n) [Y]");

                var Input = Console.ReadKey(true);

                if (!(Input.Key == ConsoleKey.Y || Input.Key == ConsoleKey.Enter))
                {
                    return;
                }

                // Executes the scan if user agree..

                try
                {
                    var Scanner = Engine.ScannerEngine;

                    if (Type == "custom")
                    {
                        if (Scanner.TrySearchFor(Signature, From, To, out var SignatureResult) && SignatureResult.IsFound)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("[*] Found the specified signature at 0x" + SignatureResult.Address.ToString("X").PadLeft(16, '0') + ".");
                            Console.ResetColor();
                        }
                        else
                        {
                            if (SignatureResult.IsErrored)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("[*] Signature scan failed because of the memory read/write handler.");
                                Console.ResetColor();
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("[*] Couldn't find the searched value.");
                                Console.ResetColor();
                            }
                        }
                    }
                    else
                    {
                        if (Scanner.TrySearchFor(Value, From, To, out var Result))
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("[*] Found the specified signature at 0x" + Result.ToString("X").PadLeft(16, '0') + ".");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
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
        }
    }
}
