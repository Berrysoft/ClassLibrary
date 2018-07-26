# Title
A title starts with 1-6 #, and will be translated to &lt;h1&gt; to &lt;h6&gt;.

Paragraphs are splitted with to line breaks. They will be translated to &lt;p&gt;.

You can even write with true HTML:
<progress value="22" max="100"/>

With raw texts togther: &&&
## Lists
Here is a list:
* Item 1
* Item 2
* Item 3

A list starts with a list item with * at start, and ends with a blank line.
## Texts
This is an inline code: `Console.WriteLine();`
And here is a **strong** text,
here is an *italic* text.

This is a code block:
``` vb.net
Dim f = Function(c)
            Return c.ToString()
        End Function
```
This is another code block:

    var f = c =>
    {
        return c.ToString();
    }
You can write raw texts in a code block:
```
<!DOCTYPE html>
<html>
    <head/>
    <body>
        <h1>Title</h1>
        <p>Paragraph</p>
    </body>
</html>
```
## Images and links
This is a image:

![ABC](tulip.jpg)

This is a hyperlink: [HyperLink](http://github.com/)
## Tables
|Hhhhhhhhh1|Hhhhhhhhh2|Hhhhhhhhh3|Hhhhhhhhh4|
|-|:-:|-:|:-|
|r1|r2|r3|r4|
|-:|:-|-|-|
|123|`abc|def`|456|789
