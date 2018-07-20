using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Berrysoft.Html.Markdown
{
    public class MarkdownDocument
    {
        private List<MarkdownLineToken> tokens;

        private MarkdownDocument()
        {
            tokens = new List<MarkdownLineToken>();
        }

        public static MarkdownDocument Load(string path)
        {
            MarkdownDocument document = new MarkdownDocument();
            using (StreamReader reader = new StreamReader(path))
            {
                int index = 0;
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    MarkdownLineToken token = GetToken(line);
                    token.Line = index;
                    document.tokens.Add(token);
                    index++;
                }
            }
            return document;
        }

        public static async Task<MarkdownDocument> LoadAsync(string path)
        {
            MarkdownDocument document = new MarkdownDocument();
            using (StreamReader reader = new StreamReader(path))
            {
                int index = 0;
                while (!reader.EndOfStream)
                {
                    string line = await reader.ReadLineAsync();
                    MarkdownLineToken token = GetToken(line);
                    token.Line = index;
                    document.tokens.Add(token);
                    index++;
                }
            }
            return document;
        }

        public HtmlDocument ToHtmlDocument()
        {
            Stack<HtmlNode> currents = new Stack<HtmlNode>();
            List<HtmlObject> temp = new List<HtmlObject>();
            HtmlDocument document = new HtmlDocument();
            currents.Push(document.Body);
            MarkdownLineTokenType lineToken = tokens[0].Type;
            foreach (var token in tokens)
            {
                if (lineToken == MarkdownLineTokenType.CodeBlock)
                {
                    if (token.Type == MarkdownLineTokenType.CodeBlock)
                    {
                        HtmlNode current = currents.Pop();
                        foreach (HtmlObject o in temp)
                        {
                            current.AddElement(o);
                        }
                        temp.Clear();
                        lineToken = MarkdownLineTokenType.None;
                    }
                    if (token.Tokens.Length > 0)
                    {
                        temp.Add(token.Value);
                        temp.Add(Environment.NewLine);
                    }
                }
                else
                {
                    if (lineToken != token.Type)
                    {
                        if (lineToken != MarkdownLineTokenType.None)
                        {
                            HtmlNode current = currents.Pop();
                            foreach (HtmlObject o in temp)
                            {
                                current.AddElement(o);
                            }
                            temp.Clear();
                        }
                        switch (token.Type)
                        {
                            case MarkdownLineTokenType.Paragraph:
                                HtmlNode n = new HtmlNode("p");
                                currents.Peek().AddElement(n);
                                currents.Push(n);
                                break;
                            case MarkdownLineTokenType.List:
                                n = new HtmlNode("ul");
                                currents.Peek().AddElement(n);
                                currents.Push(n);
                                break;
                            case MarkdownLineTokenType.CodeBlock:
                                n = new HtmlNode("code");
                                currents.Peek().AddElement(new HtmlNode("pre", n));
                                currents.Push(n);
                                break;
                        }
                    }
                    lineToken = token.Type;
                    IEnumerable<HtmlObject> objects = null;
                    if (token.Tokens.Length > 0)
                    {
                        switch (token.Tokens[0].Type)
                        {
                            case MarkdownTokenType.Head:
                                objects = GetHtmlObjects(
#if NETCOREAPP2_1
                                    token.Value.AsMemory().Slice(token.Tokens[0].Index + 1), token.Tokens.AsMemory().Slice(1));
#else
                                    token.Value.Substring(token.Tokens[0].Index + 1), token.Tokens.Skip(1));
#endif
                                HtmlNode n = new HtmlNode($"h{token.Tokens[0].Index + 1}", objects);
                                currents.Peek().AddElement(n);
                                currents.Push(n);
                                break;
                            case MarkdownTokenType.ListItem:
                                objects = GetHtmlObjects(
#if NETCOREAPP2_1
                                    token.Value.AsMemory().Slice(token.Tokens[0].Index + 1), token.Tokens.AsMemory().Slice(1));
#else
                                    token.Value.Substring(token.Tokens[0].Index + 1), token.Tokens.Skip(1));
#endif
                                temp.Add(new HtmlNode("li", objects));
                                break;
                            default:
                                objects = GetHtmlObjects(
#if NETCOREAPP2_1
                                    token.Value.AsMemory(),
#else
                                    token.Value,
#endif
                                    token.Tokens);
                                temp.AddRange(objects);
                                temp.Add(" ");
                                break;
                        }
                    }
                }
            }
            if (temp.Count > 0)
            {
                HtmlNode current = currents.Pop();
                foreach (HtmlObject o in temp)
                {
                    current.AddElement(o);
                }
                temp.Clear();
            }
            return document;
        }

#if NETCOREAPP2_1
        private static IEnumerable<HtmlObject> GetHtmlObjects(ReadOnlyMemory<char> line, Memory<MarkdownToken> tokens)
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
                    case MarkdownTokenType.Text:
                        yield return line.Slice(startIndex, token.Index - startIndex + 1).ToString();
                        break;
                }
                startIndex = token.Index + 1;
            }
        }
#else
        private static IEnumerable<HtmlObject> GetHtmlObjects(string line, IEnumerable<MarkdownToken> tokens)
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
                    case MarkdownTokenType.Text:
                        yield return line.Substring(startIndex, token.Index - startIndex + 1);
                        break;
                }
                startIndex = token.Index + 1;
            }
        }
#endif

        private static readonly Regex HeadRegex = new Regex(@"^[ ]*(#+[ ]+)([^#]+)#*$");
        private static readonly Regex ListItemRegex = new Regex(@"^[ ]*(\*[ ]+)(.*)$");
        private static readonly Regex CodeBlockRegex = new Regex(@"^[ ]*(\`\`\`)(.*)$");
        private static MarkdownLineToken GetToken(string line)
        {
            List<MarkdownToken> tokensList = new List<MarkdownToken>();
            MarkdownLineToken lineToken = new MarkdownLineToken() { Value = line };
            tokensList.Clear();
            if (line.Length == 0)
            {
                lineToken.Type = MarkdownLineTokenType.None;
            }
            else if (HeadRegex.IsMatch(line))
            {
                lineToken.Type = MarkdownLineTokenType.Head;
                var matches = HeadRegex.Match(line).Groups;
                if (matches.Count > 2)
                {
                    tokensList.Add(new MarkdownToken() { Index = matches[1].Index + matches[1].Length - 1, Type = MarkdownTokenType.Head });
                    tokensList.AddRange(GetTextTokens(matches[2].Value));
                }
            }
            else if (ListItemRegex.IsMatch(line))
            {
                lineToken.Type = MarkdownLineTokenType.List;
                var matches = ListItemRegex.Match(line).Groups;
                tokensList.Add(new MarkdownToken() { Index = matches[1].Index + matches[1].Length - 1, Type = MarkdownTokenType.ListItem });
                if (matches.Count > 2)
                {
                    tokensList.AddRange(GetTextTokens(matches[2].Value));
                }
            }
            else if (CodeBlockRegex.IsMatch(line))
            {
                lineToken.Type = MarkdownLineTokenType.CodeBlock;
            }
            else
            {
                lineToken.Type = MarkdownLineTokenType.Paragraph;
                tokensList.AddRange(GetTextTokens(line));
            }
            lineToken.Tokens = tokensList.ToArray();
            return lineToken;
        }

        private static readonly Regex InlineCodeRegex = new Regex(@"([\`])([^`]+)([\`])");
        private static readonly Regex StrongRegex = new Regex(@"(\*\*)([^\*]+)(\*\*)");
        private static readonly Regex ItalicRegex = new Regex(@"[^\*](\*)([^\*\`]+)(\*)");
        private static IEnumerable<MarkdownToken> GetTextTokens(string text)
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
#if NETCOREAPP2_1
            if (!(text.EndsWith('*') || text.EndsWith('`')))
#else
            if (!(text.EndsWith("*") || text.EndsWith("`")))
#endif
            {
                result.Add(new MarkdownToken() { Index = text.Length - 1, Type = MarkdownTokenType.Text });
            }
            result.Sort((t1, t2) => t1.Index.CompareTo(t2.Index));
            return result;
        }
    }
}
