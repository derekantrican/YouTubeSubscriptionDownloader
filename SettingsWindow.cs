using PocketSharp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
            checkBoxAddPocket.Checked = Settings.AddToPocket;
            checkBoxSerializeSubscriptions.Checked = Settings.SerializeSubscriptions;

            checkBoxDownloadVideos_CheckedChanged(null, null);
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (checkBoxDownloadVideos.Checked && !Directory.Exists(textBoxDownloadDirectory.Text))
            {
                MessageBox.Show("The directory \"" + textBoxDownloadDirectory.Text + "\" does not exist. Please either create it or choose a different one");
                return;
            }

            //Save settings
            Settings.DownloadDirectory = textBoxDownloadDirectory.Text;
            Settings.PreferredQuality = comboBoxPreferredQuality.Text;
            Settings.ShowNotifications = checkBoxShowNotifications.Checked;
            Settings.DownloadVideos = checkBoxDownloadVideos.Checked;
            if (!checkBoxAddPocket.Checked)
                Settings.AddToPocket = false; //Only set this to true if successfully Authed (down below)

            Settings.SerializeSubscriptions = checkBoxSerializeSubscriptions.Checked;

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

        private void checkBoxAddPocket_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAddPocket.Checked && !Settings.AddToPocket) //Authorize Pocket only if this checkbox gets checked and it isn't already authorized
                AuthorizePocket();
        }

        private async void AuthorizePocket()
        {
            Settings.pocketClient.CallbackUri = "https://getpocket.com/a/queue/"; //Todo: Need to change this to an automatically closing page
            string requestCode = await Settings.pocketClient.GetRequestCode();
            Uri authenticationUri = Settings.pocketClient.GenerateAuthenticationUri();
            Process.Start(authenticationUri.ToString());

            PocketUser user = null;
            while (true)
            {
                try
                {
                    user = await Settings.pocketClient.GetUser(requestCode);
                    break;
                }
                catch { }
                System.Threading.Thread.Sleep(500);
            }

            Settings.PocketAuthCode = user.Code;
            Settings.AddToPocket = true;
        }
    }
}
