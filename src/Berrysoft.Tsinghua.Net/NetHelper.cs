using System;
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
        private const string LoginData = "action=login&username={0}&password={{MD5_HEX}}{1}&ac_id=1";
        private const string LogoutData = "action=logout";
        /// <summary>
        /// Initializes a new instance of the <see cref="NetHelper"/> class.
        /// </summary>
        public NetHelper()
            : base()
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
        /// Login to the network.
        /// </summary>
        /// <returns>The response of the website.</returns>
        public Task<string> LoginAsync() => PostAsync(LogUri, string.Format(LoginData, Username, CryptographyHelper.GetMD5(Password)));
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
    }
}
