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
Imports System.ComponentModel
Imports System.Drawing
Imports System.Data
Imports System.Windows.Forms
Imports Rebex.Net


''' <summary>
''' Summary description for ImapFolderSelector.
''' </summary>

Public Class ImapFolderSelector
    Inherits System.Windows.Forms.UserControl

    Private WithEvents treeView As System.Windows.Forms.TreeView

    Private _imap As Imap
    Private _lastSelected As TreeNode
    Private _initialized As Boolean

    Public ReadOnly Property IsInitialized() As Boolean
        Get
            Return _initialized
        End Get
    End Property

    Public ReadOnly Property SelectedFolder() As String
        Get
            Dim node As TreeNode = treeView.SelectedNode
            If node Is Nothing Then
                Return Nothing
            End If
            Dim tag As Object() = CType(node.Tag, Object())
            Dim folder As ImapFolder = CType(tag(0), ImapFolder)

            If Not folder.IsSelectable Then
                Return Nothing
            End If
            Return folder.Name
        End Get
    End Property

    Public Event SelectedFolderChanged As EventHandler


    ''' <summary>
    ''' Sets the internal IMAP object.
    ''' </summary>
    Public Sub SetClient(ByVal imap As Imap)
        _imap = imap
        If _initialized OrElse imap Is Nothing Then
            Return
        End If
        GetFolderList(treeView.Nodes, "", Nothing)

        _initialized = True

        Dim node As TreeNode
        For Each node In treeView.Nodes
            If String.Compare(node.Text, "Inbox", True) = 0 Then
                treeView.SelectedNode = node
                Exit For
            End If
        Next node
    End Sub       'SetClient


    Private Sub GetFolderList(ByVal nodes As TreeNodeCollection, ByVal name As String, ByVal delimiter As String)
        Cursor.Current = Cursors.WaitCursor
        Dim folders As ImapFolderCollection = _imap.GetFolderList(name, ImapFolderListMode.All, False)
        Cursor.Current = Cursors.Default
        Dim folder As ImapFolder
        For Each folder In folders
            Dim folderName As String = folder.Name
            If Not (delimiter Is Nothing) Then
                Dim p As Integer = folderName.LastIndexOf(delimiter)
                If p >= 0 Then
                    folderName = folderName.Substring((p + delimiter.Length))
                End If
            End If
            Dim node As New TreeNode(folderName)
            node.Tag = New Object() {folder, folder.CanContainInferiors}
            nodes.Add(node)
        Next folder
    End Sub       'GetFolderList

    ''' <summary> 
    ''' Required designer variable.
    ''' </summary>
    Private components As System.ComponentModel.Container = Nothing


    Public Sub New()
        ' This call is required by the Windows.Forms Form Designer.
        InitializeComponent()
    End Sub       'New


    ''' <summary> 
    ''' Clean up any resources being used.
    ''' </summary>
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub       'Dispose

#Region "Component Designer generated code"

    ''' <summary> 
    ''' Required method for Designer support - do not modify 
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
        Me.treeView = New System.Windows.Forms.TreeView
        Me.SuspendLayout()
        ' 
        ' treeView
        ' 
        Me.treeView.Dock = System.Windows.Forms.DockStyle.Fill
        Me.treeView.ImageIndex = -1
        Me.treeView.Location = New System.Drawing.Point(0, 0)
        Me.treeView.Name = "treeView"
        Me.treeView.SelectedImageIndex = -1
        Me.treeView.Size = New System.Drawing.Size(150, 150)
        Me.treeView.TabIndex = 0
        ' 
        ' ImapFolderSelector
        ' 
        Me.Controls.Add(treeView)
        Me.Name = "ImapFolderSelector"
        Me.ResumeLayout(False)
    End Sub       'InitializeComponent 
#End Region


    Private Sub treeView_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles treeView.MouseDown
        If Not Enabled Then
            Return
        End If
        Dim sel As TreeNode = treeView.GetNodeAt(e.X, e.Y)
        If Not (sel Is Nothing) AndAlso Not treeView.SelectedNode Is sel Then
            treeView.SelectedNode = sel
        End If
    End Sub       'treeView_MouseDown

    Public Sub RefreshFolder()
        Dim node As TreeNode = treeView.SelectedNode
        Dim tag As Object() = CType(node.Tag, Object())
        Dim folder As ImapFolder = CType(tag(0), ImapFolder)
        Dim expandable As Boolean = CBool(tag(1))
        If expandable AndAlso Not (_imap Is Nothing) Then
            GetFolderList(node.Nodes, folder.Name, folder.Delimiter)
            node.Tag = New Object() {folder, False}
        End If
    End Sub       'RefreshFolder


    Private Sub treeView_AfterSelect(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles treeView.AfterSelect
        If Not Enabled Then
            treeView.SelectedNode = _lastSelected
            Return
        End If

        Dim node As TreeNode = treeView.SelectedNode
        If node Is _lastSelected Then
            Return
        End If
        _lastSelected = node
        If node Is Nothing Then
            Return
        End If

        RaiseEvent SelectedFolderChanged(Me, New EventArgs)
    End Sub       'treeView_AfterSelect
End Class   'ImapFolderSelector 'Rebex.Samples.ImapBrowser