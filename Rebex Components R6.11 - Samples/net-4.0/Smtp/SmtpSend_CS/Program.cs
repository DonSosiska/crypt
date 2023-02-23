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
using System.Collections;
using Rebex.Net;
using Rebex.Mail;
using Rebex.Mime.Headers;
using Rebex.Security.Certificates;

namespace Rebex.Samples
{
    /// <summary>
    /// Sample application for sending e-mail via SMTP.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Program entrypoint.
        /// </summary>
        /// <param name="args">Program arguments.</param>
        /// <returns>Status. (0=success, 2=bad arguments)</returns>
        [STAThread]
        static int Main(string[] args)
        {
            // set the trial license key
            Rebex.Licensing.Key = Rebex.Samples.TrialKey.Key;


            Arguments config;

            try
            {
                config = new Arguments(args);
            }
            catch(Exception e)
            {
                string applicationName = AppDomain.CurrentDomain.FriendlyName;
                Console.WriteLine("=====================================================================");
                Console.WriteLine(" {0} ", applicationName);
                Console.WriteLine("=====================================================================");
                Console.WriteLine("");
                Console.WriteLine("Sends e-mail from command line.");
                Console.WriteLine("");
                Console.WriteLine("The program is a sample for Rebex Secure Mail for .NET component.");
                Console.WriteLine("For more info, see http://www.rebex.net/secure-mail.net/");

                Console.WriteLine("");
                Console.WriteLine("Syntax: {0} server[:port]\n" +
                    "        [-security none|explicit|implicit]\n" +
                    "        [-sign yes]\n" +
                    "        [-encrypt yes]\n" +
                    "        [-certificate path1 [-certificate path2 [...]]]\n" +
                    "        -from mail@domain\n" + 
                    "        -to mail@domain;mail2@domain2\n" +
                    "        [-subject \"subject\"]\n" +
                    "        [-body \"text\"]\n" +
                    "        [-file file.eml|file.msg]\n" +
                    "        [-username login]\n" +
                    "        [-password pass]\n",
                    applicationName);
                
                Console.WriteLine("Specific error: " + e.Message);
                
                return 2;
            }

            // build the message
            MailMessage message = config.CreateMessage();

            //
            // Sign and/or encrypt email if needed
            //
            if (config.Sign || config.Encrypt)
            {
                // load the certificates specified on the command line
                CertificateStore specified = config.LoadCertificates();
        
                // if the email should be both signed and encrypted it's recomended
                // to sign it first and then encrypt it. This ensures proper
                // handling in Outlook and OutlookExpress. 

                //
                // Sign email
                // 
                if (config.Sign)
                {
                    ArrayList signers = new ArrayList();
                    CertificateStore my = new CertificateStore(CertificateStoreName.My);
                    
                    // Get all needed certificates located in user's certificate store or specified on the command line
                    foreach (MailAddress address in message.From)
                    {
                        Certificate certificate = FindCertificate(address.Address, specified, my, CertificateFindOptions.HasPrivateKey | CertificateFindOptions.IsTimeValid);
                        
                        if (certificate == null)
                        {
                            Console.WriteLine("Certificate for signer '{0}' was not found.", address.Address);
                            return 3;
                        }
                        // add the certificate to signers collection
                        signers.Add(certificate);
                    }
                    
                    // Sign the message
                    message.Sign((Certificate[])signers.ToArray(typeof(Certificate)));
                }

                //
                // Encrypt email
                // 
                if (config.Encrypt)
                {
                    ArrayList recipients = new ArrayList();
                    CertificateStore addressBook;
                    if (CertificateStore.Exists(CertificateStoreName.AddressBook))
                        addressBook = new CertificateStore(CertificateStoreName.AddressBook);
                    else
                        addressBook = null;

                    // Prepare certificates located in user's certificate store or specified on the command line						
                    foreach (MailAddress address in message.To)
                    {
                        Certificate certificate = FindCertificate(address.Address, specified, addressBook, CertificateFindOptions.IsTimeValid);
                        if (certificate == null)
                        {
                            Console.WriteLine("Certificate for recipient '{0}' was not found.", address.Address);
                            return 3;
                        }
                        recipients.Add(certificate);
                    }


                    // Add sender certificates to recipients array now.
                    // Otherwise they will not be able to read the message they have sent.
                    CertificateStore my = new CertificateStore(CertificateStoreName.My);
                    
                    // Get all needed certificates located in user's certificate store or specified on the command line
                    foreach (MailAddress address in message.From)
                    {
                        Certificate certificate = FindCertificate(address.Address, specified, my, CertificateFindOptions.IsTimeValid);
                        
                        if (certificate != null)
                        {
                            // add the certificate to recipients collection
                            recipients.Add(certificate);
                        }						
                    }

                    // Encrypt the message
                    message.Encrypt((Certificate[])recipients.ToArray(typeof(Certificate)));
                }

            }



            Smtp client = new Smtp();
            try
            {
                // set SSL parameters to accept all server certificates...
                // do not do this in production code, server certificates should
                // be verified - use ValidatingCertificate event instead
                client.Settings.SslAcceptAllCertificates = true;

                // connect to the server
                Console.WriteLine("Connecting {2} to {0}:{1}...", config.Server, config.Port, config.Security.ToString().ToLower());
                client.Connect(config.Server, config.Port, config.Security);

                // login if username and password was submitted
                if (config.User != null && config.Password != null)
                {
                    Console.WriteLine("Logging in as {0}...", config.User);
                    client.Login(config.User, config.Password);
                }

                // send message
                Console.WriteLine("Sending message...");
                client.Send(message);
            
                return 0;
            }
            catch (Exception x)
            {
                // Show exception message
                Console.WriteLine("Error occured: {0}", x.Message);

                // Show list of rejected recipients + rejection reason
                SmtpException sx = x as SmtpException;
                if (sx != null)
                {
                    // Display recipients reject by the server
                    foreach (SmtpRejectedRecipient rr in sx.GetRejectedRecipients())
                    {
                        Console.WriteLine ("Rejected: {0} - {1}", rr.Address, rr.Response.Description);
                    }
                }

                // Show exception details
                Console.WriteLine();
                Console.WriteLine(x);
                return 1;
            }
            finally
            {
                // disconnect
                Console.WriteLine("Disconnecting...");
                client.Disconnect();
                client.Dispose();
            }
        }

        /// <summary>
        /// Finds the certificate in the certificate store
        /// </summary>
        /// <param name="address">The email address.</param>
        /// <param name="specified">Primary certificate store.</param>
        /// <param name="additional">Additional certificate store.</param>
        /// <param name="options">CertificateFindOptions.</param>
        /// <returns></returns>
        static Certificate FindCertificate(string address, CertificateStore specified, CertificateStore additional, CertificateFindOptions options)
        {
            // try to find certificate in the primary CertificateStore
            Certificate[] certificate = specified.FindCertificatesForMailAddress(address, options);
            
            // if not found try to locate it in the secondary CertificateStore
            if (certificate.Length == 0 && additional != null)
                certificate = additional.FindCertificatesForMailAddress(address, options);
                
            if (certificate.Length == 0)
                return null;
                
            return certificate[0];
        }

    }
}