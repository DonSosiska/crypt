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
using System.IO;
using Rebex.Net;
using Rebex.Mail;
using Rebex.Mime.Headers;
using Rebex.Security.Certificates;

namespace Rebex.Samples
{
    /// <summary>
    /// Summary description for Arguments.
    /// </summary>
    public class Arguments
    {
        private string _server;
        private int _port = Smtp.DefaultPort;
        private string _user;
        private string _password;
        private string _from;
        private string _to;
        private string _subject;
        private string _body;
        private string _filename;
        private SslMode _security = SslMode.None;
        private bool _sign;
        private bool _encrypt;
        private readonly ArrayList _certificates = new ArrayList();

        /// <summary>
        /// Build the mail message according to parameters specified on the command line.
        /// </summary>
        public MailMessage CreateMessage()
        {
            MailMessage message = new MailMessage();

            // If filename is set load the message from the file
            if (_filename != null)
            {	
                message.Load(_filename);
            }
            else
            {
                // filename is not set - set subject and body
                // from properties 
                if (_subject != null) 
                    message.Subject = _subject;
                            
                if (_body != null) 
                    message.BodyText = _body;
            }
                    
            // set From:
            if (_from != null) 
                message.From = new MailAddress(_from);

            // make sure that sender is specified
            if (message.From.Count == 0)
                throw new ApplicationException("Missing sender ('From').");
                        
            // set To:
            if (_to != null) 
                message.To = new MailAddressCollection(_to);
                    
            // make sure, that at least one recipient is specified
            if (message.To.Count == 0)
                throw new ApplicationException("Missing recipients ('To').");

            return message;
        }

        /// <summary>
        /// Loads the certificates specified on the command line into a memory-based certificate store.
        /// </summary>
        public CertificateStore LoadCertificates()
        {
            // create an empty memory-based certificate store
            CertificateStore store = new CertificateStore(new object[0]);

            // and fill it with certificates specified on the command line
            for (int i=0; i<_certificates.Count; i++)
            {
                Certificate certificate;
                string path = (string)_certificates[i];
                if (!File.Exists(path))
                    throw new ApplicationException(string.Format("Certificate file '{0}' not found.", path));

                string ext = Path.GetExtension(path).ToLower();
                switch (ext)
                {
                    case ".cer":
                    case ".der":
                        certificate = Certificate.LoadDer(path);
                        store.Add(certificate);
                        break;
                    case ".pfx":
                    case ".p12":
                        Console.WriteLine("Please enter password for '{0}':", path);
                        string password = Console.ReadLine();
                        certificate = Certificate.LoadPfx(path, password);
                        store.Add(certificate);
                        break;
                    default:
                        throw new ApplicationException(string.Format("Unsupported certificate type '{0}'.", ext));
                }
            }

            return store;
        }

        /// <summary>
        /// Creates new instance of Arguments class.
        /// </summary>
        /// <param name="args">Command line application arguments.</param>
        public Arguments(string[] args)
        {
            if (args.Length < 1)
                throw new ApplicationException("Expected server argument.");
            ParseServerHostnameAndPort(args[0]);

            // parse command line arguments and copy values to properties.
            for (int i = 1; i < args.Length; i++)
            {
                string arg = args[i];

                // all parameters starts with "-" or "/" 
                if (!arg.StartsWith("-") && !arg.StartsWith("/"))
                {
                    throw new ApplicationException(string.Format("Unexpected argument '{0}'.", arg));
                }

                if (i >= args.Length-1)
                    throw new ApplicationException(string.Format("Missing value for argument '{0}'.", arg));

                i++;
                switch (arg.Substring(1).ToLower())
                {
                    case "username":
                        _user = args[i];
                        break;
                        
                    case "password":
                        _password = args[i];
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
                        
                    case "file":
                        _filename = args[i];
                        break;
                    case "security":
                        _security = (SslMode) Enum.Parse (typeof(SslMode), args[i], true);
                        break;

                    case "sign":
                        _sign = (string.Compare(args[i], "yes", true) == 0);
                        break;

                    case "encrypt":
                        _encrypt = (string.Compare(args[i], "yes", true) == 0);
                        break;

                    case "certificate":
                        _certificates.Add(args[i]);
                        break;

                    default:
                        throw new ApplicationException(string.Format("Unknown argument '{0}'.", arg));
                }
            }
            
            // check mandatory arguments
            if (_server == null)
                throw new ApplicationException("Server not specified.");
        }

        /// <summary>
        /// SMTP server name or ip address.
        /// </summary>
        public string Server
        {
            get { return _server; }
        }

        /// <summary>
        /// SMTP server port.
        /// </summary>
        public int Port
        {
            get { return _port; }
        }

        /// <summary>
        /// Username for logging to the server.
        /// </summary>
        public string User
        {
            get { return _user; }
        }

        /// <summary>
        /// Password for logging to the server.
        /// </summary>
        public string Password
        {
            get { return _password; }
        }

        /// <summary>
        /// Security for connecting to the server.
        /// </summary>
        public SslMode Security
        {
            get { return _security; }
        }

        /// <summary>
        /// Specifies whether to encrypt the message.
        /// </summary>
        public bool Encrypt
        {
            get { return _encrypt; }
        }

        /// <summary>
        /// Specifies whether to sign the message.
        /// </summary>
        public bool Sign
        {
            get { return _sign; }
        }

        /// <summary>
        /// Reads server hostname and server port from argument in the form of "servername:port".
        /// </summary>
        /// <param name="serverAddress">Host[:port].</param>
        private void ParseServerHostnameAndPort(string serverAddress)
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
    }
}
