namespace ABeko.Logic.Engines.Memory
{
    using System;
    using System.Collections.Generic;

    using ABeko.Logic.Native;

    public partial class MemoryEngine : IDisposable
    {
        /// <summary>
        /// Gets the memory regions.
        /// </summary>
        public List<MemoryBasicInformation> GetMemoryRegions(Func<MemoryBasicInformation, bool> Comparer = null)
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(nameof(BekoEngine), "The engine is disposed");
            }

            if (!this.RequestsHandler.TryGetSystemInfo(out var SystemInfo))
            {
                throw new Exception("Failed to retrieve the current system information");
            }

            return this.GetMemoryRegions(SystemInfo, (ulong) SystemInfo.MinimumApplicationAddress, (ulong) SystemInfo.MaximumApplicationAddress, Comparer);
        }

        /// <summary>
        /// Gets the memory regions between the specified range.
        /// </summary>
        /// <param name="From">The address to begin the scan from.</param>
        /// <param name="To">The address to begin the scan at.</param>
        public List<MemoryBasicInformation> GetMemoryRegions(ulong From, ulong To, Func<MemoryBasicInformation, bool> Comparer = null)
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(nameof(BekoEngine), "The engine is disposed");
            }

            if (!this.RequestsHandler.TryGetSystemInfo(out var SystemInfo))
            {
                throw new Exception("Failed to retrieve the current system information");
            }

            if (From < (ulong) SystemInfo.MinimumApplicationAddress)
            {
                throw new Exception("The from parameter is inferior to the minimum usermode memory address");
            }

            if (To > (ulong) SystemInfo.MaximumApplicationAddress)
            {
                throw new Exception("The to parameter is superior to the maximum usermode memory address");
            }

            return this.GetMemoryRegions(SystemInfo, From, To, Comparer);
        }

        /// <summary>
        /// Gets the memory regions between the specified range.
        /// </summary>
        /// <param name="SystemInfo">The system information.</param>
        /// <param name="From">The address to begin the scan from.</param>
        /// <param name="To">The address to begin the scan at.</param>
        /// <exception cref="ObjectDisposedException">BekoEngine - The engine is disposed</exception>
        /// <exception cref="ArgumentException">The from parameter is superior or equal to the to parameter</exception>
        /// <exception cref="Exception">
        /// The from parameter is inferior to the minimum usermode memory address
        /// or
        /// The to parameter is superior to the maximum usermode memory address
        /// </exception>
        public List<MemoryBasicInformation> GetMemoryRegions(SystemInfo SystemInfo, ulong From, ulong To, Func<MemoryBasicInformation, bool> Comparer = null)
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(nameof(BekoEngine), "The engine is disposed");
            }

            if (From >= To)
            {
                throw new ArgumentException("The from parameter is superior or equal to the to parameter");
            }

            var MemoryRegions = new List<MemoryBasicInformation>();
            var Requests      = this.RequestsHandler;

            if (From < (ulong) SystemInfo.MinimumApplicationAddress)
            {
                throw new Exception("The from parameter is inferior to the minimum usermode memory address");
            }

            if (To > (ulong) SystemInfo.MaximumApplicationAddress)
            {
                throw new Exception("The to parameter is superior to the maximum usermode memory address");
            }

            var CurrentAddr = From;
            var MaximumAddr = To;

            while (CurrentAddr < MaximumAddr)
            {
                if (Requests.TryGetMemoryRegionInfo(CurrentAddr, out var RegionInfo))
                {
                    CurrentAddr += (ulong) RegionInfo.RegionSize;

                    if (Comparer != null)
                    {
                        if (!Comparer(RegionInfo))
                        {
                            continue;
                        }
                    }
                    
                    MemoryRegions.Add(RegionInfo);
                }
                else
                {
                    CurrentAddr += SystemInfo.PageSize;
                }
            }

            return MemoryRegions;
        }
    }
}
