using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace YouTubeSubscriptionDownloader
{
    public partial class PlaylistManager : Form
    {
        public PlaylistManager()
        {
            InitializeComponent();

            //Turn on double buffering for faster grid loading
            gridPlaylists.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(gridPlaylists, true, null);

            foreach (Subscription playlist in Common.TrackedSubscriptions)
                Add(playlist);

            PlaylistManager_Resize(null, null);
        }

        private void Add(Subscription playlist)
        {
            int newRowIndex = gridPlaylists.Rows.Add(playlist.Title, playlist.FilterRegex);
            gridPlaylists.Rows[newRowIndex].Tag = playlist;
        }

        public delegate void SubscriptionsUpdatedDelegate(List<Subscription> updatedSubscriptions);
        public event SubscriptionsUpdatedDelegate SubscriptionsUpdated;

        private void buttonSave_Click(object sender, EventArgs e)
        {
            List<Subscription> resultPlaylists = new List<Subscription>();
            foreach (DataGridViewRow item in gridPlaylists.Rows)
                resultPlaylists.Add(item.Tag as Subscription);

            SubscriptionsUpdated.Invoke(resultPlaylists); //Todo: this takes a long time

            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void GridPlaylists_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int index = this.gridPlaylists.HitTest(e.X, e.Y).RowIndex;

                if (index < 0)
                    return;

                this.gridPlaylists.ClearSelection();
                this.gridPlaylists.Rows[index].Selected = true;

                ContextMenu contexMenu = new ContextMenu();
                MenuItem openItem = new MenuItem() { Text = "Open" };
                openItem.Click += (s, args) => { Process.Start("https://www.youtube.com/playlist?list=" + (gridPlaylists.Rows[index].Tag as Subscription).PlaylistIdToWatch); };
                contexMenu.MenuItems.Add(openItem);

                MenuItem removeItem = new MenuItem() { Text = "Remove" };
                removeItem.Click += (s, args) => { gridPlaylists.Rows.RemoveAt(index); };
                contexMenu.MenuItems.Add(removeItem);

                contexMenu.Show((sender as DataGridView), new Point(e.X, e.Y));

                return;
            }
        }

        private void GridPlaylists_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                gridPlaylists.Rows.Remove(gridPlaylists.SelectedRows[0]);
        }

        private void pictureBoxAdd_Click(object sender, EventArgs e)
        {
            if (IsValidPlaylist(textBoxPlaylistURL.Text))
            {
                List<Subscription> playlists = new List<Subscription>();
                foreach (DataGridViewRow item in gridPlaylists.Rows)
                    playlists.Add(item.Tag as Subscription);

                if (playlists.FirstOrDefault(p => textBoxPlaylistURL.Text.Contains(p.PlaylistIdToWatch)) != null)
                    MessageBox.Show("That playlist already exists in the list");
                else
                {
                    AddPlaylistToList(textBoxPlaylistURL.Text);
                    gridPlaylists.FirstDisplayedScrollingRowIndex = gridPlaylists.RowCount - 1; //Scroll to bottom of list to show item was added
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

            Subscription playlistSubscription = YouTubeFunctions.GetPlaylistAsSubscription(playlistID);
            playlistSubscription.FilterRegex = textBoxRegex.Text;

            Add(playlistSubscription);
        }

        private void PlaylistManager_Resize(object sender, EventArgs e)
        {
            //Do resizing/repositioning of all contents in the window

            gridPlaylists.Height = this.Height - 189;
            gridPlaylists.Width = this.Width - 22;
            gridPlaylists.Columns[0].Width = (int)(gridPlaylists.Width * 0.75) - 5;
            gridPlaylists.Columns[1].Width = -2; //AutoSize

            pictureBoxAdd.Left = this.Width - 48;
            textBoxPlaylistURL.Width = this.Width - 135;
            textBoxRegex.Width = this.Width - 175;

            labelPlaylistURL.Top = this.Height - 180;
            textBoxPlaylistURL.Top = this.Height - 183;
            pictureBoxAdd.Top = this.Height - 183;

            labelRegex.Top = this.Height - 157;
            textBoxRegex.Top = this.Height - 160;
        }
    }
}
