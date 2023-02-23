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
using Rebex.Net;
using Rebex.Mail;

namespace Rebex.Samples
{
    /// <summary>
    /// Sample application for downloading mail messages from POP3 servers.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Program entrypoint.
        /// </summary>
        /// <param name="args">Program arguments.</param>
        /// <returns>Status. (0=success, anything else = failure)</returns>
        [STAThread]
        static int Main(string[] args)
        {
            // set the trial license key
            Rebex.Licensing.Key = Rebex.Samples.TrialKey.Key;


            //
            // Parse command line arguments. Show command line syntax in case of error.
            //
            Arguments config;
            try
            {
                config = new Arguments(args);
            }
            catch (Exception e)
            {
                string applicationName = AppDomain.CurrentDomain.FriendlyName;
                Console.WriteLine("=====================================================================");
                Console.WriteLine(" {0} ", applicationName);
                Console.WriteLine("=====================================================================");
                Console.WriteLine("");
                Console.WriteLine("Downloads e-mail messages from POP3 server.");
                Console.WriteLine("");
                Console.WriteLine("The program is a sample for Rebex Mail for .NET component.");
                Console.WriteLine("For more info, see http://www.rebex.net/mail.net/");
                Console.WriteLine("");

                Console.WriteLine();
                Console.WriteLine("Syntax: {0} none|explicit|implicit server[:port] -username username -password password [-format mime|msg]", applicationName);
                Console.WriteLine("Specific error: " + e.Message);
                return 2;
            }

            Pop3 client = new Pop3();
            try
            {
                // set SSL parameters to accept all server certificates...
                // do not do this in production code, server certificates should
                // be verified - use ValidatingCertificate event instead
                client.Settings.SslAcceptAllCertificates = true;

                // connect to the server
                Console.WriteLine("Connecting {0} to {1}:{2}...", config.Security, config.Server, config.Port);
                client.Connect(config.Server, config.Port, config.Security);

                // Login
                Console.WriteLine("Authorizing as {0}...", config.User);
                client.Login(config.User, config.Password);

                // Get message list
                Console.WriteLine("Retrieving message list...");
                Pop3MessageCollection list = client.GetMessageList();
                Console.WriteLine("{0} messages found.", list.Count);

                // Download each message
                for (int i = 0; i < list.Count; i++)
                {
                    Pop3MessageInfo message = list[i];

                    // Create filename from email unique ID
                    string filename = FixFilename(message.UniqueId);

                    // Append the correct extension according to format
                    if (config.Format == MailFormat.Mime)
                        filename += ".eml";
                    else
                        filename += ".msg";

                    // Download messages (new only)
                    if (File.Exists(filename))
                    {
                        Console.WriteLine("Skipping message {0}...", message.UniqueId);
                    }
                    else
                    {
                        Console.WriteLine("Retrieving message {0}...", message.UniqueId);

                        if (config.Format == MailFormat.Mime)
                        {
                            // Save message from POP3 directly in MIME format (without parsing)
                            client.GetMessage(message.SequenceNumber, filename);
                        }
                        else
                        {
                            // Get message from POP3 (and parse it)													
                            MailMessage mes = client.GetMailMessage(message.SequenceNumber);

                            // Save message in MSG format
                            mes.Save(filename, MailFormat.OutlookMsg);
                        }
                    }
                }

                return 0;
            }
            catch (Exception x)
            {
                Console.WriteLine("Error occured: {0}\n", x.Message);
                Console.WriteLine(x);

                return 1;
            }
            finally
            {
                Console.WriteLine("Disconnecting...");
                client.Disconnect();
                client.Dispose();
            }
        }

        /// <summary>
        /// Creates a valid filename from string parameter.
        /// </summary>
        /// <param name="originalFilename">String to be converted - e.g. mail subject or unique ID.</param>
        /// <returns>A valid filename based on the supplied string.</returns>
        private static string FixFilename(string originalFilename)
        {
            // Characters allowed in the filename
            string allowed = " .-0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

            // Replace invalid charactes with its hex representation
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < originalFilename.Length; i++)
            {
                if (allowed.IndexOf(originalFilename[i]) < 0)
                    sb.AppendFormat("_{0:X2}", (int)originalFilename[i]);
                else
                    sb.Append(originalFilename[i]);
            }
            return sb.ToString();
        }
    }
}
