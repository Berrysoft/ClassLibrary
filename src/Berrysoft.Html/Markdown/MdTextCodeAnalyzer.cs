using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static Berrysoft.Html.Markdown.MdAnalyzerHelper;

namespace Berrysoft.Html.Markdown
{
    class MdTextCodeAnalyzer : MdTextAnalyzer
    {
        public override IEnumerable<MdToken> GetTokens(string line)
        {
            var matches = InlineCodeRegex.Matches(line);
            foreach (Match match in matches)
            {
                if (match.Index > 1)
                {
                    yield return new MdToken() { Index = match.Groups[1].Index - 1, Type = MdTokenType.Text };
                }
                yield return new MdToken() { Index = match.Groups[2].Index - 1, Type = MdTokenType.None };
                yield return new MdToken() { Index = match.Groups[3].Index - 1, Type = MdTokenType.Code };
                yield return new MdToken() { Index = match.Groups[3].Index, Type = MdTokenType.None };
            }
        }

#if NETCOREAPP2_1
        public override HtmlObject AnalyzeToken(ReadOnlyMemory<char> line, MdTokenType token)
        {
            switch (token)
            {
                case MdTokenType.Code:
                    return new HtmlNode("code", line.ToString());
                default:
                    return line.ToString();
            }
        }
#else
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
#endif
    }
}
