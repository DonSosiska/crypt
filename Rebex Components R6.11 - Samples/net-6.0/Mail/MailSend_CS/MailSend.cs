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
using Rebex.Mime.Headers;
using Rebex.Mail;

namespace Rebex.Samples.MailSend
{
    /// <summary>
    /// Send method.
    /// </summary>
    enum SendMethod
    {
        /// <summary>
        /// Send mail directly to recipient's server.
        /// </summary>
        Direct,
        /// <summary>
        /// Send mail over IIS.
        /// </summary>
        IIS,
        /// <summary>
        /// Send mail over SMTP class.
        /// </summary>
        Smtp
    };

    class MailSend
    {
        static SendMethod _method = SendMethod.Direct;
        static string _server = null;
        static int _port = Smtp.DefaultPort;
        static bool _portSet = false;
        static string _from = null;
        static string _to = null;
        static string _subject = null;
        static string _body = null;
        static ArrayList _attachments = new ArrayList();
        static SslMode _security = SslMode.None;

        /// <summary>
        /// Program entrypoint.
        /// </summary>
        /// <param name="args">Program arguments.</param>
        /// <returns>Status: 0=OK; 1=Not sent; 2=Bad arguments</returns>
        [STAThread]
        static int Main(string[] args)
        {
            // set the trial license key
            Rebex.Licensing.Key = Rebex.Samples.TrialKey.Key;


            if (args.Length > 0)
            {
                if (args[0] != "-interactive")
                {
                    RunCommandLine(args);
                }
                else
                {
                    RunInteractiveLoop();
                }
            }
            else
            {
                ShowHelp();
            }

            return 0;
        }


        /// <summary>
        /// Parse startup command line.
        /// </summary>
        /// <param name="args">Arguments.</param>
        private static void RunCommandLine(string[] args)
        {
            try
            {
                //
                // Process arguments
                //
                for (int i=0; i<args.Length; i++)
                {
                    string arg = args[i];

                    if (i >= args.Length - 1)
                        throw new ApplicationException(string.Format("Missing value for argument '{0}'.", arg));

                    i++;
                    switch (arg.Substring(1).ToLower())
                    {
                        case "method":
                            ParseMethod(args[i]);
                            break;

                        case "security":
                            ParseSecurity(args[i]);
                            break;
                    
                        case "server":
                            ParseServer(args[i]);
                            break;
                        
                        case "from":
                            _from = args[i];
                            break;
                        
                        case "to":
                            _to = args[i];
                            break;
                            
                        case "subject":
                            _subject = args[i];
                            break;
                            
                        case "body":
                            _body = args[i];
                            break;
                            
                        case "attach":
                            AddAttachedFile(args[i]);
                            break;
                        default:
                            throw new ApplicationException(string.Format("Unknown argument '{0}'.", arg));
                    }
                }

                //
                // Send the message
                // 
                Send();

            }
            catch (ApplicationException ex)
            {
                Console.WriteLine("\nError: {0}", ex.Message);
                Console.WriteLine();
                ShowHelp();
                return;
            }
        }


        /// <summary>
        /// Displays command menu for interactive mode and enters a loop.
        /// </summary>
        private static void RunInteractiveLoop()
        {
            string ret = "";
            string retValue = "";
            while (ret != "10")
            {
                try
                {
                    Console.WriteLine("MailSend\n" +
                        "0.\tSelect security (none|explicit|implicit)\n"+
                        "1.\tSelect send method (iis|smtp|direct)\n"+
                        "2.\tEnter server and port (server[:port])\n"+
                        "3.\tEnter sender address (mail@domain)\n" + 
                        "4.\tEnter recipient address (mail@domain;mail2@domain2)\n" +
                        "5.\tEnter message subject (subject)\n" +
                        "6.\tEnter message body (text)\n"+
                        "7.\tAdd an attachment\n"+
                        "8.\tSend message\n"+
                        "9.\tRemove all attachments\n"+
                        "10.\tExit\n"+
                        "************************************************");

                    Console.WriteLine("Select action(1 - 10):");
                    ret = Console.ReadLine();

                    switch (ret)
                    {
                        case "0":
                            Console.WriteLine("Current security: {0}", _security);
                            Console.WriteLine("Choose a send method (none,explicit,implicit):");
                            retValue = Console.ReadLine();
                            ParseSecurity(retValue);
                            break;

                        case "1":
                            Console.WriteLine("Current send method: {0}", _method);
                            Console.WriteLine("Choose a send method (direct,iis,smtp):");
                            retValue = Console.ReadLine();
                            ParseMethod(retValue);
                            break;
    
                        case "2":
                            if (_server != null)
                            {
                                Console.WriteLine("Current server: {0}:{1}", _server, _port);
                            }
                            Console.WriteLine("Enter a new server and port (eg.: server[:port]):");
                            retValue = Console.ReadLine();
                            ParseServer(retValue);
                            break;
                            
                        case "3":
                            if (_from != null)
                            {
                                Console.WriteLine("Current sender address: {0}", _from);
                            }
                            Console.WriteLine("Enter a new sender address: ");
                            retValue = Console.ReadLine();
                            _from = retValue ;
                            break;
                            
                        case "4":
                            if (_to != null)
                            {
                                Console.WriteLine("Current recipient's address: {0}", _to);
                            }
                            Console.WriteLine("Enter a new recipient's address:");
                            retValue = Console.ReadLine();
                            _to = retValue ;
                            break;
                            
                        case "5"://"subject":
                            if (_subject != null)
                            {
                                Console.WriteLine("Currrent message subject: {0}", _subject);
                            }
                            Console.WriteLine("Enter a new message subject:");
                            retValue = Console.ReadLine();
                            _subject = retValue ;
                            break;
                            
                        case "6"://"body":
                            if (_body != null)
                            {
                                Console.WriteLine("Current message body: {0}", _body);
                            }
                            Console.WriteLine("Enter a new message body:");
                            retValue = Console.ReadLine();
                            _body = retValue ;
                            break;
                            
                        case "7"://"attachment":
                            Console.WriteLine("Current attachments:");
                            foreach (string attFile in _attachments)
                            {
                                Console.WriteLine(attFile);
                            }
                            Console.WriteLine("******************");
                            Console.WriteLine("Enter new attachment path:");
                            retValue = Console.ReadLine();
                            AddAttachedFile(retValue);
                            break;
                            
                        case "8"://send
                            Send();
                            break;
                            
                        case "9"://remove attached files
                            _attachments.Clear();
                            break;
                    }
                }
                catch (ApplicationException ex)
                {
                    Console.WriteLine("Error: {0}", ex.Message);
                    Console.WriteLine();
                }
            }
        }

        #region Helper methods

        /// <summary>
        /// Displays syntax.
        /// </summary>
        private static void ShowHelp()
        {
            string applicationName = AppDomain.CurrentDomain.FriendlyName;
            Console.WriteLine ("=====================================================================");
            Console.WriteLine (" {0} ", applicationName);
            Console.WriteLine ("=====================================================================");		
            Console.WriteLine ("");		
            Console.WriteLine ("Sends e-mail from command line.");
            Console.WriteLine ("");		
            Console.WriteLine("The program is a sample for Rebex Secure Mail for .NET component.");
            Console.WriteLine("For more info, see http://www.rebex.net/secure-mail.net/");
            Console.WriteLine ("");

            Console.WriteLine("Syntax: {0} -method (iis|smtp|direct)\n" +
                "\t-server server[:port]\n"+
                "\t-from mail@domain\n" + 
                "\t-to mail@domain[;mail2@domain2[...]\n" +
                "\t-subject \"subject\"\n" +
                "\t-body \"text\"\n" +
                "\t[-attach file1.ext [-attach file2.ext [...]]\n" + 
                "\t[-security (none|explicit|implicit)]\n"+
                "\nOr, for interactive mode: MailSend -interactive",
                applicationName);
        }

        /// <summary>
        /// Adds attachment to the attachment list.
        /// </summary>
        /// <param name="file">Full path of a file.</param>
        private static void AddAttachedFile(string file)
        {
            if (System.IO.File.Exists(file) == true)
            {
                _attachments.Add(file);				
            }
            else
            {
                Console.WriteLine("File '{0}' not found.", file);
            }
        }

        /// <summary>
        /// Parses the send method.
        /// </summary>
        /// <param name="arg">One of these values: iis, direct, smtp.</param>
        private static void ParseMethod(string arg)
        {
            try
            {
                _method = (SendMethod)Enum.Parse(typeof(SendMethod), arg, true);
            }
            catch (ArgumentException)
            {
                throw new ApplicationException("Unknown method '" + arg + "'.");
            }
        }

        /// <summary>
        /// Parses the security.
        /// </summary>
        /// <param name="arg">One of these values: unsecure, secure, implicit.</param>
        private static void ParseSecurity(string arg)
        {
            try
            {
                _security = (SslMode)Enum.Parse(typeof(SslMode), arg, true);
                if (!_portSet)
                {
                    if (_security == SslMode.Implicit)
                    {
                        _port = Smtp.DefaultImplicitSslPort;
                    }
                    else if (_security == SslMode.Explicit)
                    {
                        _port = Smtp.AlternativeExplicitSslPort;
                    }
                }
            }
            catch (ArgumentException)
            {
                throw new ApplicationException("Unknown security '" + arg + "'.");
            }
        }

        /// <summary>
        /// Parses the hostname and port.
        /// </summary>
        /// <param name="args">host[:port].</param>
        private static void ParseServer(string args)
        {
            _port = Smtp.DefaultPort;

            int p = args.IndexOf(':');
            if (p >= 0)
            {
                _server = args.Substring(0, p);
                _port = int.Parse(args.Substring(p + 1));
                if (_port <= 0 || _port > 65535)
                {
                    throw new ApplicationException(string.Format("Invalid port {0}.", _port));
                }
                _portSet = true;
            }
            else
            {
                _server = args;
                _portSet = false;
            }
        }

        #endregion

        #region Send methods

        /// <summary>
        /// Main send method.
        /// </summary>
        private static void Send()
        {
            if (_method == SendMethod.Smtp && _server == null)
                throw new ApplicationException("Server not specified.");

            if (_from == null)
                throw new ApplicationException("Sender (-from) not specified.");
            
            if (_to == null)
                throw new ApplicationException("Recipient (-to) not specified.");

            if (_subject == null)
                throw new ApplicationException("Mail subject not specified.");

            if (_body == null)
                throw new ApplicationException("Mail body not specified.");

            MailMessage message = new MailMessage();
            message.From = new MailAddressCollection(_from);
            message.To = new MailAddressCollection(_to);
            message.Subject = _subject;
            message.BodyText = _body;

            foreach (string attFile in _attachments)
            {
                message.Attachments.Add(new Attachment(attFile));
            }

            try
            {
                switch (_method)
                {
                    case SendMethod.Smtp:
                        SendSmtp(message);
                        break;
                    case SendMethod.Direct:
                        SendSmtpDirect(message);
                        break;
                    case SendMethod.IIS:
                        SendSmtpViaIis(message);
                        break;
                }
            }
            catch (Exception x)
            {
                Console.WriteLine("Error occured: {0}\n", x.Message);
                Console.WriteLine(x);
            }
            finally
            {
                _attachments.Clear();
            }
        }

        /// <summary>
        /// Send using the specified SMTP server.
        /// </summary>
        private static void SendSmtp(MailMessage message)
        {
            Smtp client = new Smtp();
            try
            { 
                // set SSL parameters to accept all server certificates...
                // do not do this in production code, server certificates should
                // be verified - use ValidatingCertificate event instead
                client.Settings.SslAcceptAllCertificates = true;

                // connect
                Console.WriteLine("Connecting {2} to {0}:{1}...", _server, _port, _security );
                client.Connect(_server, _port, _security);

                Console.WriteLine("Sending message...");
                client.Send(message);
            }
            finally
            {
                // disconnect
                Console.WriteLine("Disconnecting...");
                client.Disconnect();
            }
        }

        /// <summary>
        /// Send using local MS IIS SMTP agent.
        /// </summary>
        private static void SendSmtpViaIis(MailMessage message)
        {
            Console.WriteLine("Sending message through local IIS or Exchange spool directory...");
            MailSpool.Send(MailServerType.Iis, message);
        }

        /// <summary>
        /// Send directly to recipient's SMTP server.
        /// </summary>
        private static void SendSmtpDirect(MailMessage message)
        {
            Console.WriteLine("Sending message directly through recipient's SMTP server...");
            Smtp.SendDirect(message);
        }

        #endregion

    }
}
