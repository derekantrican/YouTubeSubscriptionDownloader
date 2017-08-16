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

        System.Windows.Forms.Timer timer = null;

        YouTubeService service;
        static string[] Scopes = { YouTubeService.Scope.Youtube };

        List<Subscription> userSubscriptions = new List<Subscription>();

        public Form1()
        {
            InitializeComponent();

            if (!Directory.Exists(UserSettings))
                Directory.CreateDirectory(UserSettings);

            Settings.ReadSettings();

            initializePocket();
            initializeTimer();

            buttonStop.Enabled = false;
        }

        private void initializePocket()
        {
            if (Settings.AddToPocket && Settings.PocketAuthCode != "")
                Settings.pocketClient = new PocketClient("69847-fc525ffd3205de609a7429bf", Settings.PocketAuthCode, "https://getpocket.com/a/queue/");
        }

        private void initializeTimer()
        {
            if (timer != null)
                timer.Dispose();

            timer = new System.Windows.Forms.Timer();
            timer.Interval = 5 * 60 * 1000; //every 5 minutes
            timer.Tick += Timer_Tick;
        }

        private void showNotification(string notificationSubTitle, string notificationTitle = "")
        {
            if (Settings.ShowNotifications)
                notifyIconFormInTray.ShowBalloonTip(1000, notificationTitle, notificationSubTitle, ToolTipIcon.None);
        }

        private void Log(string itemToLog)
        {
            DateTime date = DateTime.Now;

            this.richTextBoxLog.Invoke((MethodInvoker)delegate
            {
                richTextBoxLog.Text += "[" + date + "] " + itemToLog + Environment.NewLine;

                richTextBoxLog.SelectionStart = richTextBoxLog.Text.Length;
                richTextBoxLog.ScrollToCaret();
            });
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            richTextBoxLog.Clear();
            buttonStart.Enabled = false;
            buttonStop.Enabled = true;

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

            Log("Retrieving subscriptions...");
            userSubscriptions = new List<Subscription>(); //Clear out the userSubscriptions list
            SubscriptionsResource.ListRequest listSubscriptions = service.Subscriptions.List("snippet");
            listSubscriptions.Order = SubscriptionsResource.ListRequest.OrderEnum.Alphabetical;
            listSubscriptions.Mine = true;
            listSubscriptions.MaxResults = 50;
            SubscriptionListResponse response = listSubscriptions.Execute();

            while (response.NextPageToken != null)
            {
                userSubscriptions.AddRange(ConvertSubscriptionItems(response.Items.ToList()));
                listSubscriptions.PageToken = response.NextPageToken;
                response = listSubscriptions.Execute();
            }

            userSubscriptions.AddRange(ConvertSubscriptionItems(response.Items.ToList()));

            //Todo: userSubscriptions needs to be serialized to an XML, then read so that we don't have to do all these calls every time,
            //      but we may have to update the subscriptions if the user subscribes to a new channel (or unsubscribes from another)

            Log("Retrieving uploads playlists...");
            GetUploadsPlaylists(); //Todo: shouldn't have to do this if we serialize, then read userSubscriptions
            Log("Retrieving most recent upload date...");
            GetMostRecentUploadDate(); //Todo: shouldn't have to do this if we serialize, then read userSubscriptions

            Log("Iterations started");
            initializeTimer();
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Log("Checking for new uploads...");
            Task.Run(() => CheckForNewVideoFromSubscriptions());
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

        private void GetUploadsPlaylists()
        {
            foreach (Subscription sub in userSubscriptions)
            {
                if (string.IsNullOrWhiteSpace(sub.UploadsPlaylist))
                {
                    ChannelsResource.ListRequest listRequest = service.Channels.List("contentDetails");
                    listRequest.Id = sub.Id;
                    ChannelListResponse response = listRequest.Execute();

                    if (response.Items.Count <= 0)
                        continue;

                    sub.UploadsPlaylist = response.Items.FirstOrDefault().ContentDetails.RelatedPlaylists.Uploads;
                }
            }
        }

        private void GetMostRecentUploadDate()
        {
            foreach (Subscription sub in userSubscriptions)
            {
                if (!string.IsNullOrWhiteSpace(sub.UploadsPlaylist))
                {
                    if (sub.Title == "Watch List")
                    {
                        PlaylistItemsResource.ListRequest listRequest = service.PlaylistItems.List("contentDetails");
                        listRequest.PlaylistId = sub.UploadsPlaylist;
                        listRequest.MaxResults = 1;
                        PlaylistItemListResponse response = listRequest.Execute();
                        sub.LastVideoPublishDate = (DateTime)response.Items.FirstOrDefault().Snippet.PublishedAt;
                    }
                    else
                    {
                        PlaylistItemsResource.ListRequest listRequest = service.PlaylistItems.List("snippet");
                        listRequest.PlaylistId = sub.UploadsPlaylist;
                        listRequest.MaxResults = 1;
                        PlaylistItemListResponse response = listRequest.Execute();
                        sub.LastVideoPublishDate = (DateTime)response.Items.FirstOrDefault().Snippet.PublishedAt;
                    }
                }
            }
        }

        private void CheckForNewVideoFromSubscriptions()
        {
            foreach (Subscription sub in userSubscriptions)
            {
                if (!string.IsNullOrWhiteSpace(sub.UploadsPlaylist))
                {
                    PlaylistItemsResource.ListRequest listRequest = service.PlaylistItems.List("snippet");
                    listRequest.PlaylistId = sub.UploadsPlaylist;
                    listRequest.MaxResults = 1;
                    PlaylistItemListResponse response = listRequest.Execute();
                    PlaylistItemSnippet newUploadDetails = response.Items.FirstOrDefault().Snippet;

                    DateTime newUploadPublishedDate = (DateTime)newUploadDetails.PublishedAt;
                    if (newUploadPublishedDate > sub.LastVideoPublishDate)
                    {

                        //Todo: eventually, I should loop back through the uploads (response.Items) until I find all of the ones uploaded since
                        //      sub.LastVideoPublishDate. This way, if there are 5 uploads since this method last checked, we will get all 5

                        showNotification(newUploadDetails.Title, "New video from " + sub.Title);
                        Log("New uploaded detected: " + sub.Title + " (" + newUploadDetails.Title + ")");
                        DownloadYouTubeVideo(response.Items.FirstOrDefault().Snippet.ResourceId.VideoId, Settings.DownloadDirectory);
                        AddYouTubeVideoToPocket(response.Items.FirstOrDefault().Snippet.ResourceId.VideoId);

                        sub.LastVideoPublishDate = newUploadPublishedDate;
                    }
                }
            }
        }

        private async void AddYouTubeVideoToPocket(string youTubeVideoId)
        {
            if (Settings.AddToPocket)
            {
                Log("Adding video to Pocket...");
                string youTubeURL = "https://www.youtube.com/watch?v=" + youTubeVideoId;
                await Settings.pocketClient.Add(new Uri(youTubeURL));
                Log("Video added to Pocket");
            }
        }

        private async void DownloadYouTubeVideo(string youTubeVideoId, string destinationFolder)
        {
            if (Settings.DownloadVideos)
            {
                Log("Downloading video...");
                YoutubeClient client = new YoutubeClient();
                var videoInfo = await client.GetVideoInfoAsync(youTubeVideoId);

                //Get video @ 720p quality. If that doesn't exist, get the highest quality there is
                MixedStreamInfo streamInfo = null;
                if (Settings.PreferredQuality != "Highest")
                    streamInfo = videoInfo.MixedStreams.Where(p => p.VideoQualityLabel == Settings.PreferredQuality).FirstOrDefault();

                if (Settings.PreferredQuality == "Highest" || streamInfo == null)
                    streamInfo = videoInfo.MixedStreams.OrderBy(s => s.VideoQuality).Last();

                string fileExtension = streamInfo.Container.GetFileExtension();
                string fileName = "[" + videoInfo.Author.Title + "] " + videoInfo.Title + "." + fileExtension;

                //Remove invalid characters from filename
                string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
                Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
                fileName = r.Replace(fileName, "");

                Progress<double> downloadProgress = new Progress<double>();
                //downloadProgress.ProgressChanged += DownloadProgress_ProgressChanged;
                await client.DownloadMediaStreamAsync(streamInfo, Path.Combine(destinationFolder,fileName), downloadProgress);
                Log("Download complete");
            }
        }

        private void SerializeSubscriptions()
        {
            if (userSubscriptions == null || userSubscriptions.Count <= 0)
            {
                Log("The program has not been started so no subscriptions have been retrieved");
                Log("Start the program and try again");
                return;
            }

            string serializationPath = Path.Combine(UserSettings, "Subscriptions.xml");
            TextWriter writer = new StreamWriter(serializationPath);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Subscription>));
            xmlSerializer.Serialize(writer, userSubscriptions); //Could wrap this in a try{ }catch{ } (and writer.Close() in a finally{ }), but we're catching all exceptions already
            writer.Close();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            buttonStop.Enabled = false;
            buttonStart.Enabled = true;

            timer.Stop();

            Log("Iterations stopped");
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (Form.ModifierKeys == Keys.Control)
            {
                SerializeSubscriptions();
                MessageBox.Show("Subscriptions Serialized");
            }
            else
            {
                SettingsWindow settingsWindow = new SettingsWindow();
                settingsWindow.ShowDialog();
            }
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
        }
    }
}
