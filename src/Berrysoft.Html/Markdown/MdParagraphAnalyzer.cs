using System;
using System.Collections.Generic;
using static Berrysoft.Html.Markdown.MdAnalyzerHelper;

namespace Berrysoft.Html.Markdown
{
    class MdParagraphAnalyzer : MdAnalyzer
    {
        public override MdAnalyzer GetToken(string line, out MdLineToken token)
        {
            List<MdToken> tokensList = new List<MdToken>();
            token = new MdLineToken() { Value = line };
            MdAnalyzer result = this;
            if (line.Length == 0)
            {
                token.Type = MdLineTokenType.ParagraphEnd;
            }
            else if (HeadRegex.IsMatch(line))
            {
                return HeadAnalyzer.GetToken(line, out token);
            }
            else if (ListItemRegex.IsMatch(line))
            {
                return ListAnalyzer.GetToken(line, out token);
            }
            else if (CodeBlockRegex.IsMatch(line))
            {
                token.Type = MdLineTokenType.Code;
                result = CodeAnalyzer;
            }
            else
            {
                token.Type = MdLineTokenType.Paragraph;
                tokensList.AddRange(GetTextTokens(line));
            }
            token.Tokens = tokensList.ToArray();
            return result;
        }

        public override HtmlNode AnalyzeToken(MdLineToken token, HtmlNode current)
        {
            switch (token.Type)
            {
                case MdLineTokenType.Paragraph:
                    if (current.Name != "p")
                    {
                        HtmlNode p = new HtmlNode("p");
                        current.AddElement(p);
                        current = p;
                    }
#if NETCOREAPP2_1
                    foreach (var obj in GetHtmlObjects(token.Value.AsMemory(), token.Tokens))
#else
                    foreach (var obj in GetHtmlObjects(token.Value, token.Tokens))
#endif
                    {
                        current.AddElement(obj);
                    }
                    current.AddElement(" ");
                    return current;
                case MdLineTokenType.ParagraphEnd:
                    return current.Parent;
                default:
                    return null;
            }
        }
    }
}
