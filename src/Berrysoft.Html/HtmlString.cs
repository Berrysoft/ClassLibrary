using System;
using System.Text;

namespace Berrysoft.Html
{
    /// <summary>
    /// Represents a HTML string.
    /// </summary>
    public class HtmlString : HtmlObject
    {
        /// <summary>
        /// Initializes a new instance of <see cref="HtmlString"/> class.
        /// </summary>
        /// <param name="value">The value of the string.</param>
        public HtmlString(string value)
            : this(value, Encoding.UTF8)
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="HtmlString"/> class with encoding.
        /// </summary>
        /// <param name="value">The value of the string.</param>
        /// <param name="encoding">The specified encoding.</param>
        public HtmlString(string value, Encoding encoding)
            : base(encoding)
        {
            htmlValue = value;
        }

        private string htmlValue;
        /// <summary>
        /// The value of the string.
        /// </summary>
        public string Value => htmlValue;

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <returns>The value of the string.</returns>
        public override string ToString() => htmlValue;
    }
}
