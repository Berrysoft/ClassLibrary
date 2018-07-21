using System;

namespace Berrysoft.Html.Markdown
{
    enum MdTokenType
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

    struct MdToken
    {
        public int Index;
        public MdTokenType Type;
    }

    enum MdLineTokenType
    {
        Head,
        Paragraph,
        ParagraphEnd,
        List,
        ListEnd,
        Code,
        CodeEnd
    }

    struct MdLineToken
    {
        public int Line;
        public string Value;
        public MdLineTokenType Type;
        public MdToken[] Tokens;
    }
}
