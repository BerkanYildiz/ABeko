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

        /// <summary>
        /// Gets the base address for the UI system.
        /// </summary>
        public string UiBaseAddress
        {
            get
            {
                return "0x" + ((ulong) this.BaseAddress).ToString("X").PadLeft(16, '0');
            }
        }

        /// <summary>
        /// Gets the end address for the UI system.
        /// </summary>
        public string UiEndAddress
        {
            get
            {
                return "0x" + ((ulong) this.EndAddress).ToString("X").PadLeft(16, '0');
            }
        }

        /// <summary>
        /// Gets the allocation size for the UI system.
        /// </summary>
        public string UiSize
        {
            get
            {
                return "0x" + ((ulong) this.RegionSize).ToString("X").PadLeft(16, '0');
            }
        }

        /// <summary>
        /// Gets the state for the UI system.
        /// </summary>
        public string UiState
        {
            get
            {
                switch (this.State)
                {
                    case MemoryPageState.MEM_COMMIT:
                    {
                        return "Commited";
                    }

                    case MemoryPageState.MEM_RESERVE:
                    {
                        return "Reserved";
                    }

                    case MemoryPageState.MEM_FREE:
                    {
                        return "Free";
                    }

                    default:
                    {
                        return "(NULL)";
                    }
                }
            }
        }

        /// <summary>
        /// Gets the type for the UI system.
        /// </summary>
        public string UiType
        {
            get
            {
                switch (this.Type)
                {
                    case MemoryPageType.MEM_IMAGE:
                    {
                        return "Image";
                    }

                    case MemoryPageType.MEM_MAPPED:
                    {
                        return "Mapped";
                    }

                    case MemoryPageType.MEM_PRIVATE:
                    {
                        return "Private";
                    }

                    default:
                    {
                        return "(NULL)";
                    }
                }
            }
        }

        /// <summary>
        /// Gets the protection for the UI system.
        /// </summary>
        public string UiProtection
        {
            get
            {
                switch (this.Protect)
                {
                    case MemoryPagePermissions.PAGE_EXECUTE:
                    {
                        return "X";
                    }

                    case MemoryPagePermissions.PAGE_EXECUTE_READ:
                    {
                        return "RX";
                    }

                    case MemoryPagePermissions.PAGE_EXECUTE_READWRITE:
                    {
                        return "RWX";
                    }

                    case MemoryPagePermissions.PAGE_EXECUTE_WRITECOPY:
                    {
                        return "CWX";
                    }

                    case MemoryPagePermissions.PAGE_READWRITE:
                    {
                        return "RW";
                    }

                    case MemoryPagePermissions.PAGE_READONLY:
                    {
                        return "R";
                    }

                    case MemoryPagePermissions.PAGE_NOACCESS:
                    {
                        return "NOA";
                    }

                    case MemoryPagePermissions.PAGE_GUARD:
                    {
                        return "G";
                    }

                    case MemoryPagePermissions.PAGE_WRITECOPY:
                    {
                        return "WC";
                    }

                    case MemoryPagePermissions.PAGE_WRITECOMBINE:
                    {
                        return "WCB";
                    }

                    default:
                    {
                        return "(NULL)";
                    }
                }
            }
        }
    }
}
