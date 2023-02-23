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

using System.Windows.Forms;

namespace Rebex.Samples
{
    partial class UcConnectionEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabSsl = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmbSuites = new System.Windows.Forms.ComboBox();
            this.cbAllowTls13 = new System.Windows.Forms.CheckBox();
            this.cbAllowTls12 = new System.Windows.Forms.CheckBox();
            this.cbAllowTls11 = new System.Windows.Forms.CheckBox();
            this.cbAllowTls10 = new System.Windows.Forms.CheckBox();
            this.label23 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.pnlServerCertificate = new System.Windows.Forms.Panel();
            this.pnlLocalyStoredCertificate = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.tbServerCertificateThumbprint = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.rbServerCertificateAny = new System.Windows.Forms.RadioButton();
            this.rbServerCertificateStored = new System.Windows.Forms.RadioButton();
            this.rbServerCertificateWindows = new System.Windows.Forms.RadioButton();
            this.label7 = new System.Windows.Forms.Label();
            this.tabProxy = new System.Windows.Forms.TabPage();
            this.pnlProxy = new System.Windows.Forms.Panel();
            this.label19 = new System.Windows.Forms.Label();
            this.tbProxyUsername = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.cmbProxyType = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.tbProxyPassword = new System.Windows.Forms.TextBox();
            this.tbProxyPort = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbProxyHost = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.tabServer = new System.Windows.Forms.TabPage();
            this.grpCredentials = new System.Windows.Forms.GroupBox();
            this.pnlClientCertificate = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.btnClearClientCertificate = new System.Windows.Forms.Button();
            this.tbClientCertificate = new System.Windows.Forms.TextBox();
            this.btnViewClientCertificate = new System.Windows.Forms.Button();
            this.btnImportClientCertificate = new System.Windows.Forms.Button();
            this.pnlMailbox = new System.Windows.Forms.Panel();
            this.tbMailbox = new System.Windows.Forms.TextBox();
            this.lblMailbox = new System.Windows.Forms.Label();
            this.pnlUsernamePassword = new System.Windows.Forms.Panel();
            this.cbSingleSignOn = new System.Windows.Forms.CheckBox();
            this.cbStorePassword = new System.Windows.Forms.CheckBox();
            this.tbServerUser = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbServerPassword = new System.Windows.Forms.TextBox();
            this.grpConnection = new System.Windows.Forms.GroupBox();
            this.pnlHostnameProtocol = new System.Windows.Forms.Panel();
            this.tbServerPort = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbProtocol = new System.Windows.Forms.ComboBox();
            this.tbServerHost = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabSsl.SuspendLayout();
            this.panel1.SuspendLayout();
            this.pnlServerCertificate.SuspendLayout();
            this.pnlLocalyStoredCertificate.SuspendLayout();
            this.tabProxy.SuspendLayout();
            this.pnlProxy.SuspendLayout();
            this.tabServer.SuspendLayout();
            this.grpCredentials.SuspendLayout();
            this.pnlClientCertificate.SuspendLayout();
            this.pnlMailbox.SuspendLayout();
            this.pnlUsernamePassword.SuspendLayout();
            this.grpConnection.SuspendLayout();
            this.pnlHostnameProtocol.SuspendLayout();
            this.tabControlMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabSsl
            // 
            this.tabSsl.Controls.Add(this.panel1);
            this.tabSsl.Controls.Add(this.pnlServerCertificate);
            this.tabSsl.Location = new System.Drawing.Point(4, 22);
            this.tabSsl.Name = "tabSsl";
            this.tabSsl.Padding = new System.Windows.Forms.Padding(3);
            this.tabSsl.Size = new System.Drawing.Size(612, 331);
            this.tabSsl.TabIndex = 2;
            this.tabSsl.Text = "TLS/SSL";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cmbSuites);
            this.panel1.Controls.Add(this.cbAllowTls13);
            this.panel1.Controls.Add(this.cbAllowTls12);
            this.panel1.Controls.Add(this.cbAllowTls11);
            this.panel1.Controls.Add(this.cbAllowTls10);
            this.panel1.Controls.Add(this.label23);
            this.panel1.Controls.Add(this.label22);
            this.panel1.Location = new System.Drawing.Point(3, 132);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(606, 67);
            this.panel1.TabIndex = 45;
            // 
            // cmbSuites
            // 
            this.cmbSuites.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSuites.FormattingEnabled = true;
            this.cmbSuites.Items.AddRange(new object[] {
            "All ciphers",
            "Secure only"});
            this.cmbSuites.Location = new System.Drawing.Point(122, 32);
            this.cmbSuites.Name = "cmbSuites";
            this.cmbSuites.Size = new System.Drawing.Size(117, 21);
            this.cmbSuites.TabIndex = 6;
            // 
            // cbAllowTls13
            // 
            this.cbAllowTls13.AutoSize = true;
            this.cbAllowTls13.Location = new System.Drawing.Point(122, 9);
            this.cbAllowTls13.Name = "cbAllowTls13";
            this.cbAllowTls13.Size = new System.Drawing.Size(64, 17);
            this.cbAllowTls13.TabIndex = 2;
            this.cbAllowTls13.Text = "TLS 1.3";
            this.cbAllowTls13.UseVisualStyleBackColor = true;
            // 
            // cbAllowTls12
            // 
            this.cbAllowTls12.AutoSize = true;
            this.cbAllowTls12.Checked = true;
            this.cbAllowTls12.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAllowTls12.Location = new System.Drawing.Point(192, 9);
            this.cbAllowTls12.Name = "cbAllowTls12";
            this.cbAllowTls12.Size = new System.Drawing.Size(64, 17);
            this.cbAllowTls12.TabIndex = 3;
            this.cbAllowTls12.Text = "TLS 1.2";
            this.cbAllowTls12.UseVisualStyleBackColor = true;
            // 
            // cbAllowTls11
            // 
            this.cbAllowTls11.AutoSize = true;
            this.cbAllowTls11.Checked = true;
            this.cbAllowTls11.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAllowTls11.Location = new System.Drawing.Point(262, 9);
            this.cbAllowTls11.Name = "cbAllowTls11";
            this.cbAllowTls11.Size = new System.Drawing.Size(64, 17);
            this.cbAllowTls11.TabIndex = 4;
            this.cbAllowTls11.Text = "TLS 1.1";
            this.cbAllowTls11.UseVisualStyleBackColor = true;
            // 
            // cbAllowTls10
            // 
            this.cbAllowTls10.AutoSize = true;
            this.cbAllowTls10.Checked = true;
            this.cbAllowTls10.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAllowTls10.Location = new System.Drawing.Point(332, 9);
            this.cbAllowTls10.Name = "cbAllowTls10";
            this.cbAllowTls10.Size = new System.Drawing.Size(64, 17);
            this.cbAllowTls10.TabIndex = 5;
            this.cbAllowTls10.Text = "TLS 1.0";
            this.cbAllowTls10.UseVisualStyleBackColor = true;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(6, 35);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(77, 13);
            this.label23.TabIndex = 1;
            this.label23.Text = "Allowed suites:";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(6, 10);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(88, 13);
            this.label22.TabIndex = 0;
            this.label22.Text = "Allowed protocol:";
            // 
            // pnlServerCertificate
            // 
            this.pnlServerCertificate.BackColor = System.Drawing.Color.Transparent;
            this.pnlServerCertificate.Controls.Add(this.pnlLocalyStoredCertificate);
            this.pnlServerCertificate.Controls.Add(this.rbServerCertificateAny);
            this.pnlServerCertificate.Controls.Add(this.rbServerCertificateStored);
            this.pnlServerCertificate.Controls.Add(this.rbServerCertificateWindows);
            this.pnlServerCertificate.Controls.Add(this.label7);
            this.pnlServerCertificate.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlServerCertificate.Location = new System.Drawing.Point(3, 3);
            this.pnlServerCertificate.Name = "pnlServerCertificate";
            this.pnlServerCertificate.Size = new System.Drawing.Size(606, 149);
            this.pnlServerCertificate.TabIndex = 43;
            // 
            // pnlLocalyStoredCertificate
            // 
            this.pnlLocalyStoredCertificate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlLocalyStoredCertificate.Controls.Add(this.label9);
            this.pnlLocalyStoredCertificate.Controls.Add(this.tbServerCertificateThumbprint);
            this.pnlLocalyStoredCertificate.Controls.Add(this.label10);
            this.pnlLocalyStoredCertificate.Location = new System.Drawing.Point(140, 67);
            this.pnlLocalyStoredCertificate.Name = "pnlLocalyStoredCertificate";
            this.pnlLocalyStoredCertificate.Size = new System.Drawing.Size(463, 79);
            this.pnlLocalyStoredCertificate.TabIndex = 41;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 19);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(60, 13);
            this.label9.TabIndex = 46;
            this.label9.Text = "(SH1 hash)";
            // 
            // tbServerCertificateThumbprint
            // 
            this.tbServerCertificateThumbprint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbServerCertificateThumbprint.Location = new System.Drawing.Point(90, 3);
            this.tbServerCertificateThumbprint.Multiline = true;
            this.tbServerCertificateThumbprint.Name = "tbServerCertificateThumbprint";
            this.tbServerCertificateThumbprint.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbServerCertificateThumbprint.Size = new System.Drawing.Size(371, 73);
            this.tbServerCertificateThumbprint.TabIndex = 45;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 6);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(63, 13);
            this.label10.TabIndex = 42;
            this.label10.Text = "Thumbprint:";
            // 
            // rbServerCertificateAny
            // 
            this.rbServerCertificateAny.AutoSize = true;
            this.rbServerCertificateAny.Location = new System.Drawing.Point(122, 1);
            this.rbServerCertificateAny.Name = "rbServerCertificateAny";
            this.rbServerCertificateAny.Size = new System.Drawing.Size(128, 17);
            this.rbServerCertificateAny.TabIndex = 31;
            this.rbServerCertificateAny.Text = "Accept any certificate";
            this.rbServerCertificateAny.UseVisualStyleBackColor = true;
            // 
            // rbServerCertificateStored
            // 
            this.rbServerCertificateStored.AutoSize = true;
            this.rbServerCertificateStored.Location = new System.Drawing.Point(122, 47);
            this.rbServerCertificateStored.Name = "rbServerCertificateStored";
            this.rbServerCertificateStored.Size = new System.Drawing.Size(197, 17);
            this.rbServerCertificateStored.TabIndex = 33;
            this.rbServerCertificateStored.Text = "Trust locally stored server certificate:";
            this.rbServerCertificateStored.UseVisualStyleBackColor = true;
            this.rbServerCertificateStored.CheckedChanged += new System.EventHandler(this.rbServerCertificateStored_CheckedChanged);
            // 
            // rbServerCertificateWindows
            // 
            this.rbServerCertificateWindows.AutoSize = true;
            this.rbServerCertificateWindows.Checked = true;
            this.rbServerCertificateWindows.Location = new System.Drawing.Point(122, 24);
            this.rbServerCertificateWindows.Name = "rbServerCertificateWindows";
            this.rbServerCertificateWindows.Size = new System.Drawing.Size(209, 17);
            this.rbServerCertificateWindows.TabIndex = 32;
            this.rbServerCertificateWindows.TabStop = true;
            this.rbServerCertificateWindows.Text = "Use Windows certificates infrastructure";
            this.rbServerCertificateWindows.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 3);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(90, 26);
            this.label7.TabIndex = 9;
            this.label7.Text = "Server certificate \r\nvalidation:";
            // 
            // tabProxy
            // 
            this.tabProxy.BackColor = System.Drawing.SystemColors.Control;
            this.tabProxy.Controls.Add(this.pnlProxy);
            this.tabProxy.Location = new System.Drawing.Point(4, 22);
            this.tabProxy.Name = "tabProxy";
            this.tabProxy.Padding = new System.Windows.Forms.Padding(3);
            this.tabProxy.Size = new System.Drawing.Size(612, 331);
            this.tabProxy.TabIndex = 1;
            this.tabProxy.Text = "Proxy";
            // 
            // pnlProxy
            // 
            this.pnlProxy.Controls.Add(this.label19);
            this.pnlProxy.Controls.Add(this.tbProxyUsername);
            this.pnlProxy.Controls.Add(this.label11);
            this.pnlProxy.Controls.Add(this.cmbProxyType);
            this.pnlProxy.Controls.Add(this.label13);
            this.pnlProxy.Controls.Add(this.label14);
            this.pnlProxy.Controls.Add(this.tbProxyPassword);
            this.pnlProxy.Controls.Add(this.tbProxyPort);
            this.pnlProxy.Controls.Add(this.label6);
            this.pnlProxy.Controls.Add(this.tbProxyHost);
            this.pnlProxy.Controls.Add(this.label12);
            this.pnlProxy.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlProxy.Location = new System.Drawing.Point(3, 3);
            this.pnlProxy.Margin = new System.Windows.Forms.Padding(0);
            this.pnlProxy.Name = "pnlProxy";
            this.pnlProxy.Size = new System.Drawing.Size(606, 116);
            this.pnlProxy.TabIndex = 139;
            // 
            // label19
            // 
            this.label19.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label19.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label19.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label19.Location = new System.Drawing.Point(446, 60);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(157, 43);
            this.label19.TabIndex = 125;
            this.label19.Text = "Leave the \'User name\' and \'Password\' fields empty when not needed.";
            // 
            // tbProxyUsername
            // 
            this.tbProxyUsername.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbProxyUsername.Location = new System.Drawing.Point(99, 57);
            this.tbProxyUsername.Name = "tbProxyUsername";
            this.tbProxyUsername.Size = new System.Drawing.Size(339, 20);
            this.tbProxyUsername.TabIndex = 3;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 7);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(59, 13);
            this.label11.TabIndex = 26;
            this.label11.Text = "Proxy type:";
            // 
            // cmbProxyType
            // 
            this.cmbProxyType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbProxyType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProxyType.FormattingEnabled = true;
            this.cmbProxyType.Location = new System.Drawing.Point(99, 4);
            this.cmbProxyType.Name = "cmbProxyType";
            this.cmbProxyType.Size = new System.Drawing.Size(339, 21);
            this.cmbProxyType.TabIndex = 0;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(3, 60);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(61, 13);
            this.label13.TabIndex = 122;
            this.label13.Text = "User name:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(3, 86);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(56, 13);
            this.label14.TabIndex = 121;
            this.label14.Text = "Password:";
            // 
            // tbProxyPassword
            // 
            this.tbProxyPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbProxyPassword.Location = new System.Drawing.Point(99, 83);
            this.tbProxyPassword.Name = "tbProxyPassword";
            this.tbProxyPassword.PasswordChar = '*';
            this.tbProxyPassword.Size = new System.Drawing.Size(339, 20);
            this.tbProxyPassword.TabIndex = 4;
            // 
            // tbProxyPort
            // 
            this.tbProxyPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbProxyPort.Location = new System.Drawing.Point(474, 31);
            this.tbProxyPort.Name = "tbProxyPort";
            this.tbProxyPort.Size = new System.Drawing.Size(129, 20);
            this.tbProxyPort.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(444, 34);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 13);
            this.label6.TabIndex = 27;
            this.label6.Text = "Port:";
            // 
            // tbProxyHost
            // 
            this.tbProxyHost.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbProxyHost.Location = new System.Drawing.Point(99, 31);
            this.tbProxyHost.Name = "tbProxyHost";
            this.tbProxyHost.Size = new System.Drawing.Size(339, 20);
            this.tbProxyHost.TabIndex = 1;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(3, 34);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(88, 13);
            this.label12.TabIndex = 25;
            this.label12.Text = "Proxy host name:";
            // 
            // tabServer
            // 
            this.tabServer.BackColor = System.Drawing.SystemColors.Control;
            this.tabServer.Controls.Add(this.grpCredentials);
            this.tabServer.Controls.Add(this.grpConnection);
            this.tabServer.Location = new System.Drawing.Point(4, 22);
            this.tabServer.Name = "tabServer";
            this.tabServer.Padding = new System.Windows.Forms.Padding(3);
            this.tabServer.Size = new System.Drawing.Size(612, 331);
            this.tabServer.TabIndex = 0;
            this.tabServer.Text = "Server";
            // 
            // grpCredentials
            // 
            this.grpCredentials.BackColor = System.Drawing.Color.Transparent;
            this.grpCredentials.Controls.Add(this.pnlClientCertificate);
            this.grpCredentials.Controls.Add(this.pnlMailbox);
            this.grpCredentials.Controls.Add(this.pnlUsernamePassword);
            this.grpCredentials.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpCredentials.Location = new System.Drawing.Point(3, 114);
            this.grpCredentials.Name = "grpCredentials";
            this.grpCredentials.Size = new System.Drawing.Size(606, 136);
            this.grpCredentials.TabIndex = 1;
            this.grpCredentials.TabStop = false;
            this.grpCredentials.Text = "Credentials";
            // 
            // pnlClientCertificate
            // 
            this.pnlClientCertificate.Controls.Add(this.label8);
            this.pnlClientCertificate.Controls.Add(this.btnClearClientCertificate);
            this.pnlClientCertificate.Controls.Add(this.tbClientCertificate);
            this.pnlClientCertificate.Controls.Add(this.btnViewClientCertificate);
            this.pnlClientCertificate.Controls.Add(this.btnImportClientCertificate);
            this.pnlClientCertificate.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlClientCertificate.Location = new System.Drawing.Point(3, 91);
            this.pnlClientCertificate.Name = "pnlClientCertificate";
            this.pnlClientCertificate.Size = new System.Drawing.Size(600, 23);
            this.pnlClientCertificate.TabIndex = 3;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 5);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(85, 13);
            this.label8.TabIndex = 13;
            this.label8.Text = "Client certificate:";
            // 
            // btnClearClientCertificate
            // 
            this.btnClearClientCertificate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearClientCertificate.Location = new System.Drawing.Point(557, 0);
            this.btnClearClientCertificate.Name = "btnClearClientCertificate";
            this.btnClearClientCertificate.Size = new System.Drawing.Size(40, 23);
            this.btnClearClientCertificate.TabIndex = 2;
            this.btnClearClientCertificate.Text = "Clear";
            this.btnClearClientCertificate.UseVisualStyleBackColor = true;
            this.btnClearClientCertificate.Click += new System.EventHandler(this.btnClearClientCertificate_Click);
            // 
            // tbClientCertificate
            // 
            this.tbClientCertificate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbClientCertificate.Location = new System.Drawing.Point(99, 2);
            this.tbClientCertificate.Name = "tbClientCertificate";
            this.tbClientCertificate.ReadOnly = true;
            this.tbClientCertificate.Size = new System.Drawing.Size(339, 20);
            this.tbClientCertificate.TabIndex = 0;
            this.tbClientCertificate.TabStop = false;
            // 
            // btnViewClientCertificate
            // 
            this.btnViewClientCertificate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnViewClientCertificate.Location = new System.Drawing.Point(505, 0);
            this.btnViewClientCertificate.Name = "btnViewClientCertificate";
            this.btnViewClientCertificate.Size = new System.Drawing.Size(48, 23);
            this.btnViewClientCertificate.TabIndex = 1;
            this.btnViewClientCertificate.Text = "View...";
            this.btnViewClientCertificate.UseVisualStyleBackColor = true;
            this.btnViewClientCertificate.Click += new System.EventHandler(this.btnViewClientCertificate_Click);
            // 
            // btnImportClientCertificate
            // 
            this.btnImportClientCertificate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnImportClientCertificate.Location = new System.Drawing.Point(444, 0);
            this.btnImportClientCertificate.Name = "btnImportClientCertificate";
            this.btnImportClientCertificate.Size = new System.Drawing.Size(55, 23);
            this.btnImportClientCertificate.TabIndex = 0;
            this.btnImportClientCertificate.Text = "Import...";
            this.btnImportClientCertificate.UseVisualStyleBackColor = true;
            this.btnImportClientCertificate.Click += new System.EventHandler(this.btnImportClientCertificate_Click);
            // 
            // pnlMailbox
            // 
            this.pnlMailbox.Controls.Add(this.tbMailbox);
            this.pnlMailbox.Controls.Add(this.lblMailbox);
            this.pnlMailbox.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlMailbox.Location = new System.Drawing.Point(3, 68);
            this.pnlMailbox.Name = "pnlMailbox";
            this.pnlMailbox.Size = new System.Drawing.Size(600, 23);
            this.pnlMailbox.TabIndex = 4;
            // 
            // tbMailbox
            // 
            this.tbMailbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbMailbox.Location = new System.Drawing.Point(99, 2);
            this.tbMailbox.Name = "tbMailbox";
            this.tbMailbox.Size = new System.Drawing.Size(339, 20);
            this.tbMailbox.TabIndex = 0;
            // 
            // lblMailbox
            // 
            this.lblMailbox.AutoSize = true;
            this.lblMailbox.Location = new System.Drawing.Point(3, 5);
            this.lblMailbox.Name = "lblMailbox";
            this.lblMailbox.Size = new System.Drawing.Size(82, 13);
            this.lblMailbox.TabIndex = 13;
            this.lblMailbox.Text = "Shared mailbox:";
            // 
            // pnlUsernamePassword
            // 
            this.pnlUsernamePassword.Controls.Add(this.cbSingleSignOn);
            this.pnlUsernamePassword.Controls.Add(this.cbStorePassword);
            this.pnlUsernamePassword.Controls.Add(this.tbServerUser);
            this.pnlUsernamePassword.Controls.Add(this.label4);
            this.pnlUsernamePassword.Controls.Add(this.label5);
            this.pnlUsernamePassword.Controls.Add(this.tbServerPassword);
            this.pnlUsernamePassword.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlUsernamePassword.Location = new System.Drawing.Point(3, 16);
            this.pnlUsernamePassword.Name = "pnlUsernamePassword";
            this.pnlUsernamePassword.Size = new System.Drawing.Size(600, 52);
            this.pnlUsernamePassword.TabIndex = 0;
            // 
            // cbSingleSignOn
            // 
            this.cbSingleSignOn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbSingleSignOn.AutoSize = true;
            this.cbSingleSignOn.Location = new System.Drawing.Point(444, 4);
            this.cbSingleSignOn.Name = "cbSingleSignOn";
            this.cbSingleSignOn.Size = new System.Drawing.Size(96, 17);
            this.cbSingleSignOn.TabIndex = 16;
            this.cbSingleSignOn.TabStop = false;
            this.cbSingleSignOn.Text = "Single Sign On";
            this.cbSingleSignOn.UseVisualStyleBackColor = true;
            this.cbSingleSignOn.CheckedChanged += new System.EventHandler(this.cbSingleSignOn_CheckedChanged);
            // 
            // cbStorePassword
            // 
            this.cbStorePassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbStorePassword.AutoSize = true;
            this.cbStorePassword.Location = new System.Drawing.Point(444, 30);
            this.cbStorePassword.Name = "cbStorePassword";
            this.cbStorePassword.Size = new System.Drawing.Size(99, 17);
            this.cbStorePassword.TabIndex = 15;
            this.cbStorePassword.TabStop = false;
            this.cbStorePassword.Text = "Store password";
            this.cbStorePassword.UseVisualStyleBackColor = true;
            // 
            // tbServerUser
            // 
            this.tbServerUser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbServerUser.Location = new System.Drawing.Point(99, 2);
            this.tbServerUser.Name = "tbServerUser";
            this.tbServerUser.Size = new System.Drawing.Size(339, 20);
            this.tbServerUser.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 5);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "User name:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 31);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Password:";
            // 
            // tbServerPassword
            // 
            this.tbServerPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbServerPassword.Location = new System.Drawing.Point(99, 28);
            this.tbServerPassword.Name = "tbServerPassword";
            this.tbServerPassword.PasswordChar = '*';
            this.tbServerPassword.Size = new System.Drawing.Size(339, 20);
            this.tbServerPassword.TabIndex = 1;
            // 
            // grpConnection
            // 
            this.grpConnection.Controls.Add(this.pnlHostnameProtocol);
            this.grpConnection.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpConnection.Location = new System.Drawing.Point(3, 3);
            this.grpConnection.Name = "grpConnection";
            this.grpConnection.Size = new System.Drawing.Size(606, 111);
            this.grpConnection.TabIndex = 0;
            this.grpConnection.TabStop = false;
            this.grpConnection.Text = "Connection";
            // 
            // pnlHostnameProtocol
            // 
            this.pnlHostnameProtocol.Controls.Add(this.tbServerPort);
            this.pnlHostnameProtocol.Controls.Add(this.label3);
            this.pnlHostnameProtocol.Controls.Add(this.label1);
            this.pnlHostnameProtocol.Controls.Add(this.cmbProtocol);
            this.pnlHostnameProtocol.Controls.Add(this.tbServerHost);
            this.pnlHostnameProtocol.Controls.Add(this.label2);
            this.pnlHostnameProtocol.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHostnameProtocol.Location = new System.Drawing.Point(3, 16);
            this.pnlHostnameProtocol.Margin = new System.Windows.Forms.Padding(0);
            this.pnlHostnameProtocol.Name = "pnlHostnameProtocol";
            this.pnlHostnameProtocol.Size = new System.Drawing.Size(600, 55);
            this.pnlHostnameProtocol.TabIndex = 138;
            // 
            // tbServerPort
            // 
            this.tbServerPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbServerPort.Location = new System.Drawing.Point(468, 3);
            this.tbServerPort.Name = "tbServerPort";
            this.tbServerPort.Size = new System.Drawing.Size(129, 20);
            this.tbServerPort.TabIndex = 1;
            this.tbServerPort.TextChanged += new System.EventHandler(this.tbServerHostOrPort_TextChanged);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(441, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 27;
            this.label3.Text = "Port:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 26;
            this.label1.Text = "Protocol:";
            // 
            // cmbProtocol
            // 
            this.cmbProtocol.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbProtocol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProtocol.FormattingEnabled = true;
            this.cmbProtocol.Location = new System.Drawing.Point(99, 30);
            this.cmbProtocol.Name = "cmbProtocol";
            this.cmbProtocol.Size = new System.Drawing.Size(339, 21);
            this.cmbProtocol.TabIndex = 2;
            this.cmbProtocol.SelectedIndexChanged += new System.EventHandler(this.cmbProtocol_SelectedIndexChanged);
            // 
            // tbServerHost
            // 
            this.tbServerHost.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbServerHost.Location = new System.Drawing.Point(99, 3);
            this.tbServerHost.Name = "tbServerHost";
            this.tbServerHost.Size = new System.Drawing.Size(339, 20);
            this.tbServerHost.TabIndex = 0;
            this.tbServerHost.TextChanged += new System.EventHandler(this.tbServerHostOrPort_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 25;
            this.label2.Text = "Host name:";
            // 
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.tabServer);
            this.tabControlMain.Controls.Add(this.tabProxy);
            this.tabControlMain.Controls.Add(this.tabSsl);
            this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlMain.Location = new System.Drawing.Point(0, 0);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(620, 357);
            this.tabControlMain.TabIndex = 1008;
            // 
            // UcConnectionEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControlMain);
            this.MinimumSize = new System.Drawing.Size(620, 0);
            this.Name = "UcConnectionEditor";
            this.Size = new System.Drawing.Size(620, 357);
            this.tabSsl.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.pnlServerCertificate.ResumeLayout(false);
            this.pnlServerCertificate.PerformLayout();
            this.pnlLocalyStoredCertificate.ResumeLayout(false);
            this.pnlLocalyStoredCertificate.PerformLayout();
            this.tabProxy.ResumeLayout(false);
            this.pnlProxy.ResumeLayout(false);
            this.pnlProxy.PerformLayout();
            this.tabServer.ResumeLayout(false);
            this.grpCredentials.ResumeLayout(false);
            this.pnlClientCertificate.ResumeLayout(false);
            this.pnlClientCertificate.PerformLayout();
            this.pnlMailbox.ResumeLayout(false);
            this.pnlMailbox.PerformLayout();
            this.pnlUsernamePassword.ResumeLayout(false);
            this.pnlUsernamePassword.PerformLayout();
            this.grpConnection.ResumeLayout(false);
            this.pnlHostnameProtocol.ResumeLayout(false);
            this.pnlHostnameProtocol.PerformLayout();
            this.tabControlMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private TabPage tabSsl;
        private Panel pnlServerCertificate;
        private Panel pnlLocalyStoredCertificate;
        private Label label9;
        private TextBox tbServerCertificateThumbprint;
        private Label label10;
        private RadioButton rbServerCertificateAny;
        private RadioButton rbServerCertificateStored;
        private RadioButton rbServerCertificateWindows;
        private Label label7;
        private TabPage tabProxy;
        private Panel pnlProxy;
        private Label label19;
        private TextBox tbProxyUsername;
        private Label label11;
        private ComboBox cmbProxyType;
        private Label label13;
        private Label label14;
        private TextBox tbProxyPassword;
        private TextBox tbProxyPort;
        private Label label6;
        private TextBox tbProxyHost;
        private Label label12;
        private TabPage tabServer;
        private GroupBox grpCredentials;
        private Panel pnlUsernamePassword;
        private CheckBox cbStorePassword;
        private TextBox tbServerUser;
        private Label label4;
        private Label label5;
        private TextBox tbServerPassword;
        private GroupBox grpConnection;
        private Panel pnlHostnameProtocol;
        private TextBox tbServerPort;
        private Label label3;
        private Label label1;
        private ComboBox cmbProtocol;
        private TextBox tbServerHost;
        private Label label2;
        private TabControl tabControlMain;
        private Panel pnlClientCertificate;
        private Label label8;
        private Button btnClearClientCertificate;
        private TextBox tbClientCertificate;
        private Button btnViewClientCertificate;
        private Button btnImportClientCertificate;
        private Panel pnlMailbox;
        private TextBox tbMailbox;
        private Label lblMailbox;
        private Panel panel1;
        private ComboBox cmbSuites;
        private CheckBox cbAllowTls13;
        private CheckBox cbAllowTls12;
        private CheckBox cbAllowTls11;
        private CheckBox cbAllowTls10;
        private Label label23;
        private Label label22;
        private CheckBox cbSingleSignOn;
    }
}