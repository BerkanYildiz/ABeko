namespace ABeko.Logic.Types
{
    using System;

    public class SignatureResult
    {
        /// <summary>
        /// Gets or sets the signature.
        /// </summary>
        public Signature Signature
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        public ulong Address
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether this signature has been found in memory.
        /// </summary>
        public bool IsFound
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets a value indicating whether this signature has threw an exception.
        /// </summary>
        public bool IsErrored
        {
            get;
            internal set;
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="SignatureResult"/> class from being created.
        /// </summary>
        private SignatureResult()
        {
            // SignatureResult
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SignatureResult"/> class.
        /// </summary>
        /// <param name="Signature">The signature.</param>
        /// <param name="Address">The address.</param>
        public SignatureResult(Signature Signature, ulong Address = 0) : this()
        {
            this.SetSignature(Signature);
            this.SetAddress(Address);
        }

        /// <summary>
        /// Sets the signature.
        /// </summary>
        /// <param name="Signature">The signature.</param>
        internal void SetSignature(Signature Signature)
        {
            if (Signature == null)
            {
                throw new ArgumentNullException(nameof(Signature));
            }

            this.Signature = Signature;
        }

        /// <summary>
        /// Sets the address.
        /// </summary>
        /// <param name="Address">The address.</param>
        internal void SetAddress(ulong Address)
        {
            this.Address = Address;
        }
    }
}
