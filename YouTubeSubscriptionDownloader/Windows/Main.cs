using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using Google.Apis.YouTube.v3.Data;
using System.Net;
using YoutubeExplode;
using System.Text.RegularExpressions;
using YoutubeExplode.Models.MediaStreams;

namespace YouTubeSubscriptionDownloader
{
    public partial class Main : Form
    {
        List<string> missingLogLines = new List<string>();

        System.Windows.Forms.Timer timer = null;

        CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

        /*====================================================
         * TODO:
         * - add a setting for something like "Automatically get latest subscriptions from YouTube" that will automatically get latest subscriptions (if subscribed to a new channel on YouTube).
         *      This would also control if a subscription gets removed from this program when unsubscribed on YouTube
         *  
         * - Show Subscription Manager on first time run?
         * 
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
            //{
            //    Task task = Task.Run(() => Start(cancelTokenSource.Token));
            //    task.ContinueWith(t => HandleAsyncException(t.Exception.InnerException), TaskContinuationOptions.OnlyOnFaulted);
            //}
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

        private void ShowNotification(string notificationSubTitle, string notificationTitle = "", string imageURL = "", string videoURL = "")
        {
            if (Settings.Instance.ShowNotifications)
            {
                if (!Settings.Instance.ShowThumbnailInNotification)
                    imageURL = null;

                Notification notification2 = new Notification(notificationTitle, notificationSubTitle, imageURL, videoURL);
                this.Invoke((MethodInvoker)delegate ()
                {
                    notification2.Show();
                });
            }
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

        private void HandleAsyncException(Exception ex)
        {
            Log("Error encountered: " + ex.Message);
            Log("Please contact the developer");

            string crashPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "YouTube Subscription Downloader");
            string exceptionString = "";
            exceptionString = "[" + DateTime.Now + "] EXCEPTION MESSAGE: " + ex?.Message + Environment.NewLine + Environment.NewLine;
            exceptionString += "[" + DateTime.Now + "] INNER EXCEPTION: " + ex?.InnerException + Environment.NewLine + Environment.NewLine;
            exceptionString += "[" + DateTime.Now + "] STACK TRACE: " + ex?.StackTrace + Environment.NewLine + Environment.NewLine;
            File.AppendAllText(Path.Combine(crashPath, "CRASHREPORT (" + DateTime.Now.ToString("yyyy.MM.dd.HH.mm.ss") + ").log"), exceptionString);

            //Stop iterations
            cancelTokenSource.Cancel();

            buttonStop.Enabled = false;
            buttonStart.Enabled = true;

            timer.Stop();

            Log("Iterations stopped");
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            cancelTokenSource.Dispose();
            cancelTokenSource = new CancellationTokenSource();

            Task task = Task.Run(() => Start(cancelTokenSource.Token));
            task.ContinueWith(t => HandleAsyncException(t.Exception.InnerException), TaskContinuationOptions.OnlyOnFaulted);
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

            if (Settings.Instance.SerializeSubscriptions &&
                (Settings.Instance.DownloadVideos || Settings.Instance.AddToPocket)) //Don't run unnecessary iterations if the user doesn't want to download or add them to Pocket
            {
                Log("Looking for recent uploads");
                LookForMoreRecentUploads(token);
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
            Task task = Task.Run(() => CheckForNewVideoFromSubscriptions(cancelTokenSource.Token));
            task.ContinueWith(t => HandleAsyncException(t.Exception.InnerException), TaskContinuationOptions.OnlyOnFaulted);
        }

        private void LookForMoreRecentUploads(CancellationToken token)
        {
            for (int i = 0; i < Common.TrackedSubscriptions.Count(); i++)
            {
                if (token.IsCancellationRequested)
                    return;

                if (string.IsNullOrEmpty(Common.TrackedSubscriptions[i].PlaylistIdToWatch))
                    Common.TrackedSubscriptions[i] = AssignUploadsPlaylist(Common.TrackedSubscriptions[i]);

                Subscription sub = Common.TrackedSubscriptions[i];
                DateTime mostRecentUploadDate = GetMostRecentUploadDate(sub);
                if (sub.LastVideoPublishDate == mostRecentUploadDate)
                    continue;

                List<PlaylistItem> moreRecentUploads = YouTubeFunctions.GetMostRecentUploads(sub, sub.LastVideoPublishDate);

                foreach (PlaylistItem moreRecent in moreRecentUploads)
                {
                    Log("New uploaded detected: " + sub.Title + " (" + moreRecent.Snippet.Title + ")");

                    if (Settings.Instance.DownloadVideos)
                        DownloadYouTubeVideo(moreRecent.Snippet.ResourceId.VideoId, Settings.Instance.DownloadDirectory, token);

                    if (Settings.Instance.AddToPocket)
                        AddYouTubeVideoToPocket(moreRecent.Snippet.ResourceId.VideoId);
                }

                sub.LastVideoPublishDate = mostRecentUploadDate;
            }
        }

        private Subscription AssignUploadsPlaylist(Subscription sub)
        {
            if (string.IsNullOrWhiteSpace(sub.PlaylistIdToWatch))
            {
                string uploadsPlaylistId = YouTubeFunctions.GetChannelUploadsPlaylistId(sub);
                if (!string.IsNullOrEmpty(uploadsPlaylistId))
                    sub.PlaylistIdToWatch = uploadsPlaylistId;
            }

            return sub;
        }

        private DateTime GetMostRecentUploadDate(Subscription sub)
        {
            PlaylistItem mostRecentUpload;
            try
            {
                mostRecentUpload = YouTubeFunctions.GetMostRecentUploads(sub).FirstOrDefault();
            }
            catch (WebException)
            {
                Log("There was a problem contacting YouTube...");
            }
            catch (Google.GoogleApiException ex)
            {
                if (ex.HttpStatusCode == HttpStatusCode.InternalServerError ||
                    (ex.InnerException != null && ex.InnerException is WebException))
                    Log("There was a problem contacting YouTube...");
                else
                {
                    throw;
                }                  
            }


            PlaylistItem item = YouTubeFunctions.GetMostRecentUploads(sub).FirstOrDefault();

            if (item == null)
                return DateTime.MinValue;

            return (DateTime)item.Snippet.PublishedAt;
        }

        private void CheckForNewVideoFromSubscriptions(CancellationToken token)
        {
            if (!Common.HasInternetConnection())
                return;

            foreach (Subscription sub in Common.TrackedSubscriptions)
            {
                List<PlaylistItem> newUploads = new List<PlaylistItem>();
                try
                {
                    newUploads = YouTubeFunctions.GetMostRecentUploads(sub, sub.LastVideoPublishDate);
                }
                catch (WebException)
                {
                    Log("There was a problem contacting YouTube...");
                    continue;
                }
                catch (Google.GoogleApiException ex)
                {
                    if (ex.HttpStatusCode == HttpStatusCode.InternalServerError ||
                        (ex.InnerException != null && ex.InnerException is WebException))
                    {
                        Log("There was a problem contacting YouTube...");
                        continue;
                    }
                    else
                    {
                        throw;
                    }                       
                }

                if (newUploads.Count >= 2)
                {
                    PlaylistItem latestUpload = newUploads.OrderBy(p => p.Snippet.PublishedAt).ToList()[0];
                    PlaylistItem secondToLatestUpload = newUploads.OrderBy(p => p.Snippet.PublishedAt).ToList()[1];

                    //Sometimes we sense a new video twice in "newUploads". Not sure why, but this will prevent
                    //notifying for that video twice
                    if (latestUpload != null && secondToLatestUpload != null &&
                        latestUpload.Snippet.ResourceId.VideoId == secondToLatestUpload.Snippet.ResourceId.VideoId)
                        newUploads.RemoveAt(0);
                }

                foreach (PlaylistItem item in newUploads.OrderBy(p => p.Snippet.PublishedAt)) //Loop through uploads backwards so that newest upload is last
                {
                    PlaylistItemSnippet newUploadDetails = item.Snippet;

                    ShowNotification(newUploadDetails.Title, "New video from " + sub.Title, newUploadDetails.Thumbnails?.Standard?.Url,
                                     "https://www.youtube.com/watch?v=" + newUploadDetails.ResourceId.VideoId);
                    Log("New uploaded detected: " + sub.Title + " (" + newUploadDetails.Title + ")");
                    DownloadYouTubeVideo(newUploadDetails.ResourceId.VideoId, Settings.Instance.DownloadDirectory, token);
                    AddYouTubeVideoToPocket(newUploadDetails.ResourceId.VideoId);
                }

                if (newUploads.Count > 0)
                    sub.LastVideoPublishDate = (DateTime)newUploads.First().Snippet.PublishedAt;

                Common.SerializeSubscriptions();
            }
        }

        private async void AddYouTubeVideoToPocket(string youTubeVideoId)
        {
            if (Settings.Instance.AddToPocket)
            {
                Log("Adding video to Pocket...");
                string youTubeURL = "https://www.youtube.com/watch?v=" + youTubeVideoId;
                await Settings.pocketClient.Add(new Uri(youTubeURL));
                Log("Video added to Pocket");
            }
        }

        private async void DownloadYouTubeVideo(string youTubeVideoId, string destinationFolder, CancellationToken token)
        {
            if (Settings.Instance.DownloadVideos)
            {
                Log("Downloading video...");
                YoutubeClient client = new YoutubeClient();
                var videoInfo = await client.GetVideoAsync(youTubeVideoId);
                var streamInfoSet = await client.GetVideoMediaStreamInfosAsync(youTubeVideoId);

                MuxedStreamInfo streamInfo = null;
                if (Settings.Instance.PreferredQuality != "Highest")
                    streamInfo = streamInfoSet.Muxed.Where(p => p.VideoQualityLabel == Settings.Instance.PreferredQuality).FirstOrDefault();

                if (Settings.Instance.PreferredQuality == "Highest" || streamInfo == null)
                    streamInfo = streamInfoSet.Muxed.WithHighestVideoQuality();

                string fileExtension = streamInfo.Container.GetFileExtension();
                string fileName = "[" + videoInfo.Author + "] " + videoInfo.Title + "." + fileExtension;

                //Remove invalid characters from filename
                string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
                Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
                fileName = r.Replace(fileName, "");

                await client.DownloadMediaStreamAsync(streamInfo, Path.Combine(destinationFolder, fileName), cancellationToken: token);
                Log("Download complete");
            }
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
            manager.SubscriptionsUpdated += (List<Subscription> playlistSubscriptions) =>
            {
                Common.TrackedSubscriptions.RemoveAll(p => p.IsPlaylist);
                foreach (Subscription playlist in playlistSubscriptions)
                {
                    playlist.LastVideoPublishDate = GetMostRecentUploadDate(playlist);
                    Common.TrackedSubscriptions.Add(playlist);
                }

                Common.SerializeSubscriptions();
            };
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
