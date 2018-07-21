using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static Berrysoft.Html.Markdown.MdAnalyzerHelper;

namespace Berrysoft.Html.Markdown
{
    class MdTextCodeAnalyzer : MdTextAnalyzer
    {
        public override IEnumerable<MdToken> GetTokens(string line, int offset)
        {
            var matches = InlineCodeRegex.Matches(line);
            foreach (Match match in matches)
            {
                if (match.Index > 1)
                {
                    yield return new MdToken() { Index = match.Groups[1].Index - 1 + offset, Type = MdTokenType.Text };
                }
                yield return new MdToken() { Index = match.Groups[2].Index - 1 + offset, Type = MdTokenType.None };
                yield return new MdToken() { Index = match.Groups[3].Index - 1 + offset, Type = MdTokenType.Code };
                yield return new MdToken() { Index = match.Groups[3].Index + offset, Type = MdTokenType.None };
            }
        }

        public override HtmlObject AnalyzeToken(string line, MdTokenType token)
        {
            switch (token)
            {
                case MdTokenType.Code:
                    return new HtmlNode("code", line);
                default:
                    return line;
            }
        }
    }
}
