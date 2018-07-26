using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static Berrysoft.Html.Markdown.MdTextAnalyzerHelper;

namespace Berrysoft.Html.Markdown
{
    class MdTextImageAnalyzer : MdTextAnalyzer
    {
        public override IEnumerable<MdToken> GetTokens(string line, int offset)
        {
            var matches = PictureRegex.Matches(line);
            foreach (Match match in matches)
            {
                if (match.Index > 1)
                {
                    yield return new MdToken() { Index = match.Index - 1 + offset, Type = MdTokenType.Text };
                }
                yield return new MdToken() { Index = match.Index + match.Length - 1 + offset, Type = MdTokenType.Image };
            }
        }

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
                    return new HtmlString(line, HtmlEscapeOption.Auto);
            }
        }
    }
}
