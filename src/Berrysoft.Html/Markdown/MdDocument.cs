using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Berrysoft.Html.Markdown
{
    public class MdDocument
    {
        private List<MdElement> elements;

        private MdDocument()
        {
            elements = new List<MdElement>();
        }

        private static IEnumerable<string> ReadLines(StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                yield return reader.ReadLine();
            }
        }


        public static MdDocument Load(string path)
        {
            MdDocument document = new MdDocument();
            using (StreamReader reader = new StreamReader(path))
            {
                document.elements.AddRange(MdElementHelper.GetElements(ReadLines(reader).ToArray()));
            }
            return document;
        }

        //public static async Task<MdDocument> LoadAsync(string path)
        //{
        //    MdDocument document = new MdDocument();
        //    MdAnalyzer analyzer = MdAnalyzerHelper.GetStartAnalyzer();
        //    using (StreamReader reader = new StreamReader(path))
        //    {
        //        int index = 0;
        //        while (!reader.EndOfStream)
        //        {
        //            string line = await reader.ReadLineAsync();
        //            analyzer = analyzer.GetToken(line, out MdLineToken token);
        //            token.Line = index;
        //            document.tokens.Add(token);
        //            index++;
        //        }
        //    }
        //    return document;
        //}

        public HtmlDocument ToHtmlDocument()
        {
            HtmlDocument document = new HtmlDocument();
            HtmlNode current = document.Body;
            foreach (var e in elements)
            {
                current.AddElement(e.ToHtmlNode());
            }
            return document;
        }
    }
}
