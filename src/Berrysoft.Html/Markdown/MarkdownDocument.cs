using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Berrysoft.Html.Markdown
{
    public class MarkdownDocument
    {
        public static HtmlDocument LoadAsHtml(string path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                HtmlDocument document = new HtmlDocument();
                document.Head.AddElement(new HtmlNode("title", path));
                HtmlNode body = document.Body;
                bool in_code = false;
                bool in_ul = false;
                bool in_p = false;
                StringBuilder builder = new StringBuilder();
                List<HtmlObject> lis = new List<HtmlObject>();

                void deal_with_in_p()
                {
                    if (in_p)
                    {
                        in_p = false;
                        body.AddElement(new HtmlNode("p", builder.ToString()));
                    }
                }
                void deal_with_in_ul()
                {
                    if (in_ul)
                    {
                        in_ul = false;
                        body.AddElement(new HtmlNode("ul", lis.ToArray()));
                    }
                }

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (!in_code)
                    {
                        line = line.TrimStart();
                        if (line.Length == 0)
                        {
                            deal_with_in_p();
                            deal_with_in_ul();
                        }
                        else if (line.Length > 4 && line.StartsWith("### "))
                        {
                            deal_with_in_p();
                            deal_with_in_ul();
                            body.AddElement(new HtmlNode("h3", line.Substring(4)));
                        }
                        else if (line.Length > 3 && line.StartsWith("## "))
                        {
                            deal_with_in_p();
                            deal_with_in_ul();
                            body.AddElement(new HtmlNode("h2", line.Substring(3)));
                        }
                        else if (line.Length > 2 && line.StartsWith("# "))
                        {
                            deal_with_in_p();
                            deal_with_in_ul();
                            body.AddElement(new HtmlNode("h1", line.Substring(2)));
                        }
                        else if (line.Length > 2 && line.StartsWith("```"))
                        {
                            deal_with_in_p();
                            deal_with_in_ul();
                            if (in_code)
                            {
                                in_code = false;
                                builder.Clear();
                            }
                            else
                            {
                                in_code = true;
                                body.AddElement(new HtmlNode("pre", new HtmlNode("code", builder.ToString())));
                            }
                        }
                        else if (line.Length > 1 && line.StartsWith("* "))
                        {
                            deal_with_in_p();
                            if(!in_ul)
                            {
                                in_ul = true;
                                lis.Clear();
                            }
                            lis.Add(line.Substring(2));
                        }
                        else
                        {
                            if(in_code)
                            {
                                builder.AppendLine(line);
                            }
                            else
                            {
                                if(!in_p)
                                {
                                    in_p = true;
                                    builder.Clear();
                                }
                                builder.Append(line);
                                builder.Append(' ');
                            }
                        }
                    }
                }
                return document;
            }
        }
    }
}
