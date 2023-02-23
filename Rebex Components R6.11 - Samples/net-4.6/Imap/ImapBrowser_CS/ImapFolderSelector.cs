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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using Rebex.Net;

namespace Rebex.Samples
{
    /// <summary>
    /// Control which represents IMAP folder tree.
    /// </summary>
    public class ImapFolderSelector : System.Windows.Forms.UserControl
    {
        private System.Windows.Forms.TreeView treeView;

        private Imap _imap;
        private TreeNode _lastSelected;
        private bool _initialized;

        public bool IsInitialized
        {
            get{ return _initialized; }
        }

        public string SelectedFolder
        {
            get
            {
                TreeNode node = treeView.SelectedNode;
                if (node == null)
                    return null;

                object[] tag = (object[])node.Tag;
                ImapFolder folder = (ImapFolder)tag[0];

                if (!folder.IsSelectable)
                    return null;

                return folder.Name;
            }
        }

        public event EventHandler SelectedFolderChanged;

        /// <summary>
        /// Sets the internal IMAP object.
        /// </summary>
        public void SetClient(Imap imap)
        {
            _imap = imap;
            if (_initialized || imap == null)
                return;

            GetFolderList(treeView.Nodes, "", null);
            
            _initialized = true;

            foreach (TreeNode node in treeView.Nodes)
            {
                if (string.Compare(node.Text, "Inbox", true) == 0)
                {
                    treeView.SelectedNode = node;
                    break;
                }
            }			
        }

        private void GetFolderList(TreeNodeCollection nodes, string name, string delimiter)
        {
            Cursor.Current = Cursors.WaitCursor;
            ImapFolderCollection folders = _imap.GetFolderList(name, ImapFolderListMode.All, false);
            Cursor.Current = Cursors.Default;
            foreach (ImapFolder folder in folders)
            {
                string folderName = folder.Name;
                if (delimiter != null)
                {
                    int p = folderName.LastIndexOf(delimiter);
                    if (p >= 0)
                        folderName = folderName.Substring(p + delimiter.Length);
                }

                TreeNode node = new TreeNode(folderName);
                node.Tag = new object[]{folder, folder.CanContainInferiors};
                nodes.Add(node);
            }
        }

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public ImapFolderSelector()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.treeView = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // treeView
            // 
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.ImageIndex = -1;
            this.treeView.Location = new System.Drawing.Point(0, 0);
            this.treeView.Name = "treeView";
            this.treeView.SelectedImageIndex = -1;
            this.treeView.Size = new System.Drawing.Size(150, 150);
            this.treeView.TabIndex = 0;
            this.treeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeView_MouseDown);
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            // 
            // ImapFolderSelector
            // 
            this.Controls.Add(this.treeView);
            this.Name = "ImapFolderSelector";
            this.ResumeLayout(false);

        }
        #endregion

        private void treeView_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (!Enabled)
                return;

            TreeNode sel = treeView.GetNodeAt(e.X, e.Y);
            if (sel != null && treeView.SelectedNode != sel)
                treeView.SelectedNode = sel;
        }

        public void RefreshFolder()
        {
            TreeNode node = treeView.SelectedNode;
            object[] tag = (object[])node.Tag;
            ImapFolder folder = (ImapFolder)tag[0];
            bool expandable = (bool)tag[1];
            if (expandable && _imap != null)
            {
                GetFolderList(node.Nodes, folder.Name, folder.Delimiter);
                node.Tag = new object[] {folder, false};
            }
        }

        private void treeView_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
            if (!Enabled)
            {
                treeView.SelectedNode = _lastSelected;
                return;
            }

            TreeNode node = treeView.SelectedNode;
            if (node == _lastSelected)
                return;

            _lastSelected = node;
            if (node == null)
                return;

            if (SelectedFolderChanged != null)
                SelectedFolderChanged(this, EventArgs.Empty);
        }
    }
}
