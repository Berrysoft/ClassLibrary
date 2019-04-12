using System;

namespace Berrysoft.Tsinghua.Net
{
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

        internal static FluxUser Parse(string fluxstr)
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
    }
}
