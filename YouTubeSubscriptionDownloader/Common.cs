using PocketSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace YouTubeSubscriptionDownloader
{
    public static class Common
    {
        public static string ApplicationName = "YouTube Subscription Downloader";
        public static string UserSettings = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationName);
        public static string CredentialsPath = Path.Combine(UserSettings, "Credentials");
        public static string SubscriptionsPath = Path.Combine(UserSettings, "Subscriptions.xml");

        public static List<Subscription> TrackedSubscriptions = new List<Subscription>();

        private const string PocketConsumerKey = "69847-fc525ffd3205de609a7429bf";

        public static bool HasInternetConnection()
        {
            try
            {
                using (WebClient client = new WebClient())
                using (client.OpenRead("http://www.youtube.com"))
                    return true;
            }
            catch
            {
                //Log("!!NO INTERNET CONNECTION!!");
                return false;
            }
        }

        public static void SerializeSubscriptions(string overrideSerializationPath = null)
        {
            string serializationPath = string.IsNullOrEmpty(overrideSerializationPath) ? SubscriptionsPath : overrideSerializationPath;
            if (File.Exists(serializationPath))
                File.Delete(serializationPath);

            using (TextWriter writer = new StreamWriter(serializationPath))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Subscription>));
                xmlSerializer.Serialize(writer, TrackedSubscriptions);
            }

            if (!string.IsNullOrEmpty(overrideSerializationPath))
                MessageBox.Show("Subscriptions serialized to " + serializationPath);
        }

        public static void DeserializeSubscriptions()
        {
            if (File.Exists(SubscriptionsPath))
            {
                using (FileStream fileStream = new FileStream(SubscriptionsPath, FileMode.Open))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Subscription>));
                    TrackedSubscriptions.AddRange((List<Subscription>)xmlSerializer.Deserialize(fileStream));
                }
            }
        }

        public static void InitializePocket()
        {
            if (Settings.Instance.AddToPocket && Settings.Instance.PocketAuthCode != "")
                Settings.pocketClient = new PocketClient(PocketConsumerKey, Settings.Instance.PocketAuthCode, "https://getpocket.com/a/queue/");
        }
    }
}
