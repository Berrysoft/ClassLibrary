using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static Berrysoft.Html.Markdown.MdTextAnalyzerHelper;

namespace Berrysoft.Html.Markdown
{
    class MdTextHyperlinkAnalyzer : MdTextAnalyzer
    {
        public override IEnumerable<MdToken> GetTokens(string line, int offset)
        {
            var matches = HyperlinkRegex.Matches(line);
            foreach (Match match in matches)
            {
                if (match.Groups[2].Index > 2)
                {
                    yield return new MdToken() { Index = match.Groups[2].Index - 2 + offset, Type = MdTokenType.Text };
                }
                yield return new MdToken() { Index = match.Index + match.Length - 1 + offset, Type = MdTokenType.Hyperlink };
            }
        }

        public override HtmlObject AnalyzeToken(string line, MdTokenType token)
        {
            switch (token)
            {
                case MdTokenType.Hyperlink:
                    var match = NoStartHyperlinkRegex.Match(line);
                    var node = new HtmlNode("a", match.Groups[1].Value);
                    node.AddAttribute(new HtmlAttribute("href", match.Groups[2].Value));
                    return node;
                default:
                    return line;
            }
        }
    }
}
