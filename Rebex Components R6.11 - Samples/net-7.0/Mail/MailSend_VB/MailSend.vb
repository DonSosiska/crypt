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
Imports Rebex.Net
Imports Rebex.Mime.Headers
Imports Rebex.Mail


''' <summary>
''' Send method.
''' </summary>
Enum SendMethod
    ''' <summary>
    ''' Send mail directly to recipient's server.
    ''' </summary>
    Direct
    ''' <summary>
    ''' Send mail over IIS.
    ''' </summary>
    IIS
    ''' <summary>
    ''' Send mail over SMTP class.
    ''' </summary>
    Smtp
End Enum 'SendMethod


Class MailSend
    Private Shared _method As SendMethod = SendMethod.Direct
    Private Shared _server As String = Nothing
    Private Shared _port As Integer = Smtp.DefaultPort
    Private Shared _portSet As Boolean = False
    Private Shared _from As String = Nothing
    Private Shared _to As String = Nothing
    Private Shared _subject As String = Nothing
    Private Shared _body As String = Nothing
    Private Shared _attachments As New ArrayList
    Shared _security As SslMode = SslMode.None

    ''' <summary>
    ''' Program entrypoint.
    ''' </summary>
    ''' <param name="args">Program arguments.</param>
    ''' <returns>Status: 0=OK; 1=Not sent; 2=Bad arguments</returns>
    <STAThread()> Overloads Shared Function Main(ByVal args() As String) As Integer
            ' set the trial license key
            Rebex.Licensing.Key = Rebex.Samples.TrialKey.Key


        If args.Length > 0 Then
            If args(0) <> "-interactive" Then
                RunCommandLine(args)
            Else
                RunInteractiveLoop()
            End If
        Else
            ShowHelp()
        End If

        Return 0
    End Function    'Main



    ''' <summary>
    ''' Parse startup command line.
    ''' </summary>
    ''' <param name="args">Arguments.</param>
    Private Shared Sub RunCommandLine(ByVal args() As String)
        Try
            '
            ' Process arguments
            '
            Dim i As Integer
            For i = 0 To args.Length - 1

                Dim arg As String = args(i)

                If i >= args.Length - 1 Then
                    Throw New ApplicationException(String.Format("Missing value for argument '{0}'.", arg))
                End If

                i += 1
                Select Case arg.Substring(1).ToLower()
                    Case "method"
                        ParseMethod(args(i))

                    Case "security"
                        ParseSecurity(args(i))

                    Case "server"
                        ParseServer(args(i))

                    Case "from"
                        _from = args(i)

                    Case "to"
                        _to = args(i)

                    Case "subject"
                        _subject = args(i)

                    Case "body"
                        _body = args(i)

                    Case "attach"
                        AddAttachedFile(args(i))
                    Case Else
                        Throw New ApplicationException(String.Format("Unknown argument '{0}'.", arg))
                End Select
            Next i

            '
            ' Send the message
            ' 
            Send()

        Catch ex As ApplicationException
            Console.WriteLine()
            Console.WriteLine("Error: {0}", ex.Message)
            Console.WriteLine()
            ShowHelp()
            Return
        End Try
    End Sub 'RunCommandLine



    ''' <summary>
    ''' Displays command menu for interactive mode and enters a loop.
    ''' </summary>
    Private Shared Sub RunInteractiveLoop()
        Dim ret As String = ""
        Dim retValue As String = ""
        While ret <> "10"
            Try
                Console.WriteLine("MailSend" & ControlChars.Lf & "0." & ControlChars.Tab & "Select security (unsecure|secure|implicit)" & ControlChars.Lf & "1." & ControlChars.Tab & "Select send method (iis|smtp|direct)" & ControlChars.Lf & "2." & ControlChars.Tab & "Enter server and port (server[:port])" & ControlChars.Lf & "3." & ControlChars.Tab & "Enter sender address (mail@domain)" & ControlChars.Lf & "4." & ControlChars.Tab & "Enter recipient address (mail@domain;mail2@domain2)" & ControlChars.Lf & "5." & ControlChars.Tab & "Enter message subject (subject)" & ControlChars.Lf & "6." & ControlChars.Tab & "Enter message body (text)" & ControlChars.Lf & "7." & ControlChars.Tab & "Add an attachment" & ControlChars.Lf & "8." & ControlChars.Tab & "Send message" & ControlChars.Lf & "9." & ControlChars.Tab & "Remove all attachments" & ControlChars.Lf & "10." & ControlChars.Tab & "Exit" & ControlChars.Lf & "************************************************")
                Console.WriteLine("Select action(0 - 10):")
                ret = Console.ReadLine()

                Select Case ret
                    Case "0"
                        Console.WriteLine("Current security: {0}", _security)
                        Console.WriteLine("Choose a send method (unsecure,secure,implicit):")
                        retValue = Console.ReadLine()
                        ParseSecurity(retValue)

                    Case "1"
                        Console.WriteLine("Current send method: {0}", _method)
                        Console.WriteLine("Choose a send method (direct,iis,smtp):")
                        retValue = Console.ReadLine()
                        ParseMethod(retValue)

                    Case "2"
                        If Not (_server Is Nothing) Then
                            Console.WriteLine("Current server: {0}:{1}", _server, _port)
                        End If
                        Console.WriteLine("Enter a new server and port (eg.: server[:port]):")
                        retValue = Console.ReadLine()
                        ParseServer(retValue)

                    Case "3"
                        If Not (_from Is Nothing) Then
                            Console.WriteLine("Current sender address: {0}", _from)
                        End If
                        Console.WriteLine("Enter a new sender address: ")
                        retValue = Console.ReadLine()
                        _from = retValue

                    Case "4"
                        If Not (_to Is Nothing) Then
                            Console.WriteLine("Current recipient's address: {0}", _to)
                        End If
                        Console.WriteLine("Enter a new recipient's address:")
                        retValue = Console.ReadLine()
                        _to = retValue

                    Case "5"                '"subject":
                        If Not (_subject Is Nothing) Then
                            Console.WriteLine("Currrent message subject: {0}", _subject)
                        End If
                        Console.WriteLine("Enter a new message subject:")
                        retValue = Console.ReadLine()
                        _subject = retValue

                    Case "6"                '"body":
                        If Not (_body Is Nothing) Then
                            Console.WriteLine("Current message body: {0}", _body)
                        End If
                        Console.WriteLine("Enter a new message body:")
                        retValue = Console.ReadLine()
                        _body = retValue

                    Case "7"                '"attachment":
                        Console.WriteLine("Current attachments:")
                        Dim attFile As String
                        For Each attFile In _attachments
                            Console.WriteLine(attFile)
                        Next attFile
                        Console.WriteLine("******************")
                        Console.WriteLine("Enter new attachment path:")
                        retValue = Console.ReadLine()
                        AddAttachedFile(retValue)

                    Case "8"                'send
                        Send()

                    Case "9"                'remove attached files
                        _attachments.Clear()
                End Select
            Catch ex As ApplicationException
                Console.WriteLine("Error: {0}", ex.Message)
                Console.WriteLine()
            End Try
        End While
    End Sub 'RunInteractiveLoop

#Region "Helper methods"


    ''' <summary>
    ''' Displays syntax.
    ''' </summary>
    Private Shared Sub ShowHelp()
        Dim applicationName As String = AppDomain.CurrentDomain.FriendlyName
        Console.WriteLine("=====================================================================")
        Console.WriteLine(" {0} ", applicationName)
        Console.WriteLine("=====================================================================")
        Console.WriteLine("")
        Console.WriteLine("Sends e-mail from command line.")
        Console.WriteLine("")
        Console.WriteLine("The program is a sample for Rebex Secure Mail for .NET component.")
        Console.WriteLine("For more info, see http://www.rebex.net/secure-mail.net/")
        Console.WriteLine("")

        Console.WriteLine("Syntax: {0} -method (iis|smtp|direct)" & ControlChars.Lf & _
         ControlChars.Tab & "-server[:port]" & ControlChars.Lf & _
         ControlChars.Tab & "-from mail@domain" & ControlChars.Lf & _
         ControlChars.Tab & "-to mail@domain[;mail2@domain2[...]]" & ControlChars.Lf & _
         ControlChars.Tab & "-subject ""subject""" & ControlChars.Lf & _
         ControlChars.Tab & "-body ""text""" & ControlChars.Lf & _
         ControlChars.Tab & "[-attach file1.ext [-attach file2.ext [...]]" & ControlChars.Lf & _
         ControlChars.Tab & "[-security (none|explicit|implicit)]" & ControlChars.Lf & _
         ControlChars.Lf & "Or, for interactive mode: MailSend -interactive", _
         applicationName)
    End Sub 'ShowHelp


    ''' <summary>
    ''' Adds attachment to the attachment list.
    ''' </summary>
    ''' <param name="file">Full path of a file.</param>
    Private Shared Sub AddAttachedFile(ByVal file As String)
        If System.IO.File.Exists(file) = True Then
            _attachments.Add(file)
        Else
            Console.WriteLine("File '{0}' not found.", file)
        End If
    End Sub 'AddAttachedFile


    ''' <summary>
    ''' Parses the send method.
    ''' </summary>
    ''' <param name="arg">One of these values: iis, direct, smtp.</param>
    Private Shared Sub ParseMethod(ByVal arg As String)
        Try
            _method = CType([Enum].Parse(GetType(SendMethod), arg, True), SendMethod)
        Catch ex As ApplicationException
            Throw New ApplicationException("Unknown method '" + arg + "'.")
        End Try
    End Sub 'ParseMethod

    ' <summary>
    ' Parses the security.
    ' </summary>
    ' <param name="arg">One of these values: unsecure, secure, implicit.</param>
    Private Shared Sub ParseSecurity(ByVal arg As String)
        Try
            _security = CType([Enum].Parse(GetType(SslMode), arg, True), SslMode)
            If (_portSet = False) Then
                If _security = SslMode.Implicit Then
                    _port = Smtp.DefaultImplicitSslPort
                ElseIf _security = SslMode.Explicit Then
                    _port = Smtp.AlternativeExplicitSslPort
                End If

            End If
        Catch ex As ApplicationException
            Throw New ApplicationException("Unknown security '" + arg + "'.")
        End Try
    End Sub

    ''' <summary>
    ''' Parses the hostname and port.
    ''' </summary>
    ''' <param name="args">host[:port].</param>
    Private Shared Sub ParseServer(ByVal args As String)
        _port = Smtp.DefaultPort

        Dim p As Integer = args.IndexOf(":"c)
        If p >= 0 Then
            _server = args.Substring(0, p)
            _port = Integer.Parse(args.Substring((p + 1)))
            If _port <= 0 OrElse _port > 65535 Then
                Throw New ApplicationException(String.Format("Invalid port {0}.", _port))
            End If
            _portSet = True
        Else
            _server = args
            _portSet = False
        End If
    End Sub 'ParseServer

#End Region

#Region "Send methods"


    ''' <summary>
    ''' Main send method.
    ''' </summary>
    Private Shared Sub Send()
        If _method = SendMethod.Smtp AndAlso _server Is Nothing Then
            Throw New ApplicationException("Server not specified.")
        End If
        If _from Is Nothing Then
            Throw New ApplicationException("Sender (-from) not specified.")
        End If
        If _to Is Nothing Then
            Throw New ApplicationException("Recipient (-to) not specified.")
        End If
        If _subject Is Nothing Then
            Throw New ApplicationException("Mail subject not specified.")
        End If
        If _body Is Nothing Then
            Throw New ApplicationException("Mail body not specified.")
        End If
        Dim message As New MailMessage
        message.From = New MailAddressCollection(_from)
        message.To = New MailAddressCollection(_to)
        message.Subject = _subject
        message.BodyText = _body

        Dim attFile As String
        For Each attFile In _attachments
            message.Attachments.Add(New Attachment(attFile))
        Next attFile

        Try
            Select Case _method
                Case SendMethod.Smtp
                    SendSmtp(message)
                Case SendMethod.Direct
                    SendSmtpDirect(message)
                Case SendMethod.IIS
                    SendSmtpViaIIS(message)
            End Select
        Catch x As Exception
            Console.WriteLine("Error occured: {0}" + ControlChars.Lf, x.Message)
            Console.WriteLine(x)
        Finally
            _attachments.Clear()
        End Try
    End Sub    'Send


    ''' <summary>
    ''' Send using the specified SMTP server.
    ''' </summary>
    Private Shared Sub SendSmtp(ByVal message As MailMessage)
        Dim client As New Smtp
        Try
            ' set SSL parameters to accept all server certificates...
            ' do not do this in production code, server certificates should
            ' be verified - use CertificateVerifier.Default instead
            client.Settings.SslAcceptAllCertificates = True

            ' connect
            Console.WriteLine("Connecting {0} to {1}:{2}...", _security, _server, _port)
            client.Connect(_server, _port, _security)


            Console.WriteLine("Sending message...")
            client.Send(message)
        Finally
            ' disconnect
            Console.WriteLine("Disconnecting...")
            client.Disconnect()
        End Try
    End Sub    'SendSmtp


    ''' <summary>
    ''' Send using local MS IIS SMTP agent.
    ''' </summary>
    Private Shared Sub SendSmtpViaIIS(ByVal message As MailMessage)
        Console.WriteLine("Sending message through local IIS or Exchange spool directory...")
        MailSpool.Send(MailServerType.Iis, message)
    End Sub    'SendSmtpViaIIS


    ''' <summary>
    ''' Send directly to recipient's SMTP server.
    ''' </summary>
    Private Shared Sub SendSmtpDirect(ByVal message As MailMessage)
        Console.WriteLine("Sending message directly through recipient's SMTP server...")
        Smtp.SendDirect(message)
    End Sub    'SendSmtpDirect

#End Region

End Class 'MailSend 
