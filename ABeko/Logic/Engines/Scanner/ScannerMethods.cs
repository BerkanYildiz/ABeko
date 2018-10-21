namespace ABeko.Logic.Engines.Scanner
{
    using System;

    using ABeko.Logic.Types;

    public partial class ScannerEngine : IDisposable
    {
        /// <summary>
        /// Scans for the specified signature in the specified memory range.
        /// </summary>
        /// <param name="SignatureName">The signature name.</param>
        /// <param name="From">The address used to start the scan.</param>
        /// <param name="To">The address used to end the scan.</param>
        /// <param name="Result">The scan result.</param>
        public bool TrySearchFor(string SignatureName, ulong From, ulong To, out SignatureResult Result)
        {
            Result = this.SearchFor(SignatureName, From, To);

            if (Result != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Scans for the specified signature in the specified memory range.
        /// </summary>
        /// <param name="SignatureName">The signature name.</param>
        /// <param name="From">The address used to start the scan.</param>
        /// <param name="To">The address used to end the scan.</param>
        public SignatureResult SearchFor(string SignatureName, ulong From, ulong To)
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(nameof(BekoEngine), "The engine is disposed");
            }

            if (string.IsNullOrEmpty(SignatureName))
            {
                throw new ArgumentNullException(nameof(SignatureName));
            }

            var Signature = this.Signatures.Get(SignatureName);

            if (Signature == null)
            {
                throw new ArgumentException("SignatureName is not in the signatures dictionnary");
            }

            return this.SearchFor(Signature, From, To);
        }

        /// <summary>
        /// Scans for the specified signature in the specified memory range.
        /// </summary>
        /// <param name="Signature">The signature.</param>
        /// <param name="From">The address used to start the scan.</param>
        /// <param name="To">The address used to end the scan.</param>
        /// <param name="Result">The scan result.</param>
        public bool TrySearchFor(Signature Signature, ulong From, ulong To, out SignatureResult Result)
        {
            Result = this.SearchFor(Signature, From, To);

            if (Result != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Scans for the specified signature in the specified memory range.
        /// </summary>
        /// <param name="Signature">The signature.</param>
        /// <param name="From">The address used to start the scan.</param>
        /// <param name="To">The address used to end the scan.</param>
        public SignatureResult SearchFor(Signature Signature, ulong From, ulong To)
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(nameof(BekoEngine), "The engine is disposed");
            }

            if (Signature == null)
            {
                throw new ArgumentNullException(nameof(Signature));
            }

            if (To < From)
            {
                throw new ArgumentException("The address to start from is superior to the end address");
            }

            var Size = (long) To - (long) From;

            if (Size <= 0)
            {
                throw new ArgumentException("The address range is inferior or equal to zero", nameof(Size));
            }

            const uint BufferSize   = 4096;
            var Result              = new SignatureResult(Signature);

            for (ulong I = 0; I < (ulong) Size;)
            {
                var Address = From + I;
                var EndAddr = From + I + BufferSize;
                var Buffer  = (byte[]) null;

                Log.Info(typeof(ScannerEngine), "Scanning bytes at 0x" + Address.ToString("X").PadLeft(16, '0') + ".");

                try
                {
                    if (Size < BufferSize)
                    {
                        Buffer = this.MemoryEngine.Handler.Read(Address, (uint) Size);
                    }
                    else
                    {
                        if (EndAddr > To)
                        {
                            Buffer = this.MemoryEngine.Handler.Read(Address, (uint) (BufferSize - (EndAddr - To)));
                        }
                        else
                        {
                            Buffer = this.MemoryEngine.Handler.Read(Address, BufferSize);
                        }
                    }
                }
                catch (Exception Exception)
                {
                    // ..
                }
                finally
                {
                    if (Size < BufferSize)
                    {
                        I     += (ulong) Size;
                    }
                    else
                    {
                        if (EndAddr > To)
                        {
                            I     += (BufferSize - (EndAddr - To));
                        }
                        else
                        {
                            I     += (BufferSize);
                        }
                    }
                }

                if (Buffer == null)
                {
                    Result.IsErrored = true;
                    continue;
                }

                if (TrySearchInBuffer(Signature, Buffer, out var Offset))
                {
                    Result.SetAddress(Address + Offset);
                }
                else
                {
                    continue;
                }

                Log.Warning(typeof(ScannerEngine), " - Found the signature at 0x" + Result.Address.ToString("X").PadLeft(16, '0') + ".");
            }

            return Result;
        }

        /// <summary>
        /// Tries to scans for the specified signature in the specified memory range.
        /// </summary>
        /// <param name="Signature">The signature.</param>
        /// <param name="From">The address used to start the scan.</param>
        /// <param name="To">The address used to end the scan.</param>
        public bool TrySearchFor(byte[] Signature, ulong From, ulong To, out ulong Result)
        {
            Result = 0;

            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(nameof(BekoEngine), "The engine is disposed");
            }

            if (Signature == null)
            {
                throw new ArgumentNullException(nameof(Signature));
            }

            if (To < From)
            {
                throw new ArgumentException("The address to start from is superior to the end address");
            }

            var Size = (long) To - (long) From;

            if (Size <= 0)
            {
                throw new ArgumentException("The address range is inferior or equal to zero", nameof(Size));
            }

            if (Size < Signature.Length)
            {
                throw new ArgumentException("The memory range to search in is inferior than the size of the signature.");
            }

            const uint BufferSize   = 4096;

            for (ulong I = 0; I < (ulong) Size;)
            {
                var Address = From + I;
                var EndAddr = From + I + BufferSize;
                var Buffer  = (byte[]) null;

                Log.Info(typeof(ScannerEngine), "Scanning bytes at 0x" + Address.ToString("X").PadLeft(16, '0') + ".");

                try
                {
                    if (Size < BufferSize)
                    {
                        Buffer = this.MemoryEngine.Handler.Read(Address, (uint) Size);
                    }
                    else
                    {
                        if (EndAddr > To)
                        {
                            Buffer = this.MemoryEngine.Handler.Read(Address, (uint) (BufferSize - (EndAddr - To)));
                        }
                        else
                        {
                            Buffer = this.MemoryEngine.Handler.Read(Address, BufferSize);
                        }
                    }
                }
                catch (Exception Exception)
                {
                    // ..
                }
                finally
                {
                    if (Size < BufferSize)
                    {
                        I     += (ulong) Size;
                    }
                    else
                    {
                        if (EndAddr > To)
                        {
                            I     += (BufferSize - (EndAddr - To));
                        }
                        else
                        {
                            I     += (BufferSize);
                        }
                    }
                }

                if (Buffer == null)
                {
                    continue;
                }

                if (TrySearchInBuffer(Signature, Buffer, out var Offset))
                {
                    Result = (Address + Offset);
                    Log.Warning(typeof(ScannerEngine), " - Found the signature at 0x" + Result.ToString("X").PadLeft(16, '0') + ".");
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Tries to scan the buffer for the specified signature.
        /// </summary>
        /// <param name="Signature">The signature.</param>
        /// <param name="Buffer">The buffer.</param>
        /// <param name="Offset">The offset.</param>
        public bool TrySearchInBuffer(Signature Signature, byte[] Buffer, out ulong Offset)
        {
            Offset    = 0;
            var Found = false;

            for (uint X = 0; X < Buffer.Length - Signature.Length; X++)
            {
                for (int Y = 0; Y < Signature.Length; Y++)
                {
                    var CurrentByte = (byte) Buffer[X + Y];
                    var SigByte     = (byte) Signature[Y];

                    if (Signature.IsSpecified(Y) || !Signature.IsAnything(Y))
                    {
                        if (CurrentByte != SigByte)
                        {
                            break;
                        }
                    }

                    if (Y == Signature.Length - 1)
                    {
                        Offset  = X;
                        Found   = true;
                    }
                }

                if (Found)
                {
                    break;
                }
            }

            return Found;
        }

        /// <summary>
        /// Tries to scan the buffer for the specified signature.
        /// </summary>
        /// <param name="Signature">The signature.</param>
        /// <param name="Buffer">The buffer.</param>
        /// <param name="Offset">The offset.</param>
        public bool TrySearchInBuffer(byte[] Signature, byte[] Buffer, out ulong Offset)
        {
            Offset    = 0;
            var Found = false;

            for (uint X = 0; X < Buffer.Length - Signature.Length; X++)
            {
                for (int Y = 0; Y < Signature.Length; Y++)
                {
                    var CurrentByte = (byte) Buffer[X + Y];
                    var SigByte     = (byte) Signature[Y];

                    if (CurrentByte != SigByte)
                    {
                        break;
                    }

                    if (Y == Signature.Length - 1)
                    {
                        Offset  = X;
                        Found   = true;
                    }
                }

                if (Found)
                {
                    break;
                }
            }

            return Found;
        }
    }
}
