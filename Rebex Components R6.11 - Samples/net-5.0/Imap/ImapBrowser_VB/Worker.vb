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
Imports System.Diagnostics
Imports System.Net
Imports System.Text
Imports System.Threading
Imports System.Collections
Imports System.IO
Imports Rebex.Net
Imports Rebex.Mime
Imports Rebex.Mail

''' <summary>
''' Background worker class. Implements all non-GUI functionality.
''' Invokes the main form's methods to update the GUI.
''' </summary>

Public Class Worker
    Private _config As Configuration
    Private _imap As Imap
    Private _knownMessages As Hashtable
    Private _owner As RemoteMailbox
    Private _messageList As ImapMessageCollection
    Private _aborting As Integer
    Private _folder As String
    Private _sync As New Object
    Private _newMessage As Integer
    Private _disposed As Integer

    Private Delegate Sub ValidatingCertificateDelegate(sender As Object, e As SslCertificateValidationEventArgs)

    ''' <summary>
    ''' Initializes the instance of the worker process.
    ''' </summary>
    ''' <param name="owner">Owner form of this worker object.</param>
    ''' <param name="config">Configuration object.</param>
    Public Sub New(ByVal owner As RemoteMailbox, ByVal config As Configuration)
        _config = config
        _owner = owner
        _knownMessages = New Hashtable
        _imap = New Imap
        AddHandler _imap.Notification, AddressOf imap_Notification
        AddHandler _imap.ValidatingCertificate, AddressOf imap_ValidatingCertificate
    End Sub    'New

    Private Sub imap_ValidatingCertificate(sender As Object, e As SslCertificateValidationEventArgs)
        _owner.SafeInvoke(New ValidatingCertificateDelegate(AddressOf Verifier.ValidatingCertificate), New Object() {sender, e})
    End Sub


    Public Sub SetFolder(ByVal folder As String)
        If _folder <> folder Then
            ' Forget the old message list if folder has changed.
            _knownMessages.Clear()
            _messageList = Nothing
        End If
        _folder = folder
    End Sub    'SetFolder

    Public Sub ClearCachedMessages()
        _messageList = Nothing
    End Sub

    ''' <summary>
    ''' Gets the object that represents the IMAP session.
    ''' </summary>
    Public ReadOnly Property Client() As Imap
        Get
            Return _imap
        End Get
    End Property

    ''' <summary>
    ''' Gets or sets whether RetrieveMessageList finished sucessfully.
    ''' </summary>
    Public ReadOnly Property RetrieveMessageListFinished() As Boolean
        Get
            If (Not _messageList Is Nothing) AndAlso _knownMessages.Count = _messageList.Count Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property

    ''' <summary>
    ''' Gets the object that represents the IMAP session.
    ''' </summary>
    Public Property CheckForUpdatesEnabled() As Boolean
        Get
            ' Because the _checkForUpdatesEnabled field might have been changed from another thread,
            ' and because VB.NET still has no 'Volatile' keyword, we have to use a bit
            ' more complicated code. However, it is a thread-safe equivalent of:
            ' Return _checkForUpdatesEnabled <> 0
            Return Interlocked.CompareExchange(_checkForUpdatesEnabled, 0, 0) <> 0
        End Get
        Set(ByVal Value As Boolean)
            If Value Then
                _checkForUpdatesEnabled = 1
            Else
                _checkForUpdatesEnabled = 0
            End If
        End Set
    End Property
    Private _checkForUpdatesEnabled As Integer


    ''' <summary>
    ''' Aborts the current IMAP operation.
    ''' </summary>
    Public Sub Abort(ByVal abortHandler As EventHandler)
        SyncLock _sync
            ' Because the _aborting field might have been changed from another thread,
            ' and because VB.NET still has no 'Volatile' keyword, we have to use a bit
            ' more complicated code. However, it is a thread-safe equivalent of:
            ' If _aborting <> 0 Then Return
            If Interlocked.CompareExchange(_aborting, 0, 0) <> 0 Then Return

            If IsRunning Then
                _abortHandler = abortHandler
                _imap.Abort()
                _aborting = 1
                Return
            End If
        End SyncLock

        If Not (abortHandler Is Nothing) Then
            abortHandler(Me, New EventArgs)
        End If
    End Sub       'Abort

    ''' <summary>
    ''' Clean up any resources being used.
    ''' </summary>
    Public Sub Dispose()
        If _thread Is Nothing Then
            _imap.Disconnect()
        End If
        _imap.Dispose()
        _disposed = 1
    End Sub       'Dispose


    ''' <summary>
    ''' Gets the message list from the IMAP connection.
    ''' </summary>
    Private Sub GetMessageList()
        If _messageList Is Nothing Then
            _messageList = _imap.GetMessageList(ImapListFields.UniqueId)
        End If
    End Sub       'GetMessageList

    ' Main form methods delegates.
    Delegate Sub MessageDelegate(ByVal message As String)

    Delegate Sub AddMessageDelegate(ByVal message As ImapMessageInfo, ByVal [error] As Boolean)

    Delegate Sub ShowMessageDelegate(ByVal message As MimeMessage, ByVal raw() As Byte)


    ''' <summary>
    ''' Invokes the main form's method to update the status bar text.
    ''' </summary>
    ''' <param name="message">Status bar text.</param>
    Private Sub SetStatus(ByVal message As String)
        _owner.SafeInvoke(New MessageDelegate(AddressOf _owner.SetStatus), New Object() {message})
    End Sub       'SetStatus


    ''' <summary>
    ''' Invokes the main form's method to add a message info to the list.
    ''' </summary>
    ''' <param name="message">Message info.</param>
    ''' <param name="error">True if message was unparsable.</param>
    Private Sub AddMessage(ByVal message As ImapMessageInfo, ByVal [error] As Boolean)
        _owner.SafeInvoke(New AddMessageDelegate(AddressOf _owner.AddMessage), New Object() {message, [error]})

        SyncLock _knownMessages.SyncRoot
            _knownMessages.Add(message.UniqueId, message.UniqueId)
        End SyncLock
    End Sub       'AddMessage


    ''' <summary>
    ''' Invokes the main form's method to remove a message from the list.
    ''' </summary>
    ''' <param name="uniqueId">Message unique ID.</param>
    Private Sub RemoveMessage(ByVal uniqueId As String)
        _owner.SafeInvoke(New MessageDelegate(AddressOf _owner.RemoveMessage), New Object() {uniqueId})
        ForgetMessage(uniqueId)
    End Sub       'RemoveMessage


    ''' <summary>
    ''' Invokes the main form's method to display a message.
    ''' </summary>
    ''' <param name="message">Mime message.</param>
    ''' <param name="raw">Raw mssage data.</param>
    Private Sub ShowMessage(ByVal message As MimeMessage, ByVal raw() As Byte)
        _owner.SafeInvoke(New ShowMessageDelegate(AddressOf _owner.ShowMessage), New Object() {message, raw})
    End Sub       'ShowMessage


    ''' <summary>
    ''' Invokes the main form's method to display a MessageBox.
    ''' </summary>
    ''' <param name="message">Message to display.</param>
    Private Sub ShowMessageBox(ByVal message As String)
        _owner.SafeInvoke(New MessageDelegate(AddressOf _owner.ShowMessageBox), New Object() {message})
    End Sub       'ShowMessageBox


    ''' <summary>
    ''' Determines whether the message unique ID is known.
    ''' </summary>
    ''' <param name="uniqueId">Unique ID.</param>
    Public Function IsKnownMessage(ByVal uniqueId As String) As Boolean
        SyncLock _knownMessages.SyncRoot
            Return _knownMessages.ContainsKey(uniqueId)
        End SyncLock
    End Function          'IsKnownMessage


    ''' <summary>
    ''' Forgets the specified message unique IDs.
    ''' </summary>
    ''' <param name="uniqueId">Unique ID.</param>
    Public Sub ForgetMessage(ByVal uniqueId As String)
        SyncLock _knownMessages.SyncRoot
            _knownMessages.Remove(uniqueId)
        End SyncLock
    End Sub       'ForgetMessage


    ''' <summary>
    ''' Forgets all known message unique IDs.
    ''' </summary>
    Public Sub ForgetMessages()
        SyncLock _knownMessages.SyncRoot
            _knownMessages.Clear()
        End SyncLock
    End Sub       'ForgetMessages


    ''' <summary>
    ''' Callback method delegete.
    ''' </summary>
    Delegate Sub FinishedDelegate(ByVal [error] As Exception)

    ' Fields required by the background method.
    Private _parameter As Object = Nothing
    Private _method As BackgroundMethod = Nothing
    Private _thread As Thread = Nothing
    Private _callback As FinishedDelegate = Nothing
    Private _abortHandler As EventHandler = Nothing


    ''' <summary>
    ''' Background method delegate.
    ''' </summary>
    Delegate Sub BackgroundMethod()

    ''' <summary>
    ''' Gets the value indicating whether a background operation is running.
    ''' </summary>
    Public ReadOnly Property IsRunning() As Boolean
        Get
            Return Not (_thread Is Nothing)
        End Get
    End Property


    ''' <summary>
    ''' Starts the requested asynchronous operation.
    ''' </summary>
    ''' <param name="method">Background method.</param>
    ''' <param name="parameter">Background method argument.</param>
    ''' <param name="callback">Callback method to be called when the operation ends.</param>
    Private Sub Start(ByVal method As BackgroundMethod, ByVal parameter As Object, ByVal callback As FinishedDelegate)
        If IsRunning Then
            Throw New InvalidOperationException("Another operation is still running.")
        End If
        ' Initialize fields required by the background method.
        _aborting = 0
        _method = method
        _parameter = parameter
        _callback = callback


        ' Start the background method in a background thread.
        _thread = New Thread(New ThreadStart(AddressOf DoMethod))
        _thread.Start()
    End Sub       'Start


    ''' <summary>
    ''' Common background operation stub. Connects, logs in and executes the desired background operation.
    ''' </summary>
    Private Sub DoMethod()
        Dim connecting As Boolean = False
        Dim [error] As Exception
        Try
            If _imap.State = ImapState.Disconnected Then
                connecting = True

                ' Get configuration.
                Dim server As String = _config.GetString("server")
                Dim port As Integer = _config.GetInt32("port")
                Dim singleSignOn As Boolean = _config.GetBoolean("singleSignOn", False)
                Dim userName As String = _config.GetString("userName")
                Dim password As String = _config.GetString("password")
                Dim security As SslMode
                Select Case _config.GetInt32("security", 0)
                    Case 1
                        security = SslMode.Explicit
                    Case 2
                        security = SslMode.Implicit
                    Case Else
                        security = SslMode.None
                End Select

                _imap.Settings.SslAllowedVersions = CType(_config.GetValue("protocol", GetType(TlsVersion)), TlsVersion)
                _imap.Settings.SslAllowedSuites = CType(_config.GetValue("suite", GetType(TlsCipherSuite)), TlsCipherSuite)

                SetStatus(String.Format("Connecting to {0}...", server))

                ' Forget the old message list.
                _messageList = Nothing

                ' Connect to the server with the specified security.
                _imap.Connect(server, port, security)

                If singleSignOn Then
                    _imap.Login(ImapAuthentication.Auto)
                Else
                    _imap.Login(userName, password)
                End If

                connecting = False
            End If

            If Not (_folder Is Nothing) Then
                _imap.SelectFolder(_folder)
            End If

            ' Run desired background operation.
            _method()

            [error] = Nothing
        Catch x As Exception

            Dim px As ImapException = Nothing

            If TypeOf x Is ImapException Then
                px = CType(x, ImapException)
            End If

            If Not (px Is Nothing) AndAlso px.Status = ImapExceptionStatus.OperationAborted Then
                ' If the operation is a result of an abort, don't report it as error.
                [error] = Nothing
                _imap.Disconnect()
            Else
                [error] = x

                ' If this is a failed connection attempt, disconnect.
                If connecting Then
                    _imap.Disconnect()
                End If
            End If
        End Try

        ' Because the _disposed field might have been changed from another thread,
        ' and because VB.NET still has no 'Volatile' keyword, we have to use a bit
        ' more complicated code. However, it is a thread-safe equivalent of:
        ' If _disposed <> 0 Then Return
        If Interlocked.CompareExchange(_disposed, 0, 0) <> 0 Then Return

        Dim abortHandler As EventHandler

        SyncLock _sync
            _aborting = 0
            _thread = Nothing
            abortHandler = _abortHandler
            _abortHandler = Nothing
        End SyncLock

        ' Resets status bar and informs the main form that the operation finished.
        SetStatus("")
        If Not (_callback Is Nothing) Then
            _owner.SafeInvoke(_callback, New Object() {[error]})
        End If
        If Not (abortHandler Is Nothing) Then
            _owner.SafeInvoke(abortHandler, New Object() {New EventArgs})
        End If
    End Sub       'DoMethod

    ''' <summary>
    ''' Starts an asynchronous operation to connect to the IMAP server.
    ''' </summary>
    ''' <param name="callback">Callback method to be called when the operation ends.</param>
    Public Sub StartConnecting(ByVal callback As FinishedDelegate)
        Start(New BackgroundMethod(AddressOf DoConnect), Nothing, callback)
    End Sub       'StartConnecting


    ''' <summary>
    ''' Starts an asynchronous operation to retrieve the message list.
    ''' </summary>
    ''' <param name="callback">Callback method to be called when the operation ends.</param>
    Public Sub StartRetrieveMessageList(ByVal callback As FinishedDelegate)
        Start(New BackgroundMethod(AddressOf DoRetrieveMessageList), Nothing, callback)
    End Sub       'StartRetrieveMessageList


    ''' <summary>
    ''' Starts an asynchronous operation to delete specified messages.
    ''' </summary>
    ''' <param name="uniqueIds">List of message unique ID.</param>
    ''' <param name="callback">Callback method to be called when the operation ends.</param>
    Public Sub StartDeleteMessages(ByVal uniqueIds() As String, ByVal callback As FinishedDelegate)
        Start(New BackgroundMethod(AddressOf DoDeleteMessages), uniqueIds, callback)
    End Sub       'StartDeleteMessages


    ''' <summary>
    ''' Starts an asynchronous operation to retrieve and displays the message.
    ''' </summary>
    ''' <param name="uniqueId">Message unique ID.</param>
    ''' <param name="callback">Callback method to be called when the operation ends.</param>
    Public Sub StartRetrieveMessage(ByVal uniqueId As String, ByVal callback As FinishedDelegate)
        Start(New BackgroundMethod(AddressOf DoRetrieveMessage), uniqueId, callback)
    End Sub       'StartRetrieveMessage


    ''' <summary>
    ''' Starts an asynchronous operation to save the message into the supplied stream.
    ''' </summary>
    ''' <param name="uniqueId">Message unique ID.</param>
    ''' <param name="filePath">Output file.</param>
    ''' <param name="callback">Callback method to be called when the operation ends.</param>
    Public Sub StartSaveMessage(ByVal uniqueId As String, ByVal filePath As String, ByVal callback As FinishedDelegate)
        Start(New BackgroundMethod(AddressOf DoSaveMessage), New Object() {uniqueId, filePath}, callback)
    End Sub       'StartSaveMessage


    ''' <summary>
    ''' Starts an asynchronous operation to check for new mails.
    ''' </summary>
    Public Sub StartCheckForUpdates(ByVal callback As FinishedDelegate)
        Start(New BackgroundMethod(AddressOf DoCheckForUpdates), Nothing, callback)
    End Sub       'StartCheckForUpdates


    ''' <summary>
    ''' Dummy method.
    ''' </summary>
    Private Sub DoConnect()
    End Sub       'DoConnect


    ''' <summary>
    ''' Retrieves the message list.
    ''' </summary>
    Private Sub DoRetrieveMessageList()
        SetStatus(String.Format("Getting message IDs from folder {0}...", _imap.CurrentFolder.Name))

        GetMessageList()

        Dim i As Integer
        For i = _messageList.Count - 1 To 0 Step -1
            ' Because the _aborting field might have been changed from another thread,
            ' and because VB.NET still has no 'Volatile' keyword, we have to use a bit
            ' more complicated code. However, it is a thread-safe equivalent of:
            ' If _aborting <> 0 Then Exit For
            If Interlocked.CompareExchange(_aborting, 0, 0) <> 0 Then Exit For

            Dim message As ImapMessageInfo = _messageList(i)

            If Not IsKnownMessage(message.UniqueId) Then
                SetStatus(String.Format("Getting message ({0}) from folder {1}...", message.UniqueId, _imap.CurrentFolder.Name))
                Try
                    Dim m As ImapMessageInfo = _imap.GetMessageInfo(message.UniqueId, ImapListFields.Envelope)
                    AddMessage(m, False)
                Catch
                    AddMessage(message, True)
                End Try
            End If
        Next i
    End Sub       'DoRetrieveMessageList


    ''' <summary>
    ''' Deletes specified messages.
    ''' </summary>
    Private Sub DoDeleteMessages()
        Dim uniqueIds As String() = CType(_parameter, String())

        GetMessageList()

        Dim i As Integer
        For i = 0 To uniqueIds.Length - 1
            Dim uniqueId As String = uniqueIds(i)
            _imap.DeleteMessage(uniqueId)
            Dim info As ImapMessageInfo = _messageList.Find(uniqueId)
            If Not (info Is Nothing) Then
                _messageList.Remove(info)
            End If
            RemoveMessage(uniqueId)
        Next i

        _imap.Purge()
    End Sub       'DoDeleteMessages

    ''' <summary>
    ''' Tests if message is still on server.
    ''' </summary>
    Private Function IsMessageOnServer(ByVal uniqueId As String) As Boolean
        Dim collection As ImapMessageCollection = _imap.GetMessageList(New ImapMessageSet(uniqueId), ImapListFields.UniqueId)

        If collection.Count = 0 Then
            Dim message As String = String.Format("Requested message ({0}) not found.", uniqueId)
            SetStatus(message)
            ShowMessageBox(message)
            RemoveMessage(uniqueId)
            Return False
        Else
            Return True
        End If
    End Function

    ''' <summary>
    ''' Retrieves and displays the message.
    ''' </summary>
    Private Sub DoRetrieveMessage()
        Dim uniqueId As String = CStr(_parameter)

        GetMessageList()

        ' Test if message is still on server
        If Not IsMessageOnServer(uniqueId) Then Return

        SetStatus(String.Format("Retrieving message ({0}) from folder {1}...", uniqueId, _imap.CurrentFolder.Name))

        Dim buffer As New MemoryStream
        Dim mime As New MimeMessage

        _imap.GetMessage(uniqueId, buffer)
        buffer.Position = 0
        mime.Load(buffer)

        Dim raw As Byte() = buffer.ToArray()
        ShowMessage(mime, raw)
    End Sub       'DoRetrieveMessage


    ''' <summary>
    ''' Saves the message into the supplied stream.
    ''' </summary>
    Private Sub DoSaveMessage()
        Dim uniqueId As String = CStr(CType(_parameter, Object())(0))
        Dim filePath As String = CStr(CType(_parameter, Object())(1))

        GetMessageList()

        ' Test if message is still on server
        If Not IsMessageOnServer(uniqueId) Then Return

        SetStatus(String.Format("Retrieving message ({0}) from folder {1}...", uniqueId, _imap.CurrentFolder.Name))

        Dim extension As String = Path.GetExtension(filePath)
        If String.Equals(extension, ".msg", StringComparison.OrdinalIgnoreCase) Then
            ' get message from IMAP (and parse it)
            Dim message As MailMessage = _imap.GetMailMessage(uniqueId)

            ' save message in MSG format
            message.Save(filePath, MailFormat.OutlookMsg)
        Else
            ' get message from IMAP directly into file in MIME format (without parsing)
            _imap.GetMessage(uniqueId, filePath)
        End If
    End Sub       'DoSaveMessage


    ''' <summary>
    ''' Checks for new mails.
    ''' </summary>
    Private Sub DoCheckForUpdates()
        ' Because the _aborting and _checkForUpdatesEnabled fields might have been changed from another thread,
        ' and because VB.NET still has no 'Volatile' keyword, we have to use a bit
        ' more complicated code. However, it is a thread-safe equivalent of:
        ' While Not _aborting AndAlso _checkForUpdatesEnabled
        While Interlocked.CompareExchange(_aborting, 0, 0) = 0 AndAlso _
          Interlocked.CompareExchange(_checkForUpdatesEnabled, 0, 0) <> 0
            _newMessage = 0
            SetStatus("Checking for new mails...")
            _imap.CheckForUpdates(5000)
            If Interlocked.CompareExchange(_newMessage, 0, 0) <> 0 Then
                _messageList = Nothing
                DoRetrieveMessageList()
            End If
        End While
    End Sub       'DoCheckForUpdates


    Private Sub imap_Notification(ByVal sender As Object, ByVal e As ImapNotificationEventArgs)
        _newMessage = 1
        _imap.Abort()
    End Sub       'imap_Notification
End Class   'Worker 'Rebex.Samples.ImapBrowser