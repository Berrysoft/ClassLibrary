using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Berrysoft.Html
{
    public class HtmlDocument : HtmlObject
    {
        public HtmlDocument()
            : this(Encoding.UTF8)
        { }

        public HtmlDocument(Encoding encoding)
            : base(encoding)
        {
            head = new HtmlNode("head");
            body = new HtmlNode("body");
        }

        private HtmlDeclaration decl;
        public HtmlDeclaration Declaration
        {
            get => decl;
            set => decl = value;
        }

        private HtmlNode head;
        public HtmlNode Head => head;

        private HtmlNode body;
        public HtmlNode Body => body;

        public void Save(string path)
        {
            using (StreamWriter writer = new StreamWriter(path))
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
                    writer.Write("<{0}>", node.Name);
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
                    await writer.WriteAsync($"<{node.Name}>");
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

        public override string ToString()
        {
            return $"{(decl ?? HtmlDeclaration.Default).ToString()}<html>{head.ToString()}{body.ToString()}</html>";
        }
    }
}
