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
    public class ConsoleVerifier
    {
        public static void ValidatingCertificate(object sender, SslCertificateValidationEventArgs e)
        {
            ConsoleVerifier verifier = new ConsoleVerifier();
            TlsCertificateAcceptance acceptResult = verifier.Verify(e.ServerName, e.CertificateChain);
            if (acceptResult == TlsCertificateAcceptance.Accept)
                e.Accept();
            else
                e.Reject(acceptResult);
        }

        public TlsCertificateAcceptance Verify (string commonName, CertificateChain certificateChain)
        {
            ValidationResult res = certificateChain.Validate (commonName, 0);
                
            if (res.Valid)
                return TlsCertificateAcceptance.Accept;

            ValidationStatus status = res.Status;

            ValidationStatus[] values = (ValidationStatus[])Enum.GetValues (typeof(ValidationStatus));

            for (int i=0; i<values.Length; i++)
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
                        problem = string.Format ("Error {0:x} encountered while validating server's certificate.", res.NativeErrorCode);
                        break;
                    default:
                        problem = values[i].ToString();
                        break;
                }

                Console.WriteLine ("!! " + problem);
            }

            Certificate cert = certificateChain[0];

            Console.WriteLine ("Subject: " + cert.GetSubjectName());
            Console.WriteLine ("Issuer: " + cert.GetIssuerName());
            Console.WriteLine ("Valid from: " + cert.GetEffectiveDate());
            Console.WriteLine ("Valid to: " + cert.GetExpirationDate());

            Console.WriteLine ("Do you want to accept this certificate? [(y)es | (n)o]");
            string response = Console.ReadLine ();
            response = response.Trim().ToLower();
            if (response == "y" || response == "yes")
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

    }
}
