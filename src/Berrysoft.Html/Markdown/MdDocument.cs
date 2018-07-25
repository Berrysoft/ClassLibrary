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
