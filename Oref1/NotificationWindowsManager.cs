using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Oref1
{
    public static class NotificationWindowsManager
    {
        private static NotificationWindow _window;

        public static void ShowNotification(TimeSpan timeout, string tipTitle, string tipText, NotificationType notificationType)
        {
            if (_window == null || _window.IsDisposed)
            {
                _window = new NotificationWindow();
            }

            _window.Set(timeout, tipTitle, tipText, notificationType);
            
            _window.Show();
        }
    }
}
