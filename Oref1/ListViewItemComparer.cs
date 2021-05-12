using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using DxCK.Utils;

namespace Oref1
{
    //taken from http://msdn.microsoft.com/en-us/library/ms996467.aspx
    public class ListViewItemComparer : IComparer
    {
        private int _col;
        private SortOrder _order;

        public ListViewItemComparer()
        {
            _col = 0;
            _order = SortOrder.Ascending;
        }

        public ListViewItemComparer(int column, SortOrder order)
        {
            _col = column;
            _order = order;
        }

        public int Compare(object x, object y)
        {
            int returnVal = -1;

            returnVal = FastStringUtils.CompareFast(((ListViewItem)x).SubItems[_col].Text,
                                    ((ListViewItem)y).SubItems[_col].Text);

            // Determine whether the sort order is descending.
            if (_order == SortOrder.Descending)
            {
                // Invert the value returned by String.Compare.
                returnVal = -returnVal;
            }

            return returnVal;
        }
    }
}
