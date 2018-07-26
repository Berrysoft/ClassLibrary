using System;
using System.Text;

namespace Berrysoft.Html
{
    public enum HtmlEscapeOption
    {
        None,
        Auto,
        All
    }
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
            : this(value, Encoding.UTF8, HtmlEscapeOption.None)
        { }

        public HtmlString(string value, HtmlEscapeOption option)
            : this(value, Encoding.UTF8, option)
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="HtmlString"/> class with encoding.
        /// </summary>
        /// <param name="value">The value of the string.</param>
        /// <param name="encoding">The specified encoding.</param>
        public HtmlString(string value, Encoding encoding, HtmlEscapeOption option)
            : base(encoding)
        {
            switch (option)
            {
                case HtmlEscapeOption.None:
                    htmlValue = value;
                    break;
                case HtmlEscapeOption.Auto:
                    htmlValue = HtmlEscapeHelper.EscapeAuto(value);
                    break;
                case HtmlEscapeOption.All:
                    htmlValue = HtmlEscapeHelper.EscapeAll(value);
                    break;
            }
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
