using System;
using System.Collections.Generic;
using static Berrysoft.Html.Markdown.MdElementHelper;

namespace Berrysoft.Html.Markdown
{
    class MdCodeElement : MdElement
    {
        List<string> lines = new List<string>();
        string codeType;
        public MdCodeElement(string[] lines, ref int index)
        {
            var matches = CodeBlockRegex.Match(lines[index]).Groups;
            codeType = matches[2].Value;
            index++;
            for (; index < lines.Length; index++)
            {
                string line = lines[index];
                if (!CodeBlockRegex.IsMatch(line))
                {
                    this.lines.Add(line);
                }
                else
                {
                    break;
                }
            }
        }

        public override HtmlNode ToHtmlNode()
        {
            HtmlNode code = new HtmlNode("code", string.Join(Environment.NewLine, lines));
            code.AddAttribute(new HtmlAttribute("lang", codeType));
            return new HtmlNode("pre", code);
        }
    }
}
