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
using System.Windows.Forms;

namespace Rebex.Samples
{
    /// <summary>
    /// Summary description for MailEditorMessageView.
    /// </summary>
    public class MailTextEditor : System.Windows.Forms.Control  
    {
        private enum ViewType
        {
            Text,
            Html,
        }

        // Control for message body.
        private TextBox _view = null;

        // Indicates whether the message was modified.
        private bool _modified = false;

        private ViewType _currentView = ViewType.Text;

        public bool Modified
        {
            get
            {
                return _modified;
            }
            set
            {
                _modified = value;
            }
        }

        public bool ReadOnly
        {
            set
            {
                _view.Enabled = !value;
            }
        }


        /// <summary>
        /// Constructor.
        /// </summary>
        public MailTextEditor() 
        {
            _view = new TextBox();
            _view.Multiline = true;
            _view.TextChanged += new EventHandler(_view_TextChanged);
            _view.ScrollBars = ScrollBars.Both;
            _view.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            this.Controls.Add(_view);
        }

        /// <summary>
        /// Size of control.
        /// </summary>
        public new System.Drawing.Size Size
        {
            get
            {
                return base.Size;
            }
            set
            {
                base.Size = value;
                _view.Size = value;
            }
        }

        /// <summary>
        /// Cut to clipboard.
        /// </summary>
        public void Cut()
        {
            switch(_currentView)
            {
                case ViewType.Text:
                    _view.Cut();
                    break;
                case ViewType.Html:
                    break;
            }
        }

        /// <summary>
        /// Copy to clipboard.
        /// </summary>
        public void Copy()
        {
            switch (_currentView)
            {
                case ViewType.Text:
                    _view.Copy();
                    break;
                case ViewType.Html:
                    break;
            }
        }

        /// <summary>
        /// Paste from clipboard.
        /// </summary>
        public void Paste()
        {
            switch (_currentView)
            {
                case ViewType.Text:
                    _view.Paste();
                    break;
                case ViewType.Html:
                    break;
            }
        }

        /// <summary>
        /// Select all data.
        /// </summary>
        public void SelectAll()
        {
            switch (_currentView)
            {
                case ViewType.Text:
                    _view.SelectAll();
                    break;
                case ViewType.Html:
                    break;
            }
        }

        /// <summary>
        /// Add text to control.
        /// </summary>
        /// <param name="data"></param>
        public void AppendText(string data)
        {
            switch (_currentView)
            {
                case ViewType.Text:
                    _view.AppendText(data);
                    break;
                case ViewType.Html:
                    break;
            }
        }

        /// <summary>
        /// View text in control.
        /// </summary>
        public override string Text
        {
            get
            {
                return _view.Text.Replace("\r\n","\n");
            }
            set
            {
                _view.Text = value.Replace("\n","\r\n");
            }
        }

        private void _view_TextChanged(object sender, EventArgs e)
        {
            _modified = true;
        }
    }
}
