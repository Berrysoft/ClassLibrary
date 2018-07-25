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
        Image,
        Hyperlink,
        ListItem,
        TableBorder,
        TableDefaultAlign,
        TableLeftAlign,
        TableCenterAlign,
        TableRightAlign
    }

    struct MdToken
    {
        public int Index;
        public MdTokenType Type;
    }
}
