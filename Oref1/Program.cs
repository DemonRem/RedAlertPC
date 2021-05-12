using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Web.Script.Serialization;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Oref1
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            
            //if (!LockInstance())
            //{
            //    return;
            //}

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            //Application.Run(new AreaSelector());
        }

        private static FileStream _instanceLocker;

        private static bool LockInstance()
        {
            string lockFilePath = Path.Combine(Path.GetTempPath(), "~Oref1Lock.tmp");

            try
            {
                _instanceLocker = new FileStream(lockFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 1, FileOptions.DeleteOnClose);
            }
            catch (IOException ex)
            {
                if (GetHResult(ex, 0) == -2147024864)
                {
                    return false;
                }
            }
            catch (Exception)
            {

            }

            return true;
        }

        /// <summary>
        /// Gets the HRESULT of the specified exception.
        /// </summary>
        /// <param name="exception">The exception to test. May not be null.</param>
        /// <param name="defaultValue">The default value in case of an error.</param>
        /// <returns>The HRESULT value.</returns>
        /// <remarks>code from http://stackoverflow.com/questions/50744/wait-until-file-is-unlocked-in-net </remarks>
        public static int GetHResult(IOException exception, int defaultValue)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");

            try
            {
                const string name = "HResult";
                PropertyInfo pi = exception.GetType().GetProperty(name, BindingFlags.NonPublic | BindingFlags.Instance); // CLR2

                if (pi == null)
                {
                    pi = exception.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance); // CLR4
                }

                if (pi != null)
                {
                    return (int)pi.GetValue(exception, null);
                }
            }
            catch
            {
            }
            return defaultValue;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Trace.WriteLine((e.ExceptionObject as Exception).ToString());
        }
    }
}
