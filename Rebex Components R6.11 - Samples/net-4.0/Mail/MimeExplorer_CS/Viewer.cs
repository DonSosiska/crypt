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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Text;
using Rebex.Mime;

namespace Rebex.Samples
{
    /// <summary>
    /// Summary description for Viewer.
    /// </summary>
    public class Viewer : System.Windows.Forms.Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        private Image _image = null;
        private Control _control = null;

        public Viewer (MimeEntity entity)
        {
            InitializeComponent();

            //
            // Image entity
            //
            if (entity.ContentType.MediaType.StartsWith ("image/"))
            {
                Stream source = entity.GetContentStream ();
                try
                {
                    _image = new Bitmap (source);
                }
                catch
                {
                    _image = null;
                }
                finally
                {
                    source.Close();
                }

                if (_image != null)
                {
                    AutoScroll = true;
                    int mw = 0;//AutoScrollMargin.Width;
                    int mh = 0;//AutoScrollMargin.Height;
                    ClientSize = new Size (Math.Min (_image.Width+mw, 640), Math.Min (_image.Height+mh, 480));

                    PictureBox box = new PictureBox ();
                    box.SetBounds (0, 0, _image.Width, _image.Height);
                    box.Parent = this;
                    box.Image = _image;
                    return;
                }
            }

            AutoScroll = false;
            TextBox txt = new TextBox ();
            txt.BackColor = Color.White;
            txt.Multiline = true;
            txt.ReadOnly = true;
            txt.ScrollBars = ScrollBars.Both;
            txt.SetBounds (0, 0, ClientSize.Width, ClientSize.Height);
            txt.Font = new Font (FontFamily.GenericMonospace, 10);

            //
            // Text or other entity type
            // 
            txt.Text = FormatMimeEntity(entity);
            
            txt.Select (0,0);
            txt.Parent = this;	
            _control = txt;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if (_image != null)
                _image.Dispose ();

            if( disposing )
            {
                if(components != null)
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
            // 
            // Viewer
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(632, 453);
            this.Name = "Viewer";
            this.Text = "Viewer";
            this.SizeChanged += new System.EventHandler(this.Viewer_SizeChanged);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MinimizeBox = true;
            this.MaximizeBox = false;
            this.ShowInTaskbar = false;


        }
        #endregion

        private void Viewer_SizeChanged(object sender, System.EventArgs e)
        {
            if (_control != null)
                _control.Size = ClientSize;
        }
        
        /// <summary>
        /// Formats content of the mime entity. For text content
        /// types shows text. For all other the hex dump is returned.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private static string FormatMimeEntity (MimeEntity entity)
        {
            if (entity == null)
                return "";
            
            using (Stream source = entity.GetContentStream())
            {
            
                // 
                // Text data - show it all
                //
                if (entity.ContentType.MediaType.StartsWith ("text/"))
                {
                    if (entity.ContentString != null)
                    {
                        return entity.ContentString.Replace ("\n","\r\n");
                    }
                    else
                        return "";				
                }
            
                //
                // All other data types - show data start in hex
                //
                StringBuilder sb = new StringBuilder ((int)source.Length*3);
                int b;
                int n = 0;
                while ((b = source.ReadByte()) >= 0)
                {
                    sb.AppendFormat ("{0:x2}", b);
                    n++;
                    if (n % 8 == 0)
                        sb.Append (" ");

                    if (n == 40000)
                    {
                        sb.Append ("\r\n\r\nAnd more data follows...");
                        break;
                    }
                }
                return sb.ToString ();
            }
        }
    }
}
