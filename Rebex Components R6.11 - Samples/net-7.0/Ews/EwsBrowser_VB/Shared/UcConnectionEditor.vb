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

Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Security.Cryptography.X509Certificates
Imports System.Windows.Forms
Imports Rebex.Net
Imports Rebex.Security.Certificates


Partial Public Class UcConnectionEditor
    Inherits UserControl
#Region "Private variables"

    ''' <summary>
    ''' A protocol "before selection has changed" is remembered here.
    ''' </summary>
    Private _previousProtocol As ProtocolMode

#End Region

#Region "Events"

    ''' <summary>
    ''' Occurs when height of inner controls has been recalculated according to current visibility of their content.
    ''' </summary>		
    <Description("Occurs when height of inner controls has been recalculated according to current visibility of their content.")> _
    Public Event HeightRecalculated As EventHandler

    ''' <summary>
    ''' Occurs when rearranging within some inner control starts.<br/>
    ''' Can be used as an extension point for adding new inner controls to the sender.
    ''' </summary>
    Protected Event ControlsRearranging As EventHandler

    ''' <summary>
    ''' Occurs when rearranging within some inner control is finished.<br/>
    ''' Can be used as an extension point for adding new inner controls to the sender.
    ''' </summary>
    Protected Event ControlsRearranged As EventHandler

#End Region

#Region "Constructor"
    Public Sub New()
        InitializeComponent()

        ' design-time binding should be done in constructor (regardless filtering)
        If LicenseManager.UsageMode = LicenseUsageMode.Designtime Then
            BindComboBoxes()
        End If
    End Sub
#End Region

#Region "Field values properties"

#Region "Connection"

    ''' <summary>
    ''' Gets or sets server host address.
    ''' </summary>
    <Browsable(False)> _
    Public Property ServerHost() As String
        Get
            Return tbServerHost.Text
        End Get
        Set(value As String)
            tbServerHost.Text = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets server port.
    ''' </summary>
    <Browsable(False)> _
    Public Property ServerPort() As Integer
        Get
            Dim ret As Integer = 0
            Integer.TryParse(tbServerPort.Text, ret)
            Return ret
        End Get
        Set(value As Integer)
            tbServerPort.Text = If((value = 0), "", value.ToString())
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets used file transfer protocol mode.
    ''' </summary>
    <Browsable(False)> _
    Public Property Protocol() As ProtocolMode
        Get
            ' if nothing is selected...
            If cmbProtocol.SelectedValue Is Nothing Then
                cmbProtocol.SelectedIndex = 0
            End If
            ' ... select the first item
            Return CType(cmbProtocol.SelectedValue, ProtocolMode)
        End Get
        Set(value As ProtocolMode)
            cmbProtocol.SelectedValue = value
        End Set
    End Property

#End Region

#Region "Credentials"

    ''' <summary>
    ''' Gets or sets server login name.
    ''' </summary>
    <Browsable(False)> _
    Public Property ServerUser() As String
        Get
            Return tbServerUser.Text
        End Get
        Set(value As String)
            tbServerUser.Text = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets server login password.
    ''' </summary>
    <Browsable(False)> _
    Public Property ServerPassword() As String
        Get
            Return tbServerPassword.Text
        End Get
        Set(value As String)
            tbServerPassword.Text = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets a value, indicating whether the password can be persisted to a file (outside the control). 
    ''' </summary>
    <Browsable(False)> _
    Public Property StorePassword() As Boolean
        Get
            Return cbStorePassword.Checked
        End Get
        Set(value As Boolean)
            cbStorePassword.Checked = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets a value, indicating whether to use Single Sign On. 
    ''' </summary>
    <Browsable(False)> _
    Public Property UseSingleSignOn() As Boolean
        Get
            Return cbSingleSignOn.Checked
        End Get
        Set(value As Boolean)
            cbSingleSignOn.Checked = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets shared mailbox.
    ''' </summary>
    <Browsable(False)> _
    Public Property Mailbox() As String
        Get
            Return tbMailbox.Text
        End Get
        Set(value As String)
            tbMailbox.Text = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets filename used to load a client certificate for SSL authentication.
    ''' </summary>
    <Browsable(True)> _
    Public Property ClientCertificateFilename() As String
        Get
            Return _clientCertificateFilename
        End Get
        Set(value As String)
            _clientCertificateFilename = value
            RefreshClientCertificateField()
        End Set
    End Property
    Private _clientCertificateFilename As String = Nothing

    ''' <summary>
    ''' Gets or sets client certificate used for SSL authentication.
    ''' </summary>
    <Browsable(False)> _
    Public Property ClientCertificate() As CertificateChain
        Get
            Return _clientCertificate
        End Get
        Set(value As CertificateChain)
            _clientCertificate = value
            RefreshClientCertificateField()
        End Set
    End Property
    Private _clientCertificate As CertificateChain = Nothing

#End Region

#Region "Proxy"

    ''' <summary>
    ''' Gets or sets used proxy type (or <see cref="FileTransferProxyType.None"/> when no proxy is used).
    ''' </summary>
    <Browsable(False)> _
    Public Property ProxyType() As ProxyType
        Get
            ' if nothing is selected...
            If cmbProxyType.SelectedValue Is Nothing Then
                If cmbProxyType.Items Is Nothing OrElse cmbProxyType.Items.Count <= 0 Then
                    Return ProxyType.None
                End If
                ' ... and nothing can be selected -> just return the default
                ' ... and something can be selected -> select the first item as a side effect
                cmbProxyType.SelectedIndex = 0
            End If
            Return DirectCast(cmbProxyType.SelectedValue, ProxyType)
        End Get
        Set(value As ProxyType)
            cmbProxyType.SelectedValue = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets proxy host address.
    ''' </summary>
    <Browsable(False)> _
    Public Property ProxyHost() As String
        Get
            Return tbProxyHost.Text
        End Get
        Set(value As String)
            tbProxyHost.Text = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets proxy port.
    ''' </summary>
    <Browsable(False)> _
    Public Property ProxyPort() As Integer
        Get
            Dim ret As Integer = 0
            Integer.TryParse(tbProxyPort.Text, ret)
            Return ret
        End Get
        Set(value As Integer)
            tbProxyPort.Text = If((value = 0), "", value.ToString())
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets proxy login name.
    ''' </summary>
    <Browsable(False)> _
    Public Property ProxyUser() As String
        Get
            Return tbProxyUsername.Text
        End Get
        Set(value As String)
            tbProxyUsername.Text = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets proxy login password.
    ''' </summary>
    <Browsable(False)> _
    Public Property ProxyPassword() As String
        Get
            Return tbProxyPassword.Text
        End Get
        Set(value As String)
            tbProxyPassword.Text = value
        End Set
    End Property

#End Region

#Region "SSL specifics"

    ''' <summary>
    ''' Gets or sets hash value used for validating SSL server certificate 
    ''' </summary>
    <Browsable(False)> _
    Public Property ServerCertificateVerifyingMode() As CertificateVerifyingMode
        Get
            If rbServerCertificateAny.Checked Then
                Return CertificateVerifyingMode.AcceptAnyCertificate
            End If
            If rbServerCertificateWindows.Checked Then
                Return CertificateVerifyingMode.UseWindowsInfrastructure
            End If
            If rbServerCertificateStored.Checked Then
                Return CertificateVerifyingMode.LocalyStoredThumbprint
            End If
            If LicenseManager.UsageMode = LicenseUsageMode.Designtime Then
                Return CType(0, CertificateVerifyingMode)
            End If
            ' we should not throw exception in design time - let's return some invalid value instead
            Throw New InvalidOperationException("No CertfificateVerifyingMode has been chosen.")
        End Get
        Set(value As CertificateVerifyingMode)
            Select Case value
                Case CertificateVerifyingMode.AcceptAnyCertificate
                    rbServerCertificateAny.Checked = True
                    Exit Select
                Case CertificateVerifyingMode.UseWindowsInfrastructure
                    rbServerCertificateWindows.Checked = True
                    Exit Select
                Case CertificateVerifyingMode.LocalyStoredThumbprint
                    rbServerCertificateStored.Checked = True
                    Exit Select
                Case Else
                    Throw New InvalidOperationException(String.Format("Unexpected enum value '{0}'", value))
            End Select
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets hash value used for validating SSL server certificate 
    ''' (when <see cref="ServerCertificateVerifyingMode"/> is <c>LocalyStoredThumbprint</c>).
    ''' </summary>
    <Browsable(False)> _
    Public Property ServerCertificateThumbprint() As String
        Get
            Return tbServerCertificateThumbprint.Text
        End Get
        Set(value As String)
            tbServerCertificateThumbprint.Text = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets TLS/SSL protocol version.
    ''' </summary>
    Public Property TlsProtocol() As TlsVersion
        Get
            Dim version As TlsVersion = TlsVersion.None
            If cbAllowTls13.Checked Then
                version = version Or TlsVersion.TLS13
            End If
            If cbAllowTls12.Checked Then
                version = version Or TlsVersion.TLS12
            End If
            If cbAllowTls11.Checked Then
                version = version Or TlsVersion.TLS11
            End If
            If cbAllowTls10.Checked Then
                version = version Or TlsVersion.TLS10
            End If
            Return version
        End Get
        Set(value As TlsVersion)
            cbAllowTls13.Checked = ((value And TlsVersion.TLS13) <> 0)
            cbAllowTls12.Checked = ((value And TlsVersion.TLS12) <> 0)
            cbAllowTls11.Checked = ((value And TlsVersion.TLS11) <> 0)
            cbAllowTls10.Checked = ((value And TlsVersion.TLS10) <> 0)
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets allowed secure suites.
    ''' </summary>
    <Browsable(False)> _
    Public Property AllowedSuite() As TlsCipherSuite
        Get
            If cmbSuites.SelectedIndex = 0 Then
                Return TlsCipherSuite.All
            Else
                Return TlsCipherSuite.Secure
            End If
        End Get
        Set(value As TlsCipherSuite)
            If value = TlsCipherSuite.All Then
                cmbSuites.SelectedIndex = 0
            Else
                cmbSuites.SelectedIndex = 1
            End If
        End Set
    End Property

#End Region

#Region "Transform from/to ConnectionData"

    Public Sub GetData(data As ConnectionData)
        If data Is Nothing Then
            Throw New ArgumentNullException("data")
        End If

        data.ProxyHost = ProxyHost
        data.ProxyPassword = ProxyPassword
        data.ProxyPort = ProxyPort
        data.ProxyType = ProxyType
        data.ProxyUser = ProxyUser
        data.Protocol = Protocol
        data.ServerCertificateThumbprint = ServerCertificateThumbprint
        data.ServerCertificateVerifyingMode = ServerCertificateVerifyingMode
        data.ClientCertificateFilename = ClientCertificateFilename
        data.ClientCertificate = ClientCertificate
        data.ServerHost = ServerHost
        data.ServerPassword = ServerPassword
        data.ServerPort = ServerPort
        data.ServerUser = ServerUser
        data.StorePassword = StorePassword
        data.UseSingleSignOn = UseSingleSignOn
        data.Mailbox = Mailbox
        data.AllowedSuite = AllowedSuite
        data.TlsProtocol = TlsProtocol
    End Sub

    Public Sub SetData(data As ConnectionData)
        If data Is Nothing Then
            Throw New ArgumentNullException("data")
        End If

        Protocol = data.Protocol

        ServerHost = data.ServerHost
        ServerPort = data.ServerPort
        ServerPassword = data.ServerPassword
        UseSingleSignOn = data.UseSingleSignOn
        ServerUser = data.ServerUser
        Mailbox = data.Mailbox

        ProxyHost = data.ProxyHost
        ProxyPassword = data.ProxyPassword
        ProxyPort = data.ProxyPort
        ProxyType = data.ProxyType
        ProxyUser = data.ProxyUser

        ServerCertificateVerifyingMode = data.ServerCertificateVerifyingMode
        ServerCertificateThumbprint = data.ServerCertificateThumbprint
        ClientCertificateFilename = data.ClientCertificateFilename

        StorePassword = data.StorePassword

        AllowedSuite = data.AllowedSuite
        TlsProtocol = data.TlsProtocol
    End Sub

#End Region

#End Region

#Region "Visibility and accessibility properties"

    ''' <summary>
    ''' Gets information, whether GUI for settings of SSL client certificate is currently enabled.
    ''' </summary>
    <Browsable(False)> _
    Public ReadOnly Property IsClientCertificateEnabled() As Boolean
        Get
            Return Protocol = ProtocolMode.HTTPS
        End Get
    End Property

#End Region

#Region "Validation"

    ''' <summary>
    ''' Validates all static rules. Returns collection of errors.
    ''' </summary>
    ''' <returns>Collection of error messages or empty collection on success.</returns>
    Public Function GetValidationErrors() As List(Of String)
        Dim errors As New List(Of String)()

        If String.IsNullOrEmpty(ServerHost) Then
            errors.Add("Server host name is a required field.")
        End If
        If ServerPort <= 0 Then
            errors.Add("Server port is a required non-zero field.")
        End If

        If ProxyType <> ProxyType.None Then
            If String.IsNullOrEmpty(ProxyHost) Then
                errors.Add("Proxy host name is a required field.")
            End If
            If ProxyPort <= 0 Then
                errors.Add("Proxy port is a required non-zero field.")

                'if (FileTransferMode == FileTransferMode.FtpSslExplicit || FileTransferMode == FileTransferMode.FtpSslImplicit)
                '{
                '	FileTransferProxyType[] proxyWhenSecure = new FileTransferProxyType[] { FileTransferProxyType.Socks4, FileTransferProxyType.Socks4a, FileTransferProxyType.Socks5, FileTransferProxyType.HttpConnect };				
                '	bool ok = false;

                'for (int i = 0; i < proxyWhenSecure.Length; i++)
                '{
                '	if (proxyWhenSecure[i] == ProxyType) 
                '		ok = true;
                '}

                'if (!ok)
                '	errors.Add("This proxy type is not allowed when using SSL.");
                '}
            End If
        End If

        Return errors
    End Function

#End Region

#Region "GUI behavior methods"

    ''' <summary>
    ''' Recomputes visibility of inner controls and values according to <see cref="HideFtpGui"/> and <see cref="HideSftpGui"/>,
    ''' then removes all invisible controls from this user control 
    ''' and adjusts height both of the user control and of the parent form where the user control is placed in.
    ''' </summary>
    Public Sub AdaptToCurrentVisibility()
        ' don't execute this method in design-time
        If LicenseManager.UsageMode = LicenseUsageMode.Designtime Then
            Return
        End If

        BindComboBoxes()

        RecomputeProtocolSpecificFields()

        If ParentForm Is Nothing Then
            Throw New InvalidOperationException("AdaptToCurrentVisibility can be called only if the user control is placed into some ParentForm.")
        End If

        ' store current anchor settings and clear bottom and right anchor (that anchors would mess-up the resizing)
        Dim origAnchor As AnchorStyles = Anchor
        Anchor = AnchorStyles.Left Or AnchorStyles.Top

        Dim heightDelta As Integer = ParentForm.Height - Height
        RearrangeVisibleControls()

        ParentForm.Size = New Size(ParentForm.Width, Height + heightDelta)
        ParentForm.MinimumSize = New Size(ParentForm.MinimumSize.Width, ParentForm.Height)

        Anchor = origAnchor
    End Sub

    ''' <summary>
    ''' Fill lists in combo boxes
    ''' </summary>
    Private Sub BindComboBoxes()
        ' cmbProtocol
        ConnectionEditorUtils.BindComboWithEnum(Of ProtocolMode)(cmbProtocol, Nothing)

        ' cmbProxyType
        ConnectionEditorUtils.BindComboWithEnum(Of ProxyType)(cmbProxyType, Nothing)
    End Sub

    ''' <summary>
    ''' Removes all invisible controls from this user control's docking containers
    ''' and adjusts height of the containers and of this control according to current content.
    ''' </summary>
    Private Sub RearrangeVisibleControls()
        ' don't execute this method in design-time
        If LicenseManager.UsageMode = LicenseUsageMode.Designtime Then
            Return
        End If

        Dim thisHeaderHeight As Integer = Me.Height - tabControlMain.Height
        ' unfortunately, we cannot compute height of the remaining part of tab control using
        ' tabControlMain.Height - tabControlMain.TabPages["tabServer"].Height;
        ' because tabPage.Height is not affected by tabControl resizing performed by docking
        Dim tabHeaderHeight As Integer = 26

        Dim maxHeight As Integer = 0

        ' tabServer
        RearrangeVisibleControls(grpConnection)
        RearrangeVisibleControls(grpCredentials)
        RearrangeVisibleControls(tabControlMain.TabPages("tabServer"))
        If tabControlMain.TabPages("tabServer").Height > maxHeight Then
            maxHeight = tabControlMain.TabPages("tabServer").Height
        End If

        ' tabProxy
        RearrangeVisibleControls(tabControlMain.TabPages("tabProxy"))
        If tabControlMain.TabPages("tabProxy").Height > maxHeight Then
            maxHeight = tabControlMain.TabPages("tabProxy").Height
        End If

        ' tabSsl
        If tabControlMain.TabPages.ContainsKey("tabSsl") Then
            RearrangeVisibleControls(tabControlMain.TabPages("tabSsl"))
            If tabControlMain.TabPages("tabSsl").Height > maxHeight Then
                maxHeight = tabControlMain.TabPages("tabSsl").Height
            End If
        End If

        Me.SuspendLayout()
        tabControlMain.Size = New Size(tabControlMain.Width, maxHeight + tabHeaderHeight)
        Me.Size = New Size(Me.Width, tabControlMain.Height + thisHeaderHeight)
        Me.MinimumSize = New Size(Me.MinimumSize.Width, Me.Height)
        Me.ResumeLayout()

        OnHeightRecalculated()
    End Sub

    ''' <summary>
    ''' Removes all invisible controls from given <paramref name="container"/>
    ''' and adjusts height of the container up to the last visible control.<br/>
    ''' Using <see cref="ControlsRearranging"/> or <see cref="ControlsRearranged"/> events it can be extended 
    ''' to add new controls to the beginning or to the end of the controls collection.
    ''' </summary>
    Private Sub RearrangeVisibleControls(container As Control)
        container.SuspendLayout()

        ' rebuild control collection - remove invisible controls
        Dim lst As New ArrayList(container.Controls)
        container.Controls.Clear()

        OnControlsRearranging(container)

        For Each control As Control In lst
            If control.Visible Then
                container.Controls.Add(control)
            End If
        Next

        OnControlsRearranged(container)

        container.ResumeLayout()

        ' recalculate height of the container
        Dim lastControl As Control = container.Controls(0)
        ' docked controls are sorted in reversed order unlike the Controls collection order
        container.Size = New Size(container.Size.Width, lastControl.Location.Y + lastControl.Size.Height + container.Padding.Bottom)

    End Sub

    ''' <summary>
    ''' Enables or disables some controls and resets default values of some controls depending on the currently set protocol.
    ''' </summary>
    Public Sub RecomputeProtocolSpecificFields()
        pnlClientCertificate.Enabled = IsClientCertificateEnabled

        ConnectionEditorUtils.SetProtocol(_previousProtocol, Protocol, Sub(p As ProtocolMode) _previousProtocol = p, ServerPort, Sub(p As Integer) tbServerPort.Text = If((p = 0), "", p.ToString()))
    End Sub

    Private Sub RefreshClientCertificateField()
        If _clientCertificate IsNot Nothing Then
            tbClientCertificate.Text = String.Format("CN: {0}, expires: {1:d}", _clientCertificate.LeafCertificate.GetCommonName(), _clientCertificate.LeafCertificate.GetExpirationDate())
        ElseIf Not String.IsNullOrEmpty(_clientCertificateFilename) Then
            tbClientCertificate.Text = String.Format("Load from: {0}", ClientCertificateFilename)
        Else
            tbClientCertificate.Text = ""
        End If
    End Sub

    ''' <summary>
    ''' If the <see cref="ClientCertificateFilename"/> is defined, but the certificate has not been loaded to the <see cref="ClientCertificate"/> property, it loads the certificate.<br/>
    ''' If the certificate requires a password, the method prompts for the password.
    ''' </summary>
    ''' <returns>False - an error has occurred.</returns>
    Public Function EnsureClientCertificate() As Boolean
        If ClientCertificate Is Nothing AndAlso Not String.IsNullOrEmpty(ClientCertificateFilename) Then
            Return LoadClientCertificate()
        End If
        Return True
    End Function

    ''' <summary>
    ''' Loads a certificate from the <paramref name="ClientCertificateFilename"/> into the <see cref="ClientCertificate"/> property.<br/>
    ''' If the certificate requires a password, the method prompts for the password.
    ''' </summary>
    ''' <returns>True - the key certificate been loaded or an user does not want any certificate. False - an error occurred.</returns>
    Private Function LoadClientCertificate() As Boolean
        Try
            ' try to load the certificate 
            Dim certificate As CertificateChain = Nothing
            ' (first attempt will be with empty password - only if it won't succeed, we will prompt for the password
            Dim firstAttempt As Boolean = True
            Dim password As String = ""
            While certificate Is Nothing
                Try
                    ClientCertificate = CertificateChain.LoadPfx(ClientCertificateFilename, password, KeySetOptions.Exportable)
                    ' success
                    Return True
                    ' handle invalid password
                Catch ex As CertificateException
                    If ex.Message <> "PFX password is not valid." Then
                        ConnectionEditorUtils.ShowException("Error occurred when importing certificate.", "Import failed", ex)
                        Return False
                    End If
                    Dim pd As New PassphraseDialog(If(firstAttempt, "The certificate is password protected." & vbCr & vbLf & "Enter the certificate password:", "The password is wrong." & vbCr & vbLf & "Enter again?"))
                    pd.Size = New Size(pd.Width, pd.Height + 13)
                    ' add extra line for the 2-lines password prompt
                    If pd.ShowDialog() <> DialogResult.OK Then
                        Return True
                    End If
                    password = pd.Passphrase
                    firstAttempt = False
                Catch ex As Exception
                    ConnectionEditorUtils.ShowException("Error occurred when importing certificate.", "Import failed", ex)
                    Return False
                End Try
            End While
            ' never reached
            Return False
        Finally
            ' once the certificate has not been successfully loaded, clear the stored filename
            If ClientCertificate Is Nothing Then
                ClientCertificateFilename = Nothing
            End If
        End Try
    End Function

#End Region

#Region "Event handlers"

    Private Sub OnHeightRecalculated()
        RaiseEvent HeightRecalculated(Me, EventArgs.Empty)
    End Sub

    Private Sub OnControlsRearranging(sender As Object)
        RaiseEvent ControlsRearranging(sender, EventArgs.Empty)
    End Sub

    Private Sub OnControlsRearranged(sender As Object)
        RaiseEvent ControlsRearranged(sender, EventArgs.Empty)
    End Sub

    Private Sub cmbProtocol_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbProtocol.SelectedIndexChanged
        RecomputeProtocolSpecificFields()
    End Sub

    Private Sub tbServerHostOrPort_TextChanged(sender As Object, e As EventArgs) Handles tbServerPort.TextChanged, tbServerHost.TextChanged
        ' since we are using another server, clear server certificate or fingerprint
        If Protocol = ProtocolMode.HTTPS Then
            ServerCertificateThumbprint = ""
        End If
    End Sub

    Private Sub btnImportClientCertificate_Click(sender As Object, e As EventArgs) Handles btnImportClientCertificate.Click
        ' ask for certificate
        Dim import As New ImportCertificateDialog()

        If import.ShowDialog() = DialogResult.OK Then
            ClientCertificateFilename = import.ClientCertificateFilename
            ClientCertificate = import.ClientCertificate
        End If
    End Sub

    Private Sub btnViewClientCertificate_Click(sender As Object, e As EventArgs) Handles btnViewClientCertificate.Click
        EnsureClientCertificate()

        If ClientCertificate Is Nothing Then
            ConnectionEditorUtils.ShowWarning("No certificate has been defined.", "View certificate")
            Return
        End If
        X509Certificate2UI.DisplayCertificate(New X509Certificate2(ClientCertificate.LeafCertificate.GetRawCertData()))
    End Sub

    Private Sub btnClearClientCertificate_Click(sender As Object, e As EventArgs) Handles btnClearClientCertificate.Click
        ClientCertificate = Nothing
        ClientCertificateFilename = ""
    End Sub

    Protected Overridable Sub rbServerCertificateStored_CheckedChanged(sender As Object, e As EventArgs) Handles rbServerCertificateStored.CheckedChanged
        pnlLocalyStoredCertificate.Enabled = rbServerCertificateStored.Checked
    End Sub

    Private Sub cbSingleSignOn_CheckedChanged(sender As Object, e As EventArgs) Handles cbSingleSignOn.CheckedChanged
        tbServerUser.Enabled = Not cbSingleSignOn.Checked
        tbServerPassword.Enabled = Not cbSingleSignOn.Checked
        cbStorePassword.Enabled = Not cbSingleSignOn.Checked
    End Sub

#End Region
End Class

