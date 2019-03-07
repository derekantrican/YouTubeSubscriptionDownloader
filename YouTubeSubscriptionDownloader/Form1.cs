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
        CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

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
            {
                Task task = Task.Run(() => Start(cancelTokenSource.Token));
                task.ContinueWith(t => HandleAsyncException(t.Exception.InnerException), TaskContinuationOptions.OnlyOnFaulted);
            }
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

            if (!CheckForInternetConnection())
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


            if (token.IsCancellationRequested)
                return;

            Log("Retrieving subscriptions...");
            DeserializeSubscriptions();

            SubscriptionsResource.ListRequest listSubscriptions = service.Subscriptions.List("snippet");
            listSubscriptions.Order = SubscriptionsResource.ListRequest.OrderEnum.Alphabetical;
            listSubscriptions.Mine = true;
            listSubscriptions.MaxResults = 50;
            SubscriptionListResponse response = listSubscriptions.Execute();

            while (response.NextPageToken != null && !token.IsCancellationRequested)
            {
                tempUserSubscriptions.AddRange(ConvertSubscriptionItems(response.Items.ToList()));
                listSubscriptions.PageToken = response.NextPageToken;
                response = listSubscriptions.Execute();
            }

            tempUserSubscriptions.AddRange(ConvertSubscriptionItems(response.Items.ToList()));

            if (token.IsCancellationRequested)
                return;

            Log("Getting latest subscriptions from YouTube");
            foreach (Subscription missingSubscription in tempUserSubscriptions.Where(p => userSubscriptions.Where(o => o.Title == p.Title).FirstOrDefault() == null))
            {
                Subscription sub = AssignUploadsPlaylist(missingSubscription);
                sub.LastVideoPublishDate = GetMostRecentUploadDate(sub);
                userSubscriptions.Add(sub);
            }

            //Remove any extraneous (unsubscribed since last time the program was run) subscriptions
            List<Subscription> unsubscribedSubscriptions = userSubscriptions.Where(p => tempUserSubscriptions.Where(o => o.Title == p.Title).FirstOrDefault() == null && !p.IsPlaylist).ToList();
            foreach (Subscription unsubscribedSubscription in unsubscribedSubscriptions)
                userSubscriptions.Remove(unsubscribedSubscription);

            if (token.IsCancellationRequested)
                return;

            //Remove any duplicates
            userSubscriptions = userSubscriptions.GroupBy(p => p.PlaylistIdToWatch).Select(p => p.First()).ToList();

            if (Settings.Instance.SerializeSubscriptions &&
                (Settings.Instance.DownloadVideos || Settings.Instance.AddToPocket)) //Don't run unnecessary iterations if the user doesn't want to download or add them to Pocket
            {
                Log("Looking for recent uploads");
                LookForMoreRecentUploads(token);
            }

            if (token.IsCancellationRequested)
                return;

            Log("Iterations started");
            initializeTimer();
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
            for (int i = 0; i < userSubscriptions.Count(); i++)
            {
                if (token.IsCancellationRequested)
                    return;

                if (string.IsNullOrEmpty(userSubscriptions[i].PlaylistIdToWatch))
                    userSubscriptions[i] = AssignUploadsPlaylist(userSubscriptions[i]);

                Subscription sub = userSubscriptions[i];
                DateTime mostRecentUploadDate = GetMostRecentUploadDate(sub);
                if (sub.LastVideoPublishDate == mostRecentUploadDate)
                    continue;

                List<PlaylistItem> moreRecentUploads = new List<PlaylistItem>();
                moreRecentUploads = GetMostRecentUploads(sub, sub.LastVideoPublishDate);

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

        private List<Subscription> ConvertSubscriptionItems(List<Google.Apis.YouTube.v3.Data.Subscription> itemList)
        {
            List<Subscription> subscriptions = new List<Subscription>();

            foreach (Google.Apis.YouTube.v3.Data.Subscription item in itemList)
            {
                subscriptions.Add(new Subscription()
                {
                    ChannelId = item.Snippet.ResourceId.ChannelId,
                    Title = item.Snippet.Title
                });
            }

            return subscriptions;
        }

        private Subscription AssignUploadsPlaylist(Subscription sub)
        {
            if (string.IsNullOrWhiteSpace(sub.PlaylistIdToWatch))
            {
                string uploadsPlaylistId = GetChannelUploadsPlaylistId(sub);
                if (!string.IsNullOrEmpty(uploadsPlaylistId))
                    sub.PlaylistIdToWatch = uploadsPlaylistId;
            }

            return sub;
        }

        private string GetChannelUploadsPlaylistId(Subscription sub)
        {
            ChannelsResource.ListRequest listRequest = service.Channels.List("contentDetails");
            listRequest.Id = sub.ChannelId;
            ChannelListResponse response = listRequest.Execute();

            if (response.Items.Count <= 0)
                return null;

            return response.Items.FirstOrDefault().ContentDetails.RelatedPlaylists.Uploads;
        }

        private DateTime GetMostRecentUploadDate(Subscription sub)
        {
            PlaylistItem mostRecentUpload;
            try
            {
                mostRecentUpload = GetMostRecentUploads(sub).FirstOrDefault();
            }
            catch (WebException ex)
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


            PlaylistItem item = GetMostRecentUploads(sub).FirstOrDefault();

            if (item == null)
                return DateTime.MinValue;

            return (DateTime)item.Snippet.PublishedAt;
        }

        private List<PlaylistItem> GetMostRecentUploads(Subscription sub, DateTime? sinceDate = null)
        {
            List<PlaylistItem> resultsByDate = new List<PlaylistItem>();
            if (!string.IsNullOrWhiteSpace(sub.PlaylistIdToWatch))
            {
                PlaylistItemsResource.ListRequest listRequest = service.PlaylistItems.List("snippet,status");
                listRequest.PlaylistId = sub.PlaylistIdToWatch;
                PlaylistItemListResponse response;

                List<PlaylistItem> results = new List<PlaylistItem>();
                List<PlaylistItem> privateToPublic = new List<PlaylistItem>();
                if (sub.IsPlaylist &&
                    GetChannelUploadsPlaylistId(sub) != sub.PlaylistIdToWatch) //If this is the uploads playlist for the channel, it WILL be at least somewhat ordered by most recent
                {
                    //A playlist isn't necessarily in date order (the owner of the playlist could put them in any order).
                    //Unfortunately, that means we have to get every video in the playlist and order them by date. This will be costly for large playlists

                    listRequest.MaxResults = 50; //50 is the maximum
                    response = listRequest.Execute();
                    results.AddRange(response.Items);

                    while (response.NextPageToken != null)
                    {
                        listRequest.PageToken = response.NextPageToken;
                        response = listRequest.Execute();
                        results.AddRange(response.Items);
                    }
                }
                else
                {
                    listRequest.MaxResults = 50;
                    response = listRequest.Execute();
                    results.AddRange(response.Items);

                    //If we still haven't gotten any items older than the "sinceDate", get more
                    if (sinceDate != null)
                    {
                        while (!results.Any(p => p.Snippet.PublishedAt < sinceDate) && response.NextPageToken != null)
                        {
                            listRequest.PageToken = response.NextPageToken;
                            response = listRequest.Execute();
                            results.AddRange(response.Items);
                        }
                    }
                }

                //Check to see if any of the sub's private videos have change to public
                foreach (string videoId in sub.PrivateVideosToWatch)
                {
                    PlaylistItem matchingItem = results.Find(p => p.Snippet.ResourceId.VideoId == videoId);
                    if (matchingItem != null && matchingItem.Status.PrivacyStatus == "public")
                        privateToPublic.Add(matchingItem);
                }

                //Stop watching for private video status change if it is now in "privateToPublic"
                sub.PrivateVideosToWatch.RemoveAll(p => privateToPublic.Find(o => o.Snippet.ResourceId.VideoId == p) != null);

                List<PlaylistItem> recentPrivateVideos = results.Where(p => p.Status.PrivacyStatus == "private").ToList();
                foreach (PlaylistItem video in recentPrivateVideos)
                {
                    string videoId = video.Snippet.ResourceId.VideoId;
                    if (!sub.PrivateVideosToWatch.Contains(videoId))
                        sub.PrivateVideosToWatch.Add(videoId);

                    results.Remove(video);
                }

                if (sinceDate != null)
                    results = results.Where(p => p.Snippet.PublishedAt > sinceDate).ToList();

                results.AddRange(privateToPublic);

                ////------------------------------------
                ////   There is currently a bug with retrieving uploads playlists where the returned order does not match
                ////   the order shown on YouTube https://issuetracker.google.com/issues/65067744 . To combat this, we
                ////   will get the top 50 results and order them by upload date

                resultsByDate = results.OrderByDescending(p => p.Snippet.PublishedAt).ToList();
                ////------------------------------------
            }

            return resultsByDate;
        }

        private void CheckForNewVideoFromSubscriptions(CancellationToken token)
        {
            if (!CheckForInternetConnection())
                return;

            foreach (Subscription sub in userSubscriptions)
            {
                List<PlaylistItem> newUploads = new List<PlaylistItem>();
                try
                {
                    newUploads = GetMostRecentUploads(sub, sub.LastVideoPublishDate);
                }
                catch (WebException ex)
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
                    newUploads.RemoveAt(0);

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

        private void DeserializeSubscriptions()
        {
            string serializationPath = Path.Combine(UserSettings, "Subscriptions.xml");
            if (File.Exists(serializationPath))
            {
                FileStream fileStream = new FileStream(serializationPath, FileMode.Open);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Subscription>));
                userSubscriptions.AddRange((List<Subscription>)xmlSerializer.Deserialize(fileStream));
                fileStream.Close();
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
            cancelTokenSource.Cancel();

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
                foreach (Subscription playlist in playlistSubscriptions)
                {
                    playlist.LastVideoPublishDate = GetMostRecentUploadDate(playlist);
                    userSubscriptions.Add(playlist);
                }

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
            SerializeSubscriptions();
        }
    }
}
