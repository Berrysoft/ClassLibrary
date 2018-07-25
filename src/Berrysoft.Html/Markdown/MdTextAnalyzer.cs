using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Berrysoft.Html.Markdown
{
    abstract class MdTextAnalyzer
    {
        public abstract IEnumerable<MdToken> GetTokens(string line, int offset);

        public abstract HtmlObject AnalyzeToken(string line, MdTokenType token);
    }

    static class MdTextAnalyzerHelper
    {
        public static readonly Regex InlineCodeRegex = new Regex(@"([\`])([^`]+)([\`])");
        public static readonly Regex StrongRegex = new Regex(@"(\*\*)([^\*]+)(\*\*)");
        public static readonly Regex ItalicRegex = new Regex(@"[^\*](\*)([^\*\`]+)(\*)");
        public static readonly Regex HyperlinkRegex = new Regex(@"([^\!]|^)\[(.*)\]\((.*)\)");
        public static readonly Regex PictureRegex = new Regex(@"\!\[(.*)\]\((.*)\)");
        public static readonly Regex NoStartHyperlinkRegex = new Regex(@"\[(.*)\]\((.*)\)");

        public static readonly MdTextCodeAnalyzer TextCodeAnalyzer = new MdTextCodeAnalyzer();
        public static readonly MdTextStrongAnalyzer StrongAnalyzer = new MdTextStrongAnalyzer();
        public static readonly MdTextHyperlinkAnalyzer HyperlinkAnalyzer = new MdTextHyperlinkAnalyzer();
        public static readonly MdTextImageAnalyzer ImageAnalyzer = new MdTextImageAnalyzer();

        public static IEnumerable<MdToken> GetTextTokens(string text, int offset)
        {
            List<MdToken> result = new List<MdToken>();
            result.AddRange(TextCodeAnalyzer.GetTokens(text, offset));
            result.AddRange(StrongAnalyzer.GetTokens(text, offset));
            result.AddRange(HyperlinkAnalyzer.GetTokens(text, offset));
            result.AddRange(ImageAnalyzer.GetTokens(text, offset));
#if NETCOREAPP2_1
            if (!(text.EndsWith('*') || text.EndsWith('`') || text.EndsWith(')')))
#else
            if (!(text.EndsWith("*") || text.EndsWith("`") || text.EndsWith(")")))
#endif
            {
                result.Add(new MdToken() { Index = text.Length - 1, Type = MdTokenType.Text });
            }
            result.Sort((t1, t2) => t1.Index.CompareTo(t2.Index));
            return result;
        }
        public static IEnumerable<HtmlObject> GetHtmlObjects(string line, IEnumerable<MdToken> tokens)
        {
            int startIndex = 0;
            foreach (MdToken token in tokens)
            {
                MdTextAnalyzer analyzer;
                switch (token.Type)
                {
                    case MdTokenType.Text:
                    case MdTokenType.Code:
                        analyzer = TextCodeAnalyzer;
                        break;
                    case MdTokenType.Strong:
                    case MdTokenType.Italic:
                        analyzer = StrongAnalyzer;
                        break;
                    case MdTokenType.Image:
                        analyzer = ImageAnalyzer;
                        break;
                    case MdTokenType.Hyperlink:
                        analyzer = HyperlinkAnalyzer;
                        break;
                    default:
                        analyzer = null;
                        break;
                }
                if (analyzer != null)
                {
                    yield return analyzer.AnalyzeToken(line.Substring(startIndex, token.Index - startIndex + 1), token.Type);
                }
                startIndex = token.Index + 1;
            }
        }
    }
}
