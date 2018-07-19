using System;
using System.Collections.Generic;
using System.Text;

namespace Berrysoft.Html
{
    public class HtmlAttribute
    {
        private string name;
        public string Name => name;

        private HtmlPrimitive attrvalue;
        public HtmlPrimitive Value => attrvalue;

        public HtmlAttribute(string name, HtmlPrimitive attrvalue)
        {
            this.name = name;
            this.attrvalue = attrvalue;
        }
    }
}
