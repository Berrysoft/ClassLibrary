using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Berrysoft.Tsinghua.Net
{
    /// <summary>
    /// Exposes methods to login, logout and get flux from http://net.tsinghua.edu.cn/
    /// </summary>
    public class NetHelper : NetHelperBase, IConnect
    {
        private const string LogUri = "http://net.tsinghua.edu.cn/do_login.php";
        private const string FluxUri = "http://net.tsinghua.edu.cn/rad_user_info.php";
        private const string LogoutData = "action=logout";
        private const string LogoutUserData = "action=logout&username={0}";
        /// <summary>
        /// Initializes a new instance of the <see cref="NetHelper"/> class.
        /// </summary>
        public NetHelper()
            : base()
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="NetHelper"/> class.
        /// </summary>
        /// <param name="client">A user-specified instance of <see cref="HttpClient"/>.</param>
        public NetHelper(HttpClient client)
            : base(client)
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="NetHelper"/> class.
        /// </summary>
        /// <param name="username">The username to login.</param>
        /// <param name="password">The password to login.</param>
        public NetHelper(string username, string password)
            : base(username, password)
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="NetHelper"/> class.
        /// </summary>
        /// <param name="username">The username to login.</param>
        /// <param name="password">The password to login.</param>
        /// <param name="client">A user-specified instance of <see cref="HttpClient"/>.</param>
        public NetHelper(string username, string password, HttpClient client)
            : base(username, password, client)
        { }
        /// <summary>
        /// Login to the network.
        /// </summary>
        /// <returns>The response of the website.</returns>
        public Task<string> LoginAsync() => PostAsync(LogUri, GetLoginData());
        /// <summary>
        /// Logout from the network.
        /// </summary>
        /// <returns>The response of the website.</returns>
        public Task<string> LogoutAsync() => PostAsync(LogUri, LogoutData);
        /// <summary>
        /// Logout from the network with the specified username.
        /// </summary>
        /// <param name="username">The specified username.</param>
        /// <returns>The response of the website.</returns>
        public Task<string> LogoutAsync(string username) => PostAsync(LogUri, string.Format(LogoutUserData, username));
        /// <summary>
        /// Get information of the user online.
        /// </summary>
        /// <returns>An instance of <see cref="FluxUser"/> class of the current user.</returns>
        public async Task<FluxUser> GetFluxAsync() => FluxUser.Parse(await PostAsync(FluxUri));

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
                    ["action"] = "login",
                    ["ac_id"] = "1"
                };
            }
            loginDataDictionary["username"] = Username;
            loginDataDictionary["password"] = "{MD5_HEX}" + CryptographyHelper.GetMD5(Password);
            return loginDataDictionary;
        }
    }
}
