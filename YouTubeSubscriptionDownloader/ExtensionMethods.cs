using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeSubscriptionDownloader
{
    public static class ExtensionMethods
    {
        public static string GetAvailableThumbnailUrl(this ThumbnailDetails thumbnails)
        {
            if (thumbnails != null)
            {
                if (thumbnails.Standard != null)
                    return thumbnails.Standard.Url;
                else if (thumbnails.Medium != null)
                    return thumbnails.Medium.Url;
                else if (thumbnails.High != null)
                    return thumbnails.High.Url;
            }

            return null;
        }
    }
}
