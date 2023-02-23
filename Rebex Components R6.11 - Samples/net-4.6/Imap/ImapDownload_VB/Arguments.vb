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
    Private _server As String = Nothing
    Private _port As Integer = Imap.DefaultPort
    Private _user As String = Nothing
    Private _password As String = Nothing
    Private _folder As String = "Inbox"
    Private _format As MailFormat = MailFormat.Mime
    Private _security As SslMode = SslMode.None

    ''' <summary>
    ''' Creates instance of Arguments class. Reads command line arguments and stores it into the properties.
    ''' </summary>
    ''' <param name="args">Commandline arguments</param>
    Public Sub New(ByVal args() As String)
        ' parse command line arguments and copy values to properties.
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
                    _user = args(i)

                Case "password"
                    _password = args(i)

                Case "folder"
                    _folder = args(i)

                Case "format"
                    Select Case args(i).ToLowerInvariant()
                        Case "mime"
                            _format = MailFormat.Mime

                        Case "msg"
                            _format = MailFormat.OutlookMsg

                        Case Else
                            Throw New ApplicationException(String.Format("Invalid format '{0}'.", args(i)))
                    End Select
                Case Else
                    Throw New ApplicationException(String.Format("Unknown argument '{0}'.", arg))
            End Select
        Next i

        ' check mandatory arguments
        If User Is Nothing Then Throw New ApplicationException("Username not specified.")
        If Password Is Nothing Then Throw New ApplicationException("Password not specified.")
    End Sub 'New 

    ''' <summary>
    ''' Imap server hostname or an IP address.
    ''' </summary>
    Public ReadOnly Property Server() As String
        Get
            Return _server
        End Get
    End Property

    ''' <summary>
    ''' IMAP server port. Default is dependent on security mode.
    ''' </summary>
    Public ReadOnly Property Port() As Integer
        Get
            Return _port
        End Get
    End Property

    ''' <summary>
    ''' IMAP server username.
    ''' </summary>
    Public ReadOnly Property User() As String
        Get
            Return _user
        End Get
    End Property

    ''' <summary>
    ''' IMAP server password.
    ''' </summary>
    Public ReadOnly Property Password() As String
        Get
            Return _password
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
    ''' Folder to work with - e.g. "Inbox".
    ''' </summary>
    Public ReadOnly Property Folder() As String
        Get
            Return _folder
        End Get
    End Property

    ''' <summary>
    ''' Format for saving the fetched message, i.e. mime or msg.
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
        Dim p As Integer = serverAddress.IndexOf(":"c)

        If p >= 0 Then
            _server = serverAddress.Substring(0, p)
            _port = Integer.Parse(serverAddress.Substring((p + 1)))

            If _port <= 0 OrElse _port > 65535 Then
                Throw New ApplicationException(String.Format("Invalid port {0}.", _port))
            End If
        Else
            _server = serverAddress
            If _security = SslMode.Implicit Then
                _port = Imap.DefaultImplicitSslPort
            End If
        End If
    End Sub 'ParseServerHostnameAndPort
End Class 'Arguments
