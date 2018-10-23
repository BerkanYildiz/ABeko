namespace ABeko.Logic
{
    using System.Diagnostics;

    using ABeko.Logic.Interfaces;

    public class BekoConfig
    {
        /// <summary>
        /// Gets or sets the targeted process.
        /// </summary>
        public Process Process
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the memory handler.
        /// </summary>
        public IMemoryHandler MemoryHandler
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the requests handler.
        /// </summary>
        public IRequestsHandler RequestsHandler
        {
            get;
            set;
        }
    }
}
