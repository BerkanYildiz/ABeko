namespace ABeko.Interfaces
{
    public interface IMemory
    {
        /// <summary>
        /// Gets or sets the lastly used process identifier.
        /// </summary>
        int LastProcessId
        {
            get;
        }

        /// <summary>
        /// Sets the process identifier.
        /// </summary>
        /// <param name="ProcId">The process identifier.</param>
        void SetProcId(int ProcId);

        /// <summary>
        /// Reads bytes at the specified address and returns it.
        /// </summary>
        /// <param name="Address">The address.</param>
        byte[] Read(ulong Address, uint Size, bool UseBaseAddress = false);

        /// <summary>
        /// Reads bytes at the specified address and returns a structure from it.
        /// </summary>
        /// <param name="Address">The address.</param>
        T Read<T>(ulong Address, bool UseBaseAddress = false);

        /// <summary>
        /// Writes bytes at the specified address.
        /// </summary>
        /// <param name="Address">The address.</param>
        /// <param name="Buffer">The buffer.</param>
        void Write(ulong Address, byte[] Buffer, bool UseBaseAddress = false);

        /// <summary>
        /// Writes bytes from a structure at the specified address.
        /// </summary>
        /// <param name="Address">The address.</param>
        void Write<T>(ulong Address, T Structure, bool UseBaseAddress = false);
    }
}
