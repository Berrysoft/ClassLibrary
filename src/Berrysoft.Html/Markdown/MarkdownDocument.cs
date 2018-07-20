using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Berrysoft.Html.Markdown
{
    public class MarkdownDocument
    {
        public static HtmlDocument LoadAsHtml(string path)
        {
            List<string> lines = new List<string>();
            List<MarkdownLineToken> tokens = new List<MarkdownLineToken>();
            using (StreamReader reader = new StreamReader(path))
            {
                int index = 0;
                while(!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    lines.Add(line);
                    MarkdownLineToken token = GetToken(line);
                    token.Line = index;
                    tokens.Add(token);
                    index++;
                }
            }
            Stack<HtmlNode> currents = new Stack<HtmlNode>();
            List<HtmlObject> temp = new List<HtmlObject>();
            HtmlDocument document = new HtmlDocument();
            document.Head.AddElement(new HtmlNode("title", path));
            currents.Push(document.Body);
            MarkdownLineTokenType lineToken = tokens[0].Type;
            foreach (MarkdownLineToken token in tokens)
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
                        currents.Peek().AddElement(current);
                        current = currents.Pop();
                        currents.Peek().AddElement(current);
                        temp.Clear();
                        lineToken = MarkdownLineTokenType.None;
                    }
                    if (token.Tokens.Length > 0)
                    {
                        temp.Add(lines[token.Line]);
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
                            currents.Peek().AddElement(current);
                            temp.Clear();
                        }
                        switch (token.Type)
                        {
                            case MarkdownLineTokenType.Paragraph:
                                currents.Push(new HtmlNode("p"));
                                break;
                            case MarkdownLineTokenType.List:
                                currents.Push(new HtmlNode("ul"));
                                break;
                            case MarkdownLineTokenType.CodeBlock:
                                currents.Push(new HtmlNode("pre"));
                                currents.Push(new HtmlNode("code"));
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
                                objects = GetHtmlObjects(lines[token.Line].Substring(token.Tokens[0].Index + 1), token.Tokens.Skip(1));
                                currents.Push(new HtmlNode($"h{token.Tokens[0].Index + 1}", objects));
                                break;
                            case MarkdownTokenType.ListItem:
                                objects = GetHtmlObjects(lines[token.Line].Substring(token.Tokens[0].Index + 1), token.Tokens.Skip(1));
                                temp.Add(new HtmlNode("li", objects));
                                break;
                            default:
                                objects = GetHtmlObjects(lines[token.Line], token.Tokens);
                                temp.AddRange(objects);
                                temp.Add(" ");
                                break;
                        }
                    }
                }
            }
            while (currents.Count > 1)
            {
                HtmlNode current = currents.Pop();
                foreach (HtmlObject o in temp)
                {
                    current.AddElement(o);
                }
                temp.Clear();
                currents.Peek().AddElement(current);
            }
            return document;
        }

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
                    case MarkdownTokenType.Text:
                        yield return line.Substring(startIndex, token.Index - startIndex + 1);
                        break;
                }
                startIndex = token.Index + 1;
            }
        }

        private static readonly Regex HeadRegex = new Regex("[ ]*(#+[ ]+)([^#]+)#*");
        private static readonly Regex ListItemRegex = new Regex(@"[ ]*(\*[ ]+)(.*)");
        private static readonly Regex CodeBlockRegex = new Regex(@"(\`\`\`)(.*)");
        private static MarkdownLineToken GetToken(string line)
        {
            List<MarkdownToken> tokensList = new List<MarkdownToken>();
            MarkdownLineToken lineToken = default;
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
        private static IEnumerable<MarkdownLineToken> GetTokens(List<string> lines)
        {
            List<MarkdownToken> tokensList = new List<MarkdownToken>();
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                MarkdownLineToken lineToken = new MarkdownLineToken() { Line = i };
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
                yield return lineToken;
            }
        }

        private static readonly Regex InlineCodeRegex = new Regex(@"[ ]*([\`])([^`]+)([\`])");
        private static IEnumerable<MarkdownToken> GetTextTokens(string text)
        {
            var matches = InlineCodeRegex.Matches(text);
            foreach (Match match in matches)
            {
                if (match.Index > 1)
                {
                    yield return new MarkdownToken() { Index = match.Index - 1, Type = MarkdownTokenType.Text };
                }
                yield return new MarkdownToken() { Index = match.Groups[1].Index, Type = MarkdownTokenType.None };
                yield return new MarkdownToken() { Index = match.Groups[2].Index + match.Groups[2].Length - 1, Type = MarkdownTokenType.Code };
                yield return new MarkdownToken() { Index = match.Groups[3].Index, Type = MarkdownTokenType.None };
            }
            if (matches.Count > 0)
            {
                Match last = matches[matches.Count - 1];
                if (last.Index + last.Length >= text.Length)
                {
                    yield break;
                }
            }
            yield return new MarkdownToken() { Index = text.Length - 1, Type = MarkdownTokenType.Text };
        }
    }
}
