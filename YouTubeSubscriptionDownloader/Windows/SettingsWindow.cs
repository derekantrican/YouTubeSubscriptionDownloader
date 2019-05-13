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

            checkBoxDownloadVideos.Checked = Settings.Instance.DownloadVideos;
            textBoxDownloadDirectory.Text = Settings.Instance.DownloadDirectory;
            comboBoxPreferredQuality.SelectedIndex = comboBoxPreferredQuality.FindStringExact(Settings.Instance.PreferredQuality);
            checkBoxShowNotifications.Checked = Settings.Instance.ShowNotifications;
            comboBoxNotificationClick.SelectedIndex = Settings.Instance.NotificationClickOpensYouTubeVideo ? 0 : 1;
            checkBoxShowThumbnails.Checked = Settings.Instance.ShowThumbnailInNotification;
            checkBoxAddPocket.Checked = Settings.Instance.AddToPocket;
            checkBoxCheckForMissedUploads.Checked = Settings.Instance.CheckForMissedUploads;
            checkBoxSyncSubscriptions.Checked = Settings.Instance.SyncSubscriptionsWithYouTube;
            checkBoxRunIterationsOnStartup.Checked = Settings.Instance.StartIterationsOnStartup;
            numericUpDownIterationFrequency.Value = Settings.Instance.IterationFrequency;

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
            Settings.Instance.DownloadVideos = checkBoxDownloadVideos.Checked;
            Settings.Instance.DownloadDirectory = textBoxDownloadDirectory.Text;
            Settings.Instance.PreferredQuality = comboBoxPreferredQuality.Text;
            Settings.Instance.ShowNotifications = checkBoxShowNotifications.Checked;
            Settings.Instance.NotificationClickOpensYouTubeVideo = comboBoxNotificationClick.SelectedIndex == 0;
            Settings.Instance.ShowThumbnailInNotification = checkBoxShowThumbnails.Checked;
            if (!checkBoxAddPocket.Checked)
                Settings.Instance.AddToPocket = false; //Only set this to true if successfully Authed (down below)

            Settings.Instance.CheckForMissedUploads = checkBoxCheckForMissedUploads.Checked;
            if (!checkBoxCheckForMissedUploads.Checked)
                Common.TrackedSubscriptions.ForEach(p => p.LastVideoPublishDate = DateTime.Now);

            Settings.Instance.SyncSubscriptionsWithYouTube = checkBoxSyncSubscriptions.Checked;
            Settings.Instance.StartIterationsOnStartup = checkBoxRunIterationsOnStartup.Checked;
            Settings.Instance.IterationFrequency = Convert.ToInt32(numericUpDownIterationFrequency.Value);

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
            Settings.PocketClient.CallbackUri = "https://getpocket.com/a/queue/"; //Todo: Need to change this to an automatically closing page
            string requestCode = await Settings.PocketClient.GetRequestCode();
            Uri authenticationUri = Settings.PocketClient.GenerateAuthenticationUri();
            Process.Start(authenticationUri.ToString());

            PocketUser user = null;
            while (true)
            {
                try
                {
                    user = await Settings.PocketClient.GetUser(requestCode);
                    break;
                }
                catch { }
                System.Threading.Thread.Sleep(500);
            }

            Settings.Instance.PocketAuthCode = user.Code;
            Settings.Instance.AddToPocket = true;
        }

        private void PictureBoxGoogleWallet_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Thank you for choosing Google Pay! Please put \"derekantrican@gmail.com\" as the recipient"); //This is temporary - we should get a better URL like the PayPal one below (or a different method)
            Process.Start("https://pay.google.com/payments/u/0/home#sendRequestMoney");
        }

        private void PictureBoxPayPal_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=derekantrican@gmail.com&lc=US&item_name=YouTubeSubscriptionDownloader&currency_code=USD&bn=PP%2dDonationsBF");

        }
    }
}
