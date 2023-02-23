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
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using Rebex.Mail;
using Rebex.Mime;
using Rebex.Mime.Headers;

namespace Rebex.Samples
{
    /// <summary>
    /// MailMessage editor.
    /// </summary>
    public class MailEditor : System.Windows.Forms.Form
    {
        protected System.Windows.Forms.MenuStrip mainMenu;
        protected System.Windows.Forms.ToolStripMenuItem menuItemFile;
        protected System.Windows.Forms.ToolStripMenuItem menuItemNewMessage;
        protected System.Windows.Forms.ToolStripMenuItem menuItemSave;
        protected System.Windows.Forms.ToolStripMenuItem menuItemSaveAs;
        protected System.Windows.Forms.ToolStripMenuItem menuItemSaveAttachment;
        protected System.Windows.Forms.ToolStripMenuItem menuItemClose;
        protected System.Windows.Forms.ToolStripMenuItem menuItemEdit;
        protected System.Windows.Forms.ToolStripMenuItem menuItemCut;
        protected System.Windows.Forms.ToolStripMenuItem menuItemCopy;
        protected System.Windows.Forms.ToolStripMenuItem menuItemPaste;
        protected System.Windows.Forms.ToolStripMenuItem menuItemSelectAll;
        protected System.Windows.Forms.ToolStripMenuItem menuItemInsert;
        protected System.Windows.Forms.ToolStripMenuItem menuItemFileAttachment;
        protected System.Windows.Forms.ToolStripMenuItem menuItemOpenMessage;
        protected System.Windows.Forms.ToolStripSeparator menuItemSeparator;
        protected Rebex.Samples.MailTextEditor textEditor;
        protected System.Windows.Forms.OpenFileDialog openFileDialogMessage;
        protected System.Windows.Forms.SaveFileDialog saveFileDialogMessage;
        protected System.Windows.Forms.OpenFileDialog openFileDialogAttachment;
        protected System.Windows.Forms.SaveFileDialog saveFileDialogAttachment;
        protected System.Windows.Forms.TextBox txtFrom;
        protected System.Windows.Forms.Label lblFrom;
        protected System.Windows.Forms.ListBox listBoxAttachments;
        protected System.Windows.Forms.Label lblAttach;
        protected System.Windows.Forms.TextBox txtSubject;
        protected System.Windows.Forms.Label lblSubject;
        protected System.Windows.Forms.TextBox txtCc;
        protected System.Windows.Forms.TextBox txtTo;
        protected System.Windows.Forms.Label lblCc;
        protected System.Windows.Forms.Label lblTo;
        protected System.Windows.Forms.ContextMenuStrip contextMenuAttach;
        protected System.Windows.Forms.ToolStripMenuItem menuItemDelete;
        private IContainer components;

        // Folder browser dialog.
        private FolderBrowserDialog _folderBrowserDialog;

        // Indicates whether the message was saved.
        private bool _modified = false;

        // Current mail message.
        private MailMessage _message = null;

        // Indicates whether the message is currently editable.
        private bool _editable = true;

        // Message file path.
        private string _path;

        /// <summary>
        /// Enables or disables the editability of the message.
        /// </summary>
        public bool Editable
        {
            get
            {
                return _editable;
            }
            set
            {
                _editable = value;
                txtTo.ReadOnly = !value;
                txtCc.ReadOnly = !value;
                txtFrom.ReadOnly = !value;
                txtSubject.ReadOnly = !value;
                textEditor.ReadOnly = !value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the message is editable.
        /// </summary>
        public bool Modified
        {
            get
            {
                if (_message == null)
                    return false;
                return _modified | textEditor.Modified;
            }
            set
            {
                _modified = value;
                textEditor.Modified = value;
            }
        }

        /// <summary>
        /// Saves the message to its current location.
        /// </summary>
        protected virtual void SaveMessage()
        {
            if (_path == null)
            {
                _path = SaveAs();
                return;
            }

            SaveMessage(_path);
        }

        /// <summary>
        /// Saves message.
        /// </summary>
        /// <param name="path">Path to file.</param>
        protected virtual void SaveMessage(string path)
        {
            try
            {
                MailMessage message = ToMessage();
                string extension = Path.GetExtension(path);
                if (string.Equals(extension, ".msg", StringComparison.OrdinalIgnoreCase))
                    message.Save(path, MailFormat.OutlookMsg);
                else
                    message.Save(path, MailFormat.Mime);
                Modified = false;
            }
            catch (Exception x)
            {
                MessageBox.Show(x.ToString());
            }
        }

        /// <summary>
        /// Creates new message.
        /// </summary>
        protected virtual void NewMessage()
        {
            if (!CheckModifications())
                return;

            MailMessage message = new MailMessage();
            LoadMessage(message);
            Editable = true;
        }

        /// <summary>
        /// Closes the message.
        /// </summary>
        protected virtual void CloseMessage()
        {
            if (!CheckModifications())
                return;

            base.Close();
        }

        /// <summary>
        /// Opens a new message.
        /// </summary>
        protected virtual void OpenMessage()
        {
            if (!CheckModifications())
                return;

            if (openFileDialogMessage.ShowDialog() != DialogResult.OK)
                return;

            LoadMessage(openFileDialogMessage.FileName);
        }

        /// <summary>
        /// Asks the user whether to save or discard modifications, and save the message if desired.
        /// </summary>
        /// <returns>True if caller should proceed, false if cancel was selected.</returns>
        private bool CheckModifications()
        {
            if (!Modified)
                return true;

            DialogResult res = MessageBox.Show(this, "Would you like to save the changes you made to the current message?", "Message Editor", MessageBoxButtons.YesNoCancel);
            if (res == DialogResult.No)
                return true;
            if (res != DialogResult.Yes)
                return false;

            if (_path == null)
                _path = SaveAs();
            else
                SaveMessage(_path);

            return true;
        }

        /// <summary>
        /// Saves attachments to a folder.
        /// </summary>
        private void SaveAttachments()
        {
            if (_folderBrowserDialog.ShowDialog() != DialogResult.OK)
                return;

            string path = _folderBrowserDialog.SelectedPath;
            foreach (Attachment att in _message.Attachments)
            {
                att.Save(Path.Combine(path, att.FileName));
            }
        }

        private string SaveAs()
        {
            if (saveFileDialogMessage.ShowDialog() != DialogResult.OK)
                return null;

            string path = saveFileDialogMessage.FileName;

            SaveMessage(path);

            return path;
        }

        private void RemoveAttachment(string name)
        {
            listBoxAttachments.Items.Remove(name);

            for (int i = 0; i < _message.Attachments.Count; i++)
            {
                if (_message.Attachments[i].FileName == name)
                {
                    _message.Attachments.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// Loads a mail message.
        /// </summary>
        /// <param name="path">Path to file.</param>
        public void LoadMessage(string path)
        {
            try
            {
                MailMessage message = new MailMessage();
                _path = path;
                message.Load(_path);
                LoadMessage(message);
                Editable = true;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }

        /// <summary>
        /// Creates Mail message from dialog's data.
        /// </summary>
        /// <returns>Created mail message.</returns>
        public MailMessage ToMessage()
        {
            MailMessage msg = _message;

            msg.Subject = txtSubject.Text;
            msg.BodyText = textEditor.Text;

            return msg;
        }

        /// <summary>
        /// Initializes the editor with MailMessage data.
        /// </summary>
        /// <param name="message">Mail message.</param>
        public void LoadMessage(MailMessage message)
        {
            _message = message;
            txtFrom.Text = _message.From.ToString();
            txtTo.Text = _message.To.ToString();
            txtCc.Text = _message.CC.ToString();
            txtSubject.Text = _message.Subject;
            textEditor.Text = _message.BodyText;

            listBoxAttachments.Items.Clear();
            for (int i = 0; i < _message.Attachments.Count; i++)
            {
                Attachment attachment = _message.Attachments[i];

                string name = attachment.FileName;
                if (name == null)
                    name = string.Format("ATT{0}.DAT", i);

                listBoxAttachments.Items.Add(name);
            }

            Text = message.Subject;

            _modified = false;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public MailEditor()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            _folderBrowserDialog = new FolderBrowserDialog();

            Editable = true;
        }

        /// <summary>
        /// Cleans up any resources being used.
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

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.menuItemFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemNewMessage = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemOpenMessage = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSave = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSaveAttachment = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemClose = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemCut = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemInsert = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemFileAttachment = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialogMessage = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialogMessage = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialogAttachment = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialogAttachment = new System.Windows.Forms.SaveFileDialog();
            this.txtFrom = new System.Windows.Forms.TextBox();
            this.lblFrom = new System.Windows.Forms.Label();
            this.listBoxAttachments = new System.Windows.Forms.ListBox();
            this.contextMenuAttach = new System.Windows.Forms.ContextMenuStrip();
            this.menuItemDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.lblAttach = new System.Windows.Forms.Label();
            this.txtSubject = new System.Windows.Forms.TextBox();
            this.lblSubject = new System.Windows.Forms.Label();
            this.txtCc = new System.Windows.Forms.TextBox();
            this.txtTo = new System.Windows.Forms.TextBox();
            this.lblCc = new System.Windows.Forms.Label();
            this.lblTo = new System.Windows.Forms.Label();
            this.textEditor = new Rebex.Samples.MailTextEditor();
            this.mainMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripMenuItem[] {
            this.menuItemFile,
            this.menuItemEdit,
            this.menuItemInsert});
            // 
            // menuItemFile
            // 
            this.menuItemFile.MergeIndex = 0;
            this.menuItemFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemNewMessage,
            this.menuItemOpenMessage,
            this.menuItemSave,
            this.menuItemSaveAs,
            this.menuItemSaveAttachment,
            this.menuItemSeparator,
            this.menuItemClose});
            this.menuItemFile.Text = "&File";
            // 
            // menuItemNewMessage
            // 
            this.menuItemNewMessage.MergeIndex = 0;
            this.menuItemNewMessage.Text = "&New message";
            this.menuItemNewMessage.Click += new System.EventHandler(this.menuItemNewMessage_Click);
            // 
            // menuItemOpenMessage
            // 
            this.menuItemOpenMessage.MergeIndex = 1;
            this.menuItemOpenMessage.Text = "Open message...";
            this.menuItemOpenMessage.Click += new System.EventHandler(this.menuItemOpenMessage_Click);
            // 
            // menuItemSave
            // 
            this.menuItemSave.MergeIndex = 2;
            this.menuItemSave.Text = "S&ave";
            this.menuItemSave.Click += new System.EventHandler(this.menuItemSave_Click);
            // 
            // menuItemSaveAs
            // 
            this.menuItemSaveAs.MergeIndex = 3;
            this.menuItemSaveAs.Text = "Sa&ve as...";
            this.menuItemSaveAs.Click += new System.EventHandler(this.menuItemSaveAs_Click);
            // 
            // menuItemSaveAttachment
            // 
            this.menuItemSaveAttachment.MergeIndex = 4;
            this.menuItemSaveAttachment.Text = "Save a&ttachments...";
            this.menuItemSaveAttachment.Click += new System.EventHandler(this.menuItemSaveAttachments_Click);
            // 
            // menuItemSeparator
            // 
            this.menuItemSeparator.MergeIndex = 5;
            // 
            // menuItemClose
            // 
            this.menuItemClose.MergeIndex = 6;
            this.menuItemClose.Text = "&Close";
            this.menuItemClose.Click += new System.EventHandler(this.menuItemClose_Click);
            // 
            // menuItemEdit
            // 
            this.menuItemEdit.MergeIndex = 1;
            this.menuItemEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripMenuItem[] {
            this.menuItemCut,
            this.menuItemCopy,
            this.menuItemPaste,
            this.menuItemSelectAll});
            this.menuItemEdit.Text = "&Edit";
            // 
            // menuItemCut
            // 
            this.menuItemCut.MergeIndex = 0;
            this.menuItemCut.Text = "&Cut";
            this.menuItemCut.Click += new System.EventHandler(this.menuItemCut_Click);
            // 
            // menuItemCopy
            // 
            this.menuItemCopy.MergeIndex = 1;
            this.menuItemCopy.Text = "C&opy";
            this.menuItemCopy.Click += new System.EventHandler(this.menuItemCopy_Click);
            // 
            // menuItemPaste
            // 
            this.menuItemPaste.MergeIndex = 2;
            this.menuItemPaste.Text = "&Paste";
            this.menuItemPaste.Click += new System.EventHandler(this.menuItemPaste_Click);
            // 
            // menuItemSelectAll
            // 
            this.menuItemSelectAll.MergeIndex = 3;
            this.menuItemSelectAll.Text = "&Select all";
            this.menuItemSelectAll.Click += new System.EventHandler(this.menuItemSelectAll_Click);
            // 
            // menuItemInsert
            // 
            this.menuItemInsert.MergeIndex = 2;
            this.menuItemInsert.DropDownItems.AddRange(new System.Windows.Forms.ToolStripMenuItem[] {
            this.menuItemFileAttachment});
            this.menuItemInsert.Text = "&Insert";
            // 
            // menuItemFileAttachment
            // 
            this.menuItemFileAttachment.MergeIndex = 0;
            this.menuItemFileAttachment.Text = "&Attachment...";
            this.menuItemFileAttachment.Click += new System.EventHandler(this.menuItemFileAttachment_Click);
            // 
            // openFileDialogMessage
            // 
            this.openFileDialogMessage.Filter = "MIME mails (*.eml)|*.eml|Outlook mails (*.msg)|*.msg|All files (*.*)|*.*";
            // 
            // saveFileDialogMessage
            // 
            this.saveFileDialogMessage.Filter = "MIME mails (*.eml)|*.eml|Outlook mails (*.msg)|*.msg|All files (*.*)|*.*";
            // 
            // openFileDialogAttachment
            // 
            this.openFileDialogAttachment.Filter = "All files (*.*)|*.*";
            // 
            // saveFileDialogAttachment
            // 
            this.saveFileDialogAttachment.Filter = "All files (*.*)|*.*";
            // 
            // txtFrom
            // 
            this.txtFrom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFrom.Location = new System.Drawing.Point(88, 35);
            this.txtFrom.Name = "txtFrom";
            this.txtFrom.ReadOnly = true;
            this.txtFrom.Size = new System.Drawing.Size(664, 20);
            this.txtFrom.TabIndex = 1;
            this.txtFrom.TextChanged += new System.EventHandler(this.txtFrom_TextChanged);
            // 
            // lblFrom
            // 
            this.lblFrom.Location = new System.Drawing.Point(0, 35);
            this.lblFrom.Name = "lblFrom";
            this.lblFrom.Size = new System.Drawing.Size(80, 16);
            this.lblFrom.TabIndex = 19;
            this.lblFrom.Text = "From:";
            this.lblFrom.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // listBoxAttachments
            // 
            this.listBoxAttachments.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxAttachments.ContextMenuStrip = this.contextMenuAttach;
            this.listBoxAttachments.Location = new System.Drawing.Point(88, 131);
            this.listBoxAttachments.MultiColumn = true;
            this.listBoxAttachments.Name = "listBoxAttachments";
            this.listBoxAttachments.Size = new System.Drawing.Size(664, 56);
            this.listBoxAttachments.TabIndex = 18;
            // 
            // contextMenuAttach
            // 
            this.contextMenuAttach.Items.AddRange(new System.Windows.Forms.ToolStripMenuItem[] {
            this.menuItemDelete});
            this.contextMenuAttach.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            // 
            // menuItemDelete
            // 
            this.menuItemDelete.MergeIndex = 0;
            this.menuItemDelete.Text = "Delete";
            this.menuItemDelete.Click += new System.EventHandler(this.menuItemDelete_Click);
            // 
            // lblAttach
            // 
            this.lblAttach.Location = new System.Drawing.Point(0, 131);
            this.lblAttach.Name = "lblAttach";
            this.lblAttach.Size = new System.Drawing.Size(80, 16);
            this.lblAttach.TabIndex = 17;
            this.lblAttach.Text = "Attachments:";
            this.lblAttach.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtSubject
            // 
            this.txtSubject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSubject.Location = new System.Drawing.Point(88, 107);
            this.txtSubject.Name = "txtSubject";
            this.txtSubject.ReadOnly = true;
            this.txtSubject.Size = new System.Drawing.Size(664, 20);
            this.txtSubject.TabIndex = 16;
            this.txtSubject.TextChanged += new System.EventHandler(this.txtSubject_TextChanged);
            // 
            // lblSubject
            // 
            this.lblSubject.Location = new System.Drawing.Point(0, 107);
            this.lblSubject.Name = "lblSubject";
            this.lblSubject.Size = new System.Drawing.Size(80, 16);
            this.lblSubject.TabIndex = 15;
            this.lblSubject.Text = "Subject:";
            this.lblSubject.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtCc
            // 
            this.txtCc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCc.Location = new System.Drawing.Point(88, 83);
            this.txtCc.Name = "txtCc";
            this.txtCc.ReadOnly = true;
            this.txtCc.Size = new System.Drawing.Size(664, 20);
            this.txtCc.TabIndex = 14;
            this.txtCc.TextChanged += new System.EventHandler(this.txtCc_TextChanged);
            // 
            // txtTo
            // 
            this.txtTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTo.Location = new System.Drawing.Point(88, 59);
            this.txtTo.Name = "txtTo";
            this.txtTo.ReadOnly = true;
            this.txtTo.Size = new System.Drawing.Size(664, 20);
            this.txtTo.TabIndex = 13;
            this.txtTo.TextChanged += new System.EventHandler(this.txtTo_TextChanged);
            // 
            // lblCc
            // 
            this.lblCc.Location = new System.Drawing.Point(0, 83);
            this.lblCc.Name = "lblCc";
            this.lblCc.Size = new System.Drawing.Size(80, 16);
            this.lblCc.TabIndex = 12;
            this.lblCc.Text = "Cc:";
            this.lblCc.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTo
            // 
            this.lblTo.Location = new System.Drawing.Point(0, 59);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(80, 16);
            this.lblTo.TabIndex = 11;
            this.lblTo.Text = "To:";
            this.lblTo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textEditor
            // 
            this.textEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textEditor.BackColor = System.Drawing.SystemColors.Control;
            this.textEditor.Location = new System.Drawing.Point(0, 195);
            this.textEditor.Modified = false;
            this.textEditor.Name = "textEditor";
            this.textEditor.Size = new System.Drawing.Size(760, 222);
            this.textEditor.TabIndex = 21;
            this.textEditor.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textEditor_KeyPress);
            // 
            // MailEditor
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(760, 417);
            this.Controls.Add(this.txtFrom);
            this.Controls.Add(this.txtSubject);
            this.Controls.Add(this.txtCc);
            this.Controls.Add(this.txtTo);
            this.Controls.Add(this.lblFrom);
            this.Controls.Add(this.listBoxAttachments);
            this.Controls.Add(this.lblAttach);
            this.Controls.Add(this.lblSubject);
            this.Controls.Add(this.lblCc);
            this.Controls.Add(this.lblTo);
            this.Controls.Add(this.textEditor);
            this.Controls.Add(this.mainMenu);
            this.MainMenuStrip = this.mainMenu;
            this.Name = "MailEditor";
            this.Text = "Mail Message Editor";
            this.Load += new System.EventHandler(this.MailEditor_Load);
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        /// <summary>
        /// Deletes attachment files.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBoxAttachments_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar != 'd' && e.KeyChar != 'D')
                return;

            if (listBoxAttachments.SelectedItem == null)
                return;

            if (MessageBox.Show("Do you want to remove the attachment?", "Remove Attachment", MessageBoxButtons.YesNoCancel) != DialogResult.Yes)
                return;

            string file = listBoxAttachments.SelectedItem.ToString();
            RemoveAttachment(file);
        }

        /// <summary>
        /// Deletes attachment.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItemDelete_Click(object sender, System.EventArgs e)
        {
            if (listBoxAttachments.SelectedItem == null)
                return;

            string file = listBoxAttachments.SelectedItem.ToString();
            RemoveAttachment(file);
        }

        /// <summary>
        /// Adds new attached file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItemFileAttachment_Click(object sender, System.EventArgs e)
        {
            if (openFileDialogAttachment.ShowDialog() != DialogResult.OK)
                return;

            Attachment att = new Attachment(openFileDialogAttachment.FileName);
            listBoxAttachments.Items.Add(att.FileName);
            _message.Attachments.Add(att);

            _modified = true;
        }

        /// <summary>
        /// Creates new message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItemNewMessage_Click(object sender, System.EventArgs e)
        {
            NewMessage();
        }

        /// <summary>
        /// Opens message from file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItemOpenMessage_Click(object sender, System.EventArgs e)
        {
            OpenMessage();
        }

        /// <summary>
        /// Saves message to file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItemSave_Click(object sender, System.EventArgs e)
        {
            SaveMessage();
        }

        /// <summary>
        /// Saves message to a new file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItemSaveAs_Click(object sender, System.EventArgs e)
        {
            SaveAs();
        }

        /// <summary>
        /// Saves all attachments to folder.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItemSaveAttachments_Click(object sender, System.EventArgs e)
        {
            SaveAttachments();
        }

        /// <summary>
        /// Closes application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItemClose_Click(object sender, System.EventArgs e)
        {
            CloseMessage();
        }

        private void menuItemCut_Click(object sender, System.EventArgs e)
        {
            textEditor.Cut();
        }

        private void menuItemCopy_Click(object sender, System.EventArgs e)
        {
            textEditor.Copy();
        }

        private void menuItemPaste_Click(object sender, System.EventArgs e)
        {
            textEditor.Paste();
        }

        private void menuItemSelectAll_Click(object sender, System.EventArgs e)
        {
            textEditor.SelectAll();
        }

        private void txtFrom_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                _message.From = new MailAddressCollection(txtFrom.Text);
                txtFrom.BackColor = SystemColors.Window;
            }
            catch (MimeException x)
            {
                if (x.Status != MimeExceptionStatus.HeaderParserError)
                    throw;

                _message.From = new MailAddressCollection();
                txtFrom.BackColor = Color.Orange;
            }
            _modified = true;
        }

        private void txtTo_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                _message.To = new MailAddressCollection(txtTo.Text);
                txtTo.BackColor = SystemColors.Window;
            }
            catch (MimeException x)
            {
                if (x.Status != MimeExceptionStatus.HeaderParserError)
                    throw;

                _message.To = new MailAddressCollection();
                txtTo.BackColor = Color.Red;
            }
            _modified = true;
        }

        private void txtCc_TextChanged(object sender, System.EventArgs e)
        {
            try
            {
                _message.CC = new MailAddressCollection(txtCc.Text);
                txtCc.BackColor = SystemColors.Window;
            }
            catch (MimeException x)
            {
                if (x.Status != MimeExceptionStatus.HeaderParserError)
                    throw;
                _message.CC = new MailAddressCollection();
                txtCc.BackColor = Color.Red;
            }
            _modified = true;
        }

        private void txtSubject_TextChanged(object sender, System.EventArgs e)
        {
            _modified = true;
            Text = txtSubject.Text;
        }

        private void textEditor_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            _modified = true;
        }

        private void MailEditor_Load(object sender, EventArgs e)
        {
            _message = new MailMessage();
        }

    }
}
