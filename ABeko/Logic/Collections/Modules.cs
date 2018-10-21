namespace ABeko.Logic.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    public class Modules
    {
        /// <summary>
        /// Gets or sets the dictionnary.
        /// </summary>
        private Dictionary<ulong, ProcessModule> Dictionnary
        {
            get;
        }

        /// <summary>
        /// Gets the length of the dictionnary.
        /// </summary>
        public uint Length
        {
            get
            {
                return (uint) this.Dictionnary.Count;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Modules"/> class.
        /// </summary>
        public Modules()
        {
            this.Dictionnary = new Dictionary<ulong, ProcessModule>();
        }

        /// <summary>
        /// Adds the specified module.
        /// </summary>
        /// <param name="Module">The module.</param>
        /// <exception cref="System.ArgumentNullException">Module is empty.</exception>
        /// <exception cref="System.Exception">Module is already in the dictionnary.</exception>
        public void Add(ProcessModule Module)
        {
            if (Module == null)
            {
                throw new ArgumentNullException(nameof(Module));
            }

            if (this.Dictionnary.ContainsKey((ulong) Module.BaseAddress))
            {
                throw new Exception("Module is already in the dictionnary");
            }

            this.Dictionnary.Add((ulong) Module.BaseAddress, Module);
        }

        /// <summary>
        /// Gets the specified module.
        /// </summary>
        /// <param name="Name">The base address of the module.</param>
        /// <exception cref="System.Exception">BaseAddress is not in the dictionnary.</exception>
        public ProcessModule Get(ulong BaseAddress)
        {
            if (!this.Dictionnary.ContainsKey(BaseAddress))
            {
                throw new Exception("BaseAddress is not in the dictionnary");
            }

            return this.Dictionnary[BaseAddress];
        }

        /// <summary>
        /// Removes the specified module.
        /// </summary>
        /// <param name="Module">The module.</param>
        /// <exception cref="System.ArgumentNullException">Module is null.</exception>
        public void Remove(ProcessModule Module)
        {
            if (Module == null)
            {
                throw new ArgumentNullException(nameof(Module));
            }

            this.Remove((ulong) Module.BaseAddress);
        }

        /// <summary>
        /// Removes the specified module.
        /// </summary>
        /// <param name="BaseAddress">The base address of the module.</param>
        /// <exception cref="System.Exception">BaseAddress is not in the dictionnary.</exception>
        public void Remove(ulong BaseAddress)
        {
            if (!this.Dictionnary.ContainsKey(BaseAddress))
            {
                throw new Exception("BaseAddress is not in the dictionnary");
            }

            this.Dictionnary.Remove(BaseAddress);
        }
    }
}
