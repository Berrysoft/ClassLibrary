using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Berrysoft.Html
{
    public abstract class HtmlNode : HtmlObject
    {
        public HtmlNode(string name)
        {
            this.name = name;
            this.attrs = new Collection<HtmlAttribute>();
        }

        private string name;
        public string Name => name;

        private Collection<HtmlAttribute> attrs;
        public virtual ICollection<HtmlAttribute> Attributes() => attrs;

        public virtual HtmlAttribute Attribute(string name) => attrs.FirstOrDefault(attr => attr.Name == name);

        public abstract HtmlNode Node(string name);

        public abstract IEnumerable<HtmlNode> Nodes();

        public abstract IEnumerable<HtmlNode> Nodes(string name);

        public abstract void AddNode(HtmlNode node);

        public abstract void RemoveNode(HtmlNode node);

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("<{0}>", name);
            foreach (HtmlNode node in Nodes())
            {
                builder.Append(node.ToString());
            }
            builder.AppendFormat("</{0}>", name);
            return builder.ToString();
        }
    }
}
