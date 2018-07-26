using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Berrysoft.Html.Markdown
{
    /// <summary>
    /// Represents a Markdown document.
    /// </summary>
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

        /// <summary>
        /// Load a Markdown file with path.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <returns>An instance of <see cref="MdDocument"/> class.</returns>
        public static MdDocument Load(string path)
        {
            MdDocument document = new MdDocument();
            using (StreamReader reader = new StreamReader(path))
            {
                document.elements.AddRange(MdElementHelper.GetElements(ReadLines(reader).ToArray()));
            }
            return document;
        }

        /// <summary>
        /// Convert the Markdown document to an instance of <see cref="HtmlDocument"/> class.
        /// </summary>
        /// <returns>An instance of <see cref="HtmlDocument"/> class.</returns>
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
