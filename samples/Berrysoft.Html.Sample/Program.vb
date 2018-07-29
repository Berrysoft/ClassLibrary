Imports Berrysoft.Html.Markdown
Imports BenchmarkDotNet.Attributes
Imports BenchmarkDotNet.Running

Module Program
    Sub Main(args As String())
        BenchmarkRunner.Run(Of MdBenchmark)()
    End Sub
End Module

<ClrJob(True), CoreJob>
<RankColumn, MemoryDiagnoser>
Public Class MdBenchmark
    <Benchmark>
    Public Sub LoadMd()
        MdDocument.Load("test.md")
    End Sub

    <Benchmark>
    Public Sub GetHtml()
        MdDocument.Load("test.md").ToHtmlDocument()
    End Sub
End Class
