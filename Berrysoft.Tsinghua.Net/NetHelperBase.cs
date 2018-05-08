using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Berrysoft.Tsinghua.Net
{
    public interface IConnect
    {
        Task<string> LoginAsync();
        Task<string> LogoutAsync();
        Task<FluxUser> GetFluxAsync();
    }
    public class FluxUser
    {
        public FluxUser(string username, long flux, TimeSpan onlineTime, decimal balance)
        {
            Username = username;
            Flux = flux;
            OnlineTime = onlineTime;
            Balance = balance;
        }
        public string Username { get; }
        public long Flux { get; }
        public TimeSpan OnlineTime { get; }
        public decimal Balance { get; }
    }
    public class NetHelperBase
    {
        private static readonly HttpClient Client = new HttpClient();
        public string Username { get; }
        public string Password { get; }
        public NetHelperBase(string username, string password)
        {
            Username = username;
            Password = password;
        }
        public static async Task<string> PostAsync(string uri, string data)
        {
            using (StringContent content = new StringContent(data ?? string.Empty, Encoding.ASCII, "application/x-www-form-urlencoded"))
            {
                using (HttpResponseMessage response = await Client.PostAsync(uri, content))
                {
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }
        public static async Task<string> GetAsync(string uri)
        {
            return await Client.GetStringAsync(uri);
        }
        internal static FluxUser GetFluxUser(string fluxstr)
        {
            string[] r = fluxstr.Split(',');
            if (string.IsNullOrWhiteSpace(r[0]))
            {
                return null;
            }
            else
            {
                return new FluxUser(
                    r[0],
                    long.Parse(r[6]),
                    TimeSpan.FromSeconds(long.Parse(r[2]) - long.Parse(r[1])),
                    decimal.Parse(r[11]));
            }
        }
        internal static string GetMD5(string input)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                return sBuilder.ToString();
            }
        }
    }
}
