using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Berrysoft.Html
{
    /// <summary>
    /// Represents a HTML node.
    /// </summary>
    public class HtmlNode : HtmlObject
    {
        /// <summary>
        /// Initializes a new instance of <see cref="HtmlNode"/> class with its name and elements.
        /// </summary>
        /// <param name="name">The name of the node.</param>
        /// <param name="elements">The elements of the node.</param>
        public HtmlNode(string name, params HtmlObject[] elements)
            : this(name, Encoding.UTF8, elements)
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="HtmlNode"/> class with its name and elements.
        /// </summary>
        /// <param name="name">The name of the node.</param>
        /// <param name="elements">The elements of the node.</param>
        public HtmlNode(string name, IEnumerable<HtmlObject> elements)
            : this(name, Encoding.UTF8, elements)
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="HtmlNode"/> class with its name, encoding and elements.
        /// </summary>
        /// <param name="name">The name of the node.</param>
        /// <param name="encoding">The specified encoding.</param>
        /// <param name="elements">The elements of the node.</param>
        public HtmlNode(string name, Encoding encoding, params HtmlObject[] elements)
            : this(name, encoding, (IEnumerable<HtmlObject>)elements)
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="HtmlNode"/> class with its name, encoding and elements.
        /// </summary>
        /// <param name="name">The name of the node.</param>
        /// <param name="encoding">The specified encoding.</param>
        /// <param name="elements">The elements of the node.</param>
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
        /// <summary>
        /// The name of the node.
        /// </summary>
        public string Name => name;

        private Collection<HtmlAttribute> attrs;
        /// <summary>
        /// Gets the attributes of the node.
        /// </summary>
        /// <returns>An sequence of <see cref="HtmlAttribute"/> class.</returns>
        public IEnumerable<HtmlAttribute> Attributes() => attrs;

        /// <summary>
        /// Gets an attribute by its name.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <returns>The specified attribute.</returns>
        public HtmlAttribute Attribute(string name) => attrs.FirstOrDefault(attr => attr.Name == name);

        /// <summary>
        /// Adds an attribute to the node.
        /// </summary>
        /// <param name="attr">An instance of <see cref="HtmlAttribute"/> class.</param>
        public void AddAttribute(HtmlAttribute attr) => attrs.Add(attr);

        /// <summary>
        /// Removes an attribute of the node.
        /// </summary>
        /// <param name="attr">An instance of <see cref="HtmlAttribute"/> class.</param>
        /// <returns><see langword="true"/> if removes successfully; otherwise, <see langword="false"/>.</returns>
        public bool RemoveAttribute(HtmlAttribute attr) => attrs.Remove(attr);

        private Collection<HtmlObject> objs;
        /// <summary>
        /// Gets the elements of the node.
        /// </summary>
        /// <returns>An sequences of <see cref="HtmlObject"/> class.</returns>
        public IEnumerable<HtmlObject> Elements() => objs;

        /// <summary>
        /// Gets an element with its index.
        /// </summary>
        /// <param name="index">The index of the element.</param>
        /// <returns>An instance of <see cref="HtmlObject"/> class.</returns>
        public HtmlObject ElementAt(int index) => objs[index];

        /// <summary>
        /// Adds an element to the node.
        /// </summary>
        /// <param name="element">An instance of <see cref="HtmlObject"/> class.</param>
        public void AddElement(HtmlObject element)
        {
            element.Parent = this;
            objs.Add(element);
        }

        /// <summary>
        /// Removes an element of the node.
        /// </summary>
        /// <param name="element">An instance of <see cref="HtmlObject"/> class.</param>
        /// <returns><see langword="true"/> if removes successfully; otherwise, <see langword="false"/>.</returns>
        public bool RemoveElement(HtmlObject element)
        {
            if (objs.Remove(element))
            {
                element.Parent = null;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the value string of the node.
        /// </summary>
        /// <returns>A string.</returns>
        public override string ToString()
        {
            if (objs.Count > 0)
            {
#if NETCOREAPP
                return $"<{Name} {string.Join(' ', Attributes())}>{string.Concat(Elements())}</{Name}>";
#else
                return $"<{Name} {string.Join(" ", Attributes().Select(attr => attr.ToString()))}>{string.Concat(Elements())}</{Name}>";
#endif
            }
            else
            {
#if NETCOREAPP
                return $"<{Name} {string.Join(' ', Attributes())} />";
#else
                return $"<{Name} {string.Join(" ", Attributes().Select(attr => attr.ToString()))} />";
#endif
            }
        }
    }
}
