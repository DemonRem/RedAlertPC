using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Oref1;

namespace MasterSeeker
{
    public static class ListViewUtils
    {
        /// <summary>
        /// Select all rows on the given listview
        /// </summary>
        /// <param name="list">The listview whose items are to be selected</param>
        public static void SelectAllItems(ListView list)
        {
            SetItemState(list, -1, 2, 2);
        }

        /// <summary>
        /// Deselect all rows on the given listview
        /// </summary>
        /// <param name="list">The listview whose items are to be deselected</param>
        public static void DeselectAllItems(ListView list)
        {
            SetItemState(list, -1, 2, 0);
        }

        /// <summary>
        /// Set the item state on the given item
        /// </summary>
        /// <param name="list">The listview whose item's state is to be changed</param>
        /// <param name="itemIndex">The index of the item to be changed</param>
        /// <param name="mask">Which bits of the value are to be set?</param>
        /// <param name="value">The value to be set</param>
        private static void SetItemState(ListView list, int itemIndex, int mask, int value)
        {
            PInvokeWin32.LVITEM lvItem = new PInvokeWin32.LVITEM();
            lvItem.stateMask = mask;
            lvItem.state = value;
            PInvokeWin32.SendMessageLVItem(list.Handle, PInvokeWin32.LVM_SETITEMSTATE, itemIndex, ref lvItem);
        }
    }
}
