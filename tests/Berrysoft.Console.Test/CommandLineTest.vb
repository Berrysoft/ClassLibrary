Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()>
Public Class CommandLineTest
    Class StdArgTestClass
        Inherits CommandLine
        Public Sub New(args() As String)
            MyBase.New(args)
        End Sub

        <[Option]("a"c, "aaa", Required:=True)>
        Public Property A As String

        <[Option]("b"c, "bbb")>
        Public Property B As Integer

        <[Option]("c"c, "ccc")>
        Public Property C As Date

        <[Option]("d"c, Nothing)>
        Public Property D As Boolean

        Protected Overrides Sub PrintUsage(opt As OptionAttribute)
            Debug.WriteLine(opt.HelpText)
        End Sub
    End Class

    <TestMethod()>
    Public Sub StdTest()
        Dim args() As String = {"-a", "aaa", "--bbb", "123"}
        Dim argClass As New StdArgTestClass(args)
        argClass.Parse()
        Assert.AreEqual(argClass.A, "aaa")
        Assert.AreEqual(argClass.B, 123)
    End Sub

    <TestMethod()>
    Public Sub RepeatTest()
        Dim args() As String = {"-a", "1", "--aaa", "2"}
        Dim argClass As New StdArgTestClass(args)
        Assert.ThrowsException(Of ArgRepeatedException)(Sub() argClass.Parse())
    End Sub

    <TestMethod()>
    Public Sub RequireTest()
        Dim args() As String = {"-b", "456"}
        Dim argClass As New StdArgTestClass(args)
        Assert.ThrowsException(Of ArgRequiredException)(Sub() argClass.Parse())
    End Sub

    <TestMethod()>
    Public Sub OptionTest()
        Dim argclass As New StdArgTestClass({})
        Dim options() As String = {"a", "b", "c", "d"}
        Dim i As Integer = 0
        For Each op In argclass.GetOptionAttributes()
            Assert.AreEqual(op.ShortArg, options(i))
            i += 1
        Next
    End Sub

    <TestMethod()>
    Public Sub BooleanTest()
        Dim argclass As New StdArgTestClass({"-a", "aaa", "-d"})
        argclass.Parse()
        Assert.IsTrue(argclass.D)
    End Sub

End Class
