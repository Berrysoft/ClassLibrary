using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Berrysoft.Html
{
    static class HtmlEscapeHelper
    {
        public static string EscapeAll(string str)
        {
            return str.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
        }

        private static readonly Regex EscapeRegex = new Regex(@"&([a-z]+|#[0-9]+);");
        private static readonly Regex HtmlLabelRegex = new Regex(@"<(/?[a-z]+[^>]*/?)>");
        public static string EscapeAuto(string str)
        {
            StringBuilder builder = new StringBuilder();
            int start = 0;
            int i;
            for (i = 0; i < str.Length; i++)
            {
                switch (str[i])
                {
                    case '&':
                        var matchText = EscapeRegex.Match(str, i);
                        if (!matchText.Success || matchText.Index != i)
                        {
                            builder.Append(str, start, i - start);
                            builder.Append("&amp;");
                            start = i + 1;
                        }
                        break;
                    case '<':
                        var match = HtmlLabelRegex.Match(str, i);
                        if (!match.Success || match.Index != i)
                        {
                            builder.Append(str, start, i - start);
                            builder.Append("&lt;");
                            start = i + 1;
                        }
                        else
                        {
                            builder.Append(str, start, i - start);
                            builder.Append(match.Value);
                            start = match.Index + match.Length;
                            i = start - 1;
                        }
                        break;
                    case '>':
                        builder.Append(str, start, i - start);
                        builder.Append("&gt;");
                        start = i + 1;
                        break;
                }
            }
            if (start < i)
            {
                builder.Append(str, start, i - start);
            }
            return builder.ToString();
        }
    }
}
