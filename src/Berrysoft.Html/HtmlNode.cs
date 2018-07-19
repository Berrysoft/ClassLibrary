using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Berrysoft.Html
{
    public class HtmlNode : HtmlObject
    {
        public HtmlNode(string name, params HtmlObject[] elements)
            : this(name, Encoding.UTF8, elements)
        { }

        public HtmlNode(string name, IEnumerable<HtmlObject> elements)
            : this(name, Encoding.UTF8, elements)
        { }

        public HtmlNode(string name, Encoding encoding, params HtmlObject[] elements)
            : this(name, encoding, (IEnumerable<HtmlObject>)elements)
        { }

        public HtmlNode(string name, Encoding encoding, IEnumerable<HtmlObject> elements)
            : base(encoding)
        {
            this.name = name;
            this.attrs = new Collection<HtmlAttribute>();
            this.objs = new Collection<HtmlObject>();
            foreach (HtmlObject e in elements)
            {
                objs.Add(e);
            }
        }

        private string name;
        public string Name => name;

        private Collection<HtmlAttribute> attrs;
        public virtual IEnumerable<HtmlAttribute> Attributes() => attrs;

        public virtual HtmlAttribute Attribute(string name) => attrs.FirstOrDefault(attr => attr.Name == name);

        public virtual void AddAttribute(HtmlAttribute attr) => attrs.Add(attr);

        public virtual void RemoveAttribute(HtmlAttribute attr) => attrs.Remove(attr);

        private Collection<HtmlObject> objs;
        public virtual IEnumerable<HtmlObject> Elements() => objs;

        public virtual HtmlObject Element(string name) => objs.FirstOrDefault(obj => ((HtmlNode)obj).name == name);

        public virtual void AddElement(HtmlObject element) => objs.Add(element);

        public virtual void RemoveElement(HtmlObject element) => objs.Remove(element);

        public override string ToString()
        {
#if NETCOREAPP2_1
            return $"<{Name} {string.Join(' ', Attributes())}>{string.Concat(Elements())}</{Name}>";
#else
            return $"<{Name} {string.Join(" ", Attributes().Select(attr => attr.ToString()))}>{string.Concat(Elements())}</{Name}>";
#endif
        }
    }
}
