namespace ABeko.Logic.Interfaces
{
    using System;

    using ABeko.Logic.Native;

    public interface IRequestsHandler : IDisposable
    {
        /// <summary>
        /// Sets the process identifier.
        /// </summary>
        /// <param name="ProcId">The process identifier.</param>
        void SetProcId(int ProcId);

        /// <summary>
        /// Tries to get the current system information.
        /// </summary>
        bool TryGetSystemInfo(out SystemInfo SystemInfo);

        /// <summary>
        /// Tries to get the memory region information at the specified address.
        /// </summary>
        /// <param name="Address">The address.</param>
        /// <exception cref="ArgumentException">Address is equal to zero</exception>
        bool TryGetMemoryRegionInfo(ulong Address, out MemoryBasicInformation RegionInfo);
    }
}
