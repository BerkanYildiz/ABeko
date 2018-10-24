namespace ABeko.GUI.Types
{
    using System;
    using System.Globalization;

    public class RetrievedAddress
    {
        /// <summary>
        /// Gets or sets the address where the value has been found.
        /// </summary>
        public string Address
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the value at the specified address.
        /// </summary>
        public string Value
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RetrievedAddress"/> class.
        /// </summary>
        public RetrievedAddress()
        {
            this.Address = "0x0000000000000000";
            this.Value   = "(NULL)";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RetrievedAddress"/> class.
        /// </summary>
        /// <param name="Address">The address.</param>
        /// <param name="Value">The value.</param>
        public RetrievedAddress(ulong Address, string Value)
        {
            this.Address = "0x" + Address.ToString("X").PadLeft(16, '0');
            this.Value   = Value;
        }

        /// <summary>
        /// Gets the address.
        /// </summary>
        /// <exception cref="Exception">The address was null or empty</exception>
        public ulong GetAddress()
        {
            if (string.IsNullOrEmpty(this.Address))
            {
                throw new Exception("The address was null or empty");
            }

            return ulong.Parse(this.Address, NumberStyles.HexNumber, new NumberFormatInfo());
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        public override string ToString()
        {
            return "{ 'Address': '" + this.Address + "', 'Value': '" + this.Value + "' }";
        }
    }
}
