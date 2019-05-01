using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
                Add(playlist);
        }

        private void Add(Subscription playlist)
        {
            ListViewItem item = new ListViewItem()
            {
                Text = playlist.Title,
                Tag = playlist
            };

            item.SubItems.Add(playlist.FilterRegex);

            listViewPlaylists.Items.Add(item);
        }

        public delegate void SubscriptionsUpdatedDelegate(List<Subscription> updatedSubscriptions);
        public event SubscriptionsUpdatedDelegate SubscriptionsUpdated;

        private void buttonSave_Click(object sender, EventArgs e)
        {
            List<Subscription> resultPlaylists = new List<Subscription>();
            foreach (ListViewItem item in listViewPlaylists.Items)
                resultPlaylists.Add(item.Tag as Subscription);

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
                Point p = this.listViewPlaylists.PointToClient(new Point(e.X, e.Y));
                int index = this.listViewPlaylists.GetItemAt(p.X, p.Y).Index;

                if (index < 0)
                    return;

                this.listViewPlaylists.Items[index].Selected = true;

                ContextMenu contexMenu = new ContextMenu();
                MenuItem openItem = new MenuItem() { Text = "Open" };
                openItem.Click += (s, args) => { Process.Start("https://www.youtube.com/playlist?list=" + (listViewPlaylists.Items[index].Tag as Subscription).PlaylistIdToWatch); };
                contexMenu.MenuItems.Add(openItem);

                MenuItem removeItem = new MenuItem() { Text = "Remove" };
                removeItem.Click += (s, args) => { listViewPlaylists.Items.RemoveAt(index); };
                contexMenu.MenuItems.Add(removeItem);

                contexMenu.Show((sender as ListBox), new Point(e.X, e.Y));

                return;
            }
        }

        private void listBoxPlaylists_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                listViewPlaylists.Items.Remove(listViewPlaylists.SelectedItems[0]);
        }

        private void pictureBoxAdd_Click(object sender, EventArgs e)
        {
            if (IsValidPlaylist(textBoxPlaylistURL.Text))
            {
                List<Subscription> playlists = new List<Subscription>();
                foreach (ListViewItem item in listViewPlaylists.Items)
                    playlists.Add(item.Tag as Subscription);

                if (playlists.FirstOrDefault(p => textBoxPlaylistURL.Text.Contains(p.PlaylistIdToWatch)) != null)
                    MessageBox.Show("That playlist already exists in the list");
                else
                {
                    AddPlaylistToList(textBoxPlaylistURL.Text);
                    listViewPlaylists.EnsureVisible(listViewPlaylists.Items.Count - 1); //Scroll to bottom of list to show item was added
                    textBoxPlaylistURL.Clear();
                    textBoxRegex.Clear();
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
            playlistSubscription.ChannelId = channelId;
            playlistSubscription.PlaylistIdToWatch = playlistID;
            playlistSubscription.Title = playlistTitle;
            playlistSubscription.IsPlaylist = true;
            playlistSubscription.FilterRegex = textBoxRegex.Text;

            Add(playlistSubscription);
        }
    }
}
