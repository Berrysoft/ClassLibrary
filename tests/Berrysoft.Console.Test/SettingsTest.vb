Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()>
Public Class SettingsTest
    <Settings("settings")>
    Class StdSettingsClass
        Inherits XmlSettings

        <Settings("aaa")>
        Public Property A As Integer

        <Settings("bbb", AllowMultiple:=True)>
        Public Property B As String()

        Protected Overrides Function ChangeType(name As String, value As Object, conversionType As Type) As Object
            If name = "bbb" Then
                Dim array As String() = value
                Return array
            Else
                Return Convert.ChangeType(value, conversionType)
            End If
        End Function

        Protected Overrides Function ChangeBackType(name As String, value As Object, conversionType As Type) As Object
            If name = "bbb" Then
                Dim array As String() = value
                Return array
            Else
                Return value.ToString()
            End If
        End Function
    End Class
    Private Const FileName As String = "settings.xml"
    <TestMethod()>
    Public Sub StdSettingsTest()
        Dim settings As New StdSettingsClass()
        settings.Open(FileName)
        Assert.AreEqual(settings.A, 123)
        Assert.AreEqual(settings.B(0), "a")
        Assert.AreEqual(settings.B(1), "b")
        settings.B(0) = "hhh"
        settings.Save(FileName)
        settings.B(0) = "asdf"
        settings.Open(FileName)
        Assert.AreEqual(settings.B(0), "hhh")
        settings.B(0) = "a"
        settings.Save(FileName)
    End Sub

    <TestMethod()>
    Public Sub StdSettingsTestAsync()
        StdSettingsTestAsyncInternal().Wait()
    End Sub
    Private Async Function StdSettingsTestAsyncInternal() As Task
        Dim settings As New StdSettingsClass()
        Await settings.OpenAsync(FileName)
        Assert.AreEqual(settings.A, 123)
        Assert.AreEqual(settings.B(0), "a")
        Assert.AreEqual(settings.B(1), "b")
        settings.B(0) = "hhh"
        Await settings.SaveAsync(FileName)
        settings.B(0) = "asdf"
        Await settings.OpenAsync(FileName)
        Assert.AreEqual(settings.B(0), "hhh")
        settings.B(0) = "a"
        Await settings.SaveAsync(FileName)
    End Function

End Class
