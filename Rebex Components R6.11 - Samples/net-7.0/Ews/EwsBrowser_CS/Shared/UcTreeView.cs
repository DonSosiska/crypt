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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Rebex.Net;

namespace Rebex.Samples
{
    /// <summary>
    /// Represents hierarchical tree view.
    /// </summary>
    public partial class UcTreeView : UserControl
    {
        /// <summary>
        /// Retrieves child nodes of the node identified by the specified id.
        /// </summary>
        /// <remarks>
        /// Due to Designer issues method has to be declared as virtual instead of abstract.
        /// </remarks>
        protected virtual Task<IEnumerable<INodeInfo>> GetItemsAsync(object id) { throw new NotImplementedException(); }

        /// <summary>
        /// Returns selected node.
        /// </summary>
        protected Node SelectedNode { get; private set; }

        /// <summary>
        /// Occurs when selection is changed.
        /// </summary>
        public event EventHandler SelectionChanged;

        /// <summary>
        /// Occurs when an exception is raised.
        /// </summary>
        public event EventHandler<ErrorOccuredEventArgs> ErrorOccured;

        /// <summary>
        /// Initializes new instance of the <see cref="UcTreeView"/>.
        /// </summary>
        /// <remarks>
        /// This class is not intended to be used publicly.
        /// Only descendants can call the constructor.
        /// </remarks>
        protected UcTreeView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Clears the content of the <see cref="UcTreeView"/>. 
        /// </summary>
        public virtual void Clear()
        {
            try
            {
                treeView.SuspendLayout();
                treeView.Nodes.Clear();
                treeView.ResumeLayout();
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        /// <summary>
        /// Binds the root object to the <see cref="UcTreeView"/> and starts retrieval of its child nodes.
        /// Also selects the initial node if found.
        /// </summary>
        protected async Task BindAsync(object rootId, string initialName)
        {
            try
            {
                await LoadNodesAsync(rootId, treeView.Nodes);

                if (initialName != null)
                {
                    foreach (Node node in treeView.Nodes)
                    {
                        if (node.Text.Equals(initialName, StringComparison.OrdinalIgnoreCase))
                        {
                            treeView.SelectedNode = node;
                            return;
                        }
                    }
                }

                if (treeView.Nodes.Count > 0)
                    treeView.SelectedNode = treeView.Nodes[0];
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        /// <summary>
        /// Retrieves child nodes and adds them to the <see cref="UcTreeView"/>.
        /// </summary>
        private async Task LoadNodesAsync(object id, TreeNodeCollection nodes)
        {
            try
            {
                IEnumerable<INodeInfo> list = await GetItemsAsync(id);

                treeView.SuspendLayout();
                nodes.Clear();
                foreach (INodeInfo info in list)
                {
                    nodes.Add(new Node(info));
                }
                treeView.ResumeLayout();
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        /// <summary>
        /// Handles node expansion event.
        /// </summary>
        private async void treeView_AfterExpand(object sender, TreeViewEventArgs e)
        {
            var node = (Node)e.Node;
            if (!node.IsLoaded)
            {
                try
                {
                    await LoadNodesAsync(node.Info.Id, node.Nodes);
                    node.IsLoaded = true;
                }
                catch (Exception ex)
                {
                    HandleError(ex);
                }
            }
        }

        /// <summary>
        /// Handles node selection event.
        /// </summary>
        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var node = treeView.SelectedNode as Node;
            if (node == null)
                return;

            if (SelectedNode == node)
                return;

            SelectedNode = node;

            var h = SelectionChanged;
            if (h != null)
                h(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handles errors by raising the <see cref="ErrorOccured"/> event.
        /// Throws exception if not handled by event.
        /// </summary>
        private void HandleError(Exception ex)
        {
            EventHandler<ErrorOccuredEventArgs> handler = ErrorOccured;
            if (handler != null)
            {
                var args = new ErrorOccuredEventArgs(ex);

                handler(this, args);

                if (args.Handled)
                    return;
            }

            throw new Exception("An unhandled exception occurred. --> " + ex.Message, ex);
        }

        /// <summary>
        /// A node of the the <see cref="UcTreeView"/>.
        /// </summary>
        protected class Node : TreeNode
        {
            public INodeInfo Info { get; private set; }
            public bool IsLoaded { get; set; }

            public Node(INodeInfo info)
                : base(info.Name)
            {
                Info = info;
                Info.Owner = this;

                if (info.HasChildNodes == false)
                    IsLoaded = true;
                else
                    Nodes.Add(new TreeNode("Loading..."));
            }
        }

        /// <summary>
        /// Defines interface for <see cref="UcTreeView"/>'s items.
        /// </summary>
        protected interface INodeInfo
        {
            object Id { get; }
            string Name { get; }
            int? ItemsTotal { get; }
            bool? HasChildNodes { get; }
            Node Owner { get; set; }
        }

        /// <summary>
        /// Represents an <see cref="UcTreeView"/>'s item.
        /// </summary>
        public class NodeInfo<TId> : INodeInfo
        {
            private Node _owner;
            private bool? _hasChildNodes;

            /// <summary>
            /// Gets the ID of the info.
            /// </summary>
            public TId Id { get; private set; }
            /// <summary>
            /// Gets the Name of the info.
            /// </summary>
            public string Name { get; private set; }
            /// <summary>
            /// Gets the total items count within the info.
            /// </summary>
            public int? ItemsTotal { get; private set; }
            /// <summary>
            /// Gets the Path of the info.
            /// </summary>
            public string Path { get { return _owner == null ? Name : _owner.FullPath; } }

            /// <summary>
            /// Initializes new instance of the <see cref="NodeInfo{TId}"/>.
            /// </summary>
            public NodeInfo(TId id, string name, int? itemsTotal, bool? hasChildNodes)
            {
                Id = id;
                Name = name;
                ItemsTotal = itemsTotal;
                _hasChildNodes = hasChildNodes;
            }

            object INodeInfo.Id { get { return this.Id; } }
            bool? INodeInfo.HasChildNodes { get { return _hasChildNodes; } }
            Node INodeInfo.Owner { get { return _owner; } set { _owner = value; } }
        }
    }
}
