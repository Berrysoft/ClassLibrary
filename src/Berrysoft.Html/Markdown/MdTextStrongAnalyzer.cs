using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static Berrysoft.Html.Markdown.MdAnalyzerHelper;

namespace Berrysoft.Html.Markdown
{
    class MdTextStrongAnalyzer : MdTextAnalyzer
    {
        public override IEnumerable<MdToken> GetTokens(string line)
        {
            var matches = StrongRegex.Matches(line);
            foreach (Match match in matches)
            {
                if (match.Index > 1)
                {
                    yield return new MdToken() { Index = match.Groups[1].Index - 1, Type = MdTokenType.Text };
                }
                yield return new MdToken() { Index = match.Groups[2].Index - 1, Type = MdTokenType.None };
                yield return new MdToken() { Index = match.Groups[3].Index - 1, Type = MdTokenType.Strong };
                yield return new MdToken() { Index = match.Groups[3].Index + 1, Type = MdTokenType.None };
            }
            matches = ItalicRegex.Matches(line);
            foreach (Match match in matches)
            {
                if (match.Index > 1)
                {
                    yield return new MdToken() { Index = match.Groups[1].Index - 1, Type = MdTokenType.Text };
                }
                yield return new MdToken() { Index = match.Groups[2].Index - 1, Type = MdTokenType.None };
                yield return new MdToken() { Index = match.Groups[3].Index - 1, Type = MdTokenType.Italic };
                yield return new MdToken() { Index = match.Groups[3].Index, Type = MdTokenType.None };
            }
        }

#if NETCOREAPP2_1
        public override HtmlObject AnalyzeToken(ReadOnlyMemory<char> line, MdTokenType token)
        {
            switch (token)
            {
                case MdTokenType.Strong:
                    return new HtmlNode("strong", line.ToString());
                case MdTokenType.Italic:
                    return new HtmlNode("em", line.ToString());
                default:
                    return line.ToString();
            }
        }
#else
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
#endif
    }
}
