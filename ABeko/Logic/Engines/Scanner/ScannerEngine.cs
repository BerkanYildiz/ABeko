﻿namespace ABeko.Logic.Engines.Scanner
{
    using System;

    using ABeko.Logic.Collections;
    using ABeko.Logic.Engines.Memory;

    public partial class ScannerEngine : IDisposable
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
        /// Gets the signatures dictionnary.
        /// </summary>
        public Signatures Signatures
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the memory engine.
        /// </summary>
        private MemoryEngine MemoryEngine
        {
            get
            {
                if (this.BekoEngine == null)
                {
                    return null;
                }

                return this.BekoEngine.MemoryEngine;
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
        /// Prevents a default instance of the <see cref="ScannerEngine"/> class from being created.
        /// </summary>
        private ScannerEngine()
        {
            this.Signatures = new Signatures();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScannerEngine"/> class.
        /// </summary>
        /// <param name="Memory">The memory engine.</param>
        public ScannerEngine(BekoEngine BekoEngine) : this()
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
                throw new ArgumentNullException(nameof(BekoEngine));
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
