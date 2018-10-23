namespace ABeko.Logic.Engines.Scanner.Events
{
    using System;

    public class ScanInfoEvent : EventArgs
    {
        /// <summary>
        /// Gets the current index.
        /// </summary>
        public int Current
        {
            get;
        }

        /// <summary>
        /// Gets the total indexes.
        /// </summary>
        public int Total
        {
            get;
        }

        /// <summary>
        /// Gets the percentage.
        /// </summary>
        public int Percentage
        {
            get
            {
                return (int) (((decimal) this.Current / (decimal) this.Total) * 100);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScanInfoEvent"/> class.
        /// </summary>
        public ScanInfoEvent(int CurrentIndex, int TotalIndex)
        {
            this.Current = CurrentIndex;
            this.Total   = TotalIndex;
        }
    }
}
