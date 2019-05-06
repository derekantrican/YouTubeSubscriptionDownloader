using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using Google.Apis.YouTube.v3.Data;

namespace YouTubeSubscriptionDownloader
{
    public partial class Main : Form
    {
        List<string> missingLogLines = new List<string>();

        System.Windows.Forms.Timer timer = null;

        CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

        /*====================================================
         * TODO:
         * - currently there is no "Get subscriptions from YouTube" functionality. Need to add this to Subscription Manager
         * 
         * - add a setting for something like "Automatically get latest subscriptions from YouTube" that will automatically get latest subscriptions (if subscribed to a new channel on YouTube).
         *      This would also control if a subscription gets removed from this program when unsubscribed on YouTube
         *  
         * - show Subscription Manager on first time run?
         * 
         * - need to handle editing in Subscription Manager
         * 
         * - improve speed later (make lots of things async)
         * ====================================================
         */

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

            if (File.Exists(Common.SubscriptionsPath)) //Don't show the message if there are no subscriptions saved (eg first time startup)
            {
                Log("Getting subscriptions...");
                Common.DeserializeSubscriptions();
            }

            buttonStop.Enabled = false;

            Log("Ready!");

            //TEMP
            //if (Settings.Instance.StartIterationsOnStartup || start)
            //    Start(cancelTokenSource.Token);
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

        private void Start(CancellationToken token)
        {
            if (this.IsHandleCreated)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    richTextBoxLog.Clear();
                    buttonStart.Enabled = false;
                    buttonStop.Enabled = true;
                });
            }

            if (!Common.HasInternetConnection())
            {
                if (this.IsHandleCreated)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        buttonStart.Enabled = true;
                        buttonStop.Enabled = false;
                    });
                }

                return;
            }

            if (token.IsCancellationRequested)
                return;

            //Log("Getting latest subscriptions from YouTube");
            //foreach (Subscription missingSubscription in tempUserSubscriptions.Where(p => Common.TrackedSubscriptions.Where(o => o.Title == p.Title).FirstOrDefault() == null))
            //{
            //    Subscription sub = AssignUploadsPlaylist(missingSubscription);
            //    sub.LastVideoPublishDate = GetMostRecentUploadDate(sub);
            //    Common.TrackedSubscriptions.Add(sub);
            //}

            //Remove any extraneous (unsubscribed since last time the program was run) subscriptions
            //List<Subscription> unsubscribedSubscriptions = Common.TrackedSubscriptions.Where(p => tempUserSubscriptions.Where(o => o.Title == p.Title).FirstOrDefault() == null && !p.IsPlaylist).ToList();
            //foreach (Subscription unsubscribedSubscription in unsubscribedSubscriptions)
            //    Common.TrackedSubscriptions.Remove(unsubscribedSubscription);

            //if (token.IsCancellationRequested)
            //    return;

            //Remove any duplicates
            //Common.TrackedSubscriptions = Common.TrackedSubscriptions.GroupBy(p => p.PlaylistIdToWatch).Select(p => p.First()).ToList();

            if (Settings.Instance.CheckForMissedUploads &&
                (Settings.Instance.DownloadVideos || Settings.Instance.AddToPocket)) //Don't run unnecessary iterations if the user doesn't want to download or add them to Pocket
            {
                Log("Looking for recent uploads");
                bool tempNotificationSetting = Settings.Instance.ShowNotifications;
                Settings.Instance.ShowNotifications = false; //Turn off notifications temporarily because we don't want a bunch of notifications on startup
                CheckForNewVideoFromSubscriptions(token);
                Settings.Instance.ShowNotifications = tempNotificationSetting;
            }

            if (token.IsCancellationRequested)
                return;

            Log("Iterations started");
            InitializeTimer();
            this.Invoke((MethodInvoker)delegate { timer.Start(); });
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Log("Checking for new uploads...");
            CheckForNewVideoFromSubscriptions(cancelTokenSource.Token);
        }

        private void CheckForNewVideoFromSubscriptions(CancellationToken token)
        {
            if (!Common.HasInternetConnection())
                return;

            foreach (Subscription sub in Common.TrackedSubscriptions)
            {
                if (token.IsCancellationRequested)
                    return;

                List<PlaylistItem> newUploads = YouTubeFunctions.GetMostRecentUploads(sub, sub.LastVideoPublishDate);
                foreach (PlaylistItem item in newUploads)
                {
                    PlaylistItemSnippet newUploadDetails = item.Snippet;
                    Log("New uploaded detected: " + sub.Title + " (" + newUploadDetails.Title + ")");
                    DoActionsForNewUpload(newUploadDetails, sub, token);
                }

                if (newUploads.Count > 0)
                    sub.LastVideoPublishDate = (DateTime)newUploads.First().Snippet.PublishedAt;
            }

            Common.SerializeSubscriptions();
        }

        private void DoActionsForNewUpload(PlaylistItemSnippet newUpload, Subscription sub, CancellationToken downloadCancellation)
        {
            if (Settings.Instance.ShowNotifications)
            {
                ShowNotification(newUpload.Title, "New video from " + sub.Title, newUpload.Thumbnails?.Standard?.Url,
                                 Common.YOUTUBEBASEURL + newUpload.ResourceId.VideoId);
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

            Notification notification2 = new Notification(notificationTitle, notificationSubTitle, imageURL, videoURL);
            this.Invoke((MethodInvoker)delegate ()
            {
                notification2.Show();
            });
        }

        private void DownloadYouTubeVideo(string youTubeVideoId, string destinationFolder, CancellationToken token)
        {
            Log("Downloading video...");
            YouTubeFunctions.DownloadYouTubeVideoAsync(youTubeVideoId, destinationFolder, token).Wait(); //Async
            Log("Download complete");
        }

        private void AddYouTubeVideoToPocket(string youTubeVideoId)
        {
            Log("Adding video to Pocket...");
            string youTubeURL = Common.YOUTUBEBASEURL + youTubeVideoId;
            Settings.PocketClient.Add(new Uri(youTubeURL)).Wait(); //Async
            Log("Video added to Pocket");
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            cancelTokenSource.Cancel();

            buttonStop.Enabled = false;
            buttonStart.Enabled = true;

            timer.Stop();

            Log("Iterations stopped");
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

            PlaylistManager manager = new PlaylistManager();
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
        }
    }
}
