using System;

namespace Berrysoft.Tsinghua.Physics
{
    public static class KnownFtpFolders
    {
        public static readonly Uri Academic = new Uri("/Academic", UriKind.Relative);
        public static readonly Uri Downloads = new Uri("/Downloads", UriKind.Relative);
        public static readonly Uri Entertainment = new Uri("/Entertainment", UriKind.Relative);
        public static readonly Uri Comic = new Uri("/Entertainment/Comic", UriKind.Relative);
        public static readonly Uri Video = new Uri("/Entertainment/Video", UriKind.Relative);
        public static readonly Uri Softwares = new Uri("/Softwares", UriKind.Relative);
    }
}
