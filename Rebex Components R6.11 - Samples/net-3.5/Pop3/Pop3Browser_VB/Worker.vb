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
    Private _pop As Pop3
    Private _knownMessages As Hashtable
    Private _owner As RemoteMailbox
    Private _messageList As Pop3MessageCollection
    Private _aborting As Integer

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
        _aborting = 0

        _pop = New Pop3
        AddHandler _pop.ValidatingCertificate, AddressOf pop_ValidatingCertificate
    End Sub 'New

    Private Sub pop_ValidatingCertificate(sender As Object, e As SslCertificateValidationEventArgs)
        _owner.SafeInvoke(New ValidatingCertificateDelegate(AddressOf Verifier.ValidatingCertificate), New Object() {sender, e})
    End Sub


    ''' <summary>
    ''' Aborts the current POP3 operation.
    ''' </summary>
    Public Sub Abort()
        _pop.Abort()
        _aborting = 1
    End Sub 'Abort


    ''' <summary>
    ''' Gets the message list from the POP3 connection.
    ''' </summary>
    Private Sub GetMessageList()
        If _messageList Is Nothing Then
            _messageList = _pop.GetMessageList()
        End If
    End Sub 'GetMessageList

    ' Main form methods delegates.
    Delegate Sub MessageDelegate(ByVal message As String)

    Delegate Sub AddMessageDelegate(ByVal message As Pop3MessageInfo, ByVal [error] As Boolean)

    Delegate Sub ShowMessageDelegate(ByVal message As MimeMessage, ByVal raw() As Byte)


    ''' <summary>
    ''' Invokes the main form's method to update the status bar text.
    ''' </summary>
    ''' <param name="message">Status bar text.</param>
    Private Sub SetStatus(ByVal message As String)
        _owner.SafeInvoke(New MessageDelegate(AddressOf _owner.SetStatus), New Object() {message})
    End Sub 'SetStatus


    ''' <summary>
    ''' Invokes the main form's method to add a message info to the list.
    ''' </summary>
    ''' <param name="message">Message info.</param>
    ''' <param name="error">True if message was unparsable.</param>
    Private Sub AddMessage(ByVal message As Pop3MessageInfo, ByVal [error] As Boolean)
        _owner.SafeInvoke(New AddMessageDelegate(AddressOf _owner.AddMessage), New Object() {message, [error]})

        SyncLock _knownMessages.SyncRoot
            _knownMessages.Add(message.UniqueId, message.UniqueId)
        End SyncLock
    End Sub 'AddMessage


    ''' <summary>
    ''' Invokes the main form's method to remove a message from the list.
    ''' </summary>
    ''' <param name="uniqueId">Message unique ID.</param>
    Private Sub RemoveMessage(ByVal uniqueId As String)
        _owner.SafeInvoke(New MessageDelegate(AddressOf _owner.RemoveMessage), New Object() {uniqueId})
        ForgetMessage(uniqueId)
    End Sub 'RemoveMessage


    ''' <summary>
    ''' Invokes the main form's method to display a message.
    ''' </summary>
    ''' <param name="message">Mime message.</param>
    ''' <param name="raw">Raw mssage data.</param>
    Private Sub ShowMessage(ByVal message As MimeMessage, ByVal raw() As Byte)
        _owner.SafeInvoke(New ShowMessageDelegate(AddressOf _owner.ShowMessage), New Object() {message, raw})
    End Sub 'ShowMessage


    ''' <summary>
    ''' Determines whether the message unique ID is known.
    ''' </summary>
    ''' <param name="uniqueId">Unique ID.</param>
    Public Function IsKnownMessage(ByVal uniqueId As String) As Boolean
        SyncLock _knownMessages.SyncRoot
            Return _knownMessages.ContainsKey(uniqueId)
        End SyncLock
    End Function 'IsKnownMessage


    ''' <summary>
    ''' Forgets the specified message unique IDs.
    ''' </summary>
    ''' <param name="uniqueId">Unique ID.</param>
    Public Sub ForgetMessage(ByVal uniqueId As String)
        SyncLock _knownMessages.SyncRoot
            _knownMessages.Remove(uniqueId)
        End SyncLock
    End Sub 'ForgetMessage


    ''' <summary>
    ''' Forgets all known message unique IDs.
    ''' </summary>
    Public Sub ForgetMessages()
        SyncLock _knownMessages.SyncRoot
            _knownMessages.Clear()
        End SyncLock
    End Sub 'ForgetMessages


    ''' <summary>
    ''' Callback method delegete.
    ''' </summary>
    Delegate Sub FinishedDelegate(ByVal [error] As Exception)

    ' Fields required by the background method.
    Private _parameter As Object = Nothing
    Private _method As BackgroundMethod = Nothing
    Private _thread As Thread = Nothing
    Private _callback As FinishedDelegate = Nothing


    ''' <summary>
    ''' Background method delegate.
    ''' </summary>
    Delegate Sub BackgroundMethod()

    ''' <summary>
    ''' Gets the value indicating whether a background operation is running.
    ''' </summary>

    Public ReadOnly Property Running() As Boolean
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
        ' Initialize fields required by the background method.
        _aborting = 0
        _method = method
        _parameter = parameter
        _callback = callback

        ' Start the background method in a background thread.
        _thread = New Thread(New ThreadStart(AddressOf DoMethod))
        _thread.Start()
    End Sub 'Start


    ''' <summary>
    ''' Common background operation stub. Connects, logs in and executes the desired background operation.
    ''' </summary>
    Private Sub DoMethod()
        Dim connecting As Boolean = False
        Dim [error] As Exception
        Try
            If _pop.State = Pop3State.Disconnected Then
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

                _pop.Settings.SslAllowedVersions = CType(_config.GetValue("protocol", GetType(TlsVersion)), TlsVersion)
                _pop.Settings.SslAllowedSuites = CType(_config.GetValue("suite", GetType(TlsCipherSuite)), TlsCipherSuite)

                SetStatus(String.Format("Connecting to {0}...", server))

                ' Forget the old message list.
                _messageList = Nothing

                ' Connect to the server with the specified security.
                _pop.Connect(server, port, security)

                If singleSignOn Then
                    _pop.Login(Pop3Authentication.Auto)
                Else
                    _pop.Login(userName, password)
                End If

                connecting = False
            End If

            ' Run desired background operation.
            _method()

            [error] = Nothing
        Catch x As Exception

            Dim px As Pop3Exception = TryCast(x, Pop3Exception)
            If Not (px Is Nothing) AndAlso px.Status = Pop3ExceptionStatus.OperationAborted Then
                ' If the operation is a result of an abort, don't report it as error.
                [error] = Nothing
                _pop.Disconnect()
            Else
                [error] = x

                ' If this is a failed connection attempt, disconnect.
                If connecting Then
                    _pop.Disconnect()
                End If
            End If
        Finally
            _thread = Nothing
        End Try

        ' Resets status bar and informs the main form that the operation finished.
        SetStatus("")
        If Not (_callback Is Nothing) Then
            _owner.SafeInvoke(_callback, New Object() {[error]})
        End If
    End Sub 'DoMethod

    ''' <summary>
    ''' Starts an asynchronous operation to retrieve the message list.
    ''' </summary>
    ''' <param name="callback">Callback method to be called when the operation ends.</param>
    Public Sub StartRetrieveMessageList(ByVal callback As FinishedDelegate)
        Start(New BackgroundMethod(AddressOf DoRetrieveMessageList), Nothing, callback)
    End Sub 'StartRetrieveMessageList


    ''' <summary>
    ''' Starts an asynchronous operation to delete specified messages.
    ''' </summary>
    ''' <param name="uniqueIds">List of message unique ID.</param>
    ''' <param name="callback">Callback method to be called when the operation ends.</param>
    Public Sub StartDeleteMessages(ByVal uniqueIds() As String, ByVal callback As FinishedDelegate)
        Start(New BackgroundMethod(AddressOf DoDeleteMessages), uniqueIds, callback)
    End Sub 'StartDeleteMessages


    ''' <summary>
    ''' Starts an asynchronous operation to retrieve and displays the message.
    ''' </summary>
    ''' <param name="uniqueId">Message unique ID.</param>
    ''' <param name="callback">Callback method to be called when the operation ends.</param>
    Public Sub StartRetrieveMessage(ByVal uniqueId As String, ByVal callback As FinishedDelegate)
        Start(New BackgroundMethod(AddressOf DoRetrieveMessage), uniqueId, callback)
    End Sub 'StartRetrieveMessage


    ''' <summary>
    ''' Starts an asynchronous operation to save the message into the supplied stream.
    ''' </summary>
    ''' <param name="uniqueId">Message unique ID.</param>
    ''' <param name="filePath">Output file.</param>
    ''' <param name="callback">Callback method to be called when the operation ends.</param>
    Public Sub StartSaveMessage(ByVal uniqueId As String, ByVal filePath As String, ByVal callback As FinishedDelegate)
        Start(New BackgroundMethod(AddressOf DoSaveMessage), New Object() {uniqueId, filePath}, callback)
    End Sub 'StartSaveMessage


    ''' <summary>
    ''' Retrieves the message list.
    ''' </summary>
    Private Sub DoRetrieveMessageList()
        SetStatus("Getting message IDs...")

        GetMessageList()

        Dim i As Integer
        For i = _messageList.Count - 1 To 0 Step -1

            ' Because the _aborting field might have been changed from another thread,
            ' and because VB.NET still has no 'Volatile' keyword, we have to use a bit
            ' more complicated code. However, it is a thread-safe equivalent of:
            ' If _aborting <> 0 Then Exit For
            If Interlocked.CompareExchange(_aborting, 0, 0) <> 0 Then Exit For

            Dim message As Pop3MessageInfo = _messageList(i)

            If Not IsKnownMessage(message.UniqueId) Then
                SetStatus(String.Format("Getting message {0}...", message.UniqueId))
                Try
                    Dim m As Pop3MessageInfo = _pop.GetMessageInfo(message.SequenceNumber, Pop3ListFields.FullHeaders)
                    AddMessage(m, False)
                Catch
                    AddMessage(message, True)
                End Try
            End If

        Next i

    End Sub 'DoRetrieveMessageList


    ''' <summary>
    ''' Deletes specified messages.
    ''' </summary>
    Private Sub DoDeleteMessages()
        Dim uniqueIds As String() = CType(_parameter, String())

        GetMessageList()

        Dim i As Integer
        For i = 0 To uniqueIds.Length - 1
            Dim info As Pop3MessageInfo = _messageList.Find(uniqueIds(i))
            _pop.Delete(info.SequenceNumber)
            _messageList.Remove(info)
            RemoveMessage(uniqueIds(i))
        Next i
        _pop.Disconnect()
    End Sub 'DoDeleteMessages


    ''' <summary>
    ''' Retrieves and displays the message.
    ''' </summary>
    Private Sub DoRetrieveMessage()
        Dim uniqueId As String = CStr(_parameter)
        SetStatus(String.Format("Retrieving message {0}...", uniqueId))

        GetMessageList()

        Dim sequenceNumber As Integer = _messageList.Find(uniqueId).SequenceNumber
        Dim buffer As New MemoryStream
        _pop.GetMessage(sequenceNumber, buffer)
        buffer.Position = 0
        Dim raw As Byte() = buffer.ToArray()
        Dim mime As New MimeMessage
        mime.Load(buffer)
        ShowMessage(mime, raw)
    End Sub 'DoRetrieveMessage


    ''' <summary>
    ''' Saves the message into the supplied stream.
    ''' </summary>
    Private Sub DoSaveMessage()
        Dim uniqueId As String = CStr(CType(_parameter, Object())(0))
        Dim filePath As String = CStr(CType(_parameter, Object())(1))

        SetStatus(String.Format("Retrieving message {0}...", uniqueId))
        GetMessageList()
        Dim sequenceNumber As Integer = _messageList.Find(uniqueId).SequenceNumber

        Dim extension As String = Path.GetExtension(filePath)
        If String.Equals(extension, ".msg", StringComparison.OrdinalIgnoreCase) Then
            ' get message from POP3 (and parse it)
            Dim message As MailMessage = _pop.GetMailMessage(sequenceNumber)

            ' save message in MSG format
            message.Save(filePath, MailFormat.OutlookMsg)
        Else
            ' get message from POP3 directly into file in MIME format (without parsing)
            _pop.GetMessage(sequenceNumber, filePath)
        End If
    End Sub 'DoSaveMessage
End Class 'Worker

