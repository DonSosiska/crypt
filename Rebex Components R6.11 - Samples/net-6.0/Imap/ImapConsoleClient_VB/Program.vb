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
Imports System.Collections
Imports System.IO
Imports Rebex.Net
Imports Rebex.Mail


''' <summary>
''' Sample commandline IMAP client
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
                Case Else
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
    ''' Imap client
    ''' </summary>
    Private _imap As Imap

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
        Console.WriteLine("help        connect       disconnect")
        Console.WriteLine("login       head          list")
        Console.WriteLine("get         folders       select")
        Console.WriteLine("examine     unselect      create")
        Console.WriteLine("rename      deletefolder  delete")
        Console.WriteLine("purge       secure        quit")
    End Sub 'Help

    ''' <summary>
    ''' Quit the session.
    ''' </summary>
    Private Sub Quit()
        If _imap Is Nothing Then
            Exit Sub
        End If

        If _imap.State <> ImapState.Disconnected Then
            Console.WriteLine("Disconnecting...")
            _imap.Disconnect()
        End If

        Console.WriteLine()
    End Sub 'Quit

    ''' <summary>
    ''' Get IMAP folders.
    ''' </summary>
    Private Sub GetFolderList(ByVal folder As String)
        If _imap.State = ImapState.Disconnected Then
            Console.WriteLine("Not connected.")
            Return
        End If

        Dim list As ImapFolderCollection = _imap.GetFolderList(folder)

        Dim f As ImapFolder
        For Each f In list
            Console.WriteLine("* {0}", f.Name)
        Next f
    End Sub 'GetFolderList

    ''' <summary>
    ''' Select or examine folder.
    ''' </summary>
    ''' <param name="folderName">MailBox name.</param>
    Private Sub SelectFolder(ByVal foldername As String, ByVal [readOnly] As Boolean)
        If _imap.State = ImapState.Disconnected Then
            Console.WriteLine("Not connected.")
            Return
        End If

        If foldername Is Nothing OrElse foldername.Length = 0 Then
            If [readOnly] Then
                Console.WriteLine("Usage: examine [foldername]")
            Else
                Console.WriteLine("Usage: select [foldername]")
            End If
            Return
        End If

        _imap.SelectFolder(foldername, [readOnly])
        Console.WriteLine("Folder: {0}", _imap.CurrentFolder.Name)
        Console.WriteLine("Message count: {0}", _imap.CurrentFolder.TotalMessageCount)
        Console.WriteLine("Not seen messages: {0}", _imap.CurrentFolder.NotSeenMessageCount)
        Console.WriteLine("Recent messages: {0}", _imap.CurrentFolder.RecentMessageCount)
    End Sub 'SelectFolder

    ''' <summary>
    ''' Close currently selected folder.
    ''' </summary>
    Private Sub UnselectFolder()
        If _imap.State = ImapState.Disconnected Then
            Console.WriteLine("Not connected.")
            Return
        End If

        _imap.UnselectFolder()
    End Sub 'UnselectFolder

    ''' <summary>
    ''' Create new folder.
    ''' </summary>
    Private Sub CreateFolder(ByVal foldername As String)
        If _imap.State = ImapState.Disconnected Then
            Console.WriteLine("Not connected.")
            Return
        End If

        _imap.CreateFolder(foldername)
    End Sub 'CreateFolder

    ''' <summary>
    ''' Rename folder.
    ''' </summary>
    Private Sub RenameFolder(ByVal folderName As String)
        If _imap.State = ImapState.Disconnected Then
            Console.WriteLine("Not connected.")
            Return
        End If

        Dim names As String()
        If Not folderName Is Nothing Then names = folderName.Split(" ") Else names = Nothing
        If names Is Nothing OrElse names.Length <> 2 Then
            Console.WriteLine("Usage: rename [current_folder_name] [new_folder_name]")
            Return
        End If

        Dim oldFolderName As String = names(0)
        Dim newFolderName As String = names(1)

        ' renama folder
        _imap.RenameFolder(oldFolderName, newFolderName)
    End Sub 'RenameFolder


    ''' <summary>
    ''' Delete specified folder.
    ''' </summary>
    ''' <param name="folderName"></param>
    Private Sub DeleteFolder(ByVal folderName As String)
        If _imap.State = ImapState.Disconnected Then
            Console.WriteLine("Not connected.")
            Return
        End If

        _imap.DeleteFolder(folderName)
    End Sub 'DeleteFolder


    ''' <summary>
    ''' Remove messages marked as deleted.
    ''' </summary>
    Private Sub Purge()
        If _imap.State = ImapState.Disconnected Then
            Console.WriteLine("Not connected.")
            Return
        End If

        _imap.Purge()
    End Sub 'Purge


    ''' <summary>
    ''' Show header of one message.
    ''' </summary>
    ''' <param name="uniqueId">Message unique ID.</param>
    Private Sub ShowMessageHeaders(ByVal uniqueId As String)
        ' check connection state
        If _imap.State = ImapState.Disconnected Then
            Console.WriteLine("Not connected.")
            Return
        End If

        ' show help if message unique ID is empty
        If uniqueId Is Nothing OrElse uniqueId.Length = 0 Then
            Console.WriteLine("Usage: head uniqueID")
            Return
        End If

        ' download message headers
        Dim messageInfo As ImapMessageInfo = _imap.GetMessageInfo(uniqueId, ImapListFields.FullHeaders)

        ' show headers
        Dim i As Integer
        For i = 0 To messageInfo.Headers.Count - 1
            Console.WriteLine(messageInfo.Headers(i))
        Next i
    End Sub 'ShowMessageHeaders

    ''' <summary>
    ''' Download one message.
    ''' </summary>
    ''' <param name="uniqueIdAndPath">Messsage unique ID. [path for saving message]</param>
    Private Sub DownloadMessage(ByVal uniqueIdAndPath As String)
        If _imap.State = ImapState.Disconnected Then
            Console.WriteLine("Not connected.")
            Return
        End If

        ' check number of parameters
        Dim p As String() = Nothing

        If Not (uniqueIdAndPath Is Nothing) AndAlso uniqueIdAndPath <> "" Then
            p = uniqueIdAndPath.Split(" ")
        End If
        If p Is Nothing OrElse p.Length < 0 OrElse p.Length > 2 Then
            Console.WriteLine("Usage: retr id [path]")
            Return
        End If

        If p.Length = 1 Then
            ' show message
            _imap.GetMessage(p(0), Console.OpenStandardOutput())
        Else
            ' download message and save it to file
            Dim localFilename As String = p(1)

            Dim extension As String = Path.GetExtension(localFilename)
            If String.Equals(extension, ".msg", StringComparison.OrdinalIgnoreCase) Then
                ' get message from IMAP (and parse it)
                Dim message As MailMessage = _imap.GetMailMessage(p(0))

                ' save message in MSG format
                message.Save(localFilename, MailFormat.OutlookMsg)
            Else
                ' get message from IMAP directly into file in MIME format (without parsing)
                _imap.GetMessage(p(0), localFilename)
            End If
        End If
    End Sub 'DownloadMessage


    ''' <summary>
    ''' Show message list.
    ''' </summary>
    Private Sub GetMessageList()
        Dim messageList As ImapMessageCollection

        If _imap.State = ImapState.Disconnected Then
            Console.WriteLine("Not connected.")
            Return
        End If

        '
        ' get message list
        '
        Console.WriteLine("Getting message list...")

        messageList = _imap.GetMessageList()

        If messageList.Count = 0 Then
            Console.WriteLine("No messages found.")
            Return
        End If

        '
        ' show list
        '
        ' header
        Console.WriteLine("+--------------------------------------------------------------------------+")
        Console.WriteLine("+   Unique ID         | From                                               +")
        Console.WriteLine("+                     | Subject                                            +")
        Console.WriteLine("+                     | SEQ     Date                                  Size +")
        Console.WriteLine("+--------------------------------------------------------------------------+")

        ' messages
        Dim message As ImapMessageInfo
        For Each message In messageList
            Console.WriteLine("| {0} | {1} |" & ControlChars.Lf & _
                "|                     | {2} |" & ControlChars.Lf & _
                "|                     | {3}   {4}   {5}B |", _
                message.UniqueId.ToString().PadLeft(19), _
                message.From.ToString().PadRight(50), _
                message.Subject.Substring(0, Math.Min(50, message.Subject.Length)).PadRight(50), _
                message.SequenceNumber.ToString().PadRight(4), _
                message.ReceivedDate.LocalTime.ToString("yyyy-MM-dd HH:mm:ss").PadRight(32), _
                message.Length.ToString().PadLeft(7))

            Console.WriteLine("+--------------------------------------------------------------------------+")
        Next message
    End Sub 'GetMessageList


    ''' <summary>
    ''' Delete message.
    ''' </summary>
    ''' <param name="uniqueId">Message unique ID.</param>
    Private Sub DeleteMessage(ByVal uniqueId As String)
        If _imap.State = ImapState.Disconnected Then
            Console.WriteLine("Not connected.")
            Return
        End If

        If uniqueId Is Nothing OrElse uniqueId.Length = 0 Then
            Console.WriteLine("Usage: delete uniqueID")
            Return
        End If

        ' Mark message as deleted
        _imap.DeleteMessage(uniqueId)
    End Sub 'DeleteMessage


    ''' <summary>
    ''' Open a new session.
    ''' </summary>
    ''' <param name="arg">Hostname[:port] [none|explicit|implicit].</param>
    ''' <returns>Successful?</returns>
    Private Function Connect(ByVal arg As String) As Boolean
        ' create IMAP object
        _imap = New Imap
        _imap.LogWriter = New ConsoleLogWriter(LogLevel.Info)

        ' handle certificate validation
        AddHandler _imap.ValidatingCertificate, AddressOf ConsoleVerifier.ValidatingCertificate

        Dim host As String
        Dim port As Integer = -1
        Dim portSet As Boolean = False
        Dim mode As SslMode = SslMode.None

        Try
            ' check imap status
            If _imap.State <> ImapState.Disconnected Then
                Console.WriteLine("Already connected.")
                Return False
            End If

            ' hostname included?
            If arg Is Nothing OrElse arg.Trim() = "" Then
                ' read hostname
                Console.Write("Hostname: ")
                host = Console.ReadLine().Trim()

                ' read security
                Console.Write("Security [none|explicit|implicit] (default=none): ")
                Dim security As String = Console.ReadLine().Trim()
                If security <> "" Then
                    Try
                        mode = CType([Enum].Parse(GetType(SslMode), security, True), SslMode)
                    Catch
                        Console.WriteLine("Invalid security type. Use one of none|explicit|implicit.")
                        Return False
                    End Try
                End If

                ' read port
                If (mode = SslMode.Implicit) Then
                    Console.Write("Port (default={0}): ", Imap.DefaultImplicitSslPort)
                Else
                    Console.Write("Port (default={0}): ", Imap.DefaultPort)
                End If

                Dim sPort As String = Console.ReadLine().Trim()
                If sPort <> "" Then
                    portSet = True
                    Integer.TryParse(sPort, port)
                End If
            Else
                Dim p As String() = arg.Trim().Split(" ")

                ' parse host and security
                Select Case p.Length
                    Case 1
                        host = p(0)
                    Case 2
                        host = p(0)
                        Try
                            mode = CType([Enum].Parse(GetType(SslMode), p(1), True), SslMode)
                        Catch
                            Console.WriteLine("Invalid security type. Use one of none|explicit|implicit.")
                            Return False
                        End Try
                    Case Else
                        Console.WriteLine("Usage: connect hostname[:port] [none|explicit|implicit]")
                        Return False
                End Select

                ' parse port
                p = host.Split(":")
                Select Case p.Length
                    Case 1
                        Exit Select
                    Case 2
                        host = p(0)
                        portSet = True
                        Integer.TryParse(p(1), port)
                    Case Else
                        Console.WriteLine("Usage: connect hostname[:port] [none|explicit|implicit]")
                        Return False
                End Select
            End If

            If portSet AndAlso (port <= 0 OrElse port > 65535) Then
                Console.WriteLine("Invalid port number.")
                Return False
            End If

            ' try to connect

            If (portSet) Then
                _imap.Connect(host, port, mode)
            Else
                _imap.Connect(host, mode)
            End If

            If mode <> SslMode.None Then
                _secured = True
            Else
                _secured = False
            End If
            Return True

        Catch e As ImapException
            If e.Status = ImapExceptionStatus.ProtocolError Then
                Console.WriteLine("Server does not support SSL/TLS security, disconnecting.")
                _imap.Disconnect()
                Return False
            End If
            Console.WriteLine(e.Message)
            Return False

        Catch e As TlsException
            Console.WriteLine(e.Message)
            Console.WriteLine("TLS negotiation failed, disconnecting.")
            _imap.Disconnect()
            Return False
        End Try
    End Function 'Connect

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
        _imap.Login(user, pass)
        _logged = True

        Console.WriteLine("Logged in successfully.")
        Console.WriteLine("Use 'folders' to get a folder list or 'select Inbox' to select the Inbox folder, or type 'help' for list of supported commands.")
    End Sub 'Login

    ''' <summary>
    ''' Disconnect.
    ''' </summary>
    Private Sub Disconnect()

        ' check imap status

        If _imap.State = ImapState.Disconnected Then
            Console.WriteLine("Not connected.")
            Return
        End If

        _imap.Disconnect()
        _logged = False
        _secured = False
        Console.WriteLine("Disconnect successful.")
    End Sub

    ''' <summary>
    ''' Secure the connection.
    ''' </summary>
    Private Sub Secure()

        ' check imap status

        If _imap.State = ImapState.Disconnected Then
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
            _imap.Secure()
            _secured = True
            Console.WriteLine("Connection is secured.")
        Catch e As ImapException
            If e.Status = ImapExceptionStatus.ProtocolError Then
                Console.WriteLine("Server does not support SSL/TLS security, disconnecting.")
            End If
            Console.WriteLine(e.Message)
            _imap.Disconnect()

        Catch e As TlsException
            Console.WriteLine(e.Message)
            Console.WriteLine("TLS negotiation failed, disconnecting.")
            _imap.Disconnect()
        End Try
    End Sub

    ''' <summary>
    ''' Main loop.
    ''' </summary>
    Public Sub Run()
        While True
            Console.Write("imap> ")

            Dim command As String = Console.ReadLine().Trim()

            Dim param As String = Nothing

            Dim i As Integer = command.IndexOf(" ")

            If i > 0 Then
                param = command.Substring((i + 1))
                command = command.Substring(0, i)
            End If

            Try
                Select Case command.ToLower()
                    Case "?", "help" ' Show help
                        Help()

                    Case "!", "bye", "exit", "quit" ' Quit application
                        Quit()
                        Return

                    Case "open", "connect" ' Connect to IMAP server
                        If Connect(param) Then Login()

                    Case "close", "disconnect" ' Disconnect
                        Disconnect()

                    Case "secure" ' Secure the connection
                        Secure()

                    Case "login" ' Login
                        Login()

                    Case "head" ' Show message headers
                        ShowMessageHeaders(param)

                    Case "list" ' Show message list
                        GetMessageList()

                    Case "get", "retr" ' Download message
                        DownloadMessage(param)

                    Case "folders" ' Show folder list
                        GetFolderList(param)

                    Case "select" ' Select folder
                        SelectFolder(param, False)

                    Case "examine" ' Examine
                        SelectFolder(param, True)

                    Case "unselect" ' Unselect folder
                        UnselectFolder()

                    Case "create" ' Create folder
                        CreateFolder(param)

                    Case "rename" ' Rename folder
                        RenameFolder(param)

                    Case "deletefolder" ' Delete folder
                        DeleteFolder(param)

                    Case "del", "delete" ' Mark message as deleted
                        DeleteMessage(param)

                    Case "purge" ' Remove deleted message from folder
                        Purge()

                    Case Else ' Default - invalid command
                        Console.WriteLine("Invalid command.")
                End Select
            Catch e As ArgumentException
                Console.WriteLine(e.Message)
            Catch e As InvalidOperationException
                Console.WriteLine(e.Message)
            Catch e As ImapException
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


        Dim program As New Program

        If args.Length > 0 Then
            Dim param As String = args(0)
            Dim i As Integer
            For i = 1 To args.Length - 1
                param = param & " " & args(i)
            Next i

            If program.Connect(param) Then program.Login()

        End If
        program.Run()
    End Sub 'Main

End Class 'Program
