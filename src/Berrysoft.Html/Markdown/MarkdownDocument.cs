using System;
using System.Collections.Generic;
using System.IO;
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
                //HtmlDocument document = new HtmlDocument();
                //IEnumerable<MarkdownElement> elements = ParseLines(ReadLines(reader));
                //foreach (var e in elements)
                //{
                //    document.Body.AddElement(e.GetHtmlObject());
                //}
                //return document;
                lines.AddRange(ReadLines(reader));
            }
            List<MarkdownToken> tokens = new List<MarkdownToken>(GetTokens(lines));

        }

        private static IEnumerable<MarkdownToken> GetTokens(List<string> lines)
        {
            MarkdownTokenType blockType = MarkdownTokenType.None;
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                if (blockType == MarkdownTokenType.Code)
                {

                }
                else
                {
                    line = line.Trim();
                    if (line[0] == '#')
                    {
                        yield return new MarkdownToken() { Line = i, Index = -1, Type = MarkdownTokenType.None };
                        int j = 0;
                        for (; j < line.Length; j++)
                        {
                            if (line[j] != '#')
                                break;
                        }
                        yield return new MarkdownToken() { Line = i, Index = j - 1, Type = MarkdownTokenType.Head };
                        int k = line.Length - 1;
                        for (; k >= 0; k--)
                        {
                            if (line[k] != '#')
                                break;
                        }
                        foreach (var token in GetTextTokens(line.Substring(j, k - j + 1)))
                        {
                            var t = token;
                            t.Line = i;
                            t.Index += j;
                            yield return t;
                        }
                        yield return new MarkdownToken() { Line = i, Index = line.Length - 1, Type = MarkdownTokenType.Head };
                    }
                    else if (line.StartsWith("* "))
                    {
                        blockType = MarkdownTokenType.List;
                        yield return new MarkdownToken() { Line = i, Index = -1, Type = MarkdownTokenType.List };
                        yield return new MarkdownToken() { Line = i, Index = 1, Type = MarkdownTokenType.ListItem };
                        foreach (var token in GetTextTokens(line.Substring(2)))
                        {
                            var t = token;
                            t.Line = i;
                            t.Index += 2;
                            yield return t;
                        }
                    }
                    else if (line.StartsWith("```"))
                    {
                        blockType = MarkdownTokenType.CodeBlock;
                        yield return new MarkdownToken() { Line = i, Index = -1, Type = MarkdownTokenType.CodeBlock };
                    }
                }
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
        }

        private static IEnumerable<string> ReadLines(StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                yield return reader.ReadLine();
            }
        }

        private enum BlockType
        {
            None,
            Paragraph,
            List,
            Code
        }

        private static IEnumerable<MarkdownElement> ParseLines(IEnumerable<string> lines)
        {
            List<string> temp = new List<string>();
            BlockType type = BlockType.None;
            MarkdownElement te;

            MarkdownElement DealWithOldType()
            {
                MarkdownElement result = null;
                switch (type)
                {
                    case BlockType.Paragraph:
                        result = new MarkdownParagraph(temp);
                        temp.Clear();
                        break;
                    case BlockType.List:
                        result = new MarkdownList(temp);
                        temp.Clear();
                        break;
                    case BlockType.Code:
                        result = new MarkdownCode(temp);
                        temp.Clear();
                        break;
                    default:
                        result = null;
                        break;
                }
                return result;
            }

            foreach (string line in lines)
            {
                if (type == BlockType.Code)
                {
                    if (line.StartsWith("```"))
                    {
                        if ((te = DealWithOldType()) != null)
                        {
                            yield return te;
                        }
                        type = BlockType.None;
                    }
                    else
                    {
                        temp.Add(line);
                    }
                }
                else
                {
#if NETCOREAPP2_1
                    if (line.StartsWith('#'))
#else
                    if (line.StartsWith("#"))
#endif
                    {
                        if ((te = DealWithOldType()) != null)
                        {
                            yield return te;
                        }
                        type = BlockType.None;
                        int head = 1;
                        for (int i = 1; i < line.Length; i++)
                        {
                            if (line[i] != '#')
                            {
                                head = i;
                                break;
                            }
                        }
                        if (head == line.Length - 1)
                        {
                            continue;
                        }
                        int back = line.Length;
#if NETCOREAPP2_1
                        if (line.EndsWith('#'))
#else
                        if (line.EndsWith("#"))
#endif
                        {
                            for (int i = line.Length - 1; i >= 0; i--)
                            {
                                if (line[i] != '#')
                                {
                                    back = i + 1;
                                    break;
                                }
                            }
                            if (back < head)
                            {
                                back = line.Length;
                            }
                        }
                        if (back == head)
                        {
                            continue;
                        }
                        yield return new MarkdownHead(line.Substring(head, back - head).Trim(), head);
                    }
#if NETCOREAPP2_1
                    else if (line.StartsWith('*'))
#else
                    else if (line.StartsWith("*"))
#endif
                    {
                        if (type != BlockType.List)
                        {
                            if ((te = DealWithOldType()) != null)
                            {
                                yield return te;
                            }
                            type = BlockType.List;
                        }
                        temp.Add(line.Substring(1).Trim());
                    }
                    else if (line.StartsWith("```"))
                    {
                        if ((te = DealWithOldType()) != null)
                        {
                            yield return te;
                            type = BlockType.Code;
                        }
                    }
                    else
                    {
                        if (type == BlockType.None)
                        {
                            if ((te = DealWithOldType()) != null)
                            {
                                yield return te;
                            }
                            type = BlockType.Paragraph;
                        }
                        temp.Add(line.Trim());
                    }
                }
            }
            if ((te = DealWithOldType()) != null)
            {
                yield return te;
            }
        }
    }
}
