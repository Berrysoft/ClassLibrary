using System;
using System.Text;

namespace Berrysoft.Html
{
    public class HtmlDocument : HtmlObject
    {
        public HtmlDocument()
            : this(Encoding.UTF8)
        { }

        public HtmlDocument(Encoding encoding)
            :base(encoding)
        {
            head = new HtmlNode("head");
            body = new HtmlNode("body");
        }

        private HtmlDeclaration decl;
        public HtmlDeclaration Declaration
        {
            get => decl;
            set => decl = value;
        }

        private HtmlNode head;
        public HtmlNode Head => head;

        private HtmlNode body;
        public HtmlNode Body => body;

        public override string ToString()
        {
            return $"{decl?.ToString() ?? HtmlDeclaration.Default.ToString()}<html>{head.ToString()}{body.ToString()}</html>";
        }
    }
}
