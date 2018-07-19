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

        public static implicit operator HtmlObject(string value) => new HtmlString(value);

        public static explicit operator string(HtmlObject value) => value.ToString();
    }
}
