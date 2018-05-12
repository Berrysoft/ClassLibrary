﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Berrysoft.Tsinghua.Net
{
    public class NetUser
    {
        public NetUser(IPAddress address, DateTime loginTime, string client)
        {
            Address = address;
            LoginTime = loginTime;
            Client = client;
        }
        public IPAddress Address { get; }
        public DateTime LoginTime { get; }
        public string Client { get; }
    }
    public class UseregHelper : NetHelperBase
    {
        private const string LogUri = "https://usereg.tsinghua.edu.cn/do.php";
        private const string InfoUri = "https://usereg.tsinghua.edu.cn/online_user_ipv4.php";
        private const string LoginData = "action=login&user_login_name={0}&user_password={1}";
        private const string LogoutData = "action=logout";
        private const string DropData = "action=drop&user_ip={0}";
        public UseregHelper(string username, string password)
            : base(username, GetMD5(password ?? string.Empty))
        { }
        public Task<string> LoginAsync()
        {
            return PostAsync(LogUri, string.Format(LoginData, Username, Password));
        }
        public Task<string> LogoutAsync()
        {
            return PostAsync(LogUri, LogoutData);
        }
        public Task<string> LogoutAsync(IPAddress ip)
        {
            return PostAsync(InfoUri, string.Format(DropData, ip.ToString()));
        }
        public async Task<IEnumerable<NetUser>> GetUsersAsync()
        {
            try
            {
                string userhtml = await GetAsync(InfoUri);
                var info = Regex.Matches(userhtml, @"<tr align=""center"">.+?</tr>", RegexOptions.Singleline);
                return from Match r in info
                       let details = Regex.Matches(r.Value, @"(?<=\<td class=""maintd""\>)(.*?)(?=\</td\>)")
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
    }
}