using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using Google.Apis.YouTube.v3.Data;
using System.Threading.Tasks;

namespace YouTubeSubscriptionDownloader
{
    public partial class Main : Form
    {
        List<string> missingLogLines = new List<string>();

        System.Windows.Forms.Timer timer = null;

        CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

        public Main(bool start = false)
        {
            InitializeComponent();
            this.HandleCreated += Form1_HandleCreated;

            if (!Directory.Exists(Common.UserSettings))
                Directory.CreateDirectory(Common.UserSettings);

            Log("Reading settings...");
            Settings.ReadSettings();

            Common.InitializePocket();
            InitializeTimer();

            if (YouTubeFunctions.Service == null)
            {
                Log("Authorizing YouTube...");
                YouTubeFunctions.AuthService();
            }

            if (Settings.Instance.FirstTimeShowSubscriptionManager)
            {
                MessageBox.Show("Welcome to YouTube Subscription Downloader!\n\nSince this is your first time running the program, " +
                                "we'll show you the Subscription Manager where you can set up your tracked subscriptions");

                SubscriptionManager manager = new SubscriptionManager();
                manager.ShowDialog();

                Settings.Instance.FirstTimeShowSubscriptionManager = false;
            }

            if (File.Exists(Common.SubscriptionsPath)) //Don't show the message if there are no subscriptions saved (eg first time startup)
            {
                Log("Getting subscriptions...");
                Common.DeserializeSubscriptions();

                if (!Settings.Instance.CheckForMissedUploads)
                    Common.TrackedSubscriptions.ForEach(p => p.LastVideoPublishDate = DateTime.Now);
            }

            Log("Ready!");

            if (Settings.Instance.StartIterationsOnStartup || start)
                Start(cancelTokenSource.Token);
        }

        private void Form1_HandleCreated(object sender, EventArgs e)
        {
            if (missingLogLines.Count > 0)
            {
                foreach (string line in missingLogLines)
                    richTextBoxLog.Text += line;

                richTextBoxLog.SelectionStart = richTextBoxLog.Text.Length;
                richTextBoxLog.ScrollToCaret();

                missingLogLines.Clear();
            }
        }

        private void InitializeTimer()
        {
            if (timer != null)
                timer.Dispose();

            timer = new System.Windows.Forms.Timer();
            timer.Interval = Settings.Instance.IterationFrequency * 60 * 1000;
            timer.Tick += Timer_Tick;
        }

        private void Log(string itemToLog)
        {
            DateTime date = DateTime.Now;

            if (!this.IsHandleCreated ||
                !this.richTextBoxLog.IsHandleCreated)
            {
                missingLogLines.Add("[" + date + "] " + itemToLog + Environment.NewLine);
                return;
            }

            this.richTextBoxLog.Invoke((MethodInvoker)delegate
            {
                richTextBoxLog.Text += "[" + date + "] " + itemToLog + Environment.NewLine;

                richTextBoxLog.SelectionStart = richTextBoxLog.Text.Length;
                richTextBoxLog.ScrollToCaret();
            });
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            cancelTokenSource.Dispose();
            cancelTokenSource = new CancellationTokenSource();

            Start(cancelTokenSource.Token);
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            cancelTokenSource.Cancel();

            buttonStart.Enabled = true;
            buttonStop.Enabled = false;

            timer.Stop();

            Log("Iterations stopped");
        }

        private void Start(CancellationToken token)
        {
            richTextBoxLog.Clear();
            buttonStart.Enabled = false;
            buttonStop.Enabled = true;
            InitializeTimer();

            if (!Common.HasInternetConnection())
            {
                buttonStop_Click(null, null);
                return;
            }

            if (token.IsCancellationRequested)
                return;

            if (Settings.Instance.CheckForMissedUploads && (Settings.Instance.DownloadVideos || Settings.Instance.AddToPocket)) //Don't run unnecessary iterations if the user doesn't want to download or add them to Pocket
            {
                Log("Looking for recent uploads");
                Task.Run(() => CheckForNewVideoFromSubscriptionsAsync(token, false /*Turn off notifications temporarily because we don't want a bunch of notifications on startup*/)).ContinueWith(t => StartIterations(false));
            }
            else
            {
                StartIterations();
            }
        }

        private void StartIterations(bool runImmediateTick = true)
        {
            Log("Iterations started");
            timer.Start();

            if (runImmediateTick)
            {
                Timer_Tick(null, null); //Run first "tick" now rather than waiting for the timer interval to elapse before first "tick"
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Log("Checking for new uploads...");
            Task.Run(() => CheckForNewVideoFromSubscriptionsAsync(cancelTokenSource.Token));
        }

        private void CheckForNewVideoFromSubscriptionsAsync(CancellationToken token, bool showNotifications = true)
        {
            if (!Common.HasInternetConnection())
            {
                Log("!!NO INTERNET CONNECTION!!");
                return;
            }

            //Check for a "Subscriptions-new.xml" to update the current one with
            string newXml = Common.SubscriptionsPath.Replace(".xml", "-new.xml");
            if (File.Exists(newXml))
            {
                Common.DeserializeSubscriptions(newXml);
                File.Delete(newXml);
            }

            if (Settings.Instance.SyncSubscriptionsWithYouTube)
                YouTubeFunctions.UpdateYTSubscriptions().Wait();

            bool tempNotificationSetting = Settings.Instance.ShowNotifications;
            if (!showNotifications)
                Settings.Instance.ShowNotifications = false;

            foreach (Subscription sub in Common.TrackedSubscriptions)
            {
                if (token.IsCancellationRequested)
                    return;

                List<PlaylistItem> newUploads = YouTubeFunctions.GetMostRecentUploadsAsync(sub).Result;
                foreach (PlaylistItem item in newUploads)
                {
                    PlaylistItemSnippet newUploadDetails = item.Snippet;
                    Log($"New uploaded detected: {sub.Title} ({newUploadDetails.Title})");
                    DoActionsForNewUpload(newUploadDetails, sub, token);
                }

                if (newUploads.Count > 0)
                    sub.LastVideoPublishDate = (DateTime)newUploads.First().Snippet.PublishedAt;
            }

            Settings.Instance.ShowNotifications = tempNotificationSetting;

            Common.SerializeSubscriptions();
        }

        private void DoActionsForNewUpload(PlaylistItemSnippet newUpload, Subscription sub, CancellationToken downloadCancellation)
        {
            if (Settings.Instance.ShowNotifications)
            {
                ShowNotification(newUpload.Title, "New video from " + sub.Title, newUpload.Thumbnails.GetAvailableThumbnailUrl(),
                                 Common.YOUTUBEVIDEOBASEURL + newUpload.ResourceId.VideoId);
            }

            if (Settings.Instance.DownloadVideos)
                DownloadYouTubeVideo(newUpload.ResourceId.VideoId, Settings.Instance.DownloadDirectory, downloadCancellation);

            if (Settings.Instance.AddToPocket)
                AddYouTubeVideoToPocket(newUpload.ResourceId.VideoId);
        }

        private void ShowNotification(string notificationSubTitle, string notificationTitle = "", string imageURL = "", string videoURL = "")
        {
            if (!Settings.Instance.ShowThumbnailInNotification)
                imageURL = null;

            this.Invoke((MethodInvoker)delegate
            {
                Notification notification = new Notification(notificationTitle, notificationSubTitle, imageURL, videoURL);
                if (notification.Screen != null)
                {
                    notification.Show();
                }
            });
        }

        private void DownloadYouTubeVideo(string youTubeVideoId, string destinationFolder, CancellationToken token)
        {
            Log("Downloading video...");
            _ = YouTubeFunctions.DownloadYouTubeVideoAsync(youTubeVideoId, destinationFolder, token);
        }

        private async void AddYouTubeVideoToPocket(string youTubeVideoId)
        {
            Log("Adding video to Pocket...");
            string youTubeURL = Common.YOUTUBEVIDEOBASEURL + youTubeVideoId;
            await Settings.PocketClient.Add(new Uri(youTubeURL));
            Log("Video added to Pocket");
        }

        private void pictureBoxSettings_Click(object sender, EventArgs e)
        {
            if (Form.ModifierKeys == Keys.Control)
                Common.SerializeSubscriptions(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Subscriptions.xml"));
            else
            {
                SettingsWindow settingsWindow = new SettingsWindow();
                settingsWindow.ShowDialog();
            }
        }

        private void pictureBoxPlaylists_Click(object sender, EventArgs e)
        {
            if (timer.Enabled)
            {
                MessageBox.Show("Please stop the program first");
                return;
            }

            SubscriptionManager manager = new SubscriptionManager();
            manager.ShowDialog();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                notifyIconFormInTray.Visible = true;
            }
        }

        private void notifyIconFormInTray_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            notifyIconFormInTray.Visible = false;

            //Move the window if it's not visible on any screen
            bool visible = false;
            foreach (Screen screen in Screen.AllScreens)
            {
                Point formLocation = new Point(this.Left, this.Top);
                if (screen.WorkingArea.Contains(formLocation))
                {
                    visible = true;
                    break;
                }
            }

            if (!visible)
            {
                this.Left = Screen.PrimaryScreen.WorkingArea.Left;
                this.Top = Screen.PrimaryScreen.WorkingArea.Top;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Common.SerializeSubscriptions();
            Settings.SaveSettings();
        }
    }
}
