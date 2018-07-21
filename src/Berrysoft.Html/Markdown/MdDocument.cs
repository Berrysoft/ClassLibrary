using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Berrysoft.Html.Markdown
{
    public class MdDocument
    {
        private List<MdLineToken> tokens;

        private MdDocument()
        {
            tokens = new List<MdLineToken>();
        }

        public static MdDocument Load(string path)
        {
            MdDocument document = new MdDocument();
            MdAnalyzer analyzer = MdAnalyzerHelper.GetStartAnalyzer();
            using (StreamReader reader = new StreamReader(path))
            {
                int index = 0;
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    analyzer = analyzer.GetToken(line, out MdLineToken token);
                    token.Line = index;
                    document.tokens.Add(token);
                    index++;
                }
            }
            return document;
        }

        public static async Task<MdDocument> LoadAsync(string path)
        {
            MdDocument document = new MdDocument();
            MdAnalyzer analyzer = MdAnalyzerHelper.GetStartAnalyzer();
            using (StreamReader reader = new StreamReader(path))
            {
                int index = 0;
                while (!reader.EndOfStream)
                {
                    string line = await reader.ReadLineAsync();
                    analyzer = analyzer.GetToken(line, out MdLineToken token);
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
                MdAnalyzer analyzer = MdAnalyzerHelper.GetAnalyzerFromToken(token.Type);
                current = analyzer.AnalyzeToken(token, current);
            }
            return document;
        }
    }
}
