Imports Berrysoft.Console
Imports Berrysoft.Tsinghua.Net

Module Program
    Sub Main(args As String())
        Console.WriteLine("Tsinghua Net Cross Platform Client")
        Dim command As New TsinghuaNetCommand()
        If args.Length = 0 Then
            command.PrintUsage()
            Return
        End If
        Try
            command.Parse(args)
        Catch ex As ArgRepeatedException
            Console.Error.WriteLine("Argument repeated: {0}", ex.ArgName)
            Return
        Catch ex As ArgInvalidException
            Console.Error.WriteLine("Argument not valid: {0}", ex.ArgName)
            Return
        Catch ex As InvalidCastException
            Console.Error.WriteLine("Argument type invalid: {0}", ex.Message)
            Return
        Catch ex As Exception
            Console.Error.WriteLine("Exception occured: {0}", ex.Message)
            Return
        End Try
        Dim helper As IConnect = Nothing
        If command.Login Then
            If command.Logout Then
                Console.Error.WriteLine("Cannot login and logout at the same time!")
                Return
            End If
            Dim username As String = command.Username
            If username Is Nothing Then
                Console.Error.WriteLine("Please input username.")
                Return
            End If
            Dim password As String = If(command.Password, String.Empty)
            helper = CreateHelper(username, password, command.Host)
            If helper Is Nothing Then
                Console.Error.WriteLine("Invalid host.")
                Return
            End If
            Login(helper).Wait()
        ElseIf command.Logout Then
            helper = CreateHelper(Nothing, Nothing, command.Host)
            If helper Is Nothing Then
                Console.Error.WriteLine("Invalid host.")
                Return
            End If
            Logout(helper, command.Username).Wait()
        End If
        If command.Flux Then
            If helper Is Nothing Then
                helper = CreateHelper(Nothing, Nothing, command.Host)
                If helper Is Nothing Then
                    Console.Error.WriteLine("Invalid host.")
                    Return
                End If
            End If
            GetFlux(helper).Wait()
        End If
        helper?.Dispose()
    End Sub
    Function CreateHelper(username As String, password As String, host As String) As IConnect
        Dim helper As IConnect
        Select Case host
            Case Nothing, "net"
                helper = New NetHelper(username, password)
            Case "auth4"
                helper = New Auth4Helper(username, password)
            Case "auth6"
                helper = New Auth6Helper(username, password)
            Case Else
                helper = Nothing
        End Select
        Return helper
    End Function
    Async Function Login(helper As IConnect) As Task
        Try
            Console.WriteLine(Await helper.LoginAsync())
        Catch ex As Exception
            Console.Error.WriteLine("Exception occured: {0}", ex.Message)
        End Try
    End Function
    Async Function Logout(helper As IConnect, username As String) As Task
        Try
            If username Is Nothing Then
                Console.WriteLine(Await helper.LogoutAsync())
            Else
                Console.WriteLine(Await helper.LogoutAsync(username))
            End If
        Catch ex As Exception
            Console.Error.WriteLine("Exception occured: {0}", ex.Message)
        End Try
    End Function
    Async Function GetFlux(helper As IConnect) As Task
        Try
            Dim flux As FluxUser = Await helper.GetFluxAsync()
            Console.WriteLine("Username: {0}", flux.Username)
            Console.WriteLine("Flux: {0}", FluxToString(flux.Flux))
            Console.WriteLine("Online time: {0}", flux.OnlineTime)
            Console.WriteLine("Balance: гд{0}", flux.Balance.ToString("N2"))
        Catch ex As Exception
            Console.Error.WriteLine("Exception occured: {0}", ex.Message)
        End Try
    End Function
    Function FluxToString(f As Long) As String
        Select Case f
            Case < 1_000
                Return $"{f.ToString()}B"
            Case < 1_000_000
                Return $"{(f / 1_000).ToString("N2")}KB"
            Case < 1_000_000_000
                Return $"{(f / 1_000_000).ToString("N2")}MB"
            Case Else
                Return $"{(f / 1_000_000_000).ToString("N2")}GB"
        End Select
    End Function
End Module
