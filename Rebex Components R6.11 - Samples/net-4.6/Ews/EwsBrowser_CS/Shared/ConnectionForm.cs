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
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Collections.Generic;
using System.ComponentModel;

namespace Rebex.Samples
{
    /// <summary>
    /// Represents a connection dialog for connecting to an EWS server.
    /// </summary>
    public partial class ConnectionForm : Form
    {
        private readonly ConnectionFormData _data;

        /// <summary>
        /// Gets the data holder object.
        /// </summary>
        public ConnectionFormData Data { get { return _data; } }

        private ConnectionForm() 
            : this(new ConnectionFormData())
        {
            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
                throw new InvalidOperationException("Don't use the parameter-less constructor. It is intended for Visual Studio design mode only.");
        }

        /// <summary>
        /// Initializes new instance of the <see cref="ConnectionForm"/>.
        /// </summary>
        public ConnectionForm(ConnectionFormData data)
        {
            if (data == null)
                throw new ArgumentNullException();

            _data = data;

            InitializeComponent();

            ucConnectionEditor.AdaptToCurrentVisibility();

            SetData();
        }

        /// <summary>
        /// Propagate current form fields values to the <see cref="Data"/> object.
        /// </summary>
        private void GetData()
        {
            ucConnectionEditor.GetData(_data);
        }

        /// <summary>
        /// Propagate current <see cref="Data"/> values to form fields.
        /// </summary>
        private void SetData()
        {
            ucConnectionEditor.SetData(_data);
        }

        /// <summary>
        /// Handles 'Connect' button click event..
        /// </summary>
        private void btnConnect_Click(object sender, EventArgs e)
        {
            List<string> errors = ucConnectionEditor.GetValidationErrors();
            if (errors.Count > 0)
            {
                string msg = string.Join("\r\n", errors.ToArray());
                ConnectionEditorUtils.ShowWarning(msg, "Invalid data");
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// Handles 'Esc' key press event.
        /// </summary>
        private void ConnectionForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            // when 'esc' is pressed the form is closed
            if ((int)(e.KeyChar) == Convert.ToChar(Keys.Escape))
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        /// <summary>
        /// Handles form closing event.
        /// </summary>
        private void ConnectionForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // we gather values from form-fields to the Data object regardless the closing reason
            GetData();
        }
    }
}
