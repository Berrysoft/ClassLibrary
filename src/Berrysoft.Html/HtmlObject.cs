using System;
using System.Text;

namespace Berrysoft.Html
{
    /// <summary>
    /// Represents a HTML object.
    /// </summary>
    public abstract class HtmlObject
    {
        /// <summary>
        /// Initializes a new instance of <see cref="HtmlObject"/> class.
        /// </summary>
        public HtmlObject()
            : this(Encoding.UTF8)
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="HtmlObject"/> class with encoding.
        /// </summary>
        /// <param name="encoding">The specified encoding.</param>
        public HtmlObject(Encoding encoding)
        {
            this.encoding = encoding;
        }

        private Encoding encoding;
        /// <summary>
        /// The encoding of the HTML object.
        /// </summary>
        public Encoding Encoding => encoding;

        /// <summary>
        /// The parent node of the HTML object.
        /// </summary>
        public virtual HtmlNode Parent { get; internal set; }

        /// <summary>
        /// Convert string to HTML object implicitly.
        /// </summary>
        /// <param name="value">The string value.</param>
        public static implicit operator HtmlObject(string value) => new HtmlString(value);

        /// <summary>
        /// Convert HTML object to string explicitly.
        /// </summary>
        /// <param name="value">The HTML object.</param>
        public static explicit operator string(HtmlObject value) => value.ToString();
    }
}
