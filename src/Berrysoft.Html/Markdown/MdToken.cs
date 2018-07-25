using System;

namespace Berrysoft.Html.Markdown
{
    enum MdTokenType
    {
        None,
        Text,
        Code,
        Strong,
        Italic,
        Image,
        Hyperlink
    }

    struct MdToken
    {
        public int Index;
        public MdTokenType Type;
    }
}
