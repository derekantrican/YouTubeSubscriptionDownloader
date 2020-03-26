using PocketSharp;
using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace YouTubeSubscriptionDownloader
{
    public class Settings
    {
        public static PocketClient PocketClient = new PocketClient(Common.POCKETCONSUMERKEY);
        public static Settings Instance = GetDefaultValues();

        #region Settings
        public string DownloadDirectory { get; set; }
        public string PreferredQuality { get; set; }
        public bool ShowNotifications { get; set; }
        public bool ShowThumbnailInNotification { get; set; }
        public bool DownloadVideos { get; set; }
        public bool AddToPocket { get; set; }
        public string PocketAuthCode { get; set; }
        public bool CheckForMissedUploads { get; set; }
        public int IterationFrequency { get; set; } //In minutes
        public bool StartIterationsOnStartup { get; set; }
        public bool NotificationClickOpensYouTubeVideo { get; set; }
        public bool SyncSubscriptionsWithYouTube { get; set; }
        public bool FirstTimeShowSubscriptionManager { get; set; }
        public bool FirstTimeNotifySyncSetting { get; set; }

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
            defaultSettings.CheckForMissedUploads = false;
            defaultSettings.IterationFrequency = 5;
            defaultSettings.StartIterationsOnStartup = false;
            defaultSettings.NotificationClickOpensYouTubeVideo = true;
            defaultSettings.SyncSubscriptionsWithYouTube = false;
            defaultSettings.FirstTimeShowSubscriptionManager = true;
            defaultSettings.FirstTimeNotifySyncSetting = true;

            return defaultSettings;
        }
        #endregion Settings

        #region Save Settings
        public static void SaveSettings()
        {
            TextWriter writer = new StreamWriter(Common.SettingsPath);
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
                if (!File.Exists(Common.SettingsPath))
                {
                    Instance = GetDefaultValues();
                    return;
                }

                using (FileStream fileStream = new FileStream(Common.SettingsPath, FileMode.Open))
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
                Common.HandleException(ex);
            }
        }
        #endregion Read Settings
    }
}
