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
using System.Text;
using Rebex.Net;
using System.IO;
using Rebex.Mail;

namespace Rebex.Samples
{
    /// <summary>
    /// Simple command line POP3 client
    /// </summary>
    class Program
    {

        public static string ReadLine()
        {
            StringBuilder line = new StringBuilder();
            bool complete = false;

            while (!complete)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        complete = true;
                        break;

                    case ConsoleKey.Backspace:
                        if (line.Length > 0)
                        {
                            line = line.Remove(line.Length - 1, 1);
                            Console.Write(key.KeyChar);
                            Console.Write(" ");
                            Console.Write(key.KeyChar);
                        }
                        break;

                    default:
                        if (((int)key.KeyChar >= 0x20) || (key.Key == ConsoleKey.Tab))
                        {
                            line = line.Append(key.KeyChar);
                            Console.Write("*");
                        }
                        break;
                }
            }

            Console.WriteLine();
            return line.ToString();
        }

        /// <summary>
        /// POP3 client.
        /// </summary>
        private Pop3 _pop3;
        
        private SslMode _security = SslMode.None;
        private bool _logged = false;
        private bool _secured = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Program"/> class.
        /// </summary>
        public Program()
        {
            
        }

        /// <summary>
        /// Show program help.
        /// </summary>
        private void Help()
        {
            Console.WriteLine("help      connect");     
            Console.WriteLine("login     list");
            Console.WriteLine("head      get");
            Console.WriteLine("delete    undelete");
            Console.WriteLine("secure");
            Console.WriteLine("quit      disconnect");
        }

        /// <summary>
        /// Quit the session.
        /// </summary>
        private void Quit()
        {
            if (_pop3 == null)
            {
                return;
            }

            if (_pop3.State != Pop3State.Disconnected)
            {
                Console.WriteLine("Disconnecting...");
                _pop3.Disconnect();
            }

            Console.WriteLine();		
        }

        /// <summary>
        /// Show headers of one message.
        /// </summary>
        /// <param name="messageId">Message ID.</param>
        private void ShowMessageHeaders(string messageId)
        {
            // check session state
            if (_pop3.State == Pop3State.Disconnected)
            {
                Console.WriteLine("Not connected.");
                return;
            }
            
            // show help if message id is empty
            if (messageId == null || messageId.Length == 0)
            {
                Console.WriteLine("Usage: head id");
                return;
            }

            // try to parse message id
            int id = ParseMessageId(messageId);
            
            // is id valid?
            if (id <= 0)
            {
                Console.WriteLine("Invalid message ID.");
                return;
            }

            // download message headers
            Pop3MessageInfo messageInfo = _pop3.GetMessageInfo(id, Pop3ListFields.FullHeaders);
            
            // show headers
            for (int i=0; i < messageInfo.Headers.Count; i++)
                Console.WriteLine(messageInfo.Headers[i]);
        }

        /// <summary>
        /// Download the specified message.
        /// </summary>
        /// <param name="messageId">Messsage ID (and optionally path for saving message). e.g.: '7' or '7 mail.eml'.</param>
        private void DownloadMessage(string messageId)
        {
            if (_pop3.State == Pop3State.Disconnected)
            {
                Console.WriteLine("Not connected.");
                return;
            }

            // check parameters

            string[] p = null;
            
            if (messageId != null && messageId != "") 
                p = messageId.Split(' ');

            if (p == null || p.Length < 0 || p.Length > 2)
            {
                Console.WriteLine("Usage: retr id [path]");
                return;
            }
                        
            // try to parse message id
            int id = ParseMessageId(p[0]);

            // is id valid?
            if (id <= 0)
            {
                Console.WriteLine("Invalid message ID.");
                return;
            }

            if (p.Length == 1)
            {
                // display message to console
                _pop3.GetMessage(id, Console.OpenStandardOutput());	
            }
            else
            {
                // download message and save it to file
                string localFilename = p[1];
                string extension = Path.GetExtension(localFilename);

                if (string.Equals(extension, ".msg", StringComparison.OrdinalIgnoreCase))
                {
                    //get message from POP3, parse it
                    MailMessage message = _pop3.GetMailMessage(id);
                    //save the message in MSG format
                    message.Save(localFilename, MailFormat.OutlookMsg);
                }
                else
                {
                    // save message from POP3 directly in MIME format (without parsing)
                    _pop3.GetMessage(id, localFilename);
                }
            }
        }

        /// <summary>
        /// Show message list.
        /// </summary>
        private void List()
        {
            Pop3MessageCollection messageList;

            if (_pop3.State == Pop3State.Disconnected)
            {
                Console.WriteLine("Not connected.");
                return;
            }
            
            //
            // get message list
            //
            int messageCount = _pop3.GetMessageCount();
            
            Console.WriteLine
            (
                "Getting {0} ({1} bytes)...", 
                FormatMessageCount(messageCount).ToString(),
                _pop3.GetMailboxSize()
            );

            messageList = _pop3.GetMessageList(Pop3ListFields.Fast);

            if (messageList.Count == 0)
            {
                Console.WriteLine("No messages found.");
                return;
            }

            //
            // show list
            //
            
            // header
            Console.WriteLine("+----------------------------------------------------------------------+");
            Console.WriteLine("| S.No |    Length | Unique ID                                         |");
            Console.WriteLine("+----------------------------------------------------------------------+");

            // message list
            foreach (Pop3MessageInfo message in messageList)
            {
                Console.WriteLine(
                    "| {0} |{1} B | {2} |",
                    message.SequenceNumber.ToString().PadLeft(4),
                    message.Length.ToString().PadLeft(8),
                    message.UniqueId.ToString().PadLeft(48)
                );
                
            }

            Console.WriteLine("+----------------------------------------------------------------------+");
        }

        /// <summary>
        /// Delete message.
        /// </summary>
        /// <param name="messageId">Message ID.</param>
        private void Delete(string messageId)
        {
            if (_pop3.State == Pop3State.Disconnected)
            {
                Console.WriteLine("Not connected.");
                return;
            }

            if (messageId == null || messageId.Length == 0)
            {
                Console.WriteLine("Usage: delete id");
                return;
            }

            int id = ParseMessageId(messageId);

            if (id <= 0)
            {
                Console.WriteLine("Invalid message ID.");
                return;
            }
            
            // Mark message as deleted
            _pop3.Delete(id);
        }

        /// <summary>
        /// Undelete deleted messages.
        /// </summary>
        private void Undelete()
        {
            if (_pop3.State == Pop3State.Disconnected)
            {
                Console.WriteLine("Not connected.");
                return;
            }

            _pop3.Undelete();
        }

        /// <summary>
        /// Open a new session.
        /// </summary>
        /// <param name="host">Host [port] [none|explicit|implicit].</param>
        /// <returns>Successful?</returns>
        private bool Connect(string host)
        {
            // create Pop3 object 
            _pop3 = new Pop3();
            _pop3.LogWriter = new ConsoleLogWriter(LogLevel.Info);

            // handle certificate validation
            _pop3.ValidatingCertificate += new EventHandler<SslCertificateValidationEventArgs>(ConsoleVerifier.ValidatingCertificate);

            int port = -1;

            try
            {
                // check pop3 status
                if (_pop3.State != Pop3State.Disconnected)
                {
                    Console.WriteLine("Already connected.");
                    return false;
                }

                // hostname included?
                if (host == null || host.Trim() == "")
                {
                    // read hostname
                    Console.Write("Hostname: ");
                    host = Console.ReadLine().Trim();

                    // read port
                    Console.Write("Port (default={0}): ", Pop3.DefaultPort);
                    string sPort = Console.ReadLine().Trim();
                    if (sPort == "")
                        port = Pop3.DefaultPort;
                    else
                        try	{ port = int.Parse(sPort); }
                        catch {	port = -1; }

                    // read security
                    Console.Write("Security [none|explicit|implicit] (default=none): ");
                    string security = Console.ReadLine().Trim();
                    if( security == "" )
                        _security = SslMode.None;
                    else
                        try	{ _security = (SslMode) Enum.Parse (typeof(SslMode), security, true);	}
                        catch
                        {
                            Console.WriteLine("Invalid security type. Use one of none|explicit|implicit.");
                            return false;
                        }
                }
                else
                {
                    // check number of parameters

                    string [] p = host.Split(' ',':');

                    if (p.Length < 1 || p.Length > 3)
                    {
                        Console.WriteLine("Usage: connect hostname [port] [none|explicit|implicit]");
                        return false;
                    }
                    host = p[0];
                    
                    // set port and security
                    port = Pop3.DefaultPort;
                    _security = SslMode.None;
                    switch(p.Length)
                    {				
                        case 2:
                            try	{ port = int.Parse(p[1]); }
                            catch 
                            {	
                                try	{ _security = (SslMode) Enum.Parse (typeof(SslMode), p[1], true);	}
                                catch
                                {
                                    Console.WriteLine("Invalid argument '{0}'.",p[1]);
                                    Console.WriteLine("Usage: hostname [port] [none|explicit|implicit]");
                                    return false;
                                } 
                            }
                            break;

                        case 3:
                            try	{ port = int.Parse(p[1]); }
                            catch {	port = -1; }

                            try	{ _security = (SslMode) Enum.Parse (typeof(SslMode), p[2], true);	}
                            catch
                            {
                                Console.WriteLine("Invalid security type. Use one of none|explicit|implicit.");
                                return false;
                            }
                            break;
                    }
                }

                if (port <= 0 || port > 65535)
                {
                    Console.WriteLine("Invalid port number.");
                    return false;
                }

                _secured = _security != SslMode.None;

                _pop3.Connect(host, port, _security);
                
                if (_secured)
                {
                    Console.WriteLine("Connection was secured using {0}.", _pop3.TlsSocket.Cipher.Protocol);
                    Console.WriteLine("Connection is using cipher {0}.", _pop3.TlsSocket.Cipher);
                }
                return true;
            }
            catch(Pop3Exception e)
            {
                if (e.Status == Pop3ExceptionStatus.ProtocolError)
                {
                    Console.WriteLine("Server does not support SSL/TLS security.");
                    _pop3.Disconnect();
                    return false;
                }
                Console.WriteLine(e.Message);
                return false;
            }
            catch(TlsException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("TLS negotiation failed, disconnecting.");
                _pop3.Disconnect();
                return false;
            }
        }

        /// <summary>
        /// Login.
        /// </summary>
        private void Login()
        {
            // get username
            Console.Write("User: ");
            string user = Console.ReadLine();

            // get password
            Console.Write("Password: ");
            string pass = ReadLine();

            // login
            _pop3.Login(user, pass);
            _logged = true;

            Console.WriteLine("Logged in successfully.");
        }

        /// <summary>
        /// Disconnect.
        /// </summary>
        /// If set to <param name="rollback">rollback</param>, don't delete messages marked as deleted. 
        /// It is same as you type Undelete and then Disconnect without arguments
        public void Disconnect(string rollback)
        {
            // check pop3 status
            if (_pop3.State == Pop3State.Disconnected)
            {
                Console.WriteLine("Not connected.");
                return;
            }

            if( rollback == null || rollback == "" )
            {
                Console.WriteLine("Disconnecting...");
                // commit changes and disconnect
                _pop3.Disconnect(false);
            }
            else if( rollback.ToLower() == "rollback" ) 
            {
                Console.WriteLine("Disconnecting...");
                // undelete and disconnect
                _pop3.Disconnect(true);
            }
            else
            {
                Console.WriteLine("Usage: disconnect [rollback]");
                return;
            }
            _logged = false;
            _secured = false;
            Console.WriteLine("Disconnect successful.");
        }

        /// <summary>
        /// Secure the connection.
        /// </summary>
        public void Secure()
        {

            if (_pop3.State == Pop3State.Disconnected)
            {
                Console.WriteLine("Not connected.");
                return;
            }

            if(_logged)
            {
                Console.WriteLine("'secure' command is not available after logged in.");
                return;
            }

            if(_secured)
            {
                Console.WriteLine("Connection is already secure.");
                return;
            }

            try
            {
                _pop3.Secure();
                _secured = true;
                Console.WriteLine("Connection is secured.");
            }
            catch(Pop3Exception e)
            {
                if (e.Status == Pop3ExceptionStatus.ProtocolError)
                {
                    Console.WriteLine("Server does not support SSL/TLS security, disconnecting.");
                }
                Console.WriteLine(e.Message);
            }
            catch(TlsException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("TLS negotiation failed, disconnecting.");
                return;
            }

        }

        /// <summary>
        /// Main loop.
        /// </summary>
        public void Run()
        {
            while (true)
            {
                Console.Write("pop3> ");

                string command = Console.ReadLine().Trim();

                string param = null;

                int i = command.IndexOf(' ');

                if (i > 0)
                {
                    param = command.Substring(i + 1);
                    command = command.Substring(0, i);
                }

                try
                {
                    switch (command.ToLower())
                    {
                        //
                        // Show help
                        //
                        case "?":
                        case "help":
                            Help();
                            break;
                        
                        //
                        // Quit application
                        //
                        case "!":
                        case "bye":
                        case "exit":
                        case "quit":
                            Quit();
                            return;
                        
                        //
                        // Connect to POP3 server
                        //
                        case "open":
                        case "connect":
                            if (!Connect(param)) break;
                            goto case "login";
                        
                        //
                        // Disconnect to POP3 server
                        //
                        case "close":
                        case "disconnect":
                            Disconnect(param);
                            break;

                        //
                        // Secure the connection
                        //
                        case "secure":
                            Secure();
                            break;

                        //
                        // Login to POP3 server
                        //
                        case "login":
                            Login();
                            break;
                        
                        //
                        // Show message headers
                        //
                        case "head":
                            ShowMessageHeaders(param);
                            break;
                        
                        //
                        // Show message list
                        //
                        case "list":
                            List();
                            break;
                        
                        //
                        // Show full message
                        //
                        case "get":
                        case "retr":
                            DownloadMessage(param);
                            break;
                        
                        //
                        // Delete a message
                        //
                        case "del":
                        case "delete":
                            Delete(param);
                            break;
                        
                        // 
                        // Undelete a message
                        //
                        case "undel":
                        case "undelete":
                            Undelete();
                            break;
                        
                        //
                        // Default - invalid command
                        //	
                        default:
                            Console.WriteLine("Invalid command.");
                            break;
                    }
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (InvalidOperationException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (Pop3Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // set the trial license key
            Rebex.Licensing.Key = Rebex.Samples.TrialKey.Key;


            Program program = new Program();

            if (args.Length > 0)
            {
                string param = args[0];
                for (int i = 1; i < args.Length; i ++)
                    param += " " + args[i];

                if (program.Connect(param))
                    program.Login();
            } 

            program.Run();
        }
        
        /// <summary>
        /// Converts string message ID to a number. If the string is invalid, it returns 0.
        /// </summary>
        /// <param name="id">String message ID.</param>
        /// <returns>Valid ID or 0.</returns>
        public int ParseMessageId(string id)
        {
            try
            {
                return int.Parse(id);
            }
            catch
            {
                return 0;
            }
        }
        
        /// <summary>
        /// Converts message count from integer to human-friendly form. 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public string FormatMessageCount (int count)
        {
            if (count == 1)
                return "1 message";
            else
                return string.Format("{0} messages", count);
        }
    }
}
