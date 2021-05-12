using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.ComponentModel;

namespace WindowsFormsApplication1
{
    
    [ProvideProperty("ForwardMouseMoveEvents", typeof(Control))]
    [ProvideProperty("ForwardMouseClickEvents", typeof(Control))]
    [ProvideProperty("ForwardClickEvents", typeof(Control))]
    public class ExtendedPanel : Panel, IExtenderProvider
    {
        private Dictionary<Control, bool> _forwardMouseMoveEventsDic =
            new Dictionary<Control, bool>();

        private Dictionary<Control, bool> _forwardMouseClickEventsDic =
            new Dictionary<Control, bool>();

        private Dictionary<Control, bool> _forwardClickEventsDic =
            new Dictionary<Control, bool>();

        public ExtendedPanel()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.Selectable, true);
            //TabStop = true;
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);

            if (GetForwardMouseMoveEvents(e.Control))
            {
                e.Control.MouseEnter += new EventHandler(Control_MouseEnter);
                e.Control.MouseLeave += new EventHandler(Control_MouseLeave);
                e.Control.MouseMove += new MouseEventHandler(Control_MouseMove);
                e.Control.MouseHover += new EventHandler(Control_MouseHover);
            }

            if (GetForwardClickEvents(e.Control))
            {
                e.Control.Click += new EventHandler(Control_Click);
                e.Control.DoubleClick += new EventHandler(Control_DoubleClick);
            }

            if (GetForwardMouseClickEvents(e.Control))
            {
                e.Control.MouseClick += new MouseEventHandler(control_MouseClick);
                e.Control.MouseDoubleClick += new MouseEventHandler(control_MouseDoubleClick);
                e.Control.MouseDown += new MouseEventHandler(control_MouseDown);
                e.Control.MouseUp += new MouseEventHandler(control_MouseUp);
            }
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);

            if (GetForwardMouseMoveEvents(e.Control))
            {
                e.Control.MouseEnter -= new EventHandler(Control_MouseEnter);
                e.Control.MouseLeave -= new EventHandler(Control_MouseLeave);
                e.Control.MouseMove -= new MouseEventHandler(Control_MouseMove);
                e.Control.MouseHover -= new EventHandler(Control_MouseHover);
            }

            if (GetForwardClickEvents(e.Control))
            {
                e.Control.Click -= new EventHandler(Control_Click);
                e.Control.DoubleClick -= new EventHandler(Control_DoubleClick);
            }

            if (GetForwardMouseClickEvents(e.Control))
            {
                e.Control.MouseClick -= new MouseEventHandler(control_MouseClick);
                e.Control.MouseDoubleClick -= new MouseEventHandler(control_MouseDoubleClick);
                e.Control.MouseDown -= new MouseEventHandler(control_MouseDown);
                e.Control.MouseUp -= new MouseEventHandler(control_MouseUp);
            }
        }

        #region IExtenderProvider Members

        public bool CanExtend(object extendee)
        {
            Control control = extendee as Control;

            if (control == null)
            {
                return false;
            }
            else
            {
                return ControlExists(this.Controls, control);
            }
        }

        private bool ControlExists(ControlCollection controlCollection, Control targetControl)
        {
            foreach (Control control in controlCollection)
            {
                if (control == targetControl)
                {
                    return true;
                }
                else
                {
                    if (ControlExists(control.Controls, targetControl))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void SetForwardMouseMoveEvents(Control control, bool value)
        {
            bool oldValue = GetForwardMouseMoveEvents(control);
            _forwardMouseMoveEventsDic[control] = value;

            if (oldValue != value)
            {
                if (value)
                {
                    control.MouseEnter += new EventHandler(Control_MouseEnter);
                    control.MouseLeave += new EventHandler(Control_MouseLeave);
                    control.MouseMove += new MouseEventHandler(Control_MouseMove);
                    control.MouseHover += new EventHandler(Control_MouseHover);
                }
                else
                {
                    control.MouseEnter -= new EventHandler(Control_MouseEnter);
                    control.MouseLeave -= new EventHandler(Control_MouseLeave);
                    control.MouseMove -= new MouseEventHandler(Control_MouseMove);
                    control.MouseHover -= new EventHandler(Control_MouseHover);
                }
            }
        }

        [DefaultValue(true)]
        public bool GetForwardMouseMoveEvents(Control control)
        {
            bool value;

            if (!_forwardMouseMoveEventsDic.TryGetValue(control, out value))
            {
                value = true;
            }

            return value;
        }

        public void SetForwardMouseClickEvents(Control control, bool value)
        {
            bool oldValue = GetForwardMouseClickEvents(control);
            _forwardMouseClickEventsDic[control] = value;

            if (oldValue != value)
            {
                if (value)
                {
                    control.MouseClick += new MouseEventHandler(control_MouseClick);
                    control.MouseDoubleClick += new MouseEventHandler(control_MouseDoubleClick);
                    control.MouseDown += new MouseEventHandler(control_MouseDown);
                    control.MouseUp += new MouseEventHandler(control_MouseUp);
                }
                else
                {
                    control.MouseClick -= new MouseEventHandler(control_MouseClick);
                    control.MouseDoubleClick -= new MouseEventHandler(control_MouseDoubleClick);
                    control.MouseDown -= new MouseEventHandler(control_MouseDown);
                    control.MouseUp -= new MouseEventHandler(control_MouseUp);
                }
            }
        }

        [DefaultValue(true)]
        public bool GetForwardMouseClickEvents(Control control)
        {
            bool value;

            if (!_forwardMouseClickEventsDic.TryGetValue(control, out value))
            {
                value = true;
            }

            return value;
        }

        public void SetForwardClickEvents(Control control, bool value)
        {
            bool oldValue = GetForwardClickEvents(control);
            _forwardClickEventsDic[control] = value;

            if (oldValue != value)
            {
                if (value)
                {
                    control.Click += new EventHandler(Control_Click);
                    control.DoubleClick += new EventHandler(Control_DoubleClick);
                }
                else
                {
                    control.Click -= new EventHandler(Control_Click);
                    control.DoubleClick -= new EventHandler(Control_DoubleClick);
                }
            }
        }

        [DefaultValue(true)]
        public bool GetForwardClickEvents(Control control)
        {
            bool value;

            if (!_forwardClickEventsDic.TryGetValue(control, out value))
            {
                value = true;
            }

            return value;
        }

        #endregion

        #region event forwards

        private void Control_Click(object sender, EventArgs e)
        {
            OnClick(e);
        }

        private void Control_MouseLeave(object sender, EventArgs e)
        {
            OnMouseLeave(e);
        }

        private void Control_MouseEnter(object sender, EventArgs e)
        {
            OnMouseEnter(e);
        }

        private void Control_MouseHover(object sender, EventArgs e)
        {
            OnMouseHover(e);
        }

        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            OnMouseMove(e);
        }

        void control_MouseUp(object sender, MouseEventArgs e)
        {
            OnMouseUp(e);
        }

        void control_MouseDown(object sender, MouseEventArgs e)
        {
            OnMouseDown(e);
        }

        void control_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            OnMouseDoubleClick(e);
        }

        void control_MouseClick(object sender, MouseEventArgs e)
        {
            OnMouseClick(e);
        }

        void Control_DoubleClick(object sender, EventArgs e)
        {
            OnDoubleClick(e);
        }

        #endregion
    }
}
