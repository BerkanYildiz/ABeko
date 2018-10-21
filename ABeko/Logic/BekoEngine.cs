namespace ABeko.Logic
{
    using System;
    using System.Diagnostics;

    using ABeko.Interfaces;
    using ABeko.Logic.Engines;
    using ABeko.Logic.Engines.Memory;

    using ScannerEngine = ABeko.Logic.Engines.Scanner.ScannerEngine;

    public class BekoEngine : IDisposable
    {
        /// <summary>
        /// Gets or sets the event invoked when this instance is loaded.
        /// </summary>
        public EventHandler Loaded
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the event invoked when this instance is disposed.
        /// </summary>
        public EventHandler Disposed
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the engine configuration.
        /// </summary>
        public BekoConfig Configuration
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the memory engine.
        /// </summary>
        public MemoryEngine MemoryEngine
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the scanner engine.
        /// </summary>
        public ScannerEngine ScannerEngine
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the targeted process from the configuration.
        /// </summary>
        public Process Process
        {
            get
            {
                if (this.IsDisposed)
                {
                    throw new ObjectDisposedException(nameof(BekoEngine), "The engine is disposed");
                }

                if (this.Configuration == null)
                {
                    return null;
                }

                return this.Configuration.Process;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                this.Configuration.Process = value;

                if (this.Configuration.MemoryHandler == null)
                {
                    throw new Exception("MemoryHandler was null when setting process.");
                }

                this.Configuration.MemoryHandler.SetProcId(value.Id);
            }
        }

        /// <summary>
        /// Gets the memory handler from the configuration.
        /// </summary>
        public IMemory MemoryHandler
        {
            get
            {
                if (this.IsDisposed)
                {
                    throw new ObjectDisposedException(nameof(BekoEngine), "The engine is disposed");
                }

                if (this.Configuration == null)
                {
                    return null;
                }

                return this.Configuration.MemoryHandler;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is loaded.
        /// </summary>
        public bool IsLoaded
        {
            get;
            private set;
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
        /// Prevents a default instance of the <see cref="BekoEngine"/> class from being created.
        /// </summary>
        private BekoEngine()
        {
            this.MemoryEngine = new MemoryEngine(this);
            this.ScannerEngine = new ScannerEngine(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BekoEngine"/> class.
        /// </summary>
        /// <param name="Configuration">The configuration.</param>
        public BekoEngine(BekoConfig Configuration) : this()
        {
            this.SetConfiguration(Configuration);
        }

        /// <summary>
        /// Sets the engine configuration.
        /// </summary>
        /// <param name="Configuration">The configuration.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Configuration - Configuration is null
        /// or
        /// Process - Configuration->Process is null
        /// </exception>
        public void SetConfiguration(BekoConfig Configuration)
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(nameof(BekoEngine), "The engine is disposed");
            }

            if (Configuration == null)
            {
                throw new ArgumentNullException(nameof(Configuration), "Configuration is null");
            }

            if (Configuration.Process == null)
            {
                throw new ArgumentNullException(nameof(Configuration.Process), "Configuration->Process is null");
            }

            if (Configuration.MemoryHandler == null)
            {
                throw new ArgumentNullException(nameof(Configuration.MemoryHandler), "Configuration->MemoryHandler is null");
            }

            this.Configuration = Configuration;

            // ..

            Configuration.MemoryHandler.SetProcId(Configuration.Process.Id);
        }

        /// <summary>
        /// Sets the process.
        /// </summary>
        /// <param name="Process">The process.</param>
        /// <exception cref="ArgumentNullException">Process</exception>
        public void SetProcess(Process Process)
        {
            if (Process == null)
            {
                throw new ArgumentNullException(nameof(Process));
            }

            if (this.Process == Process)
            {
                return;
            }

            this.Configuration.Process = Process;
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

            if (this.MemoryEngine != null)
            {
                this.MemoryEngine.Dispose();
            }

            if (this.ScannerEngine != null)
            {
                this.ScannerEngine.Dispose();
            }

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

        /// <summary>
        /// Returns a new instance of the <see cref="BekoEngine"/> class,
        /// and initializes it with the given configuration.
        /// </summary>
        /// <param name="Configuration">The configuration.</param>
        public static BekoEngine FromConfiguration(BekoConfig Configuration)
        {
            return new BekoEngine(Configuration);
        }
    }
}
