namespace ABeko.Logic.Engines.Scanner
{
    using System;
    using System.Collections.Generic;

    using ABeko.Logic.Engines.Scanner.Events;
    using ABeko.Logic.Native;
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
        /// <param name="UICallback">The UI callback.</param>
        public bool TrySearchFor(string SignatureName, ulong From, ulong To, out SignatureResult Result, Action<ScanInfoEvent> UICallback = null)
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

            return TrySearchFor(Signature, From, To, out Result, UICallback);
        }

        /// <summary>
        /// Scans for the specified signature in the specified memory range.
        /// </summary>
        /// <param name="SignatureName">The signature name.</param>
        /// <param name="MemoryRegions">The memory regions.</param>
        /// <param name="Result">The scan result.</param>
        /// <param name="UICallback">The UI callback.</param>
        public bool TrySearchFor(string SignatureName, List<MemoryBasicInformation> MemoryRegions, out SignatureResult Result, Action<ScanInfoEvent> UICallback = null)
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

            return TrySearchFor(Signature, MemoryRegions, out Result, UICallback);
        }

        /// <summary>
        /// Scans for the specified signature in the specified memory range.
        /// </summary>
        /// <param name="Signature">The signature.</param>
        /// <param name="From">The address used to start the scan.</param>
        /// <param name="To">The address used to end the scan.</param>
        /// <param name="Result">The scan result address.</param>
        /// <param name="UICallback">The UI callback.</param>
        public bool TrySearchFor(Signature Signature, ulong From, ulong To, out SignatureResult Result, Action<ScanInfoEvent> UICallback = null)
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(nameof(BekoEngine), "The engine is disposed");
            }

            if (Signature == null)
            {
                throw new ArgumentException("SignatureName is not in the signatures dictionnary");
            }

            Result = this.SearchFor(Signature, From, To, UICallback);

            if (Result != null)
            {
                return Result.IsFound;
            }

            return false;
        }

        /// <summary>
        /// Scans for the specified signature in the specified memory range.
        /// </summary>
        /// <param name="Signature">The signature.</param>
        /// <param name="MemoryRegions">The memory regions.</param>
        /// <param name="Result">The scan result address.</param>
        /// <param name="UICallback">The UI callback.</param>
        public bool TrySearchFor(Signature Signature, List<MemoryBasicInformation> MemoryRegions, out SignatureResult Result, Action<ScanInfoEvent> UICallback = null)
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(nameof(BekoEngine), "The engine is disposed");
            }

            if (Signature == null)
            {
                throw new ArgumentException("SignatureName is not in the signatures dictionnary");
            }

            Result = this.SearchFor(Signature, MemoryRegions, UICallback);

            if (Result != null)
            {
                return Result.IsFound;
            }

            return false;
        }

        /// <summary>
        /// Scans for the specified signature in the specified memory range.
        /// </summary>
        /// <param name="Signature">The signature.</param>
        /// <param name="From">The address used to start the scan.</param>
        /// <param name="To">The address used to end the scan.</param>
        /// <param name="UICallback">The UI callback.</param>
        public SignatureResult SearchFor(Signature Signature, ulong From, ulong To, Action<ScanInfoEvent> UICallback = null)
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
            
            if (Size < Signature.Length)
            {
                throw new ArgumentException("The memory range to search in is inferior than the size of the signature");
            }

            const uint BufferSize   = 0x1000;
            var Result              = new SignatureResult(Signature);

            for (ulong I = 0; I < (ulong) Size;)
            {
                var Address = From + I;
                var EndAddr = From + I + BufferSize;
                var Buffer  = (byte[]) null;

                try
                {
                    if (Size < BufferSize)
                    {
                        Buffer = this.MemoryEngine.MemoryHandler.Read(Address, (uint) Size);
                    }
                    else
                    {
                        if (EndAddr > To)
                        {
                            Buffer = this.MemoryEngine.MemoryHandler.Read(Address, (uint) (BufferSize - (EndAddr - To)));
                        }
                        else
                        {
                            Buffer = this.MemoryEngine.MemoryHandler.Read(Address, BufferSize);
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

                if (Buffer != null)
                {
                    if (TrySearchInBuffer(Signature, Buffer, out ulong Offset))
                    {
                        Result.SetAddress(Address + Offset);
                        Result.IsFound = true;
                        Log.Warning(typeof(ScannerEngine), " - Found the searched signature at 0x" + Result.Address.ToString("X").PadLeft(16, '0') + ".");
                        break;
                    }
                }
                else
                {
                    Result.IsErrored = true;
                }

                if (UICallback != null)
                {
                    try
                    {
                        UICallback.Invoke(new ScanInfoEvent((int) I / (int) BufferSize, (int) Size / (int) BufferSize));
                    }
                    catch (Exception)
                    {
                        // ..
                    }
                }
            }

            return Result;
        }

        /// <summary>
        /// Scans for the specified signature in the specified memory range.
        /// </summary>
        /// <param name="Signature">The signature.</param>
        /// <param name="MemoryRegions">The memory regions.</param>
        /// <param name="UICallback">The UI callback.</param>
        public SignatureResult SearchFor(Signature Signature, List<MemoryBasicInformation> MemoryRegions, Action<ScanInfoEvent> UICallback = null)
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(nameof(BekoEngine), "The engine is disposed");
            }

            if (Signature == null)
            {
                throw new ArgumentNullException(nameof(Signature));
            }

            var Result = (SignatureResult) null;

            foreach (var MemoryRegion in MemoryRegions)
            {
                Result = SearchFor(Signature, (ulong) MemoryRegion.BaseAddress, (ulong) MemoryRegion.EndAddress, UICallback);

                if (Result.IsFound)
                {
                    break;
                }
            }

            return Result;
        }

        /// <summary>
        /// Tries to scans for the specified signature in the specified memory range.
        /// </summary>
        /// <param name="Signature">The signature.</param>
        /// <param name="From">The address used to start the scan.</param>
        /// <param name="To">The address used to end the scan.</param>
        /// <param name="Result">The scan result address.</param>
        /// <param name="UICallback">The UI callback.</param>
        public bool TrySearchFor(byte[] Signature, ulong From, ulong To, out ulong Result, Action<ScanInfoEvent> UICallback = null)
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
                throw new ArgumentException("The memory range to search in is inferior than the size of the signature");
            }

            const uint BufferSize = 0x1000;

            for (ulong I = 0; I < (ulong) Size;)
            {
                var Address = From + I;
                var EndAddr = From + I + BufferSize;
                var Buffer  = (byte[]) null;

                try
                {
                    if (Size < BufferSize)
                    {
                        Buffer = this.MemoryEngine.MemoryHandler.Read(Address, (uint) Size);
                    }
                    else
                    {
                        if (EndAddr > To)
                        {
                            Buffer = this.MemoryEngine.MemoryHandler.Read(Address, (uint) (BufferSize - (EndAddr - To)));
                        }
                        else
                        {
                            Buffer = this.MemoryEngine.MemoryHandler.Read(Address, BufferSize);
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

                if (Buffer != null)
                {
                    if (TrySearchInBuffer(Signature, Buffer, out ulong Offset))
                    {
                        Result = (Address + Offset);
                        Log.Warning(typeof(ScannerEngine), " - Found the searched value at 0x" + Result.ToString("X").PadLeft(16, '0') + ".");
                        return true;
                    }
                }

                if (UICallback != null)
                {
                    try
                    {
                        UICallback.Invoke(new ScanInfoEvent((int) I / (int) BufferSize, (int) Size / (int) BufferSize));
                    }
                    catch (Exception)
                    {
                        // ..
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Tries to scans for the specified signature in the specified memory range.
        /// </summary>
        /// <param name="Signature">The signature.</param>
        /// <param name="From">The address used to start the scan.</param>
        /// <param name="To">The address used to end the scan.</param>
        /// <param name="Results">The scan result addresses.</param>
        /// <param name="UICallback">The UI callback.</param>
        public bool TrySearchFor(byte[] Signature, ulong From, ulong To, out List<ulong> Results, Action<ScanInfoEvent> UICallback = null)
        {
            Results = new List<ulong>();

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
                throw new ArgumentException("The memory range to search in is inferior than the size of the signature");
            }

            const uint BufferSize = 0x1000;

            for (ulong I = 0; I < (ulong) Size;)
            {
                var Address = From + I;
                var EndAddr = From + I + BufferSize;
                var Buffer  = (byte[]) null;

                try
                {
                    if (Size < BufferSize)
                    {
                        Buffer = this.MemoryEngine.MemoryHandler.Read(Address, (uint) Size);
                    }
                    else
                    {
                        if (EndAddr > To)
                        {
                            Buffer = this.MemoryEngine.MemoryHandler.Read(Address, (uint) (BufferSize - (EndAddr - To)));
                        }
                        else
                        {
                            Buffer = this.MemoryEngine.MemoryHandler.Read(Address, BufferSize);
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

                if (Buffer != null)
                {
                    if (TrySearchInBuffer(Signature, Buffer, out List<ulong> Offsets))
                    {
                        foreach (var Offset in Offsets)
                        {
                            Results.Add(Address + Offset);
                            Log.Warning(typeof(ScannerEngine), " - Found the searched value at 0x" + (Address + Offset).ToString("X").PadLeft(16, '0') + ".");
                        }
                    }
                }

                if (UICallback != null)
                {
                    try
                    {
                        UICallback.Invoke(new ScanInfoEvent((int) I / (int) BufferSize, (int) Size / (int) BufferSize));
                    }
                    catch (Exception)
                    {
                        // ..
                    }
                }
            }

            return Results.Count > 0;
        }

        /// <summary>
        /// Tries to scans for the specified signature in the specified memory range.
        /// </summary>
        /// <param name="Signature">The signature.</param>
        /// <param name="From">The address used to start the scan.</param>
        /// <param name="To">The address used to end the scan.</param>
        /// <param name="Result">The scan result address.</param>
        /// <param name="UICallback">The UI callback.</param>
        public bool TrySearchFor(byte[] Signature, List<MemoryBasicInformation> Regions, out ulong Result, Action<ScanInfoEvent> UICallback = null)
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

            foreach (var Region in Regions)
            {
                if (TrySearchFor(Signature, (ulong) Region.BaseAddress, (ulong) Region.EndAddress, out Result, UICallback))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Tries to scans for the specified signature in the specified memory range.
        /// </summary>
        /// <param name="Signature">The signature.</param>
        /// <param name="From">The address used to start the scan.</param>
        /// <param name="To">The address used to end the scan.</param>
        /// <param name="Results">The scan result address.</param>
        /// <param name="UICallback">The UI callback.</param>
        public bool TrySearchFor(byte[] Signature, List<MemoryBasicInformation> Regions, out List<ulong> Results, Action<ScanInfoEvent> UICallback = null)
        {
            Results = new List<ulong>();

            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(nameof(BekoEngine), "The engine is disposed");
            }

            if (Signature == null)
            {
                throw new ArgumentNullException(nameof(Signature));
            }

            foreach (var Region in Regions)
            {
                if (TrySearchFor(Signature, (ulong) Region.BaseAddress, (ulong) Region.EndAddress, out List<ulong> CurrentResults, UICallback))
                {
                    Results.AddRange(CurrentResults);
                }
            }

            return Results.Count > 0;
        }

        /// <summary>
        /// Tries to scan the buffer for the specified signature.
        /// </summary>
        /// <param name="Signature">The signature.</param>
        /// <param name="Buffer">The buffer.</param>
        /// <param name="Offset">The offset.</param>
        public bool TrySearchInBuffer(Signature Signature, byte[] Buffer, out ulong Offset)
        {
            Offset = 0;

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
                        Offset = X;
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Tries to scan the buffer for the specified signature.
        /// </summary>
        /// <param name="Signature">The signature.</param>
        /// <param name="Buffer">The buffer.</param>
        /// <param name="Offsets">The offsets.</param>
        public bool TrySearchInBuffer(Signature Signature, byte[] Buffer, out List<ulong> Offsets)
        {
            Offsets = new List<ulong>();

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
                        Offsets.Add(X);
                        X += (uint) Y;
                    }
                }
            }

            return Offsets.Count > 0;
        }

        /// <summary>
        /// Tries to scan the buffer for the specified signature.
        /// </summary>
        /// <param name="Signature">The signature.</param>
        /// <param name="Buffer">The buffer.</param>
        /// <param name="Offset">The offset.</param>
        public bool TrySearchInBuffer(byte[] Signature, byte[] Buffer, out ulong Offset)
        {
            Offset = 0;

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
                        Offset = X;
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Tries to scan the buffer for the specified signature.
        /// </summary>
        /// <param name="Signature">The signature.</param>
        /// <param name="Buffer">The buffer.</param>
        /// <param name="Offsets">The offsets.</param>
        public bool TrySearchInBuffer(byte[] Signature, byte[] Buffer, out List<ulong> Offsets)
        {
            Offsets = new List<ulong>();

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
                        Offsets.Add(X);
                        X += (uint) Y;
                    }
                }
            }

            return Offsets.Count > 0;
        }
    }
}
