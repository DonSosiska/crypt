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

namespace Rebex.Samples
{
    /// <summary>
    /// Summary description for Verifier.
    /// </summary>
    public class VerifierForm : System.Windows.Forms.Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lblProblem;
        private System.Windows.Forms.Label lblSubject;
        private System.Windows.Forms.Label lblIssuer;
        private System.Windows.Forms.Label lblValidFrom;
        private System.Windows.Forms.Label lblValidTo;
        private System.Windows.Forms.Button btnReject;
        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblHostname;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnOkAndTrust;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtThumbprint;

        private bool _accepted = false;
        private bool _addIssuerCertificateAuthothorityToTrustedCaStore = false;

        /// <summary>
        /// Gets or sets a problem to display.
        /// </summary>
        public string Problem
        {
            get { return lblProblem.Text; }
            set { lblProblem.Text = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to display the 'Accept and Add' button.
        /// </summary>
        public bool ShowAddIssuerToTrustedCaStoreButton
        {
            get { return btnOkAndTrust.Visible; }
            set { btnOkAndTrust.Visible = value; }
        }

        /// <summary>
        /// Gets a value indicating whether the certificate was accepted.
        /// </summary>
        public bool Accepted
        {
            get { return _accepted; }
        }

        /// <summary>
        /// Gets a value indicating whether the 'Accept and Add' button was clicked.
        /// </summary>
        public bool AddIssuerCertificateAuthothorityToTrustedCaStore
        {
            get { return _addIssuerCertificateAuthothorityToTrustedCaStore; }
        }

        /// <summary>
        /// Initializes new instance of the <see cref="VerifierForm"/>.
        /// </summary>
        public VerifierForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes new instance of the <see cref="VerifierForm"/>.
        /// </summary>
        public VerifierForm(Certificate cert)
            : this()
        {
            lblHostname.Text = cert.GetCommonName();
            lblSubject.Text = cert.GetSubjectName();
            lblIssuer.Text = cert.GetIssuerName();
            lblValidFrom.Text = cert.GetEffectiveDate().ToString();
            lblValidTo.Text = cert.GetExpirationDate().ToString();
            txtThumbprint.Text = cert.Thumbprint;
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

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnReject = new System.Windows.Forms.Button();
            this.btnAccept = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtThumbprint = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.lblHostname = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblValidTo = new System.Windows.Forms.Label();
            this.lblValidFrom = new System.Windows.Forms.Label();
            this.lblIssuer = new System.Windows.Forms.Label();
            this.lblSubject = new System.Windows.Forms.Label();
            this.lblProblem = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOkAndTrust = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnReject
            // 
            this.btnReject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReject.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnReject.Location = new System.Drawing.Point(254, 334);
            this.btnReject.Name = "btnReject";
            this.btnReject.Size = new System.Drawing.Size(72, 23);
            this.btnReject.TabIndex = 2;
            this.btnReject.Text = "Reject";
            this.btnReject.Click += new System.EventHandler(this.btnReject_Click);
            // 
            // btnAccept
            // 
            this.btnAccept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAccept.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnAccept.Location = new System.Drawing.Point(336, 334);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(72, 23);
            this.btnAccept.TabIndex = 1;
            this.btnAccept.Text = "Accept";
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panel1.Controls.Add(this.txtThumbprint);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.lblHostname);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.lblValidTo);
            this.panel1.Controls.Add(this.lblValidFrom);
            this.panel1.Controls.Add(this.lblIssuer);
            this.panel1.Controls.Add(this.lblSubject);
            this.panel1.Controls.Add(this.lblProblem);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Location = new System.Drawing.Point(8, 8);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(400, 320);
            this.panel1.TabIndex = 3;
            // 
            // txtThumbprint
            // 
            this.txtThumbprint.BackColor = System.Drawing.SystemColors.Window;
            this.txtThumbprint.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtThumbprint.Location = new System.Drawing.Point(80, 201);
            this.txtThumbprint.Name = "txtThumbprint";
            this.txtThumbprint.ReadOnly = true;
            this.txtThumbprint.Size = new System.Drawing.Size(312, 13);
            this.txtThumbprint.TabIndex = 16;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(8, 201);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(72, 23);
            this.label9.TabIndex = 14;
            this.label9.Text = "Thumbprint:";
            // 
            // lblHostname
            // 
            this.lblHostname.Location = new System.Drawing.Point(80, 32);
            this.lblHostname.Name = "lblHostname";
            this.lblHostname.Size = new System.Drawing.Size(312, 16);
            this.lblHostname.TabIndex = 13;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(8, 32);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(72, 23);
            this.label8.TabIndex = 12;
            this.label8.Text = "Hostname:";
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label6.Location = new System.Drawing.Point(8, 8);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(216, 23);
            this.label6.TabIndex = 11;
            this.label6.Text = "Certificate details:";
            // 
            // lblValidTo
            // 
            this.lblValidTo.Location = new System.Drawing.Point(80, 176);
            this.lblValidTo.Name = "lblValidTo";
            this.lblValidTo.Size = new System.Drawing.Size(312, 23);
            this.lblValidTo.TabIndex = 10;
            // 
            // lblValidFrom
            // 
            this.lblValidFrom.Location = new System.Drawing.Point(80, 152);
            this.lblValidFrom.Name = "lblValidFrom";
            this.lblValidFrom.Size = new System.Drawing.Size(312, 23);
            this.lblValidFrom.TabIndex = 9;
            // 
            // lblIssuer
            // 
            this.lblIssuer.Location = new System.Drawing.Point(80, 104);
            this.lblIssuer.Name = "lblIssuer";
            this.lblIssuer.Size = new System.Drawing.Size(312, 40);
            this.lblIssuer.TabIndex = 8;
            // 
            // lblSubject
            // 
            this.lblSubject.Location = new System.Drawing.Point(80, 56);
            this.lblSubject.Name = "lblSubject";
            this.lblSubject.Size = new System.Drawing.Size(312, 40);
            this.lblSubject.TabIndex = 7;
            // 
            // lblProblem
            // 
            this.lblProblem.ForeColor = System.Drawing.Color.Red;
            this.lblProblem.Location = new System.Drawing.Point(8, 238);
            this.lblProblem.Name = "lblProblem";
            this.lblProblem.Size = new System.Drawing.Size(384, 72);
            this.lblProblem.TabIndex = 6;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.panel2.Location = new System.Drawing.Point(8, 227);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(384, 3);
            this.panel2.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(8, 176);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 23);
            this.label5.TabIndex = 4;
            this.label5.Text = "Valid to:";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(8, 152);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 23);
            this.label4.TabIndex = 3;
            this.label4.Text = "Valid from:";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(8, 104);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 23);
            this.label3.TabIndex = 2;
            this.label3.Text = "Issuer:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 23);
            this.label2.TabIndex = 1;
            this.label2.Text = "Subject:";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.Location = new System.Drawing.Point(8, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(256, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "CERTIFICATE INFORMATION:";
            // 
            // btnOkAndTrust
            // 
            this.btnOkAndTrust.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOkAndTrust.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnOkAndTrust.Location = new System.Drawing.Point(16, 334);
            this.btnOkAndTrust.Name = "btnOkAndTrust";
            this.btnOkAndTrust.Size = new System.Drawing.Size(232, 23);
            this.btnOkAndTrust.TabIndex = 5;
            this.btnOkAndTrust.Text = "OK && Always &Trust This Authority";
            this.btnOkAndTrust.Visible = false;
            this.btnOkAndTrust.Click += new System.EventHandler(this.btnOkAndTrust_Click);
            // 
            // VerifierForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(418, 364);
            this.ControlBox = false;
            this.Controls.Add(this.btnOkAndTrust);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnAccept);
            this.Controls.Add(this.btnReject);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "VerifierForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Certificate";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary>
        /// Handles 'Accept' button click event.
        /// </summary>
        private void btnAccept_Click(object sender, System.EventArgs e)
        {
            _accepted = true;
            this.Close();
        }

        /// <summary>
        /// Handles 'Reject' button click event.
        /// </summary>
        private void btnReject_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Handles 'Accept and Add' button click event.
        /// </summary>
        private void btnOkAndTrust_Click(object sender, System.EventArgs e)
        {
            _accepted = true;
            _addIssuerCertificateAuthothorityToTrustedCaStore = true;
            this.Close();
        }
    }
}
