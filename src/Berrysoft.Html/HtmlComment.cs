using System;
using System.Collections.Generic;
using System.Text;

namespace Berrysoft.Html
{
    /// <summary>
    /// Represents a block of HTML comment.
    /// </summary>
    public class HtmlComment : HtmlObject
    {
        /// <summary>
        /// Initializes a new instance of <see cref="HtmlComment"/> class.
        /// </summary>
        /// <param name="comment">The content of the comment.</param>
        public HtmlComment(string comment)
            : this(comment, Encoding.UTF8)
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="HtmlComment"/> class with encoding.
        /// </summary>
        /// <param name="comment">The content of the comment.</param>
        /// <param name="encoding">The specified encoding.</param>
        public HtmlComment(string comment, Encoding encoding)
            : base(encoding)
        {
            this.comment = comment;
        }

        private string comment;
        /// <summary>
        /// The content of the comment.
        /// </summary>
        public string Value => comment;

        /// <summary>
        /// Get the value string.
        /// </summary>
        /// <returns>A string.</returns>
        public override string ToString()
        {
            return $"<!--{comment}-->";
        }
    }
}
