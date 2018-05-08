using System;
using System.Threading.Tasks;

namespace Berrysoft.Tsinghua.Net
{
    public class NetHelper : NetHelperBase, IConnect
    {
        private const string LogUri = "https://net.tsinghua.edu.cn/do_login.php";
        private const string FluxUri = "https://net.tsinghua.edu.cn/rad_user_info.php";
        private const string LoginData = "action=login&username={0}&password={1}&ac_id=1";
        private const string LogoutData = "action=logout";
        public NetHelper(string username, string password)
            : base(username, "{MD5_HEX}" + GetMD5(password))
        { }
        public Task<string> LoginAsync()
        {
            return PostAsync(LogUri, string.Format(LoginData, Username, Password));
        }
        public Task<string> LogoutAsync()
        {
            return PostAsync(LogUri, LogoutData);
        }
        public async Task<FluxUser> GetFluxAsync()
        {
            return GetFluxUser(await PostAsync(FluxUri, null));
        }
    }
}
