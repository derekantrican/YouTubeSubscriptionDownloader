using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Apis.YouTube.v3;
using Google.Apis.Auth.OAuth2;
using System.IO;
using System.Threading;
using Google.Apis.Util.Store;
using Google.Apis.Services;
using Google.Apis.YouTube.v3.Data;
using System.Net.Mail;
using System.Net;
using YoutubeExplode;
using YoutubeExplode.Models;
using System.Text.RegularExpressions;
using YoutubeExplode.Models.MediaStreams;
using System.Xml.Serialization;
using PocketSharp;

namespace YouTubeSubscriptionDownloader
{
    public partial class Form1 : Form
    {
        static string ApplicationName = "YouTube Subscription Downloader";
        static string UserSettings = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationName);
        static string CredentialsPath = Path.Combine(UserSettings, "Credentials");

        List<string> missingLogLines = new List<string>();

        System.Windows.Forms.Timer timer = null;

        YouTubeService service = null;
        static string[] Scopes = { YouTubeService.Scope.Youtube };

        List<Subscription> userSubscriptions = new List<Subscription>();

        public Form1(bool start = false)
        {
            InitializeComponent();
            this.HandleCreated += Form1_HandleCreated;

            if (!Directory.Exists(UserSettings))
                Directory.CreateDirectory(UserSettings);

            Settings.ReadSettings();

            initializePocket();
            initializeTimer();

            buttonStop.Enabled = false;

            if (Settings.Instance.StartIterationsOnStartup || start)
                buttonStart_Click(null, null);
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

        private bool CheckForInternetConnection()
        {
            try
            {
                using (WebClient client = new WebClient())
                using (client.OpenRead("http://www.youtube.com"))
                    return true;
            }
            catch
            {
                Log("!!NO INTERNET CONNECTION!!");
                return false;
            }
        }

        private void initializePocket()
        {
            if (Settings.Instance.AddToPocket && Settings.Instance.PocketAuthCode != "")
                Settings.pocketClient = new PocketClient("69847-fc525ffd3205de609a7429bf", Settings.Instance.PocketAuthCode, "https://getpocket.com/a/queue/");
        }

        private void initializeTimer()
        {
            if (timer != null)
                timer.Dispose();

            timer = new System.Windows.Forms.Timer();
            timer.Interval = Settings.Instance.IterationFrequency * 60 * 1000;
            timer.Tick += Timer_Tick;
        }

        private void showNotification(string notificationSubTitle, string notificationTitle = "")
        {
            if (Settings.Instance.ShowNotifications)
                notifyIconFormInTray.ShowBalloonTip(1000, notificationTitle, notificationSubTitle, ToolTipIcon.None);
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
            if (!CheckForInternetConnection())
                return;

            richTextBoxLog.Clear();
            buttonStart.Enabled = false;
            buttonStop.Enabled = true;

            List<Subscription> tempUserSubscriptions = new List<Subscription>();

            if (service == null)
            {
                Log("Authorizing...");

                UserCredential credential;
                string clientSecretString = "{\"installed\":" +
                                                "{" +
                                                    "\"client_id\":\"761670588704-lgl5qbcv5odmq1vlq3lcgqv67fr8vkdn.apps.googleusercontent.com\"," +
                                                    "\"project_id\":\"youtube-downloader-174123\"," +
                                                    "\"auth_uri\":\"https://accounts.google.com/o/oauth2/auth\"," +
                                                    "\"token_uri\":\"https://accounts.google.com/o/oauth2/token\"," +
                                                    "\"auth_provider_x509_cert_url\":\"https://www.googleapis.com/oauth2/v1/certs\"," +
                                                    "\"client_secret\":\"_uzJUnD4gNiIpIL991kmCuvB\"," +
                                                    "\"redirect_uris\":[\"urn:ietf:wg:oauth:2.0:oob\",\"http://localhost\"]" +
                                                "}" +
                                            "}";
                byte[] byteArray = Encoding.ASCII.GetBytes(clientSecretString);

                using (var stream = new MemoryStream(byteArray))
                {
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(CredentialsPath, true)).Result;
                }

                service = new YouTubeService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName
                });
            }

            Log("Retrieving subscriptions...");
            DeserializeSubscriptions();

            SubscriptionsResource.ListRequest listSubscriptions = service.Subscriptions.List("snippet");
            listSubscriptions.Order = SubscriptionsResource.ListRequest.OrderEnum.Alphabetical;
            listSubscriptions.Mine = true;
            listSubscriptions.MaxResults = 50;
            SubscriptionListResponse response = listSubscriptions.Execute();

            while (response.NextPageToken != null)
            {
                tempUserSubscriptions.AddRange(ConvertSubscriptionItems(response.Items.ToList()));
                listSubscriptions.PageToken = response.NextPageToken;
                response = listSubscriptions.Execute();
            }

            tempUserSubscriptions.AddRange(ConvertSubscriptionItems(response.Items.ToList()));

            Log("Getting latest subscriptions from YouTube");
            foreach (Subscription missingSubscription in tempUserSubscriptions.Where(p => userSubscriptions.Where(o => o.Title == p.Title).FirstOrDefault() == null))
            {
                Subscription sub = GetUploadsPlaylist(missingSubscription);
                sub.LastVideoPublishDate = GetMostRecentUploadDate(sub);
                userSubscriptions.Add(sub);
            }

            //Remove any extraneous (unsubscribed since last time the program was run) subscriptions
            List<Subscription> unsubscribedSubscriptions = userSubscriptions.Where(p => tempUserSubscriptions.Where(o => o.Title == p.Title).FirstOrDefault() == null && !p.IsPlaylist).ToList();
            foreach (Subscription unsubscribedSubscription in unsubscribedSubscriptions)
                userSubscriptions.Remove(unsubscribedSubscription);

            //Remove any duplicates
            userSubscriptions = userSubscriptions.GroupBy(p => p.UploadsPlaylist).Select(p => p.First()).ToList();

            if (Settings.Instance.SerializeSubscriptions &&
                (Settings.Instance.DownloadVideos || Settings.Instance.AddToPocket)) //Don't run unnecessary iterations if the user doesn't want to download or add them to Pocket
            {
                Log("Looking for recent uploads");
                LookForMoreRecentUploads();
            }

            Log("Iterations started");
            initializeTimer();
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Log("Checking for new uploads...");
            Task.Run(() => CheckForNewVideoFromSubscriptions());
        }

        private void LookForMoreRecentUploads()
        {
            for (int i = 0; i < userSubscriptions.Count(); i++)
            {
                if (string.IsNullOrEmpty(userSubscriptions[i].UploadsPlaylist))
                    userSubscriptions[i] = GetUploadsPlaylist(userSubscriptions[i]);

                Subscription sub = userSubscriptions[i];
                DateTime mostRecentUploadDate = GetMostRecentUploadDate(sub);
                if (sub.LastVideoPublishDate == mostRecentUploadDate)
                    continue;

                List<PlaylistItem> moreRecentUploads = new List<PlaylistItem>();

                //Todo: get all videos between sub.LastVideoPublishDate and mostRecentUploadDate
                PlaylistItemsResource.ListRequest listRequest = service.PlaylistItems.List("snippet");
                listRequest.PlaylistId = sub.UploadsPlaylist;
                listRequest.MaxResults = 50;
                PlaylistItemListResponse response = listRequest.Execute();
                List<PlaylistItem> responseItems = response.Items.ToList();

                ////------------------------------------
                ////   There is currently a bug with retrieving uploads playlists where the returned order does not match
                ////   the order shown on YouTube https://issuetracker.google.com/issues/65067744 . To combat this, we
                ////   will reorder the results by upload date

                responseItems = responseItems.OrderByDescending(p => p.Snippet.PublishedAt).ToList();
                ////------------------------------------

                //while (responseItems.Where(p => p.Id != )) //Todo: In the future we should store the "mostRecentUploadId" on a Subscription and get that instead
                while (responseItems.Where(p => p.Snippet.PublishedAt <= sub.LastVideoPublishDate).FirstOrDefault() == null)
                {
                    moreRecentUploads.AddRange(responseItems);

                    listRequest.PageToken = response.NextPageToken;
                    responseItems = listRequest.Execute().Items.ToList();
                }

                foreach (PlaylistItem item in responseItems)
                {
                    if (item.Snippet.PublishedAt <= sub.LastVideoPublishDate) //Todo: In the future we should store the "mostRecentUploadId" on a Subscription and get that instead
                        break;

                    moreRecentUploads.Add(item);
                }

                foreach (PlaylistItem moreRecent in moreRecentUploads)
                {
                    if (Settings.Instance.DownloadVideos)
                        DownloadYouTubeVideo(moreRecent.Snippet.ResourceId.VideoId, Settings.Instance.DownloadDirectory);

                    if (Settings.Instance.AddToPocket)
                        AddYouTubeVideoToPocket(moreRecent.Snippet.ResourceId.VideoId);
                }

                sub.LastVideoPublishDate = mostRecentUploadDate;
            }
        }

        private List<Subscription> ConvertSubscriptionItems(List<Google.Apis.YouTube.v3.Data.Subscription> itemList)
        {
            List<Subscription> subscriptions = new List<Subscription>();

            foreach (Google.Apis.YouTube.v3.Data.Subscription item in itemList)
            {
                subscriptions.Add(new Subscription()
                {
                    Id = item.Snippet.ResourceId.ChannelId,
                    Title = item.Snippet.Title
                });
            }

            return subscriptions;
        }

        private Subscription GetUploadsPlaylist(Subscription sub)
        {
            if (string.IsNullOrWhiteSpace(sub.UploadsPlaylist))
            {
                ChannelsResource.ListRequest listRequest = service.Channels.List("contentDetails");
                listRequest.Id = sub.Id;
                ChannelListResponse response = listRequest.Execute();

                if (response.Items.Count <= 0)
                    return sub;

                sub.UploadsPlaylist = response.Items.FirstOrDefault().ContentDetails.RelatedPlaylists.Uploads;
            }

            return sub;
        }

        private DateTime GetMostRecentUploadDate(Subscription sub)
        {
            PlaylistItem item = GetMostRecentUploads(sub).FirstOrDefault();

            if (item == null)
                return DateTime.MinValue;

            return (DateTime)item.Snippet.PublishedAt;
        }

        private List<PlaylistItem> GetMostRecentUploads(Subscription sub, int qty = 15)
        {
            if (!string.IsNullOrWhiteSpace(sub.UploadsPlaylist))
            {
                PlaylistItemsResource.ListRequest listRequest = service.PlaylistItems.List("snippet");
                listRequest.PlaylistId = sub.UploadsPlaylist;

                ////------------------------------------
                ////   There is currently a bug with retrieving uploads playlists where the returned order does not match
                ////   the order shown on YouTube https://issuetracker.google.com/issues/65067744 . To combat this, we
                ////   will get the top 15 results and order them by upload date (hopefully 15 is enough to contain the
                ////   most recent upload).

                listRequest.MaxResults = qty;
                PlaylistItemListResponse response = listRequest.Execute();
                List<PlaylistItem> resultsByDate = response.Items.OrderByDescending(p => p.Snippet.PublishedAt).ToList();
                ////------------------------------------

                return resultsByDate;
            }

            return new List<PlaylistItem>();
        }

        private List<PlaylistItem> GetUploadsSince(Subscription sub, DateTime date)
        {
            List<PlaylistItem> mostRecentUploads = GetMostRecentUploads(sub);

            return mostRecentUploads.Where(p => p.Snippet.PublishedAt > date).ToList();
        }

        private void CheckForNewVideoFromSubscriptions()
        {
            if (!CheckForInternetConnection())
                return;

            foreach (Subscription sub in userSubscriptions)
            {
                List<PlaylistItem> newUploads = GetUploadsSince(sub, sub.LastVideoPublishDate);
                foreach (PlaylistItem item in newUploads.OrderBy(p => p.Snippet.PublishedAt)) //Loop through uploads backwards so that newest upload is last
                {
                    PlaylistItemSnippet newUploadDetails = item.Snippet;
                    showNotification(newUploadDetails.Title, "New video from " + sub.Title);
                    Log("New uploaded detected: " + sub.Title + " (" + newUploadDetails.Title + ")");
                    DownloadYouTubeVideo(newUploadDetails.ResourceId.VideoId, Settings.Instance.DownloadDirectory);
                    AddYouTubeVideoToPocket(newUploadDetails.ResourceId.VideoId);
                }

                if (newUploads.Count > 0)
                    sub.LastVideoPublishDate = (DateTime)newUploads.First().Snippet.PublishedAt;

                SerializeSubscriptions();
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

        private async void DownloadYouTubeVideo(string youTubeVideoId, string destinationFolder)
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

                await client.DownloadMediaStreamAsync(streamInfo, Path.Combine(destinationFolder, fileName));
                Log("Download complete");
            }
        }

        private void DeserializeSubscriptions()
        {
            string serializationPath = Path.Combine(UserSettings, "Subscriptions.xml");
            if (File.Exists(serializationPath))
            {
                FileStream fileStream = new FileStream(serializationPath, FileMode.Open);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Subscription>));
                userSubscriptions.AddRange((List<Subscription>)xmlSerializer.Deserialize(fileStream));
            }
        }

        private void SerializeSubscriptions(string overrideSerializationPath = null)
        {
            if (userSubscriptions == null || userSubscriptions.Count <= 0)
            {
                Log("The program has not been started so no subscriptions have been retrieved");
                Log("Start the program and try again");
                return;
            }

            List<Subscription> subscriptionsToSerialize = new List<Subscription>();
            if (Settings.Instance.SerializeSubscriptions)
                subscriptionsToSerialize = userSubscriptions;
            else
                subscriptionsToSerialize = userSubscriptions.Where(p => p.IsPlaylist).ToList();

            string serializationPath = string.IsNullOrEmpty(overrideSerializationPath) ? Path.Combine(UserSettings, "Subscriptions.xml") : overrideSerializationPath;
            if (File.Exists(serializationPath))
                File.Delete(serializationPath);

            if (subscriptionsToSerialize.Any())
            {
                TextWriter writer = new StreamWriter(serializationPath);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Subscription>));
                xmlSerializer.Serialize(writer, subscriptionsToSerialize); //Could wrap this in a try{ }catch{ } (and writer.Close() in a finally{ }), but we're catching all exceptions already
                writer.Close();

                if (!string.IsNullOrEmpty(overrideSerializationPath))
                    MessageBox.Show("Subscriptions serialized to " + serializationPath);
            }
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            buttonStop.Enabled = false;
            buttonStart.Enabled = true;

            timer.Stop();

            Log("Iterations stopped");
        }

        private void pictureBoxSettings_Click(object sender, EventArgs e)
        {
            if (Form.ModifierKeys == Keys.Control)
                SerializeSubscriptions(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Subscriptions.xml"));
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

            PlaylistManager manager = new PlaylistManager(service, userSubscriptions.Where(p => p.IsPlaylist).ToList());
            manager.SubscriptionsUpdated += (List<Subscription> playlistSubscriptions) => 
            {
                userSubscriptions.RemoveAll(p => p.IsPlaylist);
                playlistSubscriptions.ForEach(p => userSubscriptions.Add(p));
                SerializeSubscriptions();
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
            foreach(Screen screen in Screen.AllScreens)
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
            SerializeSubscriptions();
        }
    }
}
