namespace ABeko.Helpers
{
    using System;
    using System.Linq;

    internal static class HexExtensions
    {
        /// <summary>
        /// Converts an hex string to a byte array.
        /// </summary>
        /// <param name="Hex">The hexadecimal string.</param>
        internal static byte[] ToByteArray(this string Hex)
        {
            return Enumerable.Range(0, Hex.Length).Where(x => x % 2 == 0).Select(x => Convert.ToByte(Hex.Substring(x, 2), 16)).ToArray();
        }

        /// <summary>
        /// Determines if the given string is a valid hex string.
        /// </summary>
        /// <param name="Hex">The hexadecimal string.</param>
        internal static bool IsValidHex(this string Hex)
        {
            if (Hex.All("0123456789abcdefABCDEF".Contains))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Trims the specified hexadecimal string.
        /// </summary>
        /// <param name="Hex">The hexadecimal.</param>
        internal static void TrimHex(this string Hex)
        {
            Hex = Hex.Replace("-", "").Replace("0x", "").Replace(",", "").Trim();
        }
    }
}