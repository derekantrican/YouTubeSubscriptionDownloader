using System;
using System.Collections.Generic;

namespace YouTubeSubscriptionDownloader
{
    [Serializable]
    public class Subscription
    {
        public Subscription()
        {
            PrivateVideosToWatch = new List<string>();
        }

        public string ChannelId { get; set; }
        public string Title { get; set; }
        public string ThumbnailUrl { get; set; }
        public string PlaylistIdToWatch { get; set; }
        public DateTime LastVideoPublishDate { get; set; }
        public string FilterRegex { get; set; }
        public bool IsPlaylist { get; set; }
        public bool IsPlaylistUploadsPlaylist { get; set; }
        public List<string> PrivateVideosToWatch { get; set; }

        public override string ToString()
        {
            return Title;
        }

        public string GetPlaylistUrl()
        {
            return $"{Common.YOUTUBEPLAYLISTBASEURL}{PlaylistIdToWatch}";
        }
    }
}
