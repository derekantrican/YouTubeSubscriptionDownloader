using PocketSharp;
using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace YouTubeSubscriptionDownloader
{
    public class Settings
    {

        private static string ApplicationName = "YouTube Subscription Downloader";
        private static string userSettingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationName);
        private static string settingsPath = Path.Combine(userSettingsPath, "Settings.xml");
        public static PocketClient pocketClient = new PocketClient("69847-fc525ffd3205de609a7429bf");
        public static Settings Instance = GetDefaultValues();

        #region Settings
        public string DownloadDirectory { get; set; }
        public string PreferredQuality { get; set; }
        public bool ShowNotifications { get; set; }
        public bool ShowThumbnailInNotification { get; set; }
        public bool DownloadVideos { get; set; }
        public bool AddToPocket { get; set; }
        public string PocketAuthCode { get; set; }
        public bool SerializeSubscriptions { get; set; }
        public int IterationFrequency { get; set; } //In minutes
        public bool StartIterationsOnStartup { get; set; }
        public bool NotificationClickOpensYouTubeVideo { get; set; }

        private static Settings GetDefaultValues()
        {
            Settings defaultSettings = new Settings();

            defaultSettings.DownloadDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            defaultSettings.PreferredQuality = "Highest";
            defaultSettings.ShowNotifications = true;
            defaultSettings.ShowThumbnailInNotification = true;
            defaultSettings.DownloadVideos = true;
            defaultSettings.AddToPocket = false;
            defaultSettings.PocketAuthCode = "";
            defaultSettings.SerializeSubscriptions = false;
            defaultSettings.IterationFrequency = 5;
            defaultSettings.StartIterationsOnStartup = false;
            defaultSettings.NotificationClickOpensYouTubeVideo = true;

            return defaultSettings;
        }
        #endregion Settings

        #region Save Settings
        public static void SaveSettings()
        {
            TextWriter writer = new StreamWriter(settingsPath);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Settings));
            xmlSerializer.Serialize(writer, Instance);
            writer.Close();
        }
        #endregion Save Settings

        #region Read Settings
        public static void ReadSettings()
        {
            try
            {
                using (FileStream fileStream = new FileStream(settingsPath, FileMode.Open))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(Settings));
                    Instance = (Settings)xmlSerializer.Deserialize(fileStream);
                }

                foreach (PropertyInfo prop in typeof(Settings).GetProperties())
                {
                    if (prop.GetValue(Instance) == null) //Check to see if any of the properties are null
                        prop.SetValue(Instance, prop.GetValue(GetDefaultValues())); //Replace that value with the default value
                }
            }
            catch (Exception ex)
            {
                Instance = GetDefaultValues();

                //Dump Exception
                string crashPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "YouTube Subscription Downloader");
                string exceptionString = "";
                exceptionString = "[" + DateTime.Now + "] EXCEPTION MESSAGE: " + ex?.Message + Environment.NewLine + Environment.NewLine;
                exceptionString += "[" + DateTime.Now + "] INNER EXCEPTION: " + ex?.InnerException + Environment.NewLine + Environment.NewLine;
                exceptionString += "[" + DateTime.Now + "] STACK TRACE: " + ex?.StackTrace + Environment.NewLine + Environment.NewLine;
                File.AppendAllText(Path.Combine(crashPath, "CRASHREPORT (" + DateTime.Now.ToString("yyyy.MM.dd.HH.mm.ss") + ").log"), exceptionString);
            }
        }
        #endregion Read Settings
    }
}
