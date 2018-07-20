using System;

namespace Berrysoft.Html.Markdown
{
    enum MarkdownTokenType
    {
        None,
        Head,
        Text,
        Code,
        ListItem,
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
        public MarkdownLineTokenType Type;
        public MarkdownToken[] Tokens;
    }
}
