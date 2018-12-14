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
        private const string LogoutData = "action=logout";
        private const string LogoutUserData = "action=logout&username={0}";
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
        /// Logout from the network with the specified username.
        /// When a user logged in through <see cref="AuthHelper"/> and logged out through <see cref="NetHelper"/>,
        /// he should call this method with his username explicitly, or he can't logout.
        /// </summary>
        /// <param name="username">The specified username.</param>
        /// <returns>The response of the website.</returns>
        public Task<string> LogoutAsync(string username) => PostAsync(LogUri, string.Format(LogoutUserData, username));
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
        private async Task<Dictionary<string, string>> GetLoginDataAsync()
        {
            //const string passwordMD5 = "5e543256c480ac577d30f76f9120eb74";
            string token = await GetChallengeAsync();
            string passwordMD5 = CryptographyHelper.GetHMACMD5(token);
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
            loginDataDictionary["info"] = "{SRBX1}" + CryptographyHelper.Base64Encode(CryptographyHelper.XEncode(string.Format(LoginInfoJson, Username, Password), token));
            loginDataDictionary["username"] = Username;
            loginDataDictionary["chksum"] = CryptographyHelper.GetSHA1(string.Format(ChkSumData, token, Username, passwordMD5, loginDataDictionary["info"]));
            return loginDataDictionary;
        }
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
