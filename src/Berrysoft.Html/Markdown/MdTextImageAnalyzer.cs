using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static Berrysoft.Html.Markdown.MdAnalyzerHelper;

namespace Berrysoft.Html.Markdown
{
    class MdTextImageAnalyzer : MdTextAnalyzer
    {
        public override IEnumerable<MdToken> GetTokens(string line)
        {
            var matches = PictureRegex.Matches(line);
            foreach (Match match in matches)
            {
                if (match.Index > 1)
                {
                    yield return new MdToken() { Index = match.Index - 1, Type = MdTokenType.Text };
                }
                yield return new MdToken() { Index = match.Index + match.Length - 1, Type = MdTokenType.Image };
            }
        }

#if NETCOREAPP2_1
        public override HtmlObject AnalyzeToken(ReadOnlyMemory<char> line, MdTokenType token)
        {
            switch (token)
            {
                case MdTokenType.Image:
                    var match = NoStartHyperlinkRegex.Match(line.ToString());
                    var node = new HtmlNode("img");
                    node.AddAttribute(new HtmlAttribute("src", match.Groups[2].Value));
                    node.AddAttribute(new HtmlAttribute("alt", match.Groups[1].Value));
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
                case MdTokenType.Image:
                    var match = NoStartHyperlinkRegex.Match(line);
                    var node = new HtmlNode("img");
                    node.AddAttribute(new HtmlAttribute("src", match.Groups[2].Value));
                    node.AddAttribute(new HtmlAttribute("alt", match.Groups[1].Value));
                    return node;
                default:
                    return line;
            }
        }
#endif
    }
}
