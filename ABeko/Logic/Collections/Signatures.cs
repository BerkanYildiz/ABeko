namespace ABeko.Logic.Collections
{
    using System;
    using System.Collections.Generic;

    using ABeko.Logic.Types;

    public class Signatures
    {
        /// <summary>
        /// Gets or sets the dictionnary.
        /// </summary>
        private Dictionary<string, Signature> Dictionnary
        {
            get;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Signatures"/> class.
        /// </summary>
        public Signatures()
        {
            this.Dictionnary = new Dictionary<string, Signature>();
        }

        /// <summary>
        /// Adds the specified signature.
        /// </summary>
        /// <param name="Signature">The signature.</param>
        /// <exception cref="System.Exception">Signature is already in the dictionnary.</exception>
        public void Add(Signature Signature)
        {
            if (Signature == null)
            {
                throw new ArgumentNullException(nameof(Signature));
            }

            if (this.Dictionnary.ContainsKey(Signature.Name))
            {
                throw new Exception("Signature is already in the dictionnary.");
            }

            this.Dictionnary.Add(Signature.Name, Signature);
        }

        /// <summary>
        /// Gets the specified signature.
        /// </summary>
        /// <param name="Name">The name of the signature.</param>
        /// <exception cref="System.Exception">Signature is already in the dictionnary.</exception>
        public Signature Get(string Name)
        {
            if (string.IsNullOrEmpty(Name))
            {
                throw new ArgumentNullException(nameof(Name));
            }

            if (!this.Dictionnary.ContainsKey(Name))
            {
                throw new Exception("Signature is not in the dictionnary.");
            }

            return this.Dictionnary[Name];
        }

        /// <summary>
        /// Removes the specified signature.
        /// </summary>
        /// <param name="Signature">The signature.</param>
        /// <exception cref="System.Exception">Signature is already in the dictionnary.</exception>
        public void Remove(Signature Signature)
        {
            if (Signature == null)
            {
                throw new ArgumentNullException(nameof(Signature));
            }

            this.Remove(Signature.Name);
        }

        /// <summary>
        /// Removes the specified signature.
        /// </summary>
        /// <param name="Name">The name of the signature.</param>
        /// <exception cref="System.Exception">Signature is already in the dictionnary.</exception>
        public void Remove(string Name)
        {
            if (string.IsNullOrEmpty(Name))
            {
                throw new ArgumentNullException(nameof(Name));
            }

            if (!this.Dictionnary.ContainsKey(Name))
            {
                throw new Exception("Signature is not in the dictionnary.");
            }

            this.Dictionnary.Remove(Name);
        }
    }
}
