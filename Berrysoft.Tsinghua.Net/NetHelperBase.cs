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
    public class NetHelperBase : IDisposable
    {
        private HttpClient client;
        private readonly object syncLock = new object();
        public string Username { get; set; }
        public string Password { get; set; }
        public NetHelperBase()
            : this(string.Empty, string.Empty)
        { }
        public NetHelperBase(string username, string password)
        {
            Username = username;
            Password = password;
            client = new HttpClient();
        }
        protected async Task<string> PostAsync(string uri, string data)
        {
            using (StringContent content = new StringContent(data ?? string.Empty, Encoding.ASCII, "application/x-www-form-urlencoded"))
            {
                using (HttpResponseMessage response = await client.PostAsync(uri, content))
                {
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }
        protected Task<string> GetAsync(string uri)
        {
            return client.GetStringAsync(uri);
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
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
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
        #region IDisposable Support
        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    client.Dispose();
                }
                disposedValue = true;
            }
        }
        public void Dispose() => Dispose(true);
        #endregion
    }
}
