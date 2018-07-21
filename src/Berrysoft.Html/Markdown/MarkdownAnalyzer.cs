using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Berrysoft.Html.Markdown
{
    abstract class MarkdownAnalyzer
    {
        protected static readonly Regex HeadRegex = new Regex(@"^[ ]*(#+[ ]+)([^#]+)#*$");
        protected static readonly Regex ListItemRegex = new Regex(@"^[ ]*(\*[ ]+)(.*)$");
        protected static readonly Regex CodeBlockRegex = new Regex(@"^[ ]*(\`\`\`)(.*)$");
        protected static readonly Regex InlineCodeRegex = new Regex(@"([\`])([^`]+)([\`])");
        protected static readonly Regex StrongRegex = new Regex(@"(\*\*)([^\*]+)(\*\*)");
        protected static readonly Regex ItalicRegex = new Regex(@"[^\*](\*)([^\*\`]+)(\*)");
        protected static readonly Regex HyperlinkRegex = new Regex(@"([^\!]|^)\[(.*)\]\((.*)\)");
        protected static readonly Regex PictureRegex = new Regex(@"\!\[(.*)\]\((.*)\)");

        public abstract MarkdownAnalyzer GetToken(string line, out MarkdownLineToken token);

        internal static IEnumerable<MarkdownToken> GetTextTokens(string text)
        {
            List<MarkdownToken> result = new List<MarkdownToken>();
            var matches = InlineCodeRegex.Matches(text);
            foreach (Match match in matches)
            {
                if (match.Index > 1)
                {
                    result.Add(new MarkdownToken() { Index = match.Groups[1].Index - 1, Type = MarkdownTokenType.Text });
                }
                result.Add(new MarkdownToken() { Index = match.Groups[2].Index - 1, Type = MarkdownTokenType.None });
                result.Add(new MarkdownToken() { Index = match.Groups[3].Index - 1, Type = MarkdownTokenType.Code });
                result.Add(new MarkdownToken() { Index = match.Groups[3].Index, Type = MarkdownTokenType.None });
            }
            matches = StrongRegex.Matches(text);
            foreach (Match match in matches)
            {
                if (match.Index > 1)
                {
                    result.Add(new MarkdownToken() { Index = match.Groups[1].Index - 1, Type = MarkdownTokenType.Text });
                }
                result.Add(new MarkdownToken() { Index = match.Groups[2].Index - 1, Type = MarkdownTokenType.None });
                result.Add(new MarkdownToken() { Index = match.Groups[3].Index - 1, Type = MarkdownTokenType.Strong });
                result.Add(new MarkdownToken() { Index = match.Groups[3].Index + 1, Type = MarkdownTokenType.None });
            }
            matches = ItalicRegex.Matches(text);
            foreach (Match match in matches)
            {
                if (match.Index > 1)
                {
                    result.Add(new MarkdownToken() { Index = match.Groups[1].Index - 1, Type = MarkdownTokenType.Text });
                }
                result.Add(new MarkdownToken() { Index = match.Groups[2].Index - 1, Type = MarkdownTokenType.None });
                result.Add(new MarkdownToken() { Index = match.Groups[3].Index - 1, Type = MarkdownTokenType.Italic });
                result.Add(new MarkdownToken() { Index = match.Groups[3].Index, Type = MarkdownTokenType.None });
            }
            matches = PictureRegex.Matches(text);
            foreach (Match match in matches)
            {
                if (match.Index > 1)
                {
                    result.Add(new MarkdownToken() { Index = match.Index - 1, Type = MarkdownTokenType.Text });
                }
                result.Add(new MarkdownToken() { Index = match.Index + match.Length - 1, Type = MarkdownTokenType.Picture });
            }
            matches = HyperlinkRegex.Matches(text);
            foreach (Match match in matches)
            {
                if (match.Groups[2].Index > 2)
                {
                    result.Add(new MarkdownToken() { Index = match.Groups[2].Index - 2, Type = MarkdownTokenType.Text });
                }
                result.Add(new MarkdownToken() { Index = match.Index + match.Length - 1, Type = MarkdownTokenType.Hyperlink });
            }
#if NETCOREAPP2_1
            if (!(text.EndsWith('*') || text.EndsWith('`') || text.EndsWith(')')))
#else
            if (!(text.EndsWith("*") || text.EndsWith("`") || text.EndsWith(")")))
#endif
            {
                result.Add(new MarkdownToken() { Index = text.Length - 1, Type = MarkdownTokenType.Text });
            }
            result.Sort((t1, t2) => t1.Index.CompareTo(t2.Index));
            return result;
        }


#if NETCOREAPP2_1
        internal static IEnumerable<HtmlObject> GetHtmlObjects(ReadOnlyMemory<char> line, Memory<MarkdownToken> tokens)
        {
            int startIndex = 0;
            for (int i = 0; i < tokens.Length; i++)
            {
                MarkdownToken token = tokens.Span[i];
                switch (token.Type)
                {
                    case MarkdownTokenType.Code:
                        yield return new HtmlNode("code", line.Slice(startIndex, token.Index - startIndex + 1).ToString());
                        break;
                    case MarkdownTokenType.Strong:
                        yield return new HtmlNode("strong", line.Slice(startIndex, token.Index - startIndex + 1).ToString());
                        break;
                    case MarkdownTokenType.Italic:
                        yield return new HtmlNode("i", line.Slice(startIndex, token.Index - startIndex + 1).ToString());
                        break;
                    case MarkdownTokenType.Picture:
                        var match = PictureRegex.Match(line.ToString(), startIndex);
                        var node = new HtmlNode("img");
                        node.AddAttribute(new HtmlAttribute("src", match.Groups[2].Value));
                        node.AddAttribute(new HtmlAttribute("alt", match.Groups[1].Value));
                        yield return node;
                        break;
                    case MarkdownTokenType.Hyperlink:
                        match = HyperlinkRegex.Match(line.ToString(), startIndex > 0 ? startIndex - 1 : startIndex);
                        node = new HtmlNode("a", match.Groups[2].Value);
                        node.AddAttribute(new HtmlAttribute("href", match.Groups[3].Value));
                        yield return node;
                        break;
                    case MarkdownTokenType.Text:
                        yield return line.Slice(startIndex, token.Index - startIndex + 1).ToString();
                        break;
                }
                startIndex = token.Index + 1;
            }
        }
#else
        internal static IEnumerable<HtmlObject> GetHtmlObjects(string line, IEnumerable<MarkdownToken> tokens)
        {
            int startIndex = 0;
            foreach (MarkdownToken token in tokens)
            {
                switch (token.Type)
                {
                    case MarkdownTokenType.Code:
                        yield return new HtmlNode("code", line.Substring(startIndex, token.Index - startIndex + 1));
                        break;
                    case MarkdownTokenType.Strong:
                        yield return new HtmlNode("strong", line.Substring(startIndex, token.Index - startIndex + 1));
                        break;
                    case MarkdownTokenType.Italic:
                        yield return new HtmlNode("i", line.Substring(startIndex, token.Index - startIndex + 1));
                        break;
                    case MarkdownTokenType.Picture:
                        var match = PictureRegex.Match(line, startIndex);
                        var node = new HtmlNode("img");
                        node.AddAttribute(new HtmlAttribute("src", match.Groups[2].Value));
                        node.AddAttribute(new HtmlAttribute("alt", match.Groups[1].Value));
                        yield return node;
                        break;
                    case MarkdownTokenType.Hyperlink:
                        match = HyperlinkRegex.Match(line, startIndex > 0 ? startIndex - 1 : startIndex);
                        node = new HtmlNode("a", match.Groups[2].Value);
                        node.AddAttribute(new HtmlAttribute("href", match.Groups[3].Value));
                        yield return node;
                        break;
                    case MarkdownTokenType.Text:
                        yield return line.Substring(startIndex, token.Index - startIndex + 1);
                        break;
                }
                startIndex = token.Index + 1;
            }
        }
#endif

        protected static readonly MarkdownHeadAnalyzer HeadAnalyzer = new MarkdownHeadAnalyzer();
        protected static readonly MarkdownParagraphAnalyzer ParagraphAnalyzer = new MarkdownParagraphAnalyzer();
        protected static readonly MarkdownListAnalyzer ListAnalyzer = new MarkdownListAnalyzer();
        protected static readonly MarkdownCodeAnalyzer CodeAnalyzer = new MarkdownCodeAnalyzer();

        public static MarkdownAnalyzer GetStartAnalyzer() => ParagraphAnalyzer;

        public abstract HtmlNode AnalyzeToken(MarkdownLineToken token, HtmlNode current);

        public static MarkdownAnalyzer GetAnalyzerFromToken(MarkdownLineTokenType token)
        {
            switch (token)
            {
                case MarkdownLineTokenType.Head:
                    return HeadAnalyzer;
                case MarkdownLineTokenType.Paragraph:
                case MarkdownLineTokenType.ParagraphEnd:
                    return ParagraphAnalyzer;
                case MarkdownLineTokenType.List:
                case MarkdownLineTokenType.ListEnd:
                    return ListAnalyzer;
                case MarkdownLineTokenType.Code:
                case MarkdownLineTokenType.CodeEnd:
                    return CodeAnalyzer;
                default:
                    return null;
            }
        }
    }

    class MarkdownHeadAnalyzer : MarkdownAnalyzer
    {
        public override MarkdownAnalyzer GetToken(string line, out MarkdownLineToken token)
        {
            token = new MarkdownLineToken() { Value = line, Type = MarkdownLineTokenType.Head };
            List<MarkdownToken> tokensList = new List<MarkdownToken>();
            var matches = HeadRegex.Match(line).Groups;
            if (matches.Count > 2)
            {
                tokensList.Add(new MarkdownToken() { Index = matches[1].Index + matches[1].Length - 1, Type = MarkdownTokenType.Head });
                tokensList.AddRange(GetTextTokens(matches[2].Value));
            }
            token.Tokens = tokensList.ToArray();
            return ParagraphAnalyzer;
        }

        public override HtmlNode AnalyzeToken(MarkdownLineToken token, HtmlNode current)
        {
            switch (token.Type)
            {
                case MarkdownLineTokenType.Head:
                    if (current.Name == "p" || current.Name == "ul")
                    {
                        current = current.Parent;
                    }
#if NETCOREAPP2_1
                    HtmlNode node = new HtmlNode($"h{token.Tokens[0].Index + 1}", GetHtmlObjects(token.Value.AsMemory().Slice(token.Tokens[0].Index + 1), token.Tokens.AsMemory().Slice(1)));
#else
                    HtmlNode node = new HtmlNode($"h{token.Tokens[0].Index + 1}", GetHtmlObjects(token.Value.Substring(token.Tokens[0].Index + 1), token.Tokens.Skip(1)));
#endif
                    current.AddElement(node);
                    return current;
                default:
                    return null;
            }
        }
    }

    class MarkdownParagraphAnalyzer : MarkdownAnalyzer
    {
        public override MarkdownAnalyzer GetToken(string line, out MarkdownLineToken token)
        {
            List<MarkdownToken> tokensList = new List<MarkdownToken>();
            token = new MarkdownLineToken() { Value = line };
            MarkdownAnalyzer result = this;
            if (line.Length == 0)
            {
                token.Type = MarkdownLineTokenType.ParagraphEnd;
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
                token.Type = MarkdownLineTokenType.Code;
                result = CodeAnalyzer;
            }
            else
            {
                token.Type = MarkdownLineTokenType.Paragraph;
                tokensList.AddRange(GetTextTokens(line));
            }
            token.Tokens = tokensList.ToArray();
            return result;
        }

        public override HtmlNode AnalyzeToken(MarkdownLineToken token, HtmlNode current)
        {
            switch (token.Type)
            {
                case MarkdownLineTokenType.Paragraph:
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
                case MarkdownLineTokenType.ParagraphEnd:
                    return current.Parent;
                default:
                    return null;
            }
        }
    }

    class MarkdownListAnalyzer : MarkdownAnalyzer
    {
        public override MarkdownAnalyzer GetToken(string line, out MarkdownLineToken token)
        {
            token = new MarkdownLineToken() { Value = line, Type = MarkdownLineTokenType.List };
            MarkdownAnalyzer result = this;
            List<MarkdownToken> tokensList = new List<MarkdownToken>();
            if (line.Length == 0)
            {
                token.Type = MarkdownLineTokenType.ListEnd;
                result = ParagraphAnalyzer;
            }
            else if (HeadRegex.IsMatch(line))
            {
                return ParagraphAnalyzer.GetToken(line, out token);
            }
            else
            {
                token.Type = MarkdownLineTokenType.List;
                var matches = ListItemRegex.Match(line).Groups;
                tokensList.Add(new MarkdownToken() { Index = matches[1].Index + matches[1].Length - 1, Type = MarkdownTokenType.ListItem });
                if (matches.Count > 2)
                {
                    tokensList.AddRange(GetTextTokens(matches[2].Value));
                }
            }
            token.Tokens = tokensList.ToArray();
            return result;
        }

        public override HtmlNode AnalyzeToken(MarkdownLineToken token, HtmlNode current)
        {
            switch (token.Type)
            {
                case MarkdownLineTokenType.ListEnd:
                    return current.Parent;
                case MarkdownLineTokenType.List:
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

    class MarkdownCodeAnalyzer : MarkdownAnalyzer
    {
        public override MarkdownAnalyzer GetToken(string line, out MarkdownLineToken token)
        {
            token = new MarkdownLineToken() { Value = line, Type = MarkdownLineTokenType.Code };
            MarkdownAnalyzer result = this;
            if (CodeBlockRegex.IsMatch(line))
            {
                token.Type = MarkdownLineTokenType.CodeEnd;
                result = ParagraphAnalyzer;
            }
            token.Tokens = Array.Empty<MarkdownToken>();
            return result;
        }

        public override HtmlNode AnalyzeToken(MarkdownLineToken token, HtmlNode current)
        {
            switch (token.Type)
            {
                case MarkdownLineTokenType.Code:
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
                case MarkdownLineTokenType.CodeEnd:
                    return current.Parent.Parent;
                default:
                    return null;
            }
        }
    }
}
