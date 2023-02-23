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


Partial Class UcTreeView
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

#Region "Component Designer generated code"

    ''' <summary> 
    ''' Required method for Designer support - do not modify 
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
        Me.treeView = New System.Windows.Forms.TreeView()
        Me.SuspendLayout()
        ' 
        ' treeView
        ' 
        Me.treeView.Dock = System.Windows.Forms.DockStyle.Fill
        Me.treeView.FullRowSelect = True
        Me.treeView.HideSelection = False
        Me.treeView.HotTracking = True
        Me.treeView.Location = New System.Drawing.Point(0, 0)
        Me.treeView.Name = "treeView"
        Me.treeView.PathSeparator = "/"
        Me.treeView.Size = New System.Drawing.Size(150, 150)
        Me.treeView.TabIndex = 0
        AddHandler Me.treeView.AfterExpand, (AddressOf Me.treeView_AfterExpand)
        AddHandler Me.treeView.AfterSelect, (AddressOf Me.treeView_AfterSelect)
        ' 
        ' UcTreeView
        ' 
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0F, 13.0F)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.treeView)
        Me.Name = "UcTreeView"
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private treeView As System.Windows.Forms.TreeView
End Class

