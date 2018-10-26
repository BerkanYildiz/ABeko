namespace ABeko.GUI
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;

    /// <summary>
    /// Logique d'interaction pour ProcessSelectionWindow.xaml
    /// </summary>
    public partial class ProcessSelectionWindow : Window
    {
        /// <summary>
        /// Gets or sets the available processes.
        /// </summary>
        public List<Process> AvailableProcesses
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessSelectionWindow"/> class.
        /// </summary>
        public ProcessSelectionWindow()
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
            this.AvailableProcesses = new List<Process>(Process.GetProcesses());

            for (var I = 0; I < this.AvailableProcesses.Count; I++)
            {
                var Process = this.AvailableProcesses[I];

                if (Process == null)
                {
                    continue;
                }

                this.ProcessesSelectionList.Items.Insert(I, Process.ProcessName);
            }
        }

        /// <summary>
        /// Called when the software is closing.
        /// </summary>
        /// <param name="Sender">The sender.</param>
        /// <param name="Args">The <see cref="CancelEventArgs"/> instance containing the event data.</param>
        private void OnClosing(object Sender, CancelEventArgs Args)
        {
            if (this.AvailableProcesses != null)
            {
                this.AvailableProcesses.Clear();
                this.AvailableProcesses = null;
            }
        }
    }
}
