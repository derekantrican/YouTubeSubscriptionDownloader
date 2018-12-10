using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YouTubeSubscriptionDownloader
{
    public partial class PlaylistManager : Form
    {
        YouTubeService service;

        public PlaylistManager(YouTubeService service, List<Subscription> exisitingPlaylists)
        {
            InitializeComponent();

            this.service = service;

            foreach (Subscription playlist in exisitingPlaylists)
                listBoxPlaylists.Items.Add(playlist);
        }

        public delegate void SubscriptionsUpdatedDelegate(List<Subscription> updatedSubscriptions);
        public event SubscriptionsUpdatedDelegate SubscriptionsUpdated;

        private void buttonSave_Click(object sender, EventArgs e)
        {
            List<Subscription> resultPlaylists = listBoxPlaylists.Items.Cast<Subscription>().ToList();
            SubscriptionsUpdated.Invoke(resultPlaylists);

            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void listBoxPlaylists_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int index = this.listBoxPlaylists.IndexFromPoint(new Point(e.X, e.Y));

                if (index < 0)
                    return;

                this.listBoxPlaylists.SelectedIndex = index;

                ContextMenu removeMenu = new ContextMenu();
                MenuItem removeItem = new MenuItem() { Text = "Remove" };
                removeItem.Click += (s, args) => { listBoxPlaylists.Items.RemoveAt(index); };
                removeMenu.MenuItems.Add(removeItem);
                removeMenu.Show((sender as ListBox), new Point(e.X, e.Y));

                return;
            }
        }

        private void listBoxPlaylists_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                listBoxPlaylists.Items.Remove(listBoxPlaylists.SelectedItem);
        }

        private void pictureBoxAdd_Click(object sender, EventArgs e)
        {
            if (IsValidPlaylist(textBoxPlaylistURL.Text))
            {
                if (listBoxPlaylists.Items.Cast<Subscription>().FirstOrDefault(p => textBoxPlaylistURL.Text.Contains(p.UploadsPlaylist)) != null)
                    MessageBox.Show("That playlist already exists in the list");
                else
                {
                    AddPlaylistToList(textBoxPlaylistURL.Text);
                    listBoxPlaylists.TopIndex = listBoxPlaylists.Items.Count - 1; //Scroll to bottom of list to show item was added
                    textBoxPlaylistURL.Clear();
                }
            }
            else
                MessageBox.Show("That is not a valid playlist URL or ID");
        }

        private bool IsValidPlaylist(string playlistURL)
        {
            string playlistID = Regex.Match(playlistURL, @"(PL|UU)[\w-]*").ToString();
            return !string.IsNullOrEmpty(playlistID);
        }

        private void AddPlaylistToList(string playlistURL)
        {
            string playlistID = Regex.Match(playlistURL, @"(PL|UU)[\w-]*").ToString();

            PlaylistsResource.ListRequest listRequest = service.Playlists.List("snippet");
            listRequest.Id = playlistID;
            PlaylistListResponse response = listRequest.Execute();

            string channelId = response.Items.FirstOrDefault().Snippet.ChannelId;
            string playlistTitle = response.Items.FirstOrDefault().Snippet.Title;

            Subscription playlistSubscription = new Subscription();
            playlistSubscription.LastVideoPublishDate = DateTime.Now;
            playlistSubscription.Id = channelId;
            playlistSubscription.UploadsPlaylist = playlistID;
            playlistSubscription.Title = playlistTitle;
            playlistSubscription.IsPlaylist = true;

            listBoxPlaylists.Items.Add(playlistSubscription);
        }
    }
}
