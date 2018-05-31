using System;
using System.Security.Cryptography;
using System.Text;

namespace Berrysoft.Tsinghua.Net
{
    /// <summary>
    /// Methods of cryptography.
    /// </summary>
    internal static class CryptographyHelper
    {
        /// <summary>
        /// Get hex string of a byte array.
        /// </summary>
        /// <param name="data">A <see cref="byte"/> array contains data.</param>
        /// <returns>A hex string.</returns>
        private static string GetHexString(byte[] data)
        {
            //StringBuilder is 5 times faster than stack array and array.
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
        /// <summary>
        /// Get MD5 encoded string created by <see cref="MD5"/>.
        /// </summary>
        /// <param name="input">Original string.</param>
        /// <returns>An encoded string.</returns>
        /// <remarks>The <paramref name="input"/> string is encoded into <see cref="byte"/> array by UTF-8.</remarks>
        public static string GetMD5(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                return GetHexString(data);
            }
        }
        /// <summary>
        /// Get MD5 encoded string created by <see cref="SHA1"/>.
        /// </summary>
        /// <param name="input">Original string.</param>
        /// <returns>An encoded string.</returns>
        /// <remarks>The <paramref name="input"/> string is encoded into <see cref="byte"/> array by ISO-8859-1.</remarks>
        public static string GetSHA1(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            using (SHA1 sha1 = SHA1.Create())
            {
                const int ISO_8859_1 = 28591;
                byte[] data = sha1.ComputeHash(Encoding.GetEncoding(ISO_8859_1).GetBytes(input));
                return GetHexString(data);
            }
        }
    }
}
