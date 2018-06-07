using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Berrysoft.Tsinghua.Net
{
    /// <summary>
    /// A simple class represents the status of a connection.
    /// </summary>
    public class NetUser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetUser"/> class.
        /// </summary>
        /// <param name="address">IP address the connection was allocated.</param>
        /// <param name="loginTime">Online time used this time of the connection.</param>
        /// <param name="client">The client used by this connection.</param>
        public NetUser(IPAddress address, DateTime loginTime, string client)
        {
            Address = address;
            LoginTime = loginTime;
            Client = client;
        }
        /// <summary>
        /// IP address the connection was allocated.
        /// </summary>
        public IPAddress Address { get; }
        /// <summary>
        /// Online time used this time of the connection.
        /// </summary>
        public DateTime LoginTime { get; }
        /// <summary>
        /// The client used by this connection. It may be "Unknown" through <see cref="NetHelper"/>, and "Windows NT", "Windows 8", "Windows 7" or "Unknown" through <see cref="AuthHelper"/>.
        /// </summary>
        public string Client { get; }
    }
    /// <summary>
    /// Exposes methods to login, logout, get connection information and drop connections from https://usereg.tsinghua.edu.cn/
    /// </summary>
    public class UseregHelper : NetHelperBase
    {
        private const string LogUri = "https://usereg.tsinghua.edu.cn/do.php";
        private const string InfoUri = "https://usereg.tsinghua.edu.cn/online_user_ipv4.php";
        private const string LogoutData = "action=logout";
        private const string DropData = "action=drop&user_ip={0}";
        /// <summary>
        /// Initializes a new instance of the <see cref="UseregHelper"/> class.
        /// </summary>
        public UseregHelper()
            : base()
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="UseregHelper"/> class.
        /// </summary>
        /// <param name="client">A user-specified instance of <see cref="HttpClient"/>.</param>
        public UseregHelper(HttpClient client)
            : base(client)
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="UseregHelper"/> class.
        /// </summary>
        /// <param name="username">The username to login.</param>
        /// <param name="password">The password to login.</param>
        public UseregHelper(string username, string password)
            : base(username, password)
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="UseregHelper"/> class.
        /// </summary>
        /// <param name="username">The username to login.</param>
        /// <param name="password">The password to login.</param>
        /// <param name="client">A user-specified instance of <see cref="HttpClient"/>.</param>
        public UseregHelper(string username, string password, HttpClient client)
            : base(username, password, client)
        { }
        /// <summary>
        /// Login to the website.
        /// </summary>
        /// <returns>The response of the website.</returns>
        public Task<string> LoginAsync() => PostAsync(LogUri, GetLoginData());
        /// <summary>
        /// Logout from the website.
        /// </summary>
        /// <returns>The response of the website.</returns>
        public Task<string> LogoutAsync() => PostAsync(LogUri, LogoutData);
        /// <summary>
        /// Drop the IP address from network.
        /// </summary>
        /// <param name="ip">The IP address to be dropped.</param>
        /// <returns>The response of the website.</returns>
        public Task<string> LogoutAsync(IPAddress ip) => PostAsync(InfoUri, string.Format(DropData, ip.ToString()));

        private static readonly Regex TableRegex = new Regex(@"<tr align=""center"">.+?</tr>", RegexOptions.Singleline);
        private static readonly Regex ItemRegex = new Regex(@"(?<=\<td class=""maintd""\>)(.*?)(?=\</td\>)");
        /// <summary>
        /// Get all connections of this user.
        /// </summary>
        /// <returns><see cref="IEnumerable{NetUser}"/></returns>
        public async Task<IEnumerable<NetUser>> GetUsersAsync()
        {
            try
            {
                string userhtml = await GetAsync(InfoUri);
                return from Match r in TableRegex.Matches(userhtml)
                       let details = ItemRegex.Matches(r.Value)
                       select new NetUser(
                           IPAddress.Parse(details[0].Value),
                           DateTime.ParseExact(details[1].Value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                           details[10].Value);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private Dictionary<string, string> loginDataDictionary;
        /// <summary>
        /// Get login data with username and password.
        /// </summary>
        /// <returns>A dictionary contains the data.</returns>
        private Dictionary<string, string> GetLoginData()
        {
            if (loginDataDictionary == null)
            {
                loginDataDictionary = new Dictionary<string, string>()
                {
                    ["action"] = "login"
                };
            }
            loginDataDictionary["user_login_name"] = Username;
            loginDataDictionary["user_password"] = CryptographyHelper.GetMD5(Password);
            return loginDataDictionary;
        }
    }
}
