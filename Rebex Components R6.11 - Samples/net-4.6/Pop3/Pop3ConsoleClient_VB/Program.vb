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
Imports System.Text
Imports Rebex.Net
Imports System.IO
Imports Rebex.Mail


''' <summary>
''' Sample commandline POP3 client
''' </summary>

Class Program

        Public Function ReadLine() As String
            Dim line As StringBuilder = New StringBuilder
            Dim complete As Boolean = False

            While (Not complete)
                Dim key As ConsoleKeyInfo = Console.ReadKey(True)
                Select Case key.Key
                    Case ConsoleKey.Enter
                        complete = True
                    Case ConsoleKey.Backspace
                        If (line.Length > 0) Then
                            line = line.Remove(line.Length - 1, 1)
                            Console.Write(key.KeyChar)
                            Console.Write(" ")
                            Console.Write(key.KeyChar)
                        End If
                    case else
                        If ((key.KeyChar >= " "c) OrElse (key.Key = ConsoleKey.Tab)) Then
                            line = line.Append(key.KeyChar)
                            Console.Write("*")
                        End If
                End Select
            End While

            Console.WriteLine()
            Return line.ToString()
        End Function

    ''' <summary>
    ''' Pop3 client 
    ''' </summary>
    Private _pop3 As Pop3

    Private _security As SslMode = SslMode.None
    Private _logged As Boolean = False
    Private _secured As Boolean = False

    ''' <summary>
    ''' Initializes a new instance of the <see cref="Program"/> class.
    ''' </summary>
    Public Sub New()
       
    End Sub 'New

    ''' <summary>
    ''' Show program help.
    ''' </summary>
    Private Sub Help()
        Console.WriteLine("help      connect")
        Console.WriteLine("login     head")
        Console.WriteLine("list      retr")
        Console.WriteLine("delete    undelete")
        Console.WriteLine("secure")
        Console.WriteLine("quit      disconnect")
    End Sub 'Help

    ''' <summary>
    ''' Quit the session.
    ''' </summary>
    Private Sub Quit()
        If _pop3 Is Nothing Then
            Return
        End If

        If _pop3.State <> Pop3State.Disconnected Then
            Console.WriteLine("Disconnecting...")
            _pop3.Disconnect()
        End If

        Console.WriteLine()
    End Sub 'Quit


    ''' <summary>
    ''' Show header of one message.
    ''' </summary>
    ''' <param name="stringMessageId">String containing the Message ID supplied by user.</param>
    Private Sub ShowMessageHeaders(ByVal stringMessageId As String)
        ' Check connection
        If _pop3.State = Pop3State.Disconnected Then
            Console.WriteLine("Not connected.")
            Return
        End If

        ' empty message id? Show help
        If stringMessageId Is Nothing OrElse stringMessageId.Length = 0 Then
            Console.WriteLine("Usage: head id")
            Return
        End If

        ' try to parse message id
        Dim id As Integer = ParseMessageId(stringMessageId)

        ' is id valid?
        If id <= 0 Then
            Console.WriteLine("Invalid message ID.")
            Return
        End If

        ' Download message headers
        Dim messageInfo As Pop3MessageInfo = _pop3.GetMessageInfo(id, Pop3ListFields.FullHeaders)

        ' Show headers
        Dim i As Integer
        For i = 0 To messageInfo.Headers.Count - 1
            Console.WriteLine(messageInfo.Headers(i))
        Next i
    End Sub 'ShowMessageHeaders

    ''' <summary>
    ''' Download one message.
    ''' </summary>
    ''' <param name="param">Messsage ID. [path for saving message]</param>
    Private Sub DownloadMessage(ByVal param As String)
        If _pop3.State = Pop3State.Disconnected Then
            Console.WriteLine("Not connected.")
            Return
        End If

        ' check number of parameters
        Dim p As String() = Nothing

        If Not (param Is Nothing) AndAlso param <> "" Then
            p = param.Split(" ")
        End If
        If p Is Nothing OrElse p.Length < 0 OrElse p.Length > 2 Then
            Console.WriteLine("Usage: retr id [path]")
            Return
        End If

        ' try to parse message id
        Dim id As Integer = ParseMessageId(p(0))

        ' is id valid?
        If id <= 0 Then
            Console.WriteLine("Invalid message ID.")
            Return
        End If

        If p.Length = 1 Then
            ' show only
            _pop3.GetMessage(id, Console.OpenStandardOutput())
        Else
            ' download message and save it to file
            Dim localFilename As String = p(1)
            Dim extension As String = Path.GetExtension(localFilename)

            If (String.Equals(extension, ".msg", StringComparison.OrdinalIgnoreCase)) Then
                ' get message from POP3, parse it
                Dim message As MailMessage = _pop3.GetMailMessage(id)
                ' save the message in MSG format
                message.Save(localFilename, MailFormat.OutlookMsg)
            Else
                ' save message from POP3 directly in MIME format (without parsing)
                _pop3.GetMessage(id, localFilename)
            End If

        End If
    End Sub 'DownloadMessage


    ''' <summary>
    ''' Show message list.
    ''' </summary>
    Private Sub List()
        Dim messageList As Pop3MessageCollection

        If _pop3.State = Pop3State.Disconnected Then
            Console.WriteLine("Not connected.")
            Return
        End If

        '
        ' get message list
        '
        Dim messageCount As Integer = _pop3.GetMessageCount()

        Console.WriteLine("Getting {0} ({1} bytes)...", FormatMessageCount(messageCount).ToString(), _pop3.GetMailboxSize())
        messageList = _pop3.GetMessageList(Pop3ListFields.Fast)

        If messageList.Count = 0 Then
            Console.WriteLine("No messages found.")
            Return
        End If

        '
        ' show list
        '
        ' header
        Console.WriteLine("+----------------------------------------------------------------------+")
        Console.WriteLine("| S.No |    Length | Unique ID                                         |")
        Console.WriteLine("+----------------------------------------------------------------------+")

        ' message list
        Dim message As Pop3MessageInfo
        For Each message In messageList
            Console.WriteLine("| {0} |{1} B | {2} |", _
                message.SequenceNumber.ToString().PadLeft(4), _
                message.Length.ToString().PadLeft(8), _
                message.UniqueId.ToString().PadLeft(48))
        Next message

        Console.WriteLine("+----------------------------------------------------------------------+")
    End Sub 'List


    ''' <summary>
    ''' Delete message.
    ''' </summary>
    ''' <param name="messageIdString">Message ID.</param>
    Private Sub Delete(ByVal messageIdString As String)
        If _pop3.State = Pop3State.Disconnected Then
            Console.WriteLine("Not connected.")
            Return
        End If

        If messageIdString Is Nothing OrElse messageIdString.Length = 0 Then
            Console.WriteLine("Usage: delete id")
            Return
        End If

        Dim id As Integer = ParseMessageId(messageIdString)

        If id <= 0 Then
            Console.WriteLine("Invalid message ID.")
            Return
        End If

        ' Mark message as deleted
        _pop3.Delete(id)
    End Sub 'Delete


    ''' <summary>
    ''' Undelete message.
    ''' </summary>
    Private Sub Undelete()
        If _pop3.State = Pop3State.Disconnected Then
            Console.WriteLine("Not connected.")
            Return
        End If

        _pop3.Undelete()
    End Sub 'Undelete


    ''' <summary>
    ''' Open a new session.
    ''' </summary>
    ''' <param name="host">Host [port] [none|explicit|implicit].</param>
    ''' <returns>Successful?</returns>
    Private Function Connect(ByVal host As String) As Boolean
        ' create Pop3 object 
        _pop3 = New Pop3
        _pop3.LogWriter = New ConsoleLogWriter(LogLevel.Info)

        ' handle certificate validation
        AddHandler _pop3.ValidatingCertificate, AddressOf ConsoleVerifier.ValidatingCertificate

        Dim port As Integer = -1

        Try
            ' check pop3 status
            If _pop3.State <> Pop3State.Disconnected Then
                Console.WriteLine("Already connected.")
                Return False
            End If

            ' hostname included?
            If host Is Nothing OrElse host.Trim() = "" Then
                ' read hostname
                Console.Write("Hostname: ")
                host = Console.ReadLine().Trim()

                ' read port
                Console.Write("Port (default={0}): ", Pop3.DefaultPort)
                Dim sPort As String = Console.ReadLine().Trim()
                If sPort = "" Then
                    port = Pop3.DefaultPort
                Else
                    Try
                        port = Integer.Parse(sPort)
                    Catch
                        port = -1
                    End Try
                End If

                ' read security
                Console.Write("Security [none|explicit|implicit] (default=unsecure): ")
                Dim security As String = Console.ReadLine().Trim()
                If security = "" Then
                    _security = SslMode.None
                Else
                    Try
                        _security = CType([Enum].Parse(GetType(SslMode), security, True), SslMode)
                    Catch
                        Console.WriteLine("Invalid security type. Use one of none|explicit|implicit.")
                        Return False
                    End Try
                End If
            Else

                ' check number of parameters
                Dim p As String() = host.Split(" ")

                If p.Length < 1 OrElse p.Length > 3 Then
                    Console.WriteLine("Usage: hostname [port] [none|explicit|implicit]")
                    Return False
                End If
                ' set hsot
                host = p(0)

                ' set port and security
                port = Pop3.DefaultPort
                _security = SslMode.None
                Select Case p.Length
                    Case 2
                        Try
                            port = Integer.Parse(p(1))
                        Catch
                            Try
                                _security = CType([Enum].Parse(GetType(SslMode), p(1), True), SslMode)
                            Catch
                                Console.WriteLine("Invalid argument '{0}'.", p(1))
                                Console.WriteLine("Usage: connect hostname [port] [none|explicit|implicit]")
                                Return False
                            End Try
                        End Try

                    Case 3
                        Try
                            port = Integer.Parse(p(1))
                        Catch
                            port = -1
                        End Try

                        Try
                            _security = CType([Enum].Parse(GetType(SslMode), p(2), True), SslMode)
                        Catch
                            Console.WriteLine("Invalid security type. Use one of none|explicit|implicit.")
                            Return False
                        End Try
                End Select
            End If

            If port <= 0 OrElse port > 65535 Then
                Console.WriteLine("Invalid port number.")
                Return False
            End If

            _secured = _security <> SslMode.None

            ' try to connect
            _pop3.Connect(host, port, _security)

            If _secured Then
                Console.WriteLine("Connection was secured using {0}.", _pop3.TlsSocket.Cipher.Protocol)
                Console.WriteLine("Connection is using cipher {0}.", _pop3.TlsSocket.Cipher)
            End If

            Return True

        Catch e As Pop3Exception
            If e.Status = Pop3ExceptionStatus.ProtocolError Then
                Console.WriteLine("Server does not support SSL/TLS security, disconnecting.")
                _pop3.Disconnect()
                Return False
            End If
            Console.WriteLine(e.Message)
            Return False

        Catch e As TlsException
            Console.WriteLine(e.Message)
            Console.WriteLine("TLS negotiation failed, disconnecting.")
            _pop3.Disconnect()
            Return False
        End Try
    End Function 'Connect

    ''' <summary>
    ''' Disconnect.
    ''' </summary>
    ''' If set to <param name="rollback">rollback</param>, don't delete messages marked as deleted. 
    ''' It is same as you type Undelete and then Disconnect without arguments
    Private Sub Disconnect(ByVal rollback As String)

        ' check pop3 status

        If _pop3.State = Pop3State.Disconnected Then
            Console.WriteLine("Not connected.")
            Return
        End If

        If rollback Is Nothing Or rollback = "" Then
            Console.WriteLine("Disconnecting...")
            ' commit changes and disconnect
            _pop3.Disconnect(False)
        ElseIf rollback.ToLower() = "rollback" Then
            Console.WriteLine("Disconnecting...")
            ' undelete and disconnect
            _pop3.Disconnect(True)
        Else
            Console.WriteLine("Usage: disconnect [rollback]")
            Return
        End If

        _logged = False
        _secured = False
        Console.WriteLine("Disconnect successful.")
    End Sub

    ''' <summary>
    ''' Login
    ''' </summary>
    Private Sub Login()
        ' get username
        Console.Write("User: ")
        Dim user As String = Console.ReadLine()

        ' get password
        Console.Write("Password: ")
        Dim pass As String = ReadLine()

        ' login
        _pop3.Login(user, pass)
        _logged = True

        Console.WriteLine("Logged in successfully.")
    End Sub 'Login

    ''' <summary>
    ''' Secure the connection.
    ''' </summary>
    Private Sub Secure()

        ' check pop3 status

        If _pop3.State = Pop3State.Disconnected Then
            Console.WriteLine("Not connected.")
            Return
        End If

        If _logged Then
            Console.WriteLine("'secure' command is not available after logged in.")
            Return
        End If

        If _secured Then
            Console.WriteLine("Connection is already secure.")
            Return
        End If

        Try
            _pop3.Secure()
            _secured = True
            Console.WriteLine("Connection is secured.")
        Catch e As Pop3Exception
            If e.Status = Pop3ExceptionStatus.ProtocolError Then
                Console.WriteLine("Server does not support SSL/TLS security, disconnecting.")
            End If
            Console.WriteLine(e.Message)
            _pop3.Disconnect()

        Catch e As TlsException
            Console.WriteLine(e.Message)
            Console.WriteLine("TLS negotiation failed, disconnecting.")
            _pop3.Disconnect()
        End Try
    End Sub

    ''' <summary>
    ''' Main loop.
    ''' </summary>
    Public Sub Run()
        While True
            Console.Write("pop3> ")

            Dim command As String = Console.ReadLine().Trim()
            Dim param As String = Nothing
            Dim i As Integer = command.IndexOf(" ")

            If i > 0 Then
                param = command.Substring(i + 1)
                command = command.Substring(0, i)
            End If

            Try
                Select Case command.ToLower()
                    Case "?", "help" ' Show help
                        Help()

                    Case "!", "bye", "exit", "quit" ' Quit application
                        Quit()
                        Return

                    Case "open", "connect" ' Connect to POP3 server
                        If Connect(param) Then Login()

                    Case "login" ' Login to POP3 server
                        Login()

                    Case "close", "disconnect" ' Disconnect
                        Disconnect(param)

                    Case "secure" ' Secure the connection
                        Secure()

                    Case "head" ' Show message headers
                        ShowMessageHeaders(param)

                    Case "list" ' Show message list
                        List()

                    Case "get", "retr" ' Show full message
                        DownloadMessage(param)

                    Case "del", "delete" ' Delete a message
                        Delete(param)

                    Case "undel", "undelete" ' Undelete a message
                        Undelete()

                    Case Else ' Default - invalid command
                        Console.WriteLine("Invalid command.")
                End Select
            Catch e As ArgumentException
                Console.WriteLine(e.Message)
            Catch e As InvalidOperationException
                Console.WriteLine(e.Message)
            Catch e As Pop3Exception
                Console.WriteLine(e.Message)
            End Try
        End While
    End Sub 'Run

    ''' <summary>
    ''' The main entry point for the application.
    ''' </summary>
    <STAThread()> _
    Overloads Shared Sub Main(ByVal args() As String)
            ' set the trial license key
            Rebex.Licensing.Key = Rebex.Samples.TrialKey.Key


        Dim program As New program

        If args.Length > 0 Then
            Dim param As String = args(0)
            Dim i As Integer
            For i = 1 To args.Length - 1
                param = param & " " & args(i)
            Next i
            If program.Connect(param) Then
                program.Login()
            End If
        End If
        program.Run()
    End Sub 'Main


    ''' <summary>
    ''' Converts string message id to integer. If string is invalid returns -1
    ''' </summary>
    ''' <param name="stringId">string message id</param>
    ''' <returns>valid id or -1</returns>
    Public Function ParseMessageId(ByVal stringId As String) As Integer
        Try
            Return Integer.Parse(stringId)
        Catch
            Return -1
        End Try
    End Function 'ParseMessageId


    ''' <summary>
    ''' Converts message count from integer to human-friendly form. 
    ''' </summary>
    ''' <param name="count"></param>
    ''' <returns></returns>
    Public Function FormatMessageCount(ByVal count As Integer) As String
        If count = 1 Then
            Return "1 message"
        Else
            Return String.Format("{0} messages", count)
        End If
    End Function 'FormatMessageCount

End Class 'Program
