namespace ABeko.Logic.Native
{
    using System;
    using System.Runtime.InteropServices;

    using ABeko.Logic.Native.Enums;

    [StructLayout(LayoutKind.Sequential)] 
    public struct MemoryBasicInformation
    { 
        public IntPtr BaseAddress;
        public IntPtr AllocationBase;
        public MemoryPagePermissions AllocationProtect;
        public IntPtr RegionSize;
        public MemoryPageState State;
        public MemoryPagePermissions Protect;
        public MemoryPageType Type;

        public IntPtr EndAddress
        {
            get
            {
                return (IntPtr) ((ulong) this.BaseAddress + (ulong) this.RegionSize);
            }
        }
    }
}
