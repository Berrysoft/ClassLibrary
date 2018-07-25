using System;
using System.Collections.Generic;
using static Berrysoft.Html.Markdown.MdElementHelper;
using static Berrysoft.Html.Markdown.MdTextAnalyzerHelper;

namespace Berrysoft.Html.Markdown
{
    class MdListElement : MdElement
    {
        List<string> lines = new List<string>();
        public MdListElement(string[] lines, ref int index)
        {
            for (; index < lines.Length; index++)
            {
                string line = lines[index];
                if (!string.IsNullOrWhiteSpace(line) && !HeadRegex.IsMatch(line) && !CodeBlockRegex.IsMatch(line))
                {
                    if (ListItemRegex.IsMatch(line))
                    {
                        this.lines.Add(line);
                    }
                    else
                    {
                        this.lines[this.lines.Count - 1] += ' ' + line;
                    }
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
            HtmlNode node = new HtmlNode("ul");
            foreach (string line in lines)
            {
                HtmlNode li = new HtmlNode("li");
                var matches = ListItemRegex.Match(line).Groups;
                foreach (HtmlObject o in GetHtmlObjects(matches[2].Value, GetTextTokens(matches[2].Value, 0)))
                {
                    li.AddElement(o);
                }
                node.AddElement(li);
            }
            return node;
        }
    }
}
