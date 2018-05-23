using System;
using System.Collections.Generic;
using System.Json;
using System.Threading.Tasks;

namespace Berrysoft.Tsinghua.Net
{
    public class AuthHelper : NetHelperBase, IConnect
    {
        private const string LogUriBase = "https://auth{0}.tsinghua.edu.cn/cgi-bin/srun_portal";
        private const string FluxUriBase = "https://auth{0}.tsinghua.edu.cn/rad_user_info.php";
        private const string ChallengeUriBase = "https://auth{0}.tsinghua.edu.cn/cgi-bin/get_challenge";
        private const string LogoutData = "action=logout&ac_id=1&ip=&double_stack=1";
        private const string ChallengeData = "username={0}&double_stack=1&ip&callback=callback";
        private readonly string LogUri;
        private readonly string FluxUri;
        private readonly string ChallengeUri;
        private AuthHelper(int version)
            : this(string.Empty, string.Empty, version)
        { }
        private AuthHelper(string username, string password, int version)
            : base(username, password)
        {
            LogUri = string.Format(LogUriBase, version);
            FluxUri = string.Format(FluxUriBase, version);
            ChallengeUri = string.Format(ChallengeUriBase, version);
        }
        public static AuthHelper CreateAuth4Helper() => new AuthHelper(4);
        public static AuthHelper CreateAuth4Helper(string username, string password) => new AuthHelper(username, password, 4);
        public static AuthHelper CreateAuth6Helper() => new AuthHelper(6);
        public static AuthHelper CreateAuth6Helper(string username, string password) => new AuthHelper(username, password, 6);
        public async Task<string> LoginAsync()
        {
            const string n = "200";
            const string type = "1";
            const string passwordMD5 = "5e543256c480ac577d30f76f9120eb74";
            string challenge = await GetChallengeAsync();
            if (challenge == null) return null;
            JsonValue result = JsonValue.Parse(challenge);
            string token = result["challenge"];
            var info = new JsonObject()
            {
                ["username"] = Username,
                ["password"] = Password,
                ["ip"] = string.Empty,
                ["acid"] = "1",
                ["enc_ver"] = "srun_bx1"
            };
            var data = new Dictionary<string, string>
            {
                ["info"] = "{SRBX1}" + Base64Encode(Encode(info.ToString(), token)),
                ["action"] = "login",
                ["ac_id"] = "1",
                ["double_stack"] = "1",
                ["n"] = n,
                ["type"] = type,
                ["username"] = Username,
                ["password"] = "{MD5}" + passwordMD5
            };
            data["chksum"] = GetSHA1(token + Username + token + passwordMD5 + token + "1" + token + "" + token + n + token + type + token + data["info"]);
            return await PostAsync(LogUri, data);
        }
        public Task<string> LogoutAsync() => PostAsync(LogUri, LogoutData);
        public async Task<FluxUser> GetFluxAsync() => GetFluxUser(await PostAsync(FluxUri, (string)null));
        private static unsafe List<long> S(string a, bool b)
        {
            int c = a.Length;
            List<long> v = new List<long>();
            for (int i = 0; i < c; i += 4)
            {
                long value = 0;
                byte* p = (byte*)&value;
                p[0] = (byte)a[i];
                p[1] = (byte)(i + 1 >= c ? 0 : a[i + 1]);
                p[2] = (byte)(i + 2 >= c ? 0 : a[i + 2]);
                p[3] = (byte)(i + 3 >= c ? 0 : a[i + 3]);
                v.Add(value);
                p = null;
            }
            if (b) v.Add(c);
            return v;
        }
        private static unsafe string L(List<long> a, bool b)
        {
            int d = a.Count;
            long c = (d - 1) << 2;
            List<string> aa = new List<string>();
            if (b)
            {
                long m = a[d - 1];
                if (m < c - 3 || m > c)
                {
                    return null;
                }
                c = m;
            }
            for (int i = 0; i < d; i++)
            {
                long value = a[i];
                byte* p = (byte*)&value;
                aa.Add(((char)p[0]).ToString() + ((char)p[1]).ToString() + ((char)p[2]).ToString() + ((char)p[3]).ToString());
                p = null;
            }
            if (b)
            {
                return String.Concat(aa).Substring(0, (int)c);
            }
            else
            {
                return String.Concat(aa);
            }
        }
        private static string Encode(string str, string key)
        {
            long RightShift(long x, int nn)
            {
                return x >> nn;
            }
            long LeftShift(long x, int nn)
            {
                return (x << nn) & 4294967295;
            }
            if (str.Length == 0) return String.Empty;
            List<long> v = S(str, true);
            List<long> k = S(key, false);
            while (k.Count < 4)
            {
                k.Add(0);
            }
            int n = v.Count - 1;
            long z = v[n];
            long y = v[0];
            uint c = 0x86014019 | 0x183639A0;
            double q = Math.Floor(6.0 + 52 / (n + 1));
            long d = 0;
            while (0 < q)
            {
                q -= 1;
                d = d + c & (0x8CE0D9BF | 0x731F2640);
                long e = RightShift(d, 2) & 3;
                for (int p = 0; p < n; p++)
                {
                    y = v[p + 1];
                    long mm = RightShift(z, 5) ^ LeftShift(y, 2);
                    mm += RightShift(y, 3) ^ LeftShift(z, 4) ^ (d ^ y);
                    long tt = (p & 3) ^ e;
                    mm += k[(int)tt] ^ z;
                    z = v[p] = v[p] + mm & (0xEFB8D130 | 0x10472ECF);
                }
                y = v[0];
                long m = RightShift(z, 5) ^ LeftShift(y, 2);
                m += RightShift(y, 3) ^ LeftShift(z, 4) ^ (d ^ y);
                long t = (n & 3) ^ e;
                m += k[(int)t] ^ z;
                z = v[n] = v[n] + m & (0xBB390742 | 0x44C6F8BD);
            }
            return L(v, false);
        }
        private unsafe static string Base64Encode(string t)
        {
            string n = "LVoJPiCN2R8G90yg+hmFHuacZ1OWMnrsSTXkYpUq/3dlbfKwv6xztjI7DeBE45QA";
            string u = "";
            int a = t.Length;
            string r = "=";
            for (int o = 0; o < a; o += 3)
            {
                int h = 0;
                byte* p = (byte*)&h;
                p[2] = (byte)t[o];
                p[1] = (byte)(o + 1 < a ? t[o + 1] : 0);
                p[0] = (byte)(o + 2 < a ? t[o + 2] : 0);
                for (var i = 0; i < 4; i += 1)
                {
                    if (o * 8 + i * 6 > a * 8)
                    {
                        u += r;
                    }
                    else
                    {
                        u += n[h >> 6 * (3 - i) & 63];
                    }
                }
                p = null;
            }
            return u;
        }
        private async Task<string> GetChallengeAsync()
        {
            string result = await GetAsync(ChallengeUri + "?" + string.Format(ChallengeData, Username));
            int begin = result.IndexOf('{');
            int end = result.LastIndexOf('}');
            return result.Substring(begin, end - begin + 1);
        }
    }
}
