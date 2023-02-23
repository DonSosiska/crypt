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
using System.Text;

using Rebex.Net;
using Rebex.Security.Certificates;

namespace Rebex.Samples
{
    /// <summary>
    /// Represents certificate's thumbprint verifier.
    /// </summary>
    public class ThumbprintVerifier
    {
        bool _isAccepted;
        string _thumbprint;

        /// <summary>
        /// Gets the value indicating whether the validated certificate was accepted.
        /// </summary>
        public bool IsAccepted { get { return _isAccepted; } }

        /// <summary>
        /// Gets or sets the thumbprint for certificate validation.
        /// If a certificate with different thumbprint was accepted, 
        /// this value is changed to the accepted certificate's thumbprint.
        /// </summary>
        public string Thumbprint { get { return _thumbprint; } }

        /// <summary>
        /// Initializes new instance of the <see cref="ThumbprintVerifier"/>.
        /// </summary>
        public ThumbprintVerifier(string thumbprint)
        {
            _thumbprint = thumbprint;
        }

        /// <summary>
        /// Validates certificate based on the specified thumbprint.
        /// </summary>
        public void ValidatingCertificate(object sender, SslCertificateValidationEventArgs e)
        {
            TlsCertificateAcceptance acceptResult = TlsCertificateAcceptance.Bad;

            // compare thumbprints
            if (string.Equals(Thumbprint, e.Certificate.Thumbprint, StringComparison.OrdinalIgnoreCase))
            {
                acceptResult = TlsCertificateAcceptance.Accept;
            }
            else
            {
                string problem;
                if (string.IsNullOrEmpty(Thumbprint))
                    problem = "No locally stored thumbprint to compare this server certificate with.";
                else
                    problem = "Locally stored thumbprint differs from server certificate's thumbprint.";

                // check if the certificate is valid
                ValidationResult res = e.CertificateChain.Validate(e.ServerName, 0);
                if (!res.Valid)
                    problem = string.Format("{0}\r\n{1}", problem, Verifier.GetProblemString(res));

                // ask to trust this certificate
                VerifierForm certForm = new VerifierForm(e.Certificate);
                certForm.Problem = problem;
                certForm.ShowDialog();
                if (certForm.Accepted)
                    acceptResult = TlsCertificateAcceptance.Accept;
            }

            if (acceptResult == TlsCertificateAcceptance.Accept)
            {
                _thumbprint = e.Certificate.Thumbprint;
                _isAccepted = true;
                e.Accept();
            }
            else
            {
                _isAccepted = false;
                e.Reject(acceptResult);
            }
        }
    }
}
