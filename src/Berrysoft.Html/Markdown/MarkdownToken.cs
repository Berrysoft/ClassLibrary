using System;

namespace Berrysoft.Html.Markdown
{
    enum MarkdownTokenType
    {
        None,
        Head,
        Text,
        Code,
        Strong,
        Italic,
        Picture,
        Hyperlink,
        ListItem
    }

    struct MarkdownToken
    {
        public int Index;
        public MarkdownTokenType Type;
    }

    enum MarkdownLineTokenType
    {
        None,
        Head,
        Paragraph,
        CodeBlock,
        List
    }

    struct MarkdownLineToken
    {
        public int Line;
        public string Value;
        public MarkdownLineTokenType Type;
        public MarkdownToken[] Tokens;
    }
}
