using System;
using System.Collections.Generic;
using System.Text;

namespace Berrysoft.Html
{
    /// <summary>
    /// Represents the HTML declaration.
    /// </summary>
    public class HtmlDeclaration
    {
        /// <summary>
        /// Gets a default instance of <see cref="HtmlDeclaration"/> class.
        /// </summary>
        public static readonly HtmlDeclaration Default = new HtmlDeclaration();

        /// <summary>
        /// Gets the value of the declaration.
        /// </summary>
        /// <returns>The value of the declaration.</returns>
        public override string ToString()
        {
            return "<!DOCTYPE html>";
        }
    }
}
