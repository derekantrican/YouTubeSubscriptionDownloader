using PocketSharp;
using PocketSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace YouTubeSubscriptionDownloader
{
    public class Settings
    {

        private static string ApplicationName = "YouTube Subscription Downloader";
        private static string userSettingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationName);
        private static string settingsPath = Path.Combine(userSettingsPath, "Settings.xml");
        public static PocketClient pocketClient = new PocketClient("69847-fc525ffd3205de609a7429bf");
        public static Settings Instance = new Settings();

        #region Settings
        public string DownloadDirectory { get; set; }
        public string PreferredQuality { get; set; }
        public bool ShowNotifications { get; set; }
        public bool DownloadVideos { get; set; }
        public bool AddToPocket { get; set; }
        public string PocketAuthCode { get; set; }
        public bool SerializeSubscriptions { get; set; }

        private static Settings GetDefaultValues()
        {
            Settings defaultSettings = new Settings();

            defaultSettings.DownloadDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            defaultSettings.PreferredQuality = "Highest";
            defaultSettings.ShowNotifications = true;
            defaultSettings.DownloadVideos = true;
            defaultSettings.AddToPocket = false;
            defaultSettings.PocketAuthCode = "";
            defaultSettings.SerializeSubscriptions = false;

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
            }
        }
        #endregion Read Settings
    }
}
