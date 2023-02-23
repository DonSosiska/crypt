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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Rebex.Security.Certificates;
using System.Security.Cryptography.X509Certificates;

namespace Rebex.Samples
{
    /// <summary>
    /// Represents a dialog form for loading certificates.
    /// </summary>
    public partial class ImportCertificateDialog : Form
    {
        /// <summary>
        /// Gets or sets client certificate filename used for SSL authentication.<br/>
        /// </summary>
        public string ClientCertificateFilename
        {
            get { return _clientCertificateFilename; }
            set { _clientCertificateFilename = value; }
        }
        private string _clientCertificateFilename = null;

        /// <summary>
        /// Gets or sets client certificate used for SSL authentication.
        /// </summary>
        [Browsable(false)]
        public CertificateChain ClientCertificate
        {
            get { return _clientCertificate; }
            set { _clientCertificate = value; }
        }
        private CertificateChain _clientCertificate = null;

        /// <summary>
        /// Initializes new instance of the <see cref="ImportCertificateDialog"/>.
        /// </summary>
        public ImportCertificateDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles 'Open from File' button click.
        /// </summary>
        private void btnFile_Click(object sender, EventArgs e)
        {
            // ask for certificate file
            OpenFileDialog fd = new OpenFileDialog();
            fd.Title = "Choose certificate file";
            fd.Filter = string.Format("{0} (*.pfx)|*.pfx|{1}|*.*", "Certificate files", "All files");
            if (fd.ShowDialog() != DialogResult.OK)
                return;
            ClientCertificateFilename = fd.FileName;
            LoadClientCertificate();

            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        /// <summary>
        /// Handles 'Open from Store' button click.
        /// </summary>
        private void btnStore_Click(object sender, EventArgs e)
        {

            CertificateStore store = new CertificateStore(CertificateStoreName.My, CertificateStoreLocation.CurrentUser);
            Certificate[] certs = store.FindCertificates(CertificateFindOptions.None);

            X509Certificate2Collection x509Certs = new X509Certificate2Collection();

            foreach (Certificate cert in certs)
            {
                x509Certs.Add(cert);
            }

            // ask for certificate from store
            x509Certs = X509Certificate2UI.SelectFromCollection(x509Certs,
                "Certificate store: MY",
                "Select a certificate from the store",
                X509SelectionFlag.SingleSelection);

            if (x509Certs.Count == 1)
            {
                Certificate certificate = new Certificate(x509Certs[0]);
                ClientCertificate = CertificateChain.BuildFrom(certificate);

                DialogResult = DialogResult.OK;
            }
            else
            {
                DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary>
        /// Loads a certificate from the <paramref name="ClientCertificateFilename"/> into the <see cref="ClientCertificate"/> property.<br/>
        /// If the certificate requires a password, the method prompts for the password.
        /// </summary>
        /// <returns>True - the key certificate been loaded or an user does not want any certificate. False - an error occurred.</returns>
        private bool LoadClientCertificate()
        {
            try
            {
                // try to load the certificate 
                CertificateChain certificate = null;
                // (first attempt will be with empty password - only if it won't succeed, we will prompt for the password
                bool firstAttempt = true;
                string password = "";
                while (certificate == null)
                {
                    try
                    {
                        ClientCertificate = CertificateChain.LoadPfx(ClientCertificateFilename, password, KeySetOptions.Exportable);
                        // success
                        return true;
                    }
                    // handle invalid password
                    catch (CertificateException ex)
                    {
                        if (ex.Message != "PFX password is not valid.")
                        {
                            ConnectionEditorUtils.ShowException("Error occurred when importing certificate.", "Import failed", ex);
                            return false;
                        }
                        PassphraseDialog pd = new PassphraseDialog(firstAttempt ? "The certificate is password protected.\r\nEnter the certificate password:" : "The password is wrong.\r\nEnter again?");
                        pd.Size = new Size(pd.Width, pd.Height + 13);	// add extra line for the 2-lines password prompt
                        if (pd.ShowDialog() != DialogResult.OK)
                            return true;
                        password = pd.Passphrase;
                        firstAttempt = false;
                    }
                    catch (Exception ex)
                    {
                        ConnectionEditorUtils.ShowException("Error occurred when importing certificate.", "Import failed", ex);
                        return false;
                    }
                }
                return false; // never reached
            }
            finally
            {
                // once the certificate has not been successfully loaded, clear the stored filename
                if (ClientCertificate == null)
                    ClientCertificateFilename = null;
            }
        }
    }
}
