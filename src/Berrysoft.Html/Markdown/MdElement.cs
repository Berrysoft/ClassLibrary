using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Berrysoft.Html.Markdown
{
    abstract class MdElement
    {
        public abstract HtmlNode ToHtmlNode();
    }

    static class MdElementHelper
    {
        public static readonly Regex HeadRegex = new Regex(@"^(#+)([ ]+)([^#]+)#*$");
        public static readonly Regex ListItemRegex = new Regex(@"^(\*[ ]+)(.*)$");
        public static readonly Regex CodeBlockRegex = new Regex(@"^(\`\`\`)[ ]*(.*)$");
        public static readonly Regex CodeBlockIndentRegex = new Regex(@"^[ ]{4}");
        public static readonly Regex TableRegex = new Regex(@"[\|]?([^\|]+)");
        public static readonly Regex TableAlignRegex = new Regex(@"[\|]?(:?)-+(:?)");

        public static IEnumerable<MdElement> GetElements(string[] lines)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                else if (HeadRegex.IsMatch(line))
                {
                    yield return new MdHeadElement(line);
                }
                else if (ListItemRegex.IsMatch(line))
                {
                    yield return new MdListElement(lines, ref i);
                }
                else if (CodeBlockRegex.IsMatch(line) || CodeBlockIndentRegex.IsMatch(line))
                {
                    yield return new MdCodeElement(lines, ref i);
                }
#if NETCOREAPP2_1
                else if (TableRegex.IsMatch(line) && line.Contains('|'))
#else
                else if (TableRegex.IsMatch(line) && line.Contains("|"))
#endif
                {
                    yield return new MdTableElement(lines, ref i);
                }
                else
                {
                    yield return new MdParaElement(lines, ref i);
                }
            }
        }
    }
}
