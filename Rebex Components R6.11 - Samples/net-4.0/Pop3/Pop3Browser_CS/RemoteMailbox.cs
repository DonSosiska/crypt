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
using System.IO;
using System.Windows.Forms;
using System.Text;
using Rebex.Mime;
using Rebex.Net;
using Rebex.Samples;

namespace Rebex.Samples
{
    /// <summary>
    /// Application's main form.
    /// </summary>
    public class RemoteMailbox : System.Windows.Forms.Form
    {
        public static readonly string ConfigFilePath = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), string.Format("Rebex{0}Secure Mail{0}Pop3Browser.xml",Path.DirectorySeparatorChar));

        // Background worker object.
        private readonly Worker _worker;

        // Application configuration.
        private readonly Configuration _config;

        // Allowed TLS/SSL protocol.
        private TlsVersion _protocol = SecurityConfig.DefaultTlsVersion;

        // Allowed TLS/SSL cipher suite.
        private TlsCipherSuite _suite = TlsCipherSuite.All;

        // ListViewItem sorting object.
        private ListViewItemSorter _listViewItemSorter;

        private string _lastUserName;
        private string _lastPassword;

        #region Controls

        private System.Windows.Forms.MenuStrip menuMain;
        private System.Windows.Forms.ToolStripMenuItem menuMailbox;
        private System.Windows.Forms.ToolStripMenuItem menuClear;
        private System.Windows.Forms.ContextMenuStrip menuContext;
        private System.Windows.Forms.ToolStripMenuItem menuView;
        private System.Windows.Forms.ToolStripMenuItem menuDelete;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.TextBox txtServer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.ColumnHeader columnHeaderFrom;
        private System.Windows.Forms.ColumnHeader columnHeaderSubject;
        private System.Windows.Forms.ColumnHeader columnHeaderSize;
        private System.Windows.Forms.ColumnHeader columnHeaderDate;
        private System.Windows.Forms.ToolStripMenuItem menuItemProperties;
        private System.Windows.Forms.ToolStripMenuItem menuItemSave;
        private System.Windows.Forms.ToolStripMenuItem menuExit;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.ComboBox cbSecurity;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox cbSingleSignOn;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        #endregion

        /// <summary>
        /// Initializes the RemoteMailbox instance and loads the configuration.
        /// </summary>
        public RemoteMailbox()
        {
            InitializeComponent();

            CheckForIllegalCrossThreadCalls = true;

            _config = new Configuration(ConfigFilePath);
            _worker = new Worker(this, _config);

            txtServer.Text = _config.GetString("server");
            txtPort.Text = _config.GetInt32("port", Pop3.DefaultPort).ToString();
            txtUserName.Text = _config.GetString("userName");
            txtPassword.Text = _config.GetString("password");
            cbSecurity.SelectedIndex = _config.GetInt32("security", 0);
            _protocol = (TlsVersion)_config.GetValue("protocol", typeof(TlsVersion));
            _suite = (TlsCipherSuite)_config.GetValue("suite", typeof(TlsCipherSuite));
            cbSingleSignOn.Checked = _config.GetBoolean("singleSignOn", false);

            if (_protocol == TlsVersion.None)
                _protocol = SecurityConfig.DefaultTlsVersion;
            if (_suite == TlsCipherSuite.None)
                _suite = TlsCipherSuite.All;

            _listViewItemSorter = new ListViewItemSorter();
            _listViewItemSorter.SortColumn = this.columnHeaderDate.Index;
            _listViewItemSorter.Sorting = SortOrder.Descending;
            listView.ListViewItemSorter = _listViewItemSorter;
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

        /// <summary>
        /// Wrapper for Invoke method that doesn't throw an exception after the object has been
        /// disposed while the calling method was running in a background thread.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="args"></param>
        public void SafeInvoke(Delegate method, params object[] args)
        {
            try
            {
                if (!IsDisposed)
                    Invoke(method, args);
            }
            catch (ObjectDisposedException) { }
        }

        /// <summary>
        /// Updates the status bar text. Called from a background thread.
        /// </summary>
        public void SetStatus(string message)
        {
            statusLabel.Text = (message != null) ? message : "";
        }

        /// <summary>
        /// Adds a message info to the list. Called from a background thread.
        /// </summary>
        /// <param name="message">Message info.</param>
        /// <param name="error">True if message was unparsable.</param>
        public void AddMessage(Pop3MessageInfo message, bool error)
        {
            if (message.HeadersParsed)
            {
                string subject = (message.Subject != null) ? message.Subject : "n/a";
                string date = (message.Date != null) ? message.Date.LocalTime.ToString("yyyy-MM-dd HH:mm:ss") : "n/a";
                ListViewItem item = new ListViewItem(message.From.ToString());
                item.Tag = message;
                item.SubItems.Add(subject);
                item.SubItems.Add(message.Length.ToString());
                item.SubItems.Add(date);
                listView.Items.Add(item);
            }
            else
            {
                ListViewItem item = new ListViewItem("n/a");
                item.Tag = message;
                item.SubItems.Add("n/a");
                item.SubItems.Add(message.Length.ToString());
                item.SubItems.Add("n/a");
                item.ForeColor = Color.Red;
                listView.Items.Add(item);
            }
        }

        /// <summary>
        /// Removes a message from the list. Called from a background thread.
        /// </summary>
        /// <param name="uniqueId">Message unique ID.</param>
        public void RemoveMessage(string uniqueId)
        {
            try
            {
                foreach (ListViewItem item in listView.Items)
                {
                    Pop3MessageInfo m = (Pop3MessageInfo)item.Tag;
                    if (m.UniqueId == uniqueId)
                    {
                        listView.Items.Remove(item);
                        return;
                    }
                }

                throw new Exception("Message not found.");
            }
            catch (Exception x)
            {
                ReportError(x);
            }
        }

        /// <summary>
        /// Displays a message. Called from a background thread.
        /// </summary>
        /// <param name="message">Mime message.</param>
        /// <param name="raw">Raw mssage data.</param>
        public void ShowMessage(MimeMessage message, byte[] raw)
        {
            try
            {
                SetStatus("Formatting message...");
                MessageView v = new MessageView(message, raw);
                v.Show();
            }
            catch (Exception x)
            {
                ReportError(x);
            }
        }

        /// <summary>
        /// Enables/disables fields which cannot be changed
        /// while the background update is in progress.
        /// </summary>
        private void SetSessionConfigEnabled(bool state)
        {
            txtServer.Enabled = state;
            txtPort.Enabled = state;
            cbSingleSignOn.Enabled = state;

            if (!cbSingleSignOn.Checked)
            {
                txtUserName.Enabled = state;
                txtPassword.Enabled = state;
            }

            cbSecurity.Enabled = state;
            btnSettings.Enabled = state;
        }

        private void SetContextMenuEnabled(bool state)
        {
            menuView.Enabled = state;
            menuDelete.Enabled = state;
            menuItemSave.Enabled = state;
            menuItemProperties.Enabled = state;
        }

        /// <summary>
        /// Re-enables session config fields.
        /// </summary>
        private void OnFinished(Exception error)
        {
            if (error == null)
            {
                btnOpen.Text = "&Refresh";
                SetSessionConfigEnabled(false);
            }
            else
            {
                ReportError(error);
                SetSessionConfigEnabled(true);
            }
            SetContextMenuEnabled(true);
        }

        /// <summary>
        /// Reports an error to the user.
        /// </summary>
        /// <param name="error">Error.</param>
        private void ReportError(Exception error)
        {
            statusLabel.Text = "";

            if (listView.Items.Count == 0)
                btnOpen.Text = "&Open";
            else
                btnOpen.Text = "&Refresh";

            string message;

            Pop3Exception pop3error = error as Pop3Exception;
            if (pop3error != null)
            {
                if (pop3error.Status == Pop3ExceptionStatus.ProtocolError)
                    message = string.Format("Server reported error: {0}", error.Message);
                else
                    message = string.Format("Error occured: {0}", error.Message);
            }
            else
            {
                message = error.ToString();
            }

            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Prepares the form to start a background operation.
        /// </summary>
        private void PrepareForWorker()
        {
            if (txtPort.Text.Trim() == string.Empty)
                txtPort.Text = Pop3.DefaultPort.ToString();

            _config.SetValue("server", txtServer.Text);
            _config.SetValue("port", int.Parse(txtPort.Text));
            _config.SetValue("userName", txtUserName.Text);
            _config.SetValue("password", txtPassword.Text);
            _config.SetValue("singleSignOn", cbSingleSignOn.Checked);
            _config.SetValue("security", cbSecurity.SelectedIndex);
            _config.SetValue("protocol", _protocol);
            _config.SetValue("suite", _suite);
            _config.Save();

            btnOpen.Text = "&Break";
            SetSessionConfigEnabled(false);
            SetContextMenuEnabled(false);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(RemoteMailbox));
            this.menuMain = new System.Windows.Forms.MenuStrip();
            this.menuMailbox = new System.Windows.Forms.ToolStripMenuItem();
            this.menuClear = new System.Windows.Forms.ToolStripMenuItem();
            this.menuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuContext = new System.Windows.Forms.ContextMenuStrip();
            this.menuView = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemProperties = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSave = new System.Windows.Forms.ToolStripMenuItem();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnOpen = new System.Windows.Forms.Button();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.listView = new System.Windows.Forms.ListView();
            this.columnHeaderFrom = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderSubject = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderSize = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderDate = new System.Windows.Forms.ColumnHeader();
            this.btnSettings = new System.Windows.Forms.Button();
            this.cbSecurity = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cbSingleSignOn = new System.Windows.Forms.CheckBox();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.menuMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuMain
            // 
            this.menuMain.Items.AddRange(new System.Windows.Forms.ToolStripMenuItem[] {
                                                                                     this.menuMailbox});
            // 
            // menuMailbox
            // 
            this.menuMailbox.MergeIndex = 0;
            this.menuMailbox.DropDownItems.AddRange(new System.Windows.Forms.ToolStripMenuItem[] {
                                                                                        this.menuClear,
                                                                                        this.menuExit});
            this.menuMailbox.Text = "Mailbox";
            // 
            // menuClear
            // 
            this.menuClear.MergeIndex = 0;
            this.menuClear.Text = "&Clear";
            this.menuClear.Click += new System.EventHandler(this.menuClear_Click);
            // 
            // menuExit
            // 
            this.menuExit.MergeIndex = 1;
            this.menuExit.Text = "E&xit";
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // menuContext
            // 
            this.menuContext.Items.AddRange(new System.Windows.Forms.ToolStripMenuItem[] {
                                                                                        this.menuView,
                                                                                        this.menuDelete,
                                                                                        this.menuItemProperties,
                                                                                        this.menuItemSave});
            // 
            // menuView
            // 
            this.menuView.MergeIndex = 0;
            this.menuView.Text = "&View...";
            this.menuView.Click += new System.EventHandler(this.menuView_Click);
            // 
            // menuDelete
            // 
            this.menuDelete.MergeIndex = 1;
            this.menuDelete.Text = "&Delete";
            this.menuDelete.Click += new System.EventHandler(this.menuDelete_Click);
            // 
            // menuItemProperties
            // 
            this.menuItemProperties.MergeIndex = 2;
            this.menuItemProperties.Text = "&Properties...";
            this.menuItemProperties.Click += new System.EventHandler(this.menuItemProperties_Click);
            // 
            // menuItemSave
            // 
            this.menuItemSave.MergeIndex = 3;
            this.menuItemSave.Text = "&Save As...";
            this.menuItemSave.Click += new System.EventHandler(this.menuItemSave_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(520, 108);
            this.panel2.TabIndex = 2;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.btnSettings);
            this.panel3.Controls.Add(this.cbSecurity);
            this.panel3.Controls.Add(this.label5);
            this.panel3.Controls.Add(this.btnOpen);
            this.panel3.Controls.Add(this.txtPassword);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Controls.Add(this.txtPort);
            this.panel3.Controls.Add(this.label3);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(240, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(280, 108);
            this.panel3.TabIndex = 3;
            // 
            // btnOpen
            // 
            this.btnOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpen.Location = new System.Drawing.Point(192, 16);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(80, 23);
            this.btnOpen.TabIndex = 5;
            this.btnOpen.Text = "Open";
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassword.Location = new System.Drawing.Point(72, 48);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(200, 20);
            this.txtPassword.TabIndex = 4;
            this.txtPassword.Text = "";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(8, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 16);
            this.label4.TabIndex = 7;
            this.label4.Text = "Password:";
            // 
            // txtPort
            // 
            this.txtPort.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPort.Location = new System.Drawing.Point(72, 16);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(112, 20);
            this.txtPort.TabIndex = 2;
            this.txtPort.Text = "110";
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.SystemColors.Control;
            this.label3.Location = new System.Drawing.Point(8, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 16);
            this.label3.TabIndex = 5;
            this.label3.Text = "Port:";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.txtUserName);
            this.panel4.Controls.Add(this.txtServer);
            this.panel4.Controls.Add(this.cbSingleSignOn);
            this.panel4.Controls.Add(this.label2);
            this.panel4.Controls.Add(this.label1);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(240, 108);
            this.panel4.TabIndex = 2;
            // 
            // txtUserName
            // 
            this.txtUserName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUserName.Location = new System.Drawing.Point(96, 48);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(128, 20);
            this.txtUserName.TabIndex = 3;
            this.txtUserName.Text = "";
            //
            // cbSingleSignOn
            //
            this.cbSingleSignOn.Location = new System.Drawing.Point(8, 80);
            this.cbSingleSignOn.Name = "cbSingleSignOn";
            this.cbSingleSignOn.Size = new System.Drawing.Size(200, 20);
            this.cbSingleSignOn.Text = "Single sign-on authentication";
            this.cbSingleSignOn.Checked = false;
            this.cbSingleSignOn.Enabled = true;
            this.cbSingleSignOn.CheckedChanged += new EventHandler(cbSingleSignOn_CheckedChanged);
            // 
            // txtServer
            // 
            this.txtServer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.txtServer.Location = new System.Drawing.Point(96, 16);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(128, 20);
            this.txtServer.TabIndex = 1;
            this.txtServer.Text = "";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 16);
            this.label2.TabIndex = 6;
            this.label2.Text = "Username:";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 16);
            this.label1.TabIndex = 4;
            this.label1.Text = "Hostname:";
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 368);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(520, 22);
            this.statusStrip.TabIndex = 0;
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(90, 21);
            // 
            // listView
            // 
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                                                                                       this.columnHeaderFrom,
                                                                                       this.columnHeaderSubject,
                                                                                       this.columnHeaderSize,
                                                                                       this.columnHeaderDate});
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.FullRowSelect = true;
            this.listView.GridLines = true;
            this.listView.Location = new System.Drawing.Point(0, 108);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(520, 260);
            this.listView.TabIndex = 6;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.DoubleClick += new System.EventHandler(this.listView_DoubleClick);
            this.listView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.listView_MouseUp);
            this.listView.ColumnClick += new ColumnClickEventHandler(this.listView_ColumnClick);
            // 
            // columnHeaderFrom
            // 
            this.columnHeaderFrom.Text = "From";
            this.columnHeaderFrom.Width = 120;
            // 
            // columnHeaderSubject
            // 
            this.columnHeaderSubject.Text = "Subject";
            this.columnHeaderSubject.Width = 160;
            // 
            // columnHeaderSize
            // 
            this.columnHeaderSize.Text = "Size (bytes)";
            this.columnHeaderSize.Width = 70;
            // 
            // columnHeaderDate
            // 
            this.columnHeaderDate.Text = "Date";
            this.columnHeaderDate.Width = 120;
            // 
            // btnSettings
            // 
            this.btnSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSettings.Location = new System.Drawing.Point(192, 80);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(80, 23);
            this.btnSettings.TabIndex = 14;
            this.btnSettings.Text = "Settings...";
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // cbSecurity
            // 
            this.cbSecurity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSecurity.Items.AddRange(new object[] {
                                                            "No security",
                                                            "Explicit TLS/SSL",
                                                            "Implicit TLS/SSL"});
            this.cbSecurity.Location = new System.Drawing.Point(72, 80);
            this.cbSecurity.MaxDropDownItems = 3;
            this.cbSecurity.Name = "cbSecurity";
            this.cbSecurity.Size = new System.Drawing.Size(120, 21);
            this.cbSecurity.TabIndex = 13;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(8, 80);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 16);
            this.label5.TabIndex = 12;
            this.label5.Text = "Security:";
            // 
            // RemoteMailbox
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(520, 390);
            this.Controls.Add(this.listView);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuMain;
            this.Name = "RemoteMailbox";
            this.Text = "POP3 Mailbox Browser";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.RemoteMailbox_Closing);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.menuMain.ResumeLayout(false);
            this.menuMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        // Cancels the close request if a background operation is running.
        private void RemoteMailbox_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = _worker.Running;
        }

        // Clears the message list.
        private void menuClear_Click(object sender, System.EventArgs e)
        {
            listView.Items.Clear();
            _worker.ForgetMessages();
        }

        // Close the application.
        private void menuExit_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        // View message.
        private void menuView_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (_worker.Running)
                    return;

                ListView.SelectedListViewItemCollection selCol = listView.SelectedItems;
                if (selCol != null && selCol.Count == 1)
                {
                    Pop3MessageInfo m = (Pop3MessageInfo)selCol[0].Tag;

                    PrepareForWorker();
                    _worker.StartRetrieveMessage(m.UniqueId, new Worker.FinishedDelegate(OnFinished));
                }
            }
            catch (Exception x)
            {
                ReportError(x);
            }
        }

        // Delete message.
        private void menuDelete_Click(object sender, System.EventArgs e)
        {
            try
            {
                ListView.SelectedListViewItemCollection selCol = listView.SelectedItems;
                if ((selCol != null) && (selCol.Count > 0))
                {
                    string[] uniqueIds = new string[selCol.Count];
                    for (int i = 0; i < selCol.Count; i++)
                    {
                        Pop3MessageInfo m = (Pop3MessageInfo)selCol[i].Tag;
                        uniqueIds[i] = m.UniqueId;
                    }

                    PrepareForWorker();
                    _worker.StartDeleteMessages(uniqueIds, new Worker.FinishedDelegate(OnFinished));
                }
            }
            catch (Exception x)
            {
                ReportError(x);
            }
        }

        // Show message properties.
        private void menuItemProperties_Click(object sender, System.EventArgs e)
        {
            try
            {
                ListView.SelectedListViewItemCollection selCol = listView.SelectedItems;
                if (selCol != null && selCol.Count == 1)
                {
                    Pop3MessageInfo m = (Pop3MessageInfo)selCol[0].Tag;

                    string info = string.Format("Unique ID: {0}\r\n\r\nLength: {1}", m.UniqueId, m.Length);
                    MessageBox.Show(info, "Message Properties");
                }
            }
            catch (Exception x)
            {
                ReportError(x);
            }
        }

        // Save the message.
        private void menuItemSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                ListView.SelectedListViewItemCollection selCol = listView.SelectedItems;
                if (selCol != null && selCol.Count == 1)
                {
                    Pop3MessageInfo m = (Pop3MessageInfo)selCol[0].Tag;

                    PrepareForWorker();
                    SaveFileDialog save = new SaveFileDialog();
                    save.AddExtension = true;
                    save.Filter = "MIME mails (*.eml)|*.eml|Outlook mails (*.msg)|*.msg|All files (*.*)|*.*";
                    save.DefaultExt = "eml";
                    save.FileName = FixFilename(m.UniqueId) + ".eml";
                    DialogResult res = save.ShowDialog();
                    if (res != DialogResult.OK)
                        return;
                    _worker.StartSaveMessage(m.UniqueId, save.FileName, new Worker.FinishedDelegate(OnFinished));
                }
            }
            catch (Exception x)
            {
                ReportError(x);
            }
        }

        /// <summary>
        /// Creates a valid filename from the supplied string.
        /// </summary>
        /// <param name="originalFilename">String to be converted - e.g. mail subject.</param>
        /// <returns></returns>
        private static string FixFilename(string originalFilename)
        {
            // Characters allowed in the filename
            string allowed = " .-0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

            // replace invalid charactes with it's hex representation
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < originalFilename.Length; i++)
            {
                if (allowed.IndexOf(originalFilename[i]) < 0)
                    sb.AppendFormat("_{0:X2}", (int)originalFilename[i]);
                else
                    sb.Append(originalFilename[i]);
            }
            return sb.ToString();
        }

        // List messages or abort operation
        private void btnOpen_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (!_worker.Running)
                {
                    PrepareForWorker();
                    _worker.StartRetrieveMessageList(new Worker.FinishedDelegate(OnFinished));
                }
                else
                {
                    _worker.Abort();
                }
            }
            catch (Exception x)
            {
                ReportError(x);
            }
        }

        // TLS/SSL settings
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

        // Message selected.
        private void listView_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ListViewItem sel = listView.GetItemAt(e.X, e.Y);
                if (sel != null)
                {
                    ListView.SelectedListViewItemCollection selCol = listView.SelectedItems;
                    if (selCol != null)
                    {
                        if (selCol.Count > 0)
                            menuContext.Show(listView, new Point(e.X, e.Y));
                    }
                }
            }
        }

        // Message double-clicked.
        private void listView_DoubleClick(object sender, System.EventArgs e)
        {
            menuView_Click(sender, e);
        }

        // Column header click
        private void listView_ColumnClick(object sender, System.Windows.Forms.ColumnClickEventArgs e)
        {
            _listViewItemSorter.SortColumn = e.Column;

            if (e.Column == this.columnHeaderSize.Index)
                _listViewItemSorter.CompareType = ListViewItemSorter.CompareTypes.Integers;
            else
                _listViewItemSorter.CompareType = ListViewItemSorter.CompareTypes.Strings;

            if (_listViewItemSorter.Sorting == SortOrder.Descending)
                _listViewItemSorter.Sorting = SortOrder.Ascending;
            else
                _listViewItemSorter.Sorting = SortOrder.Descending;

            listView.Sort();
        }

        // Single sign on CheckBox changed.
        private void cbSingleSignOn_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSingleSignOn.Checked)
            {
                // turning on single sign
                // save password and username for later use without single sign on
                _lastUserName = txtUserName.Text;
                _lastPassword = txtPassword.Text;
                // hide the username and password from GUI
                txtPassword.Text = String.Empty;
                txtUserName.Text = String.Empty;
                txtPassword.Enabled = false;
                txtUserName.Enabled = false;
            }
            else
            {
                // disabling single sign on
                // restore the saved username and password
                txtUserName.Text = _lastUserName;
                txtPassword.Text = _lastPassword;
                txtPassword.Enabled = true;
                txtUserName.Enabled = true;
            }
        }
    }
}
