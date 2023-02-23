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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using Rebex.Mail;
using Rebex.Net;
using Rebex.Samples;
using System.Threading.Tasks;


namespace Rebex.Samples
{
    /// <summary>
    /// Summary description for MailForm.
    /// </summary>
    public class MailForm : System.Windows.Forms.Form
    {			
        #region Properties

        public static readonly string ConfigFilePath = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), string.Format("Rebex{0}Secure Mail{0}MailSendWinForm.xml",Path.DirectorySeparatorChar));

        /// <summary>
        /// Configuration object.
        /// </summary>
        private readonly Configuration _config;

        //Allowed TLS/SSL protocol.
        private TlsVersion _protocol = SecurityConfig.DefaultTlsVersion;

        //Allowed TLS/SSL cipher suite.
        private TlsCipherSuite _suite = TlsCipherSuite.All;

        /// <summary>
        /// SMTP object.
        /// </summary>
        private readonly Smtp _smtp;

        /// <summary>
        /// SMTP server hostname.
        /// </summary>
        public string ServerHost
        {
            get
            {
                return txtServer.Text;
            }
        }
        
        /// <summary>
        /// SMTP server port.
        /// </summary>
        public int ServerPort
        {
            get
            {
                // this could throw an exception which is presented to the user...
                return int.Parse(txtPort.Text);
            }
        }

        /// <summary>
        /// Progress dialog.
        /// </summary>
        private Progress _progress;

        /// <summary>
        /// Attachment filename.
        /// </summary>
        public string AttachmentFilename
        {
            get
            {
                return txtAttachmentFilename.Text;
            }
            set
            {
                txtAttachmentFilename.Text = value;
                
                // Enables or disables remove attachment button.
                bool hasAttachment = txtAttachmentFilename.Text.Length > 0;
                btnRemoveAttachment.Enabled = hasAttachment;
            }
        }

        /// <summary>
        /// Used for recovering username and password after 
        /// turning single sign-on on and off.
        /// </summary>
        private string _lastUserName, _lastPassword;

        #endregion		

        #region Form constructor and Dispose method

        /// <summary>
        /// Main form constructor. Initializes form and loads configuration.
        /// </summary>
        public MailForm()
        {
            InitializeComponent();

            CheckForIllegalCrossThreadCalls = true;

            _config = new Configuration(ConfigFilePath);
            _smtp = new Smtp();
            _smtp.ValidatingCertificate += Verifier.ValidatingCertificate;

            UpdateMessageSendingStatus(false);
            
            LoadConfigValues();

            radioSendAsyncAwait.Enabled = false;
            radioSendAsyncAwait.Text = "Send in the background using async and await keywords. Awailable in .net 4.5 version.";
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null) 
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        #endregion

        #region WinForm controls

        private System.Windows.Forms.RadioButton radioSendSync;
        private System.Windows.Forms.RadioButton radioSendAsync;
        private System.Windows.Forms.RadioButton radioSendAsyncWithProgress;
        private System.Windows.Forms.RadioButton radioSendAsyncAwait;
        private System.Windows.Forms.TextBox txtAttachmentFilename;
        private System.Windows.Forms.Button btnSetAttachment;
        private System.Windows.Forms.Button btnRemoveAttachment;		
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.OpenFileDialog openFileDialog1; 
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TextBox txtServer;
        private System.Windows.Forms.TextBox txtSubject;
        private System.Windows.Forms.TextBox txtBcc;
        private System.Windows.Forms.TextBox txtCc;
        private System.Windows.Forms.TextBox txtTo;
        private System.Windows.Forms.TextBox txtFrom;
        private System.Windows.Forms.TextBox txtBody;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.ComboBox cbSecurity;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.CheckBox cbSingleSignOn;		

        private System.ComponentModel.Container components = null;

        #endregion

        #region Load and save configuration
        
        /// <summary>
        /// Loads the configuration.
        /// </summary>
        private void LoadConfigValues()
        {
            txtServer.Text = _config.GetString("server");
            txtPort.Text = _config.GetInt32("port", 25).ToString();
            txtUsername.Text = _config.GetString("username");
            txtPassword.Text = _config.GetString("password");
            cbSingleSignOn.Checked = _config.GetBoolean("singleSignOn", false);
            txtFrom.Text = _config.GetString("from");
            txtTo.Text = _config.GetString("to");
            txtCc.Text = _config.GetString("cc");
            txtBcc.Text = _config.GetString("bcc");
            txtSubject.Text = _config.GetString("subject");
            cbSecurity.SelectedIndex = _config.GetInt32("security", 0);
            _protocol = (TlsVersion)_config.GetValue("protocol", typeof(TlsVersion));
            _suite = (TlsCipherSuite)_config.GetValue("suite", typeof(TlsCipherSuite));
            if (_protocol == TlsVersion.None)
                _protocol = SecurityConfig.DefaultTlsVersion;
            if (_suite == TlsCipherSuite.None)
                _suite = TlsCipherSuite.All;
        }

        /// <summary>
        /// Saves the configuration.
        /// </summary>
        private void SaveConfigValues()
        {
            _config.SetValue("server", ServerHost);
            _config.SetValue("port", ServerPort);
            _config.SetValue("username", txtUsername.Text);
            _config.SetValue("password", txtPassword.Text);
            _config.SetValue("singleSignOn", cbSingleSignOn.Checked);
            _config.SetValue("from", txtFrom.Text);
            _config.SetValue("to",txtTo.Text);
            _config.SetValue("cc",txtCc.Text);
            _config.SetValue("bcc",txtBcc.Text);
            _config.SetValue("subject",txtSubject.Text);
            _config.SetValue("security", cbSecurity.SelectedIndex);
            _config.SetValue("protocol", _protocol);
            _config.SetValue("suite", _suite);
            _config.Save();
        }

        #endregion
        
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MailForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.cbSingleSignOn = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbSecurity = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.btnSettings = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtAttachmentFilename = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.btnSetAttachment = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.txtSubject = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtBcc = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtCc = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtTo = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtFrom = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtBody = new System.Windows.Forms.TextBox();
            this.btnRemoveAttachment = new System.Windows.Forms.Button();
            this.btnSend = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.radioSendSync = new System.Windows.Forms.RadioButton();
            this.radioSendAsync = new System.Windows.Forms.RadioButton();
            this.radioSendAsyncWithProgress = new System.Windows.Forms.RadioButton();
            this.radioSendAsyncAwait = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pictureBox1);
            this.groupBox1.Controls.Add(this.txtPort);
            this.groupBox1.Controls.Add(this.cbSingleSignOn);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.txtPassword);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtUsername);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtServer);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cbSecurity);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.btnSettings);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox1.Location = new System.Drawing.Point(8, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(496, 132);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Server settings";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
            this.pictureBox1.Location = new System.Drawing.Point(328, 16);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(160, 88);
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(248, 24);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(50, 20);
            this.txtPort.TabIndex = 10;
            // 
            // cbSingleSignOn
            // 
            this.cbSingleSignOn.Location = new System.Drawing.Point(208, 50);
            this.cbSingleSignOn.Name = "cbSingleSignOn";
            this.cbSingleSignOn.Size = new System.Drawing.Size(100, 20);
            this.cbSingleSignOn.TabIndex = 11;
            this.cbSingleSignOn.Text = "Single sign-on";
            this.cbSingleSignOn.CheckedChanged += new System.EventHandler(this.cbSingleSignOn_CheckedChanged);
            // 
            // label10
            // 
            this.label10.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label10.Location = new System.Drawing.Point(208, 24);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(40, 24);
            this.label10.TabIndex = 7;
            this.label10.Text = "Port:";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(64, 72);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(128, 20);
            this.txtPassword.TabIndex = 30;
            // 
            // label3
            // 
            this.label3.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label3.Location = new System.Drawing.Point(8, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 24);
            this.label3.TabIndex = 5;
            this.label3.Text = "Password:";
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(64, 48);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(128, 20);
            this.txtUsername.TabIndex = 20;
            // 
            // label2
            // 
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label2.Location = new System.Drawing.Point(8, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 24);
            this.label2.TabIndex = 3;
            this.label2.Text = "Username:";
            // 
            // txtServer
            // 
            this.txtServer.Location = new System.Drawing.Point(64, 24);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(128, 20);
            this.txtServer.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label1.Location = new System.Drawing.Point(8, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 24);
            this.label1.TabIndex = 1;
            this.label1.Text = "Server:";
            // 
            // cbSecurity
            // 
            this.cbSecurity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSecurity.Items.AddRange(new object[] {
            "No security",
            "Explicit TLS/SSL",
            "Implicit TLS/SSL"});
            this.cbSecurity.Location = new System.Drawing.Point(64, 96);
            this.cbSecurity.MaxDropDownItems = 3;
            this.cbSecurity.Name = "cbSecurity";
            this.cbSecurity.Size = new System.Drawing.Size(128, 21);
            this.cbSecurity.TabIndex = 113;
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(8, 96);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(56, 24);
            this.label12.TabIndex = 112;
            this.label12.Text = "Security:";
            // 
            // btnSettings
            // 
            this.btnSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSettings.Location = new System.Drawing.Point(200, 96);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(100, 23);
            this.btnSettings.TabIndex = 114;
            this.btnSettings.Text = "Settings...";
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtAttachmentFilename);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.btnSetAttachment);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.txtSubject);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.txtBcc);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.txtCc);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.txtTo);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.txtFrom);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.txtBody);
            this.groupBox2.Controls.Add(this.btnRemoveAttachment);
            this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox2.Location = new System.Drawing.Point(8, 136);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(496, 264);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Message composing";
            // 
            // txtAttachmentFilename
            // 
            this.txtAttachmentFilename.BackColor = System.Drawing.SystemColors.Window;
            this.txtAttachmentFilename.Location = new System.Drawing.Point(80, 88);
            this.txtAttachmentFilename.Name = "txtAttachmentFilename";
            this.txtAttachmentFilename.ReadOnly = true;
            this.txtAttachmentFilename.Size = new System.Drawing.Size(280, 20);
            this.txtAttachmentFilename.TabIndex = 101;
            // 
            // label11
            // 
            this.label11.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label11.Location = new System.Drawing.Point(8, 112);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(80, 22);
            this.label11.TabIndex = 17;
            this.label11.Text = "Message body:";
            // 
            // btnSetAttachment
            // 
            this.btnSetAttachment.Location = new System.Drawing.Point(368, 88);
            this.btnSetAttachment.Name = "btnSetAttachment";
            this.btnSetAttachment.Size = new System.Drawing.Size(56, 23);
            this.btnSetAttachment.TabIndex = 100;
            this.btnSetAttachment.Text = "Add";
            this.btnSetAttachment.Click += new System.EventHandler(this.btnSetAttachment_Click);
            // 
            // label9
            // 
            this.label9.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label9.Location = new System.Drawing.Point(8, 88);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(64, 24);
            this.label9.TabIndex = 14;
            this.label9.Text = "Attachment:";
            // 
            // txtSubject
            // 
            this.txtSubject.Location = new System.Drawing.Point(80, 64);
            this.txtSubject.Name = "txtSubject";
            this.txtSubject.Size = new System.Drawing.Size(408, 20);
            this.txtSubject.TabIndex = 60;
            // 
            // label8
            // 
            this.label8.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label8.Location = new System.Drawing.Point(8, 64);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(64, 24);
            this.label8.TabIndex = 12;
            this.label8.Text = "Subject:";
            // 
            // txtBcc
            // 
            this.txtBcc.Location = new System.Drawing.Point(272, 40);
            this.txtBcc.Name = "txtBcc";
            this.txtBcc.Size = new System.Drawing.Size(216, 20);
            this.txtBcc.TabIndex = 90;
            // 
            // label6
            // 
            this.label6.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label6.Location = new System.Drawing.Point(216, 40);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 23);
            this.label6.TabIndex = 10;
            this.label6.Text = "Bcc:";
            // 
            // txtCc
            // 
            this.txtCc.Location = new System.Drawing.Point(272, 16);
            this.txtCc.Name = "txtCc";
            this.txtCc.Size = new System.Drawing.Size(216, 20);
            this.txtCc.TabIndex = 80;
            // 
            // label7
            // 
            this.label7.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label7.Location = new System.Drawing.Point(216, 16);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(56, 24);
            this.label7.TabIndex = 8;
            this.label7.Text = "Cc:";
            // 
            // txtTo
            // 
            this.txtTo.Location = new System.Drawing.Point(80, 40);
            this.txtTo.Name = "txtTo";
            this.txtTo.Size = new System.Drawing.Size(112, 20);
            this.txtTo.TabIndex = 50;
            // 
            // label5
            // 
            this.label5.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label5.Location = new System.Drawing.Point(8, 40);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 24);
            this.label5.TabIndex = 6;
            this.label5.Text = "To:";
            // 
            // txtFrom
            // 
            this.txtFrom.Location = new System.Drawing.Point(80, 16);
            this.txtFrom.Name = "txtFrom";
            this.txtFrom.Size = new System.Drawing.Size(112, 20);
            this.txtFrom.TabIndex = 40;
            // 
            // label4
            // 
            this.label4.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label4.Location = new System.Drawing.Point(8, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 24);
            this.label4.TabIndex = 4;
            this.label4.Text = "From:";
            // 
            // txtBody
            // 
            this.txtBody.Location = new System.Drawing.Point(8, 136);
            this.txtBody.Multiline = true;
            this.txtBody.Name = "txtBody";
            this.txtBody.Size = new System.Drawing.Size(480, 120);
            this.txtBody.TabIndex = 70;
            // 
            // btnRemoveAttachment
            // 
            this.btnRemoveAttachment.Enabled = false;
            this.btnRemoveAttachment.Location = new System.Drawing.Point(432, 88);
            this.btnRemoveAttachment.Name = "btnRemoveAttachment";
            this.btnRemoveAttachment.Size = new System.Drawing.Size(56, 23);
            this.btnRemoveAttachment.TabIndex = 100;
            this.btnRemoveAttachment.Text = "Remove";
            this.btnRemoveAttachment.Click += new System.EventHandler(this.btnRemoveAttachment_Click);
            // 
            // btnSend
            // 
            this.btnSend.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSend.Location = new System.Drawing.Point(408, 408);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(96, 72);
            this.btnSend.TabIndex = 110;
            this.btnSend.Text = "Send";
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // radioSendSync
            // 
            this.radioSendSync.Location = new System.Drawing.Point(8, 408);
            this.radioSendSync.Name = "radioSendSync";
            this.radioSendSync.Size = new System.Drawing.Size(384, 24);
            this.radioSendSync.TabIndex = 111;
            this.radioSendSync.Text = "Simple send - blocks UI until the operation is finished.";
            // 
            // radioSendAsync
            // 
            this.radioSendAsync.Checked = true;
            this.radioSendAsync.Location = new System.Drawing.Point(8, 432);
            this.radioSendAsync.Name = "radioSendAsync";
            this.radioSendAsync.Size = new System.Drawing.Size(384, 24);
            this.radioSendAsync.TabIndex = 111;
            this.radioSendAsync.TabStop = true;
            this.radioSendAsync.Text = "Send in the background. UI is not blocked.";
            // 
            // radioSendAsyncWithProgress
            // 
            this.radioSendAsyncWithProgress.Location = new System.Drawing.Point(8, 456);
            this.radioSendAsyncWithProgress.Name = "radioSendAsyncWithProgress";
            this.radioSendAsyncWithProgress.Size = new System.Drawing.Size(392, 24);
            this.radioSendAsyncWithProgress.TabIndex = 111;
            this.radioSendAsyncWithProgress.Text = "Send in the background. UI is not blocked. Progress bar is displayed.";
            // 
            // radioSendAsyncAwait
            // 
            this.radioSendAsyncAwait.Location = new System.Drawing.Point(8, 486);
            this.radioSendAsyncAwait.Name = "radioSendAsyncAwait";
            this.radioSendAsyncAwait.Size = new System.Drawing.Size(392, 34);
            this.radioSendAsyncAwait.TabIndex = 112;
            this.radioSendAsyncAwait.Text = "Send in the background using async and await keywords. UI is not blocked.";
            // 
            // MailForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(514, 532);
            this.Controls.Add(this.radioSendAsyncAwait);
            this.Controls.Add(this.radioSendSync);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.radioSendAsync);
            this.Controls.Add(this.radioSendAsyncWithProgress);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MailForm";
            this.Text = "Rebex Mail Message WinForm Sender";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        #region Message sending methods

        private void btnSend_Click(object sender, System.EventArgs e)
        {
            SaveConfigValues();
            
            // create mail message from values in the form
            MailMessage message = GetMailMessage();
            
            if (radioSendSync.Checked)
            {
                //
                // Send the message using Smpt.Send method.
                // 
                // The UI is blocked until the transfer is finished.
                //
                // This is very simple, but the window is not responsive during
                // the operation. Even though, it is good enough when the messages
                // are not long and the connection to he SMTP server is fast and reliable.
            
                SendMessageSync(message);
            }
            else if (radioSendAsync.Checked)
            {
                // 
                // Send the message asynchronously using Smtp.BeginSend and Smtp.EndSend methods.
                // 
                // The UI is not blocked, as the message is send asynchronously in the background.
                // The main windows is notified when transfer is finished.

                SendMessageAsync(message);
            }
            else if (radioSendAsyncWithProgress.Checked)
            {
                // 
                // Send the message asynchronously using Smtp.BeginSend and Smtp.EndSend methods
                // and display a dialog with a progress bar.
                // 
                // The UI is not blocked, as the message is send asynchronously in the background.
                // The main windows is notified when transfer is finished, and the progress window
                // is notified when a block of data is transferred.

                SendMessageWithProgressDialog(message);
            }
        }


        /// <summary>
        /// Send the message synchronously. Returns when the whole message is transferred to SMTP server.
        /// </summary>
        /// <param name="message"></param>
        private void SendMessageSync(MailMessage message)
        {
            Cursor lastCursor = this.Cursor;
            try
            {
                // set the wait cursor
                this.Cursor = Cursors.WaitCursor;
                
                // connect and login
                ConnectAndLogin();
                
                // send message without progress dialog
                _smtp.Send(message);

                MessageBox.Show("Mail message was sent successfully.");
            }
            catch (Exception x)
            {
                DisplayError(x);
            }
            finally
            {
                // hide wait cursor
                this.Cursor = lastCursor;

                // close the connection
                _smtp.Disconnect();
            }
        }

        
        /// <summary>
        /// Send the message asynchronously in the background. After the transfer is finished,
        /// appropriate delegate is called depending on asynchronous API.
        /// </summary>
        /// <param name="message"></param>
        private void SendMessageAsync(MailMessage message)
        {
            try
            {
                // connect to the smtp server
                ConnectAndLogin();
                
                // disable buttons
                UpdateMessageSendingStatus(true);
                
                // Start sending... 
                _smtp.SendAsync(message).ContinueWith(SendCompleted);
            }
            catch (Exception x)
            {
                // close the connection
                _smtp.Disconnect();
                
                // enable the send button
                UpdateMessageSendingStatus(false);
                
                // re-throw the exception
                DisplayError(x);
            }
        }

        /// <summary>
        /// Send the message asynchronously in the background and show transfer progress dialog.
        /// After the transfer is finished, the appropriate delegate is called depending on asynchronous API.
        /// </summary>
        /// <param name="message"></param>
        protected void SendMessageWithProgressDialog(MailMessage message)
        {
            try
            {
                // connect to smtp server			
                ConnectAndLogin();

                PrepareProgress(message);
                
                UpdateMessageSendingStatus(true);
                
                // Start sending...
                _smtp.SendAsync(message).ContinueWith(SendCompleted);
            }
            catch (Exception x)
            {
                // close the connection
                _smtp.Disconnect();
                
                // enable the send button
                UpdateMessageSendingStatus(false);

                // close progress dialog
                if (_progress != null) 
                {
                    _progress.Unbind(_smtp);
                    _progress.Close();
                    _progress = null;
                }

                DisplayError(x);
            }
        }

        #endregion

        #region Methods and delagates for handling asynchronous results
        private void SendCompleted(Task task)
        {
            if (this.InvokeRequired)
            {
                Invoke(new Action<Task>(SendCompleted), task);
                return;
            }
            try
            {
                // unbind progress dialog from the Smtp object's events
                if (_progress != null)
                    _progress.Unbind(_smtp);

                if (task.IsFaulted)
                    throw task.Exception;
                if (_progress == null)
                {
                    // show message progress bar is not visible
                    MessageBox.Show("Mail message was sent successfully.");
                }
                else
                {
                    // indicate success on progress bar window,
                    // then forget about it - it will stay around until
                    // it is closed by the user
                    _progress.SetFinished();
                    _progress = null;
                }
            }
            catch (Exception x)
            {
                if (_progress != null)
                {
                    _progress.Close();
                    _progress = null;
                }

                DisplayError(x);
            }
            finally
            {
                // close the connection
                _smtp.Disconnect();
                
                // enable the send button
                UpdateMessageSendingStatus(false);
            }
        }
        
        #endregion
        
        #region Helper methods
        
        /// <summary>
        /// Returns mail message constructed from WinForms properties.
        /// </summary>
        public MailMessage GetMailMessage()
        {
            // create message object
            MailMessage message = new MailMessage();
        
            // fill message properties
            message.From = txtFrom.Text;
            message.To = txtTo.Text;
            message.CC = txtCc.Text;
            message.Bcc = txtBcc.Text;
            message.Subject = txtSubject.Text;
            message.BodyText = txtBody.Text;
            
            // add attachment if needed
            if (AttachmentFilename.Length > 0)
            {
                message.Attachments.Add(new Attachment(AttachmentFilename));
            }

            return message;				
        }

        
        /// <summary>
        /// Connects to SMTP server. Logins with username and password if needed.
        /// </summary>
        public void ConnectAndLogin()
        {
            SslMode security;

            PrepareConnect(out security);

            // connect to the server
            _smtp.Connect(ServerHost, ServerPort, security);
            // login if needed

            if (cbSingleSignOn.Checked)
                _smtp.Login(SmtpAuthentication.Auto); //single sign-on authentication
            else if (txtUsername.Text.Length > 0)
                _smtp.Login(txtUsername.Text, txtPassword.Text);
        }

        private void PrepareConnect(out SslMode security)
        {

            switch (cbSecurity.SelectedIndex)
            {
                case 1:
                    security = SslMode.Explicit;
                    break;
                case 2:
                    security = SslMode.Implicit;
                    break;
                default:
                    security = SslMode.None;
                    break;
            }

            _smtp.Settings.SslAllowedVersions = (TlsVersion)_config.GetValue("protocol", typeof(TlsVersion));
            _smtp.Settings.SslAllowedSuites = (TlsCipherSuite)_config.GetValue("suite", typeof(TlsCipherSuite));
        }

        private void PrepareProgress(MailMessage message)
        {
            // create and show progress dialog bound to the Smtp object's events
            _progress = new Progress(_smtp);
            _progress.Show();

            // write message to memory stream to determine its total length
            MemoryStream stream = new MemoryStream();
            message.Save(stream);
            // set message length (and initialize progress bar)				
            _progress.MessageLength = stream.Length;
        }

        private void FinishProgress()
        {
            // unbind progress dialog from the Smtp object's events
            if (_progress != null)
            {
                _progress.Unbind(_smtp);

                // indicate success on progress bar window,
                // then forget about it - it will stay around until
                // it is closed by the user
                _progress.SetFinished();
                _progress = null;
            }
        }
        #endregion
        
        #region UI helper methods - button click handlers and helper methods.
        
        /// <summary>
        /// Updates UI - disables/enables "Send" button and changes it's text.
        /// </summary>
        /// <param name="messageIsSending"></param>
        public void UpdateMessageSendingStatus(bool messageIsSending)
        {
            if (messageIsSending)
            {
                btnSend.Enabled = false;
                btnSend.Text = "Sending...";
            }
            else
            {
                btnSend.Enabled = true;
                btnSend.Text = "Send";
            }
        }
        
        /// <summary>
        /// Set attachment filename.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSetAttachment_Click(object sender, System.EventArgs e)
        {
            openFileDialog1.Filter = "All files (*.*)|*.*";
            openFileDialog1.ShowDialog();

            string fileName = openFileDialog1.FileName;
            
            if (fileName.Length > 0)
                AttachmentFilename = fileName;
        }

        /// <summary>
        /// Unset attachment filename.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemoveAttachment_Click(object sender, System.EventArgs e)
        {
            AttachmentFilename = "";
        }

        /// <summary>
        /// Display error message based on an exception to the user.
        /// </summary>
        /// <param name="x"></param>
        private void DisplayError(Exception x)
        {
            AggregateException aggregateEx = x as AggregateException;
            if (aggregateEx != null)
            {
                foreach (Exception ex in aggregateEx.InnerExceptions)
                {
                    DisplayError(ex);
                }
                return;
            }
            SmtpException sx = x as SmtpException;
            if (sx != null && sx.Status == SmtpExceptionStatus.AsyncError)
                x = sx.InnerException;

            string message =
                x.Message + "\r\n" +
                "------------------------------------------------------\r\n" + 
                x.ToString();
            MessageBox.Show(this, message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Security settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSettings_Click(object sender, System.EventArgs e)
        {
            SecurityConfig config = new SecurityConfig();
            config.Protocol = _protocol;
            config.AllowedSuite = _suite;
            if (config.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _protocol = config.Protocol;
                _suite = config.AllowedSuite;
            }
        }

        /// <summary>
        /// Single sign on CheckBox changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param> 
        private void cbSingleSignOn_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSingleSignOn.Checked)
            {
                // turning on single sign
                // save password and username for later use without single sign on
                _lastUserName = txtUsername.Text;
                _lastPassword = txtPassword.Text;
                // hide the username and password from GUI
                txtPassword.Text = String.Empty;
                txtUsername.Text = String.Empty;
                txtPassword.Enabled = false;
                txtUsername.Enabled = false;
            }
            else
            {
                // disabling single sign on
                // restore the saved username and password
                txtUsername.Text = _lastUserName;
                txtPassword.Text = _lastPassword;
                txtPassword.Enabled = true;
                txtUsername.Enabled = true;
            }
        }

        #endregion

    }
}
