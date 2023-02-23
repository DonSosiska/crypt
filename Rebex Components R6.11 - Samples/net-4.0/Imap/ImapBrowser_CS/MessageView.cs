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
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Rebex.Net;
using Rebex.Mail;
using Rebex.Mime;
using Rebex.Mime.Headers;
using System.Drawing;


namespace Rebex.Samples
{
    /// <summary>
    /// Simple message viewer.
    /// </summary>
    public class MessageView : System.Windows.Forms.Form
    {
        private readonly MailMessage _message;
        private readonly byte[] _rawMessage;

        #region Controls
        
        private System.Windows.Forms.TabControl views;
        private System.Windows.Forms.TabPage tabMessage;
        private System.Windows.Forms.TabPage tabAttachments;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TabPage tabHeaders;
        private System.Windows.Forms.TabPage tabRawMessage;
        private System.Windows.Forms.ColumnHeader headerValue;
        private System.Windows.Forms.ColumnHeader headerName;
        private System.Windows.Forms.ColumnHeader attachmentName;
        private System.Windows.Forms.ColumnHeader attachmentMimeType;
        private System.Windows.Forms.ColumnHeader attachmentSize;
        private System.Windows.Forms.ColumnHeader attachmentId;
        private System.Windows.Forms.TextBox txtSubject;
        private System.Windows.Forms.TextBox txtFrom;
        private System.Windows.Forms.ListView attachmentsListView;
        private System.Windows.Forms.ListView headersListView;
        private System.Windows.Forms.TextBox txtCc;
        private System.Windows.Forms.TextBox txtTo;
        private System.Windows.Forms.RichTextBox txtRawText;
        private TabControl tabBody;
        private TabPage tabBodyText;
        private TextBox txtTextBody;
        private TabPage tabBodyHtml;
        private TextBox txtHtmlBody;
    
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageView"/> class.
        /// </summary>
        /// <param name="message">The parsed MimeMessage.</param>
        /// <param name="raw">The raw unparsed message data obtained from the server.</param>
        public MessageView(MailMessage message, byte[] raw)
        {
            InitializeComponent();

            CheckForIllegalCrossThreadCalls = true;

            _message = message;
            _rawMessage = raw;

            SuspendLayout();
            
            //
            // Initialize tabs
            //
            
            // Basic mail message info - from, to, txtSubject, body, ...
            FillMailMessageTab();
        
            // Attachments
            FillAttachmentsTab();
        
            // Message headers
            FillMessageHeaders();
                                                        
            // Raw (unparsed) message data
            FillRawTab();								
                
            ResumeLayout(false);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MessageView));
            this.views = new System.Windows.Forms.TabControl();
            this.tabMessage = new System.Windows.Forms.TabPage();
            this.tabBody = new System.Windows.Forms.TabControl();
            this.tabBodyText = new System.Windows.Forms.TabPage();
            this.txtTextBody = new System.Windows.Forms.TextBox();
            this.tabBodyHtml = new System.Windows.Forms.TabPage();
            this.txtHtmlBody = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSubject = new System.Windows.Forms.TextBox();
            this.txtCc = new System.Windows.Forms.TextBox();
            this.txtTo = new System.Windows.Forms.TextBox();
            this.txtFrom = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tabAttachments = new System.Windows.Forms.TabPage();
            this.attachmentsListView = new System.Windows.Forms.ListView();
            this.attachmentId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.attachmentMimeType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.attachmentName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.attachmentSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabHeaders = new System.Windows.Forms.TabPage();
            this.headersListView = new System.Windows.Forms.ListView();
            this.headerName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.headerValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabRawMessage = new System.Windows.Forms.TabPage();
            this.txtRawText = new System.Windows.Forms.RichTextBox();
            this.views.SuspendLayout();
            this.tabMessage.SuspendLayout();
            this.tabBody.SuspendLayout();
            this.tabBodyText.SuspendLayout();
            this.tabBodyHtml.SuspendLayout();
            this.tabAttachments.SuspendLayout();
            this.tabHeaders.SuspendLayout();
            this.tabRawMessage.SuspendLayout();
            this.SuspendLayout();
            // 
            // views
            // 
            this.views.Controls.Add(this.tabMessage);
            this.views.Controls.Add(this.tabAttachments);
            this.views.Controls.Add(this.tabHeaders);
            this.views.Controls.Add(this.tabRawMessage);
            this.views.Dock = System.Windows.Forms.DockStyle.Fill;
            this.views.Location = new System.Drawing.Point(0, 0);
            this.views.Name = "views";
            this.views.SelectedIndex = 0;
            this.views.Size = new System.Drawing.Size(692, 470);
            this.views.TabIndex = 0;
            // 
            // tabMessage
            // 
            this.tabMessage.Controls.Add(this.tabBody);
            this.tabMessage.Controls.Add(this.label1);
            this.tabMessage.Controls.Add(this.txtSubject);
            this.tabMessage.Controls.Add(this.txtCc);
            this.tabMessage.Controls.Add(this.txtTo);
            this.tabMessage.Controls.Add(this.txtFrom);
            this.tabMessage.Controls.Add(this.label2);
            this.tabMessage.Controls.Add(this.label3);
            this.tabMessage.Controls.Add(this.label4);
            this.tabMessage.Location = new System.Drawing.Point(4, 22);
            this.tabMessage.Name = "tabMessage";
            this.tabMessage.Size = new System.Drawing.Size(684, 444);
            this.tabMessage.TabIndex = 3;
            this.tabMessage.Text = "Mail Message";
            // 
            // tabBody
            // 
            this.tabBody.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabBody.Controls.Add(this.tabBodyText);
            this.tabBody.Controls.Add(this.tabBodyHtml);
            this.tabBody.Location = new System.Drawing.Point(3, 121);
            this.tabBody.Name = "tabBody";
            this.tabBody.SelectedIndex = 0;
            this.tabBody.Size = new System.Drawing.Size(678, 320);
            this.tabBody.TabIndex = 10;
            // 
            // tabBodyText
            // 
            this.tabBodyText.Controls.Add(this.txtTextBody);
            this.tabBodyText.Location = new System.Drawing.Point(4, 22);
            this.tabBodyText.Name = "tabBodyText";
            this.tabBodyText.Padding = new System.Windows.Forms.Padding(3);
            this.tabBodyText.Size = new System.Drawing.Size(670, 294);
            this.tabBodyText.TabIndex = 0;
            this.tabBodyText.Text = "Text";
            this.tabBodyText.UseVisualStyleBackColor = true;
            // 
            // txtTextBody
            // 
            this.txtTextBody.BackColor = System.Drawing.SystemColors.Window;
            this.txtTextBody.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtTextBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTextBody.Location = new System.Drawing.Point(3, 3);
            this.txtTextBody.Multiline = true;
            this.txtTextBody.Name = "txtTextBody";
            this.txtTextBody.ReadOnly = true;
            this.txtTextBody.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtTextBody.Size = new System.Drawing.Size(664, 288);
            this.txtTextBody.TabIndex = 7;
            // 
            // tabBodyHtml
            // 
            this.tabBodyHtml.Controls.Add(this.txtHtmlBody);
            this.tabBodyHtml.Location = new System.Drawing.Point(4, 22);
            this.tabBodyHtml.Name = "tabBodyHtml";
            this.tabBodyHtml.Padding = new System.Windows.Forms.Padding(3);
            this.tabBodyHtml.Size = new System.Drawing.Size(670, 294);
            this.tabBodyHtml.TabIndex = 1;
            this.tabBodyHtml.Text = "HTML";
            this.tabBodyHtml.UseVisualStyleBackColor = true;
            // 
            // txtHtmlBody
            // 
            this.txtHtmlBody.BackColor = System.Drawing.SystemColors.Window;
            this.txtHtmlBody.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtHtmlBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtHtmlBody.Location = new System.Drawing.Point(3, 3);
            this.txtHtmlBody.Multiline = true;
            this.txtHtmlBody.Name = "txtHtmlBody";
            this.txtHtmlBody.ReadOnly = true;
            this.txtHtmlBody.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtHtmlBody.Size = new System.Drawing.Size(664, 288);
            this.txtHtmlBody.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 23);
            this.label1.TabIndex = 5;
            this.label1.Text = "From:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtSubject
            // 
            this.txtSubject.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSubject.BackColor = System.Drawing.SystemColors.Window;
            this.txtSubject.Location = new System.Drawing.Point(112, 95);
            this.txtSubject.Name = "txtSubject";
            this.txtSubject.ReadOnly = true;
            this.txtSubject.Size = new System.Drawing.Size(568, 20);
            this.txtSubject.TabIndex = 3;
            // 
            // txtCc
            // 
            this.txtCc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCc.BackColor = System.Drawing.SystemColors.Window;
            this.txtCc.Location = new System.Drawing.Point(112, 66);
            this.txtCc.Name = "txtCc";
            this.txtCc.Size = new System.Drawing.Size(568, 20);
            this.txtCc.TabIndex = 7;
            // 
            // txtTo
            // 
            this.txtTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTo.BackColor = System.Drawing.SystemColors.Window;
            this.txtTo.Location = new System.Drawing.Point(112, 37);
            this.txtTo.Name = "txtTo";
            this.txtTo.Size = new System.Drawing.Size(568, 20);
            this.txtTo.TabIndex = 8;
            // 
            // txtFrom
            // 
            this.txtFrom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFrom.BackColor = System.Drawing.SystemColors.Window;
            this.txtFrom.Location = new System.Drawing.Point(112, 8);
            this.txtFrom.Name = "txtFrom";
            this.txtFrom.ReadOnly = true;
            this.txtFrom.Size = new System.Drawing.Size(568, 20);
            this.txtFrom.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 23);
            this.label2.TabIndex = 5;
            this.label2.Text = "To:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(8, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 23);
            this.label3.TabIndex = 5;
            this.label3.Text = "Cc:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(8, 95);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 23);
            this.label4.TabIndex = 5;
            this.label4.Text = "Subject:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tabAttachments
            // 
            this.tabAttachments.Controls.Add(this.attachmentsListView);
            this.tabAttachments.Location = new System.Drawing.Point(4, 22);
            this.tabAttachments.Name = "tabAttachments";
            this.tabAttachments.Size = new System.Drawing.Size(684, 444);
            this.tabAttachments.TabIndex = 4;
            this.tabAttachments.Text = "Attachments";
            // 
            // attachmentsListView
            // 
            this.attachmentsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.attachmentId,
            this.attachmentMimeType,
            this.attachmentName,
            this.attachmentSize});
            this.attachmentsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.attachmentsListView.FullRowSelect = true;
            this.attachmentsListView.GridLines = true;
            this.attachmentsListView.Location = new System.Drawing.Point(0, 0);
            this.attachmentsListView.Name = "attachmentsListView";
            this.attachmentsListView.Size = new System.Drawing.Size(684, 444);
            this.attachmentsListView.TabIndex = 0;
            this.attachmentsListView.UseCompatibleStateImageBehavior = false;
            this.attachmentsListView.View = System.Windows.Forms.View.Details;
            // 
            // attachmentId
            // 
            this.attachmentId.Text = "Id";
            this.attachmentId.Width = 41;
            // 
            // attachmentMimeType
            // 
            this.attachmentMimeType.Text = "MIME type";
            this.attachmentMimeType.Width = 100;
            // 
            // attachmentName
            // 
            this.attachmentName.Text = "Name";
            this.attachmentName.Width = 450;
            // 
            // attachmentSize
            // 
            this.attachmentSize.Text = "Size (bytes)";
            this.attachmentSize.Width = 80;
            // 
            // tabHeaders
            // 
            this.tabHeaders.Controls.Add(this.headersListView);
            this.tabHeaders.Location = new System.Drawing.Point(4, 22);
            this.tabHeaders.Name = "tabHeaders";
            this.tabHeaders.Size = new System.Drawing.Size(684, 444);
            this.tabHeaders.TabIndex = 0;
            this.tabHeaders.Text = "Headers";
            // 
            // headersListView
            // 
            this.headersListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.headerName,
            this.headerValue});
            this.headersListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headersListView.FullRowSelect = true;
            this.headersListView.GridLines = true;
            this.headersListView.Location = new System.Drawing.Point(0, 0);
            this.headersListView.Name = "headersListView";
            this.headersListView.Size = new System.Drawing.Size(684, 444);
            this.headersListView.TabIndex = 0;
            this.headersListView.UseCompatibleStateImageBehavior = false;
            this.headersListView.View = System.Windows.Forms.View.Details;
            // 
            // headerName
            // 
            this.headerName.Text = "Name";
            this.headerName.Width = 200;
            // 
            // headerValue
            // 
            this.headerValue.Text = "Value";
            this.headerValue.Width = 400;
            // 
            // tabRawMessage
            // 
            this.tabRawMessage.Controls.Add(this.txtRawText);
            this.tabRawMessage.Location = new System.Drawing.Point(4, 22);
            this.tabRawMessage.Name = "tabRawMessage";
            this.tabRawMessage.Size = new System.Drawing.Size(684, 444);
            this.tabRawMessage.TabIndex = 2;
            this.tabRawMessage.Text = "Raw Message";
            // 
            // txtRawText
            // 
            this.txtRawText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRawText.BackColor = System.Drawing.SystemColors.Window;
            this.txtRawText.Location = new System.Drawing.Point(0, 0);
            this.txtRawText.Name = "txtRawText";
            this.txtRawText.ReadOnly = true;
            this.txtRawText.Size = new System.Drawing.Size(680, 440);
            this.txtRawText.TabIndex = 0;
            this.txtRawText.Text = "";
            // 
            // MessageView
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(692, 470);
            this.Controls.Add(this.views);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MessageView";
            this.Text = "MessageView";
            this.views.ResumeLayout(false);
            this.tabMessage.ResumeLayout(false);
            this.tabMessage.PerformLayout();
            this.tabBody.ResumeLayout(false);
            this.tabBodyText.ResumeLayout(false);
            this.tabBodyText.PerformLayout();
            this.tabBodyHtml.ResumeLayout(false);
            this.tabBodyHtml.PerformLayout();
            this.tabAttachments.ResumeLayout(false);
            this.tabHeaders.ResumeLayout(false);
            this.tabRawMessage.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary>
        /// Loads content of the main mail message tab
        /// </summary>
        public void FillMailMessageTab()
        {
            // from, to, cc
            this.txtFrom.Text = _message.From.ToString();
            this.txtTo.Text = _message.To.ToString();
            this.txtCc.Text = _message.CC.ToString();
            
            // subject
            this.txtSubject.Text = _message.Subject; 
            
            // body
            this.txtTextBody.Text = _message.BodyText.Replace("\n","\r\n");
            this.txtHtmlBody.Text = _message.BodyHtml.Replace("\n","\r\n");
            if (string.IsNullOrEmpty(_message.BodyText))
                this.tabBody.SelectTab(this.tabBodyHtml);
            else
                this.tabBody.SelectTab(this.tabBodyText);

            // window title
            this.Text = _message.Subject;
        }
        
        /// <summary>
        /// Loads content of the message headers tab.
        /// </summary>
        public void FillMessageHeaders()
        {
            MimeHeaderCollection headers = _message.Headers;

            // show all message header
            for (int i=0; i < headers.Count; i++)
            {
                MimeHeader header = headers[i];
                
                // add name column
                ListViewItem item = new ListViewItem(header.Name);
                
                // add header raw content column
                item.SubItems.Add(header.Raw);
                
                // show unparsed (corrupted) headers in red				
                if (header.Unparsable)
                    item.ForeColor = Color.Red;

                // add row to the ListView				
                headersListView.Items.Add(item);
            }
        }
        
        /// <summary>
        /// Loads the content of the attachments tab.
        /// </summary>
        public void FillAttachmentsTab()
        {
            // iterate through all message attachments
            for (int i = 0; i < _message.Attachments.Count; i++)
            {
                Attachment attachment = _message.Attachments[i];
                
                ListViewItem item = new ListViewItem(i.ToString()); // attachment index
                item.SubItems.Add(attachment.MediaType); // type
                item.SubItems.Add(attachment.FileName); // filename
                item.SubItems.Add(attachment.GetContentLength().ToString()); // length in bytes 
                attachmentsListView.Items.Add(item);
            }	
        }

        /// <summary>
        /// Loads the content of the raw message tab.
        /// </summary>
        public void FillRawTab()
        {		
            if (_rawMessage != null)
            {
                int len = Math.Min(_rawMessage.Length, 500000);
                string text = Encoding.Default.GetString(_rawMessage, 0, len);
                txtRawText.Text = text;
                if (len < _rawMessage.Length)
                    txtRawText.Text += "\n ... TRIMMED BY SAMPLE TO 500.000 BYTES ...";
            }
            else
            {
                txtRawText.Text = "";
            }			
        }	
    }
}
