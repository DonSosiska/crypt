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
using System.Collections;
using System.IO;
using Rebex.Net;
using Rebex.Mail;

namespace Rebex.Samples
{
    /// <summary>
    /// Simple command line IMAP client.
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
        /// IMAP client.
        /// </summary>
        private Imap _imap;

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
            Console.WriteLine("help        connect       disconnect");
            Console.WriteLine("login       head          list");
            Console.WriteLine("get         folders       select");
            Console.WriteLine("examine     unselect      create");
            Console.WriteLine("rename      deletefolder  delete");
            Console.WriteLine("purge       secure        quit");
        }

        /// <summary>
        /// Quit the session.
        /// </summary>
        private void Quit()
        {
            if (_imap == null)
                return;

            if (_imap.State != ImapState.Disconnected)
            {
                Console.WriteLine("Disconnecting...");
                _imap.Disconnect();
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Retrieve and display IMAP folder list.
        /// </summary>
        private void GetFolderList(string folder)
        {
            if (_imap.State == ImapState.Disconnected)
            {
                Console.WriteLine("Not connected.");
                return;
            }

            ImapFolderCollection list = _imap.GetFolderList(folder);

            foreach (ImapFolder f in list)
            {
                Console.WriteLine("* {0}", f.Name);
            }
        }

        /// <summary>
        /// Select or examine folder.
        /// </summary>
        /// <param name="folderName">Mailbox name.</param>
        /// <param name="readOnly">Whether to select folder in read-only mode.</param>
        private void SelectFolder(string folderName, bool readOnly)
        {
            if (_imap.State == ImapState.Disconnected)
            {
                Console.WriteLine("Not connected.");
                return;
            }

            if (folderName == null || folderName.Length == 0)
            {
                if (readOnly)
                    Console.WriteLine("Usage: examine [foldername]");
                else
                    Console.WriteLine("Usage: select [foldername]");
                return;
            }

            _imap.SelectFolder(folderName, readOnly);
            Console.WriteLine("Folder: {0}", _imap.CurrentFolder.Name);
            Console.WriteLine("Total messages: {0}", _imap.CurrentFolder.TotalMessageCount);
            Console.WriteLine("Unread messages: {0}", _imap.CurrentFolder.NotSeenMessageCount);
            Console.WriteLine("Recent messages: {0}", _imap.CurrentFolder.RecentMessageCount);
        }

        /// <summary>
        /// Close currently selected folder.
        /// </summary>
        private void UnselectFolder()
        {
            if (_imap.State == ImapState.Disconnected)
            {
                Console.WriteLine("Not connected.");
                return;
            }

            _imap.UnselectFolder();
        }

        /// <summary>
        /// Create new folder.
        /// </summary>
        private void CreateFolder(string foldername)
        {
            if (_imap.State == ImapState.Disconnected)
            {
                Console.WriteLine("Not connected.");
                return;
            }

            _imap.CreateFolder(foldername);
        }

        /// <summary>
        /// Rename folder.
        /// </summary>
        private void RenameFolder(string folderName)
        {
            if (_imap.State == ImapState.Disconnected)
            {
                Console.WriteLine("Not connected.");
                return;
            }

            string[] names;
            if (folderName != null)
                names = folderName.Split(' ');
            else
                names = null;

            if (names == null || names.Length != 2)
            {
                Console.WriteLine("Usage: rename [current_folder_name] [new_folder_name]");
                return;
            }

            string oldfolderName = names[0];
            string newfolderName = names[1];

            // rename folder
            _imap.RenameFolder(oldfolderName, newfolderName);
        }

        /// <summary>
        /// Delete specified folder.
        /// </summary>
        /// <param name="folderName"></param>
        private void DeleteFolder(string folderName)
        {
            if (_imap.State == ImapState.Disconnected)
            {
                Console.WriteLine("Not connected.");
                return;
            }

            _imap.DeleteFolder(folderName);
        }

        /// <summary>
        /// Remove messages marked as deleted.
        /// </summary>
        private void Purge()
        {
            if (_imap.State == ImapState.Disconnected)
            {
                Console.WriteLine("Not connected.");
                return;
            }

            _imap.Purge();
        }

        /// <summary>
        /// Show headers of one message.
        /// </summary>
        /// <param name="uniqueId">Message unique ID.</param>
        private void ShowMessageHeaders(string uniqueId)
        {
            // check session state
            if (_imap.State == ImapState.Disconnected)
            {
                Console.WriteLine("Not connected.");
                return;
            }

            // show help if message unique ID is empty
            if (uniqueId == null || uniqueId.Length == 0)
            {
                Console.WriteLine("Usage: head uniqueID");
                return;
            }

            // download message headers
            ImapMessageInfo messageInfo = _imap.GetMessageInfo(uniqueId, ImapListFields.FullHeaders);

            // show headers
            for (int i = 0; i < messageInfo.Headers.Count; i++)
                Console.WriteLine(messageInfo.Headers[i]);
        }

        /// <summary>
        /// Download one message.
        /// </summary>
        /// <param name="uniqueIdAndPath">Messsage unique ID [path for saving message].</param>
        private void DownloadMessage(string uniqueIdAndPath)
        {
            if (_imap.State == ImapState.Disconnected)
            {
                Console.WriteLine("Not connected.");
                return;
            }

            // check parameters

            string[] p = null;

            if (uniqueIdAndPath != null && uniqueIdAndPath != "")
                p = uniqueIdAndPath.Split(' ');

            if (p == null || p.Length < 0 || p.Length > 2)
            {
                Console.WriteLine("Usage: get uniqueID [path]");
                return;
            }

            if (p.Length == 1)
            {
                // display message to console
                _imap.GetMessage(p[0], Console.OpenStandardOutput());
            }
            else
            {
                // download message and save it to file
                string localFilename = p[1];

                string extension = Path.GetExtension(localFilename);
                if (string.Equals(extension, ".msg", StringComparison.OrdinalIgnoreCase))
                {
                    // get message from IMAP (and parse it)
                    MailMessage message = _imap.GetMailMessage(p[0]);

                    // save message in MSG format
                    message.Save(localFilename, MailFormat.OutlookMsg);
                }
                else
                {
                    // get message from IMAP directly into file in MIME format (without parsing)
                    _imap.GetMessage(p[0], localFilename);
                }
            }
        }

        /// <summary>
        /// Show message list.
        /// </summary>
        private void GetMessageList()
        {
            ImapMessageCollection messageList;

            if (_imap.State == ImapState.Disconnected)
            {
                Console.WriteLine("Not connected.");
                return;
            }

            //
            // get message list
            //
            Console.WriteLine("Getting message list...");

            messageList = _imap.GetMessageList();

            if (messageList.Count == 0)
            {
                Console.WriteLine("No messages found.");
                return;
            }

            //
            // show list
            //

            // header
            Console.WriteLine("+--------------------------------------------------------------------------+");
            Console.WriteLine("+   Unique ID         | From                                               +");
            Console.WriteLine("+                     | Subject                                            +");
            Console.WriteLine("+                     | Seq.no  Date                                  Size +");
            Console.WriteLine("+--------------------------------------------------------------------------+");

            // messages
            foreach (ImapMessageInfo message in messageList)
            {
                Console.WriteLine(
                    "| {0} | {1} |\n" +
                    "|                     | {2} |\n" +
                    "|                     | {3}   {4}   {5}B |",

                    message.UniqueId.ToString().PadLeft(19),
                    message.From.ToString().PadRight(50),
                    message.Subject.Substring(0, Math.Min(50, message.Subject.Length)).PadRight(50),
                    message.SequenceNumber.ToString().PadRight(4),
                    message.ReceivedDate.LocalTime.ToString("yyyy-MM-dd HH:mm:ss").PadRight(32),
                    message.Length.ToString().PadLeft(7)
                );
                Console.WriteLine("+--------------------------------------------------------------------------+");
            }
        }

        /// <summary>
        /// Delete message.
        /// </summary>
        /// <param name="uniqueId">Message unique ID.</param>
        private void DeleteMessage(string uniqueId)
        {
            if (_imap.State == ImapState.Disconnected)
            {
                Console.WriteLine("Not connected.");
                return;
            }

            if (uniqueId == null || uniqueId.Length == 0)
            {
                Console.WriteLine("Usage: delete uniqueID");
                return;
            }

            // Mark message as deleted
            _imap.DeleteMessage(uniqueId);
        }

        /// <summary>
        /// Open a new session.
        /// </summary>
        /// <param name="arg">Hostname[:port] [none|explicit|implicit].</param>
        /// <returns>Successful?</returns>
        private bool Connect(string arg)
        {
            // create IMAP object
            _imap = new Imap();
            _imap.LogWriter = new ConsoleLogWriter(LogLevel.Info);

            // handle certificate validation
            _imap.ValidatingCertificate += new EventHandler<SslCertificateValidationEventArgs>(ConsoleVerifier.ValidatingCertificate);

            string host;
            int port = -1;
            bool portSet = false;
            SslMode sslMode = SslMode.None;

            try
            {
                // check imap status
                if (_imap.State != ImapState.Disconnected)
                {
                    Console.WriteLine("Already connected.");
                    return false;
                }

                // hostname included?
                if (arg == null || arg.Trim() == "")
                {
                    // read hostname
                    Console.Write("Hostname: ");
                    host = Console.ReadLine().Trim();

                    // read security
                    Console.Write("Security [none|explicit|implicit] (default=none): ");
                    string security = Console.ReadLine().Trim();
                    if (security != "")
                    {
                        try { sslMode = (SslMode)Enum.Parse(typeof(SslMode), security, true); }
                        catch
                        {
                            Console.WriteLine("Invalid security type. Use one of none|explicit|implicit.");
                            return false;
                        }
                    }

                    // read port
                    if (sslMode == SslMode.Implicit)
                        Console.Write("Port (default={0}): ", Imap.DefaultImplicitSslPort);
                    else
                        Console.Write("Port (default={0}): ", Imap.DefaultPort);

                    string sPort = Console.ReadLine().Trim();
                    if (sPort != "")
                    {
                        portSet = true;
                        int.TryParse(sPort, out port);
                    }
                }
                else
                {
                    string[] p = arg.Trim().Split(' ');

                    // parse host and security
                    switch (p.Length)
                    {
                        case 1:
                            host = p[0];
                            break;

                        case 2:
                            host = p[0];
                            try { sslMode = (SslMode)Enum.Parse(typeof(SslMode), p[1], true); }
                            catch
                            {
                                Console.WriteLine("Invalid security type. Use one of none|explicit|implicit.");
                                return false;
                            }
                            break;

                        default:
                            Console.WriteLine("Usage: connect hostname[:port] [none|explicit|implicit]");
                            return false;
                    }

                    // parse port
                    p = host.Split(':');
                    switch (p.Length)
                    {
                        case 1:
                            break;

                        case 2:
                            host = p[0];
                            portSet = true;
                            int.TryParse(p[1], out port);
                            break;

                        default:
                            Console.WriteLine("Usage: connect hostname[:port] [none|explicit|implicit]");
                            return false;
                    }
                }

                if (portSet && (port <= 0 || port > 65535))
                {
                    Console.WriteLine("Invalid port number.");
                    return false;
                }

                // try to connect
                if (portSet)
                    _imap.Connect(host, port, sslMode);
                else
                    _imap.Connect(host, sslMode);

                _secured = (sslMode != SslMode.None);
                return true;
            }
            catch (ImapException e)
            {
                Console.WriteLine(e.Message);
                if (e.Status == ImapExceptionStatus.ProtocolError)
                {
                    _imap.Disconnect();
                    return false;
                }
                
                return false;
            }
            catch (TlsException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("TLS negotiation failed, disconnecting.");
                _imap.Disconnect();
                return false;
            }
        }

        /// <summary>
        /// Disconnect.
        /// </summary>
        private void Disconnect()
        {
            // check imap status

            if (_imap.State == ImapState.Disconnected)
            {
                Console.WriteLine("Not connected.");
                return;
            }

            _imap.Disconnect();
            _logged = false;
            _secured = false;
            Console.WriteLine("Disconnect successful.");
        }

        /// <summary>
        /// Secure the connection.
        /// </summary>
        private void Secure()
        {
            // check imap status

            if (_imap.State == ImapState.Disconnected)
            {
                Console.WriteLine("Not connected.");
                return;
            }

            if (_logged)
            {
                Console.WriteLine("'secure' command is not available after logged in.");
                return;
            }

            if (_secured)
            {
                Console.WriteLine("Connection is already secure.");
                return;
            }

            try
            {
                _imap.Secure();
                _secured = true;
                Console.WriteLine("Connection is secured.");
            }
            catch (ImapException e)
            {
                if (e.Status == ImapExceptionStatus.ProtocolError)
                {
                    Console.WriteLine("Server does not support SSL/TLS security, disconnecting.");
                }
                Console.WriteLine(e.Message);
                _imap.Disconnect();
            }
            catch (TlsException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("TLS negotiation failed, disconnecting.");
                _imap.Disconnect();
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
            _imap.Login(user, pass);
            _logged = true;
            

            Console.WriteLine("Logged in successfully.");
            Console.WriteLine("Use 'folders' to get a folder list or 'select Inbox' to select the Inbox folder, or type 'help' for list of supported commands.");
        }

        /// <summary>
        /// Main loop.
        /// </summary>
        public void Run()
        {
            while (true)
            {
                Console.Write("imap> ");

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
                        // Connect to IMAP server
                        //
                        case "open":
                        case "connect":
                            if (Connect(param))
                                Login();
                            break;

                        // 
                        // Disconnect
                        //
                        case "close":
                        case "disconnect":
                            Disconnect();
                            break;

                        // 
                        // Secure
                        //
                        case "secure":
                            Secure();
                            break;

                        //
                        // Login
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
                            GetMessageList();
                            break;

                        // 
                        // Download message
                        //
                        case "get":
                        case "retr":
                            DownloadMessage(param);
                            break;

                        // 
                        // Show folder list
                        //
                        case "folders":
                            GetFolderList(param);
                            break;

                        // 
                        // Select folder
                        //
                        case "select":
                            SelectFolder(param, false);
                            break;

                        //
                        // Examine
                        //
                        case "examine":
                            SelectFolder(param, true);
                            break;

                        //
                        // Unselect folder
                        //
                        case "unselect":
                            UnselectFolder();
                            break;

                        // 
                        // Create folder
                        // 
                        case "create":
                            CreateFolder(param);
                            break;

                        // 
                        // Rename folder
                        //
                        case "rename":
                            RenameFolder(param);
                            break;

                        // 
                        // Delete folder
                        //
                        case "deletefolder":
                            DeleteFolder(param);
                            break;

                        //
                        // Mark message as deleted
                        //
                        case "del":
                        case "delete":
                            DeleteMessage(param);
                            break;

                        //
                        // Remove deleted message from folder
                        //
                        case "purge":
                            Purge();
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
                catch (ImapException e)
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
                for (int i = 1; i < args.Length; i++)
                    param += " " + args[i];

                if (program.Connect(param))
                    program.Login();
            }

            program.Run();
        }

    }
}
