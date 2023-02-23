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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

using Rebex.Mail;
using Rebex.Mime;
using Rebex.Net;
using Rebex.Security.Certificates;

namespace Rebex.Samples
{
    /// <summary>
    /// Exchange Browser sample.
    /// </summary>
    public partial class MainForm : Form
    {
        private const string TitleFormat = "Exchange Browser - {0}";

        // EWS client object
        private Ews _ews;

        // currently selected folder
        private EwsFolderTree.FolderInfo _currentFolder;

        // cache for downloaded EWS items
        private readonly Dictionary<int, EwsListViewItem> _itemCache;

        // list view item with additional fields for working with Ews items
        private class EwsListViewItem : ListViewItem
        {
            public EwsItemId Id { get; private set; }
            public EwsItemType ItemType { get; private set; }

            public EwsListViewItem(EwsItemId id, EwsItemType type, params string[] columns)
                : base(columns)
            {
                Id = id;
                ItemType = type;
            }

            public static EwsListViewItem GetLoadingItem()
            {
                return new EwsListViewItem(null, EwsItemType.Unknown, "Loading...", "", "");
            }
        }

        /// <summary>
        /// Initializes new instance of the form.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            _itemCache = new Dictionary<int, EwsListViewItem>();

            folderTree.ErrorOccured += folderTree_ErrorOccured;

            SetConnectMenu(false);
        }

        /// <summary>
        /// Handles 'Shown' (form is shown for the first time) event.
        /// </summary>
        private void MainForm_Shown(object sender, EventArgs e)
        {
            Connect();
        }

        /// <summary>
        /// Handles clicking on 'Connect' menu item.
        /// </summary>
        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Connect();
        }

        /// <summary>
        /// Handles clicking on 'Disconnect' menu item.
        /// </summary>
        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Disconnect();
        }

        /// <summary>
        /// Handles clicking on 'Exit' menu item.
        /// </summary>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Handles form closing event.
        /// </summary>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Disconnect();
        }

        /// <summary>
        /// Handles an item requests of the virtual item list view.
        /// </summary>
        private void itemListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            EwsListViewItem item;
            if (_itemCache.TryGetValue(e.ItemIndex, out item))
            {
                e.Item = item;
            }
            else
            {
                e.Item = EwsListViewItem.GetLoadingItem();
                LoadItems(e.ItemIndex);
            }
        }

        /// <summary>
        /// Handles double-clicking an item in item list view.
        /// </summary>
        private void itemListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            EwsListViewItem item = GetSelectedItem();
            if (item == null || item.Id == null)
                return;

            ShowItem(item.Id, item.ItemType == EwsItemType.Message);
        }

        /// <summary>
        /// Handles clicking on 'View' context menu.
        /// </summary>
        private void viewMeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EwsListViewItem item = GetSelectedItem();
            if (item == null || item.Id == null)
                return;

            ShowItem(item.Id, item.ItemType == EwsItemType.Message);
        }

        /// <summary>
        /// Handles clicking on 'Delete' context menu.
        /// </summary>
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EwsListViewItem item = GetSelectedItem();
            if (item == null || item.Id == null)
                return;

            DialogResult result = MessageBox.Show("Are you sure you want to delete selected item?", "Delete prompt", MessageBoxButtons.YesNo);
            if (result != DialogResult.Yes)
                return;

            DeleteItem(item.Id);
        }

        /// <summary>
        /// Handles clicking on 'Save As' context menu.
        /// </summary>
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EwsListViewItem item = GetSelectedItem();
            if (item == null || item.Id == null)
                return;

            DownloadItem(item.Id, item.ItemType == EwsItemType.Message);
        }

        /// <summary>
        /// Handles changes in selection of folder tree.
        /// </summary>
        private void folderTree_SelectionChanged(object sender, EventArgs e)
        {
            if (_currentFolder == folderTree.SelectedFolder)
                return;

            _currentFolder = folderTree.SelectedFolder;

            this.Text = string.Format(TitleFormat, _currentFolder.Path);

            _itemCache.Clear();
            itemListView.VirtualListSize = 0;
            itemListView.VirtualListSize = _currentFolder.ItemsTotal;
        }

        /// <summary>
        /// Handles errors in folder tree.
        /// </summary>
        void folderTree_ErrorOccured(object sender, ErrorOccuredEventArgs e)
        {
            ReportError(e.Error);
            e.Handled = true;
        }

        /// <summary>
        /// Connects and log in client based on the <see cref="ConectionForm"/> values.
        /// After successful login initializes folder tree.
        /// </summary>
        private async void Connect()
        {
            try
            {
                // load connection settings
                ConnectionFormData data = new ConnectionFormData();
                data.LoadConfig();

                // show connection dialog
                ConnectionForm connection = new ConnectionForm(data);
                connection.ShowDialog();

                // not confirmed?
                if (connection.DialogResult != DialogResult.OK)
                    return;

                // save confirmed connection settings
                data = connection.Data;
                data.SaveConfig();

                // create EWS object
                _ews = new Ews();

                // proxy
                if (data.ProxyType != ProxyType.None)
                {
                    _ews.Proxy = new Proxy(data.ProxyType, data.ProxyHost, data.ProxyPort);
                    if (data.ProxyUser.Length > 0)
                    {
                        _ews.Proxy.AuthenticationMethod = ProxyAuthentication.Basic;
                        _ews.Proxy.Credentials = new System.Net.NetworkCredential(data.ProxyUser, data.ProxyPassword);
                    }
                }

                // security
                bool isSecure = (data.Protocol == ProtocolMode.HTTPS);
                if (isSecure)
                {
                    // server certificate handler
                    switch (data.ServerCertificateVerifyingMode)
                    {
                        case CertificateVerifyingMode.AcceptAnyCertificate:
                            _ews.Settings.SslAcceptAllCertificates = true;
                            break;

                        case CertificateVerifyingMode.LocalyStoredThumbprint:
                            _ews.ValidatingCertificate += (s, e) =>
                            {
                                var verifier = new ThumbprintVerifier(data.ServerCertificateThumbprint);
                                verifier.ValidatingCertificate(s, e);

                                // save accepted thumbprint if differs from the stored one
                                if (verifier.IsAccepted && data.ServerCertificateThumbprint != verifier.Thumbprint)
                                {
                                    data.ServerCertificateThumbprint = verifier.Thumbprint;
                                    data.SaveConfig();
                                }
                            };
                            break;

                        default:
                            _ews.ValidatingCertificate += Verifier.ValidatingCertificate;
                            break;
                    }

                    // client certificate handler
                    if (data.ClientCertificate != null)
                    {
                        _ews.Settings.SslClientCertificateRequestHandler = CertificateRequestHandler.CreateRequestHandler(data.ClientCertificate);
                    }
                    else if (!string.IsNullOrEmpty(data.ClientCertificateFilename))
                    {
                        // certificate not loaded yet, ask for password
                        PassphraseDialog pp = new PassphraseDialog();
                        if (pp.ShowDialog() == DialogResult.OK)
                        {
                            CertificateChain chain = CertificateChain.LoadPfx(data.ClientCertificateFilename, pp.Passphrase);
                            _ews.Settings.SslClientCertificateRequestHandler = CertificateRequestHandler.CreateRequestHandler(chain);
                        }
                        else
                        {
                            throw new ApplicationException("Password wasn't entered.");
                        }
                    }
                    else
                    {
                        _ews.Settings.SslClientCertificateRequestHandler = new RequestHandler();
                    }

                    _ews.Settings.SslAllowedSuites = data.AllowedSuite;
                    _ews.Settings.SslAllowedVersions = data.TlsProtocol;
                }

                SetConnectMenu(true);
                SetWorking(true);
                try
                {
                    // connect
                    await _ews.ConnectAsync(data.ServerHost, data.ServerPort, isSecure ? SslMode.Implicit : SslMode.None);

                    // login
                    if (!_ews.IsAuthenticated)
                    {
                        if (data.UseSingleSignOn)
                            await _ews.LoginAsync(EwsAuthentication.Auto);
                        else
                            await _ews.LoginAsync(data.ServerUser, data.ServerPassword);
                    }

                    // fill folder tree
                    EwsFolderId folderId;
                    if (string.IsNullOrEmpty(data.Mailbox))
                        folderId = EwsFolderId.Root;
                    else
                        folderId = new EwsFolderId(EwsSpecialFolder.Root, data.Mailbox);

                    // bind folder tree
                    await folderTree.BindAsync(_ews, folderId);
                }
                finally
                {
                    SetWorking(false);
                }
            }
            catch (Exception ex)
            {
                ReportError(ex);
                Disconnect();
            }
        }

        /// <summary>
        /// Disconnects the client object and clears the form.
        /// </summary>
        private void Disconnect()
        {
            if (_ews != null)
            {
                // stop and dispose EWS client object
                _ews.Dispose();
                _ews = null;

                // clear controls
                _itemCache.Clear();
                folderTree.Clear();
                itemListView.VirtualListSize = 0;
            }

            // reset connect menu
            SetConnectMenu(false);
        }

        /// <summary>
        /// Loads items around given index.
        /// </summary>
        /// <param name="offset">Index of the item, which causes the necessity of loading items.</param>
        private async void LoadItems(int offset)
        {
            // quit if busy
            if (_ews.IsBusy)
                return;

            SetWorking(true);
            try
            {
                // compute indices for page view
                int pageSize = 20;
                int start = (offset / pageSize) * pageSize;
                int end = start + pageSize;
                if (start > 0 && !_itemCache.ContainsKey(start - pageSize))
                    start -= pageSize;
                if (!_itemCache.ContainsKey(end + pageSize))
                    end += pageSize;

                // get list of requested messages
                var pageView = EwsPageView.CreateIndexed(start, end - start);
                var list = await _ews.GetMessageListAsync(_currentFolder.Id, EwsItemFields.Envelope, pageView);

                // update total items count
                int total = list.PageResult.ItemsTotal;
                if (itemListView.VirtualListSize != total)
                {
                    _itemCache.Clear();
                    itemListView.VirtualListSize = 0;
                    itemListView.VirtualListSize = total;
                }

                // fill cache
                for (int i = 0; i < list.Count; i++)
                {
                    EwsMessageInfo info = list[i];
                    var item = new EwsListViewItem(info.Id, info.ItemType, Convert.ToString(info.From), info.Subject, Convert.ToString(info.ReceivedDate));
                    _itemCache[i + start] = item;
                }

                // redraw content 
                itemListView.Invalidate();
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
            finally
            {
                SetWorking(false);
            }
        }

        /// <summary>
        /// Shows a form with detailed message info.
        /// </summary>
        private async void ShowItem(EwsItemId id, bool hasMime)
        {
            // quit if busy
            if (_ews.IsBusy)
                return;

            SetWorking(true);
            try
            {
                MemoryStream buffer = new MemoryStream();
                if (hasMime)
                {
                    // download message into memory
                    await _ews.GetMessageAsync(id, buffer);

                    // parse downloaded MIME data
                    buffer.Position = 0;
                    MimeMessage message = new MimeMessage();
                    message.Load(buffer);

                    // show parsed message
                    byte[] raw = buffer.ToArray();
                    MessageView view = new MessageView(message, raw);
                    view.Show();
                }
                else
                {
                    // download item into memory
                    await _ews.GetItemAsync(id, buffer, EwsItemFormat.Xml);

                    // convert raw data to string
                    string content = Encoding.UTF8.GetString(buffer.ToArray());

                    // show item
                    StringView view = new StringView("Item view", content);
                    view.Show();
                }
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
            finally
            {
                SetWorking(false);
            }
        }

        /// <summary>
        /// Deletes specified item.
        /// </summary>
        private async void DeleteItem(EwsItemId id)
        {
            // quit if busy
            if (_ews.IsBusy)
                return;

            SetWorking(true);
            try
            {
                // delete the item
                await _ews.DeleteItemAsync(id);

                // clear Cache and ListView to force items to be reloaded
                int size = itemListView.VirtualListSize;
                _itemCache.Clear();
                itemListView.VirtualListSize = 0;
                itemListView.VirtualListSize = Math.Max(0, size - 1);
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
            finally
            {
                SetWorking(false);
            }
        }

        /// <summary>
        /// Downloads specified item.
        /// </summary>
        private async void DownloadItem(EwsItemId id, bool hasMime)
        {
            // quit if busy
            if (_ews.IsBusy)
                return;

            SetWorking(true);
            try
            {
                // prepare save dialog
                SaveFileDialog save = new SaveFileDialog();
                save.AddExtension = true;
                if (hasMime)
                {
                    save.Filter = "MIME mails (*.eml)|*.eml|SOAP XML (*.xml)|*.xml|Exchange native binary (*.bin)|*.bin|All files (*.*)|*.*";
                    save.DefaultExt = "eml";
                    save.FileName = id.Value + ".eml";
                }
                else
                {
                    save.Filter = "SOAP XML (*.xml)|*.xml|Exchange native binary (*.bin)|*.bin|All files (*.*)|*.*";
                    save.DefaultExt = "xml";
                    save.FileName = id.Value + ".xml";
                }

                DialogResult res = save.ShowDialog();
                if (res != DialogResult.OK)
                    return;

                // determine save format from extension
                string extension = Path.GetExtension(save.FileName);

                EwsItemFormat format;
                switch (extension.ToLowerInvariant())
                {
                    case ".bin": format = EwsItemFormat.Native; break;
                    case ".xml": format = EwsItemFormat.Xml; break;
                    default: format = hasMime ? EwsItemFormat.Mime : EwsItemFormat.Xml; break;
                }

                // download message in specified format
                await _ews.GetItemAsync(id, save.FileName, format);
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
            finally
            {
                SetWorking(false);
            }
        }

        /// <summary>
        /// Gets ID of selected ListView item or null if no item is selected.
        /// </summary>
        private EwsListViewItem GetSelectedItem()
        {
            var indices = itemListView.SelectedIndices;
            if (indices == null || indices.Count != 1)
                return null;

            int index = indices[0];
            return itemListView.Items[index] as EwsListViewItem;
        }

        /// <summary>
        /// Enables and disables connection menu items.
        /// </summary>
        private void SetConnectMenu(bool connected)
        {
            connectToolStripMenuItem.Enabled = !connected;
            disconnectToolStripMenuItem.Enabled = connected;
            if (!connected)
                this.Text = string.Format(TitleFormat, "Disconnected");
        }

        /// <summary>
        /// Performs initial/final UI changes when work begins/ends.
        /// </summary>
        private void SetWorking(bool working)
        {
            Cursor cursor = working ? Cursors.WaitCursor : Cursors.Default;

            this.Cursor = cursor;
            Cursor.Current = cursor;
            folderTree.Cursor = cursor;
            itemListView.Cursor = cursor;
        }

        /// <summary>
        /// Reports an error to the user.
        /// </summary>
        private void ReportError(Exception error)
        {
            string message;

            if (error is ApplicationException)
                message = string.Format(error.Message);
            else if (error is EwsException)
                message = string.Format("Error occurred: {0}", error.Message);
            else
                message = error.ToString();

            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
