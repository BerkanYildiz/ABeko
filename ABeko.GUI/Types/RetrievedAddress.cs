namespace ABeko.GUI.Types
{
    using System;
    using System.Globalization;

    public class ObservedAddress : RetrievedAddress
    {
        /// <summary>
        /// Gets or sets a value indicating whether this observede address is active.
        /// </summary>
        public string Active
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the address where the value has been found.
        /// </summary>
        public string Address
        {
            get
            {
                return "0x" + this.AddressPtr.ToString("X").PadLeft(16, '0');
            }
        }

        /// <summary>
        /// Gets or sets the address where the value has been found.
        /// </summary>
        public ulong AddressPtr
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
        /// Gets or sets the value datatype.
        /// </summary>
        public string Type
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RetrievedAddress"/> class.
        /// </summary>
        /// <param name="Address">The address.</param>
        /// <param name="Value">The value.</param>
        public ObservedAddress(RetrievedAddress RetrievedAddress) : this(RetrievedAddress.GetAddress(), RetrievedAddress.Value)
        {
            // ObservedAddress
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RetrievedAddress"/> class.
        /// </summary>
        /// <param name="Address">The address.</param>
        /// <param name="Value">The value.</param>
        public ObservedAddress(ulong Address, object Value, string Active = "Yes", string Description = "<No description>", string Type = "(NULL)")
        {
            this.Active      = Active;
            this.Description = Description;
            this.Type        = Type;
            this.AddressPtr  = Address;
            this.Value       = Value.ToString();
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
