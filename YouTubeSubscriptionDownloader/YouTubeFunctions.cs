using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Models.MediaStreams;

namespace YouTubeSubscriptionDownloader
{
    public class YouTubeFunctions
    {
        private static string[] Scopes = { YouTubeService.Scope.Youtube };
        public static YouTubeService Service { get; set; }

        public static void AuthService()
        {
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
                    Environment.UserName,
                    CancellationToken.None,
                    new FileDataStore(Common.CredentialsPath, true)).Result;
            }

            Service = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = Common.ApplicationName
            });
        }

        public static List<Subscription> GetUserSubscriptions(CancellationToken token)
        {
            List<Subscription> result = new List<Subscription>();

            SubscriptionsResource.ListRequest listSubscriptions = YouTubeFunctions.Service.Subscriptions.List("snippet");
            listSubscriptions.Order = SubscriptionsResource.ListRequest.OrderEnum.Alphabetical;
            listSubscriptions.Mine = true;
            listSubscriptions.MaxResults = 50;
            SubscriptionListResponse response = listSubscriptions.Execute();

            while (response.NextPageToken != null && !token.IsCancellationRequested)
            {
                result.AddRange(ConvertSubscriptionItems(response.Items.ToList()));
                listSubscriptions.PageToken = response.NextPageToken;
                response = listSubscriptions.Execute();
            }

            result.AddRange(ConvertSubscriptionItems(response.Items.ToList()));

            //Populate uploads playlists for subscriptions
            result.ForEach(p => p.PlaylistIdToWatch = GetChannelUploadsPlaylistId(p));

            return result;
        }

        private static List<Subscription> ConvertSubscriptionItems(List<Google.Apis.YouTube.v3.Data.Subscription> itemList)
        {
            List<Subscription> subscriptions = new List<Subscription>();

            foreach (Google.Apis.YouTube.v3.Data.Subscription item in itemList)
            {
                Subscription sub = new Subscription()
                {
                    ChannelId = item.Snippet.ResourceId.ChannelId,
                    Title = item.Snippet.Title
                };

                subscriptions.Add(sub);
            }

            return subscriptions;
        }

        public static Subscription GetPlaylistAsSubscription(string playlistId)
        {
            PlaylistsResource.ListRequest listRequest = Service.Playlists.List("snippet");
            listRequest.Id = playlistId;
            PlaylistListResponse response = listRequest.Execute();

            string channelId = response.Items.FirstOrDefault().Snippet.ChannelId;
            string playlistTitle = response.Items.FirstOrDefault().Snippet.Title;

            Subscription playlistSubscription = new Subscription();
            playlistSubscription.LastVideoPublishDate = DateTime.Now;
            playlistSubscription.ChannelId = channelId;
            playlistSubscription.PlaylistIdToWatch = playlistId;
            playlistSubscription.Title = playlistTitle;
            playlistSubscription.IsPlaylist = true;

            return playlistSubscription;
        }

        public static List<PlaylistItem> GetMostRecentUploads(Subscription sub, DateTime? sinceDate = null)
        {
            List<PlaylistItem> resultsByDate = new List<PlaylistItem>();
            if (!string.IsNullOrWhiteSpace(sub.PlaylistIdToWatch))
            {
                try
                {
                    PlaylistItemsResource.ListRequest listRequest = Service.PlaylistItems.List("snippet,status");
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

                    //Remove any that do not match the regex filter
                    if (!string.IsNullOrEmpty(sub.FilterRegex))
                        results.RemoveAll(p => !Regex.IsMatch(p.Snippet.Title, sub.FilterRegex));

                    //Check to see if any of the sub's private videos have change to public
                    foreach (string videoId in sub.PrivateVideosToWatch)
                    {
                        PlaylistItem matchingItem = results.Find(p => p.Snippet.ResourceId.VideoId == videoId);
                        if (matchingItem != null && matchingItem.Status.PrivacyStatus == "public")
                            privateToPublic.Add(matchingItem);
                    }

                    //Stop watching for private video status change if it is now in "privateToPublic"
                    sub.PrivateVideosToWatch.RemoveAll(p => privateToPublic.Find(o => o.Snippet.ResourceId.VideoId == p) != null);

                    List<PlaylistItem> recentPrivateVideos = results.Where(p => p.Status != null && p.Status.PrivacyStatus == "private").ToList();
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
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception when trying to get uploads: {ex.Message}");
                }
            }

            //Sometimes we sense a new video twice in "newUploads". Not sure why (if it's a YouTube API bug or my code), but this will prevent
            //notifying for that video twice (removes duplicate videoIds from the results list)
            resultsByDate = (from v in resultsByDate
                             group v by v.Snippet.ResourceId.VideoId into g
                             select g.First()).ToList();

            return resultsByDate;
        }

        public static string GetChannelUploadsPlaylistId(Subscription sub)
        {
            ChannelsResource.ListRequest listRequest = Service.Channels.List("contentDetails");
            listRequest.Id = sub.ChannelId;
            ChannelListResponse response = listRequest.Execute();

            if (response.Items.Count <= 0)
                return null;

            return response.Items.FirstOrDefault().ContentDetails.RelatedPlaylists.Uploads;
        }

        public static async Task DownloadYouTubeVideoAsync(string youTubeVideoId, string destinationFolder, CancellationToken token)
        {
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
        }
    }
}
