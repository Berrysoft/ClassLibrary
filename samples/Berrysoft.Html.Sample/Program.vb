Imports System.IO
Imports Berrysoft.Html.Markdown

Module Program
    Sub Main(args As String())
        Dim document = MdDocument.Load("test.md")
        Dim htm = document.ToHtmlDocument()
        htm.Save("test.html")
    End Sub
End Module
