using System;
using System.Collections.Generic;
using static Berrysoft.Html.Markdown.MdElementHelper;
using static Berrysoft.Html.Markdown.MdTextAnalyzerHelper;

namespace Berrysoft.Html.Markdown
{
    class MdParaElement : MdElement
    {
        List<string> lines = new List<string>();
        public MdParaElement(string[] lines, ref int index)
        {
            for (; index < lines.Length; index++)
            {
                string line = lines[index];
                if (!string.IsNullOrWhiteSpace(line) && !HeadRegex.IsMatch(line) && !ListItemRegex.IsMatch(line) && !CodeBlockRegex.IsMatch(line))
                {
                    this.lines.Add(line);
                }
                else
                {
                    index--;
                    break;
                }
            }
        }

        public override HtmlNode ToHtmlNode()
        {
            HtmlNode node = new HtmlNode("p");
            foreach (string line in lines)
            {
                foreach (HtmlObject o in GetHtmlObjects(line, GetTextTokens(line, 0)))
                {
                    node.AddElement(o);
                }
                node.AddElement(" ");
            }
            return node;
        }
    }
}
