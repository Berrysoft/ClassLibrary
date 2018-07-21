using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static Berrysoft.Html.Markdown.MdAnalyzerHelper;

namespace Berrysoft.Html.Markdown
{
    class MdTextHyperlinkAnalyzer : MdTextAnalyzer
    {
        public override IEnumerable<MdToken> GetTokens(string line)
        {
            var matches = HyperlinkRegex.Matches(line);
            foreach (Match match in matches)
            {
                if (match.Groups[2].Index > 2)
                {
                    yield return new MdToken() { Index = match.Groups[2].Index - 2, Type = MdTokenType.Text };
                }
                yield return new MdToken() { Index = match.Index + match.Length - 1, Type = MdTokenType.Hyperlink };
            }
        }

#if NETCOREAPP2_1
        public override HtmlObject AnalyzeToken(ReadOnlyMemory<char> line, MdTokenType token)
        {
            switch (token)
            {
                case MdTokenType.Hyperlink:
                    var match = NoStartHyperlinkRegex.Match(line.ToString());
                    var node = new HtmlNode("a", match.Groups[1].Value);
                    node.AddAttribute(new HtmlAttribute("href", match.Groups[2].Value));
                    return node;
                default:
                    return line.ToString();
            }
        }
#else
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
#endif
    }
}
