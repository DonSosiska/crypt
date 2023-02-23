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
Imports System.Drawing
Imports System.Collections
Imports System.ComponentModel
Imports System.IO
Imports System.Windows.Forms
Imports System.Text
Imports Rebex.Mime
Imports Rebex.Net
Imports Rebex.Samples
Imports Rebex.Mail

''' <summary>
''' Application's main form.
''' </summary>
Public Class RemoteMailbox
    Inherits System.Windows.Forms.Form

    Public Shared ReadOnly ConfigFilePath As String = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), String.Format("Rebex{0}Secure Mail{0}Pop3Browser.xml", Path.DirectorySeparatorChar))

    ' Background worker object.
    Private _worker As Worker

    ' Application configuration.
    Private _config As Configuration

    ' Allowed TLS/SSL protocol.
    Private _protocol As TlsVersion = SecurityConfig.DefaultTlsVersion

    ' Allowed TLS/SSL cipher suite.
    Private _suite As TlsCipherSuite = TlsCipherSuite.All

    ' ListViewItem sorting object.
    Private _listViewItemSorter As ListViewItemSorter

    Private _lastUserName As String
    Private _lastPassword As String

#Region "Controls"

    Private menuMain As System.Windows.Forms.MenuStrip
    Private menuMailbox As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents menuClear As System.Windows.Forms.ToolStripMenuItem
    Private menuContext As System.Windows.Forms.ContextMenuStrip
    Private WithEvents menuView As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents menuDelete As System.Windows.Forms.ToolStripMenuItem
    Private panel2 As System.Windows.Forms.Panel
    Private panel3 As System.Windows.Forms.Panel
    Private WithEvents btnOpen As System.Windows.Forms.Button
    Private txtPassword As System.Windows.Forms.TextBox
    Private label4 As System.Windows.Forms.Label
    Private txtPort As System.Windows.Forms.TextBox
    Private label3 As System.Windows.Forms.Label
    Private panel4 As System.Windows.Forms.Panel
    Private txtUserName As System.Windows.Forms.TextBox
    Private txtServer As System.Windows.Forms.TextBox
    Private label2 As System.Windows.Forms.Label
    Private label1 As System.Windows.Forms.Label
    Private statusStrip As System.Windows.Forms.StatusStrip
    Private statusLabel As System.Windows.Forms.ToolStripStatusLabel
    Private WithEvents listView As System.Windows.Forms.ListView
    Private columnHeaderFrom As System.Windows.Forms.ColumnHeader
    Private columnHeaderSubject As System.Windows.Forms.ColumnHeader
    Private columnHeaderSize As System.Windows.Forms.ColumnHeader
    Private columnHeaderDate As System.Windows.Forms.ColumnHeader
    Private WithEvents menuItemProperties As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents menuItemSave As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents menuExit As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents label5 As System.Windows.Forms.Label
    Private WithEvents cbSecurity As System.Windows.Forms.ComboBox
    Private WithEvents btnSettings As System.Windows.Forms.Button
    Private WithEvents cbSingleSignOn As System.Windows.Forms.CheckBox

#End Region

    ''' <summary>
    ''' Initializes the RemoteMailbox instance and loads the configuration.
    ''' </summary>
    Public Sub New()
        InitializeComponent()

        CheckForIllegalCrossThreadCalls = True

        _config = New Configuration(ConfigFilePath)
        _worker = New Worker(Me, _config)

        txtServer.Text = _config.GetString("server")
        txtPort.Text = _config.GetInt32("port", Pop3.DefaultPort).ToString()
        txtUserName.Text = _config.GetString("userName")
        txtPassword.Text = _config.GetString("password")
        cbSecurity.SelectedIndex = _config.GetInt32("security", 0)
        _protocol = CType(_config.GetValue("protocol", GetType(TlsVersion)), TlsVersion)
        _suite = CType(_config.GetValue("suite", GetType(TlsCipherSuite)), TlsCipherSuite)
        cbSingleSignOn.Checked = _config.GetBoolean("singleSignOn", False)
        If _protocol = 0 Then
            _protocol = SecurityConfig.DefaultTlsVersion
        End If
        If _suite = 0 Then
            _suite = TlsCipherSuite.All
        End If

        _listViewItemSorter = New ListViewItemSorter
        _listViewItemSorter.SortColumn = Me.columnHeaderDate.Index
        _listViewItemSorter.Sorting = SortOrder.Descending
        listView.ListViewItemSorter = _listViewItemSorter

    End Sub 'New

    ''' <summary>
    ''' Wrapper for Invoke method that doesn't throw an exception after the object has been
    ''' disposed while the calling method was running in a background thread.
    ''' </summary>
    ''' <param name="method"></param>
    ''' <param name="args"></param>
    Public Sub SafeInvoke(ByVal method As [Delegate], ByVal args() As Object)
        Try
            If Not IsDisposed Then Invoke(method, args)
        Catch x As ObjectDisposedException
        End Try
    End Sub

    ''' <summary>
    ''' Updates the status bar text. Called from a background thread.
    ''' </summary>
    Public Sub SetStatus(ByVal message As String)
        statusLabel.Text = IIf(Not (message Is Nothing), message, "")
    End Sub 'SetStatus


    ''' <summary>
    ''' Adds a message info to the list. Called from a background thread.
    ''' </summary>
    ''' <param name="m">Message info.</param>
    ''' <param name="error">True if message was unparsable.</param>
    Public Sub AddMessage(ByVal m As Pop3MessageInfo, ByVal [error] As Boolean)
        If m.HeadersParsed Then
            Dim subject As String = IIf(Not (m.Subject Is Nothing), m.Subject, "n/a")
            Dim [date] As String = "n/a"
            If Not (m.Date Is Nothing) Then [date] = m.Date.LocalTime.ToString("yyyy-MM-dd HH:mm:ss")
            Dim item As New ListViewItem(m.From.ToString())
            item.Tag = m
            item.SubItems.Add(subject)
            item.SubItems.Add(m.Length.ToString())
            item.SubItems.Add([date])
            listView.Items.Add(item)
        Else
            Dim item As New ListViewItem("n/a")
            item.Tag = m
            item.SubItems.Add("n/a")
            item.SubItems.Add(m.Length.ToString())
            item.SubItems.Add("n/a")
            item.ForeColor = Color.Red
            listView.Items.Add(item)
        End If
    End Sub 'AddMessage


    ''' <summary>
    ''' Removes a message from the list. Called from a background thread.
    ''' </summary>
    ''' <param name="uniqueId">Message unique ID.</param>
    Public Sub RemoveMessage(ByVal uniqueID As String)
        Try
            Dim item As ListViewItem
            For Each item In listView.Items
                Dim m As Pop3MessageInfo = CType(item.Tag, Pop3MessageInfo)
                If m.UniqueId = uniqueID Then
                    listView.Items.Remove(item)
                    Return
                End If
            Next item

            Throw New Exception("Message not found.")
        Catch x As Exception
            ReportError(x)
        End Try
    End Sub 'RemoveMessage


    ''' <summary>
    ''' Displays a message. Called from a background thread.
    ''' </summary>
    ''' <param name="message">Mime message.</param>
    ''' <param name="raw">Raw mssage data.</param>
    Public Sub ShowMessage(ByVal message As MimeMessage, ByVal raw() As Byte)
        Try
            SetStatus("Formatting message...")
            Dim v As New MessageView(New MailMessage(message), raw)
            v.Show()
        Catch x As Exception
            ReportError(x)
        End Try
    End Sub 'ShowMessage


    ''' <summary>
    ''' Enables/disables fields which cannot be changed
    ''' while the background update is in progress.
    ''' </summary>
    Private Sub SetSessionConfigEnabled(ByVal state As Boolean)
        txtServer.Enabled = state
        txtPort.Enabled = state
        cbSingleSignOn.Enabled = state

        If Not cbSingleSignOn.Checked Then
            txtUserName.Enabled = state
            txtPassword.Enabled = state
        End If

        cbSecurity.Enabled = state
        btnSettings.Enabled = state
    End Sub 'SetSessionConfigEnabled


    Private Sub SetContextMenuEnabled(ByVal state As Boolean)
        menuView.Enabled = state
        menuDelete.Enabled = state
        menuItemSave.Enabled = state
        menuItemProperties.Enabled = state
    End Sub 'SetContextMenuEnabled


    ''' <summary>
    ''' Re-enables session config fields.
    ''' </summary>
    Private Sub OnFinished(ByVal [error] As Exception)
        If [error] Is Nothing Then
            btnOpen.Text = "&Refresh"
            SetSessionConfigEnabled(False)
        Else
            ReportError([error])
            SetSessionConfigEnabled(True)
        End If
        SetContextMenuEnabled(True)
    End Sub 'OnFinished


    ''' <summary>
    ''' Reports an error to the user.
    ''' </summary>
    ''' <param name="error">Error.</param>
    Private Sub ReportError(ByVal [error] As Exception)
        statusLabel.Text = ""
        If listView.Items.Count = 0 Then
            btnOpen.Text = "&Open"
        Else
            btnOpen.Text = "&Refresh"
        End If

        Dim message As String

        If TypeOf [error] Is Pop3Exception Then
            Dim pop3error As Pop3Exception = CType([error], Pop3Exception)
            If pop3error.Status = Pop3ExceptionStatus.ProtocolError Then
                message = String.Format("Server reported error: {0}", [error].Message)
            Else
                message = String.Format("Error occured: {0}", [error].Message)
            End If
        Else
            message = [error].ToString()
        End If

        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub 'ReportError


    ''' <summary>
    ''' Prepares the form to start a background operation.
    ''' </summary>
    Private Sub PrepareForWorker()
        If txtPort.Text.Trim() = String.Empty Then
            txtPort.Text = Pop3.DefaultPort.ToString()
        End If

        _config.SetValue("server", txtServer.Text)
        _config.SetValue("port", Integer.Parse(txtPort.Text))
        _config.SetValue("userName", txtUserName.Text)
        _config.SetValue("password", txtPassword.Text)
        _config.SetValue("singleSignOn", cbSingleSignOn.Checked)
        _config.SetValue("security", cbSecurity.SelectedIndex)
        _config.SetValue("protocol", _protocol)
        _config.SetValue("suite", _suite)
        _config.Save()

        btnOpen.Text = "&Break"
        SetSessionConfigEnabled(False)
        SetContextMenuEnabled(False)
    End Sub 'PrepareForWorker

#Region "Windows Form Designer generated code"

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(RemoteMailbox))
        Me.menuMain = New System.Windows.Forms.MenuStrip
        Me.menuMailbox = New System.Windows.Forms.ToolStripMenuItem
        Me.menuClear = New System.Windows.Forms.ToolStripMenuItem
        Me.menuExit = New System.Windows.Forms.ToolStripMenuItem
        Me.menuContext = New System.Windows.Forms.ContextMenuStrip
        Me.menuView = New System.Windows.Forms.ToolStripMenuItem
        Me.menuDelete = New System.Windows.Forms.ToolStripMenuItem
        Me.menuItemProperties = New System.Windows.Forms.ToolStripMenuItem
        Me.menuItemSave = New System.Windows.Forms.ToolStripMenuItem
        Me.panel2 = New System.Windows.Forms.Panel
        Me.panel3 = New System.Windows.Forms.Panel
        Me.btnOpen = New System.Windows.Forms.Button
        Me.txtPassword = New System.Windows.Forms.TextBox
        Me.label4 = New System.Windows.Forms.Label
        Me.txtPort = New System.Windows.Forms.TextBox
        Me.label3 = New System.Windows.Forms.Label
        Me.panel4 = New System.Windows.Forms.Panel
        Me.txtUserName = New System.Windows.Forms.TextBox
        Me.txtServer = New System.Windows.Forms.TextBox
        Me.label2 = New System.Windows.Forms.Label
        Me.label1 = New System.Windows.Forms.Label
        Me.statusStrip = New System.Windows.Forms.StatusStrip
        Me.statusLabel = New System.Windows.Forms.ToolStripStatusLabel
        Me.listView = New System.Windows.Forms.ListView
        Me.columnHeaderFrom = New System.Windows.Forms.ColumnHeader
        Me.columnHeaderSubject = New System.Windows.Forms.ColumnHeader
        Me.columnHeaderSize = New System.Windows.Forms.ColumnHeader
        Me.columnHeaderDate = New System.Windows.Forms.ColumnHeader
        Me.label5 = New System.Windows.Forms.Label
        Me.cbSecurity = New System.Windows.Forms.ComboBox
        Me.btnSettings = New System.Windows.Forms.Button
        Me.cbSingleSignOn = New System.Windows.Forms.CheckBox
        Me.panel2.SuspendLayout()
        Me.panel3.SuspendLayout()
        Me.panel4.SuspendLayout()
        Me.statusStrip.SuspendLayout()
        Me.menuMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'menuMain
        '
        Me.menuMain.Items.AddRange(New System.Windows.Forms.ToolStripMenuItem() {Me.menuMailbox})
        '
        'menuMailbox
        '
        Me.menuMailbox.MergeIndex = 0
        Me.menuMailbox.DropDownItems.AddRange(New System.Windows.Forms.ToolStripMenuItem() {Me.menuClear, Me.menuExit})
        Me.menuMailbox.Text = "Mailbox"
        '
        'menuClear
        '
        Me.menuClear.MergeIndex = 0
        Me.menuClear.Text = "&Clear"
        '
        'menuExit
        '
        Me.menuExit.MergeIndex = 1
        Me.menuExit.Text = "E&xit"
        '
        'menuContext
        '
        Me.menuContext.Items.AddRange(New System.Windows.Forms.ToolStripMenuItem() {Me.menuView, Me.menuDelete, Me.menuItemProperties, Me.menuItemSave})
        '
        'menuView
        '
        Me.menuView.MergeIndex = 0
        Me.menuView.Text = "&View..."
        '
        'menuDelete
        '
        Me.menuDelete.MergeIndex = 1
        Me.menuDelete.Text = "&Delete"
        '
        'menuItemProperties
        '
        Me.menuItemProperties.MergeIndex = 2
        Me.menuItemProperties.Text = "&Properties..."
        '
        'menuItemSave
        '
        Me.menuItemSave.MergeIndex = 3
        Me.menuItemSave.Text = "&Save As..."
        '
        'panel2
        '
        Me.panel2.Controls.Add(Me.panel3)
        Me.panel2.Controls.Add(Me.panel4)
        Me.panel2.Dock = System.Windows.Forms.DockStyle.Top
        Me.panel2.Location = New System.Drawing.Point(0, 0)
        Me.panel2.Name = "panel2"
        Me.panel2.Size = New System.Drawing.Size(520, 108)
        Me.panel2.TabIndex = 2
        '
        'panel3
        '
        Me.panel3.Controls.Add(Me.btnSettings)
        Me.panel3.Controls.Add(Me.cbSecurity)
        Me.panel3.Controls.Add(Me.label5)
        Me.panel3.Controls.Add(Me.btnOpen)
        Me.panel3.Controls.Add(Me.txtPassword)
        Me.panel3.Controls.Add(Me.label4)
        Me.panel3.Controls.Add(Me.txtPort)
        Me.panel3.Controls.Add(Me.label3)
        Me.panel3.Dock = System.Windows.Forms.DockStyle.Right
        Me.panel3.Location = New System.Drawing.Point(240, 0)
        Me.panel3.Name = "panel3"
        Me.panel3.Size = New System.Drawing.Size(280, 108)
        Me.panel3.TabIndex = 3
        '
        'btnOpen
        '
        Me.btnOpen.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnOpen.Location = New System.Drawing.Point(192, 16)
        Me.btnOpen.Name = "btnOpen"
        Me.btnOpen.Size = New System.Drawing.Size(80, 23)
        Me.btnOpen.TabIndex = 5
        Me.btnOpen.Text = "Open"
        '
        'txtPassword
        '
        Me.txtPassword.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
           Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtPassword.Location = New System.Drawing.Point(72, 48)
        Me.txtPassword.Name = "txtPassword"
        Me.txtPassword.PasswordChar = Microsoft.VisualBasic.ChrW(42)
        Me.txtPassword.Size = New System.Drawing.Size(200, 20)
        Me.txtPassword.TabIndex = 4
        Me.txtPassword.Text = ""
        '
        'label4
        '
        Me.label4.Location = New System.Drawing.Point(8, 48)
        Me.label4.Name = "label4"
        Me.label4.Size = New System.Drawing.Size(64, 16)
        Me.label4.TabIndex = 7
        Me.label4.Text = "Password:"
        '
        'txtPort
        '
        Me.txtPort.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
           Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtPort.Location = New System.Drawing.Point(72, 16)
        Me.txtPort.Name = "txtPort"
        Me.txtPort.Size = New System.Drawing.Size(112, 20)
        Me.txtPort.TabIndex = 2
        '
        'label3
        '
        Me.label3.BackColor = System.Drawing.SystemColors.Control
        Me.label3.Location = New System.Drawing.Point(8, 16)
        Me.label3.Name = "label3"
        Me.label3.Size = New System.Drawing.Size(64, 16)
        Me.label3.TabIndex = 5
        Me.label3.Text = "Port:"
        '
        'panel4
        '
        Me.panel4.Controls.Add(Me.txtUserName)
        Me.panel4.Controls.Add(Me.txtServer)
        Me.panel4.Controls.Add(Me.label2)
        Me.panel4.Controls.Add(Me.label1)
        Me.panel4.Controls.Add(Me.cbSingleSignOn)
        Me.panel4.Dock = System.Windows.Forms.DockStyle.Left
        Me.panel4.Location = New System.Drawing.Point(0, 0)
        Me.panel4.Name = "panel4"
        Me.panel4.Size = New System.Drawing.Size(240, 108)
        Me.panel4.TabIndex = 2
        '
        'txtUserName
        '
        Me.txtUserName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
           Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtUserName.Location = New System.Drawing.Point(96, 48)
        Me.txtUserName.Name = "txtUserName"
        Me.txtUserName.Size = New System.Drawing.Size(128, 20)
        Me.txtUserName.TabIndex = 3
        Me.txtUserName.Text = ""
        '
        ' cbSingleSignOn
        '
        Me.cbSingleSignOn.Location = New System.Drawing.Point(8, 80)
        Me.cbSingleSignOn.Name = "cbSingleSignOn"
        Me.cbSingleSignOn.Size = New System.Drawing.Size(200, 20)
        Me.cbSingleSignOn.Text = "Single sign-on authentication"
        Me.cbSingleSignOn.Checked = False
        Me.cbSingleSignOn.Enabled = True
        '
        'txtServer
        '
        Me.txtServer.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
           Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtServer.Location = New System.Drawing.Point(96, 16)
        Me.txtServer.Name = "txtServer"
        Me.txtServer.Size = New System.Drawing.Size(128, 20)
        Me.txtServer.TabIndex = 1
        Me.txtServer.Text = ""
        '
        'label2
        '
        Me.label2.Location = New System.Drawing.Point(8, 48)
        Me.label2.Name = "label2"
        Me.label2.Size = New System.Drawing.Size(80, 16)
        Me.label2.TabIndex = 6
        Me.label2.Text = "Username:"
        '
        'label1
        '
        Me.label1.Location = New System.Drawing.Point(8, 16)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(72, 16)
        Me.label1.TabIndex = 4
        Me.label1.Text = "Hostname:"
        '
        'statusStrip
        '
        Me.statusStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.statusLabel})
        Me.statusStrip.Location = New System.Drawing.Point(0, 368)
        Me.statusStrip.Name = "statusStrip"
        Me.statusStrip.Size = New System.Drawing.Size(520, 22)
        Me.statusStrip.TabIndex = 0
        '
        'statusLabel
        '
        Me.statusLabel.Name = "statusLabel"
        Me.statusLabel.Size = New System.Drawing.Size(90, 21)
        '
        'listView
        '
        Me.listView.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.columnHeaderFrom, Me.columnHeaderSubject, Me.columnHeaderSize, Me.columnHeaderDate})
        Me.listView.Dock = System.Windows.Forms.DockStyle.Fill
        Me.listView.FullRowSelect = True
        Me.listView.GridLines = True
        Me.listView.Location = New System.Drawing.Point(0, 108)
        Me.listView.Name = "listView"
        Me.listView.Size = New System.Drawing.Size(520, 260)
        Me.listView.TabIndex = 6
        Me.listView.View = System.Windows.Forms.View.Details
        '
        'columnHeaderFrom
        '
        Me.columnHeaderFrom.Text = "From"
        Me.columnHeaderFrom.Width = 120
        '
        'columnHeaderSubject
        '
        Me.columnHeaderSubject.Text = "Subject"
        Me.columnHeaderSubject.Width = 160
        '
        'columnHeaderSize
        '
        Me.columnHeaderSize.Text = "Size (bytes)"
        Me.columnHeaderSize.Width = 70
        '
        'columnHeaderDate
        '
        Me.columnHeaderDate.Text = "Date"
        Me.columnHeaderDate.Width = 120

        '
        'label5
        '
        Me.label5.Location = New System.Drawing.Point(8, 80)
        Me.label5.Name = "label5"
        Me.label5.Size = New System.Drawing.Size(64, 16)
        Me.label5.TabIndex = 9
        Me.label5.Text = "Security:"
        '
        'cbSecurity
        '
        Me.cbSecurity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbSecurity.Items.AddRange(New Object() {"No security", "Explicit TLS/SSL", "Implicit TLS/SSL"})
        Me.cbSecurity.Location = New System.Drawing.Point(72, 80)
        Me.cbSecurity.MaxDropDownItems = 3
        Me.cbSecurity.Name = "cbSecurity"
        Me.cbSecurity.Size = New System.Drawing.Size(120, 21)
        Me.cbSecurity.TabIndex = 10
        '
        'btnSettings
        '
        Me.btnSettings.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSettings.Location = New System.Drawing.Point(192, 80)
        Me.btnSettings.Name = "btnSettings"
        Me.btnSettings.Size = New System.Drawing.Size(80, 23)
        Me.btnSettings.TabIndex = 11
        Me.btnSettings.Text = "Settings..."
        '
        'RemoteMailbox
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(520, 390)
        Me.Controls.Add(Me.listView)
        Me.Controls.Add(Me.panel2)
        Me.Controls.Add(Me.statusStrip)
        Me.Controls.Add(Me.menuMain)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.menuMain
        Me.Name = "RemoteMailbox"
        Me.Text = "POP3 Mailbox Browser"
        Me.panel2.ResumeLayout(False)
        Me.panel3.ResumeLayout(False)
        Me.panel4.ResumeLayout(False)
        Me.statusStrip.ResumeLayout(False)
        Me.statusStrip.PerformLayout()
        Me.menuMain.ResumeLayout(False)
        Me.menuMain.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub 'InitializeComponent 
#End Region


    ' Cancels the close request if a background operation is running.
    Private Sub RemoteMailbox_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
        e.Cancel = _worker.Running
    End Sub 'RemoteMailbox_Closing


    ' Clears the message list.
    Private Sub menuClear_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuClear.Click
        listView.Items.Clear()
        _worker.ForgetMessages()
    End Sub 'menuClear_Click


    ' Close the application.
    Private Sub menuExit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuExit.Click
        Close()
    End Sub 'menuExit_Click


    ' View message.
    Private Sub menuView_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuView.Click
        Try
            If _worker.Running Then
                Return
            End If
            Dim selCol As listView.SelectedListViewItemCollection = listView.SelectedItems
            If Not (selCol Is Nothing) AndAlso selCol.Count = 1 Then
                Dim m As Pop3MessageInfo = CType(selCol(0).Tag, Pop3MessageInfo)

                PrepareForWorker()
                _worker.StartRetrieveMessage(m.UniqueId, New Worker.FinishedDelegate(AddressOf OnFinished))
            End If
        Catch x As Exception
            ReportError(x)
        End Try
    End Sub 'menuView_Click


    ' Delete message.
    Private Sub menuDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuDelete.Click
        Try
            Dim selCol As listView.SelectedListViewItemCollection = listView.SelectedItems
            If Not (selCol Is Nothing) AndAlso selCol.Count > 0 Then
                Dim uniqueIds(selCol.Count - 1) As String
                Dim i As Integer
                For i = 0 To selCol.Count - 1
                    Dim m As Pop3MessageInfo = CType(selCol(i).Tag, Pop3MessageInfo)
                    uniqueIds(i) = m.UniqueId
                Next i

                PrepareForWorker()
                _worker.StartDeleteMessages(uniqueIds, New Worker.FinishedDelegate(AddressOf OnFinished))
            End If
        Catch x As Exception
            ReportError(x)
        End Try
    End Sub 'menuDelete_Click


    ' Show message properties.
    Private Sub menuItemProperties_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuItemProperties.Click
        Try
            Dim selCol As listView.SelectedListViewItemCollection = listView.SelectedItems
            If Not (selCol Is Nothing) AndAlso selCol.Count = 1 Then
                Dim m As Pop3MessageInfo = CType(selCol(0).Tag, Pop3MessageInfo)

                Dim info As String = String.Format("Unique ID: {0}" & ControlChars.Cr & ControlChars.Lf & ControlChars.Cr & ControlChars.Lf & "Length: {1}", m.UniqueId, m.Length)
                MessageBox.Show(info, "Message Properties")
            End If
        Catch x As Exception
            ReportError(x)
        End Try
    End Sub 'menuItemProperties_Click


    ' Save the message.
    Private Sub menuItemSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuItemSave.Click
        Try
            Dim selCol As listView.SelectedListViewItemCollection = listView.SelectedItems
            If Not (selCol Is Nothing) AndAlso selCol.Count = 1 Then
                Dim m As Pop3MessageInfo = CType(selCol(0).Tag, Pop3MessageInfo)

                PrepareForWorker()
                Dim save As New SaveFileDialog
                save.AddExtension = True
                save.Filter = "MIME mails (*.eml)|*.eml|Outlook mails (*.msg)|*.msg|All files (*.*)|*.*"
                save.DefaultExt = "eml"
                save.FileName = FixFilename(m.UniqueId) & ".eml"
                Dim res As DialogResult = save.ShowDialog()
                If res <> System.Windows.Forms.DialogResult.OK Then
                    Return
                End If
                _worker.StartSaveMessage(m.UniqueId, save.FileName, New Worker.FinishedDelegate(AddressOf OnFinished))
            End If
        Catch x As Exception
            ReportError(x)
        End Try
    End Sub 'menuItemSave_Click

    ''' <summary>
    ''' Creates a valid filename from string parameter.
    ''' </summary>
    ''' <param name="originalFilename">String to be converted - e.g. mail subject or unique ID.</param>
    ''' <returns>A valid filename based on the supplied string.</returns>
    Private Shared Function FixFilename(ByVal originalFilename As String) As String
        ' Characters allowed in the filename
        Dim allowed As String = " .-0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"

        ' Replace invalid charactes with its hex representation
        Dim sb As New StringBuilder
        Dim i As Integer
        Dim ch As Char() = originalFilename.ToCharArray()

        For i = 0 To originalFilename.Length - 1
            If allowed.IndexOf(ch(i)) < 0 Then
                sb.AppendFormat("_{0:X2}", CInt(Val(ch(i))))
            Else
                sb.Append(ch(i))
            End If
        Next i
        Return sb.ToString()
    End Function 'FixFilename

    ' List messages or abort operation
    Private Sub btnOpen_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnOpen.Click
        Try
            If Not _worker.Running Then
                PrepareForWorker()
                _worker.StartRetrieveMessageList(New Worker.FinishedDelegate(AddressOf OnFinished))
            Else
                _worker.Abort()
            End If
        Catch x As Exception
            ReportError(x)
        End Try
    End Sub 'btnOpen_Click

    ' TLS/SSL settings
    Private Sub btnSettings_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSettings.Click
        Dim config As New SecurityConfig

        config.Protocol = _protocol
        config.AllowedSuite = _suite
        If config.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            _protocol = config.Protocol
            _suite = config.AllowedSuite
        End If
    End Sub

    ' Message selected.
    Private Sub listView_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles listView.MouseUp
        If e.Button = System.Windows.Forms.MouseButtons.Right Then
            Dim sel As ListViewItem = listView.GetItemAt(e.X, e.Y)
            If Not (sel Is Nothing) Then
                Dim selCol As listView.SelectedListViewItemCollection = listView.SelectedItems
                If Not (selCol Is Nothing) Then
                    If selCol.Count > 0 Then
                        menuContext.Show(listView, New Point(e.X, e.Y))
                    End If
                End If
            End If
        End If
    End Sub 'listView_MouseUp

    ' Message double-clicked.
    Private Sub listView_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles listView.DoubleClick
        menuView_Click(sender, e)
    End Sub 'listView_DoubleClick

    ' Column header click
    Private Sub listView_ColumnClick(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) Handles listView.ColumnClick
        _listViewItemSorter.SortColumn = e.Column

        If e.Column = Me.columnHeaderSize.Index Then
            _listViewItemSorter.CompareType = ListViewItemSorter.CompareTypes.Integers
        Else
            _listViewItemSorter.CompareType = ListViewItemSorter.CompareTypes.Strings
        End If


        If _listViewItemSorter.Sorting = SortOrder.Descending Then
            _listViewItemSorter.Sorting = SortOrder.Ascending
        Else
            _listViewItemSorter.Sorting = SortOrder.Descending
        End If

        listView.Sort()
    End Sub 'listView_ColumnClick

    ' Single sign on CheckBox changed.
    Private Sub cbSingleSignOn_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cbSingleSignOn.CheckedChanged
        If cbSingleSignOn.Checked Then
            ' turning on single sign
            ' save password and username for later use without single sign on
            _lastUserName = txtUserName.Text
            _lastPassword = txtPassword.Text
            ' hide the username and password from GUI
            txtPassword.Text = String.Empty
            txtUserName.Text = String.Empty
            txtPassword.Enabled = False
            txtUserName.Enabled = False
        Else
            ' disabling single sign on
            ' restore the saved username and password
            txtUserName.Text = _lastUserName
            txtPassword.Text = _lastPassword
            txtPassword.Enabled = True
            txtUserName.Enabled = True
        End If
    End Sub    'cbSingleSignOn_CheckedChanged
End Class 'RemoteMailbox
