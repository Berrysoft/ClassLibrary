Imports Berrysoft.Html.Markdown

Module Program
    Sub Main(args As String())
        MdDocument.Load("test.md").ToHtmlDocument().Save("test.html")
    End Sub
End Module
