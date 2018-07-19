using System;
using System.Collections.Generic;
using System.Linq;

namespace Berrysoft.Html.Markdown
{
    abstract class MarkdownElement
    {
        public MarkdownElement(string value)
        {
            this.value = value;
        }

        private string value;
        public string Value => value;

        public abstract HtmlObject GetHtmlObject();

        internal static IEnumerable<HtmlObject> ParseString(string line)
        {
            int index;
            bool in_code = false;
#if NETCOREAPP2_1
            ReadOnlyMemory<char> span = line.AsMemory();
            while ((index = span.Span.IndexOf('`')) > 0)
            {
                HtmlObject str = span.Slice(0, index).ToString();
                if (index + 1 >= span.Length)
                {
                    span = ReadOnlyMemory<char>.Empty;
                }
                else
                {
                    span = span.Slice(index + 1);
                }
                if (in_code)
                {
                    yield return new HtmlNode("code", str);
                }
                else
                {
                    yield return str;
                }
                in_code = !in_code;
            }
            if (!span.IsEmpty)
            {
                if (in_code)
                {
                    yield return new HtmlNode("code", span.ToString());
                }
                else
                {
                    yield return span.ToString();
                }
            }
#else
            while ((index = line.IndexOf('`')) > 0)
            {
                HtmlObject str = line.Substring(0, index);
                if (index + 1 >= line.Length)
                {
                    line = null;
                }
                else
                {
                    line = line.Substring(index + 1);
                }
                if (in_code)
                {
                    yield return new HtmlNode("code", str);
                }
                else
                {
                    yield return str;
                }
                in_code = !in_code;
            }
            if (line != null)
            {
                if (in_code)
                {
                    yield return new HtmlNode("code", line);
                }
                else
                {
                    yield return line;
                }
            }
#endif
        }
    }
    class MarkdownParagraph : MarkdownElement
    {
        public MarkdownParagraph(IEnumerable<string> line)
#if NETCOREAPP2_1
            : base(string.Join(' ', line))
#else
            : base(string.Join(" ", line))
#endif
        { }

        public override HtmlObject GetHtmlObject() => new HtmlNode("p", ParseString(Value));
    }
    class MarkdownCode : MarkdownElement
    {
        public MarkdownCode(IEnumerable<string> lines)
            : base(string.Join(Environment.NewLine, lines))
        {
            this.lines = lines.ToArray();
        }

        private string[] lines;

        public override HtmlObject GetHtmlObject()
            => new HtmlNode("pre", new HtmlNode("code", lines.Select(line => (new MarkdownCodeLine(line)).GetHtmlObject())));
    }
    class MarkdownCodeLine : MarkdownElement
    {
        public MarkdownCodeLine(string line)
            : base(line)
        { }

        public override HtmlObject GetHtmlObject() => Value + Environment.NewLine;
    }
    class MarkdownHead : MarkdownElement
    {
        public MarkdownHead(string head, int rank)
            : base(head)
        {
            this.rank = rank;
        }

        private int rank;
        public int Rank => rank;

        public override HtmlObject GetHtmlObject() => new HtmlNode($"h{rank}", ParseString(Value));
    }
    class MarkdownList : MarkdownElement
    {
        public MarkdownList(IEnumerable<string> lines)
            : base(string.Join(Environment.NewLine, lines))
        {
            this.lines = lines.ToArray();
        }

        private string[] lines;

        public override HtmlObject GetHtmlObject()
            => new HtmlNode("ul", lines.Select(line => (new MarkdownItem(line)).GetHtmlObject()));
    }
    class MarkdownItem : MarkdownElement
    {
        public MarkdownItem(string line)
            : base(line)
        { }

        public override HtmlObject GetHtmlObject() => new HtmlNode("li", ParseString(Value));
    }
}
