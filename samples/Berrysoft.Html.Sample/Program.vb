Imports System.IO
Imports Berrysoft.Html.Markdown

Module Program
    Sub Main(args As String())
        Dim document = MarkdownDocument.LoadAsHtml("test.md")
        Using writer As New StreamWriter("test.html")
            writer.Write(document.ToString())
        End Using
    End Sub
End Module
