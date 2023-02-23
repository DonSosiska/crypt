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

namespace Rebex.Samples
{
    /// <summary>
    /// Summary description for SecureConfig.
    /// </summary>
    public class SecurityConfig : System.Windows.Forms.Form
    {
        public const TlsVersion DefaultTlsVersion = TlsVersion.TLS13 | TlsVersion.TLS12;

        private System.Windows.Forms.ComboBox cbSuite;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.CheckBox cbTLS11;
        private System.Windows.Forms.CheckBox cbTLS12;
        private System.Windows.Forms.CheckBox cbTLS10;
        private System.Windows.Forms.CheckBox cbTLS13;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Button cmdCancel;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
    
        public TlsVersion Protocol
        {
            set
            {
                cbTLS13.Checked = ((value & TlsVersion.TLS13) != 0);
                cbTLS12.Checked = ((value & TlsVersion.TLS12) != 0);
                cbTLS11.Checked = ((value & TlsVersion.TLS11) != 0);
                cbTLS10.Checked = ((value & TlsVersion.TLS10) != 0);
            }
            get 
            {
                TlsVersion version = TlsVersion.None;
                if (cbTLS13.Checked) version |= TlsVersion.TLS13;
                if (cbTLS12.Checked) version |= TlsVersion.TLS12;
                if (cbTLS11.Checked) version |= TlsVersion.TLS11;
                if (cbTLS10.Checked) version |= TlsVersion.TLS10;
                return version;
            }
        }

        public TlsCipherSuite AllowedSuite
        {
            set
            {
                switch (value)		
                {
                    case TlsCipherSuite.Secure:
                        cbSuite.SelectedIndex = 1;
                        break;
                    default:
                        cbSuite.SelectedIndex = 0;
                        break;
                }
            }
            get
            {
                if (cbSuite.SelectedIndex == 0)
                    return TlsCipherSuite.All;
                else
                    return TlsCipherSuite.Secure;
            }
        }

        public SecurityConfig()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            DialogResult = DialogResult.Ignore;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if (components != null) 
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
            this.cbSuite = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.cbTLS11 = new System.Windows.Forms.CheckBox();
            this.cbTLS12 = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.cmdOK = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cbTLS13 = new System.Windows.Forms.CheckBox();
            this.cbTLS10 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // cbSuite
            // 
            this.cbSuite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSuite.Items.AddRange(new object[] {
            "All ciphers",
            "Secure only"});
            this.cbSuite.Location = new System.Drawing.Point(48, 88);
            this.cbSuite.Name = "cbSuite";
            this.cbSuite.Size = new System.Drawing.Size(168, 21);
            this.cbSuite.TabIndex = 6;
            // 
            // label14
            // 
            this.label14.Location = new System.Drawing.Point(8, 72);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(112, 23);
            this.label14.TabIndex = 8;
            this.label14.Text = "Allowed suites:";
            // 
            // cbTLS11
            // 
            this.cbTLS11.Location = new System.Drawing.Point(126, 25);
            this.cbTLS11.Name = "cbTLS11";
            this.cbTLS11.Size = new System.Drawing.Size(72, 16);
            this.cbTLS11.TabIndex = 4;
            this.cbTLS11.Text = "TLS 1.1";
            // 
            // cbTLS12
            // 
            this.cbTLS12.Checked = true;
            this.cbTLS12.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbTLS12.Location = new System.Drawing.Point(48, 41);
            this.cbTLS12.Name = "cbTLS12";
            this.cbTLS12.Size = new System.Drawing.Size(72, 16);
            this.cbTLS12.TabIndex = 3;
            this.cbTLS12.Text = "TLS 1.2";
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point(8, 8);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(112, 23);
            this.label13.TabIndex = 7;
            this.label13.Text = "Allowed protocols:";
            // 
            // cmdOK
            // 
            this.cmdOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.cmdOK.Location = new System.Drawing.Point(168, 120);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(64, 24);
            this.cmdOK.TabIndex = 0;
            this.cmdOK.Text = "OK";
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.cmdCancel.Location = new System.Drawing.Point(96, 120);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(64, 24);
            this.cmdCancel.TabIndex = 1;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // cbTLS13
            // 
            this.cbTLS13.Checked = true;
            this.cbTLS13.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbTLS13.Location = new System.Drawing.Point(48, 25);
            this.cbTLS13.Name = "cbTLS13";
            this.cbTLS13.Size = new System.Drawing.Size(72, 16);
            this.cbTLS13.TabIndex = 2;
            this.cbTLS13.Text = "TLS 1.3";
            // 
            // cbTLS10
            // 
            this.cbTLS10.Location = new System.Drawing.Point(126, 41);
            this.cbTLS10.Name = "cbTLS10";
            this.cbTLS10.Size = new System.Drawing.Size(72, 16);
            this.cbTLS10.TabIndex = 5;
            this.cbTLS10.Text = "TLS 1.0";
            // 
            // SecurityConfig
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(240, 157);
            this.Controls.Add(this.cbTLS13);
            this.Controls.Add(this.cbTLS10);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cbTLS11);
            this.Controls.Add(this.cbTLS12);
            this.Controls.Add(this.cbSuite);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label13);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SecurityConfig";
            this.ShowInTaskbar = false;
            this.Text = "Security Settings";
            this.ResumeLayout(false);

        }
        #endregion

        private void cmdOK_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close ();
        }

        private void cmdCancel_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close ();
        }
    }
}
