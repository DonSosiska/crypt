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
using Rebex.Net;

namespace Rebex.Samples
{
    /// <summary>
    /// Sample application for sendig email via SMTP.
    /// </summary>
    class Program
    {
        private static string _server = null;
        private static int    _port = Smtp.DefaultPort;
        private static string _from = null;
        private static string _to = null;
        private static string _subject = null;
        private static string _body = null;

        /// <summary>
        /// Program entrypoint.
        /// </summary>
        /// <param name="args">Program arguments.</param>
        /// <returns>Status: 0=OK; 1=Not sent; 2=Bad arguments;</returns>
        [STAThread]
        static int Main(string[] args)
        {
            // set the trial license key
            Rebex.Licensing.Key = Rebex.Samples.TrialKey.Key;


            // read command line
            int result = ParseCommandline(args);
            if (result != 0)
                return result;
            
            try
            {
                // send message
                Console.WriteLine("Sending message using {0}:{1}...", _server, _port);
                Smtp.Send(_from, _to, _subject, _body, _server, _port);
                return 0;
            }
            catch (Exception x)
            {
                Console.WriteLine("Error occured: {0}\n", x.Message);
                Console.WriteLine(x);
                return 1;
            }
        }
                
        /// <summary>
        /// Reads server hostname and server port from argument in the form of "servername:port".
        /// </summary>
        /// <param name="serverAddress">Host[:port].</param>
        private static void ParseServerHostnameAndPort(string serverAddress)
        {
            int p = serverAddress.IndexOf(':');
            
            if (p >= 0)
            {
                _server = serverAddress.Substring(0, p);
                _port = int.Parse(serverAddress.Substring(p + 1));
                
                if (_port <= 0 || _port > 65535)
                {
                    throw new ApplicationException(string.Format("Invalid port {0}.", _port));
                }
            }
            else
            {
                _server = serverAddress;
            }
        }

        /// <summary>
        /// Reads arguments from command line and stores them into class properties.
        /// </summary>
        /// <param name="args">Arguments passed to the Main function</param>
        /// <returns>0 = success, anything other = fail</returns>
        private static int ParseCommandline(string[] args)
        {			
            try
            {
                for (int i=0; i<args.Length; i++)
                {
                    string arg = args[i];
                
                    // most parameters starts with "-" or "/" 
                    if (!arg.StartsWith("-") && !arg.StartsWith("/"))
                    {
                        if (_server != null)
                            throw new ApplicationException(string.Format("Unexpected argument '{0}'.", arg));
                            
                        ParseServerHostnameAndPort(arg);
                        continue;
                    }

                    if (i >= args.Length - 1)
                        throw new ApplicationException(string.Format("Missing value for argument '{0}'.", arg));

                    switch (arg.Substring(1).ToLower())
                    {
                        case "from":
                            i++;
                            _from = args[i];
                            break;
                            
                        case "to":
                            i++;
                            _to = args[i];
                            break;
                            
                        case "subject":
                            i++;
                            _subject = args[i];
                            break;
                            
                        case "body":
                            i++;
                            _body = args[i];
                            break;
                            
                        default:
                            throw new ApplicationException(string.Format("Unknown argument '{0}'.", arg));
                    }
                }

                //
                // Check mandatory arguments
                //
                if (_server == null)
                    throw new ApplicationException("Server name not specified.");

                if (_from == null)
                    throw new ApplicationException("Sender (from:) not specified.");
            
                if (_to == null)
                    throw new ApplicationException("Recipient (to:) not specified.");

                if (_subject == null)
                    throw new ApplicationException("Mail subject not specified.");

                if (_body == null)
                    throw new ApplicationException("Mail body not specified.");
                
                return 0; // success
            }
            catch(ApplicationException ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
                
                string applicationName = AppDomain.CurrentDomain.FriendlyName;
                Console.WriteLine("=====================================================================");
                Console.WriteLine(" {0} ", applicationName);
                Console.WriteLine("=====================================================================");		
                Console.WriteLine("");		
                Console.WriteLine("Sends simple e-mail from the command line.");
                Console.WriteLine("");		
                Console.WriteLine("The program is a sample for Rebex Secure Mail for .NET component.");
                Console.WriteLine("For more info, see http://www.rebex.net/secure-mail.net/");

                Console.WriteLine();
                Console.WriteLine("Syntax: {0} server[:port]\n" +
                    "\t-from mail@domain\n" + 
                    "\t-to mail@domain;mail2@domain2\n" +
                    "\t-subject \"subject\"\n" +
                    "\t-body \"text\"\n", applicationName);

                return 2;
            }
        }
    }
}