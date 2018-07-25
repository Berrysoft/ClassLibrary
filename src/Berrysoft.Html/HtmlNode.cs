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
                AddElement(e);
            }
        }

        private string name;
        public string Name => name;

        private Collection<HtmlAttribute> attrs;
        public IEnumerable<HtmlAttribute> Attributes() => attrs;

        public HtmlAttribute Attribute(string name) => attrs.FirstOrDefault(attr => attr.Name == name);

        public void AddAttribute(HtmlAttribute attr) => attrs.Add(attr);

        public bool RemoveAttribute(HtmlAttribute attr) => attrs.Remove(attr);

        private Collection<HtmlObject> objs;
        public IEnumerable<HtmlObject> Elements() => objs;

        public HtmlObject Element(string name) => objs.FirstOrDefault(obj => ((HtmlNode)obj).name == name);

        public HtmlObject ElementAt(int index) => objs[index];

        public void AddElement(HtmlObject element)
        {
            element.Parent = this;
            objs.Add(element);
        }

        public bool RemoveElement(HtmlObject element)
        {
            if (objs.Remove(element))
            {
                element.Parent = null;
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            if (objs.Count > 0)
            {
#if NETCOREAPP2_1
                return $"<{Name} {string.Join(' ', Attributes())}>{string.Concat(Elements())}</{Name}>";
#else
                return $"<{Name} {string.Join(" ", Attributes().Select(attr => attr.ToString()))}>{string.Concat(Elements())}</{Name}>";
#endif
            }
            else
            {
#if NETCOREAPP2_1
                return $"<{Name} {string.Join(' ', Attributes())} />";
#else
                return $"<{Name} {string.Join(" ", Attributes().Select(attr => attr.ToString()))} />";
#endif
            }
        }
    }
}
