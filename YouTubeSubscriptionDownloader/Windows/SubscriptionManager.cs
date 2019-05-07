using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
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

        private void buttonSave_Click(object sender, EventArgs e)
        {
            Common.TrackedSubscriptions.Clear();
            foreach (DataGridViewRow item in gridPlaylists.Rows)
            {
                Subscription sub = item.Tag as Subscription;
                Common.TrackedSubscriptions.Add(sub);
            }

            Common.SerializeSubscriptions();

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
                openItem.Click += (s, args) => { Process.Start(Common.YOUTUBEPLAYLISTBASEURL + (gridPlaylists.Rows[index].Tag as Subscription).PlaylistIdToWatch); };
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

        private void ButtonGetYTSubscriptions_Click(object sender, EventArgs e)
        {
            //Todo: maybe add an alert (only for the first time) that there is a setting to do this automatically
            Cursor.Current = Cursors.WaitCursor;

            List<Subscription> existingSubs = new List<Subscription>();
            foreach (DataGridViewRow item in gridPlaylists.Rows)
                existingSubs.Add(item.Tag as Subscription);

            List<Subscription> userSubs = YouTubeFunctions.GetUserSubscriptionsAsync(CancellationToken.None).Result;
            foreach (Subscription sub in userSubs)
            {
                //Make sure it is not already in the list
                if (existingSubs.FirstOrDefault(p => p.PlaylistIdToWatch == sub.PlaylistIdToWatch) == null)
                    Add(sub);
            }

            gridPlaylists.FirstDisplayedScrollingRowIndex = gridPlaylists.RowCount - 1; //Scroll to bottom of list to show item was added
            Cursor.Current = Cursors.Default;
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

            gridPlaylists.Height = this.Height - 227;
            gridPlaylists.Width = this.Width - 22;
            gridPlaylists.Columns[0].Width = (int)(gridPlaylists.Width * 0.75) - 5;
            gridPlaylists.Columns[1].Width = -2; //AutoSize

            groupBoxManualSub.Width = this.Width - 25;
            textBoxPlaylistURL.Width = this.Width - 139;
            textBoxRegex.Width = this.Width - 179;

            buttonGetYTSubscriptions.Top = this.Height - 221;
            buttonGetYTSubscriptions.Left = this.Width - 206;
            groupBoxManualSub.Top = this.Height - 180;
            pictureBoxAdd.Left = groupBoxManualSub.Width - 26;
        }
    }
}
