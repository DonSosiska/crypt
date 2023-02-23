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

namespace Rebex.Samples
{
    /// <summary>
    /// Provides data for error handling events.
    /// </summary>
    public class ErrorOccuredEventArgs : EventArgs
    {
        private Exception _error;
        private bool _handled;

        /// <summary>
        /// An error provided by the producer.
        /// </summary>
        public Exception Error
        {
            get { return _error; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the error was handled.
        /// </summary>
        public bool Handled
        {
            get { return _handled; }
            set { _handled = value; }
        }

        /// <summary>
        /// Initializes new instance of the <see cref="ErrorOccuredEventArgs"/>.
        /// </summary>
        public ErrorOccuredEventArgs(Exception error) : this(error, false) { }

        /// <summary>
        /// Initializes new instance of the <see cref="ErrorOccuredEventArgs"/>.
        /// </summary>
        public ErrorOccuredEventArgs(Exception error, bool handled)
        {
            _error = error;
            _handled = handled;
        }
    }
}
