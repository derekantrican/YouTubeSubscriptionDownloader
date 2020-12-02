﻿using Google;
using PocketSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
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
        public static string SettingsPath = Path.Combine(UserSettings, "Settings.xml");

        public const string YOUTUBEVIDEOBASEURL = "https://www.youtube.com/watch?v=";
        public const string YOUTUBEPLAYLISTBASEURL = "https://www.youtube.com/playlist?list=";
        public const string POCKETCONSUMERKEY = "69847-fc525ffd3205de609a7429bf";

        public static List<Subscription> TrackedSubscriptions = new List<Subscription>();

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

        public static void DeserializeSubscriptions(string subscriptionsPath = null)
        {
            if (File.Exists(subscriptionsPath ?? SubscriptionsPath))
            {
                using (FileStream fileStream = new FileStream(subscriptionsPath ?? SubscriptionsPath, FileMode.Open))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Subscription>));
                    TrackedSubscriptions = (List<Subscription>)xmlSerializer.Deserialize(fileStream);
                }
            }
        }

        public static void InitializePocket()
        {
            if (Settings.Instance.AddToPocket && Settings.Instance.PocketAuthCode != "")
                Settings.PocketClient = new PocketClient(POCKETCONSUMERKEY, Settings.Instance.PocketAuthCode, "https://getpocket.com/a/queue/");
        }

        public static void HandleException(Exception ex)
        {
            if (ex is WebException ||
                ex.InnerException is WebException ||
                (ex is TaskCanceledException && (ex as TaskCanceledException).CancellationToken.IsCancellationRequested == false) || //HttpClient timeout
                ex is GoogleApiException && (ex as GoogleApiException).HttpStatusCode == HttpStatusCode.InternalServerError ||
                ex.Message.Contains("That’s an error")) //Handles the Google error pages like https://i.imgur.com/lFDKFro.png (Todo: in the future handle this with exponential backoff)
            {
                Console.WriteLine("There was a problem contacting YouTube");
            }
            else if (ex is GoogleApiException && (ex as GoogleApiException).HttpStatusCode == HttpStatusCode.Forbidden)
                Console.WriteLine("API quota exceeded. Will retry on next iteration.");
            else
                DumpException(ex);
        }

        public static void DumpException(Exception ex)
        {
            string exceptionString = "";
            exceptionString += $"[{DateTime.Now}] EXCEPTION TYPE: {ex?.GetType()}\n\n";
            exceptionString += $"[{DateTime.Now}] EXCEPTION MESSAGE: {ex?.Message}\n\n";
            exceptionString += $"[{DateTime.Now}] INNER EXCEPTION: {ex?.InnerException}\n\n";
            exceptionString += $"[{DateTime.Now}] STACK TRACE: {ex?.StackTrace}\n\n";
            File.AppendAllText(Path.Combine(UserSettings, "CRASHREPORT (" + DateTime.Now.ToString("yyyy.MM.dd.HH.mm.ss") + ").log"), exceptionString);

            MessageBox.Show("There was an unhandled exception. Please contact the developer and relay this information: \n\nMessage: " + ex?.Message);
        }
    }
}
