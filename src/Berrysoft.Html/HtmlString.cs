using System;
using System.Collections.Generic;
using System.Text;

namespace Berrysoft.Html
{
    public class HtmlString : HtmlObject
    {
        public HtmlString(string value)
            : this(value, Encoding.UTF8)
        { }

        public HtmlString(string value, Encoding encoding)
            : base(encoding)
        {
            htmlValue = value;
        }

        private string htmlValue;
        public string Value => htmlValue;

        public override string ToString() => htmlValue;
    }
}
