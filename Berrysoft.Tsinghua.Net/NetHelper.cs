using System;
using System.Threading.Tasks;

namespace Berrysoft.Tsinghua.Net
{
    public class NetHelper : NetHelperBase, IConnect
    {
        private const string LogUri = "http://net.tsinghua.edu.cn/do_login.php";
        private const string FluxUri = "http://net.tsinghua.edu.cn/rad_user_info.php";
        private const string LoginData = "action=login&username={0}&password={{MD5_HEX}}{1}&ac_id=1";
        private const string LogoutData = "action=logout";
        public NetHelper()
            : base()
        { }
        public NetHelper(string username, string password)
            : base(username, password)
        { }
        public Task<string> LoginAsync() => PostAsync(LogUri, string.Format(LoginData, Username, GetMD5(Password)));
        public Task<string> LogoutAsync() => PostAsync(LogUri, LogoutData);
        public async Task<FluxUser> GetFluxAsync() => GetFluxUser(await PostAsync(FluxUri, null));
    }
}
