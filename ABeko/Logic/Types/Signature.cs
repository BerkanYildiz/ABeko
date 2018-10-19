namespace ABeko.Logic.Types
{
    using System;

    using ABeko.Helpers;

    public class Signature
    {
        /// <summary>
        /// Gets or sets the signature name.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the signature string.
        /// </summary>
        public string Sig
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the signature string as bytes.
        /// </summary>
        public byte[] SigBytes
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the signature mask.
        /// </summary>
        public SignatureMask Mask
        {
            get;
            private set;
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="Signature"/> class from being created.
        /// </summary>
        private Signature()
        {
            // Signature
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Signature"/> class.
        /// </summary>
        /// <param name="Name">The name.</param>
        /// <param name="Signature">The signature.</param>
        /// <param name="Mask">The mask.</param>
        public Signature(string Name, string Signature, SignatureMask Mask) : this()
        {
            this.SetName(Name);
            this.SetSig(Signature);
            this.SetMask(Mask);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Signature"/> class.
        /// </summary>
        /// <param name="Name">The name.</param>
        /// <param name="Signature">The signature.</param>
        /// <param name="Mask">The mask.</param>
        public Signature(string Name, byte[] Signature, SignatureMask Mask) : this()
        {
            this.SetName(Name);
            this.SetSig(Signature);
            this.SetMask(Mask);
        }

        /// <summary>
        /// Sets the name of this signature.
        /// </summary>
        /// <param name="Name">The name.</param>
        public void SetName(string Name)
        {
            if (string.IsNullOrEmpty(Name))
            {
                throw new ArgumentNullException(nameof(Name));
            }

            this.Name = Name;
        }

        /// <summary>
        /// Sets the signature value.
        /// </summary>
        /// <param name="Signature">The signature.</param>
        /// <exception cref="System.ArgumentNullException">Signature</exception>
        public void SetSig(string Signature)
        {
            if (string.IsNullOrEmpty(Signature))
            {
                throw new ArgumentNullException(nameof(Signature));
            }

            Signature = Signature.Replace(" ", string.Empty);
            Signature = Signature.Replace("-", string.Empty);
            Signature = Signature.Replace("_", string.Empty);
            Signature = Signature.Replace("0x", string.Empty);
            Signature = Signature.Replace(",", string.Empty);

            if (Signature.Length % 2 != 0)
            {
                throw new ArgumentException("Signature length is not valid", nameof(Signature));
            }

            if (!Signature.IsValidHex())
            {
                throw new ArgumentException("Signature is not a valid hex string", nameof(Signature));
            }

            this.Sig = Signature;
            this.SigBytes = Signature.ToByteArray();
        }

        /// <summary>
        /// Sets the signature value.
        /// </summary>
        /// <param name="Signature">The signature.</param>
        /// <exception cref="System.ArgumentNullException">Signature</exception>
        public void SetSig(byte[] Signature)
        {
            if (Signature == null || Signature.Length == 0)
            {
                throw new ArgumentNullException(nameof(Signature));
            }

            this.SetSig(BitConverter.ToString(Signature));
        }

        /// <summary>
        /// Sets the mask.
        /// </summary>
        /// <param name="SignatureMask">The signature mask.</param>
        /// <exception cref="System.ArgumentNullException">SignatureMask</exception>
        public void SetMask(SignatureMask SignatureMask)
        {
            if (SignatureMask == null)
            {
                throw new ArgumentNullException(nameof(SignatureMask));
            }

            this.Mask = SignatureMask;
        }

        /// <summary>
        /// Determines whether the specified offset has the anything mask.
        /// </summary>
        /// <param name="Offset">The offset.</param>
        public bool IsAnything(int Offset)
        {
            return this.Mask.IsAnything(Offset);
        }

        /// <summary>
        /// Determines whether the specified offset has the specified mask.
        /// </summary>
        /// <param name="Offset">The offset.</param>
        public bool IsSpecified(int Offset)
        {
            return this.Mask.IsSpecified(Offset);
        }
    }
}
