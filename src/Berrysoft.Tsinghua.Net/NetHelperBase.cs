using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Berrysoft.Tsinghua.Net
{
    /// <summary>
    /// Exposes methods to login, logout and get flux from the login page.
    /// </summary>
    public interface IConnect : IDisposable
    {
        /// <summary>
        /// Login to the network.
        /// </summary>
        /// <returns>The response of the website, may be a sentense or a html page.</returns>
        Task<string> LoginAsync();
        /// <summary>
        /// Logout from the network.
        /// </summary>
        /// <returns>The response of the website, may be a sentense or a html page.</returns>
        Task<string> LogoutAsync();
        /// <summary>
        /// Get information of the user online.
        /// </summary>
        /// <returns>An instance of <see cref="FluxUser"/> class of the current user.</returns>
        Task<FluxUser> GetFluxAsync();
    }
    /// <summary>
    /// A simple class represents the current user online.
    /// </summary>
    public class FluxUser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FluxUser"/> class.
        /// </summary>
        /// <param name="username">Username of the user.</param>
        /// <param name="flux">Flux used by the user this month.</param>
        /// <param name="onlineTime">Online time used this time of the user.</param>
        /// <param name="balance">The network balance of the user.</param>
        public FluxUser(string username, long flux, TimeSpan onlineTime, decimal balance)
        {
            Username = username;
            Flux = flux;
            OnlineTime = onlineTime;
            Balance = balance;
        }
        /// <summary>
        /// Username of the user.
        /// </summary>
        public string Username { get; }
        /// <summary>
        /// Flux used by the user this month.
        /// </summary>
        public long Flux { get; }
        /// <summary>
        /// Online time used this time of the user.
        /// </summary>
        public TimeSpan OnlineTime { get; }
        /// <summary>
        /// The network balance of the user.
        /// </summary>
        public decimal Balance { get; }
        /// <summary>
        /// Converts the string representation of informathion of current user to its <see cref="FluxUser"/> equivalent.
        /// </summary>
        /// <param name="fluxstr">A string containing information to convert.</param>
        /// <returns>An instance of <see cref="FluxUser"/>.</returns>
        public static FluxUser Parse(string fluxstr)
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
        /// <summary>
        /// Converts the string representation of informathion of current user to its <see cref="FluxUser"/> equivalent.
        /// A return value indicates whether the conversion succeeded or failed.
        /// </summary>
        /// <param name="fluxstr">A string containing information to convert.</param>
        /// <param name="user">An instance of <see cref="FluxUser"/>, when succeed; otherwise null.</param>
        /// <returns>true if fluxstr was converted successfully; otherwise, false.</returns>
        public static bool TryParse(string fluxstr, out FluxUser user)
        {
            string[] r = fluxstr.Split(',');
            if (!string.IsNullOrWhiteSpace(r[0])
                && long.TryParse(r[6], out long flux)
                && long.TryParse(r[2], out long endTime)
                && long.TryParse(r[1], out long startTime)
                && decimal.TryParse(r[11], out decimal balance))
            {
                user = new FluxUser(r[0], flux, TimeSpan.FromSeconds(endTime - startTime), balance);
                return true;
            }
            user = null;
            return false;
        }
    }
    /// <summary>
    /// Base class of net helpers.
    /// </summary>
    public abstract class NetHelperBase : IDisposable
    {
        private HttpClient client;
        /// <summary>
        /// The username to login.
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// The password to login.
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="NetHelperBase"/> class.
        /// </summary>
        public NetHelperBase()
            : this(string.Empty, string.Empty)
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="NetHelperBase"/> class.
        /// </summary>
        /// <param name="username">The username to login.</param>
        /// <param name="password">The password to login.</param>
        public NetHelperBase(string username, string password)
        {
            Username = username;
            Password = password;
            client = new HttpClient();
        }
        /// <summary>
        /// Send a POST request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="uri">The Uri the request is sent to.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        protected async Task<string> PostAsync(string uri)
        {
            using (HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, uri))
            {
                using (HttpResponseMessage response = await client.SendAsync(message))
                {
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }
        /// <summary>
        /// Send a POST request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="uri">The Uri the request is sent to.</param>
        /// <param name="data">The HTTP request string content sent to the server.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
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
        /// <summary>
        /// Send a POST request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="uri">The Uri the request is sent to.</param>
        /// <param name="data">The HTTP request dictionary content sent to the server.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        protected async Task<string> PostAsync(string uri, Dictionary<string, string> data)
        {
            using (HttpContent content = new FormUrlEncodedContent(data))
            {
                using (HttpResponseMessage response = await client.PostAsync(uri, content))
                {
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }
        /// <summary>
        /// Send a GET request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="uri">The Uri the request is sent to.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        protected Task<string> GetAsync(string uri)
        {
            return client.GetStringAsync(uri);
        }
        #region IDisposable Support
        private bool disposedValue = false;
        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="HttpClient"/> and optionally disposes of the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to releases only unmanaged resources.</param>
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
        /// <summary>
        /// Releases the unmanaged resources and disposes of the managed resources used by the <see cref="HttpClient"/>.
        /// </summary>
        public void Dispose() => Dispose(true);
        #endregion
    }
}
