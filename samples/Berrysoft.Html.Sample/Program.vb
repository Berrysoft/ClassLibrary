Imports System.IO
Imports Berrysoft.Html.Markdown

Module Program
    Sub Main(args As String())
        Dim document = MdDocument.Load("test.md")
        Dim htm = document.ToHtmlDocument()
        htm.SaveAsync("test.html").Wait()
    End Sub
End Module
