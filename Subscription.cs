using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeSubscriptionDownloader
{
    public class Subscription
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string UploadsPlaylist { get; set; }
        public DateTime LastVideoPublishDate { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
