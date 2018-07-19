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
            using (StreamReader reader = new StreamReader(path))
            {
                HtmlDocument document = new HtmlDocument();
                IEnumerable<MarkdownElement> elements = ParseLines(ReadLines(reader));
                foreach (var e in elements)
                {
                    document.Body.AddElement(e.GetHtmlObject());
                }
                return document;
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
