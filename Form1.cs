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

        YouTubeService service = null;
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
            if (Settings.SerializeSubscriptions)
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

            if (Settings.SerializeSubscriptions &&
                (Settings.DownloadVideos || Settings.AddToPocket)) //Don't run unnecessary iterations if the user doesn't want to download or add them to Pocket
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
                    if (Settings.DownloadVideos)
                        DownloadYouTubeVideo(moreRecent.Snippet.ResourceId.VideoId, Settings.DownloadDirectory);

                    if (Settings.AddToPocket)
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
            DateTime result = DateTime.MinValue;
            if (!string.IsNullOrWhiteSpace(sub.UploadsPlaylist))
            {
                PlaylistItemsResource.ListRequest listRequest = service.PlaylistItems.List("snippet");
                listRequest.PlaylistId = sub.UploadsPlaylist;
                listRequest.MaxResults = 1;
                PlaylistItemListResponse response = listRequest.Execute();
                return (DateTime)response.Items.FirstOrDefault().Snippet.PublishedAt;
            }

            return result;
        }

        private void CheckForNewVideoFromSubscriptions()
        {
            if (!CheckForInternetConnection())
                return;

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
                userSubscriptions = (List<Subscription>)xmlSerializer.Deserialize(fileStream);
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

            string serializationPath = string.IsNullOrEmpty(overrideSerializationPath) ? Path.Combine(UserSettings, "Subscriptions.xml") : overrideSerializationPath;

            if (File.Exists(serializationPath))
                File.Delete(serializationPath);

            TextWriter writer = new StreamWriter(serializationPath);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Subscription>));
            xmlSerializer.Serialize(writer, userSubscriptions); //Could wrap this in a try{ }catch{ } (and writer.Close() in a finally{ }), but we're catching all exceptions already
            writer.Close();

            if (!string.IsNullOrEmpty(overrideSerializationPath))
                MessageBox.Show("Subscriptions serialized to " + serializationPath);
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
                SerializeSubscriptions(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Subscriptions.xml"));
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Settings.SerializeSubscriptions)
                SerializeSubscriptions();
        }
    }
}
