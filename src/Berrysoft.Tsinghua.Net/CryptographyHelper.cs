using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <remarks>The <paramref name="input"/> string is encoded into <see cref="byte"/> array by UTF-8.</remarks>
        public static string GetSHA1(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] data = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                return GetHexString(data);
            }
        }
        /// <summary>
        /// Encode a <see cref="string"/> to its UTF-8 form.
        /// </summary>
        /// <param name="a">String to be encoded.</param>
        /// <param name="b">Whether to add the length of the string in the end.</param>
        /// <returns>A <see cref="uint"/> array contains encoded string.</returns>
        private static unsafe uint[] S(string a, bool b)
        {
            int c = a.Length;
            int n = c / 4;
            n += c % 4 != 0 ? 1 : 0;
            //Array is 30 times faster than stack array and Encoding.GetBytes().
            uint[] v;
            if (b)
            {
                v = new uint[n + 1];
                v[n] = (uint)c;
            }
            else
            {
                v = new uint[n >= 4 ? n : 4];
            }
            fixed (uint* pv = v)
            {
                byte* pb = (byte*)pv;
                for (int i = 0; i < c; i++)
                {
                    pb[i] = (byte)a[i];
                }
            }
            return v;
        }
        /// <summary>
        /// Decode a <see cref="string"/> from its UTF-8 form.
        /// </summary>
        /// <param name="a">A <see cref="uint"/> array contains the encoded string.</param>
        /// <param name="b">Whether the length of the original string is in the end.</param>
        /// <returns>Decoded string.</returns>
        private static unsafe string L(uint[] a, bool b)
        {
            int d = a.Length;
            uint c = ((uint)(d - 1)) << 2;
            if (b)
            {
                uint m = a[d - 1];
                if (m < c - 3 || m > c)
                {
                    return null;
                }
                c = m;
            }
            fixed (uint* pa = a)
            {
                byte* pb = (byte*)pa;
                int n = d << 2;
                //When the return string needs subtracted, stack array is a little faster than array;
                //otherwise, array is 1.2 times faster than stack array.
                char[] aa = new char[n];
                for (int i = 0; i < n; i++)
                {
                    aa[i] = (char)pb[i];
                }
                if (b)
                {
                    return new string(aa, 0, (int)c);
                }
                else
                {
                    return new string(aa);
                }
            }
        }
        /// <summary>
        /// Encode a string by a special TEA algorithm.
        /// </summary>
        /// <param name="str">String to be encoded.</param>
        /// <param name="key">Key to encode.</param>
        /// <returns>Encoded string.</returns>
        public static string XEncode(string str, string key)
        {
            if (str.Length == 0)
            {
                return string.Empty;
            }
            uint[] v = S(str, true);
            uint[] k = S(key, false);
            int n = v.Length - 1;
            uint z = v[n];
            uint y = v[0];
            int q = 6 + 52 / (n + 1);
            uint d = 0;
            while (q-- > 0)
            {
                d += 0x9E3779B9;
                uint e = (d >> 2) & 3;
                for (int p = 0; p <= n; p++)
                {
                    y = v[p == n ? 0 : p + 1];
                    uint m = (z >> 5) ^ (y << 2);
                    m += (y >> 3) ^ (z << 4) ^ (d ^ y);
                    m += k[(p & 3) ^ (int)e] ^ z;
                    z = v[p] += m;
                }
            }
            return L(v, false);
        }

        private static readonly string Base64N = "LVoJPiCN2R8G90yg+hmFHuacZ1OWMnrsSTXkYpUq/3dlbfKwv6xztjI7DeBE45QA";
        /// <summary>
        /// Encode a string to base64 in a special way.
        /// </summary>
        /// <param name="t">String to be encoded.</param>
        /// <returns>Encoded string.</returns>
        public unsafe static string Base64Encode(string t)
        {
            int a = t.Length;
            int len = a / 3 * 4;
            len += a % 3 != 0 ? 4 : 0;
            //Stack array is 30 times faster than array, StringBuilder and Converter.ToBase64String().
            char* u = stackalloc char[len];
            char r = '=';
            int h = 0;
            byte* p = (byte*)&h;
            int ui = 0;
            for (int o = 0; o < a; o += 3)
            {
                p[2] = (byte)t[o];
                p[1] = (byte)(o + 1 < a ? t[o + 1] : 0);
                p[0] = (byte)(o + 2 < a ? t[o + 2] : 0);
                for (int i = 0; i < 4; i += 1)
                {
                    if (o * 8 + i * 6 > a * 8)
                    {
                        u[ui++] = r;
                    }
                    else
                    {
                        u[ui++] = Base64N[h >> 6 * (3 - i) & 0x3F];
                    }
                }
            }
            return new string(u, 0, len);
        }

        private static unsafe uint[] RStr2Binl(string input)
        {
            int len = input.Length;
            uint[] output = new uint[(len + 3) / 4];
            fixed (uint* pv = output)
            {
                byte* pb = (byte*)pv;
                for (int i = 0; i < len; i++)
                {
                    pb[i] = (byte)input[i];
                }
            }
            return output;
        }

        private static unsafe string Binl2RStr(uint[] input)
        {
            int l = input.Length * 4;
            fixed (uint* pa = input)
            {
                byte* pb = (byte*)pa;
                char[] aa = new char[l];
                for (int i = 0; i < l; i++)
                {
                    aa[i] = (char)pb[i];
                }
                return new string(aa);
            }
        }

        private static int BitRol(uint num, int cnt) { return (int)((num << cnt) | (num >> (32 - cnt))); }
        private static int MD5Cmn(int q, int a, int b, int x, int s, int t) { return BitRol((uint)(a + q + x + t), s) + b; }
        private static int MD5ff(int a, int b, int c, int d, uint x, int s, int t) { return MD5Cmn((b & c) | ((~b) & d), a, b, (int)x, s, t); }
        private static int MD5gg(int a, int b, int c, int d, uint x, int s, int t) { return MD5Cmn((b & d) | (c & (~d)), a, b, (int)x, s, t); }
        private static int MD5hh(int a, int b, int c, int d, uint x, int s, int t) { return MD5Cmn(b ^ c ^ d, a, b, (int)x, s, t); }
        private static int MD5ii(int a, int b, int c, int d, uint x, int s, int t) { return MD5Cmn(c ^ (b | (~d)), a, b, (int)x, s, t); }

        private static uint[] Binl(List<uint> x, int len)
        {
            int a = 1732584193, b = -271733879, c = -1732584194, d = 271733878;
            int index = len >> 5;
            while (x.Count <= index)
                x.Add(0);
            x[index] |= (uint)(0x80 << (len % 32));
            index = (((len + 64) >> 9) << 4) + 14;
            while (x.Count < index)
                x.Add(0);
            x.Add((uint)len);
            while (x.Count % 16 != 0)
                x.Add(0);
            for (int i = 0; i < x.Count; i += 16)
            {
                int olda = a, oldb = b, oldc = c, oldd = d;

                a = MD5ff(a, b, c, d, x[i + 0], 7, -680876936);
                d = MD5ff(d, a, b, c, x[i + 1], 12, -389564586);
                c = MD5ff(c, d, a, b, x[i + 2], 17, 606105819);
                b = MD5ff(b, c, d, a, x[i + 3], 22, -1044525330);
                a = MD5ff(a, b, c, d, x[i + 4], 7, -176418897);
                d = MD5ff(d, a, b, c, x[i + 5], 12, 1200080426);
                c = MD5ff(c, d, a, b, x[i + 6], 17, -1473231341);
                b = MD5ff(b, c, d, a, x[i + 7], 22, -45705983);
                a = MD5ff(a, b, c, d, x[i + 8], 7, 1770035416);
                d = MD5ff(d, a, b, c, x[i + 9], 12, -1958414417);
                c = MD5ff(c, d, a, b, x[i + 10], 17, -42063);
                b = MD5ff(b, c, d, a, x[i + 11], 22, -1990404162);
                a = MD5ff(a, b, c, d, x[i + 12], 7, 1804603682);
                d = MD5ff(d, a, b, c, x[i + 13], 12, -40341101);
                c = MD5ff(c, d, a, b, x[i + 14], 17, -1502002290);
                b = MD5ff(b, c, d, a, x[i + 15], 22, 1236535329);

                a = MD5gg(a, b, c, d, x[i + 1], 5, -165796510);
                d = MD5gg(d, a, b, c, x[i + 6], 9, -1069501632);
                c = MD5gg(c, d, a, b, x[i + 11], 14, 643717713);
                b = MD5gg(b, c, d, a, x[i + 0], 20, -373897302);
                a = MD5gg(a, b, c, d, x[i + 5], 5, -701558691);
                d = MD5gg(d, a, b, c, x[i + 10], 9, 38016083);
                c = MD5gg(c, d, a, b, x[i + 15], 14, -660478335);
                b = MD5gg(b, c, d, a, x[i + 4], 20, -405537848);
                a = MD5gg(a, b, c, d, x[i + 9], 5, 568446438);
                d = MD5gg(d, a, b, c, x[i + 14], 9, -1019803690);
                c = MD5gg(c, d, a, b, x[i + 3], 14, -187363961);
                b = MD5gg(b, c, d, a, x[i + 8], 20, 1163531501);
                a = MD5gg(a, b, c, d, x[i + 13], 5, -1444681467);
                d = MD5gg(d, a, b, c, x[i + 2], 9, -51403784);
                c = MD5gg(c, d, a, b, x[i + 7], 14, 1735328473);
                b = MD5gg(b, c, d, a, x[i + 12], 20, -1926607734);

                a = MD5hh(a, b, c, d, x[i + 5], 4, -378558);
                d = MD5hh(d, a, b, c, x[i + 8], 11, -2022574463);
                c = MD5hh(c, d, a, b, x[i + 11], 16, 1839030562);
                b = MD5hh(b, c, d, a, x[i + 14], 23, -35309556);
                a = MD5hh(a, b, c, d, x[i + 1], 4, -1530992060);
                d = MD5hh(d, a, b, c, x[i + 4], 11, 1272893353);
                c = MD5hh(c, d, a, b, x[i + 7], 16, -155497632);
                b = MD5hh(b, c, d, a, x[i + 10], 23, -1094730640);
                a = MD5hh(a, b, c, d, x[i + 13], 4, 681279174);
                d = MD5hh(d, a, b, c, x[i + 0], 11, -358537222);
                c = MD5hh(c, d, a, b, x[i + 3], 16, -722521979);
                b = MD5hh(b, c, d, a, x[i + 6], 23, 76029189);
                a = MD5hh(a, b, c, d, x[i + 9], 4, -640364487);
                d = MD5hh(d, a, b, c, x[i + 12], 11, -421815835);
                c = MD5hh(c, d, a, b, x[i + 15], 16, 530742520);
                b = MD5hh(b, c, d, a, x[i + 2], 23, -995338651);

                a = MD5ii(a, b, c, d, x[i + 0], 6, -198630844);
                d = MD5ii(d, a, b, c, x[i + 7], 10, 1126891415);
                c = MD5ii(c, d, a, b, x[i + 14], 15, -1416354905);
                b = MD5ii(b, c, d, a, x[i + 5], 21, -57434055);
                a = MD5ii(a, b, c, d, x[i + 12], 6, 1700485571);
                d = MD5ii(d, a, b, c, x[i + 3], 10, -1894986606);
                c = MD5ii(c, d, a, b, x[i + 10], 15, -1051523);
                b = MD5ii(b, c, d, a, x[i + 1], 21, -2054922799);
                a = MD5ii(a, b, c, d, x[i + 8], 6, 1873313359);
                d = MD5ii(d, a, b, c, x[i + 15], 10, -30611744);
                c = MD5ii(c, d, a, b, x[i + 6], 15, -1560198380);
                b = MD5ii(b, c, d, a, x[i + 13], 21, 1309151649);
                a = MD5ii(a, b, c, d, x[i + 4], 6, -145523070);
                d = MD5ii(d, a, b, c, x[i + 11], 10, -1120210379);
                c = MD5ii(c, d, a, b, x[i + 2], 15, 718787259);
                b = MD5ii(b, c, d, a, x[i + 9], 21, -343485551);

                a += olda;
                b += oldb;
                c += oldc;
                d += oldd;
            }
            return new uint[] { (uint)a, (uint)b, (uint)c, (uint)d };
        }

        public static string GetHMACMD5(string key)
        {
            var bkey = RStr2Binl(key);
            uint[] ipad = new uint[16];
            uint[] opad = new uint[16];
            for (int i = 0; i < 16; i++)
            {
                ipad[i] = bkey[i] ^ 0x36363636;
                opad[i] = bkey[i] ^ 0x5C5C5C5C;
            }
            var hash = Binl(ipad.ToList(), 512);
            string result = Binl2RStr(Binl(opad.Concat(hash).ToList(), 512 + 128));
            return GetHexString(Encoding.UTF8.GetBytes(result));
        }
    }
}
