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
using Rebex.Net;
using Rebex.Mail;

namespace Rebex.Samples
{
    /// <summary>
    /// Transfer progress dialog.
    /// </summary>
    public class Progress : System.Windows.Forms.Form
    {
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ProgressBar pbMain;
        private System.Windows.Forms.Button btnOK;

        private System.ComponentModel.Container components = null;

        public Progress(Smtp smtp)
        {
            InitializeComponent();

            CheckForIllegalCrossThreadCalls = true;
            
            // bind handler
            smtp.TransferProgress += client_TransferProgressProxy;
        }

        public void Unbind(Smtp smtp)
        {
            // unbind handler
            smtp.TransferProgress -= client_TransferProgressProxy;
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

        public void SetFinished()
        {
            pbMain.Value = pbMain.Maximum;
            lblStatus.Text = "Mail message was sent successfully.";
            btnOK.Visible = true;
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pbMain = new System.Windows.Forms.ProgressBar();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // pbMain
            // 
            this.pbMain.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pbMain.Location = new System.Drawing.Point(0, 63);
            this.pbMain.Name = "pbMain";
            this.pbMain.Size = new System.Drawing.Size(298, 25);
            this.pbMain.TabIndex = 0;
            // 
            // lblStatus
            // 
            this.lblStatus.Location = new System.Drawing.Point(8, 8);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(280, 24);
            this.lblStatus.TabIndex = 1;
            this.lblStatus.Text = "Preparing email...";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(104, 32);
            this.btnOK.Name = "btnOK";
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.Visible = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // Progress
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(298, 88);
            this.ControlBox = false;
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.pbMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Progress";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Progress";
            this.ResumeLayout(false);

        }
        #endregion

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
        
        public long MessageLength
        {
            get
            {
                return pbMain.Maximum;
            }
            set
            {
                if (value > int.MaxValue)
                    pbMain.Maximum = int.MaxValue;
                else
                    pbMain.Maximum = (int)value;
            }
        }

        /// <summary>
        /// Because the TransferProgress event (which is handled by this method) will be triggered
        /// while sending a message asynchronously using the Smtp.BeginSend method, it will run in
        /// a background thread. Because we should only access methods (except Invoke) and properties
        /// of any WinForm control from the thread that created and owns the control, we must pass
        /// the event arguments to the <see cref="client_TransferProgress"/> method that runs in the
        /// control's thread. This can be accomplished easily using the Invoke method.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void client_TransferProgressProxy(object sender, SmtpTransferProgressEventArgs e)
        {
            // pass the arguments to a method that runs in the thread that owns this control
            Invoke(new EventHandler<SmtpTransferProgressEventArgs>(client_TransferProgress), new object[] { sender, e });
        }

        /// <summary>
        /// This method is called through the Invoke method by <see cref="client_TransferProgressProxy"/>
        /// when it receives a TransferProgress event. Therefore, it will always run in the main
        /// application thread that owns this control and we can safely access properties of its child
        /// controls.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void client_TransferProgress(object sender, SmtpTransferProgressEventArgs e)
        {
            if (e.State == SmtpTransferState.None)
                return;

            // update the status and progress bar according to event data

            lblStatus.Text = String.Format("{0}... {1} bytes transferred.", e.State, e.BytesTransferred);

            if ((int)e.BytesTransferred > pbMain.Maximum)
                pbMain.Value = pbMain.Maximum;
            else
                pbMain.Value = (int) e.BytesTransferred;	
        }
    }
}
