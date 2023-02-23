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
    /// Strongly typed tree view, which represents hierarchy of EWS folders.
    /// </summary>
    public class EwsFolderTree : UcTreeView
    {
        // EWS client object
        private Ews _ews;

        /// <summary>
        /// Gets the selected folder.
        /// </summary>
        public FolderInfo SelectedFolder { get { return (FolderInfo)SelectedNode.Info; } }

        /// <summary>
        /// Binds specified EWS client object to the <see cref="EwsFolderTree"/> and defines root folder.
        /// </summary>
        public Task BindAsync(Ews ews, EwsFolderId root)
        {
            _ews = ews;

            return base.BindAsync(root, "Inbox");
        }

        /// <summary>
        /// Retrieves subfolders of the specified folder.
        /// </summary>
        protected async override Task<IEnumerable<INodeInfo>> GetItemsAsync(object id)
        {
            // wait for other operations to finish
            while (_ews.IsBusy)
                await Task.Delay(100);

            var list = new List<FolderInfo>();
            // retrieve subfolders
            var folders = await _ews.GetFolderListAsync((EwsFolderId)id);
            foreach (var f in folders)
            {
                list.Add(new FolderInfo(f));
            }
            return list;
        }

        /// <summary>
        /// Represents an EWS folder.
        /// </summary>
        public class FolderInfo : NodeInfo<EwsFolderId>
        {
            /// <summary>
            /// Gets the total items count within the info.
            /// </summary>
            public new int ItemsTotal { get { return base.ItemsTotal.Value; } }

            /// <summary>
            /// Initializes new instance of the <see cref="FolderInfo"/>.
            /// </summary>
            public FolderInfo(EwsFolderInfo info)
                : base(info.Id, info.Name, info.ItemsTotal, info.ChildFolderCount > 0) { }
        }
    }
}
