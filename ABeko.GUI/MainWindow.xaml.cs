namespace ABeko.GUI
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    using ABeko.GUI.Types;
    using ABeko.Logic;
    using ABeko.Logic.Engines.Memory.Handlers;
    using ABeko.Logic.Handlers;
    using ABeko.Logic.Native.Enums;

    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Gets the engine.
        /// </summary>
        public BekoEngine Engine
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the engine configuration.
        /// </summary>
        public BekoConfig Configuration
        {
            get;
            private set;
        }

        private Cursor _cursor;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.Loaded  += this.OnLoaded;
            this.Closing += this.OnClosing;

            this.InitializeComponent();
        }

        /// <summary>
        /// Called when the software is loaded.
        /// </summary>
        /// <param name="Sender">The sender.</param>
        /// <param name="Args">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnLoaded(object Sender, RoutedEventArgs Args)
        {
            this.Configuration  = new BekoConfig
            {
                Process         = Process.GetCurrentProcess(),
                MemoryHandler   = new NativeMemoryHandler(),
                RequestsHandler = new NativeRequestsHandler()
            };

            this.Engine         = new BekoEngine(Configuration);

            // ..

            var Value           = Encoding.UTF8.GetBytes("MZ");

            // ..

            var Regions         = this.Engine.MemoryEngine.GetMemoryRegions(Region => Region.Protect == MemoryPagePermissions.PAGE_READWRITE && Region.State == MemoryPageState.MEM_COMMIT);

            if (this.Engine.ScannerEngine.TrySearchFor(Value, Regions, out List<ulong> Results))
            {
                foreach (var Result in Results)
                {
                    this.RetrievedAddresses.Items.Add(new RetrievedAddress(Result, "MZ"));
                }
            }
        }

        /// <summary>
        /// Called when the software is closing.
        /// </summary>
        /// <param name="Sender">The sender.</param>
        /// <param name="Args">The <see cref="CancelEventArgs"/> instance containing the event data.</param>
        private void OnClosing(object Sender, CancelEventArgs Args)
        {
            if (this.Engine != null)
            {
                if (this.Engine.IsDisposed)
                {
                    return;
                }

                try
                {
                    this.Engine.Dispose();
                }
                catch (Exception)
                {
                    // ..
                }
            }

            this.Engine = null;
        }

        private void OnRetrieveAddressesWidthResize(object sender, DragDeltaEventArgs Args)
        {
            if (Args.HorizontalChange > 0)
            {
                this.RetrievedAddressesGrid.Width += Args.HorizontalChange;
            }
            else if (Args.HorizontalChange < 0)
            {
                var Width = this.RetrievedAddressesGrid.Width - Math.Abs(Args.HorizontalChange);

                if (Width < this.RetrievedAddressesGrid.MinWidth)
                {
                    return;
                }

                this.RetrievedAddressesGrid.Width = Width;
            }
        }
    }
}
