using System;
using System.Collections.Generic;
using System.Linq;
using static Berrysoft.Html.Markdown.MdAnalyzerHelper;

namespace Berrysoft.Html.Markdown
{
    class MdListAnalyzer : MdAnalyzer
    {
        public override MdAnalyzer GetToken(string line, out MdLineToken token)
        {
            token = new MdLineToken() { Value = line, Type = MdLineTokenType.List };
            MdAnalyzer result = this;
            List<MdToken> tokensList = new List<MdToken>();
            if (line.Length == 0)
            {
                token.Type = MdLineTokenType.ListEnd;
                result = ParagraphAnalyzer;
            }
            else if (HeadRegex.IsMatch(line))
            {
                return ParagraphAnalyzer.GetToken(line, out token);
            }
            else
            {
                token.Type = MdLineTokenType.List;
                var matches = ListItemRegex.Match(line).Groups;
                tokensList.Add(new MdToken() { Index = matches[1].Index + matches[1].Length - 1, Type = MdTokenType.ListItem });
                if (matches.Count > 2)
                {
                    tokensList.AddRange(GetTextTokens(matches[2].Value));
                }
            }
            token.Tokens = tokensList.ToArray();
            return result;
        }

        public override HtmlNode AnalyzeToken(MdLineToken token, HtmlNode current)
        {
            switch (token.Type)
            {
                case MdLineTokenType.ListEnd:
                    return current.Parent;
                case MdLineTokenType.List:
                    if (current.Name == "p")
                    {
                        current = current.Parent;
                    }
                    if (current.Name != "ul")
                    {
                        HtmlNode ul = new HtmlNode("ul");
                        current.AddElement(ul);
                        current = ul;
                    }
#if NETCOREAPP2_1
                    HtmlNode node = new HtmlNode("li", GetHtmlObjects(token.Value.AsMemory().Slice(token.Tokens[0].Index + 1), token.Tokens.AsMemory().Slice(1)));
#else
                    HtmlNode node = new HtmlNode("li", GetHtmlObjects(token.Value.Substring(token.Tokens[0].Index + 1), token.Tokens.Skip(1)));
#endif
                    current.AddElement(node);
                    return current;
                default:
                    return null;
            }
        }
    }
}
