using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YouTubeSubscriptionDownloader
{
    public partial class SubscriptionManager : Form
    {
        public SubscriptionManager()
        {
            InitializeComponent();

            //Turn on double buffering for faster grid loading
            gridPlaylists.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(gridPlaylists, true, null);

            foreach (Subscription playlist in Common.TrackedSubscriptions)
                AddGridRow(playlist);

            PlaylistManager_Resize(null, null);
        }

        private void AddGridRow(Subscription playlist)
        {
            int newRowIndex = gridPlaylists.Rows.Add(playlist.Title, playlist.FilterRegex);
            gridPlaylists.Rows[newRowIndex].Tag = playlist;
            gridPlaylists.Rows[newRowIndex].Cells[0].ToolTipText = playlist.GetPlaylistUrl();
        }

        private List<Subscription> GetGridSubs()
        {
            List<Subscription> subsInGrid = new List<Subscription>();
            foreach (DataGridViewRow row in gridPlaylists.Rows)
            {
                Subscription sub = row.Tag as Subscription;
                sub.FilterRegex = row.Cells[1].Value == null ? "" : row.Cells[1].Value.ToString();

                subsInGrid.Add(sub);
            }

            return subsInGrid;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            Common.TrackedSubscriptions.Clear();
            Common.TrackedSubscriptions.AddRange(GetGridSubs());
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
                openItem.Click += (s, args) => { Process.Start((gridPlaylists.Rows[index].Tag as Subscription).GetPlaylistUrl()); };
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
            if (Settings.Instance.FirstTimeNotifySyncSetting)
            {
                MessageBox.Show("Tip: In the Settings, you can turn on \"Sync subscriptions with YouTube\" where the program will " +
                                "automatically start tracking any new channels you subscribe to on YouTube (and stop tracking " +
                                "unsubscribed ones)");
                Settings.Instance.FirstTimeNotifySyncSetting = false;
            }

            Cursor.Current = Cursors.WaitCursor;

            List<Subscription> existingSubs = GetGridSubs();

            Task<List<Subscription>> getUserSubs = Task.Run(() => YouTubeFunctions.GetUserSubscriptionsAsync(CancellationToken.None, true));
            getUserSubs.Wait();
            List<Subscription> userSubs = getUserSubs.Result;

            foreach (Subscription sub in userSubs)
            {
                //Make sure it is not already in the list
                if (existingSubs.FirstOrDefault(p => p.PlaylistIdToWatch == sub.PlaylistIdToWatch) == null)
                    AddGridRow(sub);
            }

            gridPlaylists.FirstDisplayedScrollingRowIndex = gridPlaylists.RowCount - 1; //Scroll to bottom of list to show item was added
            Cursor.Current = Cursors.Default;
        }

        private void pictureBoxAdd_Click(object sender, EventArgs e)
        {
            if (IsValidPlaylist(textBoxPlaylistURL.Text))
            {
                List<Subscription> playlists = GetGridSubs();
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
            return !string.IsNullOrEmpty(playlistURL) && Regex.IsMatch(playlistURL, @"(PL|UU|UC)[\w-]*|/user/");
        }

        private string GetChannelId(string channelUrl)
        {
            using (WebClient client = new WebClient())
            {
                string htmlCode = client.DownloadString(channelUrl);
                return Regex.Match(htmlCode, "\"externalId\":\"(?<channelId>[^\"]*)\"|<meta itemprop=\"channelId\" content=\"(?<channelId>[^\"]*)\">").Groups["channelId"].Value;
            }
        }

        private void AddPlaylistToList(string playlistURL)
        {
            string playlistID = Regex.Match(playlistURL, @"(PL|UU|UC)[\w-]*").Value;

            if (playlistID.StartsWith("UC"))
                playlistID = playlistID.Replace("UC", "UU");
            else if (playlistURL.Contains("/user/"))
            {
                playlistID = GetChannelId(playlistURL).Replace("UC", "UU");
            }

            if (string.IsNullOrEmpty(playlistID))
            {
                MessageBox.Show($"Could not find a playlist ID for {playlistURL}");
                return;
            }

            Subscription playlistSubscription = YouTubeFunctions.GetPlaylistAsSubscription(playlistID);
            playlistSubscription.FilterRegex = textBoxRegex.Text;

            AddGridRow(playlistSubscription);
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
