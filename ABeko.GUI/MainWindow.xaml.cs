namespace ABeko.GUI
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    using ABeko.Logic;
    using ABeko.Logic.Engines.Memory.Handlers;
    using ABeko.Logic.Handlers;
    using ABeko.Logic.Native;
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

        /// <summary>
        /// Gets the snapshot of the memory regions.
        /// </summary>
        public List<MemoryBasicInformation> MemoryRegions
        {
            get;
            private set;
        }

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

            this.MemoryRegions  = this.Engine.MemoryEngine.GetMemoryRegions(Region => (Region.Protect == MemoryPagePermissions.PAGE_READWRITE || Region.Protect == MemoryPagePermissions.PAGE_EXECUTE_READWRITE) && Region.State == MemoryPageState.MEM_COMMIT);

            if (this.AvailableMemoryRegions.ItemsSource == null)
            {
                this.AvailableMemoryRegions.ItemsSource = this.MemoryRegions;
            }
        }

        /// <summary>
        /// Called when the software is closing.
        /// </summary>
        /// <param name="Sender">The sender.</param>
        /// <param name="Args">The <see cref="CancelEventArgs"/> instance containing the event data.</param>
        private void OnClosing(object Sender, CancelEventArgs Args)
        {
            if (this.MemoryRegions != null)
            {
                this.MemoryRegions.Clear();
                this.MemoryRegions = null;
            }

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

        /// <summary>
        /// Called when the retrieved addresses datagrid has to be resized.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="Args">The <see cref="DragDeltaEventArgs"/> instance containing the event data.</param>
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
                    this.RetrievedAddressesGrid.Width = this.RetrievedAddressesGrid.MinWidth;
                }
                else
                {
                    this.RetrievedAddressesGrid.Width = Width;
                }
            }
        }

        /// <summary>
        /// Called when the open process menu has been clicked.
        /// </summary>
        /// <param name="Sender">The sender.</param>
        /// <param name="Args">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnOpenProcess(object Sender, RoutedEventArgs Args)
        {
            var ProcessSelectionWindow = new ProcessSelectionWindow();
            var ProcessSelected        = ProcessSelectionWindow.ShowDialog();
        }

        /// <summary>
        /// Called when the range has been changed.
        /// </summary>
        /// <param name="Sender">The sender.</param>
        /// <param name="Args">The <see cref="TextCompositionEventArgs"/> instance containing the event data.</param>
        private void OnRangeChanged(object Sender, TextCompositionEventArgs Args)
        {
            var AuthorizedChars = new []
            {
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
                'A', 'B', 'C', 'D', 'E', 'F'
            };

            foreach (var Character in Args.Text)
            {
                if (AuthorizedChars.Any(T => T == char.ToUpper(Character)))
                {
                    continue;
                }

                Args.Handled = true;
            }

            // ..

            this.UiMinimumRange.Text = this.UiMinimumRange.Text.Trim();
            this.UiMaximumRange.Text = this.UiMaximumRange.Text.Trim();
        }
    }
}
