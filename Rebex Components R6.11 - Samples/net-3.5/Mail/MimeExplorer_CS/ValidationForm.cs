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
using Rebex.Security.Certificates;
using Rebex.Security.Cryptography.Pkcs;
using Rebex.Samples;

namespace Rebex.Samples
{
    /// <summary>
    /// Summary description for ValidationForm.
    /// </summary>
    public class ValidationForm : System.Windows.Forms.Form
    {
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TextBox textSignProblem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox textCertProblem;
        private System.Windows.Forms.Label label6;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public ValidationForm(SignatureValidationResult result)
        {
            InitializeComponent();

            if (result == null)
                return;

            ValidationStatus certStatus = result.CertificateValidationStatus;
            foreach (ValidationStatus status in Enum.GetValues(typeof(ValidationStatus)))
            {
                if ((status & certStatus) != 0)
                    textCertProblem.Text += ValidationHelper.GetCertificateValidationStatusDescription(status) + "\r\n";
            }

            SignatureValidationStatus signStatus = result.Status;
            foreach (SignatureValidationStatus status in Enum.GetValues(typeof(SignatureValidationStatus)))
            {
                if ((status & signStatus) != 0)
                    textSignProblem.Text += ValidationHelper.GetSignatureValidationStatusDescription(status) + "\r\n";
            }
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
            this.btnClose = new System.Windows.Forms.Button();
            this.textSignProblem = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.textCertProblem = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnClose.Location = new System.Drawing.Point(312, 256);
            this.btnClose.Name = "btnClose";
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // textSignProblem
            // 
            this.textSignProblem.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.textSignProblem.Location = new System.Drawing.Point(8, 160);
            this.textSignProblem.Multiline = true;
            this.textSignProblem.Name = "textSignProblem";
            this.textSignProblem.ReadOnly = true;
            this.textSignProblem.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textSignProblem.Size = new System.Drawing.Size(384, 88);
            this.textSignProblem.TabIndex = 14;
            this.textSignProblem.Text = "";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(238)));
            this.label1.Location = new System.Drawing.Point(8, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(384, 24);
            this.label1.TabIndex = 13;
            this.label1.Text = "Signature validation problems:";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.panel2.Location = new System.Drawing.Point(8, 128);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(380, 1);
            this.panel2.TabIndex = 5;
            // 
            // textCertProblem
            // 
            this.textCertProblem.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.textCertProblem.Location = new System.Drawing.Point(8, 32);
            this.textCertProblem.Multiline = true;
            this.textCertProblem.Name = "textCertProblem";
            this.textCertProblem.ReadOnly = true;
            this.textCertProblem.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textCertProblem.Size = new System.Drawing.Size(384, 88);
            this.textCertProblem.TabIndex = 12;
            this.textCertProblem.Text = "";
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(238)));
            this.label6.Location = new System.Drawing.Point(8, 136);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(384, 24);
            this.label6.TabIndex = 11;
            this.label6.Text = "Certificate validation problems:";
            // 
            // ValidationForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(394, 288);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textCertProblem);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textSignProblem);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MinimizeBox = true;
            this.MaximizeBox = false;
            this.ShowInTaskbar = false;
            this.MaximumSize = new System.Drawing.Size(424, 332);
            this.MinimumSize = new System.Drawing.Size(424, 332);
            this.Name = "ValidationForm";
            this.Text = "Signature Is Not Valid";
            this.ResumeLayout(false);

        }
        #endregion

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
    }
}
