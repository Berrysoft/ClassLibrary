Imports Berrysoft.Data
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()>
Public Class DictionaryTest

    <TestMethod()>
    Public Sub KeyDicAddTest()
        Dim keydic As New KeyDictionary(Of Integer, String)()
        keydic.Add(123, "123")
        keydic.Add(456, "456")
        Assert.AreEqual(keydic.GetValueFromKey1(123), "123")
        Assert.AreEqual(keydic.GetValueFromKey2("456"), 456)
        keydic.SetPair(123, "abc")
        Assert.AreEqual(keydic.GetValueFromKey1(123), "abc")
        Assert.ThrowsException(Of ArgumentException)(Sub() keydic.Add(123, "aaa"))
    End Sub

    <TestMethod()>
    Public Sub KeyDicContainsAndRemoveTest()
        Dim keydic As New KeyDictionary(Of Integer, String)()
        keydic.Add(123, "123")
        keydic.Add(456, "456")
        Assert.IsTrue(keydic.ContainsKey1(123))
        Assert.IsFalse(keydic.ContainsKey2("abc"))
        keydic.RemoveKey1(123)
        Assert.IsFalse(keydic.ContainsKey2("123"))
        Assert.IsFalse(keydic.RemoveKey2("123"))
        keydic.Clear()
        Assert.IsFalse(keydic.Contains(456, "456"))
    End Sub

    <TestMethod()>
    Public Sub KeyDicCollectionTest()
        Dim keydic As New KeyDictionary(Of Integer, String)()
        keydic.Add(123, "123")
        keydic.Add(456, "456")
        Dim key1collection = keydic.Keys1
        Dim key1array() As Integer = key1collection.ToArray()
        Assert.AreEqual(key1array(0), 123)
        Assert.AreEqual(key1array(1), 456)
        Dim key2collection = keydic.Keys2
        Dim i As Integer = 0
        Dim key2array() As String = {"123", "456"}
        For Each key2 In key2collection
            Assert.AreEqual(key2, key2array(i))
            i += 1
        Next
    End Sub

    <TestMethod()>
    Public Sub KeyLookupAddTest()
        Dim keylook As New KeyLookup(Of Integer, String)()
        keylook.Add(123, "123")
        keylook.Add(456, "456")
        keylook.Add(123, "abc")
        Dim key1values() As String = keylook.GetValuesFromKey1(123).ToArray()
        Assert.AreEqual(key1values(0), "123")
        Assert.AreEqual(key1values(1), "abc")
        Assert.ThrowsException(Of ArgumentException)(Sub() keylook.Add(123, "123"))
    End Sub

    <TestMethod()>
    Public Sub KeyLookupRemoveTest()
        Dim keylook As New KeyLookup(Of Integer, String)()
        keylook.Add(123, "123")
        keylook.Add(456, "456")
        keylook.Add(123, "abc")
        keylook.RemoveKey1(123)
        Assert.AreEqual(keylook.Count, 1)
        keylook.Add(123456, "233")
        Assert.IsFalse(keylook.Remove(456, "123"))
        keylook.Remove(456, "456")
        Assert.AreEqual(keylook.Count, 1)
    End Sub

End Class