using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Berrysoft.Tsinghua.Net
{
    /// <summary>
    /// Exposes methods to login, logout and get flux from https://auth4.tsinghua.edu.cn/ or https://auth6.tsinghua.edu.cn/
    /// </summary>
    public abstract class AuthHelper : NetHelperBase, IConnect
    {
        private const string LogUriBase = "https://auth{0}.tsinghua.edu.cn/cgi-bin/srun_portal";
        private const string FluxUriBase = "https://auth{0}.tsinghua.edu.cn/rad_user_info.php";
        private const string ChallengeUriBase = "https://auth{0}.tsinghua.edu.cn/cgi-bin/get_challenge?username={{0}}&double_stack=1&ip&callback=callback";
        private const string LogoutData = "action=logout&ac_id=1&ip=&double_stack=1";
        private readonly string LogUri;
        private readonly string FluxUri;
        private readonly string ChallengeUri;
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthHelper"/> class.
        /// </summary>
        /// <param name="version">4 for auth4 and 6 for auth6</param>
        internal AuthHelper(int version)
            : this(string.Empty, string.Empty, version)
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthHelper"/> class.
        /// </summary>
        /// <param name="client">A user-specified instance of <see cref="HttpClient"/>.</param>
        /// <param name="version">4 for auth4 and 6 for auth6</param>
        internal AuthHelper(HttpClient client, int version)
            : this(string.Empty, string.Empty, client, version)
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthHelper"/> class.
        /// </summary>
        /// <param name="username">The username to login.</param>
        /// <param name="password">The password to login.</param>
        /// <param name="version">4 for auth4 and 6 for auth6</param>
        internal AuthHelper(string username, string password, int version)
            : this(username, password, null, version)
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthHelper"/> class.
        /// </summary>
        /// <param name="username">The username to login.</param>
        /// <param name="password">The password to login.</param>
        /// <param name="client">A user-specified instance of <see cref="HttpClient"/>.</param>
        /// <param name="version">4 for auth4 and 6 for auth6</param>
        internal AuthHelper(string username, string password, HttpClient client, int version)
            : base(username, password, client)
        {
            LogUri = string.Format(LogUriBase, version);
            FluxUri = string.Format(FluxUriBase, version);
            ChallengeUri = string.Format(ChallengeUriBase, version);
        }
        /// <summary>
        /// Login to the network.
        /// </summary>
        /// <returns>The response of the website.</returns>
        public async Task<string> LoginAsync() => await PostAsync(LogUri, await GetLoginDataAsync());
        /// <summary>
        /// Logout from the network.
        /// </summary>
        /// <returns>The response of the website.</returns>
        public Task<string> LogoutAsync() => PostAsync(LogUri, LogoutData);
        /// <summary>
        /// Get information of the user online.
        /// </summary>
        /// <returns>An instance of <see cref="FluxUser"/> class of the current user.</returns>
        public async Task<FluxUser> GetFluxAsync() => FluxUser.Parse(await PostAsync(FluxUri));

        private static readonly Regex ChallengeRegex = new Regex(@"""challenge"":""(.*?)""");
        /// <summary>
        /// Get "challenge" to encode the password.
        /// </summary>
        /// <returns>The content of the website.</returns>
        private async Task<string> GetChallengeAsync()
        {
            string result = await GetAsync(string.Format(ChallengeUri, Username));
            Match match = ChallengeRegex.Match(result);
            return match.Groups[1].Value;
        }

        private Dictionary<string, string> loginDataDictionary;
        private const string LoginInfoJson = "{{\"ip\": \"\", \"acid\": \"1\", \"enc_ver\": \"srun_bx1\", \"username\": \"{0}\", \"password\": \"{1}\"}}";
        private const string ChkSumData = "{0}{1}{0}{2}{0}1{0}{0}200{0}1{0}{3}";
        /// <summary>
        /// Get login data with username, password and "challenge".
        /// </summary>
        /// <returns>A dictionary contains the data.</returns>
        /// <remarks>
        /// This is a function translated from javascript.
        /// <code language="JavaScript"><![CDATA[
        /// jQuery.getJSON(url.replace("srun_portal", "get_challenge"), { "username": $data.username, "ip": $data.ip, "double_stack": "1" }, function(data) {
        ///     var token = "";
        ///                 if (data.res != "ok") {
        ///                     alert(data.error);
        ///                     return;
        ///                 }
        ///     token = data.challenge;
        ///     $data.info = "{SRBX1}" + base64.encode(jQuery.xEncode(JSON.stringify({ "username": $data.username, "password": $data.password, "ip": $data.ip, "acid": $data.ac_id, "enc_ver": enc}), token));
        ///     var hmd5 = new Hashes.MD5().hex_hmac(token, data.password);
        ///     $data.password = "{MD5}" + hmd5;
        ///     $data.chksum = new Hashes.SHA1().hex(token + $data.username + token + hmd5 + token + $data.ac_id + token + $data.ip + token + n + token + type + token + $data.info);
        ///     $data.n = n;
        ///     $data.type = type;
        ///     return get(url, $data, callback, "jsonp");
        /// });
        /// ]]></code>
        /// </remarks>
        private async Task<Dictionary<string, string>> GetLoginDataAsync()
        {
            const string passwordMD5 = "5e543256c480ac577d30f76f9120eb74";
            string token = await GetChallengeAsync();
            if (loginDataDictionary == null)
            {
                loginDataDictionary = new Dictionary<string, string>
                {
                    ["action"] = "login",
                    ["ac_id"] = "1",
                    ["double_stack"] = "1",
                    ["n"] = "200",
                    ["type"] = "1",
                    ["password"] = "{MD5}" + passwordMD5
                };
            }
            string loginInfo = string.Format(LoginInfoJson, Username, Password);
            loginDataDictionary["info"] = "{SRBX1}" + Base64Encode(XEncode(loginInfo, token));
            loginDataDictionary["username"] = Username;
            loginDataDictionary["chksum"] = CryptographyHelper.GetSHA1(string.Format(ChkSumData, token, Username, passwordMD5, loginDataDictionary["info"]));
            return loginDataDictionary;
        }
        #region Encode methods
        /// <summary>
        /// Encode a <see cref="string"/> to its UTF-8 form.
        /// </summary>
        /// <param name="a">String to be encoded.</param>
        /// <param name="b">Whether to add the length of the string in the end.</param>
        /// <returns>A <see cref="uint"/> array contains encoded string.</returns>
        /// <remarks>
        /// This is a function translated from javascript.
        /// <code language="JavaScript"><![CDATA[
        /// function s(a, b) {
        ///    var c = a.length,
        ///    v = [];
        ///    for (var i = 0; i < c; i += 4) {
        ///        v[i >> 2] = a.charCodeAt(i) | a.charCodeAt(i + 1) << 8 | a.charCodeAt(i + 2) << 16 | a.charCodeAt(i + 3) << 24;
        ///    }
        ///    if (b) {
        ///        v[v.length] = c;
        ///    }
        ///    return v;
        /// }
        /// ]]></code>
        /// </remarks>
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
        /// <remarks>
        /// This is a function translated from javascript.
        /// <code language="JavaScript"><![CDATA[
        /// function l(a, b) {
        /// var d = a.length,
        ///     c = (d - 1) << 2;
        ///     if (b) {
        ///         var m = a[d - 1];
        ///         if ((m<c - 3) || (m > c))
        ///             return null;
        ///         c = m;
        ///     }
        ///     for (var i = 0; i < d; i++) {
        ///         a[i] = String.fromCharCode(a[i] & 0xff, a[i] >>> 8 & 0xff, a[i] >>> 16 & 0xff, a[i] >>> 24 & 0xff);
        ///     }
        ///     if (b) {
        ///         return a.join('').substring(0, c);
        ///     } else {
        ///         return a.join('');
        ///     }
        /// }
        /// ]]></code>
        /// </remarks>
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
        /// <remarks>
        /// This is a function translated from javascript.
        /// <code language="JavaScript"><![CDATA[
        /// xEncode: function(str, key) {
        ///     if (str == "") {
        ///         return "";
        ///     }
        ///     var v = s(str, true),
        ///         k = s(key, false);
        ///     if (k.length < 4) {
        ///         k.length = 4;
        ///     }
        ///     var n = v.length - 1,
        ///         z = v[n],
        ///         y = v[0],
        ///         c = 0x86014019 | 0x183639A0,
        ///         m,
        ///         e,
        ///         p,
        ///         q = Math.floor(6 + 52 / (n + 1)),
        ///         d = 0;
        ///     while (0 < q--) {
        ///         d = d + c & (0x8CE0D9BF | 0x731F2640);
        ///         e = d >>> 2 & 3;
        ///         for (p = 0; p<n; p++) {
        ///             y = v[p + 1];
        ///             m = z >>> 5 ^ y << 2;
        ///             m += (y >>> 3 ^ z << 4) ^ (d ^ y);
        ///             m += k[(p & 3) ^ e] ^ z;
        ///             z = v[p] = v[p] + m & (0xEFB8D130 | 0x10472ECF);
        ///         }
        ///         y = v[0];
        ///         m = z >>> 5 ^ y << 2;
        ///         m += (y >>> 3 ^ z << 4) ^ (d ^ y);
        ///         m += k[(p & 3) ^ e] ^ z;
        ///         z = v[n] = v[n] + m & (0xBB390742 | 0x44C6F8BD);
        ///     }
        ///     return l(v, false);
        /// }
        /// ]]></code>
        /// </remarks>
        private static string XEncode(string str, string key)
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
        /// <summary>
        /// Encode a string to base64 in a special way.
        /// </summary>
        /// <param name="t">String to be encoded.</param>
        /// <returns>Encoded string.</returns>
        /// <remarks>
        /// This is a function translated from javascript.
        /// <code language="JavaScript"><![CDATA[
        /// Base64: function() {
        ///     var n = "LVoJPiCN2R8G90yg+hmFHuacZ1OWMnrsSTXkYpUq/3dlbfKwv6xztjI7DeBE45QA",
        ///         r = "=",
        ///         o = false,
        ///         f = false;
        ///     this.encode = function(t) {
        ///         var o,
        ///             i,
        ///             h,
        ///             u = "",
        ///             a = t.length;
        ///         r = r || "=";
        ///         t = f ? e(t) : t;
        ///         for (o = 0; o < a; o += 3) {
        ///             h = t.charCodeAt(o) << 16 | (o + 1 < a ? t.charCodeAt(o+1) << 8 : 0) | (o + 2 < a ? t.charCodeAt(o+2) : 0);
        ///             for ( i = 0; i < 4; i += 1) {
        ///                 if (o * 8 + i * 6 > a * 8) {
        ///                     u += r
        ///                 } else {
        ///                     u += n.charAt(h >>> 6 * (3 - i) & 63)
        ///                 }
        ///             }
        ///         }
        ///         return u
        ///     };
        /// }
        /// ]]></code>
        /// </remarks>
        private unsafe static string Base64Encode(string t)
        {
            string n = "LVoJPiCN2R8G90yg+hmFHuacZ1OWMnrsSTXkYpUq/3dlbfKwv6xztjI7DeBE45QA";
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
                        u[ui++] = n[h >> 6 * (3 - i) & 0x3F];
                    }
                }
            }
            return new string(u, 0, len);
        }
        #endregion
    }
    /// <summary>
    /// Exposes methods to login, logout and get flux from https://auth4.tsinghua.edu.cn/.
    /// </summary>
    public class Auth4Helper : AuthHelper
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Auth4Helper"/> class.
        /// </summary>
        public Auth4Helper()
            : base(4)
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Auth4Helper"/> class.
        /// </summary>
        /// <param name="client">A user-specified instance of <see cref="HttpClient"/>.</param>
        public Auth4Helper(HttpClient client)
            : base(client, 4)
        { }
        /// <summary>
        /// Initializes a new instance of <see cref="Auth4Helper"/> class.
        /// </summary>
        /// <param name="username">The username to login.</param>
        /// <param name="password">The password to login.</param>
        public Auth4Helper(string username, string password)
            : base(username, password, 4)
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Auth4Helper"/> class.
        /// </summary>
        /// <param name="username">The username to login.</param>
        /// <param name="password">The password to login.</param>
        /// <param name="client">A user-specified instance of <see cref="HttpClient"/>.</param>
        public Auth4Helper(string username, string password, HttpClient client)
            : base(username, password, client, 4)
        { }
    }
    /// <summary>
    /// Exposes methods to login, logout and get flux from https://auth6.tsinghua.edu.cn/.
    /// </summary>
    public class Auth6Helper : AuthHelper
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Auth6Helper"/> class.
        /// </summary>
        public Auth6Helper()
            : base(6)
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Auth6Helper"/> class.
        /// </summary>
        /// <param name="client">A user-specified instance of <see cref="HttpClient"/>.</param>
        public Auth6Helper(HttpClient client)
            : base(client, 6)
        { }
        /// <summary>
        /// Initializes a new instance of <see cref="Auth6Helper"/> class.
        /// </summary>
        /// <param name="username">The username to login.</param>
        /// <param name="password">The password to login.</param>
        public Auth6Helper(string username, string password)
            : base(username, password, 6)
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Auth6Helper"/> class.
        /// </summary>
        /// <param name="username">The username to login.</param>
        /// <param name="password">The password to login.</param>
        /// <param name="client">A user-specified instance of <see cref="HttpClient"/>.</param>
        public Auth6Helper(string username, string password, HttpClient client)
            : base(username, password, client, 6)
        { }
    }
}
