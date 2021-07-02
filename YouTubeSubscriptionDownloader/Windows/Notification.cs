using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace YouTubeSubscriptionDownloader
{
    public partial class Notification : Form
    {
        private static readonly List<Notification> OpenNotifications = new List<Notification>();
        private Timer lifeTimer = new Timer();
        private string videoURL = "";
        public Screen Screen;

        public Notification(string notificationTitle, string notificationMessage, string imageURL = null, string videoURL = "", Screen overrideScreen = null)
        {
            InitializeComponent();

            //Subscribe to all click events
            this.Click += NotificationClick;
            notificationImage.Click += NotificationClick;
            pictureBoxIcon.Click += NotificationClick;
            labelTitle.Click += NotificationClick;
            labelMessage.Click += NotificationClick;

            this.videoURL = videoURL;

            labelTitle.Text = notificationTitle;
            labelMessage.Text = notificationMessage;

            if (!string.IsNullOrEmpty(imageURL))
                notificationImage.Load(imageURL);
            else
            {
                int notificationImageHeight = notificationImage.Height;
                this.Controls.Remove(notificationImage);
                this.Height -= notificationImageHeight;

                //Move the rest of the controls up
                foreach (Control control in this.Controls)
                {
                    control.Location = new Point(control.Location.X, control.Location.Y - notificationImageHeight);
                }
            }

            Screen = overrideScreen ?? Settings.ScreenFromSetting();

            lifeTimer.Interval = 5000; //Show the notification for 5 seconds
            lifeTimer.Tick += LifeTimer_Tick;

            string notificationSoundPath = @"C:\Windows\media\Windows Notify System Generic.wav";
            using (var player = new System.Media.SoundPlayer(notificationSoundPath))
                player.Play();
        }

        private void LifeTimer_Tick(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Notification_Load(object sender, EventArgs e)
        {
            Debug.WriteLine($"Showing notification on screen {Screen.DeviceName}");

            // Display the form just above the system tray.
            this.Location = new Point(Screen.WorkingArea.X + Screen.WorkingArea.Width - this.Width, Screen.WorkingArea.Y + Screen.WorkingArea.Height - this.Height);

            // Move each open form upwards to make room for this one
            foreach (Notification openForm in OpenNotifications)
            {
                if (openForm.Screen == this.Screen)
                {
                    openForm.Top -= Height + (int)(Screen.WorkingArea.Height * 0.01); //Padding of 1% of screen height between multiple notifications
                }
            }

            OpenNotifications.Add(this);
            lifeTimer.Start();
        }

        private void NotificationClick(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(videoURL) && Settings.Instance.NotificationClickOpensYouTubeVideo)
                Process.Start(videoURL);

            this.Close();
        }

        //Don't steal focus when showing
        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }
    }
}
