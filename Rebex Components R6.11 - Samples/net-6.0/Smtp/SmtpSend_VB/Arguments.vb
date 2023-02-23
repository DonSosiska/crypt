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
Imports System.Collections
Imports System.IO
Imports Rebex.Net
Imports Rebex.Mail
Imports Rebex.Mime.Headers
Imports Rebex.Security.Certificates


''' <summary>
''' Summary description for Arguments.
''' </summary>
Public Class Arguments
    Private _server As String = Nothing
    Private _port As Integer = Smtp.DefaultPort
    Private _user As String = Nothing
    Private _password As String = Nothing
    Private _from As String = Nothing
    Private _to As String = Nothing
    Private _subject As String = Nothing
    Private _body As String = Nothing
    Private _filename As String = Nothing
    Private _security As SslMode = SslMode.None
    Private _sign As Boolean
    Private _encrypt As Boolean
    Private ReadOnly _certificates As ArrayList = New ArrayList

    Private _mailMessage As MailMessage = Nothing

    ''' <summary>
    ''' Get mime message according to parameters sent.
    ''' </summary>
    Public ReadOnly Property Message() As MailMessage
        Get
            If _mailMessage Is Nothing Then
                ' Message.get was called first time: create
                ' the message.
                ' 
                Dim msg As New MailMessage

                ' If filename is set load the message from the file
                If Not (_filename Is Nothing) Then
                    msg.Load(_filename)
                Else
                    ' filename is not set - set subject and body
                    ' from properties 
                    If Not (_subject Is Nothing) Then msg.Subject = _subject
                    If Not (_body Is Nothing) Then msg.BodyText = _body
                End If

                ' set From:
                If Not (_from Is Nothing) Then msg.From = New MailAddressCollection(_from)

                ' make sure that sender is specified
                If msg.From.Count = 0 Then Throw New ApplicationException("Missing sender (From:) value.")

                ' set To:
                '
                If Not (_to Is Nothing) Then msg.To = New MailAddressCollection(_to)

                ' make sure, that at least one recipient is specified
                If msg.To.Count = 0 Then Throw New ApplicationException("Missing recipients (To:) value.")
                _mailMessage = msg
            End If

            Return _mailMessage
        End Get
    End Property

    ''' <summary>
    ''' Loads the certificates specified on the command line into a memory-based certificate store.
    ''' </summary>
    Public ReadOnly Property LoadCertificates() As CertificateStore
        Get
            ' create an empty memory-based certificate store
            Dim store As CertificateStore = New CertificateStore(New Object() {})

            ' and fill it with certificates specified on the command line
            Dim i As Integer
            For i = 0 To _certificates.Count
                Dim cert As Certificate
                Dim certPath As String = _certificates(i)

                If Not File.Exists(certPath) Then Throw New ApplicationException(String.Format("Certificate file '{0}' not found.", certPath))

                Dim ext As String = Path.GetExtension(certPath).ToLower()
                Select Case ext
                    Case ".cer", ".der"
                        cert = Certificate.LoadDer(certPath)
                        store.Add(cert)
                    Case ".pfx", ".p12"
                        Console.WriteLine("Please enter password for '{0}':", certPath)
                        Dim password As String = Console.ReadLine()
                        cert = Certificate.LoadPfx(certPath, password)
                        store.Add(cert)
                    Case Else
                        Throw New ApplicationException(String.Format("Unsupported certificate type '{0}'.", ext))
                End Select
            Next

            Return store
        End Get
    End Property

    ''' <summary>
    ''' Creates new instance of Arguments class.
    ''' </summary>
    ''' <param name="args">Command line application arguments.</param>
    Public Sub New(ByVal args() As String)
        ' parse command line arguments and copy values to properties.

        If (args.Length < 1) Then Throw New ApplicationException("Expected server argument.")
        ParseServerHostnameAndPort(args(0))

        Dim i As Integer
        For i = 1 To args.Length - 1
            Dim arg As String = args(i)

            ' all parameters starts with "-" or "/" 
            If Not arg.StartsWith("-") AndAlso Not arg.StartsWith("/") Then
                Throw New ApplicationException(String.Format("Unexpected argument '{0}'.", arg))
            End If

            If i >= args.Length - 1 Then Throw New ApplicationException(String.Format("Missing value for argument '{0}'.", arg))
            i += 1
            Select Case arg.Substring(1).ToLower()
                Case "username"
                    _user = args(i)

                Case "password"
                    _password = args(i)

                Case "from"
                    _from = args(i)

                Case "to"
                    _to = args(i)

                Case "subject"
                    _subject = args(i)

                Case "body"
                    _body = args(i)

                Case "file"
                    _filename = args(i)
                Case "security"
                    _security = [Enum].Parse(GetType(SslMode), args(i), True)

                Case "sign"
                    _sign = (String.Compare(args(i), "yes", True) = 0)

                Case "encrypt"
                    _encrypt = (String.Compare(args(i), "yes", True) = 0)

                Case "certificate"
                    _certificates.Add(args(i))

                Case Else
                    Throw New ApplicationException(String.Format("Unknown argument '{0}'.", arg))
            End Select

        Next i

        ' check mandatory arguments
        If _server Is Nothing Then Throw New ApplicationException("Server not specified.")

    End Sub 'New 

    ''' <summary>
    ''' SMTP server name or ip address.
    ''' </summary>
    Public ReadOnly Property Server() As String
        Get
            Return _server
        End Get
    End Property

    ''' <summary>
    ''' SMTP server port.
    ''' </summary>
    Public ReadOnly Property Port() As Integer
        Get
            Return _port
        End Get
    End Property

    ''' <summary>
    ''' Username for logging to the server.
    ''' </summary>
    Public ReadOnly Property User() As String
        Get
            Return _user
        End Get
    End Property

    ''' <summary>
    ''' Password for logging to the server.
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
    ''' Specifies whether to encrypt the message.
    ''' </summary>
    Public ReadOnly Property Encrypt() As Boolean
        Get
            Return _encrypt
        End Get
    End Property

    ''' <summary>
    ''' Specifies whether to sign the message.
    ''' </summary>
    Public ReadOnly Property Sign() As Boolean
        Get
            Return _sign
        End Get
    End Property

    ''' <summary>
    ''' Reads server hostname and server port from argument in the form of "servername:port".
    ''' </summary>
    ''' <param name="serverAddress">Host[:port].</param>
    Private Sub ParseServerHostnameAndPort(ByVal serverAddress As String)
        Dim p As Integer = serverAddress.IndexOf(":")

        If p >= 0 Then
            _server = serverAddress.Substring(0, p)
            _port = Integer.Parse(serverAddress.Substring((p + 1)))

            If _port <= 0 OrElse _port > 65535 Then Throw New ApplicationException(String.Format("Invalid port {0}.", _port))

        Else
            _server = serverAddress
        End If
    End Sub 'ParseServerHostnameAndPort
End Class 'Arguments
