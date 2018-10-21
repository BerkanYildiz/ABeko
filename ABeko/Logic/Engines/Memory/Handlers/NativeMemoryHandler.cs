namespace ABeko.Logic.Engines.Memory.Handlers
{
    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security;

    using ABeko.Interfaces;

    public class NativeMemoryHandler : IMemory
    {
        /// <summary>
        /// Gets or sets the lastly used process identifier.
        /// </summary>
        public int LastProcessId
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the process handle.
        /// </summary>
        public IntPtr Handle
        {
            get;
            private set;
        }

        /// <summary>
        /// Sets the process identifier.
        /// </summary>
        /// <param name="ProcId">The process identifier.</param>
        public void SetProcId(int ProcId)
        {
            if (this.LastProcessId == ProcId)
            {
                return;
            }

            if (this.LastProcessId > 0)
            {
                Log.Info(typeof(NativeMemoryHandler), "Closing process handle.");
                NativeMemoryHandler.CloseHandle(this.Handle);
            }

            if (ProcId <= 0)
            {
                if (ProcId == -1)
                {
                    return;
                }
                
                throw new ArgumentException("ProcId is inferior or equal to 0", nameof(ProcId));
            }

            this.LastProcessId = ProcId;
            this.Handle = NativeMemoryHandler.OpenProcess(0x001F0FFF, false, ProcId);
        }

        /// <summary>
        /// Reads bytes at the specified address and returns it.
        /// </summary>
        /// <param name="Address">The address.</param>
        /// <param name="UseBaseAddress">Whether to preprend the base address.</param>
        public byte[] Read(ulong Address, uint Size, bool UseBaseAddress = false)
        {
            Log.Info(typeof(NativeMemoryHandler), "Reading " + Size + " bytes at 0x" + Address.ToString("X").PadLeft(16, '0') + ".");
            
            if (Address >= 0x7FFFFFFFFFF)
            {
                throw new ArgumentException("Address is outside userspace virtual memory range");
            }

            if (Address + Size > 0x7FFFFFFFFFF)
            {
                throw new ArgumentException("Address plus size is outside userspace virtual memory range");
            }

            var Buffer      = new byte[Size];
            var Read        = 0u;

            if (!ReadProcessMemory(this.Handle, Address, Buffer, Size, ref Read))
            {
                throw new Exception("Failed to read memory from remote process");
            }

            return Buffer;
        }

        /// <summary>
        /// Reads bytes at the specified address and returns a structure from it.
        /// </summary>
        /// <param name="Address">The address.</param>
        /// <param name="UseBaseAddress">Whether to preprend the base address.</param>
        public T Read<T>(ulong Address, bool UseBaseAddress = false)
        {
            Log.Info(typeof(NativeMemoryHandler), "Reading custom structure at 0x" + Address.ToString("X").PadLeft(16, '0') + ".");

            var Buffer      = default(T);
            var Size        = Marshal.SizeOf<T>();
            var Allocation  = Marshal.AllocHGlobal(Size);
            var Read        = 0u;

            if (Allocation == IntPtr.Zero)
            {
                throw new InsufficientMemoryException("Couldn't allocate memory for the buffer");
            }

            var Success     = ReadProcessMemory(this.Handle, Address, Buffer, (uint) Size, ref Read);
            
            if (Success)
            {
                Buffer      = Marshal.PtrToStructure<T>(Allocation);
            }

            Marshal.FreeHGlobal(Allocation);

            if (!Success)
            {
                throw new Exception("Failed to read memory from remote process");
            }

            return Buffer;
        }

        /// <summary>
        /// Writes bytes at the specified address.
        /// </summary>
        /// <param name="Address">The address.</param>
        /// <param name="Buffer">The buffer.</param>
        public void Write(ulong Address, byte[] Buffer, bool UseBaseAddress = false)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes bytes from a structure at the specified address.
        /// </summary>
        /// <param name="Address">The address.</param>
        public void Write<T>(ulong Address, T Structure, bool UseBaseAddress = false)
        {
            throw new NotImplementedException();
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(uint Permissions, bool InheritHandle, int ProcessId);

        [DllImport("kernel32.dll")] 
        private static extern bool ReadProcessMemory(IntPtr Handle, ulong Address, [Out, MarshalAs(UnmanagedType.AsAny)] object Buffer, uint Size, ref uint NumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError=true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr Handle);
    }
}
