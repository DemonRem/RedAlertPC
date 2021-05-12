using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DxCK.Utils;
using DxCK.Utils.Text;
using System.Collections;
using System.Diagnostics;
using Microsoft.Win32;

namespace Oref1
{
    public partial class UserSettings : Form
    {
        private UserCityConfig[] _userConfig;
        private ListViewItem[] _listViewItems;

        public UserSettings()
        {
            InitializeComponent();

            this.Icon = Resources.Logo;
        }

        private void AreaSelector_Load(object sender, EventArgs e)
        {
            UserConfigJson userConfig = UserConfiguration.GetConfiguration();

            try
            {
                _checkedChangesFromCode = true;
                checkBox2.Checked = (bool)userConfig.ShowConnectionNotifications;
                checkBox3.Checked = (bool)userConfig.ShowAlertsFromUnknownAreas;
                checkBox1.Checked = IsStartsOnStartup();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            finally
            {
                _checkedChangesFromCode = false;
            }

            Dictionary<string, UserCityConfig> userConfigDictionary = userConfig.CityConfigs.ToDictionary(x => x.City);

            List<UserCityConfig> finalUserConfig = new List<UserCityConfig>();
            List<ListViewItem> finalListViewItems = new List<ListViewItem>();

            for (int i = 0; i < Cities.ListOfCities.Count; i++)
            {
                string city = Cities.ListOfCities[i].City;

                UserCityConfig userCityConfig;

                if (!userConfigDictionary.TryGetValue(city, out userCityConfig))
                {
                    userCityConfig = new UserCityConfig();
                    userCityConfig.City = city;
                }

                ListViewItem item = new ListViewItem(new string[] { userCityConfig.City, ToDisplay(userCityConfig.DisplayAlerts), ToDisplay(userCityConfig.SoundAlerts) });
                item.Tag = userCityConfig;

                finalUserConfig.Add(userCityConfig);
                finalListViewItems.Add(item);
            }

            _userConfig = finalUserConfig.ToArray();
            _listViewItems = finalListViewItems.ToArray();

            RefillListView();
        }

        private void RefillListView()
        {
            IComparer sortingComparer = this.listView1.ListViewItemSorter;

            listView1.BeginUpdate();

            KMP[] filter = textBox1.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(query => new KMP(query))
                .ToArray();
            
            try
            {
                this.listView1.ListViewItemSorter = null;

                listView1.Items.Clear();
                
                foreach (ListViewItem listViewItem in _listViewItems.Where(item => ApplyFilter(filter, (item.Tag as UserCityConfig))))
                {
                    listView1.Items.Add(listViewItem);
                }
            }
            finally
            {
                // Call the sort method to manually sort.
                listView1.Sort();
                // Set the ListViewItemSorter property to a new ListViewItemComparer
                // object.
                this.listView1.ListViewItemSorter = sortingComparer;

                listView1.EndUpdate();
            }
        }

        private bool ApplyFilter(KMP[] filter, UserCityConfig userCityConfig)
        {
            for (int i = 0; i < filter.Length; i++)
            {
                if (!filter[i].ExistsIn(userCityConfig.City))
                {
                    return false;
                }
            }

            return true;
        }

        private string ToDisplay(bool value)
        {
            if (value)
            {
                return "כן";
            }
            else
            {
                return "לא";
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            RefillListView();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                בחרהכלToolStripMenuItem.Visible = true;
                toolStripSeparator1.Visible = false;
                toolStripSeparator2.Visible = false;

                קבלהתרעהעלהמסךToolStripMenuItem.Visible = false;
                בטלהתרעהעלהמסךToolStripMenuItem.Visible = false;

                קבלהתרעהבצלילToolStripMenuItem.Visible = false;
                בטלהתרעהבצלילToolStripMenuItem.Visible = false;
            }
            else
            {
                בחרהכלToolStripMenuItem.Visible = true;
                toolStripSeparator1.Visible = true;
                toolStripSeparator2.Visible = true;

                IEnumerable<UserCityConfig> userCityConfigs = listView1.SelectedItems.Cast<ListViewItem>().Select(listViewItem => (listViewItem.Tag as UserCityConfig));

                קבלהתרעהעלהמסךToolStripMenuItem.Visible = !userCityConfigs.All(userCityConfig => userCityConfig.DisplayAlerts);
                בטלהתרעהעלהמסךToolStripMenuItem.Visible = !userCityConfigs.All(userCityConfig => !userCityConfig.DisplayAlerts);

                קבלהתרעהבצלילToolStripMenuItem.Visible = !userCityConfigs.All(userCityConfig => userCityConfig.SoundAlerts);
                בטלהתרעהבצלילToolStripMenuItem.Visible = !userCityConfigs.All(userCityConfig => !userCityConfig.SoundAlerts);
            }
        }

        private void קבלהתרעהעלהמסךToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem listViewItem in listView1.SelectedItems.Cast<ListViewItem>())
            {
                listViewItem.SubItems[1].Text = ToDisplay(true);
                (listViewItem.Tag as UserCityConfig).DisplayAlerts = true;
            }

            Save();
        }

        private void בטלהתרעהעלהמסךToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem listViewItem in listView1.SelectedItems.Cast<ListViewItem>())
            {
                listViewItem.SubItems[1].Text = ToDisplay(false);
                (listViewItem.Tag as UserCityConfig).DisplayAlerts = false;
            }

            Save();
        }

        private void קבלהתרעהבצלילToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem listViewItem in listView1.SelectedItems.Cast<ListViewItem>())
            {
                listViewItem.SubItems[2].Text = ToDisplay(true);
                (listViewItem.Tag as UserCityConfig).SoundAlerts = true;
            }

            Save();
        }

        private void בטלהתרעהבצלילToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem listViewItem in listView1.SelectedItems.Cast<ListViewItem>())
            {
                listViewItem.SubItems[2].Text = ToDisplay(false);
                (listViewItem.Tag as UserCityConfig).SoundAlerts = false;
            }

            Save();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Save();
            this.Close();
        }

        private void Save()
        {
            UserConfiguration.SaveConfiguration( new UserConfigJson()
            {
                Version = "1.5",
                CityConfigs = _userConfig,
                ShowConnectionNotifications = checkBox2.Checked,
                ShowAlertsFromUnknownAreas = checkBox3.Checked
            });
        }

        private void textBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                listView1.Focus();
            }
        }

        private void בחרהכלToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.SelectAll();
        }

        #region Sorting
        //taken from http://msdn.microsoft.com/en-us/library/ms996467.aspx

        private int _sortColumn = -1;

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine whether the column is the same as the last column clicked.
            if (e.Column != _sortColumn)
            {
                // Set the sort column to the new column.
                _sortColumn = e.Column;
                // Set the sort order to ascending by default.
                listView1.Sorting = SortOrder.Ascending;
            }
            else
            {
                // Determine what the last sort order was and change it.
                if (listView1.Sorting == SortOrder.Ascending)
                    listView1.Sorting = SortOrder.Descending;
                else
                    listView1.Sorting = SortOrder.Ascending;
            }

            // Call the sort method to manually sort.
            listView1.Sort();
            // Set the ListViewItemSorter property to a new ListViewItemComparer
            // object.
            this.listView1.ListViewItemSorter = new ListViewItemComparer(e.Column,
                                                              listView1.Sorting);
        }

        #endregion

        private static UserSettings _instance;

        public static void ShowUserSettings()
        {
            if (_instance == null)
            {
                _instance = new UserSettings();
                _instance.Disposed += new EventHandler(_instance_Disposed);
                _instance.Show();
            }
            else
            {
                if (_instance.WindowState == FormWindowState.Minimized)
                {
                    _instance.WindowState = FormWindowState.Normal;
                }

                _instance.Activate();
            }
        }

        private static void _instance_Disposed(object sender, EventArgs e)
        {
            _instance = null;
        }

        private bool _checkedChangesFromCode;

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (!_checkedChangesFromCode)
            {
                if (checkBox1.Checked && RunningFromTemp)
                {
                    if (MessageBox.Show("תוכנת צבע אדום רצה מתיקיה זמנית, " +
                        "והקבצים שלה עשויים להימחק מאוחר יותר על ידי תהליך חיצוני.\r\n" +
                        "האם אתה בטוח שברצונך להגדיר את תוכנת צבע אדום לעלות עם עליית המחשב מתוך תיקיה זמנית?", "תוכנת צבע אדום",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign) == DialogResult.No)
                    {
                        checkBox1.Checked = false;
                        return;
                    }
                }

                HandleStartOnComputerStartup();
            }
        }

        private void HandleStartOnComputerStartup()
        {
            try
            {
                if (checkBox1.Checked)
                {
                    InstallToStartup();
                }
                else
                {
                    UninstallFromStartup();
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Startup = " + checkBox1.Checked.ToString() + "\r\n" + ex.ToString());
            }
        }

        private void InstallToStartup()
        {
            Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Run", "Oref1", "\"" + Application.ExecutablePath + "\" /startup");
        }

        private void UninstallFromStartup()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

            if (key == null)
            {
                throw new Exception(@"Cannot open key: SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
            }
            else
            {
                key.DeleteValue("Oref1", false);
            }
        }

        private bool IsStartsOnStartup()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

            if (key == null)
            {
                throw new Exception(@"Cannot open key: SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
            }
            else
            {
                return key.GetValue("Oref1") != null;
            }
        }

        private bool RunningFromTemp
        {
            get
            {
                return Application.ExecutablePath.IndexOf(@"\temp\", StringComparison.OrdinalIgnoreCase) >= 0;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (!_checkedChangesFromCode)
            {
                Save();
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (!_checkedChangesFromCode)
            {
                Save();
            }
        }
    }
}
