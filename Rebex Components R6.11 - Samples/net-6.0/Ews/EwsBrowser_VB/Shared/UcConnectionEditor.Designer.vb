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

Imports System.Windows.Forms


Partial Class UcConnectionEditor
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
        Me.tabSsl = New System.Windows.Forms.TabPage()
        Me.panel1 = New System.Windows.Forms.Panel()
        Me.cmbSuites = New System.Windows.Forms.ComboBox()
        Me.cbAllowTls13 = New System.Windows.Forms.CheckBox()
        Me.cbAllowTls12 = New System.Windows.Forms.CheckBox()
        Me.cbAllowTls11 = New System.Windows.Forms.CheckBox()
        Me.cbAllowTls10 = New System.Windows.Forms.CheckBox()
        Me.label23 = New System.Windows.Forms.Label()
        Me.label22 = New System.Windows.Forms.Label()
        Me.pnlServerCertificate = New System.Windows.Forms.Panel()
        Me.pnlLocalyStoredCertificate = New System.Windows.Forms.Panel()
        Me.label9 = New System.Windows.Forms.Label()
        Me.tbServerCertificateThumbprint = New System.Windows.Forms.TextBox()
        Me.label10 = New System.Windows.Forms.Label()
        Me.rbServerCertificateAny = New System.Windows.Forms.RadioButton()
        Me.rbServerCertificateStored = New System.Windows.Forms.RadioButton()
        Me.rbServerCertificateWindows = New System.Windows.Forms.RadioButton()
        Me.label7 = New System.Windows.Forms.Label()
        Me.tabProxy = New System.Windows.Forms.TabPage()
        Me.pnlProxy = New System.Windows.Forms.Panel()
        Me.label19 = New System.Windows.Forms.Label()
        Me.tbProxyUsername = New System.Windows.Forms.TextBox()
        Me.label11 = New System.Windows.Forms.Label()
        Me.cmbProxyType = New System.Windows.Forms.ComboBox()
        Me.label13 = New System.Windows.Forms.Label()
        Me.label14 = New System.Windows.Forms.Label()
        Me.tbProxyPassword = New System.Windows.Forms.TextBox()
        Me.tbProxyPort = New System.Windows.Forms.TextBox()
        Me.label6 = New System.Windows.Forms.Label()
        Me.tbProxyHost = New System.Windows.Forms.TextBox()
        Me.label12 = New System.Windows.Forms.Label()
        Me.tabServer = New System.Windows.Forms.TabPage()
        Me.grpCredentials = New System.Windows.Forms.GroupBox()
        Me.pnlClientCertificate = New System.Windows.Forms.Panel()
        Me.label8 = New System.Windows.Forms.Label()
        Me.btnClearClientCertificate = New System.Windows.Forms.Button()
        Me.tbClientCertificate = New System.Windows.Forms.TextBox()
        Me.btnViewClientCertificate = New System.Windows.Forms.Button()
        Me.btnImportClientCertificate = New System.Windows.Forms.Button()
        Me.pnlMailbox = New System.Windows.Forms.Panel()
        Me.tbMailbox = New System.Windows.Forms.TextBox()
        Me.lblMailbox = New System.Windows.Forms.Label()
        Me.pnlUsernamePassword = New System.Windows.Forms.Panel()
        Me.cbSingleSignOn = New System.Windows.Forms.CheckBox()
        Me.cbStorePassword = New System.Windows.Forms.CheckBox()
        Me.tbServerUser = New System.Windows.Forms.TextBox()
        Me.label4 = New System.Windows.Forms.Label()
        Me.label5 = New System.Windows.Forms.Label()
        Me.tbServerPassword = New System.Windows.Forms.TextBox()
        Me.grpConnection = New System.Windows.Forms.GroupBox()
        Me.pnlHostnameProtocol = New System.Windows.Forms.Panel()
        Me.tbServerPort = New System.Windows.Forms.TextBox()
        Me.label3 = New System.Windows.Forms.Label()
        Me.label1 = New System.Windows.Forms.Label()
        Me.cmbProtocol = New System.Windows.Forms.ComboBox()
        Me.tbServerHost = New System.Windows.Forms.TextBox()
        Me.label2 = New System.Windows.Forms.Label()
        Me.tabControlMain = New System.Windows.Forms.TabControl()
        Me.tabSsl.SuspendLayout()
        Me.panel1.SuspendLayout()
        Me.pnlServerCertificate.SuspendLayout()
        Me.pnlLocalyStoredCertificate.SuspendLayout()
        Me.tabProxy.SuspendLayout()
        Me.pnlProxy.SuspendLayout()
        Me.tabServer.SuspendLayout()
        Me.grpCredentials.SuspendLayout()
        Me.pnlClientCertificate.SuspendLayout()
        Me.pnlMailbox.SuspendLayout()
        Me.pnlUsernamePassword.SuspendLayout()
        Me.grpConnection.SuspendLayout()
        Me.pnlHostnameProtocol.SuspendLayout()
        Me.tabControlMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'tabSsl
        '
        Me.tabSsl.Controls.Add(Me.panel1)
        Me.tabSsl.Controls.Add(Me.pnlServerCertificate)
        Me.tabSsl.Location = New System.Drawing.Point(4, 22)
        Me.tabSsl.Name = "tabSsl"
        Me.tabSsl.Padding = New System.Windows.Forms.Padding(3)
        Me.tabSsl.Size = New System.Drawing.Size(612, 331)
        Me.tabSsl.TabIndex = 2
        Me.tabSsl.Text = "TLS/SSL"
        '
        'panel1
        '
        Me.panel1.Controls.Add(Me.cmbSuites)
        Me.panel1.Controls.Add(Me.cbAllowTls13)
        Me.panel1.Controls.Add(Me.cbAllowTls12)
        Me.panel1.Controls.Add(Me.cbAllowTls11)
        Me.panel1.Controls.Add(Me.cbAllowTls10)
        Me.panel1.Controls.Add(Me.label23)
        Me.panel1.Controls.Add(Me.label22)
        Me.panel1.Location = New System.Drawing.Point(3, 132)
        Me.panel1.Name = "panel1"
        Me.panel1.Size = New System.Drawing.Size(606, 67)
        Me.panel1.TabIndex = 45
        '
        'cmbSuites
        '
        Me.cmbSuites.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbSuites.FormattingEnabled = True
        Me.cmbSuites.Items.AddRange(New Object() {"All ciphers", "Secure only"})
        Me.cmbSuites.Location = New System.Drawing.Point(122, 32)
        Me.cmbSuites.Name = "cmbSuites"
        Me.cmbSuites.Size = New System.Drawing.Size(117, 21)
        Me.cmbSuites.TabIndex = 6
        '
        'cbAllowTls13
        '
        Me.cbAllowTls13.AutoSize = True
        Me.cbAllowTls13.Location = New System.Drawing.Point(122, 9)
        Me.cbAllowTls13.Name = "cbAllowTls13"
        Me.cbAllowTls13.Size = New System.Drawing.Size(64, 17)
        Me.cbAllowTls13.TabIndex = 2
        Me.cbAllowTls13.Text = "TLS 1.3"
        Me.cbAllowTls13.UseVisualStyleBackColor = True
        '
        'cbAllowTls12
        '
        Me.cbAllowTls12.AutoSize = True
        Me.cbAllowTls12.Checked = True
        Me.cbAllowTls12.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cbAllowTls12.Location = New System.Drawing.Point(192, 9)
        Me.cbAllowTls12.Name = "cbAllowTls12"
        Me.cbAllowTls12.Size = New System.Drawing.Size(64, 17)
        Me.cbAllowTls12.TabIndex = 3
        Me.cbAllowTls12.Text = "TLS 1.2"
        Me.cbAllowTls12.UseVisualStyleBackColor = True
        '
        'cbAllowTls11
        '
        Me.cbAllowTls11.AutoSize = True
        Me.cbAllowTls11.Checked = True
        Me.cbAllowTls11.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cbAllowTls11.Location = New System.Drawing.Point(262, 9)
        Me.cbAllowTls11.Name = "cbAllowTls11"
        Me.cbAllowTls11.Size = New System.Drawing.Size(64, 17)
        Me.cbAllowTls11.TabIndex = 4
        Me.cbAllowTls11.Text = "TLS 1.1"
        Me.cbAllowTls11.UseVisualStyleBackColor = True
        '
        'cbAllowTls10
        '
        Me.cbAllowTls10.AutoSize = True
        Me.cbAllowTls10.Checked = True
        Me.cbAllowTls10.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cbAllowTls10.Location = New System.Drawing.Point(332, 9)
        Me.cbAllowTls10.Name = "cbAllowTls10"
        Me.cbAllowTls10.Size = New System.Drawing.Size(64, 17)
        Me.cbAllowTls10.TabIndex = 5
        Me.cbAllowTls10.Text = "TLS 1.0"
        Me.cbAllowTls10.UseVisualStyleBackColor = True
        '
        'label23
        '
        Me.label23.AutoSize = True
        Me.label23.Location = New System.Drawing.Point(6, 35)
        Me.label23.Name = "label23"
        Me.label23.Size = New System.Drawing.Size(77, 13)
        Me.label23.TabIndex = 1
        Me.label23.Text = "Allowed suites:"
        '
        'label22
        '
        Me.label22.AutoSize = True
        Me.label22.Location = New System.Drawing.Point(6, 10)
        Me.label22.Name = "label22"
        Me.label22.Size = New System.Drawing.Size(88, 13)
        Me.label22.TabIndex = 0
        Me.label22.Text = "Allowed protocol:"
        '
        'pnlServerCertificate
        '
        Me.pnlServerCertificate.BackColor = System.Drawing.Color.Transparent
        Me.pnlServerCertificate.Controls.Add(Me.pnlLocalyStoredCertificate)
        Me.pnlServerCertificate.Controls.Add(Me.rbServerCertificateAny)
        Me.pnlServerCertificate.Controls.Add(Me.rbServerCertificateStored)
        Me.pnlServerCertificate.Controls.Add(Me.rbServerCertificateWindows)
        Me.pnlServerCertificate.Controls.Add(Me.label7)
        Me.pnlServerCertificate.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlServerCertificate.Location = New System.Drawing.Point(3, 3)
        Me.pnlServerCertificate.Name = "pnlServerCertificate"
        Me.pnlServerCertificate.Size = New System.Drawing.Size(606, 149)
        Me.pnlServerCertificate.TabIndex = 43
        '
        'pnlLocalyStoredCertificate
        '
        Me.pnlLocalyStoredCertificate.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlLocalyStoredCertificate.Controls.Add(Me.label9)
        Me.pnlLocalyStoredCertificate.Controls.Add(Me.tbServerCertificateThumbprint)
        Me.pnlLocalyStoredCertificate.Controls.Add(Me.label10)
        Me.pnlLocalyStoredCertificate.Location = New System.Drawing.Point(140, 67)
        Me.pnlLocalyStoredCertificate.Name = "pnlLocalyStoredCertificate"
        Me.pnlLocalyStoredCertificate.Size = New System.Drawing.Size(463, 79)
        Me.pnlLocalyStoredCertificate.TabIndex = 41
        '
        'label9
        '
        Me.label9.AutoSize = True
        Me.label9.Location = New System.Drawing.Point(3, 19)
        Me.label9.Name = "label9"
        Me.label9.Size = New System.Drawing.Size(60, 13)
        Me.label9.TabIndex = 46
        Me.label9.Text = "(SH1 hash)"
        '
        'tbServerCertificateThumbprint
        '
        Me.tbServerCertificateThumbprint.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tbServerCertificateThumbprint.Location = New System.Drawing.Point(90, 3)
        Me.tbServerCertificateThumbprint.Multiline = True
        Me.tbServerCertificateThumbprint.Name = "tbServerCertificateThumbprint"
        Me.tbServerCertificateThumbprint.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.tbServerCertificateThumbprint.Size = New System.Drawing.Size(371, 73)
        Me.tbServerCertificateThumbprint.TabIndex = 45
        '
        'label10
        '
        Me.label10.AutoSize = True
        Me.label10.Location = New System.Drawing.Point(3, 6)
        Me.label10.Name = "label10"
        Me.label10.Size = New System.Drawing.Size(63, 13)
        Me.label10.TabIndex = 42
        Me.label10.Text = "Thumbprint:"
        '
        'rbServerCertificateAny
        '
        Me.rbServerCertificateAny.AutoSize = True
        Me.rbServerCertificateAny.Location = New System.Drawing.Point(122, 1)
        Me.rbServerCertificateAny.Name = "rbServerCertificateAny"
        Me.rbServerCertificateAny.Size = New System.Drawing.Size(128, 17)
        Me.rbServerCertificateAny.TabIndex = 31
        Me.rbServerCertificateAny.Text = "Accept any certificate"
        Me.rbServerCertificateAny.UseVisualStyleBackColor = True
        '
        'rbServerCertificateStored
        '
        Me.rbServerCertificateStored.AutoSize = True
        Me.rbServerCertificateStored.Location = New System.Drawing.Point(122, 47)
        Me.rbServerCertificateStored.Name = "rbServerCertificateStored"
        Me.rbServerCertificateStored.Size = New System.Drawing.Size(197, 17)
        Me.rbServerCertificateStored.TabIndex = 33
        Me.rbServerCertificateStored.Text = "Trust locally stored server certificate:"
        Me.rbServerCertificateStored.UseVisualStyleBackColor = True
        '
        'rbServerCertificateWindows
        '
        Me.rbServerCertificateWindows.AutoSize = True
        Me.rbServerCertificateWindows.Checked = True
        Me.rbServerCertificateWindows.Location = New System.Drawing.Point(122, 24)
        Me.rbServerCertificateWindows.Name = "rbServerCertificateWindows"
        Me.rbServerCertificateWindows.Size = New System.Drawing.Size(209, 17)
        Me.rbServerCertificateWindows.TabIndex = 32
        Me.rbServerCertificateWindows.TabStop = True
        Me.rbServerCertificateWindows.Text = "Use Windows certificates infrastructure"
        Me.rbServerCertificateWindows.UseVisualStyleBackColor = True
        '
        'label7
        '
        Me.label7.AutoSize = True
        Me.label7.Location = New System.Drawing.Point(6, 3)
        Me.label7.Name = "label7"
        Me.label7.Size = New System.Drawing.Size(90, 26)
        Me.label7.TabIndex = 9
        Me.label7.Text = "Server certificate " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "validation:"
        '
        'tabProxy
        '
        Me.tabProxy.BackColor = System.Drawing.SystemColors.Control
        Me.tabProxy.Controls.Add(Me.pnlProxy)
        Me.tabProxy.Location = New System.Drawing.Point(4, 22)
        Me.tabProxy.Name = "tabProxy"
        Me.tabProxy.Padding = New System.Windows.Forms.Padding(3)
        Me.tabProxy.Size = New System.Drawing.Size(612, 331)
        Me.tabProxy.TabIndex = 1
        Me.tabProxy.Text = "Proxy"
        '
        'pnlProxy
        '
        Me.pnlProxy.Controls.Add(Me.label19)
        Me.pnlProxy.Controls.Add(Me.tbProxyUsername)
        Me.pnlProxy.Controls.Add(Me.label11)
        Me.pnlProxy.Controls.Add(Me.cmbProxyType)
        Me.pnlProxy.Controls.Add(Me.label13)
        Me.pnlProxy.Controls.Add(Me.label14)
        Me.pnlProxy.Controls.Add(Me.tbProxyPassword)
        Me.pnlProxy.Controls.Add(Me.tbProxyPort)
        Me.pnlProxy.Controls.Add(Me.label6)
        Me.pnlProxy.Controls.Add(Me.tbProxyHost)
        Me.pnlProxy.Controls.Add(Me.label12)
        Me.pnlProxy.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlProxy.Location = New System.Drawing.Point(3, 3)
        Me.pnlProxy.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlProxy.Name = "pnlProxy"
        Me.pnlProxy.Size = New System.Drawing.Size(606, 116)
        Me.pnlProxy.TabIndex = 139
        '
        'label19
        '
        Me.label19.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.label19.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.label19.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.label19.Location = New System.Drawing.Point(446, 60)
        Me.label19.Name = "label19"
        Me.label19.Size = New System.Drawing.Size(157, 43)
        Me.label19.TabIndex = 125
        Me.label19.Text = "Leave the 'User name' and 'Password' fields empty when not needed."
        '
        'tbProxyUsername
        '
        Me.tbProxyUsername.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tbProxyUsername.Location = New System.Drawing.Point(99, 57)
        Me.tbProxyUsername.Name = "tbProxyUsername"
        Me.tbProxyUsername.Size = New System.Drawing.Size(339, 20)
        Me.tbProxyUsername.TabIndex = 3
        '
        'label11
        '
        Me.label11.AutoSize = True
        Me.label11.Location = New System.Drawing.Point(3, 7)
        Me.label11.Name = "label11"
        Me.label11.Size = New System.Drawing.Size(59, 13)
        Me.label11.TabIndex = 26
        Me.label11.Text = "Proxy type:"
        '
        'cmbProxyType
        '
        Me.cmbProxyType.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmbProxyType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbProxyType.FormattingEnabled = True
        Me.cmbProxyType.Location = New System.Drawing.Point(99, 4)
        Me.cmbProxyType.Name = "cmbProxyType"
        Me.cmbProxyType.Size = New System.Drawing.Size(339, 21)
        Me.cmbProxyType.TabIndex = 0
        '
        'label13
        '
        Me.label13.AutoSize = True
        Me.label13.Location = New System.Drawing.Point(3, 60)
        Me.label13.Name = "label13"
        Me.label13.Size = New System.Drawing.Size(61, 13)
        Me.label13.TabIndex = 122
        Me.label13.Text = "User name:"
        '
        'label14
        '
        Me.label14.AutoSize = True
        Me.label14.Location = New System.Drawing.Point(3, 86)
        Me.label14.Name = "label14"
        Me.label14.Size = New System.Drawing.Size(56, 13)
        Me.label14.TabIndex = 121
        Me.label14.Text = "Password:"
        '
        'tbProxyPassword
        '
        Me.tbProxyPassword.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tbProxyPassword.Location = New System.Drawing.Point(99, 83)
        Me.tbProxyPassword.Name = "tbProxyPassword"
        Me.tbProxyPassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.tbProxyPassword.Size = New System.Drawing.Size(339, 20)
        Me.tbProxyPassword.TabIndex = 4
        '
        'tbProxyPort
        '
        Me.tbProxyPort.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tbProxyPort.Location = New System.Drawing.Point(474, 31)
        Me.tbProxyPort.Name = "tbProxyPort"
        Me.tbProxyPort.Size = New System.Drawing.Size(129, 20)
        Me.tbProxyPort.TabIndex = 2
        '
        'label6
        '
        Me.label6.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.label6.AutoSize = True
        Me.label6.Location = New System.Drawing.Point(444, 34)
        Me.label6.Name = "label6"
        Me.label6.Size = New System.Drawing.Size(29, 13)
        Me.label6.TabIndex = 27
        Me.label6.Text = "Port:"
        '
        'tbProxyHost
        '
        Me.tbProxyHost.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tbProxyHost.Location = New System.Drawing.Point(99, 31)
        Me.tbProxyHost.Name = "tbProxyHost"
        Me.tbProxyHost.Size = New System.Drawing.Size(339, 20)
        Me.tbProxyHost.TabIndex = 1
        '
        'label12
        '
        Me.label12.AutoSize = True
        Me.label12.Location = New System.Drawing.Point(3, 34)
        Me.label12.Name = "label12"
        Me.label12.Size = New System.Drawing.Size(88, 13)
        Me.label12.TabIndex = 25
        Me.label12.Text = "Proxy host name:"
        '
        'tabServer
        '
        Me.tabServer.BackColor = System.Drawing.SystemColors.Control
        Me.tabServer.Controls.Add(Me.grpCredentials)
        Me.tabServer.Controls.Add(Me.grpConnection)
        Me.tabServer.Location = New System.Drawing.Point(4, 22)
        Me.tabServer.Name = "tabServer"
        Me.tabServer.Padding = New System.Windows.Forms.Padding(3)
        Me.tabServer.Size = New System.Drawing.Size(612, 331)
        Me.tabServer.TabIndex = 0
        Me.tabServer.Text = "Server"
        '
        'grpCredentials
        '
        Me.grpCredentials.BackColor = System.Drawing.Color.Transparent
        Me.grpCredentials.Controls.Add(Me.pnlClientCertificate)
        Me.grpCredentials.Controls.Add(Me.pnlMailbox)
        Me.grpCredentials.Controls.Add(Me.pnlUsernamePassword)
        Me.grpCredentials.Dock = System.Windows.Forms.DockStyle.Top
        Me.grpCredentials.Location = New System.Drawing.Point(3, 114)
        Me.grpCredentials.Name = "grpCredentials"
        Me.grpCredentials.Size = New System.Drawing.Size(606, 136)
        Me.grpCredentials.TabIndex = 1
        Me.grpCredentials.TabStop = False
        Me.grpCredentials.Text = "Credentials"
        '
        'pnlClientCertificate
        '
        Me.pnlClientCertificate.Controls.Add(Me.label8)
        Me.pnlClientCertificate.Controls.Add(Me.btnClearClientCertificate)
        Me.pnlClientCertificate.Controls.Add(Me.tbClientCertificate)
        Me.pnlClientCertificate.Controls.Add(Me.btnViewClientCertificate)
        Me.pnlClientCertificate.Controls.Add(Me.btnImportClientCertificate)
        Me.pnlClientCertificate.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlClientCertificate.Location = New System.Drawing.Point(3, 91)
        Me.pnlClientCertificate.Name = "pnlClientCertificate"
        Me.pnlClientCertificate.Size = New System.Drawing.Size(600, 23)
        Me.pnlClientCertificate.TabIndex = 3
        '
        'label8
        '
        Me.label8.AutoSize = True
        Me.label8.Location = New System.Drawing.Point(3, 5)
        Me.label8.Name = "label8"
        Me.label8.Size = New System.Drawing.Size(85, 13)
        Me.label8.TabIndex = 13
        Me.label8.Text = "Client certificate:"
        '
        'btnClearClientCertificate
        '
        Me.btnClearClientCertificate.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnClearClientCertificate.Location = New System.Drawing.Point(557, 0)
        Me.btnClearClientCertificate.Name = "btnClearClientCertificate"
        Me.btnClearClientCertificate.Size = New System.Drawing.Size(40, 23)
        Me.btnClearClientCertificate.TabIndex = 2
        Me.btnClearClientCertificate.Text = "Clear"
        Me.btnClearClientCertificate.UseVisualStyleBackColor = True
        '
        'tbClientCertificate
        '
        Me.tbClientCertificate.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tbClientCertificate.Location = New System.Drawing.Point(99, 2)
        Me.tbClientCertificate.Name = "tbClientCertificate"
        Me.tbClientCertificate.ReadOnly = True
        Me.tbClientCertificate.Size = New System.Drawing.Size(339, 20)
        Me.tbClientCertificate.TabIndex = 0
        Me.tbClientCertificate.TabStop = False
        '
        'btnViewClientCertificate
        '
        Me.btnViewClientCertificate.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnViewClientCertificate.Location = New System.Drawing.Point(505, 0)
        Me.btnViewClientCertificate.Name = "btnViewClientCertificate"
        Me.btnViewClientCertificate.Size = New System.Drawing.Size(48, 23)
        Me.btnViewClientCertificate.TabIndex = 1
        Me.btnViewClientCertificate.Text = "View..."
        Me.btnViewClientCertificate.UseVisualStyleBackColor = True
        '
        'btnImportClientCertificate
        '
        Me.btnImportClientCertificate.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnImportClientCertificate.Location = New System.Drawing.Point(444, 0)
        Me.btnImportClientCertificate.Name = "btnImportClientCertificate"
        Me.btnImportClientCertificate.Size = New System.Drawing.Size(55, 23)
        Me.btnImportClientCertificate.TabIndex = 0
        Me.btnImportClientCertificate.Text = "Import..."
        Me.btnImportClientCertificate.UseVisualStyleBackColor = True
        '
        'pnlMailbox
        '
        Me.pnlMailbox.Controls.Add(Me.tbMailbox)
        Me.pnlMailbox.Controls.Add(Me.lblMailbox)
        Me.pnlMailbox.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlMailbox.Location = New System.Drawing.Point(3, 68)
        Me.pnlMailbox.Name = "pnlMailbox"
        Me.pnlMailbox.Size = New System.Drawing.Size(600, 23)
        Me.pnlMailbox.TabIndex = 4
        '
        'tbMailbox
        '
        Me.tbMailbox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tbMailbox.Location = New System.Drawing.Point(99, 2)
        Me.tbMailbox.Name = "tbMailbox"
        Me.tbMailbox.Size = New System.Drawing.Size(339, 20)
        Me.tbMailbox.TabIndex = 0
        '
        'lblMailbox
        '
        Me.lblMailbox.AutoSize = True
        Me.lblMailbox.Location = New System.Drawing.Point(3, 5)
        Me.lblMailbox.Name = "lblMailbox"
        Me.lblMailbox.Size = New System.Drawing.Size(82, 13)
        Me.lblMailbox.TabIndex = 13
        Me.lblMailbox.Text = "Shared mailbox:"
        '
        'pnlUsernamePassword
        '
        Me.pnlUsernamePassword.Controls.Add(Me.cbSingleSignOn)
        Me.pnlUsernamePassword.Controls.Add(Me.cbStorePassword)
        Me.pnlUsernamePassword.Controls.Add(Me.tbServerUser)
        Me.pnlUsernamePassword.Controls.Add(Me.label4)
        Me.pnlUsernamePassword.Controls.Add(Me.label5)
        Me.pnlUsernamePassword.Controls.Add(Me.tbServerPassword)
        Me.pnlUsernamePassword.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlUsernamePassword.Location = New System.Drawing.Point(3, 16)
        Me.pnlUsernamePassword.Name = "pnlUsernamePassword"
        Me.pnlUsernamePassword.Size = New System.Drawing.Size(600, 52)
        Me.pnlUsernamePassword.TabIndex = 0
        '
        'cbSingleSignOn
        '
        Me.cbSingleSignOn.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cbSingleSignOn.AutoSize = True
        Me.cbSingleSignOn.Location = New System.Drawing.Point(444, 4)
        Me.cbSingleSignOn.Name = "cbSingleSignOn"
        Me.cbSingleSignOn.Size = New System.Drawing.Size(96, 17)
        Me.cbSingleSignOn.TabIndex = 16
        Me.cbSingleSignOn.TabStop = False
        Me.cbSingleSignOn.Text = "Single Sign On"
        Me.cbSingleSignOn.UseVisualStyleBackColor = True
        '
        'cbStorePassword
        '
        Me.cbStorePassword.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cbStorePassword.AutoSize = True
        Me.cbStorePassword.Location = New System.Drawing.Point(444, 30)
        Me.cbStorePassword.Name = "cbStorePassword"
        Me.cbStorePassword.Size = New System.Drawing.Size(99, 17)
        Me.cbStorePassword.TabIndex = 15
        Me.cbStorePassword.TabStop = False
        Me.cbStorePassword.Text = "Store password"
        Me.cbStorePassword.UseVisualStyleBackColor = True
        '
        'tbServerUser
        '
        Me.tbServerUser.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tbServerUser.Location = New System.Drawing.Point(99, 2)
        Me.tbServerUser.Name = "tbServerUser"
        Me.tbServerUser.Size = New System.Drawing.Size(339, 20)
        Me.tbServerUser.TabIndex = 0
        '
        'label4
        '
        Me.label4.AutoSize = True
        Me.label4.Location = New System.Drawing.Point(3, 5)
        Me.label4.Name = "label4"
        Me.label4.Size = New System.Drawing.Size(61, 13)
        Me.label4.TabIndex = 11
        Me.label4.Text = "User name:"
        '
        'label5
        '
        Me.label5.AutoSize = True
        Me.label5.Location = New System.Drawing.Point(3, 31)
        Me.label5.Name = "label5"
        Me.label5.Size = New System.Drawing.Size(56, 13)
        Me.label5.TabIndex = 11
        Me.label5.Text = "Password:"
        '
        'tbServerPassword
        '
        Me.tbServerPassword.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tbServerPassword.Location = New System.Drawing.Point(99, 28)
        Me.tbServerPassword.Name = "tbServerPassword"
        Me.tbServerPassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.tbServerPassword.Size = New System.Drawing.Size(339, 20)
        Me.tbServerPassword.TabIndex = 1
        '
        'grpConnection
        '
        Me.grpConnection.Controls.Add(Me.pnlHostnameProtocol)
        Me.grpConnection.Dock = System.Windows.Forms.DockStyle.Top
        Me.grpConnection.Location = New System.Drawing.Point(3, 3)
        Me.grpConnection.Name = "grpConnection"
        Me.grpConnection.Size = New System.Drawing.Size(606, 111)
        Me.grpConnection.TabIndex = 0
        Me.grpConnection.TabStop = False
        Me.grpConnection.Text = "Connection"
        '
        'pnlHostnameProtocol
        '
        Me.pnlHostnameProtocol.Controls.Add(Me.tbServerPort)
        Me.pnlHostnameProtocol.Controls.Add(Me.label3)
        Me.pnlHostnameProtocol.Controls.Add(Me.label1)
        Me.pnlHostnameProtocol.Controls.Add(Me.cmbProtocol)
        Me.pnlHostnameProtocol.Controls.Add(Me.tbServerHost)
        Me.pnlHostnameProtocol.Controls.Add(Me.label2)
        Me.pnlHostnameProtocol.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlHostnameProtocol.Location = New System.Drawing.Point(3, 16)
        Me.pnlHostnameProtocol.Margin = New System.Windows.Forms.Padding(0)
        Me.pnlHostnameProtocol.Name = "pnlHostnameProtocol"
        Me.pnlHostnameProtocol.Size = New System.Drawing.Size(600, 55)
        Me.pnlHostnameProtocol.TabIndex = 138
        '
        'tbServerPort
        '
        Me.tbServerPort.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tbServerPort.Location = New System.Drawing.Point(468, 3)
        Me.tbServerPort.Name = "tbServerPort"
        Me.tbServerPort.Size = New System.Drawing.Size(129, 20)
        Me.tbServerPort.TabIndex = 1
        '
        'label3
        '
        Me.label3.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.label3.AutoSize = True
        Me.label3.Location = New System.Drawing.Point(441, 6)
        Me.label3.Name = "label3"
        Me.label3.Size = New System.Drawing.Size(29, 13)
        Me.label3.TabIndex = 27
        Me.label3.Text = "Port:"
        '
        'label1
        '
        Me.label1.AutoSize = True
        Me.label1.Location = New System.Drawing.Point(3, 33)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(49, 13)
        Me.label1.TabIndex = 26
        Me.label1.Text = "Protocol:"
        '
        'cmbProtocol
        '
        Me.cmbProtocol.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmbProtocol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbProtocol.FormattingEnabled = True
        Me.cmbProtocol.Location = New System.Drawing.Point(99, 30)
        Me.cmbProtocol.Name = "cmbProtocol"
        Me.cmbProtocol.Size = New System.Drawing.Size(339, 21)
        Me.cmbProtocol.TabIndex = 2
        '
        'tbServerHost
        '
        Me.tbServerHost.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tbServerHost.Location = New System.Drawing.Point(99, 3)
        Me.tbServerHost.Name = "tbServerHost"
        Me.tbServerHost.Size = New System.Drawing.Size(339, 20)
        Me.tbServerHost.TabIndex = 0
        '
        'label2
        '
        Me.label2.AutoSize = True
        Me.label2.Location = New System.Drawing.Point(3, 6)
        Me.label2.Name = "label2"
        Me.label2.Size = New System.Drawing.Size(61, 13)
        Me.label2.TabIndex = 25
        Me.label2.Text = "Host name:"
        '
        'tabControlMain
        '
        Me.tabControlMain.Controls.Add(Me.tabServer)
        Me.tabControlMain.Controls.Add(Me.tabProxy)
        Me.tabControlMain.Controls.Add(Me.tabSsl)
        Me.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tabControlMain.Location = New System.Drawing.Point(0, 0)
        Me.tabControlMain.Name = "tabControlMain"
        Me.tabControlMain.SelectedIndex = 0
        Me.tabControlMain.Size = New System.Drawing.Size(620, 357)
        Me.tabControlMain.TabIndex = 1008
        '
        'UcConnectionEditor
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.tabControlMain)
        Me.MinimumSize = New System.Drawing.Size(620, 0)
        Me.Name = "UcConnectionEditor"
        Me.Size = New System.Drawing.Size(620, 357)
        Me.tabSsl.ResumeLayout(False)
        Me.panel1.ResumeLayout(False)
        Me.panel1.PerformLayout()
        Me.pnlServerCertificate.ResumeLayout(False)
        Me.pnlServerCertificate.PerformLayout()
        Me.pnlLocalyStoredCertificate.ResumeLayout(False)
        Me.pnlLocalyStoredCertificate.PerformLayout()
        Me.tabProxy.ResumeLayout(False)
        Me.pnlProxy.ResumeLayout(False)
        Me.pnlProxy.PerformLayout()
        Me.tabServer.ResumeLayout(False)
        Me.grpCredentials.ResumeLayout(False)
        Me.pnlClientCertificate.ResumeLayout(False)
        Me.pnlClientCertificate.PerformLayout()
        Me.pnlMailbox.ResumeLayout(False)
        Me.pnlMailbox.PerformLayout()
        Me.pnlUsernamePassword.ResumeLayout(False)
        Me.pnlUsernamePassword.PerformLayout()
        Me.grpConnection.ResumeLayout(False)
        Me.pnlHostnameProtocol.ResumeLayout(False)
        Me.pnlHostnameProtocol.PerformLayout()
        Me.tabControlMain.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private tabSsl As TabPage
    Private pnlServerCertificate As Panel
    Private pnlLocalyStoredCertificate As Panel
    Private label9 As Label
    Private tbServerCertificateThumbprint As TextBox
    Private label10 As Label
    Private WithEvents rbServerCertificateAny As RadioButton
    Private WithEvents rbServerCertificateStored As RadioButton
    Private WithEvents rbServerCertificateWindows As RadioButton
    Private label7 As Label
    Private tabProxy As TabPage
    Private pnlProxy As Panel
    Private label19 As Label
    Private tbProxyUsername As TextBox
    Private label11 As Label
    Private cmbProxyType As ComboBox
    Private label13 As Label
    Private label14 As Label
    Private tbProxyPassword As TextBox
    Private tbProxyPort As TextBox
    Private label6 As Label
    Private tbProxyHost As TextBox
    Private label12 As Label
    Private tabServer As TabPage
    Private grpCredentials As GroupBox
    Private pnlUsernamePassword As Panel
    Private cbStorePassword As CheckBox
    Private tbServerUser As TextBox
    Private label4 As Label
    Private label5 As Label
    Private tbServerPassword As TextBox
    Private grpConnection As GroupBox
    Private pnlHostnameProtocol As Panel
    Private WithEvents tbServerPort As TextBox
    Private label3 As Label
    Private label1 As Label
    Private WithEvents cmbProtocol As ComboBox
    Private WithEvents tbServerHost As TextBox
    Private label2 As Label
    Private tabControlMain As TabControl
    Private pnlClientCertificate As Panel
    Private label8 As Label
    Private WithEvents btnClearClientCertificate As Button
    Private tbClientCertificate As TextBox
    Private WithEvents btnViewClientCertificate As Button
    Private WithEvents btnImportClientCertificate As Button
    Private pnlMailbox As Panel
    Private tbMailbox As TextBox
    Private lblMailbox As Label
    Private panel1 As Panel
    Private cmbSuites As ComboBox
    Private cbAllowTls13 As CheckBox
    Private cbAllowTls12 As CheckBox
    Private cbAllowTls11 As CheckBox
    Private cbAllowTls10 As CheckBox
    Private label23 As Label
    Private label22 As Label
    Private WithEvents cbSingleSignOn As CheckBox
End Class

