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

Imports Rebex.Net

Public Class SecurityConfig
    Inherits System.Windows.Forms.Form

    Public Const DefaultTlsVersion As TlsVersion = TlsVersion.TLS13 Or TlsVersion.TLS12

    Public Property Protocol() As TlsVersion
        Get
            Dim version As TlsVersion = TlsVersion.None
            If cbTLS13.Checked Then version = version Or TlsVersion.TLS13
            If cbTLS12.Checked Then version = version Or TlsVersion.TLS12
            If cbTLS11.Checked Then version = version Or TlsVersion.TLS11
            If cbTLS10.Checked Then version = version Or TlsVersion.TLS10
            Return version
        End Get
        Set(ByVal Value As TlsVersion)
            cbTLS13.Checked = (Value And TlsVersion.TLS13) <> 0
            cbTLS12.Checked = (Value And TlsVersion.TLS12) <> 0
            cbTLS11.Checked = (Value And TlsVersion.TLS11) <> 0
            cbTLS10.Checked = (Value And TlsVersion.TLS10) <> 0
        End Set
    End Property


    Public Property AllowedSuite() As TlsCipherSuite
        Get
            If cbSuite.SelectedIndex = 0 Then
                Return TlsCipherSuite.All
            Else
                Return TlsCipherSuite.Secure
            End If
        End Get
        Set(ByVal Value As TlsCipherSuite)
            Select Case Value
                Case TlsCipherSuite.Secure
                    cbSuite.SelectedIndex = 1
                Case Else
                    cbSuite.SelectedIndex = 0
            End Select
        End Set
    End Property

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        DialogResult = System.Windows.Forms.DialogResult.Ignore
    End Sub

#Region " Windows Form Designer generated code "

    Friend WithEvents cbSuite As System.Windows.Forms.ComboBox
    Friend WithEvents label14 As System.Windows.Forms.Label
    Friend WithEvents label13 As System.Windows.Forms.Label
    Friend WithEvents cmdOK As System.Windows.Forms.Button
    Friend WithEvents cmdCancel As System.Windows.Forms.Button
    Private WithEvents cbTLS13 As System.Windows.Forms.CheckBox
    Private WithEvents cbTLS10 As System.Windows.Forms.CheckBox
    Private WithEvents cbTLS11 As System.Windows.Forms.CheckBox
    Private WithEvents cbTLS12 As System.Windows.Forms.CheckBox

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
        Me.cbSuite = New System.Windows.Forms.ComboBox()
        Me.label14 = New System.Windows.Forms.Label()
        Me.label13 = New System.Windows.Forms.Label()
        Me.cmdOK = New System.Windows.Forms.Button()
        Me.cmdCancel = New System.Windows.Forms.Button()
        Me.cbTLS13 = New System.Windows.Forms.CheckBox()
        Me.cbTLS10 = New System.Windows.Forms.CheckBox()
        Me.cbTLS11 = New System.Windows.Forms.CheckBox()
        Me.cbTLS12 = New System.Windows.Forms.CheckBox()
        Me.SuspendLayout()
        '
        'cbSuite
        '
        Me.cbSuite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbSuite.Items.AddRange(New Object() {"All ciphers", "Secure only"})
        Me.cbSuite.Location = New System.Drawing.Point(48, 88)
        Me.cbSuite.Name = "cbSuite"
        Me.cbSuite.Size = New System.Drawing.Size(168, 21)
        Me.cbSuite.TabIndex = 6
        '
        'label14
        '
        Me.label14.Location = New System.Drawing.Point(8, 72)
        Me.label14.Name = "label14"
        Me.label14.Size = New System.Drawing.Size(112, 23)
        Me.label14.TabIndex = 8
        Me.label14.Text = "Allowed suites:"
        '
        'label13
        '
        Me.label13.Location = New System.Drawing.Point(8, 8)
        Me.label13.Name = "label13"
        Me.label13.Size = New System.Drawing.Size(112, 23)
        Me.label13.TabIndex = 7
        Me.label13.Text = "Allowed protocols:"
        '
        'cmdOK
        '
        Me.cmdOK.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!)
        Me.cmdOK.Location = New System.Drawing.Point(168, 120)
        Me.cmdOK.Name = "cmdOK"
        Me.cmdOK.Size = New System.Drawing.Size(64, 24)
        Me.cmdOK.TabIndex = 0
        Me.cmdOK.Text = "OK"
        '
        'cmdCancel
        '
        Me.cmdCancel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!)
        Me.cmdCancel.Location = New System.Drawing.Point(96, 120)
        Me.cmdCancel.Name = "cmdCancel"
        Me.cmdCancel.Size = New System.Drawing.Size(64, 24)
        Me.cmdCancel.TabIndex = 1
        Me.cmdCancel.Text = "Cancel"
        '
        'cbTLS13
        '
        Me.cbTLS13.Checked = True
        Me.cbTLS13.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cbTLS13.Location = New System.Drawing.Point(118, 41)
        Me.cbTLS13.Name = "cbTLS13"
        Me.cbTLS13.Size = New System.Drawing.Size(72, 16)
        Me.cbTLS13.TabIndex = 5
        Me.cbTLS13.Text = "TLS 1.3"
        '
        'cbTLS10
        '
        Me.cbTLS10.Location = New System.Drawing.Point(48, 25)
        Me.cbTLS10.Name = "cbTLS10"
        Me.cbTLS10.Size = New System.Drawing.Size(72, 16)
        Me.cbTLS10.TabIndex = 2
        Me.cbTLS10.Text = "TLS 1.0"
        '
        'cbTLS11
        '
        Me.cbTLS11.Location = New System.Drawing.Point(118, 25)
        Me.cbTLS11.Name = "cbTLS11"
        Me.cbTLS11.Size = New System.Drawing.Size(72, 16)
        Me.cbTLS11.TabIndex = 4
        Me.cbTLS11.Text = "TLS 1.1"
        '
        'cbTLS12
        '
        Me.cbTLS12.Checked = True
        Me.cbTLS12.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cbTLS12.Location = New System.Drawing.Point(48, 41)
        Me.cbTLS12.Name = "cbTLS12"
        Me.cbTLS12.Size = New System.Drawing.Size(72, 16)
        Me.cbTLS12.TabIndex = 3
        Me.cbTLS12.Text = "TLS 1.2"
        '
        'SecurityConfig
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(240, 157)
        Me.Controls.Add(Me.cbTLS11)
        Me.Controls.Add(Me.cbTLS13)
        Me.Controls.Add(Me.cbTLS10)
        Me.Controls.Add(Me.cbTLS12)
        Me.Controls.Add(Me.cmdOK)
        Me.Controls.Add(Me.cmdCancel)
        Me.Controls.Add(Me.cbSuite)
        Me.Controls.Add(Me.label14)
        Me.Controls.Add(Me.label13)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "SecurityConfig"
        Me.ShowInTaskbar = False
        Me.Text = "Security Settings"
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Sub cmdOK_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdOK.Click
        DialogResult = System.Windows.Forms.DialogResult.OK
        Close()
    End Sub 'cmdOK_Click


    Private Sub cmdCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
        DialogResult = System.Windows.Forms.DialogResult.Cancel
        Close()
    End Sub 'cmdCancel_Click

End Class

