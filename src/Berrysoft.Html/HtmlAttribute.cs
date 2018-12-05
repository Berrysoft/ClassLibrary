using System;
using System.Collections.Generic;
using System.Text;

namespace Berrysoft.Html
{
    /// <summary>
    /// Represents a HTML attribute.
    /// </summary>
    public class HtmlAttribute
    {
        private string name;
        /// <summary>
        /// The name of the attribute.
        /// </summary>
        public string Name => name;

        private string attrvalue;
        /// <summary>
        /// The value of the attribute.
        /// </summary>
        public string Value => attrvalue;

        /// <summary>
        /// Initializes a new instance of <see cref="HtmlAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="attrvalue">The value of the attribute.</param>
        public HtmlAttribute(string name, string attrvalue)
        {
            this.name = name;
            this.attrvalue = attrvalue;
        }

        /// <summary>
        /// Gets the value string of the attribute.
        /// </summary>
        /// <returns>A string.</returns>
        public override string ToString()
        {
#if NETCOREAPP2_2
            if (attrvalue.Contains('\"'))
#else
            if (attrvalue.Contains("\""))
#endif
            {
                return $"{name}=\'{attrvalue}\'";
            }
            else
            {
                return $"{name}=\"{attrvalue}\"";
            }
        }
    }
}
