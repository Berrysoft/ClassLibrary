using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Berrysoft.Html.Markdown
{
    public class MarkdownDocument
    {
        public static HtmlDocument LoadAsHtml(string path)
        {
            List<string> lines = new List<string>();
            using (StreamReader reader = new StreamReader(path))
            {
                lines.AddRange(ReadLines(reader));
            }
            List<MarkdownLineToken> tokens = new List<MarkdownLineToken>(GetTokens(lines));
            Stack<HtmlNode> currents = new Stack<HtmlNode>();
            List<HtmlObject> temp = new List<HtmlObject>();
            HtmlDocument document = new HtmlDocument();
            document.Head.AddElement(new HtmlNode("title", path));
            currents.Push(document.Body);
            MarkdownLineTokenType lineToken = tokens[0].Type;
            foreach (MarkdownLineToken token in tokens)
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
                        if (lineToken == MarkdownLineTokenType.CodeBlock)
                        {
                            current = currents.Pop();
                            currents.Peek().AddElement(current);
                        }
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
                        case MarkdownTokenType.Code:
                            temp.Add(lines[token.Line]);
                            temp.Add(Environment.NewLine);
                            break;
                        default:
                            objects = GetHtmlObjects(lines[token.Line], token.Tokens);
                            temp.AddRange(objects);
                            break;
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
            bool in_code = false;
            foreach (MarkdownToken token in tokens)
            {
                switch (token.Type)
                {
                    case MarkdownTokenType.Code:
                        startIndex = token.Index + 1;
                        in_code = !in_code;
                        break;
                    case MarkdownTokenType.Text:
                        if (in_code)
                        {
                            yield return new HtmlNode("code", line.Substring(startIndex, token.Index - startIndex + 1));
                        }
                        else
                        {
                            yield return line.Substring(startIndex, token.Index - startIndex + 1);
                        }
                        break;
                }
            }
        }

        private static IEnumerable<MarkdownLineToken> GetTokens(List<string> lines)
        {
            MarkdownLineTokenType blockType = MarkdownLineTokenType.None;
            List<MarkdownToken> tokensList = new List<MarkdownToken>();
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                MarkdownLineToken lineToken = new MarkdownLineToken() { Line = i };
                tokensList.Clear();
                if (blockType == MarkdownLineTokenType.CodeBlock)
                {
                    if (line.StartsWith("```"))
                    {
                        blockType = MarkdownLineTokenType.None;
                        lineToken.Type = MarkdownLineTokenType.CodeBlock;
                    }
                    else
                    {
                        blockType = MarkdownLineTokenType.CodeBlock;
                        lineToken.Type = MarkdownLineTokenType.CodeBlock;
                        tokensList.Add(new MarkdownToken() { Index = line.Length - 1, Type = MarkdownTokenType.Code });
                    }
                }
                else
                {
                    line = line.Trim();
#if NETCOREAPP2_1
                    if (line.StartsWith('#'))
#else
                    if (line.StartsWith("#"))
#endif
                    {
                        blockType = MarkdownLineTokenType.Head;
                        lineToken.Type = MarkdownLineTokenType.Head;
                        int j = 0;
                        for (; j < line.Length; j++)
                        {
                            if (line[j] != '#')
                                break;
                        }
                        tokensList.Add(new MarkdownToken() { Index = j - 1, Type = MarkdownTokenType.Head });
                        int k = line.Length - 1;
                        for (; k >= 0; k--)
                        {
                            if (line[k] != '#')
                                break;
                        }
                        tokensList.AddRange(GetTextTokens(line.Substring(j, k - j + 1)));
                    }
                    else if (line.StartsWith("* "))
                    {
                        blockType = MarkdownLineTokenType.List;
                        lineToken.Type = MarkdownLineTokenType.List;
                        tokensList.Add(new MarkdownToken() { Index = 1, Type = MarkdownTokenType.ListItem });
                        tokensList.AddRange(GetTextTokens(line.Substring(2)));
                    }
                    else if (line.StartsWith("```"))
                    {
                        blockType = MarkdownLineTokenType.CodeBlock;
                        lineToken.Type = MarkdownLineTokenType.CodeBlock;
                    }
                    else if (line.Length == 0)
                    {
                        blockType = MarkdownLineTokenType.Paragraph;
                        lineToken.Type = MarkdownLineTokenType.None;
                    }
                    else
                    {
                        blockType = MarkdownLineTokenType.Paragraph;
                        lineToken.Type = MarkdownLineTokenType.Paragraph;
                        tokensList.AddRange(GetTextTokens(line));
                    }
                }
                lineToken.Tokens = tokensList.ToArray();
                yield return lineToken;
            }
        }

        private static IEnumerable<MarkdownToken> GetTextTokens(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '`')
                {
                    if (i > 1)
                    {
                        yield return new MarkdownToken() { Index = i - 1, Type = MarkdownTokenType.Text };
                    }
                    yield return new MarkdownToken() { Index = i, Type = MarkdownTokenType.Code };
                }
            }
            if (text.Length > 0 && text[text.Length - 1] != '`')
            {
                yield return new MarkdownToken() { Index = text.Length - 1, Type = MarkdownTokenType.Text };
            }
        }

        private static IEnumerable<string> ReadLines(StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                yield return reader.ReadLine();
            }
        }
    }
}
