namespace ABeko.Logic.Handlers
{
    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security;

    using ABeko.Logic.Interfaces;

    public class NativeMemoryHandler : IMemoryHandler
    {
        /// <summary>
        /// Gets or sets the event invoked when this instance is disposed.
        /// </summary>
        public EventHandler Disposed
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        public bool IsDisposed
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the selected process identifier.
        /// </summary>
        public int ProcessId
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
        /// Initializes a new instance of the <see cref="NativeMemoryHandler"/> class.
        /// </summary>
        public NativeMemoryHandler()
        {
            // NativeMemoryHandler
        }

        /// <summary>
        /// Sets the process identifier.
        /// </summary>
        /// <param name="ProcId">The process identifier.</param>
        public void SetProcId(int ProcId)
        {
            if (this.ProcessId == ProcId)
            {
                return;
            }

            if (this.ProcessId > 0)
            {
                NativeMemoryHandler.CloseHandle(this.Handle);
            }

            if (ProcId < 0)
            {
                throw new ArgumentException("ProcId is inferior to 0", nameof(ProcId));
            }

            this.ProcessId = ProcId;

            if (ProcId == 0)
            {
                return;
            }

            this.Handle = NativeMemoryHandler.OpenProcess(0x001F0FFF, false, ProcId);
        }

        /// <summary>
        /// Reads bytes at the specified address and returns it.
        /// </summary>
        /// <param name="Address">The address.</param>
        /// <param name="Size">The size.</param>
        public byte[] Read(ulong Address, uint Size)
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(nameof(BekoEngine), "The memory handler is disposed");
            }
            
            if (Address >= 0x7FFFFFFFFFFF)
            {
                throw new ArgumentException("Address is outside userspace virtual memory range");
            }

            if (Address + Size > 0x7FFFFFFFFFFF)
            {
                throw new ArgumentException("Address plus size is outside userspace virtual memory range");
            }

            var Buffer      = new byte[Size];
            var Read        = 0u;

            if (!NativeMemoryHandler.ReadProcessMemory(this.Handle, Address, Buffer, Size, ref Read))
            {
                throw new Exception("Failed to read memory from remote process");
            }

            return Buffer;
        }

        /// <summary>
        /// Reads bytes at the specified address and returns a structure from it.
        /// </summary>
        /// <param name="Address">The address.</param>
        public T Read<T>(ulong Address)
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(nameof(BekoEngine), "The memory handler is disposed");
            }

            if (Address >= 0x7FFFFFFFFFFF)
            {
                throw new ArgumentException("Address is outside userspace virtual memory range");
            }
            
            var Size        = Marshal.SizeOf<T>();

            if (Address + (ulong) Size > 0x7FFFFFFFFFFF)
            {
                throw new ArgumentException("Address plus size is outside userspace virtual memory range");
            }

            var Buffer      = default(T);
            
            var Allocation  = Marshal.AllocHGlobal(Size);
            var Read        = 0u;

            if (Allocation == IntPtr.Zero)
            {
                throw new InsufficientMemoryException("Couldn't allocate memory for the buffer");
            }

            var Success     = NativeMemoryHandler.ReadProcessMemory(this.Handle, Address, Buffer, (uint) Size, ref Read);
            
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
        /// Exécute les tâches définies par l'application associées à la
        /// libération ou à la redéfinition des ressources non managées.
        /// </summary>
        public void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            this.IsDisposed = true;

            // ..

            this.SetProcId(0);

            // ..

            if (this.Disposed != null)
            {
                try
                {
                    this.Disposed.Invoke(this, EventArgs.Empty);
                }
                catch (Exception)
                {
                    // ...
                }
            }
        }

        #region Externs

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(uint Permissions, bool InheritHandle, int ProcessId);

        [DllImport("kernel32.dll")] 
        private static extern bool ReadProcessMemory(IntPtr Handle, ulong Address, [Out, MarshalAs(UnmanagedType.AsAny)] object Buffer, uint Size, ref uint NumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError=true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr Handle);

        #endregion
    }
}
