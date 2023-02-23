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
Imports System.IO
Imports System.Reflection
Imports Rebex.Mime
Imports Rebex.Mime.Headers
Imports Rebex.Security.Cryptography.Pkcs

'''<summary>
'''Shows a tree of MIME entities.
'''</summary>
Public Class MimeExplorer
    Inherits System.Windows.Forms.Form

    'Known MIME node types.
    Private Enum NodeType
        Info
        Leaf
        Folder
        FolderSave
        ApplicationLeaf
        AudioLeaf
        ImageLeaf
        TextLeaf
        TextHtmlLeaf
        MultipartDigestFolder
        MultipartSignedFolder
        EmbeddedMessage
        MultipartAlternativeFolder
        MultipartReportFolder
    End Enum 'NodeType

    Private _filterIndex As Integer
    Private _icons() As Icon

    Private WithEvents viewTree As System.Windows.Forms.TreeView
    Private menuMain As System.Windows.Forms.MenuStrip
    Private itemMain As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents itemExit As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents itemOpen As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents itemDecrypt As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents itemVerify As System.Windows.Forms.ToolStripMenuItem
    Private menuContext As System.Windows.Forms.ContextMenuStrip
    Private WithEvents itemSave As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents itemView As System.Windows.Forms.ToolStripMenuItem


    Private Function LoadIcons() As Icon()
        'Loads the icon list and initializes the icon list for the viewTree control.
        Dim il As New ImageList
        Dim a As [Assembly] = Me.GetType().Assembly
        Dim resources As String() = {"Rebex.Samples.info.ico", "Rebex.Samples.leaf.ico", "Rebex.Samples.folder.ico", "Rebex.Samples.foldersave.ico", "Rebex.Samples.application.ico", "Rebex.Samples.audio.ico", "Rebex.Samples.image.ico", "Rebex.Samples.text.ico", "Rebex.Samples.texthtml.ico", "Rebex.Samples.digest.ico", "Rebex.Samples.signed.ico", "Rebex.Samples.message.ico", "Rebex.Samples.alternative.ico", "Rebex.Samples.report.ico"}

        Dim icons As New ArrayList
        Dim i As Integer
        For i = 0 To resources.Length - 1
            Dim stream As Stream = a.GetManifestResourceStream(resources(i))

            Dim icon As New Icon(stream)
            icons.Add(icon)
            il.Images.Add(icon.ToBitmap())
        Next i

        viewTree.ImageList = il
        Return CType(icons.ToArray(GetType(Icon)), Icon())
    End Function 'LoadIcons

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Initialize the icon list.
        _filterIndex = 0
        _icons = LoadIcons()

        Dim empty As New TreeNode("No file loaded.", CInt(NodeType.Info), CInt(NodeType.Info))
        viewTree.Nodes.Add(empty)

    End Sub

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
        Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(MimeExplorer))
        Me.viewTree = New System.Windows.Forms.TreeView
        Me.menuMain = New System.Windows.Forms.MenuStrip
        Me.itemMain = New System.Windows.Forms.ToolStripMenuItem
        Me.itemOpen = New System.Windows.Forms.ToolStripMenuItem
        Me.itemExit = New System.Windows.Forms.ToolStripMenuItem
        Me.menuContext = New System.Windows.Forms.ContextMenuStrip
        Me.itemView = New System.Windows.Forms.ToolStripMenuItem
        Me.itemSave = New System.Windows.Forms.ToolStripMenuItem
        Me.itemDecrypt = New System.Windows.Forms.ToolStripMenuItem
        Me.itemVerify = New System.Windows.Forms.ToolStripMenuItem
        Me.menuMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'viewTree
        '
        Me.viewTree.Dock = System.Windows.Forms.DockStyle.Fill
        Me.viewTree.ImageIndex = -1
        Me.viewTree.Location = New System.Drawing.Point(0, 27)
        Me.viewTree.Name = "viewTree"
        Me.viewTree.SelectedImageIndex = -1
        Me.viewTree.Size = New System.Drawing.Size(520, 273)
        Me.viewTree.TabIndex = 0
        '
        'menuMain
        '
        Me.menuMain.Items.AddRange(New System.Windows.Forms.ToolStripMenuItem() {Me.itemMain})
        '
        'itemMain
        '
        Me.itemMain.MergeIndex = 0
        Me.itemMain.DropDownItems.AddRange(New System.Windows.Forms.ToolStripMenuItem() {Me.itemOpen, Me.itemExit})
        Me.itemMain.Text = "&File"
        '
        'itemOpen
        '
        Me.itemOpen.MergeIndex = 0
        Me.itemOpen.Text = "&Open"
        '
        'itemExit
        '
        Me.itemExit.MergeIndex = 1
        Me.itemExit.Text = "&Exit"
        '
        'menuContext
        '
        Me.menuContext.Items.AddRange(New System.Windows.Forms.ToolStripMenuItem() {Me.itemView, Me.itemSave})
        '
        'itemView
        '
        Me.itemView.MergeIndex = 0
        Me.itemView.Text = "&View"
        '
        'itemSave
        '
        Me.itemSave.MergeIndex = 1
        Me.itemSave.Text = "&Save..."
        Me.menuContext.Items.AddRange(New System.Windows.Forms.ToolStripMenuItem() {Me.itemDecrypt, Me.itemVerify})
        '
        'itemDecrypt
        '
        Me.itemDecrypt.MergeIndex = 2
        Me.itemDecrypt.Text = "&Decrypt"
        '
        'itemVerify
        '
        Me.itemVerify.MergeIndex = 3
        Me.itemVerify.Text = "&Verify..."
        '
        'MimeExplorer
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(520, 273)
        Me.Controls.Add(Me.viewTree)
        Me.Controls.Add(Me.menuMain)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.menuMain
        Me.Name = "MimeExplorer"
        Me.Text = "Rebex MIME Explorer"
        Me.menuMain.ResumeLayout(False)
        Me.menuMain.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
#End Region


    'Exit command handler.
    Private Sub itemExit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles itemExit.Click
        Close()
    End Sub 'itemExit_Click


    'Open command handler - browse for a file and load it.
    Private Sub itemOpen_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles itemOpen.Click
        Dim ofd As New OpenFileDialog
        ofd.Filter = "All files (*.*)|*.*|Mail files (*.eml)|*.eml|MHT files (*.mht)|*.mht"
        ofd.FilterIndex = _filterIndex
        ofd.RestoreDirectory = True
        If ofd.ShowDialog() <> System.Windows.Forms.DialogResult.OK Then
            Return
        End If
        _filterIndex = ofd.FilterIndex

        Dim [source] As Stream = ofd.OpenFile()
        If [source] Is Nothing Then
            Return
        End If

        Try
            Dim mime As New MimeMessage
            mime.Load([source])
            [Text] = Path.GetFileName(ofd.FileName) & " - Rebex MIME Explorer"
            RefillTree(mime)
        Catch x As Exception
            MessageBox.Show(x.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        [source].Close()
    End Sub 'itemOpen_Click


    'Initialize the tree control.
    Private Sub RefillTree(ByVal top As MimeEntity)
        viewTree.BeginUpdate()

        Try
            viewTree.Nodes.Clear()

            If top Is Nothing Then
                Dim mimeFree As New TreeNode("Non-MIME message.", CInt(NodeType.Info), CInt(NodeType.Info))
                viewTree.Nodes.Add(mimeFree)
            Else
                Dim tn As TreeNode = CreateTree(top)
                viewTree.Nodes.Add(tn)
            End If
        Finally
            viewTree.EndUpdate()
        End Try
    End Sub 'RefillTree


    'Build the tree from the MIME message.
    Private Shared Function CreateTree(ByVal top As MimeEntity) As TreeNode
        Dim mt As ContentType = top.ContentType

        Dim [text] As String
        Dim cl As ContentLocation = top.ContentLocation
        If Not (cl Is Nothing) Then
            [text] = String.Format("{0}: {1}", mt.MediaType, cl.ToString())
        Else
            Dim name As String = mt.Parameters("name")
            If Not (name Is Nothing) Then
                [text] = String.Format("{0} (has name ""{1}"")", mt.MediaType, name)
            Else
                Dim type As String = Nothing
                If mt.MediaType = "multipart/related" Then
                    type = mt.Parameters("type")
                End If

                If Not (type Is Nothing) Then
                    [text] = String.Format("{0} (root type is ""{1}"")", mt.MediaType, type)
                Else
                    [text] = String.Format("{0}", mt.MediaType)
                End If
            End If
        End If

        Select Case top.Kind
            Case MimeEntityKind.Enveloped
                [text] = String.Format("{0} (encrypted content)", mt.MediaType)
            Case MimeEntityKind.Signed
                [text] = String.Format("{0} (signed content)", mt.MediaType)
        End Select

        Dim image As Integer = CInt(GetLeafImage(mt))
        Dim topNode As New TreeNode([text], image, image)

        topNode.Tag = top
        Dim i As Integer
        For i = 0 To top.Parts.Count - 1
            Dim childNode As TreeNode = CreateTree(top.Parts(i))
            topNode.Nodes.Add(childNode)
        Next i

        If Not (top.ContentMessage Is Nothing) Then
            Dim childNode As TreeNode = CreateTree(top.ContentMessage)
            topNode.Nodes.Add(childNode)
        End If

        Return topNode
    End Function 'CreateTree


    ' Determine the icon to use for the specified content type.
    Private Shared Function GetLeafImage(ByVal mt As ContentType) As NodeType
        Dim parts As String() = mt.MediaType.Split("/"c)
        If parts.Length <> 2 Then
            Return NodeType.ApplicationLeaf
        End If
        Select Case parts(0)
            Case "application"
                If parts(1) = "pkcs7-mime" OrElse parts(1) = "x-pkcs7-mime" Then
                    Return NodeType.MultipartSignedFolder
                End If
                Return NodeType.ApplicationLeaf
            Case "audio"
                Return NodeType.AudioLeaf
            Case "image"
                Return NodeType.ImageLeaf
            Case "message"
                Return NodeType.EmbeddedMessage
            Case "multipart"
                Return GetMultipartImage(parts(1))
            Case "text"
                Return GetTextLeafType(mt)
            Case Else
                Return NodeType.Leaf
        End Select
    End Function 'GetLeafImage


    'Determine the icon to use for the specified multipart subtype.
    Private Shared Function GetMultipartImage(ByVal subType As String) As NodeType
        Select Case subType
            Case "alternative"
                Return NodeType.MultipartAlternativeFolder
            Case "digest"
                Return NodeType.MultipartDigestFolder
            Case "related"
                Return NodeType.FolderSave
            Case "report"
                Return NodeType.MultipartReportFolder
            Case "signed"
                Return NodeType.MultipartSignedFolder
            Case Else
                Return NodeType.Folder
        End Select
    End Function 'GetMultipartImage


    'Determine the icon to use for the specified text subtype.
    Private Shared Function GetTextLeafType(ByVal mt As ContentType) As NodeType
        If mt.MediaType = "text/html" Then
            Return NodeType.TextHtmlLeaf
        Else
            Return NodeType.TextLeaf
        End If
    End Function 'GetTextLeafType

    ' Returns the currently selected entity.
    Private Function GetSelectedEntity() As MimeEntity
        Dim sel As TreeNode = viewTree.SelectedNode
        If sel Is Nothing Then
            Return Nothing
        End If
        Return sel.Tag
    End Function 'GetSelectedEntity

    'View the selected entity using our Viewer dialog.
    Private Sub ViewCommand()
        Dim sel As TreeNode = viewTree.SelectedNode

        If Not (sel Is Nothing) Then
            If Not TypeOf sel.Tag Is MimeEntity Then Return
            Dim payload As MimeEntity = CType(sel.Tag, MimeEntity)

            If Not payload.IsMultipart AndAlso payload.ContentMessage Is Nothing Then
                Dim viewer As New viewer(payload)
                viewer.Icon = _icons(CInt(GetLeafImage(payload.ContentType)))
                viewer.ShowDialog()
            End If
        End If
    End Sub 'ViewCommand

    Private Function DecryptCommand() As Boolean
        Dim payload As MimeEntity = GetSelectedEntity()
        If payload Is Nothing Then
            Return False
        End If

        Dim data As EnvelopedData = payload.EnvelopedContentInfo
        If data Is Nothing OrElse Not data.IsEncrypted OrElse Not data.HasPrivateKey Then
            Return False
        End If

        payload.Decrypt()
        Dim content As TreeNode = CreateTree(payload.ContentMessage)
        Dim current As TreeNode = viewTree.SelectedNode
        current.Nodes.Add(content)
        current.Expand()
        Return True
    End Function 'DecryptCommand


    ' Verify the selected entity.
    Private Sub VerifyCommand()
        Dim payload As MimeEntity = GetSelectedEntity()
        If payload Is Nothing Then
            Return
        End If

        If payload.Kind <> MimeEntityKind.Signed Then
            Return
        End If

        Dim result As SignatureValidationResult = payload.ValidateSignature()
        If result.Valid Then
            ' Validation was successful, inform user
            MessageBox.Show("Signature is valid.", "Validation result", MessageBoxButtons.OK, MessageBoxIcon.None)
        Else
            Dim verifyForm As New ValidationForm(result)
            verifyForm.ShowDialog()
        End If
    End Sub 'VerifyCommand

    'View or decrypt the selected entity when double-clicked.
    Private Sub viewTree_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles viewTree.DoubleClick
        If DecryptCommand() Then
            Return
        End If
        ViewCommand()
    End Sub 'viewTree_DoubleClick


    'View the selected entity when view command is selected from the context menu.
    Private Sub itemView_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles itemView.Click
        ViewCommand()
    End Sub 'itemView_Click

    ' Decrypts the enveloped content of the selected entity.
    Private Sub itemDecrypt_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles itemDecrypt.Click
        DecryptCommand()
    End Sub 'itemDecrypt_Click

    ' Verify the signed content of the selected entity.
    Private Sub itemVerify_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles itemVerify.Click
        VerifyCommand()
    End Sub 'itemVerify_Click

    'Save the selected entity when save command is selected from the context menu.
    Private Sub itemSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles itemSave.Click
        Dim sel As TreeNode = viewTree.SelectedNode
        If sel Is Nothing Then Return

        If Not TypeOf sel.Tag Is MimeEntity Then Return
        Dim entity As MimeEntity = CType(sel.Tag, MimeEntity)

        Dim fileName As String = entity.Name
        If fileName Is Nothing Then
            Dim ext As String
            If entity.IsMultipart OrElse Not (entity.ContentMessage Is Nothing) Then
                ext = "eml"
            Else
                Dim parts As String() = entity.ContentType.MediaType.Split("/")
                If parts.Length = 2 Then
                    ext = parts(1)
                Else
                    ext = "octet-stream"
                End If
            End If
            fileName = String.Format("part{0}.{1}", Guid.NewGuid().ToString().Substring(0, 8), ext)
        End If

        Dim sfd As New SaveFileDialog
        sfd.Filter = "All files (*.*)|*.*"
        sfd.FilterIndex = 1
        sfd.FileName = fileName
        sfd.RestoreDirectory = True

        If sfd.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK Then
            Dim dest As Stream = sfd.OpenFile()
            Try
                If entity.IsMultipart OrElse Not (entity.ContentMessage Is Nothing) Then
                    entity.Save(dest)
                Else
                    Dim [source] As Stream = entity.GetContentStream()
                    Try
                        [source].Seek(0, SeekOrigin.Begin)

                        Dim buffer(1023) As Byte
                        While True
                            Dim n As Integer = [source].Read(buffer, 0, buffer.Length)
                            If n = 0 Then
                                Exit While
                            End If
                            dest.Write(buffer, 0, n)
                        End While
                    Finally
                        [source].Close()
                    End Try
                End If
            Finally
                dest.Close()
            End Try
        End If
    End Sub 'itemSave_Click


    'Handles the MouseUp event of the viewTree control.
    Private Sub viewTree_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles viewTree.MouseUp
        If e.Button = System.Windows.Forms.MouseButtons.Right Then
            Dim sel As TreeNode = viewTree.GetNodeAt(e.X, e.Y)
            If Not (sel Is Nothing) AndAlso sel Is viewTree.SelectedNode Then
                If TypeOf sel.Tag Is MimeEntity Then
                    Dim entity As MimeEntity = CType(sel.Tag, MimeEntity)
                    If entity.IsMultipart OrElse Not (entity.ContentMessage Is Nothing) Then
                        itemView.Enabled = False
                    Else
                        itemView.Enabled = True
                    End If

                    If entity.Kind = MimeEntityKind.Enveloped AndAlso entity.EnvelopedContentInfo.IsEncrypted AndAlso entity.EnvelopedContentInfo.HasPrivateKey Then
                        itemDecrypt.Enabled = True
                    Else
                        itemDecrypt.Enabled = False
                    End If
                    If entity.Kind = MimeEntityKind.Signed Then
                        itemVerify.Enabled = True
                    Else
                        itemVerify.Enabled = False
                    End If

                    menuContext.Show(viewTree, New Point(e.X, e.Y))
                End If
            End If
        End If
    End Sub 'viewTree_MouseUp


    'Handles the MouseDown event of the viewTree control.
    Private Sub viewTree_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles viewTree.MouseDown
        Dim sel As TreeNode = viewTree.GetNodeAt(e.X, e.Y)
        If Not (sel Is Nothing) AndAlso Not (viewTree.SelectedNode Is sel) Then
            viewTree.SelectedNode = sel
        End If
    End Sub 'viewTree_MouseDown 


    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class 'MimeExplorer
