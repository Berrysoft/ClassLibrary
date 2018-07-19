using System;
using System.Collections.Generic;
using System.Text;

namespace Berrysoft.Html
{
    public class HtmlDocument : HtmlObject
    {
        public HtmlDocument()
        {
            root = new HtmlRootNode();
        }

        private HtmlDeclaration decl;
        public HtmlDeclaration Declaration
        {
            get => decl;
            set => decl = value;
        }

        private HtmlRootNode root;
        public HtmlNode RootNode => root;

        public override string ToString()
        {
            return decl.ToString() + root.ToString();
        }
    }
}
