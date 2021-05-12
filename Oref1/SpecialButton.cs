using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Forms.VisualStyles;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace WindowsFormsApplication1
{
    public class SpecialButton : ExtendedPanel, IButtonControl
    {
        private bool _focused;
        private bool _mouseOver;
        private bool _pushed;
        private bool _default;

        // designer mode properties
        private bool _takeFocusOnClick = true;

        public SpecialButton()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, false);
            BackColor = Color.Transparent;
            DoubleBuffered = true;
            //TabStop = true;
            ResizeRedraw = true;
        }

        [DefaultValue(true)]
        public bool TakeFocusOnClick
        {
            get { return _takeFocusOnClick; }
            set { _takeFocusOnClick = value; }
        }

        public new bool Focused
        {
            get { return _focused; }
        }

        public bool MouseOver
        {
            get { return _mouseOver; }
        }

        public bool Pushed
        {
            get { return _pushed; }
        }

        public bool Default
        {
            get { return _default; }
        }

        [DefaultValue(false)]
        public bool ShowRectangleWhenNotHot
        {
            get;
            set;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            if (_focused || DesignMode)
            {
                if (_pushed)
                {
                    ControlPaint.DrawBorder(pevent.Graphics, new Rectangle(-1, -1, this.Width + 1, this.Height + 1), SystemColors.Highlight, ButtonBorderStyle.Inset);
                    ControlPaint.DrawBorder(pevent.Graphics, new Rectangle(0, 0, this.Width, this.Height), ThirdBlack(ThirdBlack(SystemColors.Control)), ButtonBorderStyle.Dashed);
                }
                else
                {
                    ControlPaint.DrawBorder(pevent.Graphics, new Rectangle(0, 0, this.Width, this.Height), ThirdBlack(SystemColors.Control), ButtonBorderStyle.Dashed);
                }
            }

            return;

            PushButtonState state;

            if (_mouseOver || _pushed || ShowRectangleWhenNotHot)
            {
                /*if (_pushed)
                {
                    state = PushButtonState.Pressed;
                }
                else if (_mouseOver)
                {
                    state = PushButtonState.Hot;
                }
                else if (_focused || _default)
                {
                    state = PushButtonState.Default;
                }
                else
                {
                    state = PushButtonState.Normal;
                }

                ButtonRenderer.DrawButton(pevent.Graphics, new Rectangle(0, 0, this.Width, this.Height), false, state);

                return;*/

                //ControlPaint.DrawBorder(pevent.Graphics, new Rectangle(0, 0, this.Width, this.Height), SystemColors.ActiveBorder, ButtonBorderStyle.Dashed);

                if (_pushed)
                {
                    //ControlPaint.DrawBorder(pevent.Graphics, new Rectangle(0, 0, this.Width, this.Height), SystemColors.ActiveBorder, ButtonBorderStyle.Inset);
                    //ControlPaint.DrawBorder(pevent.Graphics, new Rectangle(0, 0, this.Width, this.Height), SystemColors.HotTrack, ButtonBorderStyle.Inset);
                    
                    //ControlPaint.DrawBorder(pevent.Graphics, new Rectangle(1, 1, this.Width-2, this.Height-2), Color.White, ButtonBorderStyle.Solid);

                    //commented last worked
                    /*pevent.Graphics.FillRectangle(
                        new LinearGradientBrush(
                            new Point(0, 0),
                            new Point(this.Width, this.Height),
                            SystemColors.ButtonShadow,
                            SystemColors.ButtonHighlight),
                        new Rectangle(0, 0, this.Width, this.Height));

                    ControlPaint.DrawBorder(pevent.Graphics, new Rectangle(-1, -1, this.Width + 1, this.Height + 1), SystemColors.Highlight, ButtonBorderStyle.Inset);
                    ControlPaint.DrawBorder(pevent.Graphics, new Rectangle(0, 0, this.Width, this.Height), ThirdBlack(ThirdBlack(SystemColors.Control)), ButtonBorderStyle.Dashed);*/

                    /*pevent.Graphics.FillRectangle(
                        new LinearGradientBrush(
                            new Point(0, 0),
                            new Point(this.Width - 1, this.Height - 1),
                            GetPixel(pevent.Graphics, 0, 0),
                            GetPixel(pevent.Graphics, this.Width - 1, this.Height - 1)),
                        new Rectangle(0, 0, this.Width, this.Height));*/
                }
                else if (_mouseOver)
                {
                    /*pevent.Graphics.FillRectangle(
                        new SolidBrush(this.BackColor),
                        new Rectangle(0, 0, this.Width, this.Height));*/

                    //commented last worked
                    //ControlPaint.DrawBorder(pevent.Graphics, new Rectangle(0, 0, this.Width, this.Height), ThirdBlack(ThirdBlack(SystemColors.Control)), ButtonBorderStyle.Dashed);
                }
                /*else
                {
                    pevent.Graphics.FillRectangle(
                        new SolidBrush(this.BackColor),
                        new Rectangle(0, 0, this.Width, this.Height));
                }*/
                /*else if (Focused || _default)
                {
                    state = PushButtonState.Default;
                }
                else
                {
                    state = PushButtonState.Normal;
                }*/
            }
            else if (_focused || DesignMode)
            {
                ControlPaint.DrawBorder(pevent.Graphics, new Rectangle(0, 0, this.Width, this.Height), ThirdBlack(SystemColors.Control), ButtonBorderStyle.Dashed);
            }
            /*else
            {
                pevent.Graphics.FillRectangle(
                    new SolidBrush(this.BackColor),
                    new Rectangle(0, 0, this.Width, this.Height));
            }*/
            /*else if (DesignMode)
            {
                ButtonRenderer.DrawButton(pevent.Graphics, new Rectangle(0, 0, this.Width, this.Height), false, PushButtonState.Hot);
            }*/

            /*StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            pevent.Graphics.DrawString(Text, Font, new SolidBrush(ForeColor), new RectangleF(-1, 0, this.Width, this.Height), stringFormat);*/
        }

        public static Color ThirdBlack(Color color)
        {
            return Color.FromArgb(
                    color.A,
                    color.R * 2 / 3,
                    color.G * 2 / 3,
                    color.B * 2 / 3
                );
        }

        private Color HalfBlack(Color color)
        {
            return Color.FromArgb(
                    color.A,
                    color.R  /2,
                    color.G  /2,
                    color.B  /2
                );
        }

        private Color HalfWhite(Color color)
        {
            return Color.FromArgb(
                    color.A,
                    Math.Min(255, (color.R + 255)/2),
                    Math.Min(255, (color.G + 255)/2),
                    Math.Min(255, (color.B + 255)/2)
                );
        }

        /*[DllImport("gdi32.dll")]
        static extern int GetPixel(IntPtr hDC, int x, int y);

        private static Color GetPixel(Graphics graphics, int x, int y)
        {
            Color color = Color.Empty;
            if (graphics != null)
            {
                IntPtr hDC = graphics.GetHdc();
                int colorRef = GetPixel(hDC, x, y);
                color = Color.FromArgb(
                    (int)(colorRef & 0x000000FF),
                    (int)(colorRef & 0x0000FF00) >> 8,
                    (int)(colorRef & 0x00FF0000) >> 16);
                graphics.ReleaseHdc(hDC);
            }
            return color;

        }*/


        protected override void OnMouseEnter(EventArgs e)
        {
            _mouseOver = true;
            this.OnStateChanged();
            this.Invalidate();

            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _mouseOver = false;
            this.OnStateChanged();
            this.Invalidate();

            base.OnMouseLeave(e);
        }

        protected override void OnEnter(EventArgs e)
        {
            _focused = true;
            this.OnStateChanged();
            this.Invalidate();
            base.OnEnter(e);
        }

        protected override void OnLeave(EventArgs e)
        {
            _focused = false;
            this.OnStateChanged();
            this.Invalidate();
            base.OnLeave(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space:

                    _pushed = true;
                    this.OnStateChanged();
                    this.Invalidate();
                    break;

                case Keys.Enter:
                    OnClick(EventArgs.Empty);
                    break;
            }

            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                _pushed = false;
                this.OnStateChanged();
                this.Invalidate();
                OnClick(EventArgs.Empty);
            }

            base.OnKeyUp(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _pushed = true;
                this.OnStateChanged();

                if (_takeFocusOnClick)
                {
                    this.Focus();
                }

                this.Invalidate();
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _pushed = false;
                this.OnStateChanged();
                this.Invalidate();
                OnClick(EventArgs.Empty);
            }

            base.OnMouseUp(e);
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            this.OnClick(e);
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            SetForwardClickEvents(e.Control, false);

            base.OnControlAdded(e);
        }

        #region IButtonControl Members

        public DialogResult DialogResult
        {
            get;
            set;
        }

        public void NotifyDefault(bool value)
        {
            bool needInvalidate = _default != value;

            _default = value;
            this.OnStateChanged();

            if (needInvalidate)
            {
                this.Invalidate();
            }
        }

        public void PerformClick()
        {
            OnClick(EventArgs.Empty);
        }

        #endregion

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            OnStateChanged();
        }

        protected virtual void OnStateChanged()
        {

        }
    }
}
