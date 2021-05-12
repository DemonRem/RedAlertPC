using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace Oref1
{
    public partial class NotificationWindow : Form
    {
        private bool _timedOut;
        private Size _label3DefaultMaximumSize;

        public NotificationWindow()
        {
            InitializeComponent();
            this.MaximumSize = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);

            _label3DefaultMaximumSize = label3.MaximumSize;
        }

        public void Set(TimeSpan timeout, string tipTitle, string tipText, NotificationType notificationType)
        {
            _opacityTimer.Enabled = false;
            _timedOut = false;
            Opacity = 1;

            if (timeout >= TimeSpan.Zero)
            {
                _closeTimer.Interval = (int)timeout.TotalMilliseconds;
                _closeTimer.Enabled = true;
            }
            else
            {
                _closeTimer.Enabled = false;
            }

            label1.Text = tipTitle;
            label3.Text = tipText;

            switch (notificationType)
            {
                case NotificationType.Critical:
                    BackColor = Color.Red;
                    ForeColor = Color.White;
                    break;
                case NotificationType.Warning:
                    BackColor = Color.Yellow;
                    ForeColor = Color.Black;
                    break;
                case NotificationType.Info:
                    BackColor = Color.Green;
                    ForeColor = Color.Black;
                    break;
            }

            LayoutAndSize();
        }

        private void NotificationWindow_Shown(object sender, EventArgs e)
        {
            LayoutAndSize();
        }

        private void LayoutAndSize()
        {
            label3.MaximumSize = _label3DefaultMaximumSize;

            PerformLayout();

            if (label3.Bottom > this.Height)
            {
                label3.MaximumSize = new Size(Screen.PrimaryScreen.WorkingArea.Width - label3.Left - (label3.Right - label3.Width), 0);
            }

            SetSize();
        }

        private void SetSize()
        {
            this.Left = Screen.PrimaryScreen.WorkingArea.Right - this.Width;
            this.Top = Screen.PrimaryScreen.WorkingArea.Bottom - this.Height;
        }

        private void NotificationWindow_Load(object sender, EventArgs e)
        {

        }

        private void _closeTimer_Tick(object sender, EventArgs e)
        {
            _timedOut = true;
            _closeTimer.Enabled = false;

            if (!_isMouseInside)
            {
                _opacityTimer.Enabled = true;
            }
        }

        private void _opacityTimer_Tick(object sender, EventArgs e)
        {
            Opacity = Math.Max(0, Opacity - 0.01);

            if (Opacity == 0)
            {
                Close();
            }
        }

        private void NotificationWindow_MouseEnter(object sender, EventArgs e)
        {
            MouseInside();
        }

        private void label1_MouseEnter(object sender, EventArgs e)
        {
            MouseInside();
        }

        private void label3_MouseEnter(object sender, EventArgs e)
        {
            MouseInside();
        }

        private bool _isMouseInside;

        private void MouseInside()
        {
            _isMouseInside = true;

            if (_opacityTimer.Enabled)
            {
                _opacityTimer.Enabled = false;
                Opacity = 1;
            }
        }

        private void MouseOutside()
        {
            _isMouseInside = false;

            if (_timedOut)
            {
                _closeTimer.Interval = 1000;
                _closeTimer.Enabled = true;
            }
        }

        private void label1_MouseLeave(object sender, EventArgs e)
        {
            MouseOutside();
        }

        private void label3_MouseLeave(object sender, EventArgs e)
        {
            MouseOutside();
        }

        private void NotificationWindow_MouseLeave(object sender, EventArgs e)
        {
            MouseOutside();
        }

        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        private const int WS_EX_NOACTIVATE = 0x08000000;
        private const int WS_EX_TOOLWINDOW = 0x00000080;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams baseParams = base.CreateParams;

                baseParams.ExStyle |= (int)(
                  WS_EX_NOACTIVATE |
                  WS_EX_TOOLWINDOW);

                return baseParams;
            }
        }

        private void specialButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void specialButton1_MouseEnter(object sender, EventArgs e)
        {
            MouseInside();
        }

        private void specialButton1_MouseLeave(object sender, EventArgs e)
        {
            MouseOutside();
        }
    }
}
