using System;
using System.Collections.Generic;

namespace Berrysoft.Html
{
    class HtmlRootNode : HtmlNode
    {
        public HtmlRootNode()
            : base("html")
        { }

        private HtmlHeadNode htmlHead;
        private HtmlBodyNode htmlBody;

        public override HtmlNode Node(string name)
        {
            if (name.Equals(htmlHead.Name, StringComparison.OrdinalIgnoreCase))
            {
                return htmlHead;
            }
            else if (name.Equals(htmlBody.Name, StringComparison.OrdinalIgnoreCase))
            {
                return htmlBody;
            }
            throw ExceptionHelper.NodeNotFound(name);
        }

        public override IEnumerable<HtmlNode> Nodes()
        {
            return new HtmlNode[] { htmlHead, htmlBody };
        }

        public override IEnumerable<HtmlNode> Nodes(string name)
        {
            if (name.Equals(htmlHead.Name, StringComparison.OrdinalIgnoreCase))
            {
                return new HtmlNode[] { htmlHead };
            }
            else if (name.Equals(htmlBody.Name, StringComparison.OrdinalIgnoreCase))
            {
                return new HtmlNode[] { htmlBody };
            }
            throw ExceptionHelper.NodeNotFound(name);
        }

        public override void AddNode(HtmlNode node) => throw ExceptionHelper.NotSupported();

        public override void RemoveNode(HtmlNode node) => throw ExceptionHelper.NotSupported();

        public override string ToString()
        {
            return $"<html>{htmlHead.ToString()}{htmlBody.ToString()}</html>";
        }
    }
}
