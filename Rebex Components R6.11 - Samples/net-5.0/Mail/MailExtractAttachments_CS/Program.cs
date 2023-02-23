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
using System.IO;
using System.Text;
using Rebex.Mail;

namespace Rebex.Samples
{
    class Program
    {
        [STAThread]
        static int Main(string[] args)
        {
            // set the trial license key
            Rebex.Licensing.Key = Rebex.Samples.TrialKey.Key;


            // If no argument is specified, show syntax
            if (args.Length < 1)
            {
                ShowHelp();
                return 1;
            }

            // Load the mail message from disk
            MailMessage mail = new MailMessage();
            mail.Load(args[0]);


            // Decrypt the message if it is encrypted
            if (mail.IsEncrypted)
            {
                if (!mail.CanDecrypt)
                {
                    Console.WriteLine("Message cannot be decrypted. You do not have the private key.");
                    return 2;
                }
                mail.Decrypt();
            }

            // Validate the signature if the message is signed
            if (mail.IsSigned)
            {
                MailSignatureValidity result = mail.ValidateSignature();
                if (result.Valid)
                {
                    Console.WriteLine("The message is signed and the signature is valid.");
                }
                else
                {
                    Console.WriteLine("The message is signed, but the signature is not valid.");
                    ValidationHelper.ShowProblems(result);
                }
            }

            Console.WriteLine("Message contains {0} attachments.", mail.Attachments.Count);

            // If message has no attachments, just exit
            if (mail.Attachments.Count == 0)
                return 0;

            foreach (Attachment attachment in mail.Attachments)
            {
                // Save the file
                Console.WriteLine("Saving '{0}' ({1}).", attachment.FileName, attachment.MediaType);
                attachment.Save(attachment.FileName);
            }

            return 0;
        }
                
        private static void ShowHelp()
        {
            string applicationName = AppDomain.CurrentDomain.FriendlyName;
            Console.WriteLine("=====================================================================");
            Console.WriteLine(" {0} ", applicationName);
            Console.WriteLine("=====================================================================");
            Console.WriteLine("");
            Console.WriteLine("Extracts all attachments from an e-mail message.");
            Console.WriteLine("Supported e-mail formats: .EML (MIME) and .MSG (Microsoft Outlook).");
            Console.WriteLine("");
            Console.WriteLine("The program is a sample for Rebex Secure Mail for .NET component.");
            Console.WriteLine("For more info, see http://www.rebex.net/secure-mail.net/");
            Console.WriteLine("");
            Console.WriteLine("Syntax is: {0} <mailfile.eml|mailfile.msg>", applicationName);
        }

    }
}
