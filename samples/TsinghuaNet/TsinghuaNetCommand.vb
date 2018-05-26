﻿Imports Berrysoft.Console

Class TsinghuaNetCommand
    Inherits CommandLine
    Public Sub New(args() As String)
        MyBase.New(args)
    End Sub
    <[Option]("i"c, "login", HelpText:="Option to login")>
    Public Property Login As Boolean
    <[Option]("o"c, "logout", HelpText:="Option to logout")>
    Public Property Logout As Boolean
    <[Option]("f"c, "flux", HelpText:="Option to get flux of the user online")>
    Public Property Flux As Boolean
    <[Option]("u"c, "username", HelpText:="Username to login, required when -i")>
    Public Property Username As String
    <[Option]("p"c, "password", HelpText:="Password to login")>
    Public Property Password As String
    <[Option]("s"c, "host", HelpText:="Host to login, default: net, others: auth4, auth6")>
    Public Property Host As String
    Protected Overrides Sub PrintUsage(opt As OptionAttribute)
        Console.WriteLine("{0}{1}" & vbTab & "{2}{3}  " & vbTab & "{4}", ShortHead, opt.ShortArg, LongHead, opt.LongArg, opt.HelpText)
    End Sub
End Class