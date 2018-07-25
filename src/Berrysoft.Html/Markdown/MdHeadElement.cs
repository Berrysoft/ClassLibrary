using System;
using System.Collections.Generic;
using static Berrysoft.Html.Markdown.MdElementHelper;
using static Berrysoft.Html.Markdown.MdTextAnalyzerHelper;

namespace Berrysoft.Html.Markdown
{
    class MdHeadElement : MdElement
    {
        string line;
        public MdHeadElement(string line)
        {
            this.line = line;
        }

        public override HtmlNode ToHtmlNode()
        {
            var matches = HeadRegex.Match(line).Groups;
            int rank = matches[1].Length;
            HtmlNode node = new HtmlNode($"h{rank}");
            foreach (HtmlObject o in GetHtmlObjects(matches[3].Value, GetTextTokens(matches[3].Value, 0)))
            {
                node.AddElement(o);
            }
            return node;
        }
    }
}
