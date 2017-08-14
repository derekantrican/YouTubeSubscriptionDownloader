using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YouTubeSubscriptionDownloader
{
    public partial class SettingsWindow : Form
    {
        public SettingsWindow()
        {
            InitializeComponent();

            textBoxDownloadDirectory.Text = Settings.DownloadDirectory;
            comboBoxPreferredQuality.SelectedIndex = comboBoxPreferredQuality.FindStringExact(Settings.PreferredQuality);
            checkBoxShowNotifications.Checked = Settings.ShowNotifications;
            checkBoxDownloadVideos.Checked = Settings.DownloadVideos;

            checkBoxDownloadVideos_CheckedChanged(null, null);
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            //Save settings
            Settings.DownloadDirectory = textBoxDownloadDirectory.Text; //Todo: verify download directory is an existing directory
            Settings.PreferredQuality = comboBoxPreferredQuality.Text;
            Settings.ShowNotifications = checkBoxShowNotifications.Checked;
            Settings.DownloadVideos = checkBoxDownloadVideos.Checked;
            Settings.SaveSettings();

            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonFolderPicker_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.ShowDialog();

            if (!string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
                textBoxDownloadDirectory.Text = folderBrowserDialog.SelectedPath;
        }

        private void checkBoxDownloadVideos_CheckedChanged(object sender, EventArgs e)
        {
            textBoxDownloadDirectory.Enabled = checkBoxDownloadVideos.Checked;
            buttonFolderPicker.Enabled = checkBoxDownloadVideos.Checked;
            comboBoxPreferredQuality.Enabled = checkBoxDownloadVideos.Checked;
        }
    }
}
