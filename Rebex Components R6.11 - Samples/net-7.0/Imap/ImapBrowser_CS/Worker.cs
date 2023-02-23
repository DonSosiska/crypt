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
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using System.Collections;
using System.IO;
using Rebex.Net;
using Rebex.Mime;
using Rebex.Mail;

namespace Rebex.Samples
{
    /// <summary>
    /// Background worker class. Implements all non-GUI functionality.
    /// Invokes the main form's methods to update the GUI.
    /// </summary>
    public class Worker
    {
        private readonly Configuration _config;
        private readonly Imap _imap;
        private readonly Hashtable _knownMessages;
        private readonly RemoteMailbox _owner;
        private ImapMessageCollection _messageList;
        private volatile bool _aborting;
        private string _folder;
        private object _sync = new object();
        private volatile bool _newMessage;
        private volatile bool _disposed;

        /// <summary>
        /// Initializes the instance of the worker process.
        /// </summary>
        /// <param name="owner">Owner form of this worker object.</param>
        /// <param name="config">Configuration object.</param>
        public Worker(RemoteMailbox owner, Configuration config)
        {
            _config = config;
            _owner = owner;
            _knownMessages = new Hashtable();
            _imap = new Imap();
            _imap.Notification += imap_Notification;
            _imap.ValidatingCertificate += delegate(object sender, SslCertificateValidationEventArgs e)
            {
                _owner.SafeInvoke((EventHandler<SslCertificateValidationEventArgs>)Verifier.ValidatingCertificate, sender, e);
            };
        }

        public void SetFolder(string folder)
        {
            if (_folder != folder)
            {
                // Forget the old message list if folder has changed.
                _knownMessages.Clear();
                _messageList = null;
            }
            _folder = folder;
        }

        public void ClearCachedMessages()
        {
            _messageList = null;
        }

        /// <summary>
        /// Gets the object that represents the IMAP session.
        /// </summary>
        public Imap Client
        {
            get
            {
                return _imap;
            }
        }

        /// <summary>
        /// Gets or sets whether CheckForUpdates is enabled.
        /// </summary>
        public bool CheckForUpdatesEnabled
        {
            get{ return _checkForUpdatesEnabled; }
            set{ _checkForUpdatesEnabled = value; }
        }
        private volatile bool _checkForUpdatesEnabled;

        /// <summary>
        /// Gets or sets whether RetrieveMessageList finished successfully.
        /// </summary>
        public bool RetrieveMessageListFinished
        {
            get
            { 
                if (_messageList != null && _knownMessages.Count == _messageList.Count)
                    return true;
                else 
                    return false;
            }
        }

        /// <summary>
        /// Aborts the current IMAP operation.
        /// </summary>
        public void Abort(EventHandler abortHandler)
        {
            lock (_sync)
            {
                if (_aborting)
                    return;

                if (IsRunning)
                {
                    _abortHandler = abortHandler;
                    _imap.Abort();
                    _aborting = true;
                    return;
                }
            }

            if (abortHandler != null)
                abortHandler(this, new EventArgs());
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        public void Dispose()
        {
            if (_thread == null)
                _imap.Disconnect();
            _imap.Dispose();
            _disposed = true;
        }

        /// <summary>
        /// Gets the message list from the IMAP connection.
        /// </summary>
        private void GetMessageList()
        {
            if (_messageList == null)
                _messageList = _imap.GetMessageList(ImapListFields.UniqueId);
        }

        // Main form methods delegates.
        private delegate void MessageDelegate(string message);
        private delegate void AddMessageDelegate(ImapMessageInfo message, bool error);
        private delegate void ShowMessageDelegate(MimeMessage message, byte[] raw);

        /// <summary>
        /// Invokes the main form's method to update the status bar text.
        /// </summary>
        /// <param name="message">Status bar text.</param>
        private void SetStatus(string message)
        {
            _owner.SafeInvoke(new MessageDelegate(_owner.SetStatus), new object[] { message });
        }

        /// <summary>
        /// Invokes the main form's method to add a message info to the list.
        /// </summary>
        /// <param name="message">Message info.</param>
        /// <param name="error">True if message was unparsable.</param>
        private void AddMessage(ImapMessageInfo message, bool error)
        {
            _owner.SafeInvoke(new AddMessageDelegate(_owner.AddMessage), new object[] { message, error });

            lock (_knownMessages.SyncRoot)
            {
                _knownMessages.Add(message.UniqueId, message.UniqueId);
            }
        }

        /// <summary>
        /// Invokes the main form's method to remove a message from the list.
        /// </summary>
        /// <param name="uniqueId">Message unique ID.</param>
        private void RemoveMessage(string uniqueId)
        {
            _owner.SafeInvoke(new MessageDelegate(_owner.RemoveMessage), new object[] { uniqueId });
            ForgetMessage(uniqueId);
        }

        /// <summary>
        /// Invokes the main form's method to display a message.
        /// </summary>
        /// <param name="message">Mime message.</param>
        /// <param name="raw">Raw message data.</param>
        private void ShowMessage(MimeMessage message, byte[] raw)
        {
            _owner.SafeInvoke(new ShowMessageDelegate(_owner.ShowMessage), new object[] { message, raw });
        }

        /// <summary>
        /// Invokes the main form's method to display a MessageBox.
        /// </summary>
        /// <param name="message">Message to display.</param>
        private void ShowMessageBox(string message)
        {
            _owner.SafeInvoke(new MessageDelegate(_owner.ShowMessageBox), new object[] { message });
        }

        /// <summary>
        /// Determines whether the message unique ID is known.
        /// </summary>
        /// <param name="uniqueId">Unique ID.</param>
        public bool IsKnownMessage(string uniqueId)
        {
            lock (_knownMessages.SyncRoot)
            {
                return _knownMessages.ContainsKey(uniqueId);
            }
        }

        /// <summary>
        /// Forgets the specified message unique IDs.
        /// </summary>
        /// <param name="uniqueId">Unique ID.</param>
        public void ForgetMessage(string uniqueId)
        {
            lock (_knownMessages.SyncRoot)
            {
                _knownMessages.Remove(uniqueId);
            }
        }

        /// <summary>
        /// Forgets all known message unique IDs.
        /// </summary>
        public void ForgetMessages()
        {
            lock (_knownMessages.SyncRoot)
            {
                _knownMessages.Clear();
            }
        }

        /// <summary>
        /// Callback method delegete.
        /// </summary>
        public delegate void FinishedDelegate(Exception error);

        // Fields required by the background method.
        private object _parameter = null;
        private BackgroundMethod _method = null;
        private Thread _thread = null;
        private FinishedDelegate _callback = null;
        private EventHandler _abortHandler = null;

        /// <summary>
        /// Background method delegate.
        /// </summary>
        private delegate void BackgroundMethod();

        /// <summary>
        /// Gets the value indicating whether a background operation is running.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return _thread != null;
            }
        }

        /// <summary>
        /// Starts the requested asynchronous operation.
        /// </summary>
        /// <param name="method">Background method.</param>
        /// <param name="parameter">Background method argument.</param>
        /// <param name="callback">Callback method to be called when the operation ends.</param>
        private void Start(BackgroundMethod method, object parameter, FinishedDelegate callback)
        {
            if (IsRunning)
                throw new InvalidOperationException("Another operation is still running.");

            // Initialize fields required by the background method.
            _aborting = false;
            _method = method;
            _parameter = parameter;
            _callback = callback;


            // Start the background method in a background thread.
            _thread = new Thread(new ThreadStart(DoMethod));
            _thread.Start();
        }

        /// <summary>
        /// Common background operation stub. Connects, logs in and executes the desired background operation.
        /// </summary>
        private void DoMethod()
        {
            bool connecting = false;
            Exception error;
            try
            {
                if (_imap.State == ImapState.Disconnected)
                {
                    connecting = true;

                    // Get configuration.
                    string server = _config.GetString("server");
                    int port = _config.GetInt32("port");
                    bool singleSignOn = _config.GetBoolean("singleSignOn", false);
                    string userName = _config.GetString("userName");
                    string password = _config.GetString("password");
                    SslMode security;
                    switch (_config.GetInt32("security", 0))
                    {
                        case 1:
                            security = SslMode.Explicit;
                            break;
                        case 2:
                            security = SslMode.Implicit;
                            break;
                        default:
                            security = SslMode.None;
                            break;
                    }
                        
                    _imap.Settings.SslAllowedVersions = (TlsVersion)_config.GetValue("protocol", typeof(TlsVersion));
                    _imap.Settings.SslAllowedSuites = (TlsCipherSuite)_config.GetValue("suite", typeof(TlsCipherSuite));

                    SetStatus(string.Format("Connecting to {0}...", server));

                    // Forget the old message list.
                    _messageList = null;

                    // Connect to the server with the specified security.
                    _imap.Connect(server, port, security);

                    if (singleSignOn)
                        _imap.Login(ImapAuthentication.Auto);
                    else
                        _imap.Login(userName, password);

                    connecting = false;
                }

                if (_folder != null)
                    _imap.SelectFolder(_folder);

                // Run desired background operation.
                _method();

                error = null;
            }
            catch (Exception x)
            {

                ImapException px = x as ImapException;
                if (px != null && px.Status == ImapExceptionStatus.OperationAborted)
                {
                    // If the operation is a result of an abort, don't report it as error.
                    error = null;
                    _imap.Disconnect();
                }
                else
                {
                    error = x;

                    // If this is a failed connection attempt, disconnect.
                    if (connecting)
                        _imap.Disconnect();
                }
            }

            if (_disposed)
                return;

            EventHandler abortHandler;

            lock (_sync)
            {
                _aborting = false;
                _thread = null;
                abortHandler = _abortHandler;
                _abortHandler = null;
            }

            // Resets status bar and informs the main form that the operation finished.
            SetStatus("");
            if (_callback != null)
                _owner.SafeInvoke(_callback, new object[] { error });

            if (abortHandler != null)
                _owner.SafeInvoke(abortHandler, new object[] { null, new EventArgs() });
        }

        /// <summary>
        /// Starts an asynchronous operation to connect to the IMAP server.
        /// </summary>
        /// <param name="callback">Callback method to be called when the operation ends.</param>
        public void StartConnecting(FinishedDelegate callback)
        {
            Start(new BackgroundMethod(DoConnect), null, callback);
        }

        /// <summary>
        /// Starts an asynchronous operation to retrieve the message list.
        /// </summary>
        /// <param name="callback">Callback method to be called when the operation ends.</param>
        public void StartRetrieveMessageList(FinishedDelegate callback)
        {
            Start(new BackgroundMethod(DoRetrieveMessageList), null, callback);
        }

        /// <summary>
        /// Starts an asynchronous operation to delete specified messages.
        /// </summary>
        /// <param name="uniqueIds">List of message unique ID.</param>
        /// <param name="callback">Callback method to be called when the operation ends.</param>
        public void StartDeleteMessages(string[] uniqueIds, FinishedDelegate callback)
        {
            Start(new BackgroundMethod(DoDeleteMessages), uniqueIds, callback);
        }

        /// <summary>
        /// Starts an asynchronous operation to retrieve and displays the message.
        /// </summary>
        /// <param name="uniqueId">Message unique ID.</param>
        /// <param name="callback">Callback method to be called when the operation ends.</param>
        public void StartRetrieveMessage(string uniqueId, FinishedDelegate callback)
        {
            Start(new BackgroundMethod(DoRetrieveMessage), uniqueId, callback);
        }

        /// <summary>
        /// Starts an asynchronous operation to save the message into the supplied stream.
        /// </summary>
        /// <param name="uniqueId">Message unique ID.</param>
        /// <param name="filePath">Output file.</param>
        /// <param name="callback">Callback method to be called when the operation ends.</param>
        public void StartSaveMessage(string uniqueId, string filePath, FinishedDelegate callback)
        {
            Start(new BackgroundMethod(DoSaveMessage), new object[] { uniqueId, filePath }, callback);
        }

        /// <summary>
        /// Starts an asynchronous operation to check for new mails.
        /// </summary>
        public void StartCheckForUpdates(FinishedDelegate callback)
        {
            Start(new BackgroundMethod(DoCheckForUpdates), null, callback);
        }

        /// <summary>
        /// Dummy method.
        /// </summary>
        private void DoConnect()
        {
        }

        /// <summary>
        /// Retrieves the message list.
        /// </summary>
        private void DoRetrieveMessageList()
        {
            SetStatus(string.Format("Getting message IDs from folder {0}...", _imap.CurrentFolder.Name));

            GetMessageList();

            for (int i = _messageList.Count - 1; i >= 0; i--)
            {
                if (_aborting)
                    break;

                ImapMessageInfo message = _messageList[i];

                if (!IsKnownMessage(message.UniqueId))
                {
                    SetStatus(string.Format("Getting message ({0}) from folder {1}...", message.UniqueId, _imap.CurrentFolder.Name));
                    try
                    {
                        ImapMessageInfo m = _imap.GetMessageInfo(message.UniqueId, ImapListFields.Envelope);
                        AddMessage(m, false);
                    }
                    catch (MimeException)
                    {
                        AddMessage(message, true);
                    }
                }
            }
        }

        /// <summary>
        /// Deletes specified messages.
        /// </summary>
        private void DoDeleteMessages()
        {
            string[] uniqueIds = (string[])_parameter;

            GetMessageList();

            for (int i = 0; i < uniqueIds.Length; i++)
            {
                string uniqueId = uniqueIds[i];
                _imap.DeleteMessage(uniqueId);
                ImapMessageInfo info = _messageList.Find(uniqueId);
                if (info != null)
                    _messageList.Remove(info);
                RemoveMessage(uniqueId);
            }

            _imap.Purge();
        }

        /// <summary>
        /// Tests if message is still on server. 
        /// </summary>
        /// <param name="uniqueId">Message unique Id.</param>
        /// <returns>True if message is still on the server; false otherwise.</returns>
        private bool IsMessageOnServer(string uniqueId)
        {
            ImapMessageCollection collection = _imap.GetMessageList(new ImapMessageSet(uniqueId), ImapListFields.UniqueId);
            
            if (collection.Count == 0)
            {
                string message = string.Format("Requested message ({0}) not found.", uniqueId);
                SetStatus(message);
                ShowMessageBox(message);
                RemoveMessage(uniqueId);
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Retrieves and displays the message.
        /// </summary>
        private void DoRetrieveMessage()
        {
            string uniqueId = (string)_parameter;

            GetMessageList();

            if (!IsMessageOnServer(uniqueId))
                return;

            SetStatus(string.Format("Retrieving message ({0}) from folder {1}...", uniqueId, _imap.CurrentFolder.Name));

            MemoryStream buffer = new MemoryStream();
            MimeMessage mime = new MimeMessage();

            _imap.GetMessage(uniqueId, buffer);
            buffer.Position = 0;
            mime.Load(buffer);
            
            byte[] raw = buffer.ToArray();
            ShowMessage(mime, raw);
        }

        /// <summary>
        /// Saves the message into the supplied stream.
        /// </summary>
        private void DoSaveMessage()
        {
            string uniqueId = (string)((object[])_parameter)[0];
            string filePath = (string)((object[])_parameter)[1];

            GetMessageList();

            if (!IsMessageOnServer(uniqueId))
                return;

            SetStatus(string.Format("Retrieving message ({0}) from folder {1}...", uniqueId, _imap.CurrentFolder.Name));

            string extension = Path.GetExtension(filePath);
            if (string.Equals(extension, ".msg", StringComparison.OrdinalIgnoreCase))
            {
                // get message from IMAP (and parse it)
                MailMessage message = _imap.GetMailMessage(uniqueId);

                // save message in MSG format
                message.Save(filePath, MailFormat.OutlookMsg);
            }
            else
            {
                // get message from IMAP directly into file in MIME format (without parsing)
                _imap.GetMessage(uniqueId, filePath);
            }
        }

        /// <summary>
        /// Checks for new mails.
        /// </summary>
        private void DoCheckForUpdates()
        {
            while (!_aborting && _checkForUpdatesEnabled)
            {
                _newMessage = false;
                SetStatus("Checking for new mails...");
                _imap.CheckForUpdates(5000);
                if (_newMessage)
                {
                    _messageList = null;
                    DoRetrieveMessageList();
                }
            }
        }

        private void imap_Notification(object sender, ImapNotificationEventArgs e)
        {
            _newMessage = true;
            _imap.Abort();
        }
    }
}
