using PocketSharp;
using PocketSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public static string DownloadDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static string PreferredQuality = "Highest";
        public static bool ShowNotifications = true;
        public static bool DownloadVideos = true;
        public static bool AddToPocket = false;
        public static string PocketAuthCode = "";
        public static bool SerializeSubscriptions = false;

        public static void SaveSettings()
        {
            SaveSpecificSetting(DownloadDirectory, "DownloadDirectory");
            SaveSpecificSetting(PreferredQuality, "PreferredQuality");
            SaveSpecificSetting(ShowNotifications, "ShowNotifications");
            SaveSpecificSetting(DownloadVideos, "DownloadVideos");
            SaveSpecificSetting(AddToPocket, "AddToPocket");
            SaveSpecificSetting(PocketAuthCode, "PocketAuthCode");
            SaveSpecificSetting(SerializeSubscriptions, "SerializeSubscriptions");
        }

        public static void ReadSettings()
        {
            DownloadDirectory = (string)ReadSpecificSetting(typeof(string), "DownloadDirectory", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            PreferredQuality = (string)ReadSpecificSetting(typeof(string), "PreferredQuality", "Highest");
            ShowNotifications = (bool)ReadSpecificSetting(typeof(bool), "ShowNotifications", true);
            DownloadVideos = (bool)ReadSpecificSetting(typeof(bool), "DownloadVideos", true);
            AddToPocket = (bool)ReadSpecificSetting(typeof(bool), "AddToPocket", false);
            PocketAuthCode = (string)ReadSpecificSetting(typeof(string), "PocketAuthCode", "");
            SerializeSubscriptions = (bool)ReadSpecificSetting(typeof(bool), "SerializeSubscriptions", false);
        }

        private static void SaveSpecificSetting(object setting, string settingTag)
        {
            XmlSerializer serializer = new XmlSerializer(setting.GetType());
            StringWriter writer = new StringWriter();
            serializer.Serialize(writer, setting);

            if (!File.Exists(settingsPath))
                CreateDefaultXml();

            XDocument document = XDocument.Load(settingsPath);
            XElement element = null;

            try
            {
                element = document.Descendants("Setting").Where(x => x.Attribute("name").Value.Equals(settingTag)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception saving setting: " + ex.Message);
            }

            if (element == null)
            {
                element = new XElement("Setting",
                          new XAttribute("name", settingTag),
                          writer.ToString());
                document.Element("Settings").Add(element);
            }
            else
                element.Value = writer.ToString();

            document.Element("Settings").Attribute("updated").Value = DateTime.Now.ToString("%M/%d/yyyy HH:mm");
            document.Save(settingsPath);
        }

        private static object ReadSpecificSetting(Type expectedType, string settingTag, object defaultValue = null)
        {
            XmlSerializer serializer = new XmlSerializer(expectedType);
            StringReader reader;

            if (!File.Exists(settingsPath))
                CreateDefaultXml();

            XDocument document = XDocument.Load(settingsPath);
            XElement setting = document.Descendants("Setting").Where(x => x.Attribute("name").Value.Equals(settingTag)).FirstOrDefault();

            if (setting != null)
            {
                reader = new StringReader(setting.Value);
                return serializer.Deserialize(reader); //Ideally, we would cast this back to expectedType before returning
            }

            SaveSpecificSetting(defaultValue, settingTag);

            return defaultValue;
        }

        private static void CreateDefaultXml()
        {
            string xmlContents = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + Environment.NewLine +
                                 "<Settings updated=\"" + DateTime.Now.ToString("%M/%d/yyyy HH:mm") + "\">" + Environment.NewLine +
                                 "</Settings>";

            File.WriteAllText(settingsPath, xmlContents);
        }
    }
}
