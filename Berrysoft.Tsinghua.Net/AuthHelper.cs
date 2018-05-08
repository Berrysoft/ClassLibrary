using System;
using System.Threading.Tasks;

namespace Berrysoft.Tsinghua.Net
{
    public class AuthHelper : NetHelperBase, IConnect
    {
        private const string LoginUriBase = "https://auth{0}.tsinghua.edu.cn/srun_portal_pc.php";
        private const string LogoutUriBase = "https://auth{0}.tsinghua.edu.cn/cgi-bin/srun_portal";
        private const string FluxUriBase = "https://auth{0}.tsinghua.edu.cn/rad_user_info.php";
        private const string LoginData = "action=login&ac_id=1&user_ip=&nas_ip=&user_mac=&url=&username={0}&password={1}&save_me=0";
        private const string LogoutData = "action=logout&ac_id=1&ip=&double_stack=1";
        private readonly string LoginUri;
        private readonly string LogoutUri;
        private readonly string FluxUri;
        private AuthHelper(string username, string password, int version)
            : base(username, password)
        {
            LoginUri = string.Format(LoginUriBase, version);
            LogoutUri = string.Format(LogoutUriBase, version);
            FluxUri = string.Format(FluxUriBase, version);
        }
        public Task<string> LoginAsync()
        {
            return PostAsync(LoginUri, string.Format(LoginData, Username, Password));
        }
        public Task<string> LogoutAsync()
        {
            return PostAsync(LogoutUri, LogoutData);
        }
        public async Task<FluxUser> GetFluxAsync()
        {
            return GetFluxUser(await PostAsync(FluxUri, null));
        }
    }
}
