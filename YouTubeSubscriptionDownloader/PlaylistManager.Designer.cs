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
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAdd)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(252, 314);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 3;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(333, 314);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // labelPlaylistOverview
            // 
            this.labelPlaylistOverview.Location = new System.Drawing.Point(12, 9);
            this.labelPlaylistOverview.Name = "labelPlaylistOverview";
            this.labelPlaylistOverview.Size = new System.Drawing.Size(394, 32);
            this.labelPlaylistOverview.TabIndex = 4;
            this.labelPlaylistOverview.Text = "Here you can define a list of playlists to be checked for new uploads alongside y" +
    "our user subscriptions";
            // 
            // labelPlaylistURL
            // 
            this.labelPlaylistURL.AutoSize = true;
            this.labelPlaylistURL.Location = new System.Drawing.Point(12, 256);
            this.labelPlaylistURL.Name = "labelPlaylistURL";
            this.labelPlaylistURL.Size = new System.Drawing.Size(67, 13);
            this.labelPlaylistURL.TabIndex = 5;
            this.labelPlaylistURL.Text = "Playlist URL:";
            // 
            // textBoxPlaylistURL
            // 
            this.textBoxPlaylistURL.Location = new System.Drawing.Point(85, 253);
            this.textBoxPlaylistURL.Name = "textBoxPlaylistURL";
            this.textBoxPlaylistURL.Size = new System.Drawing.Size(278, 20);
            this.textBoxPlaylistURL.TabIndex = 6;
            // 
            // pictureBoxAdd
            // 
            this.pictureBoxAdd.Image = global::YouTubeSubscriptionDownloader.Properties.Resources.plus_black_symbol;
            this.pictureBoxAdd.Location = new System.Drawing.Point(369, 253);
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
            this.listViewPlaylists.Location = new System.Drawing.Point(12, 44);
            this.listViewPlaylists.MultiSelect = false;
            this.listViewPlaylists.Name = "listViewPlaylists";
            this.listViewPlaylists.Size = new System.Drawing.Size(400, 203);
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
            this.textBoxRegex.Location = new System.Drawing.Point(125, 276);
            this.textBoxRegex.Name = "textBoxRegex";
            this.textBoxRegex.Size = new System.Drawing.Size(238, 20);
            this.textBoxRegex.TabIndex = 9;
            // 
            // labelRegex
            // 
            this.labelRegex.AutoSize = true;
            this.labelRegex.Location = new System.Drawing.Point(12, 279);
            this.labelRegex.Name = "labelRegex";
            this.labelRegex.Size = new System.Drawing.Size(107, 13);
            this.labelRegex.TabIndex = 10;
            this.labelRegex.Text = "Filter regex (optional):";
            // 
            // PlaylistManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(420, 349);
            this.Controls.Add(this.labelRegex);
            this.Controls.Add(this.textBoxRegex);
            this.Controls.Add(this.listViewPlaylists);
            this.Controls.Add(this.pictureBoxAdd);
            this.Controls.Add(this.textBoxPlaylistURL);
            this.Controls.Add(this.labelPlaylistURL);
            this.Controls.Add(this.labelPlaylistOverview);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "PlaylistManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PlaylistManager";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAdd)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}