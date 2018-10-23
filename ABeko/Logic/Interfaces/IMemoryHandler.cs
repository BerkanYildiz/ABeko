namespace ABeko.Logic.Interfaces
{
    using System;

    public interface IMemoryHandler : IDisposable
    {
        /// <summary>
        /// Sets the process identifier.
        /// </summary>
        /// <param name="ProcId">The process identifier.</param>
        void SetProcId(int ProcId);

        /// <summary>
        /// Reads bytes at the specified address and returns it.
        /// </summary>
        /// <param name="Address">The address.</param>
        byte[] Read(ulong Address, uint Size);

        /// <summary>
        /// Reads bytes at the specified address and returns a structure from it.
        /// </summary>
        /// <param name="Address">The address.</param>
        T Read<T>(ulong Address);
    }
}
