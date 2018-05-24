using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Berrysoft.Tsinghua.Physics
{
    public class FtpClient
    {
        private const string BaseAddr = "ftp://166.111.26.13/";
        private WebClient client;
        public FtpClient()
        {
            client = new WebClient();
            client.Encoding = Encoding.UTF8;
            client.BaseAddress = BaseAddr;
        }
        public ICredentials Credentials
        {
            get => client.Credentials;
            set => client.Credentials = value;
        }
        public void CancelAsync() => client.CancelAsync();
        public void DownloadFile(string address, string fileName) => client.DownloadFile(address, fileName);
        public void DownloadFile(Uri address, string fileName) => client.DownloadFile(address, fileName);
        public Task DownloadFileAsync(string address, string fileName) => client.DownloadFileTaskAsync(address, fileName);
        public Task DownloadFileAsync(Uri address, string fileName) => client.DownloadFileTaskAsync(address, fileName);
        public byte[] UploadFile(string address, string fileName) => client.UploadFile(address, fileName);
        public byte[] UploadFile(Uri address, string fileName) => client.UploadFile(address, fileName);
        public Task<byte[]> UploadFileAsync(string address, string fileName) => client.UploadFileTaskAsync(address, fileName);
        public Task<byte[]> UploadFileAsync(Uri address, string fileName) => client.UploadFileTaskAsync(address, fileName);
    }
}
