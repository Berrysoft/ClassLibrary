using System;
using System.Collections.Generic;
using System.Text;

namespace Berrysoft.Html
{
    class HtmlHeadNode : HtmlNode
    {
        public HtmlHeadNode()
            : base("head")
        { }

        public override void AddNode(HtmlNode node)
        {
            throw new NotImplementedException();
        }

        public override HtmlNode Node(string name)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<HtmlNode> Nodes()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<HtmlNode> Nodes(string name)
        {
            throw new NotImplementedException();
        }

        public override void RemoveNode(HtmlNode node)
        {
            throw new NotImplementedException();
        }
    }

    class HtmlBodyNode : HtmlNode
    {
        public HtmlBodyNode()
            : base("body")
        { }

        public override void AddNode(HtmlNode node)
        {
            throw new NotImplementedException();
        }

        public override HtmlNode Node(string name)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<HtmlNode> Nodes()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<HtmlNode> Nodes(string name)
        {
            throw new NotImplementedException();
        }

        public override void RemoveNode(HtmlNode node)
        {
            throw new NotImplementedException();
        }
    }
}
