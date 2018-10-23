namespace ABeko.Logic.Types
{
    using System;

    using ABeko.Logic.Enums;

    public class SignatureMask
    {
        public char AnythingMask  = '?';
        public char SpecifiedMask = 'X';

        /// <summary>
        /// Gets or sets the mask string.
        /// </summary>
        public string Mask
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the length of the mask string.
        /// </summary>
        public uint Length
        {
            get
            {
                if (string.IsNullOrEmpty(this.Mask))
                {
                    return 0;
                }

                return (uint) this.Mask.Length;
            }
        }

        /// <summary>
        /// Gets the <see cref="MaskChar"/> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="MaskChar"/>.
        /// </value>
        /// <param name="I">The index.</param>
        public MaskChar this[int I]
        {
            get
            {
                if (I >= this.Mask.Length)
                {
                    throw new IndexOutOfRangeException();
                }

                var Char = this.Mask[I];

                if (Char == this.AnythingMask)
                {
                    return MaskChar.Anything;
                }

                if (Char == this.SpecifiedMask)
                {
                    return MaskChar.Specified;
                }

                return MaskChar.Unknown;
            }
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="SignatureMask"/> class from being created.
        /// </summary>
        private SignatureMask()
        {
            // SignatureMask
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SignatureMask"/> class.
        /// </summary>
        /// <param name="Mask">The mask.</param>
        /// <param name="AnythingMask">The anything mask.</param>
        /// <param name="SpecifiedMask">The specified mask.</param>
        public SignatureMask(string Mask, char AnythingMask = '?', char SpecifiedMask = 'X')
        {
            this.SetMask(Mask);
            this.SetMasks(AnythingMask, SpecifiedMask);

            foreach (var MaskChar in Mask)
            {
                if (MaskChar == this.AnythingMask || MaskChar == this.SpecifiedMask)
                {
                    continue;
                }

                throw new ArgumentException("Mask contains characters that are neither AnythingMask or SpecifiedMask");
            }
        }

        /// <summary>
        /// Sets the specified mask.
        /// </summary>
        /// <param name="Mask">The mask.</param>
        /// <exception cref="System.ArgumentNullException">Mask</exception>
        internal void SetMask(string Mask)
        {
            if (string.IsNullOrEmpty(Mask))
            {
                throw new ArgumentNullException(nameof(Mask));
            }

            this.Mask = Mask;
        }

        /// <summary>
        /// Sets the specified masks.
        /// </summary>
        /// <param name="AnythingMask">Anything mask.</param>
        /// <param name="SpecifiedMask">The specified mask.</param>
        internal void SetMasks(char AnythingMask, char SpecifiedMask)
        {
            this.SetAnythingMask(AnythingMask);
            this.SetSpecifiedMask(SpecifiedMask);
        }

        /// <summary>
        /// Sets the anything mask.
        /// </summary>
        /// <param name="AnythingMask">the anything mask.</param>
        /// <exception cref="System.ArgumentNullException">AnythingMask</exception>
        internal void SetAnythingMask(char AnythingMask)
        {
            this.AnythingMask = AnythingMask;
        }

        /// <summary>
        /// Sets the specified mask.
        /// </summary>
        /// <param name="SpecifiedMask">The specified mask.</param>
        /// <exception cref="System.ArgumentNullException">SpecifiedMask</exception>
        internal void SetSpecifiedMask(char SpecifiedMask)
        {
            this.SpecifiedMask = SpecifiedMask;
        }

        /// <summary>
        /// Determines whether the specified offset is withing range of the mask length.
        /// </summary>
        /// <param name="Offset">The offset.</param>
        public bool IsWithingRange(int Offset)
        {
            return Offset < this.Mask.Length;
        }

        /// <summary>
        /// Determines whether the specified offset is outside the range of the mask length.
        /// </summary>
        /// <param name="Offset">The offset.</param>
        public bool IsOutOfRange(int Offset)
        {
            return Offset >= this.Mask.Length;
        }

        /// <summary>
        /// Determines whether the specified offset has the anything mask.
        /// </summary>
        /// <param name="Offset">The offset.</param>
        public bool IsAnything(int Offset)
        {
            if (this.IsWithingRange(Offset))
            {
                if (this.Mask[Offset] == this.AnythingMask)
                {
                    return true;
                }
            }
            else
            {
                throw new IndexOutOfRangeException("Offset is out of the mask length range");
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified offset has the specified mask.
        /// </summary>
        /// <param name="Offset">The offset.</param>
        public bool IsSpecified(int Offset)
        {
            if (this.IsOutOfRange(Offset))
            {
                throw new IndexOutOfRangeException("Offset is out of the mask length range");
            }

            if (this.Mask[Offset] == this.SpecifiedMask)
            {
                return true;
            }

            return false;
        }
    }
}
