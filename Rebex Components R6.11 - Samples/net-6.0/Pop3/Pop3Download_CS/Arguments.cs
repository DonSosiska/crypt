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
using System.Net;
using Rebex.Net;
using Rebex.Mail;

namespace Rebex.Samples
{
    public class Arguments
    {
        private string _server = null;
        private int _port = Pop3.DefaultPort;
        private string _user = null;
        private string _password = null;
        private SslMode _security = SslMode.None;
        private MailFormat _format = MailFormat.Mime;

        /// <summary>
        /// Creates instance of Arguments class. Reads command line arguments and stores it into the properties.
        /// </summary>
        /// <param name="args">Commandline arguments</param>
        public Arguments(string[] args)
        {
            // parse command line arguments and copy values to properties.
            if (args.Length < 1)
                throw new ApplicationException("Expected security argument.");
            _security = (SslMode)Enum.Parse(typeof(SslMode), args[0], true);

            if (args.Length < 2)
                throw new ApplicationException("Expected server argument.");
            ParseServerHostnameAndPort(args[1]);

            for (int i = 2; i < args.Length; i++)
            {
                string arg = args[i];

                if (!arg.StartsWith("-") && !arg.StartsWith("/"))
                {
                    throw new ApplicationException(string.Format("Unexpected argument '{0}'.", arg));
                }

                if (i >= args.Length - 1)
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

                    case "format":
                        {
                            if (string.Equals(args[i], "msg", StringComparison.OrdinalIgnoreCase))
                                _format = MailFormat.OutlookMsg;
                            else if (string.Equals(args[i], "mime", StringComparison.OrdinalIgnoreCase))
                                _format = MailFormat.Mime;
                            else
                                throw new ApplicationException(string.Format("Uknown mail format '{0}'.", args[i]));
                        }
                        break;

                    default:
                        throw new ApplicationException(string.Format("Unknown argument '{0}'.", arg));
                }
            }

            // check mandatory arguments
            if (_user == null)
                throw new ApplicationException("Username not specified.");

            if (_password == null)
                throw new ApplicationException("Password not specified.");
        }

        /// <summary>
        /// POP3 server hostname or an IP address.
        /// </summary>
        public string Server
        {
            get
            {
                return _server;
            }
        }

        /// <summary>
        /// POP3 server port. Default is 110.
        /// </summary>
        public int Port
        {
            get
            {
                return _port;
            }
        }

        /// <summary>
        /// POP3 server username.
        /// </summary>
        public string User
        {
            get
            {
                return _user;
            }
        }

        /// <summary>
        /// POP3 server password.
        /// </summary>
        public string Password
        {
            get
            {
                return _password;
            }
        }

        /// <summary>
        /// Security for connecting to the server.
        /// </summary>
        public SslMode Security
        {
            get { return _security; }
        }

        /// <summary>
        /// MailFormat for saving emails.
        /// </summary>
        public MailFormat Format
        {
            get { return _format; }
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
