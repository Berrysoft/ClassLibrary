Imports System.IO
Imports Berrysoft.Html.Markdown

Module Program
    Sub Main(args As String())
        Dim document = MarkdownDocument.LoadAsync("test.md").Result
        Dim htm = document.ToHtmlDocument()
        htm.SaveAsync("test.html").Wait()
    End Sub
End Module
