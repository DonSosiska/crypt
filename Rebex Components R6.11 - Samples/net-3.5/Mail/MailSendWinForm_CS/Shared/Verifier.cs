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
    /// Represents certificate verifier.
    /// </summary>
    public class Verifier
    {
        /// <summary>
        /// Validates given certificate.
        /// </summary>
        public static void ValidatingCertificate(object sender, SslCertificateValidationEventArgs e)
        {
            Verifier verifier = new Verifier();
            TlsCertificateAcceptance acceptResult = verifier.Verify(e.ServerName, e.CertificateChain);
            if (acceptResult == TlsCertificateAcceptance.Accept)
                e.Accept();
            else
                e.Reject(acceptResult);
        }

        /// <summary>
        /// Validates given certificate chain.
        /// </summary>
        public TlsCertificateAcceptance Verify(string commonName, CertificateChain certificateChain)
        {
            // validate certificate chain
            ValidationResult res = certificateChain.Validate(commonName, 0);
            if (res.Valid)
                return TlsCertificateAcceptance.Accept;

            // determine whether to display button for adding root certificate
            bool showAddToStore = false;
            if ((res.Status & ValidationStatus.RootNotTrusted) != 0 && certificateChain.RootCertificate != null)
                showAddToStore = true;

            VerifierForm certForm = new VerifierForm(certificateChain.LeafCertificate);
            certForm.Problem = GetProblemString(res);
            certForm.ShowAddIssuerToTrustedCaStoreButton = showAddToStore;
            certForm.ShowDialog();

            // add certificate of the issuer CA to trusted authorities store
            if (certForm.AddIssuerCertificateAuthothorityToTrustedCaStore)
            {
                CertificateStore trustedCaStore = new CertificateStore(CertificateStoreName.Root);
                trustedCaStore.Add(certificateChain.RootCertificate);
            }

            if (certForm.Accepted)
                return TlsCertificateAcceptance.Accept;

            if ((res.Status & ValidationStatus.TimeNotValid) != 0)
                return TlsCertificateAcceptance.Expired;
            if ((res.Status & ValidationStatus.Revoked) != 0)
                return TlsCertificateAcceptance.Revoked;
            if ((res.Status & (ValidationStatus.RootNotTrusted | ValidationStatus.IncompleteChain)) != 0)
                return TlsCertificateAcceptance.UnknownAuthority;
            if ((res.Status & (ValidationStatus.Malformed | ValidationStatus.UnknownError)) != 0)
                return TlsCertificateAcceptance.Other;

            return TlsCertificateAcceptance.Bad;
        }

        /// <summary>
        /// Gets the string representation of the <see cref="ValidationStatus"/>.
        /// </summary>
        public static string GetProblemString(ValidationResult res)
        {
            ValidationStatus status = res.Status;
            StringBuilder sb = new StringBuilder();
            ValidationStatus[] values = (ValidationStatus[])Enum.GetValues(typeof(ValidationStatus));
            for (int i = 0; i < values.Length; i++)
            {
                if ((status & values[i]) == 0)
                    continue;

                status ^= values[i];
                string problem;

                switch (values[i])
                {
                    case ValidationStatus.TimeNotValid:
                        problem = "Server certificate has expired or is not valid yet.";
                        break;
                    case ValidationStatus.Revoked:
                        problem = "Server certificate has been revoked.";
                        break;
                    case ValidationStatus.RootNotTrusted:
                        problem = "Server certificate was issued by an untrusted authority.";
                        break;
                    case ValidationStatus.IncompleteChain:
                        problem = "Server certificate does not chain up to a trusted root authority.";
                        break;
                    case ValidationStatus.Malformed:
                        problem = "Server certificate is malformed.";
                        break;
                    case ValidationStatus.CnNotMatch:
                        problem = "Server hostname does not match the certificate.";
                        break;
                    case ValidationStatus.UnknownError:
                        problem = string.Format("Error {0:x} encountered while validating server's certificate.", res.NativeErrorCode);
                        break;
                    default:
                        problem = values[i].ToString();
                        break;
                }

                sb.AppendFormat("{0}\r\n", problem);
            }
            return sb.ToString();
        }
    }
}
