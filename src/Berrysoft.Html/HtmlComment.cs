using System;
using System.Collections.Generic;
using System.Text;

namespace Berrysoft.Html
{
    public class HtmlComment : HtmlObject
    {
        public HtmlComment(string comment)
            : this(comment, Encoding.UTF8)
        { }

        public HtmlComment(string comment, Encoding encoding)
            : base(encoding)
        {
            this.comment = comment;
        }

        private string comment;

        public string Value => comment;

        public override string ToString()
        {
            return $"<!--{comment}-->";
        }
    }
}
