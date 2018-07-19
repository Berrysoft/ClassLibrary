using System;
using System.Collections.Generic;
using System.Text;

namespace Berrysoft.Html
{
    public class HtmlAttribute
    {
        private string name;
        public string Name => name;

        private string attrvalue;
        public string Value => attrvalue;

        public HtmlAttribute(string name, string attrvalue)
        {
            this.name = name;
            this.attrvalue = attrvalue;
        }

        public override string ToString()
        {
#if NETCOREAPP2_1
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
