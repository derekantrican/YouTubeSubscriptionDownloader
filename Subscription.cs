using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeSubscriptionDownloader
{
    public class Subscription
    {
        public Subscription()
        {
            PrivateVideosToWatch = new List<string>();
        }

        public string ChannelId { get; set; }
        public string Title { get; set; }
        public string PlaylistIdToWatch { get; set; }
        public DateTime LastVideoPublishDate { get; set; }
        public bool IsPlaylist { get; set; }
        public List<string> PrivateVideosToWatch { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
