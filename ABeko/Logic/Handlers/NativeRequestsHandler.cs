namespace ABeko.Logic.Engines.Memory.Handlers
{
    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security;

    using ABeko.Logic.Interfaces;
    using ABeko.Logic.Native;

    public class NativeRequestsHandler : IRequestsHandler
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
        /// Initializes a new instance of the <see cref="NativeRequestsHandler"/> class.
        /// </summary>
        public NativeRequestsHandler()
        {
            // NativeRequestsHandler
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
                CloseHandle(this.Handle);
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

            this.Handle = OpenProcess(0x001F0FFF, false, ProcId);
        }

        /// <summary>
        /// Gets the current system information.
        /// </summary>
        public bool TryGetSystemInfo(out SystemInfo SystemInfo)
        {
            GetSystemInfo(out SystemInfo);
            return true;
        }

        /// <summary>
        /// Gets the memory region information of a 64 bits process.
        /// </summary>
        /// <param name="Address">The address.</param>
        /// <exception cref="ArgumentException">Address is equal to zero</exception>
        public bool TryGetMemoryRegionInfo(ulong Address, out MemoryBasicInformation RegionInfo)
        {
            if (Address == 0)
            {
                throw new ArgumentException("Address is equal to zero");
            }

            var Size    = (uint) Marshal.SizeOf<MemoryBasicInformation>();
            var Success = VirtualQueryEx(this.Handle, Address, out RegionInfo, Size) > 0;

            if (!Success)
            {
                return false;
            }

            return true;
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

        [DllImport("kernel32.dll", SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr Handle);

        [DllImport("kernel32.dll")]
        private static extern void GetSystemInfo(out SystemInfo SystemInfo);

        [DllImport("kernel32.dll")] 
        private static extern int VirtualQueryEx(IntPtr Handle, ulong Address, out MemoryBasicInformation Buffer, uint Length);

        #endregion
    }
}
