using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeSubscriptionDownloader
{
    public class Subscription
    {
        public string ChannelId { get; set; }
        public string Title { get; set; }
        public string PlaylistIdToWatch { get; set; }
        public DateTime LastVideoPublishDate { get; set; }
        public bool IsPlaylist { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
