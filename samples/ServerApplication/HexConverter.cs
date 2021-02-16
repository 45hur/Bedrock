// ***********************************************************************
// Assembly         : FineProtocol.Common
// Author           : Stephen.Wang
// Created          : 10-17-2013
//
// Last Modified By : Stephen.Wang
// Last Modified On : 03-26-2020
// ***********************************************************************
// <copyright file="HexConverter.cs" company="Aconbio">
//     Copyright (c) Aconbio. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Text;

/// <summary>
/// The Converter namespace.
/// </summary>
namespace ServerApplication
{
    /// <summary>
    /// Class HexConverter.
    /// </summary>
    public class HexConverter
    {
        /// <summary>
        /// Bytes the to hexadecimal string.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns>System.String.</returns>
        public string ByteToHexString(byte[] buffer)
        {
            string hexString = ByteToHexString(buffer, false);
            return hexString;
        }

        /// <summary>
        /// Bytes to hexadecimal string.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="isInverted">if set to <c>true</c> [is inverted].</param>
        /// <returns>System.String.</returns>
        public string ByteToHexString(byte[] buffer, bool isInverted)
        {
            string hexString = ByteToHexString(buffer, isInverted, null);
            return hexString;
        }

        /// <summary>
        /// Bytes  to hexadecimal string.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="isInverted">if set to <c>true</c> [is inverted].</param>
        /// <param name="separator">The separator.</param>
        /// <returns>System.String.</returns>
        public string ByteToHexString(byte[] buffer, bool isInverted, params char[] separator)
        {
            int index = 0;
            int count = buffer.Length;
            string hexString = ByteToHexString(buffer, index, count, isInverted, separator);
            return hexString;
        }

        /// <summary>
        /// Bytes  to hexadecimal string.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        /// <param name="isInverted">if set to <c>true</c> [is inverted].</param>
        /// <param name="separator">The separator.</param>
        /// <returns>System.String.</returns>
        public string ByteToHexString(byte[] buffer, int index, int count, bool isInverted, params char[] separator)
        {
            if (buffer == null || buffer.Length < 1)
            {
                return string.Empty;
            }

            bool hasSeparator = separator != null && separator.Length > 0;
            int capacity = count * 2;
            if (hasSeparator)
            {
                capacity += (count - 1) * separator.Length;
            }
            StringBuilder builder = new StringBuilder(capacity, 2 * capacity);

            int pos = index;
            for (int i = 0; i < count; i++)
            {
                if (isInverted)
                {
                    pos = index + count - i - 1;
                }
                else
                {
                    pos = index + i;
                }

                builder.Append(buffer[pos].ToString("X2"));
                if (hasSeparator && i + 1 < count)
                {
                    builder.Append(separator);
                }

                pos++;
            }

            return builder.ToString();

        }


        /// <summary>
        /// Tries the convert to byte array.
        /// </summary>
        /// <param name="hexString">The hexadecimal string.</param>
        /// <param name="buffer">The buffer.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool TryConvertToByteArray(string hexString, out byte[] buffer)
        {
            return TryConvertToByteArray(hexString, false, out buffer);
        }

        /// <summary>
        /// Tries the convert to byte array.
        /// </summary>
        /// <param name="hexString">The hexadecimal string.</param>
        /// <param name="isInverted">if set to <c>true</c> [is inverted].</param>
        /// <param name="buffer">The buffer.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool TryConvertToByteArray(string hexString, bool isInverted, out byte[] buffer)
        {
            return TryConvertToByteArray(hexString, false, out buffer, null);
        }

        /// <summary>
        /// Tries the convert to byte array.
        /// </summary>
        /// <param name="hexString">The hexadecimal string.</param>
        /// <param name="isInverted">if set to <c>true</c> [is inverted].</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="separator">The separator.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool TryConvertToByteArray(string hexString, bool isInverted, out byte[] buffer, params char[] separator)
        {
            buffer = null;
            bool hasSeparator = separator != null && separator.Length > 0;
            if (hasSeparator)
            {
                foreach (char replaceChar in separator)
                {
                    hexString = hexString.Replace(replaceChar.ToString(), "");
                }
            }

            if (string.IsNullOrEmpty(hexString) || hexString.Length < 2 || hexString.Length % 2 > 0)
            {
                return false;
            }

            bool succeeded = false;

            int count = hexString.Length / 2;

            try
            {
                buffer = new byte[count];
                int pos = 0;
                for (int i = 0; i < count; i++)
                {
                    if (isInverted)
                    {
                        pos = count - (i + 1) * 2;
                    }
                    else
                    {
                        pos = i * 2;
                    }

                    string text = hexString.Substring(pos, 2);
                    byte hexNumber = byte.Parse(text, System.Globalization.NumberStyles.HexNumber);
                    buffer[i] = hexNumber;
                }
                succeeded = true;
            }
            catch
            {

            }

            return succeeded;

        }


    }
}
