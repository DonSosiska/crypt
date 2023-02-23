'  
'   Rebex Sample Code License
' 
'   Copyright 2023, REBEX CR s.r.o.
'   All rights reserved.
'   https://www.rebex.net/
' 
'   Permission to use, copy, modify, and/or distribute this software for any
'   purpose with or without fee is hereby granted.
' 
'   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
'   EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
'   OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
'   NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
'   HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
'   WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
'   FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
'   OTHER DEALINGS IN THE SOFTWARE.
' 

Imports System
Imports System.Net
Imports Rebex.Net
Imports Rebex.Mail

Public Class Arguments
    Private _Server As String = Nothing
    Private _Port As Integer = Pop3.DefaultPort
    Private _User As String = Nothing
    Private _Password As String = Nothing
    Private _security As SslMode = SslMode.None
    Private _format As MailFormat = MailFormat.Mime

    ''' <summary>
    ''' Creates instance of Arguments class. Reads command line arguments and stores it into the properties.
    ''' </summary>
    ''' <param name="args">Commandline arguments</param>
    Public Sub New(ByVal args() As String)
        ' parse command line arguments and copy values ti properties.
        Dim i As Integer

        If (args.Length < 1) Then Throw New ApplicationException("Expected security argument.")
        _security = CType([Enum].Parse(GetType(SslMode), args(0), True), SslMode)

        If (args.Length < 2) Then Throw New ApplicationException("Expected server argument.")
        ParseServerHostnameAndPort(args(1))

        ' parse command line arguments and copy values to properties.
        For i = 2 To args.Length - 1
            Dim arg As String = args(i)

            If Not arg.StartsWith("-") AndAlso Not arg.StartsWith("/") Then Throw New ApplicationException(String.Format("Unexpected argument '{0}'.", arg))

            If i >= args.Length - 1 Then Throw New ApplicationException(String.Format("Missing value for argument '{0}'.", arg))

            i += 1
            Select Case arg.Substring(1).ToLower()
                Case "username"
                    _User = args(i)

                Case "password"
                    _Password = args(i)

                Case "format"
                    If String.Equals(args(i), "msg", StringComparison.OrdinalIgnoreCase) Then
                        _format = MailFormat.OutlookMsg
                    ElseIf String.Equals(args(i), "mime", StringComparison.OrdinalIgnoreCase) Then
                        _format = MailFormat.Mime
                    Else
                        Throw New ApplicationException(String.Format("Uknown mail format '{0}'.", args(i)))
                    End If
                Case Else
                    Throw New ApplicationException(String.Format("Unknown argument '{0}'.", arg))
            End Select
        Next i

        ' check mandatory arguments
        If User Is Nothing Then Throw New ApplicationException("Username not specified.")
        If Password Is Nothing Then Throw New ApplicationException("Password not specified.")

    End Sub 'New 
    ''' <summary>
    ''' POP3 server hostname or an IP address.
    ''' </summary>

    Public ReadOnly Property Server() As String
        Get
            Return _Server
        End Get
    End Property

    ''' <summary>
    ''' POP3 server port. Default is 110.
    ''' </summary>

    Public ReadOnly Property Port() As Integer
        Get
            Return _Port
        End Get
    End Property

    ''' <summary>
    ''' POP3 server username.
    ''' </summary>

    Public ReadOnly Property User() As String
        Get
            Return _User
        End Get
    End Property

    ''' <summary>
    ''' POP3 server password.
    ''' </summary>

    Public ReadOnly Property Password() As String
        Get
            Return _Password
        End Get
    End Property

    ''' <summary>
    ''' Security for connecting to the server.
    ''' </summary>
    Public ReadOnly Property Security() As SslMode
        Get
            Return _security
        End Get
    End Property

    ''' <summary>
    ''' MailFormat for saving the emails.
    ''' </summary>
    Public ReadOnly Property Format() As MailFormat
        Get
            Return _format
        End Get
    End Property

    ''' <summary>
    ''' Reads server hostname and server port from argument in the form of "servername:port".
    ''' </summary>
    ''' <param name="serverAddress">Host[:port].</param>
    Private Sub ParseServerHostnameAndPort(ByVal serverAddress As String)
        Dim p As Integer = serverAddress.IndexOf(":")

        If p >= 0 Then
            _Server = serverAddress.Substring(0, p)
            _Port = Integer.Parse(serverAddress.Substring((p + 1)))

            If _Port <= 0 OrElse _Port > 65535 Then
                Throw New ApplicationException(String.Format("Invalid port {0}.", _Port))
            End If
        Else
            _Server = serverAddress
        End If
    End Sub 'ParseServerHostnameAndPort
End Class 'Arguments
