using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static Berrysoft.Html.Markdown.MdElementHelper;
using static Berrysoft.Html.Markdown.MdTextAnalyzerHelper;

namespace Berrysoft.Html.Markdown
{
    class MdTableElement : MdElement
    {
        enum Align
        {
            Default,
            Left,
            Center,
            Right
        }

        string head;
        List<string> elements = new List<string>();
        List<Align> headsAlign = new List<Align>();
        public MdTableElement(string[] lines, ref int index)
        {
            head = lines[index];
            index++;
            var matches = TableAlignRegex.Matches(lines[index]);
            foreach (Match match in matches)
            {
                if (match.Groups[2].Length >= 1)
                {
                    if (match.Groups[1].Length >= 1)
                    {
                        headsAlign.Add(Align.Center);
                    }
                    else
                    {
                        headsAlign.Add(Align.Right);
                    }
                }
                else
                {
                    if (match.Groups[1].Length >= 1)
                    {
                        headsAlign.Add(Align.Left);
                    }
                    else
                    {
                        headsAlign.Add(Align.Default);
                    }
                }
            }
            index++;
            for (; index < lines.Length; index++)
            {
                string line = lines[index];
                if (!string.IsNullOrWhiteSpace(line))
                {
                    this.elements.Add(line);
                }
                else
                {
                    break;
                }
            }
        }

        private void AddAlign(HtmlNode node, Align align)
        {
            switch (align)
            {
                case Align.Left:
                    node.AddAttribute(new HtmlAttribute("style", "text-align:left"));
                    break;
                case Align.Center:
                    node.AddAttribute(new HtmlAttribute("style", "text-align:center"));
                    break;
                case Align.Right:
                    node.AddAttribute(new HtmlAttribute("style", "text-align:right"));
                    break;
            }
        }

        public override HtmlNode ToHtmlNode()
        {
            HtmlNode table = new HtmlNode("table");
            HtmlNode tr = new HtmlNode("tr");
            table.AddElement(new HtmlNode("thead", tr));
#if NETCOREAPP
            string[] heads = head.Split('|', StringSplitOptions.RemoveEmptyEntries);
#else
            string[] heads = head.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
#endif
            int j = 0;
            for (int i = 0; i < heads.Length; i++)
            {
                string h = heads[i];
                while (h.Count(c => c == '`') % 2 != 0)
                {
                    i++;
                    h += '|' + heads[i];
                }
                HtmlNode th = new HtmlNode("th", GetHtmlObjects(h, GetTextTokens(h, 0)));
                AddAlign(th, headsAlign[j]);
                tr.AddElement(th);
                j++;
            }
            HtmlNode tbody = new HtmlNode("tbody");
            table.AddElement(tbody);
            foreach (string line in elements)
            {
#if NETCOREAPP
                string[] es = line.Split('|', StringSplitOptions.RemoveEmptyEntries);
#else
                string[] es = line.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
#endif
                tr = new HtmlNode("tr");
                tbody.AddElement(tr);
                j = 0;
                for (int i = 0; i < es.Length; i++)
                {
                    string e = es[i];
                    while (e.Count(c => c == '`') % 2 != 0)
                    {
                        i++;
                        e += '|' + es[i];
                    }
                    HtmlNode td = new HtmlNode("td", GetHtmlObjects(e, GetTextTokens(e, 0)));
                    AddAlign(td, headsAlign[j]);
                    tr.AddElement(td);
                    j++;
                }
            }
            return table;
        }
    }
}
