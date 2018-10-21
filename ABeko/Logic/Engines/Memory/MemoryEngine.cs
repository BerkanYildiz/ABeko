namespace ABeko.Logic.Engines.Memory
{
    using System;

    using ABeko.Interfaces;
    using ABeko.Logic.Collections;

    public partial class MemoryEngine : IDisposable
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
        /// Gets or sets the engine.
        /// </summary>
        private BekoEngine BekoEngine
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the current process loaded modules.
        /// </summary>
        public Modules Modules
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the memory handler.
        /// </summary>
        public IMemory Handler
        {
            get
            {
                if (this.BekoEngine == null)
                {
                    return null;
                }

                return this.BekoEngine.Configuration.MemoryHandler;
            }
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
        /// Prevents a default instance of the <see cref="MemoryEngine"/> class from being created.
        /// </summary>
        private MemoryEngine()
        {
            this.Modules = new Modules();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryEngine"/> class.
        /// </summary>
        /// <param name="BekoEngine">The engine.</param>
        public MemoryEngine(BekoEngine BekoEngine)
        {
            this.SetBekoEngine(BekoEngine);
        }

        /// <summary>
        /// Sets the memory engine.
        /// </summary>
        /// <param name="BekoEngine">The memory engine.</param>
        /// <exception cref="System.ArgumentNullException">BekoEngine - BekoEngine is null</exception>
        internal void SetBekoEngine(BekoEngine BekoEngine)
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(nameof(BekoEngine), "The engine is disposed");
            }

            if (BekoEngine == null)
            {
                throw new ArgumentNullException(nameof(BekoEngine), "BekoEngine is null");
            }

            this.BekoEngine = BekoEngine;
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
    }
}
