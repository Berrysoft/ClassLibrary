﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Berrysoft.Html.Markdown
{
    abstract class MdAnalyzer
    {
        public abstract MdAnalyzer GetToken(string line, out MdLineToken token);

        public abstract HtmlNode AnalyzeToken(MdLineToken token, HtmlNode current);
    }

    abstract class MdTextAnalyzer
    {
        public abstract IEnumerable<MdToken> GetTokens(string line);

#if NETCOREAPP2_1
        public abstract HtmlObject AnalyzeToken(ReadOnlyMemory<char> line, MdTokenType token);
#else
        public abstract HtmlObject AnalyzeToken(string line, MdTokenType token);
#endif
    }

    static class MdAnalyzerHelper
    {
        public static readonly Regex HeadRegex = new Regex(@"^[ ]*(#+[ ]+)([^#]+)#*$");
        public static readonly Regex ListItemRegex = new Regex(@"^[ ]*(\*[ ]+)(.*)$");
        public static readonly Regex CodeBlockRegex = new Regex(@"^[ ]*(\`\`\`)(.*)$");
        public static readonly Regex InlineCodeRegex = new Regex(@"([\`])([^`]+)([\`])");
        public static readonly Regex StrongRegex = new Regex(@"(\*\*)([^\*]+)(\*\*)");
        public static readonly Regex ItalicRegex = new Regex(@"[^\*](\*)([^\*\`]+)(\*)");
        public static readonly Regex HyperlinkRegex = new Regex(@"([^\!]|^)\[(.*)\]\((.*)\)");
        public static readonly Regex PictureRegex = new Regex(@"\!\[(.*)\]\((.*)\)");
        public static readonly Regex NoStartHyperlinkRegex = new Regex(@"\[(.*)\]\((.*)\)");

        public static readonly MdHeadAnalyzer HeadAnalyzer = new MdHeadAnalyzer();
        public static readonly MdParaAnalyzer ParaAnalyzer = new MdParaAnalyzer();
        public static readonly MdListAnalyzer ListAnalyzer = new MdListAnalyzer();
        public static readonly MdCodeAnalyzer CodeAnalyzer = new MdCodeAnalyzer();

        public static MdAnalyzer GetStartAnalyzer() => ParaAnalyzer;

        public static MdAnalyzer GetAnalyzerFromToken(MdLineTokenType token)
        {
            switch (token)
            {
                case MdLineTokenType.Head:
                    return HeadAnalyzer;
                case MdLineTokenType.Para:
                case MdLineTokenType.ParaEnd:
                    return ParaAnalyzer;
                case MdLineTokenType.List:
                case MdLineTokenType.ListEnd:
                    return ListAnalyzer;
                case MdLineTokenType.Code:
                case MdLineTokenType.CodeEnd:
                    return CodeAnalyzer;
                default:
                    return null;
            }
        }

        public static readonly MdTextCodeAnalyzer TextCodeAnalyzer = new MdTextCodeAnalyzer();
        public static readonly MdTextStrongAnalyzer StrongAnalyzer = new MdTextStrongAnalyzer();
        public static readonly MdTextHyperlinkAnalyzer HyperlinkAnalyzer = new MdTextHyperlinkAnalyzer();
        public static readonly MdTextImageAnalyzer ImageAnalyzer = new MdTextImageAnalyzer();

        public static IEnumerable<MdToken> GetTextTokens(string text)
        {
            List<MdToken> result = new List<MdToken>();
            result.AddRange(TextCodeAnalyzer.GetTokens(text));
            result.AddRange(StrongAnalyzer.GetTokens(text));
            result.AddRange(HyperlinkAnalyzer.GetTokens(text));
            result.AddRange(ImageAnalyzer.GetTokens(text));
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


#if NETCOREAPP2_1
        public static IEnumerable<HtmlObject> GetHtmlObjects(ReadOnlyMemory<char> line, Memory<MdToken> tokens)
        {
            int startIndex = 0;
            for (int i = 0; i < tokens.Length; i++)
            {
                MdToken token = tokens.Span[i];
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
                    yield return analyzer.AnalyzeToken(line.Slice(startIndex, token.Index - startIndex + 1), token.Type);
                }
                startIndex = token.Index + 1;
            }
        }
#else
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
#endif
    }
}
