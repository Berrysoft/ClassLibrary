using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Berrysoft.Html
{
    /// <summary>
    /// Represents a HTML document.
    /// </summary>
    public class HtmlDocument : HtmlObject
    {
        /// <summary>
        /// Initializes a new instance of <see cref="HtmlDocument"/> class.
        /// </summary>
        public HtmlDocument()
            : this(Encoding.UTF8)
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="HtmlDocument"/> class with encoding.
        /// </summary>
        /// <param name="encoding">The specified encoding.</param>
        public HtmlDocument(Encoding encoding)
            : base(encoding)
        {
            head = new HtmlNode("head");
            body = new HtmlNode("body");
        }

        private HtmlDeclaration decl;
        /// <summary>
        /// The declaration of the document.
        /// </summary>
        public HtmlDeclaration Declaration
        {
            get => decl;
            set => decl = value;
        }

        private HtmlNode head;
        /// <summary>
        /// The &lt;head&gt; label of the document.
        /// </summary>
        public HtmlNode Head => head;

        private HtmlNode body;
        /// <summary>
        /// The &lt;body&gt; label of the document.
        /// </summary>
        public HtmlNode Body => body;

        /// <summary>
        /// Save the document to a file.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        public void Save(string path)
        {
            using (StreamWriter writer = new StreamWriter(path, false, Encoding))
            {
                writer.WriteLine((decl ?? HtmlDeclaration.Default).ToString());
                writer.WriteLine("<html>");
                WriteToStream(writer, head);
                WriteToStream(writer, body);
                writer.WriteLine("</html>");
            }
        }

        private void WriteToStream(StreamWriter writer, HtmlObject obj)
        {
            switch (obj)
            {
                case HtmlNode node:
#if NETCOREAPP || NETSTANDARD2_1
                    writer.Write("<{0} {1}>", node.Name, string.Join(' ', node.Attributes()));
#else
                    writer.Write("<{0} {1}>", node.Name, string.Join(" ", node.Attributes()));
#endif
                    foreach (HtmlObject e in node.Elements())
                    {
                        WriteToStream(writer, e);
                    }
                    writer.WriteLine("</{0}>", node.Name);
                    break;
                default:
                    writer.Write(obj.ToString());
                    break;
            }
        }

        /// <summary>
        /// Save the document to a file.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        public async Task SaveAsync(string path)
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                await writer.WriteLineAsync((decl ?? HtmlDeclaration.Default).ToString());
                await writer.WriteLineAsync("<html>");
                await WriteToStreamAsync(writer, head);
                await WriteToStreamAsync(writer, body);
                await writer.WriteLineAsync("</html>");
            }
        }

        private async Task WriteToStreamAsync(StreamWriter writer, HtmlObject obj)
        {
            switch (obj)
            {
                case HtmlNode node:
#if NETCOREAPP || NETSTANDARD2_1
                    await writer.WriteAsync($"<{node.Name} {string.Join(' ', node.Attributes())}>");
#else
                    await writer.WriteAsync($"<{node.Name} {string.Join(" ", node.Attributes())}>");
#endif
                    foreach (HtmlObject e in node.Elements())
                    {
                        await WriteToStreamAsync(writer, e);
                    }
                    await writer.WriteLineAsync($"</{node.Name}>");
                    break;
                default:
                    await writer.WriteAsync(obj.ToString());
                    break;
            }
        }

        /// <summary>
        /// Get the string form of the document.
        /// </summary>
        /// <returns>A string contains the document.</returns>
        public override string ToString()
        {
            return $"{(decl ?? HtmlDeclaration.Default).ToString()}<html>{head.ToString()}{body.ToString()}</html>";
        }
    }
}
