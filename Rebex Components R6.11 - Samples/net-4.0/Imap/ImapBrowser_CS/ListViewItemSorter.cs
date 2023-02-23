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
using System.Windows.Forms;

namespace Rebex.Samples
{
    /// <summary>
    /// Class for sorting collections of type System.Windows.Forms.ListViewItem.
    /// </summary>
    public class ListViewItemSorter : IComparer
    {
        /// <summary>
        /// Types of value comparison.
        /// </summary>
        public enum CompareTypes
        {
            /// <summary>
            /// Compare values as strings.
            /// </summary>
            Strings,

            /// <summary>
            /// Compare values as integers.
            /// </summary>
            Integers
        }

        /// <summary>
        /// Specifies the column to be sorted.
        /// </summary>
        private int _sortColumn = 0;

        /// <summary>
        /// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
        /// </summary>
        public int SortColumn
        {
            set
            {
                _sortColumn = value;
            }
            get
            {
                return _sortColumn;
            }
        }

        /// <summary>
        /// Specifies the order in which to sort.
        /// </summary>
        private SortOrder _sorting = SortOrder.None;

        /// <summary>
        /// Gets or sets the order of sorting.
        /// </summary>
        public SortOrder Sorting
        {
            set
            {
                _sorting = value;
            }
            get
            {
                return _sorting;
            }
        }

        /// <summary>
        /// Specifies the type of comparison.
        /// </summary>
        private CompareTypes _compareType = CompareTypes.Strings;

        /// <summary>
        /// Gets or sets the type of comparison.
        /// </summary>
        public CompareTypes CompareType
        {
            set
            {
                _compareType = value;
            }
            get
            {
                return _compareType;
            }
        }

        /// <summary>
        /// Case insensitive comparer object.
        /// </summary>
        private CaseInsensitiveComparer _objectCompare = new CaseInsensitiveComparer();

        /// <summary>
        /// This method is inherited from the IComparer interface.  It compares the two objects passed using a case insensitive comparison.
        /// </summary>
        /// <param name="x">First object to be compared</param>
        /// <param name="y">Second object to be compared</param>
        public int Compare(object x, object y)
        {
            // If None sorting is specified, don't compare
            if( _sorting == SortOrder.None )
                return 0;

            int result = 0;
            string stringX = ((ListViewItem) x).SubItems[_sortColumn].Text;
            string stringY = ((ListViewItem) y).SubItems[_sortColumn].Text;

            // Compare two ListViewItem values
            if( CompareType == CompareTypes.Strings)
                result = _objectCompare.Compare(stringX, stringY);
            else
                result = Comparer.Default.Compare(int.Parse(stringX), int.Parse(stringY));

            // If Descending sorting is specified, negate result
            if(_sorting == SortOrder.Descending)
                return -result;

            return result;
        }
    }
}
