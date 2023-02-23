//  
//   Rebex Sample Code License
// 
//   Copyright 2023, REBEX CR s.r.o.
//   All rights reserved.
//   https://www.rebex.net/
// 
//   Permission to use, copy, modify, and/or distribute this software for any
//   purpose with or without fee is hereby granted.
// 
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//   EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
//   OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//   NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
//   HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
//   WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
//   FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
//   OTHER DEALINGS IN THE SOFTWARE.
// 

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using Rebex.Net;
using Rebex.Security.Certificates;

namespace Rebex.Samples
{
    public partial class UcConnectionEditor : UserControl
    {
        #region Private variables

        /// <summary>
        /// A protocol "before selection has changed" is remembered here.
        /// </summary>
        private ProtocolMode _previousProtocol;

        #endregion

        #region Events

        [Description("Occurs when height of inner controls has been recalculated according to current visibility of their content.")]
        /// <summary>
        /// Occurs when height of inner controls has been recalculated according to current visibility of their content.
        /// </summary>		
        public event EventHandler HeightRecalculated;

        /// <summary>
        /// Occurs when rearranging within some inner control starts.<br/>
        /// Can be used as an extension point for adding new inner controls to the sender.
        /// </summary>
        protected event EventHandler ControlsRearranging;

        /// <summary>
        /// Occurs when rearranging within some inner control is finished.<br/>
        /// Can be used as an extension point for adding new inner controls to the sender.
        /// </summary>
        protected event EventHandler ControlsRearranged;

        #endregion

        #region Constructor
        public UcConnectionEditor()
        {
            InitializeComponent();

            // design-time binding should be done in constructor (regardless filtering)
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                BindComboBoxes();
        }
        #endregion

        #region Field values properties

        #region Connection

        /// <summary>
        /// Gets or sets server host address.
        /// </summary>
        [Browsable(false)]
        public string ServerHost
        {
            get { return tbServerHost.Text; }
            set { tbServerHost.Text = value; }
        }

        /// <summary>
        /// Gets or sets server port.
        /// </summary>
        [Browsable(false)]
        public int ServerPort
        {
            get
            {
                int ret = 0;
                int.TryParse(tbServerPort.Text, out ret);
                return ret;
            }
            set { tbServerPort.Text = (value == 0) ? "" : value.ToString(); }
        }

        /// <summary>
        /// Gets or sets used file transfer protocol mode.
        /// </summary>
        [Browsable(false)]
        public ProtocolMode Protocol
        {
            get
            {
                // if nothing is selected...
                if (cmbProtocol.SelectedValue == null)
                    cmbProtocol.SelectedIndex = 0;	// ... select the first item
                return (ProtocolMode)cmbProtocol.SelectedValue;
            }
            set { cmbProtocol.SelectedValue = value; }
        }

        #endregion

        #region Credentials

        /// <summary>
        /// Gets or sets server login name.
        /// </summary>
        [Browsable(false)]
        public string ServerUser
        {
            get { return tbServerUser.Text; }
            set { tbServerUser.Text = value; }
        }

        /// <summary>
        /// Gets or sets server login password.
        /// </summary>
        [Browsable(false)]
        public string ServerPassword
        {
            get { return tbServerPassword.Text; }
            set { tbServerPassword.Text = value; }
        }

        /// <summary>
        /// Gets or sets a value, indicating whether the password can be persisted to a file (outside the control). 
        /// </summary>
        [Browsable(false)]
        public bool StorePassword
        {
            get { return cbStorePassword.Checked; }
            set { cbStorePassword.Checked = value; }
        }

        /// <summary>
        /// Gets or sets a value, indicating whether to use Single Sign On. 
        /// </summary>
        [Browsable(false)]
        public bool UseSingleSignOn
        {
            get { return cbSingleSignOn.Checked; }
            set { cbSingleSignOn.Checked = value; }
        }

        /// <summary>
        /// Gets or sets shared mailbox.
        /// </summary>
        [Browsable(false)]
        public string Mailbox
        {
            get { return tbMailbox.Text; }
            set { tbMailbox.Text = value; }
        }

        /// <summary>
        /// Gets or sets filename used to load a client certificate for SSL authentication.
        /// </summary>
        [Browsable(true)]
        public string ClientCertificateFilename
        {
            get
            {
                return _clientCertificateFilename;
            }
            set
            {
                _clientCertificateFilename = value;
                RefreshClientCertificateField();
            }
        }
        private string _clientCertificateFilename = null;

        /// <summary>
        /// Gets or sets client certificate used for SSL authentication.
        /// </summary>
        [Browsable(false)]
        public CertificateChain ClientCertificate
        {
            get
            {
                return _clientCertificate;
            }
            set
            {
                _clientCertificate = value;
                RefreshClientCertificateField();
            }
        }
        private CertificateChain _clientCertificate = null;

        #endregion

        #region Proxy

        /// <summary>
        /// Gets or sets used proxy type (or <see cref="FileTransferProxyType.None"/> when no proxy is used).
        /// </summary>
        [Browsable(false)]
        public ProxyType ProxyType
        {
            get
            {
                // if nothing is selected...
                if (cmbProxyType.SelectedValue == null)
                {
                    if (cmbProxyType.Items == null || cmbProxyType.Items.Count <= 0)
                        return ProxyType.None;	// ... and nothing can be selected -> just return the default
                    cmbProxyType.SelectedIndex = 0;	// ... and something can be selected -> select the first item as a side effect
                }
                return (ProxyType)cmbProxyType.SelectedValue;
            }
            set
            {
                cmbProxyType.SelectedValue = value;
            }
        }

        /// <summary>
        /// Gets or sets proxy host address.
        /// </summary>
        [Browsable(false)]
        public string ProxyHost
        {
            get { return tbProxyHost.Text; }
            set { tbProxyHost.Text = value; }
        }

        /// <summary>
        /// Gets or sets proxy port.
        /// </summary>
        [Browsable(false)]
        public int ProxyPort
        {
            get
            {
                int ret = 0;
                int.TryParse(tbProxyPort.Text, out ret);
                return ret;
            }
            set { tbProxyPort.Text = (value == 0) ? "" : value.ToString(); }
        }

        /// <summary>
        /// Gets or sets proxy login name.
        /// </summary>
        [Browsable(false)]
        public string ProxyUser
        {
            get { return tbProxyUsername.Text; }
            set { tbProxyUsername.Text = value; }
        }

        /// <summary>
        /// Gets or sets proxy login password.
        /// </summary>
        [Browsable(false)]
        public string ProxyPassword
        {
            get { return tbProxyPassword.Text; }
            set { tbProxyPassword.Text = value; }
        }

        #endregion

        #region SSL specifics

        /// <summary>
        /// Gets or sets hash value used for validating SSL server certificate 
        /// </summary>
        [Browsable(false)]
        public CertificateVerifyingMode ServerCertificateVerifyingMode
        {
            get
            {
                if (rbServerCertificateAny.Checked)
                    return CertificateVerifyingMode.AcceptAnyCertificate;
                if (rbServerCertificateWindows.Checked)
                    return CertificateVerifyingMode.UseWindowsInfrastructure;
                if (rbServerCertificateStored.Checked)
                    return CertificateVerifyingMode.LocalyStoredThumbprint;
                if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                    return (CertificateVerifyingMode)0;	// we should not throw exception in design time - let's return some invalid value instead
                throw new InvalidOperationException("No CertfificateVerifyingMode has been chosen.");
            }
            set
            {
                switch (value)
                {
                    case CertificateVerifyingMode.AcceptAnyCertificate:
                        rbServerCertificateAny.Checked = true;
                        break;
                    case CertificateVerifyingMode.UseWindowsInfrastructure:
                        rbServerCertificateWindows.Checked = true;
                        break;
                    case CertificateVerifyingMode.LocalyStoredThumbprint:
                        rbServerCertificateStored.Checked = true;
                        break;
                    default:
                        throw new InvalidOperationException(string.Format("Unexpected enum value '{0}'", value));
                }
            }
        }

        /// <summary>
        /// Gets or sets hash value used for validating SSL server certificate 
        /// (when <see cref="ServerCertificateVerifyingMode"/> is <c>LocalyStoredThumbprint</c>).
        /// </summary>
        [Browsable(false)]
        public string ServerCertificateThumbprint
        {
            get { return tbServerCertificateThumbprint.Text; }
            set { tbServerCertificateThumbprint.Text = value; }
        }

        /// <summary>
        /// Gets or sets TLS/SSL protocol version.
        /// </summary>
        public TlsVersion TlsProtocol
        {
            set
            {
                cbAllowTls13.Checked = ((value & TlsVersion.TLS13) != 0);
                cbAllowTls12.Checked = ((value & TlsVersion.TLS12) != 0);
                cbAllowTls11.Checked = ((value & TlsVersion.TLS11) != 0);
                cbAllowTls10.Checked = ((value & TlsVersion.TLS10) != 0);
            }
            get
            {
                TlsVersion version = TlsVersion.None;
                if (cbAllowTls13.Checked)
                    version |= TlsVersion.TLS13;
                if (cbAllowTls12.Checked)
                    version |= TlsVersion.TLS12;
                if (cbAllowTls11.Checked)
                    version |= TlsVersion.TLS11;
                if (cbAllowTls10.Checked)
                    version |= TlsVersion.TLS10;
                return version;
            }
        }

        /// <summary>
        /// Gets or sets allowed secure suites.
        /// </summary>
        [Browsable(false)]
        public TlsCipherSuite AllowedSuite
        {
            get
            {
                if (cmbSuites.SelectedIndex == 0)
                    return TlsCipherSuite.All;
                else
                    return TlsCipherSuite.Secure;
            }
            set
            {
                if (value == TlsCipherSuite.All)
                    cmbSuites.SelectedIndex = 0;
                else
                    cmbSuites.SelectedIndex = 1;
            }
        }

        #endregion

        #region Transform from/to ConnectionData

        public void GetData(ConnectionData data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            data.ProxyHost = ProxyHost;
            data.ProxyPassword = ProxyPassword;
            data.ProxyPort = ProxyPort;
            data.ProxyType = ProxyType;
            data.ProxyUser = ProxyUser;
            data.Protocol = Protocol;
            data.ServerCertificateThumbprint = ServerCertificateThumbprint;
            data.ServerCertificateVerifyingMode = ServerCertificateVerifyingMode;
            data.ClientCertificateFilename = ClientCertificateFilename;
            data.ClientCertificate = ClientCertificate;
            data.ServerHost = ServerHost;
            data.ServerPassword = ServerPassword;
            data.ServerPort = ServerPort;
            data.ServerUser = ServerUser;
            data.StorePassword = StorePassword;
            data.UseSingleSignOn = UseSingleSignOn;
            data.Mailbox = Mailbox;
            data.AllowedSuite = AllowedSuite;
            data.TlsProtocol = TlsProtocol;
        }

        public void SetData(ConnectionData data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Protocol = data.Protocol;

            ServerHost = data.ServerHost;
            ServerPort = data.ServerPort;
            ServerPassword = data.ServerPassword;
            UseSingleSignOn = data.UseSingleSignOn;
            ServerUser = data.ServerUser;
            Mailbox = data.Mailbox;

            ProxyHost = data.ProxyHost;
            ProxyPassword = data.ProxyPassword;
            ProxyPort = data.ProxyPort;
            ProxyType = data.ProxyType;
            ProxyUser = data.ProxyUser;

            ServerCertificateVerifyingMode = data.ServerCertificateVerifyingMode;
            ServerCertificateThumbprint = data.ServerCertificateThumbprint;
            ClientCertificateFilename = data.ClientCertificateFilename;

            StorePassword = data.StorePassword;

            AllowedSuite = data.AllowedSuite;
            TlsProtocol = data.TlsProtocol;
        }

        #endregion

        #endregion

        #region Visibility and accessibility properties

        /// <summary>
        /// Gets information, whether GUI for settings of SSL client certificate is currently enabled.
        /// </summary>
        [Browsable(false)]
        public bool IsClientCertificateEnabled
        {
            get { return Protocol == ProtocolMode.HTTPS; }
        }

        #endregion

        #region Validation

        /// <summary>
        /// Validates all static rules. Returns collection of errors.
        /// </summary>
        /// <returns>Collection of error messages or empty collection on success.</returns>
        public List<string> GetValidationErrors()
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrEmpty(ServerHost))
                errors.Add("Server host name is a required field.");
            if (ServerPort <= 0)
                errors.Add("Server port is a required non-zero field.");

            if (ProxyType != ProxyType.None)
            {
                if (string.IsNullOrEmpty(ProxyHost))
                    errors.Add("Proxy host name is a required field.");
                if (ProxyPort <= 0)
                    errors.Add("Proxy port is a required non-zero field.");

                //if (FileTransferMode == FileTransferMode.FtpSslExplicit || FileTransferMode == FileTransferMode.FtpSslImplicit)
                //{
                //	FileTransferProxyType[] proxyWhenSecure = new FileTransferProxyType[] { FileTransferProxyType.Socks4, FileTransferProxyType.Socks4a, FileTransferProxyType.Socks5, FileTransferProxyType.HttpConnect };				
                //	bool ok = false;

                //for (int i = 0; i < proxyWhenSecure.Length; i++)
                //{
                //	if (proxyWhenSecure[i] == ProxyType) 
                //		ok = true;
                //}

                //if (!ok)
                //	errors.Add("This proxy type is not allowed when using SSL.");
                //}
            }

            return errors;
        }

        #endregion

        #region GUI behavior methods

        /// <summary>
        /// Recomputes visibility of inner controls and values according to <see cref="HideFtpGui"/> and <see cref="HideSftpGui"/>,
        /// then removes all invisible controls from this user control 
        /// and adjusts height both of the user control and of the parent form where the user control is placed in.
        /// </summary>
        public void AdaptToCurrentVisibility()
        {
            // don't execute this method in design-time
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            BindComboBoxes();

            RecomputeProtocolSpecificFields();

            if (ParentForm == null)
                throw new InvalidOperationException("AdaptToCurrentVisibility can be called only if the user control is placed into some ParentForm.");

            // store current anchor settings and clear bottom and right anchor (that anchors would mess-up the resizing)
            AnchorStyles origAnchor = Anchor;
            Anchor = AnchorStyles.Left | AnchorStyles.Top;

            int heightDelta = ParentForm.Height - Height;
            RearrangeVisibleControls();

            ParentForm.Size = new Size(ParentForm.Width, Height + heightDelta);
            ParentForm.MinimumSize = new Size(ParentForm.MinimumSize.Width, ParentForm.Height);

            Anchor = origAnchor;
        }

        /// <summary>
        /// Fill lists in combo boxes
        /// </summary>
        private void BindComboBoxes()
        {
            // cmbProtocol
            ConnectionEditorUtils.BindComboWithEnum<ProtocolMode>(cmbProtocol, null);

            // cmbProxyType
            ConnectionEditorUtils.BindComboWithEnum<ProxyType>(cmbProxyType, null);
        }

        /// <summary>
        /// Removes all invisible controls from this user control's docking containers
        /// and adjusts height of the containers and of this control according to current content.
        /// </summary>
        private void RearrangeVisibleControls()
        {
            // don't execute this method in design-time
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            int thisHeaderHeight = this.Height - tabControlMain.Height;
            // unfortunately, we cannot compute height of the remaining part of tab control using
            // tabControlMain.Height - tabControlMain.TabPages["tabServer"].Height;
            // because tabPage.Height is not affected by tabControl resizing performed by docking
            int tabHeaderHeight = 26;

            int maxHeight = 0;

            // tabServer
            RearrangeVisibleControls(grpConnection);
            RearrangeVisibleControls(grpCredentials);
            RearrangeVisibleControls(tabControlMain.TabPages["tabServer"]);
            if (tabControlMain.TabPages["tabServer"].Height > maxHeight)
                maxHeight = tabControlMain.TabPages["tabServer"].Height;

            // tabProxy
            RearrangeVisibleControls(tabControlMain.TabPages["tabProxy"]);
            if (tabControlMain.TabPages["tabProxy"].Height > maxHeight)
                maxHeight = tabControlMain.TabPages["tabProxy"].Height;

            // tabSsl
            if (tabControlMain.TabPages.ContainsKey("tabSsl"))
            {
                RearrangeVisibleControls(tabControlMain.TabPages["tabSsl"]);
                if (tabControlMain.TabPages["tabSsl"].Height > maxHeight)
                    maxHeight = tabControlMain.TabPages["tabSsl"].Height;
            }

            this.SuspendLayout();
            tabControlMain.Size = new Size(tabControlMain.Width, maxHeight + tabHeaderHeight);
            this.Size = new Size(this.Width, tabControlMain.Height + thisHeaderHeight);
            this.MinimumSize = new Size(this.MinimumSize.Width, this.Height);
            this.ResumeLayout();

            OnHeightRecalculated();
        }

        /// <summary>
        /// Removes all invisible controls from given <paramref name="container"/>
        /// and adjusts height of the container up to the last visible control.<br/>
        /// Using <see cref="ControlsRearranging"/> or <see cref="ControlsRearranged"/> events it can be extended 
        /// to add new controls to the beginning or to the end of the controls collection.
        /// </summary>
        private void RearrangeVisibleControls(Control container)
        {
            container.SuspendLayout();

            // rebuild control collection - remove invisible controls
            ArrayList lst = new ArrayList(container.Controls);
            container.Controls.Clear();

            OnControlsRearranging(container);

            foreach (Control control in lst)
                if (control.Visible)
                    container.Controls.Add(control);

            OnControlsRearranged(container);

            container.ResumeLayout();

            // recalculate height of the container
            Control lastControl = container.Controls[0];	// docked controls are sorted in reversed order unlike the Controls collection order
            container.Size = new Size(container.Size.Width, lastControl.Location.Y + lastControl.Size.Height + container.Padding.Bottom);

        }

        /// <summary>
        /// Enables or disables some controls and resets default values of some controls depending on the currently set protocol.
        /// </summary>
        public void RecomputeProtocolSpecificFields()
        {
            pnlClientCertificate.Enabled = IsClientCertificateEnabled;

            ConnectionEditorUtils.SetProtocol(
                _previousProtocol,
                Protocol,
                delegate(ProtocolMode p) { _previousProtocol = p; },
                ServerPort,
                delegate(int p) { tbServerPort.Text = (p == 0) ? "" : p.ToString(); }
            );
        }

        private void RefreshClientCertificateField()
        {
            if (_clientCertificate != null)
                tbClientCertificate.Text = string.Format("CN: {0}, expires: {1:d}", _clientCertificate.LeafCertificate.GetCommonName(), _clientCertificate.LeafCertificate.GetExpirationDate());
            else if (!string.IsNullOrEmpty(_clientCertificateFilename))
                tbClientCertificate.Text = string.Format("Load from: {0}", ClientCertificateFilename);
            else
                tbClientCertificate.Text = "";
        }

        /// <summary>
        /// If the <see cref="ClientCertificateFilename"/> is defined, but the certificate has not been loaded to the <see cref="ClientCertificate"/> property, it loads the certificate.<br/>
        /// If the certificate requires a password, the method prompts for the password.
        /// </summary>
        /// <returns>False - an error has occurred.</returns>
        public bool EnsureClientCertificate()
        {
            if (ClientCertificate == null && !string.IsNullOrEmpty(ClientCertificateFilename))
                return LoadClientCertificate();
            return true;
        }

        /// <summary>
        /// Loads a certificate from the <paramref name="ClientCertificateFilename"/> into the <see cref="ClientCertificate"/> property.<br/>
        /// If the certificate requires a password, the method prompts for the password.
        /// </summary>
        /// <returns>True - the key certificate been loaded or an user does not want any certificate. False - an error occurred.</returns>
        private bool LoadClientCertificate()
        {
            try
            {
                // try to load the certificate 
                CertificateChain certificate = null;
                // (first attempt will be with empty password - only if it won't succeed, we will prompt for the password
                bool firstAttempt = true;
                string password = "";
                while (certificate == null)
                {
                    try
                    {
                        ClientCertificate = CertificateChain.LoadPfx(ClientCertificateFilename, password, KeySetOptions.Exportable);
                        // success
                        return true;
                    }
                    // handle invalid password
                    catch (CertificateException ex)
                    {
                        if (ex.Message != "PFX password is not valid.")
                        {
                            ConnectionEditorUtils.ShowException("Error occurred when importing certificate.", "Import failed", ex);
                            return false;
                        }
                        PassphraseDialog pd = new PassphraseDialog(firstAttempt ? "The certificate is password protected.\r\nEnter the certificate password:" : "The password is wrong.\r\nEnter again?");
                        pd.Size = new Size(pd.Width, pd.Height + 13);	// add extra line for the 2-lines password prompt
                        if (pd.ShowDialog() != DialogResult.OK)
                            return true;
                        password = pd.Passphrase;
                        firstAttempt = false;
                    }
                    catch (Exception ex)
                    {
                        ConnectionEditorUtils.ShowException("Error occurred when importing certificate.", "Import failed", ex);
                        return false;
                    }
                }
                return false; // never reached
            }
            finally
            {
                // once the certificate has not been successfully loaded, clear the stored filename
                if (ClientCertificate == null)
                    ClientCertificateFilename = null;
            }
        }

        #endregion

        #region Event handlers

        private void OnHeightRecalculated()
        {
            if (HeightRecalculated != null)
                HeightRecalculated(this, EventArgs.Empty);
        }

        private void OnControlsRearranging(object sender)
        {
            if (ControlsRearranging != null)
                ControlsRearranging(sender, EventArgs.Empty);
        }

        private void OnControlsRearranged(object sender)
        {
            if (ControlsRearranged != null)
                ControlsRearranged(sender, EventArgs.Empty);
        }

        private void cmbProtocol_SelectedIndexChanged(object sender, EventArgs e)
        {
            RecomputeProtocolSpecificFields();
        }

        private void tbServerHostOrPort_TextChanged(object sender, EventArgs e)
        {
            // since we are using another server, clear server certificate or fingerprint
            if (Protocol == ProtocolMode.HTTPS)
                ServerCertificateThumbprint = "";
        }

        private void btnImportClientCertificate_Click(object sender, EventArgs e)
        {
            // ask for certificate
            ImportCertificateDialog import = new ImportCertificateDialog();

            if (import.ShowDialog() == DialogResult.OK)
            {
                ClientCertificateFilename = import.ClientCertificateFilename;
                ClientCertificate = import.ClientCertificate;
            }
        }

        private void btnViewClientCertificate_Click(object sender, EventArgs e)
        {
            EnsureClientCertificate();

            if (ClientCertificate == null)
            {
                ConnectionEditorUtils.ShowWarning("No certificate has been defined.", "View certificate");
                return;
            }
            X509Certificate2UI.DisplayCertificate(new X509Certificate2(ClientCertificate.LeafCertificate.GetRawCertData()));
        }

        private void btnClearClientCertificate_Click(object sender, EventArgs e)
        {
            ClientCertificate = null;
            ClientCertificateFilename = "";
        }

        protected virtual void rbServerCertificateStored_CheckedChanged(object sender, EventArgs e)
        {
            pnlLocalyStoredCertificate.Enabled = rbServerCertificateStored.Checked;
        }

        private void cbSingleSignOn_CheckedChanged(object sender, EventArgs e)
        {
            tbServerUser.Enabled = !cbSingleSignOn.Checked;
            tbServerPassword.Enabled = !cbSingleSignOn.Checked;
            cbStorePassword.Enabled = !cbSingleSignOn.Checked;
        }

        #endregion
    }
}
