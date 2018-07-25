using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static Berrysoft.Html.Markdown.MdTextAnalyzerHelper;

namespace Berrysoft.Html.Markdown
{
    class MdTextStrongAnalyzer : MdTextAnalyzer
    {
        public override IEnumerable<MdToken> GetTokens(string line, int offset)
        {
            var matches = StrongRegex.Matches(line);
            foreach (Match match in matches)
            {
                if (match.Index > 1)
                {
                    yield return new MdToken() { Index = match.Groups[1].Index - 1 + offset, Type = MdTokenType.Text };
                }
                yield return new MdToken() { Index = match.Groups[2].Index - 1 + offset, Type = MdTokenType.None };
                yield return new MdToken() { Index = match.Groups[3].Index - 1 + offset, Type = MdTokenType.Strong };
                yield return new MdToken() { Index = match.Groups[3].Index + 1 + offset, Type = MdTokenType.None };
            }
            matches = ItalicRegex.Matches(line);
            foreach (Match match in matches)
            {
                if (match.Index > 1)
                {
                    yield return new MdToken() { Index = match.Groups[1].Index - 1 + offset, Type = MdTokenType.Text };
                }
                yield return new MdToken() { Index = match.Groups[2].Index - 1 + offset, Type = MdTokenType.None };
                yield return new MdToken() { Index = match.Groups[3].Index - 1 + offset, Type = MdTokenType.Italic };
                yield return new MdToken() { Index = match.Groups[3].Index + offset, Type = MdTokenType.None };
            }
        }

        public override HtmlObject AnalyzeToken(string line, MdTokenType token)
        {
            switch (token)
            {
                case MdTokenType.Strong:
                    return new HtmlNode("strong", line);
                case MdTokenType.Italic:
                    return new HtmlNode("em", line);
                default:
                    return line;
            }
        }
    }
}
