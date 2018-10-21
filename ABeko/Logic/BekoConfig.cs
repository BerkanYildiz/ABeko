﻿namespace ABeko.Logic
{
    using System.Diagnostics;

    using ABeko.Interfaces;

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
        public IMemory MemoryHandler
        {
            get;
            set;
        }
    }
}
