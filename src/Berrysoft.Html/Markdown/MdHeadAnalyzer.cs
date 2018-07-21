using System;
using System.Collections.Generic;
using static Berrysoft.Html.Markdown.MdAnalyzerHelper;

namespace Berrysoft.Html.Markdown
{
    class MdHeadAnalyzer : MdAnalyzer
    {
        public override MdAnalyzer GetToken(string line, out MdLineToken token)
        {
            token = new MdLineToken() { Value = line, Type = MdLineTokenType.Head };
            List<MdToken> tokensList = new List<MdToken>();
            var matches = HeadRegex.Match(line).Groups;
            if (matches.Count > 2)
            {
                tokensList.Add(new MdToken() { Index = matches[1].Index + matches[1].Length - 1, Type = MdTokenType.Head });
                tokensList.AddRange(GetTextTokens(line, matches[2].Index));
            }
            token.Tokens = tokensList.ToArray();
            return ParaAnalyzer;
        }

        public override HtmlNode AnalyzeToken(MdLineToken token, HtmlNode current)
        {
            switch (token.Type)
            {
                case MdLineTokenType.Head:
                    if (current.Name == "p" || current.Name == "ul")
                    {
                        current = current.Parent;
                    }
                    HtmlNode node = new HtmlNode($"h{token.Tokens[0].Index + 1}", GetHtmlObjects(token.Value, token.Tokens));
                    current.AddElement(node);
                    return current;
                default:
                    return null;
            }
        }
    }
}
