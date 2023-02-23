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

Partial Class MainForm
    ''' <summary>
    ''' Required designer variable.
    ''' </summary>
    Private components As System.ComponentModel.IContainer = Nothing

    ''' <summary>
    ''' Clean up any resources being used.
    ''' </summary>
    ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing AndAlso (components IsNot Nothing) Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    #Region "Windows Form Designer generated code"

    ''' <summary>
    ''' Required method for Designer support - do not modify
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.splitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.folderTree = New Rebex.Samples.EwsFolderTree()
        Me.itemListView = New System.Windows.Forms.ListView()
        Me.columnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.columnHeader2 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.columnHeader3 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.contextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.viewMeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.deleteToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.saveAsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.menuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.fileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.connectToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.disconnectToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.exitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        CType(Me.splitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.splitContainer1.Panel1.SuspendLayout()
        Me.splitContainer1.Panel2.SuspendLayout()
        Me.splitContainer1.SuspendLayout()
        Me.contextMenuStrip1.SuspendLayout()
        Me.menuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'splitContainer1
        '
        Me.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.splitContainer1.Location = New System.Drawing.Point(0, 24)
        Me.splitContainer1.Name = "splitContainer1"
        '
        'splitContainer1.Panel1
        '
        Me.splitContainer1.Panel1.Controls.Add(Me.folderTree)
        '
        'splitContainer1.Panel2
        '
        Me.splitContainer1.Panel2.Controls.Add(Me.itemListView)
        Me.splitContainer1.Size = New System.Drawing.Size(624, 417)
        Me.splitContainer1.SplitterDistance = 150
        Me.splitContainer1.TabIndex = 2
        Me.splitContainer1.TabStop = False
        '
        'folderTree
        '
        Me.folderTree.Dock = System.Windows.Forms.DockStyle.Fill
        Me.folderTree.Location = New System.Drawing.Point(0, 0)
        Me.folderTree.Name = "folderTree"
        Me.folderTree.Size = New System.Drawing.Size(150, 417)
        Me.folderTree.TabIndex = 1
        '
        'itemListView
        '
        Me.itemListView.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.columnHeader1, Me.columnHeader2, Me.columnHeader3})
        Me.itemListView.ContextMenuStrip = Me.contextMenuStrip1
        Me.itemListView.Dock = System.Windows.Forms.DockStyle.Fill
        Me.itemListView.FullRowSelect = True
        Me.itemListView.GridLines = True
        Me.itemListView.Location = New System.Drawing.Point(0, 0)
        Me.itemListView.MultiSelect = False
        Me.itemListView.Name = "itemListView"
        Me.itemListView.Size = New System.Drawing.Size(470, 417)
        Me.itemListView.TabIndex = 0
        Me.itemListView.UseCompatibleStateImageBehavior = False
        Me.itemListView.View = System.Windows.Forms.View.Details
        Me.itemListView.VirtualMode = True
        '
        'columnHeader1
        '
        Me.columnHeader1.Text = "From"
        Me.columnHeader1.Width = 107
        '
        'columnHeader2
        '
        Me.columnHeader2.Text = "Subject"
        Me.columnHeader2.Width = 230
        '
        'columnHeader3
        '
        Me.columnHeader3.Text = "Date"
        Me.columnHeader3.Width = 124
        '
        'contextMenuStrip1
        '
        Me.contextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.viewMeToolStripMenuItem, Me.deleteToolStripMenuItem, Me.saveAsToolStripMenuItem})
        Me.contextMenuStrip1.Name = "contextMenuStrip1"
        Me.contextMenuStrip1.Size = New System.Drawing.Size(170, 70)
        '
        'viewMeToolStripMenuItem
        '
        Me.viewMeToolStripMenuItem.Name = "viewMeToolStripMenuItem"
        Me.viewMeToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.[Return]), System.Windows.Forms.Keys)
        Me.viewMeToolStripMenuItem.Size = New System.Drawing.Size(169, 22)
        Me.viewMeToolStripMenuItem.Text = "&View..."
        '
        'deleteToolStripMenuItem
        '
        Me.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem"
        Me.deleteToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete
        Me.deleteToolStripMenuItem.Size = New System.Drawing.Size(169, 22)
        Me.deleteToolStripMenuItem.Text = "&Delete"
        '
        'saveAsToolStripMenuItem
        '
        Me.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem"
        Me.saveAsToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.S), System.Windows.Forms.Keys)
        Me.saveAsToolStripMenuItem.Size = New System.Drawing.Size(169, 22)
        Me.saveAsToolStripMenuItem.Text = "&Save As..."
        '
        'menuStrip1
        '
        Me.menuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.fileToolStripMenuItem})
        Me.menuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.menuStrip1.Name = "menuStrip1"
        Me.menuStrip1.Size = New System.Drawing.Size(624, 24)
        Me.menuStrip1.TabIndex = 3
        Me.menuStrip1.Text = "menuStrip1"
        '
        'fileToolStripMenuItem
        '
        Me.fileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.connectToolStripMenuItem, Me.disconnectToolStripMenuItem, Me.toolStripMenuItem1, Me.exitToolStripMenuItem})
        Me.fileToolStripMenuItem.Name = "fileToolStripMenuItem"
        Me.fileToolStripMenuItem.Size = New System.Drawing.Size(37, 20)
        Me.fileToolStripMenuItem.Text = "&File"
        '
        'connectToolStripMenuItem
        '
        Me.connectToolStripMenuItem.Name = "connectToolStripMenuItem"
        Me.connectToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.O), System.Windows.Forms.Keys)
        Me.connectToolStripMenuItem.Size = New System.Drawing.Size(175, 22)
        Me.connectToolStripMenuItem.Text = "&Connect..."
        '
        'disconnectToolStripMenuItem
        '
        Me.disconnectToolStripMenuItem.Enabled = False
        Me.disconnectToolStripMenuItem.Name = "disconnectToolStripMenuItem"
        Me.disconnectToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.D), System.Windows.Forms.Keys)
        Me.disconnectToolStripMenuItem.Size = New System.Drawing.Size(175, 22)
        Me.disconnectToolStripMenuItem.Text = "&Disconnect"
        '
        'toolStripMenuItem1
        '
        Me.toolStripMenuItem1.Name = "toolStripMenuItem1"
        Me.toolStripMenuItem1.Size = New System.Drawing.Size(172, 6)
        '
        'exitToolStripMenuItem
        '
        Me.exitToolStripMenuItem.Name = "exitToolStripMenuItem"
        Me.exitToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Alt Or System.Windows.Forms.Keys.F4), System.Windows.Forms.Keys)
        Me.exitToolStripMenuItem.Size = New System.Drawing.Size(175, 22)
        Me.exitToolStripMenuItem.Text = "E&xit"
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(624, 441)
        Me.Controls.Add(Me.splitContainer1)
        Me.Controls.Add(Me.menuStrip1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.menuStrip1
        Me.Name = "MainForm"
        Me.Text = "Exchange Browser"
        Me.splitContainer1.Panel1.ResumeLayout(False)
        Me.splitContainer1.Panel2.ResumeLayout(False)
        CType(Me.splitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.splitContainer1.ResumeLayout(False)
        Me.contextMenuStrip1.ResumeLayout(False)
        Me.menuStrip1.ResumeLayout(False)
        Me.menuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    #End Region
    Private WithEvents folderTree As Rebex.Samples.EwsFolderTree
    Private splitContainer1 As System.Windows.Forms.SplitContainer
    Private WithEvents itemListView As System.Windows.Forms.ListView
    Private columnHeader1 As System.Windows.Forms.ColumnHeader
    Private columnHeader2 As System.Windows.Forms.ColumnHeader
    Private columnHeader3 As System.Windows.Forms.ColumnHeader
    Private menuStrip1 As System.Windows.Forms.MenuStrip
    Private WithEvents fileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents connectToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents disconnectToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents toolStripMenuItem1 As System.Windows.Forms.ToolStripSeparator
    Private WithEvents exitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents contextMenuStrip1 As System.Windows.Forms.ContextMenuStrip
    Private WithEvents viewMeToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents deleteToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents saveAsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
End Class

