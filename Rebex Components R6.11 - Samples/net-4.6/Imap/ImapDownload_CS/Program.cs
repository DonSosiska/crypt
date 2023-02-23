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
    /// Sample application for downloading mail messages from IMAP servers.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Program entrypoint.
        /// </summary>
        /// <param name="args">Program arguments.</param>
        /// <returns>Status. (0 = success; anything else = failure)</returns>
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
                Console.WriteLine("Downloads e-mail messages from IMAP server.");
                Console.WriteLine("");
                Console.WriteLine("The program is a sample for Rebex Secure Mail for .NET component.");
                Console.WriteLine("For more info, see http://www.rebex.net/secure-mail.net/");

                Console.WriteLine();
                Console.WriteLine("Syntax: {0} none|explicit|implicit server[:port] -username username -password password [-folder path] [-format MIME|MSG]", applicationName);
                Console.WriteLine("Specific error: " + e.Message);
                return 2;
            }

            Imap client = new Imap();
            try
            {
                // set SSL to accept all server certificates...
                // do not do this in production code, server certificates should
                // be verified - use ValidatingCertificate event instead
                client.Settings.SslAcceptAllCertificates = true;

                // connect to the server
                Console.WriteLine("Connecting {0} to {1}:{2}...", config.Security, config.Server, config.Port);
                client.Connect(config.Server, config.Port, config.Security);

                // Login
                Console.WriteLine("Authorizing as {0}...", config.User);
                client.Login(config.User, config.Password);

                // Select folder
                Console.WriteLine("Selecting folder '{0}'...", config.Folder);
                client.SelectFolder(config.Folder);
                ImapFolder folder = client.CurrentFolder;

                // Show number of messages in the folder
                Console.WriteLine("{0} messages found.", folder.TotalMessageCount);

                // Get message list
                Console.WriteLine("Retrieving message list...");
                ImapMessageCollection list = client.GetMessageList(ImapListFields.UniqueId);

                // Download each message
                for (int i = 0; i < list.Count; i++)
                {
                    ImapMessageInfo message = list[i];

                    // Create filename from email unique ID
                    string filename = FixFilename(message.UniqueId);

                    if (config.Format == Mail.MailFormat.OutlookMsg)
                        filename += ".msg";
                    else
                        filename += ".eml";

                    // Download messages (new only)
                    if (File.Exists(filename))
                    {
                        Console.WriteLine("Skipping message {0}...", message.UniqueId);
                    }
                    else
                    {
                        Console.WriteLine("Retrieving message {0} and saving as {1}...", message.UniqueId, filename);
                        if (config.Format == MailFormat.OutlookMsg)
                        {
                            // get message from IMAP (and parse it)
                            MailMessage messageToSave = client.GetMailMessage(message.UniqueId);

                            // save message in MSG format
                            messageToSave.Save(filename, MailFormat.OutlookMsg);
                        }
                        else
                        {
                            // get message from IMAP directly into file in MIME format (without parsing)
                            client.GetMessage(message.UniqueId, filename);
                        }
                    }
                }

                // Disconnect
                Console.WriteLine("Disconnecting...");
                client.Disconnect();

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
                client.Dispose();
            }
        }

        /// <summary>
        /// Creates a usable filename from string parameter.
        /// </summary>
        /// <param name="originalFilename">String to be converted - e.g. mail subject or unique ID.</param>
        /// <returns>A usable filename based on the supplied string.</returns>
        private static string FixFilename(string originalFilename)
        {
            return BitConverter.ToString(Encoding.ASCII.GetBytes(originalFilename)).Replace("-", "");
        }

    }
}
