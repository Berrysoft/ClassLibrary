using System;
using System.Text;

namespace Berrysoft.Html
{
    public abstract class HtmlObject
    {
        public HtmlObject()
            : this(Encoding.UTF8)
        { }

        public HtmlObject(Encoding encoding)
        {
            this.encoding = encoding;
        }

        private Encoding encoding;
        public Encoding Encoding => encoding;
    }
}
