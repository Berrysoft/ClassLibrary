using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Berrysoft.Html.Markdown
{
    abstract class MdAnalyzer
    {
        public abstract MdAnalyzer GetToken(string line, out MdLineToken token);

        public abstract HtmlNode AnalyzeToken(MdLineToken token, HtmlNode current);
    }

    abstract class MdTextAnalyzer
    {
        public abstract IEnumerable<MdToken> GetTokens(string line);

#if NETCOREAPP2_1
        public abstract HtmlObject AnalyzeToken(ReadOnlyMemory<char> line, Memory<MdToken> tokens);
#else
        public abstract HtmlObject AnalyzeToken(string line, IEnumerable<MdToken> tokens);
#endif
    }

    static class MdAnalyzerHelper
    {
        public static readonly Regex HeadRegex = new Regex(@"^[ ]*(#+[ ]+)([^#]+)#*$");
        public static readonly Regex ListItemRegex = new Regex(@"^[ ]*(\*[ ]+)(.*)$");
        public static readonly Regex CodeBlockRegex = new Regex(@"^[ ]*(\`\`\`)(.*)$");
        public static readonly Regex InlineCodeRegex = new Regex(@"([\`])([^`]+)([\`])");
        public static readonly Regex StrongRegex = new Regex(@"(\*\*)([^\*]+)(\*\*)");
        public static readonly Regex ItalicRegex = new Regex(@"[^\*](\*)([^\*\`]+)(\*)");
        public static readonly Regex HyperlinkRegex = new Regex(@"([^\!]|^)\[(.*)\]\((.*)\)");
        public static readonly Regex PictureRegex = new Regex(@"\!\[(.*)\]\((.*)\)");

        public static IEnumerable<MdToken> GetTextTokens(string text)
        {
            List<MdToken> result = new List<MdToken>();
            var matches = InlineCodeRegex.Matches(text);
            foreach (Match match in matches)
            {
                if (match.Index > 1)
                {
                    result.Add(new MdToken() { Index = match.Groups[1].Index - 1, Type = MdTokenType.Text });
                }
                result.Add(new MdToken() { Index = match.Groups[2].Index - 1, Type = MdTokenType.None });
                result.Add(new MdToken() { Index = match.Groups[3].Index - 1, Type = MdTokenType.Code });
                result.Add(new MdToken() { Index = match.Groups[3].Index, Type = MdTokenType.None });
            }
            matches = StrongRegex.Matches(text);
            foreach (Match match in matches)
            {
                if (match.Index > 1)
                {
                    result.Add(new MdToken() { Index = match.Groups[1].Index - 1, Type = MdTokenType.Text });
                }
                result.Add(new MdToken() { Index = match.Groups[2].Index - 1, Type = MdTokenType.None });
                result.Add(new MdToken() { Index = match.Groups[3].Index - 1, Type = MdTokenType.Strong });
                result.Add(new MdToken() { Index = match.Groups[3].Index + 1, Type = MdTokenType.None });
            }
            matches = ItalicRegex.Matches(text);
            foreach (Match match in matches)
            {
                if (match.Index > 1)
                {
                    result.Add(new MdToken() { Index = match.Groups[1].Index - 1, Type = MdTokenType.Text });
                }
                result.Add(new MdToken() { Index = match.Groups[2].Index - 1, Type = MdTokenType.None });
                result.Add(new MdToken() { Index = match.Groups[3].Index - 1, Type = MdTokenType.Italic });
                result.Add(new MdToken() { Index = match.Groups[3].Index, Type = MdTokenType.None });
            }
            matches = PictureRegex.Matches(text);
            foreach (Match match in matches)
            {
                if (match.Index > 1)
                {
                    result.Add(new MdToken() { Index = match.Index - 1, Type = MdTokenType.Text });
                }
                result.Add(new MdToken() { Index = match.Index + match.Length - 1, Type = MdTokenType.Picture });
            }
            matches = HyperlinkRegex.Matches(text);
            foreach (Match match in matches)
            {
                if (match.Groups[2].Index > 2)
                {
                    result.Add(new MdToken() { Index = match.Groups[2].Index - 2, Type = MdTokenType.Text });
                }
                result.Add(new MdToken() { Index = match.Index + match.Length - 1, Type = MdTokenType.Hyperlink });
            }
#if NETCOREAPP2_1
            if (!(text.EndsWith('*') || text.EndsWith('`') || text.EndsWith(')')))
#else
            if (!(text.EndsWith("*") || text.EndsWith("`") || text.EndsWith(")")))
#endif
            {
                result.Add(new MdToken() { Index = text.Length - 1, Type = MdTokenType.Text });
            }
            result.Sort((t1, t2) => t1.Index.CompareTo(t2.Index));
            return result;
        }


#if NETCOREAPP2_1
        public static IEnumerable<HtmlObject> GetHtmlObjects(ReadOnlyMemory<char> line, Memory<MdToken> tokens)
        {
            int startIndex = 0;
            for (int i = 0; i < tokens.Length; i++)
            {
                MdToken token = tokens.Span[i];
                switch (token.Type)
                {
                    case MdTokenType.Code:
                        yield return new HtmlNode("code", line.Slice(startIndex, token.Index - startIndex + 1).ToString());
                        break;
                    case MdTokenType.Strong:
                        yield return new HtmlNode("strong", line.Slice(startIndex, token.Index - startIndex + 1).ToString());
                        break;
                    case MdTokenType.Italic:
                        yield return new HtmlNode("em", line.Slice(startIndex, token.Index - startIndex + 1).ToString());
                        break;
                    case MdTokenType.Picture:
                        var match = PictureRegex.Match(line.ToString(), startIndex);
                        var node = new HtmlNode("img");
                        node.AddAttribute(new HtmlAttribute("src", match.Groups[2].Value));
                        node.AddAttribute(new HtmlAttribute("alt", match.Groups[1].Value));
                        yield return node;
                        break;
                    case MdTokenType.Hyperlink:
                        match = HyperlinkRegex.Match(line.ToString(), startIndex > 0 ? startIndex - 1 : startIndex);
                        node = new HtmlNode("a", match.Groups[2].Value);
                        node.AddAttribute(new HtmlAttribute("href", match.Groups[3].Value));
                        yield return node;
                        break;
                    case MdTokenType.Text:
                        yield return line.Slice(startIndex, token.Index - startIndex + 1).ToString();
                        break;
                }
                startIndex = token.Index + 1;
            }
        }
#else
        public static IEnumerable<HtmlObject> GetHtmlObjects(string line, IEnumerable<MdToken> tokens)
        {
            int startIndex = 0;
            foreach (MdToken token in tokens)
            {
                switch (token.Type)
                {
                    case MdTokenType.Code:
                        yield return new HtmlNode("code", line.Substring(startIndex, token.Index - startIndex + 1));
                        break;
                    case MdTokenType.Strong:
                        yield return new HtmlNode("strong", line.Substring(startIndex, token.Index - startIndex + 1));
                        break;
                    case MdTokenType.Italic:
                        yield return new HtmlNode("em", line.Substring(startIndex, token.Index - startIndex + 1));
                        break;
                    case MdTokenType.Picture:
                        var match = PictureRegex.Match(line, startIndex);
                        var node = new HtmlNode("img");
                        node.AddAttribute(new HtmlAttribute("src", match.Groups[2].Value));
                        node.AddAttribute(new HtmlAttribute("alt", match.Groups[1].Value));
                        yield return node;
                        break;
                    case MdTokenType.Hyperlink:
                        match = HyperlinkRegex.Match(line, startIndex > 0 ? startIndex - 1 : startIndex);
                        node = new HtmlNode("a", match.Groups[2].Value);
                        node.AddAttribute(new HtmlAttribute("href", match.Groups[3].Value));
                        yield return node;
                        break;
                    case MdTokenType.Text:
                        yield return line.Substring(startIndex, token.Index - startIndex + 1);
                        break;
                }
                startIndex = token.Index + 1;
            }
        }
#endif

        public static readonly MdHeadAnalyzer HeadAnalyzer = new MdHeadAnalyzer();
        public static readonly MdParaAnalyzer ParaAnalyzer = new MdParaAnalyzer();
        public static readonly MdListAnalyzer ListAnalyzer = new MdListAnalyzer();
        public static readonly MdCodeAnalyzer CodeAnalyzer = new MdCodeAnalyzer();

        public static MdAnalyzer GetStartAnalyzer() => ParaAnalyzer;

        public static MdAnalyzer GetAnalyzerFromToken(MdLineTokenType token)
        {
            switch (token)
            {
                case MdLineTokenType.Head:
                    return HeadAnalyzer;
                case MdLineTokenType.Para:
                case MdLineTokenType.ParaEnd:
                    return ParaAnalyzer;
                case MdLineTokenType.List:
                case MdLineTokenType.ListEnd:
                    return ListAnalyzer;
                case MdLineTokenType.Code:
                case MdLineTokenType.CodeEnd:
                    return CodeAnalyzer;
                default:
                    return null;
            }
        }
    }
}
