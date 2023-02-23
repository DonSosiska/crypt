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

using Rebex.Net;
using Rebex.Security.Certificates;

namespace Rebex.Samples
{
    /// <summary>
    /// Represents a handler for loading client certificates.
    /// </summary>
    public class RequestHandler : ICertificateRequestHandler
    {
        // certificate cache
        private static Hashtable _chosenCertificates = new Hashtable();

        public CertificateChain Request(TlsSocket socket, DistinguishedName[] issuers)
        {
            // try to locate certificate in cache
            string serverCertificateFingerprint = BitConverter.ToString(
                socket.ServerCertificate.LeafCertificate.GetCertHash());

            if (_chosenCertificates.Contains(serverCertificateFingerprint))
                return _chosenCertificates[serverCertificateFingerprint] as CertificateChain;

            // try to locate certificate in Store
            CertificateStore my = new CertificateStore("MY");
            Certificate[] certs;

            if (issuers.Length > 0)
            {
                certs = my.FindCertificates(
                    issuers,
                    CertificateFindOptions.IsTimeValid |
                    CertificateFindOptions.HasPrivateKey |
                    CertificateFindOptions.ClientAuthentication);
            }
            else
            {
                certs = my.FindCertificates(
                    CertificateFindOptions.IsTimeValid |
                    CertificateFindOptions.HasPrivateKey |
                    CertificateFindOptions.ClientAuthentication);
            }

            if (certs.Length == 0)
                return null;

            // confirm whether to use the certificate
            RequesetHandlerForm rhForm = new RequesetHandlerForm();
            rhForm.LoadData(certs);

            if (rhForm.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return null;

            CertificateChain chain = null;

            if (rhForm.Certificate != null)
                chain = CertificateChain.BuildFrom(rhForm.Certificate);

            // save chosen certificate to cache
            _chosenCertificates.Add(serverCertificateFingerprint, chain);

            return chain;
        }
    }
}
