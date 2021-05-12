using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Diagnostics;
using DxCK.Utils;

namespace MasterSeeker
{
    public class FlickerFreeListView : ListView
    {
        public FlickerFreeListView()
        {
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }

        internal IntPtr SendMessage(int msg, IntPtr wparam, IntPtr lparam)
        {
            return SendMessage(new HandleRef(this, this.Handle), msg, wparam, lparam);
        }

        private HandleRef _handleRef;

        protected override void OnHandleCreated(EventArgs e)
        {
            _handleRef = new HandleRef(this, this.Handle);
            base.OnHandleCreated(e);
        }

        internal IntPtr SendMessage(int msg, int wparam, int lparam)
        {
            return SendMessage(_handleRef, msg, wparam, lparam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(HandleRef hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, int lParam);
        
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, [In, Out] ref LVITEM lParam);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(HandleRef hwnd, out RECT lpRect);
        
        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        private const int LVM_FIRST = 0x1000;
        private const int LVM_GETHEADER = (LVM_FIRST + 31);


        public Rectangle GetColumnHeaderRectangle()
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                return new Rectangle(0, 0, 0, 0);
            }
            else
            {
                RECT rc = new RECT();
                IntPtr hwnd = SendMessage(Handle, LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);
                if (hwnd != null)
                {
                    if (GetWindowRect(new HandleRef(null, hwnd), out rc))
                    {
                        return new Rectangle(rc.Left, rc.Top, rc.Right - rc.Left, rc.Bottom - rc.Top);
                    }
                    else
                    {
                        throw new Win32Exception();
                    }
                }
                else
                {
                    throw new Win32Exception();
                }
            }
        }

        private int LVM_SETTEXTBKCOLOR = 0x1026;

        #region Draw Performance Handler

        [StructLayout(LayoutKind.Sequential)]
        private struct NMHDR
        {
            public IntPtr hwndFrom;
            public uint idFrom;
            public uint code;
        }

        private const uint NM_CUSTOMDRAW = unchecked((uint)-12);

        //commented out because caused bugs on draw items
        //with font and forecolor changed!
        /*protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x204E)
            {
                NMHDR hdr = (NMHDR)m.GetLParam(typeof(NMHDR));
                if (hdr.code == NM_CUSTOMDRAW)
                {
                    m.Result = (IntPtr)0;
                    return;
                }
            }

            base.WndProc(ref m);
        }*/

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            SendMessage(Handle, LVM_SETTEXTBKCOLOR, IntPtr.Zero, unchecked((IntPtr)(int)0xFFFFFF));
        }

        #endregion

        #region OwnerDraw

        protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }

        protected override void OnDrawSubItem(DrawListViewSubItemEventArgs e)
        {
            //base.OnDrawSubItem(e);
            //e.DrawDefault = true;
        }

        #endregion


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct LVITEM
        {
            public int mask;
            public int iItem;
            public int iSubItem;
            public int state;
            public int stateMask;
            public string pszText;
            public int cchTextMax;
            public int iImage;
            public IntPtr lParam;
            public int iIndent;
            public int iGroupId;
            public int cColumns;
            public IntPtr puColumns;
            //public void Reset();
            //public override string ToString();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (!e.Handled)
            {
                if (e.KeyCode == Keys.A && e.Modifiers == Keys.Control)
                {
                    SelectAll();
                }
            }
        }

        public void SelectAll()
        {
            ListViewUtils.SelectAllItems(this);
        }
    }
}
