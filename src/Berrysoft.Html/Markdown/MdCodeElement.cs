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
            if (CodeBlockRegex.IsMatch(lines[index]))
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
            else
            {
                for (; index < lines.Length; index++)
                {
                    string line = lines[index];
                    if (CodeBlockIndentRegex.IsMatch(line))
                    {
                        if (line.Length > 4)
                        {
                            this.lines.Add(line.Substring(4));
                        }
                        else
                        {
                            this.lines.Add(string.Empty);
                        }
                    }
                    else
                    {
                        index--;
                        break;
                    }
                }
            }
        }

        public override HtmlNode ToHtmlNode()
        {
            HtmlNode code = new HtmlNode("code", string.Join(Environment.NewLine, lines));
            if (codeType != null)
            {
                code.AddAttribute(new HtmlAttribute("lang", codeType));
            }
            return new HtmlNode("pre", code);
        }
    }
}
