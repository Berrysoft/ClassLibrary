using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Berrysoft.Html.Markdown
{
    public class MarkdownDocument
    {
        private List<MarkdownLineToken> tokens;

        private MarkdownDocument()
        {
            tokens = new List<MarkdownLineToken>();
        }

        public static MarkdownDocument Load(string path)
        {
            MarkdownDocument document = new MarkdownDocument();
            MarkdownAnalyzer analyzer = MarkdownAnalyzer.GetStartAnalyzer();
            using (StreamReader reader = new StreamReader(path))
            {
                int index = 0;
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    analyzer = analyzer.GetToken(line, out MarkdownLineToken token);
                    token.Line = index;
                    document.tokens.Add(token);
                    index++;
                }
            }
            return document;
        }

        public static async Task<MarkdownDocument> LoadAsync(string path)
        {
            MarkdownDocument document = new MarkdownDocument();
            MarkdownAnalyzer analyzer = MarkdownAnalyzer.GetStartAnalyzer();
            using (StreamReader reader = new StreamReader(path))
            {
                int index = 0;
                while (!reader.EndOfStream)
                {
                    string line = await reader.ReadLineAsync();
                    analyzer = analyzer.GetToken(line, out MarkdownLineToken token);
                    token.Line = index;
                    document.tokens.Add(token);
                    index++;
                }
            }
            return document;
        }

        public HtmlDocument ToHtmlDocument()
        {
            HtmlDocument document = new HtmlDocument();
            HtmlNode current = document.Body;
            foreach(var token in tokens)
            {
                MarkdownAnalyzer analyzer = MarkdownAnalyzer.GetAnalyzerFromToken(token.Type);
                current = analyzer.AnalyzeToken(token, current);
            }
            return document;
        }
    }
}
