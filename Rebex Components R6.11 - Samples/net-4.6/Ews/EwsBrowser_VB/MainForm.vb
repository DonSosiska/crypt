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

Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports System.Windows.Forms

Imports Rebex.Mail
Imports Rebex.Mime
Imports Rebex.Net
Imports Rebex.Security.Certificates

''' <summary>
''' Exchange Browser sample.
''' </summary>
Partial Public Class MainForm
    Inherits Form

    Private Const TitleFormat As String = "Exchange Browser - {0}"

    ' EWS client object
    Private _ews As Ews

    ' currently selected folder
    Private _currentFolder As EwsFolderTree.FolderInfo

    ' cache for downloaded EWS items
    Private ReadOnly _itemCache As Dictionary(Of Integer, EwsListViewItem)

    ' list view item with additional fields for working with Ews items
    Private Class EwsListViewItem
        Inherits ListViewItem

        Public ReadOnly Property Id() As EwsItemId
            Get
                Return m_Id
            End Get
        End Property
        Private m_Id As EwsItemId

        Public ReadOnly Property ItemType() As EwsItemType
            Get
                Return m_ItemType
            End Get
        End Property
        Private m_ItemType As EwsItemType

        Public Sub New(id As EwsItemId, type As EwsItemType, ParamArray columns As String())
            MyBase.New(columns)
            m_Id = id
            m_ItemType = type
        End Sub

        Public Shared Function GetLoadingItem() As EwsListViewItem
            Return New EwsListViewItem(Nothing, EwsItemType.Unknown, "Loading...", "", "")
        End Function
    End Class

    ''' <summary>
    ''' Initializes new instance of the form.
    ''' </summary>
    Public Sub New()
        InitializeComponent()

        _itemCache = New Dictionary(Of Integer, EwsListViewItem)()

        AddHandler folderTree.ErrorOccured, AddressOf folderTree_ErrorOccured

        SetConnectMenu(False)
    End Sub

    ''' <summary>
    ''' Handles 'Shown' (form is shown for the first time) event.
    ''' </summary>
    Private Sub MainForm_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown

        Connect()
    End Sub

    ''' <summary>
    ''' Handles clicking on 'Connect' menu item.
    ''' </summary>
    Private Sub connectToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles connectToolStripMenuItem.Click
        Connect()
    End Sub

    ''' <summary>
    ''' Handles clicking on 'Disconnect' menu item.
    ''' </summary>
    Private Sub disconnectToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles disconnectToolStripMenuItem.Click
        Disconnect()
    End Sub

    ''' <summary>
    ''' Handles clicking on 'Exit' menu item.
    ''' </summary>
    Private Sub exitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles exitToolStripMenuItem.Click
        Close()
    End Sub

    ''' <summary>
    ''' Handles form closing event.
    ''' </summary>
    Private Sub MainForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        Disconnect()
    End Sub

    ''' <summary>
    ''' Handles an item requests of the virtual item list view.
    ''' </summary>
    Private Sub itemListView_RetrieveVirtualItem(sender As Object, e As RetrieveVirtualItemEventArgs) Handles itemListView.RetrieveVirtualItem
        Dim item As EwsListViewItem = Nothing
        If _itemCache.TryGetValue(e.ItemIndex, item) Then
            e.Item = item
        Else
            e.Item = EwsListViewItem.GetLoadingItem()
            LoadItems(e.ItemIndex)
        End If
    End Sub

    ''' <summary>
    ''' Handles double-clicking an item in item list view.
    ''' </summary>
    Private Sub itemListView_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles itemListView.MouseDoubleClick
        Dim item As EwsListViewItem = GetSelectedItem()
        If item Is Nothing OrElse item.Id Is Nothing Then
            Return
        End If

        ShowItem(item.Id, item.ItemType = EwsItemType.Message)
    End Sub

    ''' <summary>
    ''' Handles clicking on 'View' context menu.
    ''' </summary>
    Private Sub viewMeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles viewMeToolStripMenuItem.Click
        Dim item As EwsListViewItem = GetSelectedItem()
        If item Is Nothing OrElse item.Id Is Nothing Then
            Return
        End If

        ShowItem(item.Id, item.ItemType = EwsItemType.Message)
    End Sub

    ''' <summary>
    ''' Handles clicking on 'Delete' context menu.
    ''' </summary>
    Private Sub deleteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles deleteToolStripMenuItem.Click
        Dim item As EwsListViewItem = GetSelectedItem()
        If item Is Nothing OrElse item.Id Is Nothing Then
            Return
        End If

        Dim result As DialogResult = MessageBox.Show("Are you sure you want to delete selected item?", "Delete prompt", MessageBoxButtons.YesNo)
        If result <> DialogResult.Yes Then
            Return
        End If

        DeleteItem(item.Id)
    End Sub

    ''' <summary>
    ''' Handles clicking on 'Save As' context menu.
    ''' </summary>
    Private Sub saveAsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles saveAsToolStripMenuItem.Click
        Dim item As EwsListViewItem = GetSelectedItem()
        If item Is Nothing OrElse item.Id Is Nothing Then
            Return
        End If

        DownloadItem(item.Id, item.ItemType = EwsItemType.Message)
    End Sub

    ''' <summary>
    ''' Handles changes in selection of folder tree.
    ''' </summary>
    Private Sub folderTree_SelectionChanged(sender As Object, e As EventArgs) Handles folderTree.SelectionChanged
        If _currentFolder Is folderTree.SelectedFolder Then
            Return
        End If

        _currentFolder = folderTree.SelectedFolder

        Me.Text = String.Format(TitleFormat, _currentFolder.Path)

        _itemCache.Clear()
        itemListView.VirtualListSize = 0
        itemListView.VirtualListSize = _currentFolder.ItemsTotal
    End Sub

    ''' <summary>
    ''' Handles errors in folder tree.
    ''' </summary>
    Private Sub folderTree_ErrorOccured(sender As Object, e As ErrorOccuredEventArgs)
        ReportError(e.Error)
        e.Handled = True
    End Sub

    ''' <summary>
    ''' Connects and log in client based on the <see cref="ConectionForm"/> values.
    ''' After successful login initializes folder tree.
    ''' </summary>
    Private Async Sub Connect()
        Try
            ' load connection settings
            Dim data As New ConnectionFormData()
            data.LoadConfig()

            ' show connection dialog
            Dim connection As New ConnectionForm(data)
            connection.ShowDialog()

            ' not confirmed?
            If connection.DialogResult <> DialogResult.OK Then
                Return
            End If

            ' save confirmed connection settings
            data = connection.Data
            data.SaveConfig()

            ' create EWS object
            _ews = New Ews()

            ' proxy
            If data.ProxyType <> ProxyType.None Then
                _ews.Proxy = New Proxy(data.ProxyType, data.ProxyHost, data.ProxyPort)
                If data.ProxyUser.Length > 0 Then
                    _ews.Proxy.AuthenticationMethod = ProxyAuthentication.Basic
                    _ews.Proxy.Credentials = New System.Net.NetworkCredential(data.ProxyUser, data.ProxyPassword)
                End If
            End If

            ' security
            Dim isSecure As Boolean = (data.Protocol = ProtocolMode.HTTPS)
            If isSecure Then
                ' server certificate handler
                Select Case data.ServerCertificateVerifyingMode
                    Case CertificateVerifyingMode.AcceptAnyCertificate
                        _ews.Settings.SslAcceptAllCertificates = True
                        Exit Select

                    Case CertificateVerifyingMode.LocalyStoredThumbprint
                        AddHandler _ews.ValidatingCertificate,
                            Sub(s, e)
                                Dim verifier As New ThumbprintVerifier(data.ServerCertificateThumbprint)
                                verifier.ValidatingCertificate(s, e)

                                ' save accepted thumbprint if differs from the stored one
                                If verifier.IsAccepted AndAlso data.ServerCertificateThumbprint <> verifier.Thumbprint Then
                                    data.ServerCertificateThumbprint = verifier.Thumbprint
                                    data.SaveConfig()
                                End If
                            End Sub
                        Exit Select
                    Case Else

                        AddHandler _ews.ValidatingCertificate, AddressOf Verifier.ValidatingCertificate
                        Exit Select
                End Select


                ' client certificate handler
                If data.ClientCertificate IsNot Nothing Then
                    _ews.Settings.SslClientCertificateRequestHandler = CertificateRequestHandler.CreateRequestHandler(data.ClientCertificate)
                ElseIf Not String.IsNullOrEmpty(data.ClientCertificateFilename) Then
                    ' certificate not loaded yet, ask for password
                    Dim pp As New PassphraseDialog()
                    If pp.ShowDialog() = DialogResult.OK Then
                        Dim chain As CertificateChain = CertificateChain.LoadPfx(data.ClientCertificateFilename, pp.Passphrase)
                        _ews.Settings.SslClientCertificateRequestHandler = CertificateRequestHandler.CreateRequestHandler(chain)
                    Else
                        Throw New ApplicationException("Password wasn't entered.")
                    End If
                Else
                    _ews.Settings.SslClientCertificateRequestHandler = New RequestHandler()
                End If

                _ews.Settings.SslAllowedSuites = data.AllowedSuite
                _ews.Settings.SslAllowedVersions = data.TlsProtocol
            End If

            SetConnectMenu(True)
            SetWorking(True)
            Try
                ' connect
                Await _ews.ConnectAsync(data.ServerHost, data.ServerPort, If(isSecure, SslMode.Implicit, SslMode.None))

                ' login
                If Not _ews.IsAuthenticated Then
                    If data.UseSingleSignOn Then
                        Await _ews.LoginAsync(EwsAuthentication.Auto)
                    Else
                        Await _ews.LoginAsync(data.ServerUser, data.ServerPassword)
                    End If
                End If

                ' fill folder tree
                Dim folderId As EwsFolderId
                If String.IsNullOrEmpty(data.Mailbox) Then
                    folderId = EwsFolderId.Root
                Else
                    folderId = New EwsFolderId(EwsSpecialFolder.Root, data.Mailbox)
                End If

                ' bind folder tree
                Await folderTree.BindAsync(_ews, folderId)
            Finally
                SetWorking(False)
            End Try
        Catch ex As Exception
            ReportError(ex)
            Disconnect()
        End Try
    End Sub

    ''' <summary>
    ''' Disconnects the client object and clears the form.
    ''' </summary>
    Private Sub Disconnect()
        If _ews IsNot Nothing Then
            ' stop and dispose EWS client object
            _ews.Dispose()
            _ews = Nothing

            ' clear controls
            _itemCache.Clear()
            folderTree.Clear()
            itemListView.VirtualListSize = 0
        End If

        ' reset connect menu
        SetConnectMenu(False)
    End Sub

    ''' <summary>
    ''' Loads items around given index.
    ''' </summary>
    ''' <param name="offset">Index of the item, which causes the necessity of loading items.</param>
    Private Async Sub LoadItems(offset As Integer)
        ' quit if busy
        If _ews.IsBusy Then
            Return
        End If

        SetWorking(True)
        Try
            ' compute indices for page view
            Dim pageSize As Integer = 20
            Dim [start] As Integer = (offset \ pageSize) * pageSize
            Dim [end] As Integer = [start] + pageSize
            If [start] > 0 AndAlso Not _itemCache.ContainsKey(start - pageSize) Then
                [start] -= pageSize
            End If
            If Not _itemCache.ContainsKey([end] + pageSize) Then
                [end] += pageSize
            End If

            ' get list of requested messages
            Dim pageView As EwsPageView = EwsPageView.CreateIndexed([start], [end] - [start])
            Dim list As EwsMessageCollection = Await _ews.GetMessageListAsync(_currentFolder.Id, EwsItemFields.Envelope, pageView)

            ' update total items count
            Dim total As Integer = list.PageResult.ItemsTotal
            If itemListView.VirtualListSize <> total Then
                _itemCache.Clear()
                itemListView.VirtualListSize = 0
                itemListView.VirtualListSize = total
            End If

            ' fill cache
            For i As Integer = 0 To list.Count - 1
                Dim info As EwsMessageInfo = list(i)
                Dim item As New EwsListViewItem(info.Id, info.ItemType, Convert.ToString(info.From), info.Subject, Convert.ToString(info.ReceivedDate))
                _itemCache(i + start) = item
            Next

            ' redraw content 
            itemListView.Invalidate()
        Catch ex As Exception
            ReportError(ex)
        Finally
            SetWorking(False)
        End Try
    End Sub

    ''' <summary>
    ''' Shows a form with detailed message info.
    ''' </summary>
    Private Async Sub ShowItem(id As EwsItemId, hasMime As Boolean)
        ' quit if busy
        If _ews.IsBusy Then
            Return
        End If

        SetWorking(True)
        Try
            Dim buffer As New MemoryStream()
            If hasMime Then
                ' download message into memory
                Await _ews.GetMessageAsync(id, buffer)

                ' parse downloaded MIME data
                buffer.Position = 0
                Dim message As New MimeMessage()
                message.Load(buffer)

                ' show parsed message
                Dim raw As Byte() = buffer.ToArray()
                Dim view As New MessageView(message, raw)
                view.Show()
            Else
                ' download item into memory
                Await _ews.GetItemAsync(id, buffer, EwsItemFormat.Xml)

                ' convert raw data to string
                Dim content As String = Encoding.UTF8.GetString(buffer.ToArray())

                ' show item
                Dim view As New StringView("Item view", content)
                view.Show()
            End If
        Catch ex As Exception
            ReportError(ex)
        Finally
            SetWorking(False)
        End Try
    End Sub

    ''' <summary>
    ''' Deletes specified item.
    ''' </summary>
    Private Async Sub DeleteItem(id As EwsItemId)
        ' quit if busy
        If _ews.IsBusy Then
            Return
        End If

        SetWorking(True)
        Try
            ' delete the item
            Await _ews.DeleteItemAsync(id)

            ' clear Cache and ListView to force items to be reloaded
            Dim size As Integer = itemListView.VirtualListSize
            _itemCache.Clear()
            itemListView.VirtualListSize = 0
            itemListView.VirtualListSize = Math.Max(0, size - 1)
        Catch ex As Exception
            ReportError(ex)
        Finally
            SetWorking(False)
        End Try
    End Sub

    ''' <summary>
    ''' Downloads specified item.
    ''' </summary>
    Private Async Sub DownloadItem(id As EwsItemId, hasMime As Boolean)
        ' quit if busy
        If _ews.IsBusy Then
            Return
        End If

        SetWorking(True)
        Try
            ' prepare save dialog
            Dim save As New SaveFileDialog()
            save.AddExtension = True
            If hasMime Then
                save.Filter = "MIME mails (*.eml)|*.eml|SOAP XML (*.xml)|*.xml|Exchange native binary (*.bin)|*.bin|All files (*.*)|*.*"
                save.DefaultExt = "eml"
                save.FileName = Convert.ToString(id.Value) & ".eml"
            Else
                save.Filter = "SOAP XML (*.xml)|*.xml|Exchange native binary (*.bin)|*.bin|All files (*.*)|*.*"
                save.DefaultExt = "xml"
                save.FileName = Convert.ToString(id.Value) & ".xml"
            End If

            Dim res As DialogResult = save.ShowDialog()
            If res <> DialogResult.OK Then
                Return
            End If

            ' determine save format from extension
            Dim extension As String = Path.GetExtension(save.FileName)

            Dim format As EwsItemFormat
            Select Case extension.ToLowerInvariant()
                Case ".bin"
                    format = EwsItemFormat.Native
                    Exit Select
                Case ".xml"
                    format = EwsItemFormat.Xml
                    Exit Select
                Case Else
                    format = If(hasMime, EwsItemFormat.Mime, EwsItemFormat.Xml)
                    Exit Select
            End Select

            ' download message in specified format
            Await _ews.GetItemAsync(id, save.FileName, format)
        Catch ex As Exception
            ReportError(ex)
        Finally
            SetWorking(False)
        End Try
    End Sub

    ''' <summary>
    ''' Gets ID of selected ListView item or null if no item is selected.
    ''' </summary>
    Private Function GetSelectedItem() As EwsListViewItem
        Dim indices As ListView.SelectedIndexCollection = itemListView.SelectedIndices
        If indices Is Nothing OrElse indices.Count <> 1 Then
            Return Nothing
        End If

        Dim index As Integer = indices(0)
        Return TryCast(itemListView.Items(index), EwsListViewItem)
    End Function

    ''' <summary>
    ''' Enables and disables connection menu items.
    ''' </summary>
    Private Sub SetConnectMenu(connected As Boolean)
        connectToolStripMenuItem.Enabled = Not connected
        disconnectToolStripMenuItem.Enabled = connected
        If Not connected Then
            Me.Text = String.Format(TitleFormat, "Disconnected")
        End If
    End Sub

    ''' <summary>
    ''' Performs initial/final UI changes when work begins/ends.
    ''' </summary>
    Private Sub SetWorking(working As Boolean)
        Dim [cursor] As Cursor = If(working, Cursors.WaitCursor, Cursors.Default)

        Me.Cursor = [cursor]
        cursor.Current = [cursor]
        folderTree.Cursor = [cursor]
        itemListView.Cursor = [cursor]
    End Sub

    ''' <summary>
    ''' Reports an error to the user.
    ''' </summary>
    Private Sub ReportError([error] As Exception)
        Dim message As String

        If TypeOf [error] Is ApplicationException Then
            message = String.Format([error].Message)
        ElseIf TypeOf [error] Is EwsException Then
            message = String.Format("Error occurred: {0}", [error].Message)
        Else
            message = [error].ToString()
        End If

        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.[Error])
    End Sub
End Class
