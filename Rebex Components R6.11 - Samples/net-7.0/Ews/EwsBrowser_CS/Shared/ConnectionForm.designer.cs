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


namespace Rebex.Samples
{
     partial class ConnectionForm
    {
        private UcConnectionEditor ucConnectionEditor;

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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnConnect = new System.Windows.Forms.Button();
            this.ucConnectionEditor = new Rebex.Samples.UcConnectionEditor();
            this.SuspendLayout();
            // 
            // btnConnect
            // 
            this.btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConnect.Location = new System.Drawing.Point(557, 350);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 3;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // ucConnectionEditor
            // 
            this.ucConnectionEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ucConnectionEditor.ClientCertificate = null;
            this.ucConnectionEditor.ClientCertificateFilename = null;
            this.ucConnectionEditor.Location = new System.Drawing.Point(12, 12);
            this.ucConnectionEditor.MinimumSize = new System.Drawing.Size(620, 0);
            this.ucConnectionEditor.Name = "ucConnectionEditor";
            this.ucConnectionEditor.Protocol = Rebex.Samples.ProtocolMode.HTTPS;
            this.ucConnectionEditor.ProxyHost = "";
            this.ucConnectionEditor.ProxyPassword = "";
            this.ucConnectionEditor.ProxyPort = 0;
            this.ucConnectionEditor.ProxyType = Rebex.Net.ProxyType.None;
            this.ucConnectionEditor.ProxyUser = "";
            this.ucConnectionEditor.ServerCertificateThumbprint = "";
            this.ucConnectionEditor.ServerCertificateVerifyingMode = Rebex.Samples.CertificateVerifyingMode.LocalyStoredThumbprint;
            this.ucConnectionEditor.ServerHost = "";
            this.ucConnectionEditor.ServerPassword = "";
            this.ucConnectionEditor.ServerPort = 0;
            this.ucConnectionEditor.ServerUser = "";
            this.ucConnectionEditor.Size = new System.Drawing.Size(620, 334);
            this.ucConnectionEditor.StorePassword = false;
            this.ucConnectionEditor.UseSingleSignOn = false;
            this.ucConnectionEditor.TabIndex = 0;
            // 
            // ConnectionForm
            // 
            this.AcceptButton = this.btnConnect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 382);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.ucConnectionEditor);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(660, 39);
            this.Name = "ConnectionForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Connection";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConnectionForm_FormClosing);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ConnectionForm_KeyPress);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnConnect;
    }	
}