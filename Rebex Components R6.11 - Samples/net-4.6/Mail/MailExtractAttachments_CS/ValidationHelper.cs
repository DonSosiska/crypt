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
using Rebex.Mail;
using Rebex.Security.Certificates;
using Rebex.Security.Cryptography.Pkcs;

namespace Rebex.Samples
{
    /// <summary>
    /// Provides various helper methods useful for signature validation.
    /// </summary>
    public class ValidationHelper
    {
        /// <summary>
        /// Displays the list of problems of the message that was not validated successfully.
        /// </summary>
        /// <param name="result">Validation result.</param>
        public static void ShowProblems(MailSignatureValidity result)
        {
            Console.WriteLine("There were following problems:");

            string[] missing = result.GetMissingSignatures();
            if (missing.Length > 0)
            {
                Console.WriteLine("- The signatures for the following senders are missing:");
                for (int i=0; i<missing.Length; i++)
                    Console.WriteLine("    {0}", missing[i]);
            }

            ValidationStatus certStatus = result.CertificateValidationStatus;
            foreach (ValidationStatus status in Enum.GetValues(typeof(ValidationStatus)))
            {
                if ((status & certStatus) != 0)
                    Console.WriteLine("- {0}", GetCertificateValidationStatusDescription(status));
            }

            SignatureValidationStatus signStatus = result.SignatureValidationStatus;
            foreach (SignatureValidationStatus status in Enum.GetValues(typeof(SignatureValidationStatus)))
            {
                if ((status & signStatus) != 0)
                    Console.WriteLine("- {0}", GetSignatureValidationStatusDescription(status));
            }
        }

        /// <summary>
        /// Returns a descriptions of a single ValidationStatus flag.
        /// </summary>
        /// <param name="status">Certificate validation status</param>
        /// <returns>A description.</returns>
        public static string GetCertificateValidationStatusDescription(ValidationStatus status)
        {
            switch (status)
            {
                case ValidationStatus.TimeNotValid:
                    return "Certificate has expired or is not valid yet.";
                case ValidationStatus.Revoked:
                    return "Certificate has been revoked.";
                case ValidationStatus.RootNotTrusted:
                    return "Certificate was issued by an untrusted authority.";
                case ValidationStatus.IncompleteChain:
                    return "Certificate does not chain up to a trusted root authority.";
                case ValidationStatus.Malformed:
                    return "Certificate is malformed.";
                default:
                    return status.ToString();
            }
        }

        /// <summary>
        /// Returns a descriptions of a single SignatureValidationStatus flag.
        /// </summary>
        /// <param name="status">Signature validation status</param>
        /// <returns>A description.</returns>
        public static string GetSignatureValidationStatusDescription(SignatureValidationStatus status)
        {
            switch (status)
            {
                case SignatureValidationStatus.CertificateNotValid:
                    return "Certificate is not valid.";
                case SignatureValidationStatus.CertificateNotAvailable:
                    return "Certificate is not available.";
                case SignatureValidationStatus.UnsupportedDigestAlgorithm:
                    return "A digest algorithm is not supported.";
                case SignatureValidationStatus.UnsupportedSignatureAlgorithm:
                    return "A signature algorithm is not supported.";
                case SignatureValidationStatus.InvalidSignature:
                    return "A signature is invalid.";
                case SignatureValidationStatus.InvalidKeyUsage:
                    return "Invalid key usage.";
                case SignatureValidationStatus.ContentTypeMismatch:
                    return "Content type mismatch.";
                default:
                    return status.ToString();
            }
        }
    }
}
