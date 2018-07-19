using System;
using System.Collections.Generic;
using System.Text;

namespace Berrysoft.Html
{
    public class HtmlDeclaration
    {
        public static readonly HtmlDeclaration Default = new HtmlDeclaration();

        public override string ToString()
        {
            return "<!DOCTYPE html>";
        }
    }
}
