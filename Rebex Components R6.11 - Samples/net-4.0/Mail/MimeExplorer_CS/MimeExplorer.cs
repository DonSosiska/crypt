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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using Rebex.Mime;
using Rebex.Mime.Headers;
using Rebex.Security.Cryptography.Pkcs;

namespace Rebex.Samples
{
    /// <summary>
    /// Shows a tree of MIME entities.
    /// </summary>
    public class MimeExplorer : System.Windows.Forms.Form
    {
        //Known MIME node types.
        private enum NodeType
        {
            Info,
            Leaf,
            Folder,
            FolderSave,
            ApplicationLeaf,
            AudioLeaf,
            ImageLeaf,
            TextLeaf,
            TextHtmlLeaf,
            MultipartDigestFolder,
            MultipartSignedFolder,
            EmbeddedMessage,
            MultipartAlternativeFolder,
            MultipartReportFolder,
        };

        private int _filterIndex;
        private readonly Icon[] _icons;


        private System.Windows.Forms.TreeView viewTree;
        private System.Windows.Forms.MenuStrip menuMain;
        private System.Windows.Forms.ToolStripMenuItem itemMain;
        private System.Windows.Forms.ToolStripMenuItem itemExit;
        private System.Windows.Forms.ToolStripMenuItem itemOpen;
        private System.Windows.Forms.ToolStripMenuItem itemSave;
        private System.Windows.Forms.ToolStripMenuItem itemView;
        private System.Windows.Forms.ToolStripMenuItem itemDecrypt;
        private System.Windows.Forms.ToolStripMenuItem itemVerify;
        private System.Windows.Forms.ContextMenuStrip menuContext;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public MimeExplorer()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            // Initialize the icon list.
            _filterIndex = 0;
            _icons = LoadIcons();

            TreeNode empty = new TreeNode("No file loaded.", (int)NodeType.Info, (int)NodeType.Info);
            viewTree.Nodes.Add(empty);
        }

        private Icon[] LoadIcons()
        {
            //Loads the icon list and initializes the icon list for the viewTree control.

            ImageList il = new ImageList();
            Assembly a = GetType().Assembly;
            string[] resources =
            {
                "Rebex.Samples.resources.info.ico",
                "Rebex.Samples.resources.leaf.ico",
                "Rebex.Samples.resources.folder.ico",
                "Rebex.Samples.resources.foldersave.ico",
                "Rebex.Samples.resources.application.ico",
                "Rebex.Samples.resources.audio.ico",
                "Rebex.Samples.resources.image.ico",
                "Rebex.Samples.resources.text.ico",
                "Rebex.Samples.resources.texthtml.ico",
                "Rebex.Samples.resources.digest.ico",
                "Rebex.Samples.resources.signed.ico",
                "Rebex.Samples.resources.message.ico",
                "Rebex.Samples.resources.alternative.ico",
                "Rebex.Samples.resources.report.ico",
            };

            ArrayList icons = new ArrayList();
            for (int i=0; i<resources.Length; i++)
            {
                Stream stream = a.GetManifestResourceStream(resources[i]);

                Icon icon = new Icon(stream);
                icons.Add(icon);
                il.Images.Add(icon.ToBitmap());
            }

            viewTree.ImageList = il;
            return (Icon[])icons.ToArray(typeof(Icon));
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if (components != null) 
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MimeExplorer));
            this.viewTree = new System.Windows.Forms.TreeView();
            this.menuMain = new System.Windows.Forms.MenuStrip();
            this.itemMain = new System.Windows.Forms.ToolStripMenuItem();
            this.itemOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.itemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuContext = new System.Windows.Forms.ContextMenuStrip();
            this.itemView = new System.Windows.Forms.ToolStripMenuItem();
            this.itemDecrypt = new System.Windows.Forms.ToolStripMenuItem();
            this.itemVerify = new System.Windows.Forms.ToolStripMenuItem();
            this.itemSave = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // viewTree
            // 
            this.viewTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewTree.ImageIndex = -1;
            this.viewTree.Location = new System.Drawing.Point(0, 27);
            this.viewTree.Name = "viewTree";
            this.viewTree.SelectedImageIndex = -1;
            this.viewTree.Size = new System.Drawing.Size(520, 273);
            this.viewTree.TabIndex = 0;
            this.viewTree.MouseDown += new System.Windows.Forms.MouseEventHandler(this.viewTree_MouseDown);
            this.viewTree.MouseUp += new System.Windows.Forms.MouseEventHandler(this.viewTree_MouseUp);
            this.viewTree.DoubleClick += new System.EventHandler(this.viewTree_DoubleClick);
            // 
            // menuMain
            // 
            this.menuMain.Items.AddRange(new System.Windows.Forms.ToolStripMenuItem[] {
                                                                                     this.itemMain});
            // 
            // itemMain
            // 
            this.itemMain.MergeIndex = 0;
            this.itemMain.DropDownItems.AddRange(new System.Windows.Forms.ToolStripMenuItem[] {
                                                                                      this.itemOpen,
                                                                                      this.itemExit});
            this.itemMain.Text = "&File";
            // 
            // itemOpen
            // 
            this.itemOpen.MergeIndex = 0;
            this.itemOpen.Text = "&Open";
            this.itemOpen.Click += new System.EventHandler(this.itemOpen_Click);
            // 
            // itemExit
            // 
            this.itemExit.MergeIndex = 1;
            this.itemExit.Text = "&Exit";
            this.itemExit.Click += new System.EventHandler(this.itemExit_Click);
            // 
            // menuContext
            // 
            this.menuContext.Items.AddRange(new System.Windows.Forms.ToolStripMenuItem[] {
                                                                                       this.itemView,
                                                                                       this.itemDecrypt, 
                                                                                       this.itemVerify,
                                                                                       this.itemSave});
            // 
            // itemView
            // 
            this.itemView.MergeIndex = 0;
            this.itemView.Text = "&View";
            this.itemView.Click += new System.EventHandler(this.itemView_Click);
            // 
            // itemSave
            // 
            this.itemSave.MergeIndex = 1;
            this.itemSave.Text = "&Save...";
            this.itemSave.Click += new System.EventHandler(this.itemSave_Click);
            // 
            // itemDecrypt
            // 
            this.itemDecrypt.MergeIndex = 2;
            this.itemDecrypt.Text = "&Decrypt";
            this.itemDecrypt.Click += new System.EventHandler(this.itemDecrypt_Click);
            // 
            // itemVerify
            // 
            this.itemVerify.MergeIndex = 3;
            this.itemVerify.Text = "&Verify...";
            this.itemVerify.Click += new System.EventHandler(this.itemVerify_Click);
            // 
            // MimeExplorer
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(520, 273);
            this.Controls.Add(this.viewTree);
            this.Controls.Add(this.menuMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuMain;
            this.Name = "MimeExplorer";
            this.Text = "Rebex MIME Explorer";
            this.menuMain.ResumeLayout(false);
            this.menuMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        // Exit command handler.
        private void itemExit_Click(object sender, System.EventArgs e)
        {
            Close();		
        }

        // Open command handler - browse for a file and load it.
        private void itemOpen_Click(object sender, System.EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All files (*.*)|*.*|Mail files (*.eml)|*.eml|MHT files (*.mht)|*.mht";
            ofd.FilterIndex = _filterIndex;
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            _filterIndex = ofd.FilterIndex;

            Stream source = ofd.OpenFile();
            if (source == null)
            {
                return;
            }

            try
            {
                MimeMessage mime = new MimeMessage();
                mime.Load(source);
                Text = Path.GetFileName(ofd.FileName) + " - Rebex MIME Explorer";
                RefillTree(mime);
            }
            catch (Exception x)
            {
                MessageBox.Show(x.ToString(), "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            source.Close();
        }

        // Initialize the tree control.
        private void RefillTree(MimeEntity top)
        {
            viewTree.BeginUpdate();

            try
            {
                viewTree.Nodes.Clear();

                if (top == null)
                {
                    TreeNode mimeFree = new TreeNode("Non-MIME message.", (int)NodeType.Info, (int)NodeType.Info);
                    viewTree.Nodes.Add(mimeFree);
                }
                else
                {
                    TreeNode tn = CreateTree(top);
                    viewTree.Nodes.Add(tn);
                }
            }
            finally
            {
                viewTree.EndUpdate();
            }
        }

        // Build the tree from the MIME message.
        private static TreeNode CreateTree(MimeEntity top)
        {
            ContentType mt = top.ContentType;

            string text;
            ContentLocation cl = top.ContentLocation;
            if (cl != null)
            {
                text = string.Format("{0}: {1}", mt.MediaType, cl.ToString());
            }
            else
            {
                string name = mt.Parameters["name"];
                if (name != null)
                {
                    text = string.Format("{0} (has name \"{1}\")", mt.MediaType, name);
                }
                else
                {
                    string type = null;
                    if (mt.MediaType == "multipart/related")
                    {
                        type = mt.Parameters["type"];
                    }

                    if (type != null)
                    {
                        text = string.Format("{0} (root type is \"{1}\")", mt.MediaType, type);
                    }
                    else
                    {
                        text = string.Format("{0}", mt.MediaType);
                    }
                }
            }

            switch (top.Kind)
            {
                case MimeEntityKind.Enveloped:
                    text = string.Format("{0} (encrypted content)", mt.MediaType);
                    break;
                case MimeEntityKind.Signed:
                    text = string.Format("{0} (signed content)", mt.MediaType);
                    break;
            }

            int image = (int)GetLeafImage(mt);
            TreeNode topNode = new TreeNode(text, image, image);

            topNode.Tag = top;
            for (int i=0; i<top.Parts.Count; i++)
            {
                TreeNode childNode = CreateTree(top.Parts[i]);
                topNode.Nodes.Add(childNode);
            }

            if (top.ContentMessage != null)
            {
                TreeNode childNode = CreateTree(top.ContentMessage);
                topNode.Nodes.Add(childNode);
            }

            return topNode;
        }

        // Determine the icon to use for the specified content type.
        private static NodeType GetLeafImage(ContentType mt)
        {
            string[] parts = mt.MediaType.Split('/');
            if (parts.Length != 2)
                return NodeType.ApplicationLeaf;

            switch (parts[0])
            {
                case "application":
                    if (parts[1] == "pkcs7-mime" || parts[1] == "x-pkcs7-mime")
                        return NodeType.MultipartSignedFolder;
                    return NodeType.ApplicationLeaf;
                case "audio":
                    return NodeType.AudioLeaf;
                case "image":
                    return NodeType.ImageLeaf;
                case "message":
                    return NodeType.EmbeddedMessage;
                case "multipart":
                    return GetMultipartImage(parts[1]);
                case "text":
                    return GetTextLeafType(mt);
                default:
                    return NodeType.Leaf;
            }
        }

        // Determine the icon to use for the specified multipart subtype.
        private static NodeType GetMultipartImage(string subType)
        {
            switch (subType)
            {
                case "alternative":
                    return NodeType.MultipartAlternativeFolder;
                case "digest":
                    return NodeType.MultipartDigestFolder;
                case "related":
                    return NodeType.FolderSave;
                case "report":
                    return NodeType.MultipartReportFolder;
                case "signed":
                    return NodeType.MultipartSignedFolder;
                default:
                    return NodeType.Folder;
            }
        }

        // Determine the icon to use for the specified text subtype.
        private static NodeType GetTextLeafType(ContentType mt)
        {
            if (mt.MediaType == "text/html")
                return NodeType.TextHtmlLeaf;
            else
                return NodeType.TextLeaf;
        }

        // Returns the currently selected entity.
        private MimeEntity GetSelectedEntity()
        {
            TreeNode sel = viewTree.SelectedNode;
            if (sel == null)
                return null;

            return sel.Tag as MimeEntity;
        }

        // View the selected entity using our Viewer dialog.
        private void ViewCommand()
        {
            MimeEntity payload = GetSelectedEntity();
            if (payload == null)
                return;

            if (!payload.IsMultipart && payload.ContentMessage == null)
            {
                Viewer viewer = new Viewer(payload);
                viewer.Icon = _icons[(int)GetLeafImage(payload.ContentType)];
                viewer.ShowDialog();
            }
        }

        // Decrypt the selected entity.
        private bool DecryptCommand()
        {
            MimeEntity payload = GetSelectedEntity();
            if (payload == null)
                return false;

            EnvelopedData data = payload.EnvelopedContentInfo;
            if (data == null || !data.IsEncrypted || !data.HasPrivateKey)
                return false;

            payload.Decrypt();
            TreeNode content = CreateTree(payload.ContentMessage);
            TreeNode current = viewTree.SelectedNode;
            current.Nodes.Add(content);
            current.Expand();
            return true;
        }

        // Verify the selected entity.
        private void VerifyCommand()
        {
            MimeEntity payload = GetSelectedEntity();
            if (payload == null)
                return;

            SignedData data = payload.SignedContentInfo;
            if (data == null)
                return;

            SignatureValidationResult result = payload.ValidateSignature();
            if (result.Valid)
            {
                // Validation was successful, inform user
                MessageBox.Show("Signature is valid.", "Validation result",
                    MessageBoxButtons.OK, MessageBoxIcon.None);
            } 
            else 
            {
                ValidationForm verifyForm = new ValidationForm(result);
                verifyForm.ShowDialog();
            }
        }

        // View or decrypt the selected entity when double-clicked.
        private void viewTree_DoubleClick(object sender, System.EventArgs e)
        {
            if (DecryptCommand())
                return;
            ViewCommand();
        }

        // View the selected entity when view command is selected from the context menu.
        private void itemView_Click(object sender, System.EventArgs e)
        {
            ViewCommand();
        }

        // Decrypts the enveloped content of the selected entity.
        private void itemDecrypt_Click(object sender, System.EventArgs e)
        {
            DecryptCommand();
        }
        // Verify the signed content of the selected entity.
        private void itemVerify_Click(object sender, System.EventArgs e)
        {
            VerifyCommand();
        }

        // Save the selected entity when save command is selected from the context menu.
        private void itemSave_Click(object sender, System.EventArgs e)
        {
            TreeNode sel = viewTree.SelectedNode;
            if (sel == null)
                return;

            MimeEntity entity = sel.Tag as MimeEntity;
            if (entity == null)
                return;

            string fileName = entity.Name;
            if (fileName == null)
            {
                string ext;
                if (entity.IsMultipart || entity.ContentMessage != null)
                {
                    ext = "eml";
                }
                else
                {
                    string[] parts = entity.ContentType.MediaType.Split('/');
                    if (parts.Length == 2)
                        ext = parts[1];
                    else
                        ext = "octet-stream";
                }

                fileName = string.Format("part{0}.{1}", Guid.NewGuid().ToString().Substring(0, 8), ext);
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "All files (*.*)|*.*";
            sfd.FilterIndex = 1;
            sfd.FileName = fileName;
            sfd.RestoreDirectory = true;
 
            if (sfd.ShowDialog(this) == DialogResult.OK)
            {
                using (Stream dest = sfd.OpenFile())
                {
                    if (entity.IsMultipart || entity.ContentMessage != null)
                    {
                        entity.Save(dest);
                    }
                    else
                    {
                        using (Stream source = entity.GetContentStream())
                        {
                            byte[] buffer = new byte[1024];
                            while (true)
                            {
                                int n = source.Read(buffer, 0, buffer.Length);
                                if (n == 0)
                                    break;
                                dest.Write(buffer, 0, n);
                            }
                        }
                    }
                }
            }
        }

        // Handles the MouseUp event of the viewTree control.
        private void viewTree_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                TreeNode sel = viewTree.GetNodeAt(e.X, e.Y);
                if ((sel != null) && (sel == viewTree.SelectedNode))
                {
                    MimeEntity entity = sel.Tag as MimeEntity;
                    if (entity != null)
                    {
                        if (entity.IsMultipart || entity.ContentMessage != null)
                            itemView.Enabled = false;
                        else
                            itemView.Enabled = true;

                        if (entity.Kind == MimeEntityKind.Enveloped &&
                            entity.EnvelopedContentInfo.IsEncrypted &&
                            entity.EnvelopedContentInfo.HasPrivateKey)
                            itemDecrypt.Enabled = true;
                        else
                            itemDecrypt.Enabled = false;

                        if(entity.Kind == MimeEntityKind.Signed)
                            itemVerify.Enabled = true;
                        else
                            itemVerify.Enabled = false;

                        menuContext.Show(viewTree, new Point(e.X, e.Y));
                    }
                }
            }		
        }
        
        // Handles the MouseDown event of the viewTree control.
        private void viewTree_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            TreeNode sel = viewTree.GetNodeAt(e.X, e.Y);
            if (sel != null && viewTree.SelectedNode != sel)
                viewTree.SelectedNode = sel;
        }

    }
}
