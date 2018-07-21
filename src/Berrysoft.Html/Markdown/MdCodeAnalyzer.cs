using System;
using System.Collections.Generic;
using static Berrysoft.Html.Markdown.MdAnalyzerHelper;

namespace Berrysoft.Html.Markdown
{
    class MdCodeAnalyzer : MdAnalyzer
    {
        public override MdAnalyzer GetToken(string line, out MdLineToken token)
        {
            token = new MdLineToken() { Value = line, Type = MdLineTokenType.Code };
            MdAnalyzer result = this;
            if (CodeBlockRegex.IsMatch(line))
            {
                token.Type = MdLineTokenType.CodeEnd;
                result = ParagraphAnalyzer;
            }
            token.Tokens = Array.Empty<MdToken>();
            return result;
        }

        public override HtmlNode AnalyzeToken(MdLineToken token, HtmlNode current)
        {
            switch (token.Type)
            {
                case MdLineTokenType.Code:
                    if (current.Name != "code")
                    {
                        HtmlNode code = new HtmlNode("code");
                        HtmlNode pre = new HtmlNode("pre", code);
                        current.AddElement(pre);
                        current = code;
                    }
                    else
                    {
                        current.AddElement(token.Value);
                        current.AddElement(Environment.NewLine);
                    }
                    return current;
                case MdLineTokenType.CodeEnd:
                    return current.Parent.Parent;
                default:
                    return null;
            }
        }
    }
}
