Module Program
    Private dictionary As New MultiMap(Of Char, String)
    Sub Main(args As String())
        dictionary.Add("��"c, "hai")
        dictionary.Add("ˮ"c, "shui")
        dictionary.Add("��"c, "chao")
        dictionary.Add("��"c, "zhao")
        dictionary.Add("��"c, "luo")

        dictionary.Add("��"c, "fu")
        dictionary.Add("��"c, "yun")
        dictionary.Add("��"c, "zhang")
        dictionary.Add("��"c, "chang")
        dictionary.Add("��"c, "xiao")

        dictionary.Add("��"c, "chao")
        dictionary.Add("��"c, "chang")

        Dim str = "��ˮ���������������両�Ƴ���������������".ToArray()
        Console.WriteLine(str)

        For i As Integer = 0 To str.Length - 1
            Dim pronounce = dictionary.GetValuesFromKey1(str(i))
            If pronounce.Count > 1 Then
                For j As Integer = 0 To pronounce.Count - 1
                    Console.Write("{0} {1}" + vbTab, j, pronounce(j))
                Next
                Console.WriteLine()
                Dim index As Integer = Console.ReadLine()
                Dim character = dictionary.GetValuesFromKey2(pronounce(index))
                If character.Count > 1 Then
                    str(i) = character.First(Function(c) c <> str(i))
                End If
            End If
        Next

        Console.WriteLine(str)
    End Sub
End Module
