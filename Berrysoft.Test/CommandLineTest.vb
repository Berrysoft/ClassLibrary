Imports Berrysoft.Console
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

    <Settings("settings")>
    Class StdSettingsClass
        Inherits XmlSettings

        <Settings("aaa")>
        Public Property A As Integer

        <Settings("bbb", AllowMultiple:=True)>
        Public Property B As String()

        Protected Overrides Function ChangeType(name As XName, value As Object, conversionType As Type) As Object
            If name = "bbb" Then
                Dim array As String() = value
                Return array
            Else
                Return MyBase.ChangeType(name, value, conversionType)
            End If
        End Function

        Protected Overrides Function ChangeBackType(name As XName, value As Object, conversionType As Type) As Object
            If name = "bbb" Then
                Dim array As String() = value
                Return array
            Else
                Return MyBase.ChangeBackType(name, value, conversionType)
            End If
        End Function
    End Class

    <TestMethod()>
    Public Sub StdSettingsTest()
        Dim settings As New StdSettingsClass()
        settings.Open("settings.xml")
        Assert.AreEqual(settings.A, 123)
        Assert.AreEqual(settings.B(0), "a")
        Assert.AreEqual(settings.B(1), "b")
        settings.B(0) = "hhh"
        settings.Save("settings.xml")
        settings.B(0) = "asdf"
        settings.Open("settings.xml")
        Assert.AreEqual(settings.B(0), "hhh")
        settings.B(0) = "a"
        settings.Save("settings.xml")
    End Sub

    <TestMethod()>
    Public Sub StdSettingsTestAsync()
        StdSettingsTestAsyncInternal().Wait()
    End Sub
    Private Async Function StdSettingsTestAsyncInternal() As Task
        Dim settings As New StdSettingsClass()
        Await settings.OpenAsync("settings.xml")
        Assert.AreEqual(settings.A, 123)
        Assert.AreEqual(settings.B(0), "a")
        Assert.AreEqual(settings.B(1), "b")
        settings.B(0) = "hhh"
        Await settings.SaveAsync("settings.xml")
        settings.B(0) = "asdf"
        Await settings.OpenAsync("settings.xml")
        Assert.AreEqual(settings.B(0), "hhh")
        settings.B(0) = "a"
        Await settings.SaveAsync("settings.xml")
    End Function

End Class
