using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Berrysoft.Tsinghua.Net;

namespace Berrysoft.Tsinghua.Physics
{
    public class ForumHelper : NetHelperBase
    {
        private const string LogUri = "http://jkparadise.space/member.php";
        private const string PostUri = "http://jkparadise.space/forum.php";
        private const string LoginData = "mod=logging&action=login&loginsubmit=yes&handlekey=login&loginfield=username&username={0}&password={1}&questionid=0&answer=";
        private const string ForumHashData = "mod=post&action=newthread&fid={0}";
        private const string PostData = "mod=post&action=newthread&fid={0}&extra=&topicsubmit=yes&formhash={1}&subject={2}&message={3}";
        public ForumHelper()
            : base()
        { }
        public ForumHelper(string username, string password)
            : base(username, password)
        { }
        public Task<string> LoginAsync() => PostAsync(LogUri, string.Format(LoginData, Username, Password));
        private async Task<string> GetForumHashAsync(string fid)
        {
            string result = await GetAsync(string.Format(PostUri + PostData, fid));
            Regex reg = new Regex("\"formhash\"\\s?value=\"[a-z0-9A-Z]+?\"");
            string strForm = reg.Match(result, 0).Value.ToLower();
            string value = strForm.Replace("formhash", "").Replace("value=", "").Replace("\"", "").Trim();
            return value;
        }
        public async Task<string> PostThemeAsync(string title, string content, string fid)
            => await PostAsync(PostUri, string.Format(PostData, fid, await GetForumHashAsync(fid), title, content));
    }
}
