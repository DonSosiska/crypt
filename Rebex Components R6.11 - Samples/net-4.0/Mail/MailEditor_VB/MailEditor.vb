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
Imports System.Windows.Forms
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.IO
Imports Rebex.Mail
Imports Rebex.Mime
Imports Rebex.Mime.Headers

''' <summary>
''' MailMessage editor.
''' </summary>
Public Class MailEditor
    Inherits System.Windows.Forms.Form

#Region "Controls"
    Protected mainMenu As System.Windows.Forms.MenuStrip
    Protected menuItemFile As System.Windows.Forms.ToolStripMenuItem
    Protected WithEvents menuItemNewMessage As System.Windows.Forms.ToolStripMenuItem
    Protected WithEvents menuItemSave As System.Windows.Forms.ToolStripMenuItem
    Protected WithEvents menuItemSaveAs As System.Windows.Forms.ToolStripMenuItem
    Protected WithEvents menuItemSaveAttachment As System.Windows.Forms.ToolStripMenuItem
    Protected WithEvents menuItemClose As System.Windows.Forms.ToolStripMenuItem
    Protected menuItemEdit As System.Windows.Forms.ToolStripMenuItem
    Protected WithEvents menuItemCut As System.Windows.Forms.ToolStripMenuItem
    Protected WithEvents menuItemCopy As System.Windows.Forms.ToolStripMenuItem
    Protected WithEvents menuItemPaste As System.Windows.Forms.ToolStripMenuItem
    Protected WithEvents menuItemSelectAll As System.Windows.Forms.ToolStripMenuItem
    Protected menuItemInsert As System.Windows.Forms.ToolStripMenuItem
    Protected WithEvents menuItemFileAttachment As System.Windows.Forms.ToolStripMenuItem
    Protected WithEvents menuItemOpenMessage As System.Windows.Forms.ToolStripMenuItem
    Protected menuItemSeparator As System.Windows.Forms.ToolStripSeparator
    Protected openFileDialogMessage As System.Windows.Forms.OpenFileDialog
    Protected saveFileDialogMessage As System.Windows.Forms.SaveFileDialog
    Protected openFileDialogAttachment As System.Windows.Forms.OpenFileDialog
    Protected saveFileDialogAttachment As System.Windows.Forms.SaveFileDialog
    Protected WithEvents txtFrom As System.Windows.Forms.TextBox
    Protected lblFrom As System.Windows.Forms.Label
    Protected listBoxAttachments As System.Windows.Forms.ListBox
    Protected lblAttach As System.Windows.Forms.Label
    Protected WithEvents txtSubject As System.Windows.Forms.TextBox
    Protected lblSubject As System.Windows.Forms.Label
    Protected WithEvents txtCc As System.Windows.Forms.TextBox
    Protected WithEvents txtTo As System.Windows.Forms.TextBox
    Protected lblCc As System.Windows.Forms.Label
    Protected lblTo As System.Windows.Forms.Label
    Protected contextMenuAttach As System.Windows.Forms.ContextMenuStrip
    Protected WithEvents menuItemDelete As System.Windows.Forms.ToolStripMenuItem
    Protected WithEvents textEditor As Rebex.Samples.MailTextEditor
#End Region

    ' Folder browser dialog.
    Private _folderBrowserDialog As FolderBrowserDialog

    ' Indicates whether the message was saved.
    Private _modified As Boolean = False

    ' Current mail message.
    Private _message As MailMessage = Nothing

    ' Indicates whether the message is currently editable.
    Private _editable As Boolean = True

    ' Message file path.
    Private _path As String

    ''' <summary>
    ''' Enables or disables the editability of the message.
    ''' </summary>
    Public Property Editable() As Boolean
        Get
            Return _editable
        End Get
        Set(ByVal Value As Boolean)
            _editable = Value
            txtTo.ReadOnly = Not Value
            txtCc.ReadOnly = Not Value
            txtFrom.ReadOnly = Not Value
            txtSubject.ReadOnly = Not Value
            textEditor.ReadOnly = Not Value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets a value indicating whether the message is editable.
    ''' </summary>
    Public Property Modified() As Boolean
        Get
            If _message Is Nothing Then
                Return False
            End If
            Return _modified Or textEditor.Modified
        End Get
        Set(ByVal Value As Boolean)
            _modified = Value
            textEditor.Modified = Value
        End Set
    End Property

    ''' <summary>
    ''' Saves the message to its current location.
    ''' </summary>
    Protected Overridable Overloads Sub SaveMessage()
        If _path Is Nothing Then
            _path = SaveAs()
            Return
        End If

        SaveMessage(_path)
    End Sub 'SaveMessage


    ''' <summary>
    ''' Saves message.
    ''' </summary>
    ''' <param name="path">Path to file.</param>
    Protected Overridable Overloads Sub SaveMessage(ByVal path As String)
        Try
            Dim message As MailMessage = ToMessage()
            Dim extension As String = System.IO.Path.GetExtension(path)
            If String.Equals(extension, ".msg", StringComparison.OrdinalIgnoreCase) Then
                message.Save(path, MailFormat.OutlookMsg)
            Else
                message.Save(path, MailFormat.Mime)
            End If
            Modified = False
        Catch x As Exception
            MessageBox.Show(x.ToString())
        End Try
    End Sub 'SaveMessage


    ''' <summary>
    ''' Creates new message.
    ''' </summary>
    Protected Overridable Sub NewMessage()
        If Not CheckModifications() Then
            Return
        End If
        Dim message As New MailMessage
        LoadMessage(message)
        Editable = True
    End Sub 'NewMessage


    ''' <summary>
    ''' Closes the message.
    ''' </summary>
    Protected Overridable Sub CloseMessage()
        If Not CheckModifications() Then
            Return
        End If
        MyBase.Close()
    End Sub 'CloseMessage


    ''' <summary>
    ''' Opens a new message.
    ''' </summary>
    Protected Overridable Sub OpenMessage()
        If Not CheckModifications() Then
            Return
        End If
        If openFileDialogMessage.ShowDialog() <> System.Windows.Forms.DialogResult.OK Then
            Return
        End If
        LoadMessage(openFileDialogMessage.FileName)
    End Sub 'OpenMessage


    ''' <summary>
    ''' Asks the user whether to save or discard modifications, and save the message if desired.
    ''' </summary>
    ''' <returns>True if caller should proceed, false if cancel was selected.</returns>
    Private Function CheckModifications() As Boolean
        If Not Modified Then
            Return True
        End If
        Dim res As DialogResult = MessageBox.Show(Me, "Would you like to save the changes you made to the current message?", "Message Editor", MessageBoxButtons.YesNoCancel)
        If res = System.Windows.Forms.DialogResult.No Then
            Return True
        End If
        If res <> System.Windows.Forms.DialogResult.Yes Then
            Return False
        End If
        If _path Is Nothing Then
            _path = SaveAs()
        Else
            SaveMessage(_path)
        End If
        Return True
    End Function 'CheckModifications


    ''' <summary>
    ''' Saves attachments to a folder.
    ''' </summary>
    Private Sub SaveAttachments()
        If _folderBrowserDialog.ShowDialog() <> System.Windows.Forms.DialogResult.OK Then
            Return
        End If
        Dim mypath As String = _folderBrowserDialog.SelectedPath
        Dim att As Attachment
        For Each att In _message.Attachments
            att.Save(Path.Combine(mypath, att.FileName))
        Next att
    End Sub 'SaveAttachments


    Private Function SaveAs() As String
        If saveFileDialogMessage.ShowDialog() <> System.Windows.Forms.DialogResult.OK Then
            Return Nothing
        End If

        Dim path As String = saveFileDialogMessage.FileName

        SaveMessage(path)

        Return path
    End Function 'SaveAs


    Private Sub RemoveAttachment(ByVal name As String)
        listBoxAttachments.Items.Remove(name)

        Dim i As Integer
        For i = 0 To _message.Attachments.Count - 1
            If _message.Attachments(i).FileName = name Then
                _message.Attachments.RemoveAt(i)
                Exit For
            End If
        Next i
    End Sub 'RemoveAttachment


    ''' <summary>
    ''' Loads a mail message.
    ''' </summary>
    ''' <param name="path">Path to file.</param>
    Public Overloads Sub LoadMessage(ByVal path As String)
        Try
            Dim message As New MailMessage
            _path = path
            message.Load(_path)
            LoadMessage(message)
            Editable = True
        Catch err As Exception
            MessageBox.Show(err.ToString())
        End Try
    End Sub 'LoadMessage


    ''' <summary>
    ''' Creates Mail message from dialog's data.
    ''' </summary>
    ''' <returns>Created mail message.</returns>
    Public Function ToMessage() As MailMessage
        Dim msg As MailMessage = _message

        msg.Subject = txtSubject.Text
        msg.BodyText = textEditor.Text

        Return msg
    End Function 'ToMessage


    ''' <summary>
    ''' Initializes the editor with MailMessage data.
    ''' </summary>
    ''' <param name="message">Mail message.</param>
    Public Overloads Sub LoadMessage(ByVal message As MailMessage)
        _message = message
        txtFrom.Text = _message.From.ToString()
        txtTo.Text = _message.To.ToString()
        txtCc.Text = _message.CC.ToString()
        txtSubject.Text = _message.Subject
        textEditor.Text = _message.BodyText

        listBoxAttachments.Items.Clear()
        Dim i As Integer
        For i = 0 To _message.Attachments.Count - 1
            Dim attachment As attachment = _message.Attachments(i)

            Dim name As String = attachment.FileName
            If name Is Nothing Then
                name = String.Format("att{0}.dat", i)
            End If
            listBoxAttachments.Items.Add(name)
        Next i

        [Text] = message.Subject

        _modified = False
    End Sub 'LoadMessage


    ''' <summary>
    ''' Constructor.
    ''' </summary>
    Public Sub New()
        '
        ' Required for Windows Form Designer support
        '
        InitializeComponent()

        _folderBrowserDialog = New FolderBrowserDialog

        Editable = True
    End Sub 'New


#Region " Windows Form Designer generated code "

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
        Me.components = New System.ComponentModel.Container()
        Me.mainMenu = New System.Windows.Forms.MenuStrip()
        Me.menuItemFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.menuItemNewMessage = New System.Windows.Forms.ToolStripMenuItem()
        Me.menuItemOpenMessage = New System.Windows.Forms.ToolStripMenuItem()
        Me.menuItemSave = New System.Windows.Forms.ToolStripMenuItem()
        Me.menuItemSaveAs = New System.Windows.Forms.ToolStripMenuItem()
        Me.menuItemSaveAttachment = New System.Windows.Forms.ToolStripMenuItem()
        Me.menuItemSeparator = New System.Windows.Forms.ToolStripSeparator()
        Me.menuItemClose = New System.Windows.Forms.ToolStripMenuItem()
        Me.menuItemEdit = New System.Windows.Forms.ToolStripMenuItem()
        Me.menuItemCut = New System.Windows.Forms.ToolStripMenuItem()
        Me.menuItemCopy = New System.Windows.Forms.ToolStripMenuItem()
        Me.menuItemPaste = New System.Windows.Forms.ToolStripMenuItem()
        Me.menuItemSelectAll = New System.Windows.Forms.ToolStripMenuItem()
        Me.menuItemInsert = New System.Windows.Forms.ToolStripMenuItem()
        Me.menuItemFileAttachment = New System.Windows.Forms.ToolStripMenuItem()
        Me.openFileDialogMessage = New System.Windows.Forms.OpenFileDialog()
        Me.saveFileDialogMessage = New System.Windows.Forms.SaveFileDialog()
        Me.openFileDialogAttachment = New System.Windows.Forms.OpenFileDialog()
        Me.saveFileDialogAttachment = New System.Windows.Forms.SaveFileDialog()
        Me.txtFrom = New System.Windows.Forms.TextBox()
        Me.lblFrom = New System.Windows.Forms.Label()
        Me.listBoxAttachments = New System.Windows.Forms.ListBox()
        Me.contextMenuAttach = New System.Windows.Forms.ContextMenuStrip()
        Me.menuItemDelete = New System.Windows.Forms.ToolStripMenuItem()
        Me.lblAttach = New System.Windows.Forms.Label()
        Me.txtSubject = New System.Windows.Forms.TextBox()
        Me.lblSubject = New System.Windows.Forms.Label()
        Me.txtCc = New System.Windows.Forms.TextBox()
        Me.txtTo = New System.Windows.Forms.TextBox()
        Me.lblCc = New System.Windows.Forms.Label()
        Me.lblTo = New System.Windows.Forms.Label()
        Me.textEditor = New Rebex.Samples.MailTextEditor()
        Me.mainMenu.SuspendLayout()
        Me.SuspendLayout()
        '
        'mainMenu
        '
        Me.mainMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.menuItemFile, Me.menuItemEdit, Me.menuItemInsert})
        '
        'menuItemFile
        '
        Me.menuItemFile.MergeIndex = 0
        Me.menuItemFile.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.menuItemNewMessage, Me.menuItemOpenMessage, Me.menuItemSave, Me.menuItemSaveAs, Me.menuItemSaveAttachment, Me.menuItemSeparator, Me.menuItemClose})
        Me.menuItemFile.Text = "&File"
        '
        'menuItemNewMessage
        '
        Me.menuItemNewMessage.MergeIndex = 0
        Me.menuItemNewMessage.Text = "&New message"
        '
        'menuItemOpenMessage
        '
        Me.menuItemOpenMessage.MergeIndex = 1
        Me.menuItemOpenMessage.Text = "Open message..."
        '
        'menuItemSave
        '
        Me.menuItemSave.MergeIndex = 2
        Me.menuItemSave.Text = "S&ave"
        '
        'menuItemSaveAs
        '
        Me.menuItemSaveAs.MergeIndex = 3
        Me.menuItemSaveAs.Text = "Sa&ve as..."
        '
        'menuItemSaveAttachment
        '
        Me.menuItemSaveAttachment.MergeIndex = 4
        Me.menuItemSaveAttachment.Text = "Save a&ttachments..."
        '
        'menuItemSeparator
        '
        Me.menuItemSeparator.MergeIndex = 5
        '
        'menuItemClose
        '
        Me.menuItemClose.MergeIndex = 6
        Me.menuItemClose.Text = "&Close"
        '
        'menuItemEdit
        '
        Me.menuItemEdit.MergeIndex = 1
        Me.menuItemEdit.DropDownItems.AddRange(New System.Windows.Forms.ToolStripMenuItem() {Me.menuItemCut, Me.menuItemCopy, Me.menuItemPaste, Me.menuItemSelectAll})
        Me.menuItemEdit.Text = "&Edit"
        '
        'menuItemCut
        '
        Me.menuItemCut.MergeIndex = 0
        Me.menuItemCut.Text = "&Cut"
        '
        'menuItemCopy
        '
        Me.menuItemCopy.MergeIndex = 1
        Me.menuItemCopy.Text = "C&opy"
        '
        'menuItemPaste
        '
        Me.menuItemPaste.MergeIndex = 2
        Me.menuItemPaste.Text = "&Paste"
        '
        'menuItemSelectAll
        '
        Me.menuItemSelectAll.MergeIndex = 3
        Me.menuItemSelectAll.Text = "&Select all"
        '
        'menuItemInsert
        '
        Me.menuItemInsert.MergeIndex = 2
        Me.menuItemInsert.DropDownItems.AddRange(New System.Windows.Forms.ToolStripMenuItem() {Me.menuItemFileAttachment})
        Me.menuItemInsert.Text = "&Insert"
        '
        'menuItemFileAttachment
        '
        Me.menuItemFileAttachment.MergeIndex = 0
        Me.menuItemFileAttachment.Text = "&Attachment..."
        '
        'openFileDialogMessage
        '
        Me.openFileDialogMessage.Filter = "MIME mails (*.eml)|*.eml|Outlook mails (*.msg)|*.msg|All files (*.*)|*.*"
        '
        'saveFileDialogMessage
        '
        Me.saveFileDialogMessage.Filter = "MIME mails (*.eml)|*.eml|Outlook mails (*.msg)|*.msg|All files (*.*)|*.*"
        '
        'openFileDialogAttachment
        '
        Me.openFileDialogAttachment.Filter = "All files (*.*)|*.*"
        '
        'saveFileDialogAttachment
        '
        Me.saveFileDialogAttachment.Filter = "All files (*.*)|*.*"
        '
        'txtFrom
        '
        Me.txtFrom.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtFrom.Location = New System.Drawing.Point(88, 35)
        Me.txtFrom.Name = "txtFrom"
        Me.txtFrom.ReadOnly = True
        Me.txtFrom.Size = New System.Drawing.Size(664, 20)
        Me.txtFrom.TabIndex = 1
        '
        'lblFrom
        '
        Me.lblFrom.Location = New System.Drawing.Point(0, 35)
        Me.lblFrom.Name = "lblFrom"
        Me.lblFrom.Size = New System.Drawing.Size(80, 16)
        Me.lblFrom.TabIndex = 19
        Me.lblFrom.Text = "From:"
        Me.lblFrom.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'listBoxAttachments
        '
        Me.listBoxAttachments.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.listBoxAttachments.ContextMenuStrip = Me.contextMenuAttach
        Me.listBoxAttachments.Location = New System.Drawing.Point(88, 131)
        Me.listBoxAttachments.MultiColumn = True
        Me.listBoxAttachments.Name = "listBoxAttachments"
        Me.listBoxAttachments.Size = New System.Drawing.Size(664, 56)
        Me.listBoxAttachments.TabIndex = 18
        '
        'contextMenuAttach
        '
        Me.contextMenuAttach.Items.AddRange(New System.Windows.Forms.ToolStripMenuItem() {Me.menuItemDelete})
        Me.contextMenuAttach.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        '
        'menuItemDelete
        '
        Me.menuItemDelete.MergeIndex = 0
        Me.menuItemDelete.Text = "Delete"
        '
        'lblAttach
        '
        Me.lblAttach.Location = New System.Drawing.Point(0, 131)
        Me.lblAttach.Name = "lblAttach"
        Me.lblAttach.Size = New System.Drawing.Size(80, 16)
        Me.lblAttach.TabIndex = 17
        Me.lblAttach.Text = "Attachments:"
        Me.lblAttach.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'txtSubject
        '
        Me.txtSubject.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtSubject.Location = New System.Drawing.Point(88, 107)
        Me.txtSubject.Name = "txtSubject"
        Me.txtSubject.ReadOnly = True
        Me.txtSubject.Size = New System.Drawing.Size(664, 20)
        Me.txtSubject.TabIndex = 16
        '
        'lblSubject
        '
        Me.lblSubject.Location = New System.Drawing.Point(0, 107)
        Me.lblSubject.Name = "lblSubject"
        Me.lblSubject.Size = New System.Drawing.Size(80, 16)
        Me.lblSubject.TabIndex = 15
        Me.lblSubject.Text = "Subject:"
        Me.lblSubject.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'txtCc
        '
        Me.txtCc.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtCc.Location = New System.Drawing.Point(88, 83)
        Me.txtCc.Name = "txtCc"
        Me.txtCc.ReadOnly = True
        Me.txtCc.Size = New System.Drawing.Size(664, 20)
        Me.txtCc.TabIndex = 14
        '
        'txtTo
        '
        Me.txtTo.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtTo.Location = New System.Drawing.Point(88, 59)
        Me.txtTo.Name = "txtTo"
        Me.txtTo.ReadOnly = True
        Me.txtTo.Size = New System.Drawing.Size(664, 20)
        Me.txtTo.TabIndex = 13
        '
        'lblCc
        '
        Me.lblCc.Location = New System.Drawing.Point(0, 83)
        Me.lblCc.Name = "lblCc"
        Me.lblCc.Size = New System.Drawing.Size(80, 16)
        Me.lblCc.TabIndex = 12
        Me.lblCc.Text = "Cc:"
        Me.lblCc.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblTo
        '
        Me.lblTo.Location = New System.Drawing.Point(0, 59)
        Me.lblTo.Name = "lblTo"
        Me.lblTo.Size = New System.Drawing.Size(80, 16)
        Me.lblTo.TabIndex = 11
        Me.lblTo.Text = "To:"
        Me.lblTo.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'textEditor
        '
        Me.textEditor.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.textEditor.BackColor = System.Drawing.SystemColors.Control
        Me.textEditor.Location = New System.Drawing.Point(0, 195)
        Me.textEditor.Modified = False
        Me.textEditor.Name = "textEditor"
        Me.textEditor.Size = New System.Drawing.Size(760, 222)
        Me.textEditor.TabIndex = 21
        '
        'MailEditor
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(760, 417)
        Me.Controls.Add(Me.txtFrom)
        Me.Controls.Add(Me.txtSubject)
        Me.Controls.Add(Me.txtCc)
        Me.Controls.Add(Me.txtTo)
        Me.Controls.Add(Me.lblFrom)
        Me.Controls.Add(Me.listBoxAttachments)
        Me.Controls.Add(Me.lblAttach)
        Me.Controls.Add(Me.lblSubject)
        Me.Controls.Add(Me.lblCc)
        Me.Controls.Add(Me.lblTo)
        Me.Controls.Add(Me.textEditor)
        Me.Controls.Add(Me.mainMenu)
        Me.MainMenuStrip = Me.mainMenu
        Me.Name = "MailEditor"
        Me.Text = "Mail Message Editor"
        Me.mainMenu.ResumeLayout(False)
        Me.mainMenu.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

    ''' <summary>
    ''' Deletes attachment files.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub listBoxAttachments_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
        If e.KeyChar <> "d"c AndAlso e.KeyChar <> "D"c Then
            Return
        End If
        If listBoxAttachments.SelectedItem Is Nothing Then
            Return
        End If
        If MessageBox.Show("Do you want to remove the attachment?", "Remove Attachment", MessageBoxButtons.YesNoCancel) <> System.Windows.Forms.DialogResult.Yes Then
            Return
        End If
        Dim file As String = listBoxAttachments.SelectedItem.ToString()
        RemoveAttachment(file)
    End Sub 'listBoxAttachments_KeyPress


    ''' <summary>
    ''' Deletes attachment.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub menuItemDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuItemDelete.Click
        If listBoxAttachments.SelectedItem Is Nothing Then
            Return
        End If
        Dim file As String = listBoxAttachments.SelectedItem.ToString()
        RemoveAttachment(file)
    End Sub 'menuItemDelete_Click


    ''' <summary>
    ''' Adds new attached file.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub menuItemFileAttachment_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuItemFileAttachment.Click
        If openFileDialogAttachment.ShowDialog() <> System.Windows.Forms.DialogResult.OK Then
            Return
        End If
        Dim att As New Attachment(openFileDialogAttachment.FileName)
        listBoxAttachments.Items.Add(att.FileName)
        _message.Attachments.Add(att)

        _modified = True
    End Sub 'menuItemFileAttachment_Click


    ''' <summary>
    ''' Creates new message.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub menuItemNewMessage_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuItemNewMessage.Click
        NewMessage()
    End Sub 'menuItemNewMessage_Click


    ''' <summary>
    ''' Opens message from file.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub menuItemOpenMessage_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuItemOpenMessage.Click
        OpenMessage()
    End Sub 'menuItemOpenMessage_Click


    ''' <summary>
    ''' Save message to file.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub menuItemSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuItemSave.Click
        SaveMessage()
    End Sub 'menuItemSave_Click


    ''' <summary>
    ''' Saves message to a new file.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub menuItemSaveAs_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuItemSaveAs.Click
        SaveAs()
    End Sub 'menuItemSaveAs_Click


    ''' <summary>
    ''' Save all attachments to folder.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub menuItemSaveAttachments_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuItemSaveAttachment.Click
        SaveAttachments()
    End Sub 'menuItemSaveAttachments_Click


    ''' <summary>
    ''' Closes application.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub menuItemClose_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuItemClose.Click
        CloseMessage()
    End Sub 'menuItemClose_Click


    Private Sub menuItemCut_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuItemCut.Click
        textEditor.Cut()
    End Sub 'menuItemCut_Click


    Private Sub menuItemCopy_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuItemCopy.Click
        textEditor.Copy()
    End Sub 'menuItemCopy_Click


    Private Sub menuItemPaste_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuItemPaste.Click
        textEditor.Paste()
    End Sub 'menuItemPaste_Click


    Private Sub menuItemSelectAll_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles menuItemSelectAll.Click
        textEditor.SelectAll()
    End Sub 'menuItemSelectAll_Click


    Private Sub txtFrom_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtFrom.TextChanged
        Try
            _message.From = New MailAddressCollection(txtFrom.Text)
            txtFrom.BackColor = SystemColors.Window
        Catch x As MimeException
            If x.Status <> MimeExceptionStatus.HeaderParserError Then
                Throw
            End If
            _message.From = New MailAddressCollection
            txtFrom.BackColor = Color.Orange
        End Try
        _modified = True
    End Sub 'txtFrom_TextChanged


    Private Sub txtTo_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtTo.TextChanged
        Try
            _message.To = New MailAddressCollection(txtTo.Text)
            txtTo.BackColor = SystemColors.Window
        Catch x As MimeException
            If x.Status <> MimeExceptionStatus.HeaderParserError Then
                Throw
            End If
            _message.To = New MailAddressCollection
            txtTo.BackColor = Color.Red
        End Try
        _modified = True
    End Sub 'txtTo_TextChanged


    Private Sub txtCc_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCc.TextChanged
        Try
            _message.CC = New MailAddressCollection(txtCc.Text)
            txtCc.BackColor = SystemColors.Window
        Catch x As MimeException
            If x.Status <> MimeExceptionStatus.HeaderParserError Then
                Throw
            End If
            _message.CC = New MailAddressCollection
            txtCc.BackColor = Color.Red
        End Try
        _modified = True
    End Sub 'txtCc_TextChanged


    Private Sub txtSubject_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtSubject.TextChanged
        _modified = True
        [Text] = txtSubject.Text
    End Sub 'txtSubject_TextChanged


    Private Sub textEditor_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles textEditor.KeyPress
        _modified = True
    End Sub 'textEditor_KeyPress

    Private Sub MailEditor_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        _message = New MailMessage
    End Sub
End Class 'MailEditor 
