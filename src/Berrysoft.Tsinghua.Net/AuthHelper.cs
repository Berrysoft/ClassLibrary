using System;
using System.Collections.Generic;
using System.Json;
using System.Text;
using System.Threading.Tasks;

namespace Berrysoft.Tsinghua.Net
{
    /// <summary>
    /// Exposes methods to login, logout and get flux from https://auth4.tsinghua.edu.cn/ or https://auth6.tsinghua.edu.cn/
    /// </summary>
    public class AuthHelper : NetHelperBase, IConnect
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
        private AuthHelper(int version)
            : this(string.Empty, string.Empty, version)
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthHelper"/> class.
        /// </summary>
        /// <param name="username">The username to login.</param>
        /// <param name="password">The password to login.</param>
        /// <param name="version">4 for auth4 and 6 for auth6</param>
        private AuthHelper(string username, string password, int version)
            : base(username, password)
        {
            LogUri = string.Format(LogUriBase, version);
            FluxUri = string.Format(FluxUriBase, version);
            ChallengeUri = string.Format(ChallengeUriBase, version);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthHelper"/> class.
        /// </summary>
        /// <returns>An instance of <see cref="AuthHelper"/> class with version 4.</returns>
        public static AuthHelper CreateAuth4Helper() => new AuthHelper(4);
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthHelper"/> class.
        /// </summary>
        /// <param name="username">The username to login.</param>
        /// <param name="password">The password to login.</param>
        /// <returns>An instance of <see cref="AuthHelper"/> class with version 4.</returns>
        public static AuthHelper CreateAuth4Helper(string username, string password) => new AuthHelper(username, password, 4);
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthHelper"/> class.
        /// </summary>
        /// <returns>An instance of <see cref="AuthHelper"/> class with version 6.</returns>
        public static AuthHelper CreateAuth6Helper() => new AuthHelper(6);
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthHelper"/> class.
        /// </summary>
        /// <param name="username">The username to login.</param>
        /// <param name="password">The password to login.</param>
        /// <returns>An instance of <see cref="AuthHelper"/> class with version 6.</returns>
        public static AuthHelper CreateAuth6Helper(string username, string password) => new AuthHelper(username, password, 6);
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
        /// <summary>
        /// Get "challenge" to encode the password.
        /// </summary>
        /// <returns>The content of the website.</returns>
        private async Task<string> GetChallengeAsync()
        {
            string result = await GetAsync(string.Format(ChallengeUri, Username));
            int begin = result.IndexOf('{');
            int end = result.LastIndexOf('}');
            return result.Substring(begin, end - begin + 1);
        }
        private Dictionary<string, string> loginDataDictionary;
        private JsonObject loginInfo;
        /// <summary>
        /// Get login data with username, password and "challenge".
        /// </summary>
        /// <returns>A dictionary contains the data.</returns>
        /// <remarks>
        /// <code><![CDATA[
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
            const string n = "200";
            const string type = "1";
            const string acid = "1";
            const string ip = "";
            const string passwordMD5 = "5e543256c480ac577d30f76f9120eb74";
            if (loginInfo==null)
            {
                loginInfo = new JsonObject()
                {
                    ["ip"] = ip,
                    ["acid"] = acid,
                    ["enc_ver"] = "srun_bx1"
                };
            }
            loginInfo["username"] = Username;
            loginInfo["password"] = Password;
            string challenge = await GetChallengeAsync();
            JsonValue result = JsonValue.Parse(challenge);
            string token = result["challenge"];
            if (loginDataDictionary == null)
            {
                loginDataDictionary = new Dictionary<string, string>
                {
                    ["action"] = "login",
                    ["ac_id"] = acid,
                    ["double_stack"] = "1",
                    ["n"] = n,
                    ["type"] = type,
                    ["password"] = "{MD5}" + passwordMD5
                };
            }
            loginDataDictionary["info"] = "{SRBX1}" + Base64Encode(Encode(loginInfo.ToString(), token));
            loginDataDictionary["username"] = Username;
            loginDataDictionary["chksum"] = CryptographyHelper.GetSHA1(token + Username + token + passwordMD5 + token + acid + token + ip + token + n + token + type + token + loginDataDictionary["info"]);
            return loginDataDictionary;
        }
        /// <summary>
        /// A function translated from javascript.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        /// <remarks>
        /// <code><![CDATA[
        /// function s(a, b) {
        ///    var c = a.length,
        ///    v = [];
        ///    for (var i = 0; i<c; i += 4) {
        ///        v[i >> 2] = a.charCodeAt(i) | a.charCodeAt(i + 1) << 8 | a.charCodeAt(i + 2) << 16 | a.charCodeAt(i + 3) << 24;
        ///    }
        ///    if (b) {
        ///        v[v.length] = c;
        ///    }
        ///    return v;
        /// }
        /// ]]></code>
        /// </remarks>
        private static unsafe List<uint> S(string a, bool b)
        {
            int c = a.Length;
            List<uint> v = new List<uint>();
            uint value = 0;
            byte* p = (byte*)&value;
            for (int i = 0; i < c; i += 4)
            {
                p[0] = (byte)a[i];
                p[1] = (byte)(i + 1 >= c ? 0 : a[i + 1]);
                p[2] = (byte)(i + 2 >= c ? 0 : a[i + 2]);
                p[3] = (byte)(i + 3 >= c ? 0 : a[i + 3]);
                v.Add(value);
            }
            if (b)
            {
                v.Add((uint)c);
            }
            return v;
        }
        /// <summary>
        /// A function translated from javascript.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        /// <remarks>
        /// <code><![CDATA[
        /// function l(a, b) {
        /// var d = a.length,
        ///     c = (d - 1) << 2;
        ///     if (b) {
        ///         var m = a[d - 1];
        ///         if ((m<c - 3) || (m > c))
        ///             return null;
        ///         c = m;
        ///     }
        ///     for (var i = 0; i<d; i++) {
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
        private static unsafe string L(List<uint> a, bool b)
        {
            int d = a.Count;
            uint c = ((uint)(d - 1)) << 2;
            StringBuilder aa = new StringBuilder();
            if (b)
            {
                uint m = a[d - 1];
                if (m < c - 3 || m > c)
                {
                    return null;
                }
                c = m;
            }
            uint value = 0;
            byte* p = (byte*)&value;
            for (int i = 0; i < d; i++)
            {
                value = a[i];
                aa.Append((char)p[0]);
                aa.Append((char)p[1]);
                aa.Append((char)p[2]);
                aa.Append((char)p[3]);
            }
            if (b)
            {
                return aa.ToString().Substring(0, (int)c);
            }
            else
            {
                return aa.ToString();
            }
        }
        /// <summary>
        /// A function translated from javascript.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <remarks>
        /// <code><![CDATA[
        /// xEncode: function(str, key) {
        ///     if (str == "") {
        ///         return "";
        ///     }
        ///     var v = s(str, true),
        ///         k = s(key, false);
        ///     if (k.length< 4) {
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
        private static string Encode(string str, string key)
        {
            if (str.Length == 0)
            {
                return String.Empty;
            }
            List<uint> v = S(str, true);
            List<uint> k = S(key, false);
            while (k.Count < 4)
            {
                k.Add(0);
            }
            int n = v.Count - 1;
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
        /// A function translated from javascript.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        /// <remarks>
        /// <code><![CDATA[
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
            StringBuilder u = new StringBuilder();
            int a = t.Length;
            char r = '=';
            int h = 0;
            byte* p = (byte*)&h;
            for (int o = 0; o < a; o += 3)
            {
                p[2] = (byte)t[o];
                p[1] = (byte)(o + 1 < a ? t[o + 1] : 0);
                p[0] = (byte)(o + 2 < a ? t[o + 2] : 0);
                for (int i = 0; i < 4; i += 1)
                {
                    if (o * 8 + i * 6 > a * 8)
                    {
                        u.Append(r);
                    }
                    else
                    {
                        u.Append(n[h >> 6 * (3 - i) & 0x3F]);
                    }
                }
            }
            return u.ToString();
        }
    }
}
