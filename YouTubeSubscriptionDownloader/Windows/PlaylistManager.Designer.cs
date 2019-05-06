namespace YouTubeSubscriptionDownloader
{
    partial class PlaylistManager
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlaylistManager));
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelPlaylistOverview = new System.Windows.Forms.Label();
            this.labelPlaylistURL = new System.Windows.Forms.Label();
            this.textBoxPlaylistURL = new System.Windows.Forms.TextBox();
            this.pictureBoxAdd = new System.Windows.Forms.PictureBox();
            this.listViewPlaylists = new System.Windows.Forms.ListView();
            this.itemText = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.itemRegex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.textBoxRegex = new System.Windows.Forms.TextBox();
            this.labelRegex = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAdd)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSave.Location = new System.Drawing.Point(613, 4);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 3;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(694, 4);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // labelPlaylistOverview
            // 
            this.labelPlaylistOverview.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelPlaylistOverview.Location = new System.Drawing.Point(0, 0);
            this.labelPlaylistOverview.Name = "labelPlaylistOverview";
            this.labelPlaylistOverview.Size = new System.Drawing.Size(781, 32);
            this.labelPlaylistOverview.TabIndex = 4;
            this.labelPlaylistOverview.Text = "Here you can define a list of playlists to be checked for new uploads alongside y" +
    "our user subscriptions";
            // 
            // labelPlaylistURL
            // 
            this.labelPlaylistURL.AutoSize = true;
            this.labelPlaylistURL.Location = new System.Drawing.Point(8, 459);
            this.labelPlaylistURL.Name = "labelPlaylistURL";
            this.labelPlaylistURL.Size = new System.Drawing.Size(67, 13);
            this.labelPlaylistURL.TabIndex = 5;
            this.labelPlaylistURL.Text = "Playlist URL:";
            // 
            // textBoxPlaylistURL
            // 
            this.textBoxPlaylistURL.Location = new System.Drawing.Point(81, 456);
            this.textBoxPlaylistURL.Name = "textBoxPlaylistURL";
            this.textBoxPlaylistURL.Size = new System.Drawing.Size(662, 20);
            this.textBoxPlaylistURL.TabIndex = 6;
            // 
            // pictureBoxAdd
            // 
            this.pictureBoxAdd.Image = global::YouTubeSubscriptionDownloader.Properties.Resources.plus_black_symbol;
            this.pictureBoxAdd.Location = new System.Drawing.Point(749, 456);
            this.pictureBoxAdd.Name = "pictureBoxAdd";
            this.pictureBoxAdd.Size = new System.Drawing.Size(20, 20);
            this.pictureBoxAdd.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxAdd.TabIndex = 7;
            this.pictureBoxAdd.TabStop = false;
            this.pictureBoxAdd.Click += new System.EventHandler(this.pictureBoxAdd_Click);
            // 
            // listViewPlaylists
            // 
            this.listViewPlaylists.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.itemText,
            this.itemRegex});
            this.listViewPlaylists.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewPlaylists.Location = new System.Drawing.Point(3, 0);
            this.listViewPlaylists.MultiSelect = false;
            this.listViewPlaylists.Name = "listViewPlaylists";
            this.listViewPlaylists.Size = new System.Drawing.Size(775, 429);
            this.listViewPlaylists.TabIndex = 8;
            this.listViewPlaylists.UseCompatibleStateImageBehavior = false;
            this.listViewPlaylists.View = System.Windows.Forms.View.Details;
            // 
            // itemText
            // 
            this.itemText.Text = "Playlist";
            this.itemText.Width = 245;
            // 
            // itemRegex
            // 
            this.itemRegex.Text = "Search (regex)";
            this.itemRegex.Width = 150;
            // 
            // textBoxRegex
            // 
            this.textBoxRegex.Location = new System.Drawing.Point(121, 479);
            this.textBoxRegex.Name = "textBoxRegex";
            this.textBoxRegex.Size = new System.Drawing.Size(622, 20);
            this.textBoxRegex.TabIndex = 9;
            // 
            // labelRegex
            // 
            this.labelRegex.AutoSize = true;
            this.labelRegex.Location = new System.Drawing.Point(8, 482);
            this.labelRegex.Name = "labelRegex";
            this.labelRegex.Size = new System.Drawing.Size(107, 13);
            this.labelRegex.TabIndex = 10;
            this.labelRegex.Text = "Filter regex (optional):";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.buttonCancel);
            this.panel1.Controls.Add(this.buttonSave);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 561);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(781, 39);
            this.panel1.TabIndex = 11;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.listViewPlaylists);
            this.panel2.Controls.Add(this.labelPlaylistURL);
            this.panel2.Controls.Add(this.labelRegex);
            this.panel2.Controls.Add(this.textBoxPlaylistURL);
            this.panel2.Controls.Add(this.textBoxRegex);
            this.panel2.Controls.Add(this.pictureBoxAdd);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 32);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(781, 529);
            this.panel2.TabIndex = 12;
            // 
            // PlaylistManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(781, 600);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.labelPlaylistOverview);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(420, 349);
            this.Name = "PlaylistManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PlaylistManager";
            this.Resize += new System.EventHandler(this.PlaylistManager_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAdd)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelPlaylistOverview;
        private System.Windows.Forms.Label labelPlaylistURL;
        private System.Windows.Forms.TextBox textBoxPlaylistURL;
        private System.Windows.Forms.PictureBox pictureBoxAdd;
        private System.Windows.Forms.ListView listViewPlaylists;
        private System.Windows.Forms.ColumnHeader itemText;
        private System.Windows.Forms.ColumnHeader itemRegex;
        private System.Windows.Forms.TextBox textBoxRegex;
        private System.Windows.Forms.Label labelRegex;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
    }
}