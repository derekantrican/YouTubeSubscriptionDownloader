using PocketSharp.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace YouTubeSubscriptionDownloader
{
    public partial class SettingsWindow : Form
    {
        public SettingsWindow()
        {
            InitializeComponent();

            textBoxDownloadDirectory.Text = Settings.Instance.DownloadDirectory;
            comboBoxPreferredQuality.SelectedIndex = comboBoxPreferredQuality.FindStringExact(Settings.Instance.PreferredQuality);
            checkBoxShowNotifications.Checked = Settings.Instance.ShowNotifications;
            comboBoxNotificationClick.SelectedIndex = Settings.Instance.NotificationClickOpensYouTubeVideo ? 0 : 1;
            checkBoxShowThumbnails.Checked = Settings.Instance.ShowThumbnailInNotification;
            checkBoxDownloadVideos.Checked = Settings.Instance.DownloadVideos;
            checkBoxAddPocket.Checked = Settings.Instance.AddToPocket;
            checkBoxSerializeSubscriptions.Checked = Settings.Instance.SerializeSubscriptions;
            numericUpDownIterationFrequency.Value = Settings.Instance.IterationFrequency;
            checkBoxRunIterationsOnStartup.Checked = Settings.Instance.StartIterationsOnStartup;

            checkBoxDownloadVideos_CheckedChanged(null, null);
            checkBoxShowNotifications_CheckedChanged(null, null);
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (checkBoxDownloadVideos.Checked && !Directory.Exists(textBoxDownloadDirectory.Text))
            {
                MessageBox.Show("The directory \"" + textBoxDownloadDirectory.Text + "\" does not exist. Please either create it or choose a different one");
                return;
            }

            //Save settings
            Settings.Instance.DownloadDirectory = textBoxDownloadDirectory.Text;
            Settings.Instance.PreferredQuality = comboBoxPreferredQuality.Text;
            Settings.Instance.ShowNotifications = checkBoxShowNotifications.Checked;
            Settings.Instance.NotificationClickOpensYouTubeVideo = comboBoxNotificationClick.SelectedIndex == 0;
            Settings.Instance.ShowThumbnailInNotification = checkBoxShowThumbnails.Checked;
            Settings.Instance.DownloadVideos = checkBoxDownloadVideos.Checked;
            if (!checkBoxAddPocket.Checked)
                Settings.Instance.AddToPocket = false; //Only set this to true if successfully Authed (down below)

            Settings.Instance.SerializeSubscriptions = checkBoxSerializeSubscriptions.Checked;
            Settings.Instance.IterationFrequency = Convert.ToInt32(numericUpDownIterationFrequency.Value);
            Settings.Instance.StartIterationsOnStartup = checkBoxRunIterationsOnStartup.Checked;

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

        private void checkBoxShowNotifications_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxShowThumbnails.Enabled = checkBoxShowNotifications.Checked;
        }

        private void checkBoxAddPocket_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAddPocket.Checked && !Settings.Instance.AddToPocket) //Authorize Pocket only if this checkbox gets checked and it isn't already authorized
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

            Settings.Instance.PocketAuthCode = user.Code;
            Settings.Instance.AddToPocket = true;
        }
    }
}
