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

Partial Class ImportCertificateDialog
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
        Me.btnStore = New System.Windows.Forms.Button()
        Me.btnFile = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'btnStore
        '
        Me.btnStore.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnStore.Location = New System.Drawing.Point(12, 12)
        Me.btnStore.Name = "btnStore"
        Me.btnStore.Size = New System.Drawing.Size(233, 23)
        Me.btnStore.TabIndex = 0
        Me.btnStore.Text = "From certificate store..."
        Me.btnStore.UseVisualStyleBackColor = True
        '
        'btnFile
        '
        Me.btnFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnFile.Location = New System.Drawing.Point(12, 41)
        Me.btnFile.Name = "btnFile"
        Me.btnFile.Size = New System.Drawing.Size(233, 23)
        Me.btnFile.TabIndex = 1
        Me.btnFile.Text = "From file..."
        Me.btnFile.UseVisualStyleBackColor = True
        '
        'ImportCertificateDialog
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(257, 75)
        Me.Controls.Add(Me.btnFile)
        Me.Controls.Add(Me.btnStore)
        Me.Name = "ImportCertificateDialog"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Import Certificate"
        Me.ResumeLayout(False)

    End Sub

    #End Region

    Private WithEvents btnStore As System.Windows.Forms.Button
    Private WithEvents btnFile As System.Windows.Forms.Button
End Class
