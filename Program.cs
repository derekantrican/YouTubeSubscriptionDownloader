using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YouTubeSubscriptionDownloader
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                if (args != null && args.Length > 0)
                {
                    if (args.ToList().Contains("start"))
                        Application.Run(new Form1(true));
                }
                else
                    Application.Run(new Form1());
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an unhandled exception. Please contact the developer and relay this information: \n\nMessage: " + ex?.Message);

                string crashPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "YouTube Subscription Downloader");
                string exceptionString = "";
                exceptionString = "[" + DateTime.Now + "] EXCEPTION MESSAGE: " + ex?.Message + Environment.NewLine + Environment.NewLine;
                exceptionString += "[" + DateTime.Now + "] INNER EXCEPTION: " + ex?.InnerException + Environment.NewLine + Environment.NewLine;
                exceptionString += "[" + DateTime.Now + "] STACK TRACE: " + ex?.StackTrace + Environment.NewLine + Environment.NewLine;
                File.AppendAllText(Path.Combine(crashPath, "CRASHREPORT (" + DateTime.Now.ToString("yyyy.MM.dd.HH.mm.ss") + ").log"), exceptionString);
            }
        }
    }
}
