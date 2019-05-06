using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeSubscriptionDownloader
{
    public static class Common
    {
        public static string ApplicationName = "YouTube Subscription Downloader";
        public static string UserSettings = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationName);
        public static string CredentialsPath = Path.Combine(UserSettings, "Credentials");
    }
}
