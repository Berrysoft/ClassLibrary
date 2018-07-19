using System;

namespace Berrysoft.Html.Markdown
{
    enum MarkdownTokenType
    {
        None,
        Head,
        Text,
        Paragraph,
        Code,
        CodeBlock,
        ListItem,
        List
    }

    struct MarkdownToken
    {
        public int Line;
        public int Index;
        public MarkdownTokenType Type;
    }
}
