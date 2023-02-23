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
Imports Rebex.Mail
Imports Rebex.Mime.Headers
Imports Rebex.Net
Imports Rebex.Samples
Imports System.Threading.Tasks


''' <summary>
''' Summary description for MailForm.
''' </summary>
Public Class MailForm
    Inherits System.Windows.Forms.Form

#Region "Properties"

    Public Shared ReadOnly ConfigFilePath As String = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), String.Format("Rebex{0}Secure Mail{0}MailSendWinForm.xml", Path.DirectorySeparatorChar))

    ''' <summary>
    ''' Configuration object.
    ''' </summary>
    Private _config As Configuration

    ''' <summary>
    ''' SMTP object.
    ''' </summary>
    Private _smtp As Smtp

    ' Allowed TLS/SSL protocol.
    Private _protocol As TlsVersion = SecurityConfig.DefaultTlsVersion

    ' Allowed TLS/SSL cipher suite.
    Private _suite As TlsCipherSuite = TlsCipherSuite.All

    ''' <summary>
    ''' SMTP server hostname.
    ''' </summary>
    Public ReadOnly Property ServerHost() As String
        Get
            Return txtServer.Text
        End Get
    End Property

    ''' <summary>
    ''' SMTP server port.
    ''' </summary>
    Public ReadOnly Property ServerPort() As Integer
        Get
            ' this could throw an exception which is presented to the user...
            Return Integer.Parse(txtPort.Text)
        End Get
    End Property

    ''' <summary>
    ''' Progress dialog.
    ''' </summary>
    Private _progress As Progress

    ''' <summary>
    ''' Attachment filename.
    ''' </summary>
    Public Property AttachmentFilename() As String
        Get
            Return txtAttachmentFilename.Text
        End Get
        Set(ByVal Value As String)
            txtAttachmentFilename.Text = Value

            ' Enables or disables remove attachment button.
            Dim hasAttachment As Boolean = txtAttachmentFilename.Text.Length > 0
            btnRemoveAttachment.Enabled = hasAttachment
        End Set
    End Property

    ''' <summary>
    ''' Used for recovering username and password after 
    ''' turning single sign-on on and off.
    ''' </summary>
    Private _lastUserName, _lastPassword As String
    Private WithEvents radioSendAsyncAwait As System.Windows.Forms.RadioButton

#End Region

#Region "Form constructor and Dispose method"


    ''' <summary>
    ''' Main form constructor. Initializes form and loads configuration.
    ''' </summary>
    Public Sub New()
        InitializeComponent()

        CheckForIllegalCrossThreadCalls = True

        _config = New Configuration(ConfigFilePath)
        _smtp = New Smtp
        AddHandler _smtp.ValidatingCertificate, AddressOf Verifier.ValidatingCertificate

        UpdateMessageSendingStatus(False)

        LoadConfigValues()

#If Not FX45 Then
        radioSendAsyncAwait.Enabled = False
        radioSendAsyncAwait.Text = "Send in the background using async and await keywords. Awailable in .net 4.5 version."
#End If
    End Sub    'New


#End Region

#Region "WinForm controls"

    Private radioSendSync As System.Windows.Forms.RadioButton
    Private radioSendAsync As System.Windows.Forms.RadioButton
    Private radioSendAsyncWithProgress As System.Windows.Forms.RadioButton
    Private txtAttachmentFilename As System.Windows.Forms.TextBox
    Private WithEvents btnSetAttachment As System.Windows.Forms.Button
    Private WithEvents btnRemoveAttachment As System.Windows.Forms.Button
    Private groupBox1 As System.Windows.Forms.GroupBox
    Private label1 As System.Windows.Forms.Label
    Private label2 As System.Windows.Forms.Label
    Private label3 As System.Windows.Forms.Label
    Private groupBox2 As System.Windows.Forms.GroupBox
    Private label4 As System.Windows.Forms.Label
    Private label5 As System.Windows.Forms.Label
    Private label6 As System.Windows.Forms.Label
    Private label7 As System.Windows.Forms.Label
    Private label8 As System.Windows.Forms.Label
    Private label9 As System.Windows.Forms.Label
    Private WithEvents btnSend As System.Windows.Forms.Button
    Private label10 As System.Windows.Forms.Label
    Private pictureBox1 As System.Windows.Forms.PictureBox
    Private label11 As System.Windows.Forms.Label
    Private openFileDialog1 As System.Windows.Forms.OpenFileDialog
    Private txtPort As System.Windows.Forms.TextBox
    Private txtPassword As System.Windows.Forms.TextBox
    Private txtUsername As System.Windows.Forms.TextBox
    Private txtServer As System.Windows.Forms.TextBox
    Private txtSubject As System.Windows.Forms.TextBox
    Private txtBcc As System.Windows.Forms.TextBox
    Private txtCc As System.Windows.Forms.TextBox
    Private txtTo As System.Windows.Forms.TextBox
    Private txtFrom As System.Windows.Forms.TextBox
    Private txtBody As System.Windows.Forms.TextBox
    Private WithEvents btnSettings As System.Windows.Forms.Button
    Private WithEvents cbSecurity As System.Windows.Forms.ComboBox
    Private WithEvents label12 As System.Windows.Forms.Label
    Private WithEvents cbSingleSignOn As System.Windows.Forms.CheckBox

#End Region

#Region "Load and save configuration"


    ''' <summary>
    ''' Loads the configuration.
    ''' </summary>
    Private Sub LoadConfigValues()
        txtServer.Text = _config.GetString("server")
        txtPort.Text = _config.GetInt32("port", 25).ToString()
        txtUsername.Text = _config.GetString("username")
        txtPassword.Text = _config.GetString("password")
        cbSingleSignOn.Checked = _config.GetBoolean("singleSignOn", False)
        txtFrom.Text = _config.GetString("from")
        txtTo.Text = _config.GetString("to")
        txtCc.Text = _config.GetString("cc")
        txtBcc.Text = _config.GetString("bcc")
        txtSubject.Text = _config.GetString("subject")
        cbSecurity.SelectedIndex = _config.GetInt32("security", 0)
        _protocol = CType(_config.GetValue("protocol", GetType(TlsVersion)), TlsVersion)
        _suite = CType(_config.GetValue("suite", GetType(TlsCipherSuite)), TlsCipherSuite)
        If _protocol = 0 Then
            _protocol = SecurityConfig.DefaultTlsVersion
        End If
        If _suite = 0 Then
            _suite = TlsCipherSuite.All
        End If
    End Sub    'LoadConfigValues


    ''' <summary>
    ''' Saves the configuration.
    ''' </summary>
    Private Sub SaveConfigValues()
        _config.SetValue("server", ServerHost)
        _config.SetValue("port", ServerPort)
        _config.SetValue("username", txtUsername.Text)
        _config.SetValue("password", txtPassword.Text)
        _config.SetValue("singleSignOn", cbSingleSignOn.Checked)
        _config.SetValue("from", txtFrom.Text)
        _config.SetValue("to", txtTo.Text)
        _config.SetValue("cc", txtCc.Text)
        _config.SetValue("bcc", txtBcc.Text)
        _config.SetValue("subject", txtSubject.Text)
        _config.SetValue("security", cbSecurity.SelectedIndex)
        _config.SetValue("protocol", _protocol)
        _config.SetValue("suite", _suite)
        _config.Save()
    End Sub    'SaveConfigValues

#End Region

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MailForm))
        Me.groupBox1 = New System.Windows.Forms.GroupBox()
        Me.btnSettings = New System.Windows.Forms.Button()
        Me.cbSecurity = New System.Windows.Forms.ComboBox()
        Me.label12 = New System.Windows.Forms.Label()
        Me.pictureBox1 = New System.Windows.Forms.PictureBox()
        Me.txtPort = New System.Windows.Forms.TextBox()
        Me.cbSingleSignOn = New System.Windows.Forms.CheckBox()
        Me.label10 = New System.Windows.Forms.Label()
        Me.txtPassword = New System.Windows.Forms.TextBox()
        Me.label3 = New System.Windows.Forms.Label()
        Me.txtUsername = New System.Windows.Forms.TextBox()
        Me.label2 = New System.Windows.Forms.Label()
        Me.txtServer = New System.Windows.Forms.TextBox()
        Me.label1 = New System.Windows.Forms.Label()
        Me.groupBox2 = New System.Windows.Forms.GroupBox()
        Me.txtAttachmentFilename = New System.Windows.Forms.TextBox()
        Me.label11 = New System.Windows.Forms.Label()
        Me.btnSetAttachment = New System.Windows.Forms.Button()
        Me.label9 = New System.Windows.Forms.Label()
        Me.txtSubject = New System.Windows.Forms.TextBox()
        Me.label8 = New System.Windows.Forms.Label()
        Me.txtBcc = New System.Windows.Forms.TextBox()
        Me.label6 = New System.Windows.Forms.Label()
        Me.txtCc = New System.Windows.Forms.TextBox()
        Me.label7 = New System.Windows.Forms.Label()
        Me.txtTo = New System.Windows.Forms.TextBox()
        Me.label5 = New System.Windows.Forms.Label()
        Me.txtFrom = New System.Windows.Forms.TextBox()
        Me.label4 = New System.Windows.Forms.Label()
        Me.txtBody = New System.Windows.Forms.TextBox()
        Me.btnRemoveAttachment = New System.Windows.Forms.Button()
        Me.btnSend = New System.Windows.Forms.Button()
        Me.openFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.radioSendSync = New System.Windows.Forms.RadioButton()
        Me.radioSendAsync = New System.Windows.Forms.RadioButton()
        Me.radioSendAsyncWithProgress = New System.Windows.Forms.RadioButton()
        Me.radioSendAsyncAwait = New System.Windows.Forms.RadioButton()
        Me.groupBox1.SuspendLayout()
        CType(Me.pictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.groupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'groupBox1
        '
        Me.groupBox1.Controls.Add(Me.btnSettings)
        Me.groupBox1.Controls.Add(Me.cbSecurity)
        Me.groupBox1.Controls.Add(Me.label12)
        Me.groupBox1.Controls.Add(Me.pictureBox1)
        Me.groupBox1.Controls.Add(Me.txtPort)
        Me.groupBox1.Controls.Add(Me.cbSingleSignOn)
        Me.groupBox1.Controls.Add(Me.label10)
        Me.groupBox1.Controls.Add(Me.txtPassword)
        Me.groupBox1.Controls.Add(Me.label3)
        Me.groupBox1.Controls.Add(Me.txtUsername)
        Me.groupBox1.Controls.Add(Me.label2)
        Me.groupBox1.Controls.Add(Me.txtServer)
        Me.groupBox1.Controls.Add(Me.label1)
        Me.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.groupBox1.Location = New System.Drawing.Point(8, 4)
        Me.groupBox1.Name = "groupBox1"
        Me.groupBox1.Size = New System.Drawing.Size(496, 132)
        Me.groupBox1.TabIndex = 1
        Me.groupBox1.TabStop = False
        Me.groupBox1.Text = "Server settings"
        '
        'btnSettings
        '
        Me.btnSettings.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSettings.Location = New System.Drawing.Point(200, 96)
        Me.btnSettings.Name = "btnSettings"
        Me.btnSettings.Size = New System.Drawing.Size(80, 23)
        Me.btnSettings.TabIndex = 36
        Me.btnSettings.Text = "Settings..."
        '
        'cbSecurity
        '
        Me.cbSecurity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbSecurity.Items.AddRange(New Object() {"No security", "Explicit TLS/SSL", "Implicit TLS/SSL"})
        Me.cbSecurity.Location = New System.Drawing.Point(64, 96)
        Me.cbSecurity.MaxDropDownItems = 3
        Me.cbSecurity.Name = "cbSecurity"
        Me.cbSecurity.Size = New System.Drawing.Size(128, 21)
        Me.cbSecurity.TabIndex = 35
        '
        'label12
        '
        Me.label12.Location = New System.Drawing.Point(8, 96)
        Me.label12.Name = "label12"
        Me.label12.Size = New System.Drawing.Size(56, 24)
        Me.label12.TabIndex = 34
        Me.label12.Text = "Security:"
        '
        'pictureBox1
        '
        Me.pictureBox1.BackColor = System.Drawing.Color.Transparent
        Me.pictureBox1.BackgroundImage = CType(resources.GetObject("pictureBox1.BackgroundImage"), System.Drawing.Image)
        Me.pictureBox1.Location = New System.Drawing.Point(328, 16)
        Me.pictureBox1.Name = "pictureBox1"
        Me.pictureBox1.Size = New System.Drawing.Size(160, 88)
        Me.pictureBox1.TabIndex = 9
        Me.pictureBox1.TabStop = False
        '
        'txtPort
        '
        Me.txtPort.Location = New System.Drawing.Point(248, 24)
        Me.txtPort.Name = "txtPort"
        Me.txtPort.Size = New System.Drawing.Size(50, 20)
        Me.txtPort.TabIndex = 10
        '
        'cbSingleSignOn
        '
        Me.cbSingleSignOn.Location = New System.Drawing.Point(208, 50)
        Me.cbSingleSignOn.Name = "cbSingleSignOn"
        Me.cbSingleSignOn.Size = New System.Drawing.Size(100, 20)
        Me.cbSingleSignOn.TabIndex = 37
        Me.cbSingleSignOn.Text = "Single sign-on"
        '
        'label10
        '
        Me.label10.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.label10.Location = New System.Drawing.Point(208, 24)
        Me.label10.Name = "label10"
        Me.label10.Size = New System.Drawing.Size(40, 24)
        Me.label10.TabIndex = 7
        Me.label10.Text = "Port:"
        '
        'txtPassword
        '
        Me.txtPassword.Location = New System.Drawing.Point(64, 72)
        Me.txtPassword.Name = "txtPassword"
        Me.txtPassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.txtPassword.Size = New System.Drawing.Size(128, 20)
        Me.txtPassword.TabIndex = 30
        '
        'label3
        '
        Me.label3.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.label3.Location = New System.Drawing.Point(8, 72)
        Me.label3.Name = "label3"
        Me.label3.Size = New System.Drawing.Size(56, 24)
        Me.label3.TabIndex = 5
        Me.label3.Text = "Password:"
        '
        'txtUsername
        '
        Me.txtUsername.Location = New System.Drawing.Point(64, 48)
        Me.txtUsername.Name = "txtUsername"
        Me.txtUsername.Size = New System.Drawing.Size(128, 20)
        Me.txtUsername.TabIndex = 20
        '
        'label2
        '
        Me.label2.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.label2.Location = New System.Drawing.Point(8, 48)
        Me.label2.Name = "label2"
        Me.label2.Size = New System.Drawing.Size(56, 24)
        Me.label2.TabIndex = 3
        Me.label2.Text = "Username:"
        '
        'txtServer
        '
        Me.txtServer.Location = New System.Drawing.Point(64, 24)
        Me.txtServer.Name = "txtServer"
        Me.txtServer.Size = New System.Drawing.Size(128, 20)
        Me.txtServer.TabIndex = 0
        '
        'label1
        '
        Me.label1.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.label1.Location = New System.Drawing.Point(8, 24)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(56, 24)
        Me.label1.TabIndex = 1
        Me.label1.Text = "Server:"
        '
        'groupBox2
        '
        Me.groupBox2.Controls.Add(Me.txtAttachmentFilename)
        Me.groupBox2.Controls.Add(Me.label11)
        Me.groupBox2.Controls.Add(Me.btnSetAttachment)
        Me.groupBox2.Controls.Add(Me.label9)
        Me.groupBox2.Controls.Add(Me.txtSubject)
        Me.groupBox2.Controls.Add(Me.label8)
        Me.groupBox2.Controls.Add(Me.txtBcc)
        Me.groupBox2.Controls.Add(Me.label6)
        Me.groupBox2.Controls.Add(Me.txtCc)
        Me.groupBox2.Controls.Add(Me.label7)
        Me.groupBox2.Controls.Add(Me.txtTo)
        Me.groupBox2.Controls.Add(Me.label5)
        Me.groupBox2.Controls.Add(Me.txtFrom)
        Me.groupBox2.Controls.Add(Me.label4)
        Me.groupBox2.Controls.Add(Me.txtBody)
        Me.groupBox2.Controls.Add(Me.btnRemoveAttachment)
        Me.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.groupBox2.Location = New System.Drawing.Point(8, 136)
        Me.groupBox2.Name = "groupBox2"
        Me.groupBox2.Size = New System.Drawing.Size(496, 264)
        Me.groupBox2.TabIndex = 3
        Me.groupBox2.TabStop = False
        Me.groupBox2.Text = "Message composing"
        '
        'txtAttachmentFilename
        '
        Me.txtAttachmentFilename.BackColor = System.Drawing.SystemColors.Window
        Me.txtAttachmentFilename.Location = New System.Drawing.Point(80, 88)
        Me.txtAttachmentFilename.Name = "txtAttachmentFilename"
        Me.txtAttachmentFilename.ReadOnly = True
        Me.txtAttachmentFilename.Size = New System.Drawing.Size(280, 20)
        Me.txtAttachmentFilename.TabIndex = 101
        '
        'label11
        '
        Me.label11.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.label11.Location = New System.Drawing.Point(8, 112)
        Me.label11.Name = "label11"
        Me.label11.Size = New System.Drawing.Size(80, 22)
        Me.label11.TabIndex = 17
        Me.label11.Text = "Message body:"
        '
        'btnSetAttachment
        '
        Me.btnSetAttachment.Location = New System.Drawing.Point(368, 88)
        Me.btnSetAttachment.Name = "btnSetAttachment"
        Me.btnSetAttachment.Size = New System.Drawing.Size(56, 23)
        Me.btnSetAttachment.TabIndex = 100
        Me.btnSetAttachment.Text = "Add"
        '
        'label9
        '
        Me.label9.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.label9.Location = New System.Drawing.Point(8, 88)
        Me.label9.Name = "label9"
        Me.label9.Size = New System.Drawing.Size(64, 24)
        Me.label9.TabIndex = 14
        Me.label9.Text = "Attachment:"
        '
        'txtSubject
        '
        Me.txtSubject.Location = New System.Drawing.Point(80, 64)
        Me.txtSubject.Name = "txtSubject"
        Me.txtSubject.Size = New System.Drawing.Size(408, 20)
        Me.txtSubject.TabIndex = 60
        '
        'label8
        '
        Me.label8.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.label8.Location = New System.Drawing.Point(8, 64)
        Me.label8.Name = "label8"
        Me.label8.Size = New System.Drawing.Size(64, 24)
        Me.label8.TabIndex = 12
        Me.label8.Text = "Subject:"
        '
        'txtBcc
        '
        Me.txtBcc.Location = New System.Drawing.Point(272, 40)
        Me.txtBcc.Name = "txtBcc"
        Me.txtBcc.Size = New System.Drawing.Size(216, 20)
        Me.txtBcc.TabIndex = 90
        '
        'label6
        '
        Me.label6.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.label6.Location = New System.Drawing.Point(216, 40)
        Me.label6.Name = "label6"
        Me.label6.Size = New System.Drawing.Size(56, 23)
        Me.label6.TabIndex = 10
        Me.label6.Text = "Bcc:"
        '
        'txtCc
        '
        Me.txtCc.Location = New System.Drawing.Point(272, 16)
        Me.txtCc.Name = "txtCc"
        Me.txtCc.Size = New System.Drawing.Size(216, 20)
        Me.txtCc.TabIndex = 80
        '
        'label7
        '
        Me.label7.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.label7.Location = New System.Drawing.Point(216, 16)
        Me.label7.Name = "label7"
        Me.label7.Size = New System.Drawing.Size(56, 24)
        Me.label7.TabIndex = 8
        Me.label7.Text = "Cc:"
        '
        'txtTo
        '
        Me.txtTo.Location = New System.Drawing.Point(80, 40)
        Me.txtTo.Name = "txtTo"
        Me.txtTo.Size = New System.Drawing.Size(112, 20)
        Me.txtTo.TabIndex = 50
        '
        'label5
        '
        Me.label5.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.label5.Location = New System.Drawing.Point(8, 40)
        Me.label5.Name = "label5"
        Me.label5.Size = New System.Drawing.Size(64, 24)
        Me.label5.TabIndex = 6
        Me.label5.Text = "To:"
        '
        'txtFrom
        '
        Me.txtFrom.Location = New System.Drawing.Point(80, 16)
        Me.txtFrom.Name = "txtFrom"
        Me.txtFrom.Size = New System.Drawing.Size(112, 20)
        Me.txtFrom.TabIndex = 40
        '
        'label4
        '
        Me.label4.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.label4.Location = New System.Drawing.Point(8, 16)
        Me.label4.Name = "label4"
        Me.label4.Size = New System.Drawing.Size(64, 24)
        Me.label4.TabIndex = 4
        Me.label4.Text = "From:"
        '
        'txtBody
        '
        Me.txtBody.Location = New System.Drawing.Point(8, 136)
        Me.txtBody.Multiline = True
        Me.txtBody.Name = "txtBody"
        Me.txtBody.Size = New System.Drawing.Size(480, 120)
        Me.txtBody.TabIndex = 70
        '
        'btnRemoveAttachment
        '
        Me.btnRemoveAttachment.Enabled = False
        Me.btnRemoveAttachment.Location = New System.Drawing.Point(432, 88)
        Me.btnRemoveAttachment.Name = "btnRemoveAttachment"
        Me.btnRemoveAttachment.Size = New System.Drawing.Size(56, 23)
        Me.btnRemoveAttachment.TabIndex = 100
        Me.btnRemoveAttachment.Text = "Remove"
        '
        'btnSend
        '
        Me.btnSend.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.btnSend.Location = New System.Drawing.Point(408, 408)
        Me.btnSend.Name = "btnSend"
        Me.btnSend.Size = New System.Drawing.Size(96, 72)
        Me.btnSend.TabIndex = 110
        Me.btnSend.Text = "Send"
        '
        'radioSendSync
        '
        Me.radioSendSync.Location = New System.Drawing.Point(8, 408)
        Me.radioSendSync.Name = "radioSendSync"
        Me.radioSendSync.Size = New System.Drawing.Size(384, 24)
        Me.radioSendSync.TabIndex = 111
        Me.radioSendSync.Text = "Simple send - blocks UI until the operation is finished."
        '
        'radioSendAsync
        '
        Me.radioSendAsync.Checked = True
        Me.radioSendAsync.Location = New System.Drawing.Point(8, 432)
        Me.radioSendAsync.Name = "radioSendAsync"
        Me.radioSendAsync.Size = New System.Drawing.Size(384, 24)
        Me.radioSendAsync.TabIndex = 111
        Me.radioSendAsync.TabStop = True
        Me.radioSendAsync.Text = "Send in the background. UI is not blocked."
        '
        'radioSendAsyncWithProgress
        '
        Me.radioSendAsyncWithProgress.Location = New System.Drawing.Point(8, 456)
        Me.radioSendAsyncWithProgress.Name = "radioSendAsyncWithProgress"
        Me.radioSendAsyncWithProgress.Size = New System.Drawing.Size(392, 24)
        Me.radioSendAsyncWithProgress.TabIndex = 111
        Me.radioSendAsyncWithProgress.Text = "Send in the background. UI is not blocked. Progress bar is displayed."
        '
        'radioSendAsyncAwait
        '
        Me.radioSendAsyncAwait.Location = New System.Drawing.Point(8, 486)
        Me.radioSendAsyncAwait.Name = "radioSendAsyncAwait"
        Me.radioSendAsyncAwait.Size = New System.Drawing.Size(392, 34)
        Me.radioSendAsyncAwait.TabIndex = 112
        Me.radioSendAsyncAwait.Text = "Send in the background using async and await keywords. UI is not blocked."
        '
        'MailForm
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(514, 532)
        Me.Controls.Add(Me.radioSendAsyncAwait)
        Me.Controls.Add(Me.radioSendSync)
        Me.Controls.Add(Me.btnSend)
        Me.Controls.Add(Me.groupBox2)
        Me.Controls.Add(Me.groupBox1)
        Me.Controls.Add(Me.radioSendAsync)
        Me.Controls.Add(Me.radioSendAsyncWithProgress)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "MailForm"
        Me.Text = "Rebex Mail Message WinForm Sender"
        Me.groupBox1.ResumeLayout(False)
        Me.groupBox1.PerformLayout()
        CType(Me.pictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.groupBox2.ResumeLayout(False)
        Me.groupBox2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub    'InitializeComponent 
#End Region

#Region "Message sending methods"


    Private Sub btnSend_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSend.Click
        SaveConfigValues()

        ' create mail message from values in the form
        Dim message As MailMessage = GetMailMessage()

        If radioSendSync.Checked Then

            ' Send the message using Smpt.Send method.
            ' 
            ' The UI is blocked until the transfer is finished.
            '
            ' This is very simple, but the window is not responsive during
            ' the operation. Even though, it is good enough when the messages
            ' are not long and the connection to he SMTP server is fast and reliable.
            SendMessageSync(message)

        ElseIf radioSendAsync.Checked Then

            ' Send the message asynchronously using Smtp.BeginSend and Smtp.EndSend methods.
            ' 
            ' The UI is not blocked, as the message is send asynchronously in the background.
            ' The main windows is notified when transfer is finished.
            SendMessageAsync(message)

        ElseIf radioSendAsyncWithProgress.Checked Then

            ' Send the message asynchronously using Smtp.BeginSend and Smtp.EndSend methods
            ' and display a dialog with a progress bar.
            ' 
            ' The UI is not blocked, as the message is send asynchronously in the background.
            ' The main windows is notified when transfer is finished, and the progress window
            ' is notified when a block of data is transferred.
            SendMessageWithProgressDialog(message)

#If FX45 Then
        ElseIf radioSendAsyncAwait.Checked Then
            SendMessageUsingAwait(message)

#End If

        End If
    End Sub 'btnSend_Click

#If FX45 Then
    Private Async Sub SendMessageUsingAwait(ByVal message As MailMessage)
        Try
            Await ConnectAndLoginAsync()

            PrepareProgress(message)

            UpdateMessageSendingStatus(True)

            Await _smtp.SendAsync(message)

            FinishProgress()
        Catch ex As Exception
            ' close progress dialog
            If (_progress IsNot Nothing) Then
                _progress.Unbind(_smtp)
                _progress.Close()
                _progress = Nothing
            End If

            DisplayError(ex)
        Finally
            Disconnect()
        End Try
    End Sub

    Private Async Sub Disconnect()
        Try
            ' close the connection
            Await _smtp.DisconnectAsync()

        Catch x As Exception
            DisplayError(x)
        Finally
            ' enable the send button
            UpdateMessageSendingStatus(False)
        End Try
    End Sub

#End If

    ''' <summary>
    ''' Send the message synchronously. Returns when the whole message is transferred to SMTP server.
    ''' </summary>
    ''' <param name="message"></param>
    Private Sub SendMessageSync(ByVal message As MailMessage)
        Dim lastCursor As Cursor = Me.Cursor
        Try
            ' set the wait cursor
            Me.Cursor = Cursors.WaitCursor

            ' connect and login
            ConnectAndLogin()

            ' send message without progress dialog
            _smtp.Send(message)

            MessageBox.Show("Mail message was sent successfully.")
        Catch x As Exception
            DisplayError(x)
        Finally
            ' hide wait cursor
            Me.Cursor = lastCursor

            ' close the connection
            _smtp.Disconnect()
        End Try
    End Sub 'SendMessageSync



    ''' <summary>
    ''' Send the message asynchronously in the background. After the transfer is finished,
    ''' the appropriate delegate is called depending on asynchronous API.
    ''' </summary>
    ''' <param name="message"></param>
    Private Sub SendMessageAsync(ByVal message As MailMessage)
        Try
            ' connect to the smtp server
            ConnectAndLogin()

            ' disable buttons
            UpdateMessageSendingStatus(True)

            ' Start sending... 
            _smtp.SendAsync(message).ContinueWith(AddressOf SendCompleted)
        Catch x As Exception
            ' close the connection
            _smtp.Disconnect()

            ' enable the send button
            UpdateMessageSendingStatus(False)

            ' re-throw the exception
            DisplayError(x)
        End Try
    End Sub 'SendMessageAsync


    ''' <summary>
    ''' Send the message asynchronously in the background and show transfer progress dialog.
    ''' After the transfer is finished, the appropriate delegate is called depending on asynchronous API.
    ''' </summary>
    ''' <param name="message"></param>
    Protected Sub SendMessageWithProgressDialog(ByVal message As MailMessage)
        Try
            ' connect to smtp server            
            ConnectAndLogin()

            PrepareProgress(message)

            UpdateMessageSendingStatus(True)

            ' Start sending...
            _smtp.SendAsync(message).ContinueWith(AddressOf SendCompleted)
        Catch x As Exception
            ' close the connection
            _smtp.Disconnect()

            ' enable the send button
            UpdateMessageSendingStatus(False)

            ' close progress dialog
            If Not (_progress Is Nothing) Then
                _progress.Unbind(_smtp)
                _progress.Close()
                _progress = Nothing
            End If

            DisplayError(x)
        End Try
    End Sub 'SendMessageWithProgressDialog

#End Region

#Region "Methods and delagates for handling asynchronous results"
    Private Sub SendCompleted(ByVal sendTask As Task)

        If Me.InvokeRequired Then
            Invoke(New Action(Of Task)(AddressOf SendCompleted), New Object() {sendTask})
            Return
        End If
        Try
            ' unbind progress dialog from the Smtp object's events
            If Not (_progress Is Nothing) Then
                _progress.Unbind(_smtp)
            End If
            If sendTask.IsFaulted Then
                Throw sendTask.Exception
            End If
            If _progress Is Nothing Then
                ' show message progress bar is not visible
                MessageBox.Show("Mail message was sent successfully.")
            Else
                ' indicate success on progress bar window,
                ' then forget about it - it will stay around until
                ' it is closed by the user
                _progress.SetFinished()
                _progress = Nothing
            End If
        Catch x As Exception
            If Not (_progress Is Nothing) Then
                _progress.Close()
                _progress = Nothing
            End If

            DisplayError(x)
        Finally
            ' close the connection
            _smtp.Disconnect()

            ' enable the send button
            UpdateMessageSendingStatus(False)
        End Try
    End Sub    'AsyncSendCompleted

#End Region

#Region "Helper methods"


    ''' <summary>
    ''' Returns mail message constructed from WinForms properties.
    ''' </summary>
    Public Function GetMailMessage() As MailMessage
        ' create message object
        Dim message As New MailMessage

        ' fill message properties
        message.From = New MailAddressCollection(txtFrom.Text)
        message.To = New MailAddressCollection(txtTo.Text)
        message.CC = New MailAddressCollection(txtCc.Text)
        message.Bcc = New MailAddressCollection(txtBcc.Text)
        message.Subject = txtSubject.Text
        message.BodyText = txtBody.Text

        ' add attachment if needed
        If AttachmentFilename.Length > 0 Then
            message.Attachments.Add(New Attachment(AttachmentFilename))
        End If

        Return message
    End Function 'GetMailMessage


    ''' <summary>
    ''' Connects to SMTP server. Logins with username and password if needed.
    ''' </summary>
    Public Sub ConnectAndLogin()
        Dim security As SslMode = Nothing

        PrepareConnect(security)

        ' connect to the server
        _smtp.Connect(ServerHost, ServerPort, security)

        If cbSingleSignOn.Checked Then
            ' single sign-on authentication
            _smtp.Login(SmtpAuthentication.Auto)
        ElseIf txtUsername.Text.Length > 0 Then
            ' login if needed
            _smtp.Login(txtUsername.Text, txtPassword.Text)
        End If

    End Sub 'ConnectAndLogin 

#If FX45 Then
    Private Async Function ConnectAndLoginAsync() As Task
        Dim security As SslMode = Nothing

        PrepareConnect(security)

        ' connect to the server
        Await _smtp.ConnectAsync(ServerHost, ServerPort, security)

        If cbSingleSignOn.Checked Then
            ' single sign-on authentication
            Await _smtp.LoginAsync(SmtpAuthentication.Auto)
        ElseIf txtUsername.Text.Length > 0 Then
            ' login if needed
            Await _smtp.LoginAsync(txtUsername.Text, txtPassword.Text, SmtpAuthentication.Auto)
        End If
    End Function
#End If

    Private Sub PrepareConnect(ByRef security As SslMode)
        Select Case cbSecurity.SelectedIndex
            Case 1
                security = SslMode.Explicit
            Case 2
                security = SslMode.Implicit
            Case Else
                security = SslMode.None
        End Select

        _smtp.Settings.SslAllowedVersions = CType(_config.GetValue("protocol", GetType(TlsVersion)), TlsVersion)
        _smtp.Settings.SslAllowedSuites = CType(_config.GetValue("suite", GetType(TlsCipherSuite)), TlsCipherSuite)
    End Sub

    Private Sub PrepareProgress(ByVal message As MailMessage)
        ' create and show progress dialog bound to the Smtp object's events
        _progress = New Progress(_smtp)
        _progress.Show()

        ' write message to memory stream to determine its total length
        Dim stream As New MemoryStream
        message.Save(stream)
        ' set message length (and initialize progress bar)                
        _progress.MessageLength = stream.Length
    End Sub

    Private Sub FinishProgress()
        If (_progress IsNot Nothing) Then
            _progress.Unbind(_smtp)
            ' indicate success on progress bar window,
            ' then forget about it - it will stay around until
            ' it is closed by the user
            _progress.SetFinished()
            _progress = Nothing
        End If
    End Sub
#End Region

#Region "UI helper methods - button click handlers and helper methods."


    ''' <summary>
    ''' Updates UI - disables/enables "Send" button and changes it's text.
    ''' </summary>
    ''' <param name="messageIsSending"></param>
    Public Sub UpdateMessageSendingStatus(ByVal messageIsSending As Boolean)
        If messageIsSending Then
            btnSend.Enabled = False
            btnSend.Text = "Sending..."
        Else
            btnSend.Enabled = True
            btnSend.Text = "Send"
        End If
    End Sub    'UpdateMessageSendingStatus


    ''' <summary>
    ''' Set attachment filename.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnSetAttachment_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSetAttachment.Click
        openFileDialog1.Filter = "All files (*.*)|*.*"
        openFileDialog1.ShowDialog()

        Dim fileName As String = openFileDialog1.FileName

        If fileName.Length > 0 Then
            AttachmentFilename = fileName
        End If
    End Sub    'btnSetAttachment_Click


    ''' <summary>
    ''' Unset attachment filename.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnRemoveAttachment_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRemoveAttachment.Click
        AttachmentFilename = ""
    End Sub    'btnRemoveAttachment_Click


    ''' <summary>
    ''' Display error message based on an exception to the user.
    ''' </summary>
    ''' <param name="x"></param>
    Private Sub DisplayError(ByVal x As Exception)
        Dim aggregateEx As AggregateException = TryCast(x, AggregateException)
        If aggregateEx IsNot Nothing Then
            For Each ex As Exception In aggregateEx.InnerExceptions
                DisplayError(ex)
            Next
            Return
        End If
        If TypeOf x Is SmtpException Then
            Dim sx As SmtpException = CType(x, SmtpException)
            If sx.Status = SmtpExceptionStatus.AsyncError Then
                x = sx.InnerException
            End If
        End If

        Dim message As String = _
            x.Message + ControlChars.Cr + ControlChars.Lf & _
            "------------------------------------------------------" & ControlChars.Cr & ControlChars.Lf & _
            x.ToString()
        MessageBox.Show(Me, message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub    'DisplayError


    ''' <summary>
    ''' Security settings.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnSettings_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSettings.Click
        Dim config As New SecurityConfig

        config.Protocol = _protocol
        config.AllowedSuite = _suite
        If config.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            _protocol = config.Protocol
            _suite = config.AllowedSuite
        End If
    End Sub

    ''' <summary>
    ''' Single sign on CheckBox changed.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param> 
    Private Sub cbSingleSignOn_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cbSingleSignOn.CheckedChanged
        If cbSingleSignOn.Checked Then
            ' turning on single sign
            ' save password and username for later use without single sign on
            _lastUserName = txtUsername.Text
            _lastPassword = txtPassword.Text
            ' hide the username and password from GUI
            txtPassword.Text = String.Empty
            txtUsername.Text = String.Empty
            txtPassword.Enabled = False
            txtUsername.Enabled = False
        Else
            ' disabling single sign on
            ' restore the saved username and password
            txtUsername.Text = _lastUserName
            txtPassword.Text = _lastPassword
            txtPassword.Enabled = True
            txtUsername.Enabled = True
        End If
    End Sub

#End Region

End Class 'MailForm
