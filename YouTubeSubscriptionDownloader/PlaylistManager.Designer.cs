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
            this.listBoxPlaylists = new System.Windows.Forms.ListBox();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelPlaylistOverview = new System.Windows.Forms.Label();
            this.labelPlaylistURL = new System.Windows.Forms.Label();
            this.textBoxPlaylistURL = new System.Windows.Forms.TextBox();
            this.pictureBoxAdd = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAdd)).BeginInit();
            this.SuspendLayout();
            // 
            // listBoxPlaylists
            // 
            this.listBoxPlaylists.FormattingEnabled = true;
            this.listBoxPlaylists.Location = new System.Drawing.Point(12, 44);
            this.listBoxPlaylists.Name = "listBoxPlaylists";
            this.listBoxPlaylists.Size = new System.Drawing.Size(394, 121);
            this.listBoxPlaylists.TabIndex = 0;
            this.listBoxPlaylists.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBoxPlaylists_KeyDown);
            this.listBoxPlaylists.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listBoxPlaylists_MouseDown);
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(250, 226);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 3;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(331, 226);
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
            this.labelPlaylistURL.Location = new System.Drawing.Point(12, 180);
            this.labelPlaylistURL.Name = "labelPlaylistURL";
            this.labelPlaylistURL.Size = new System.Drawing.Size(67, 13);
            this.labelPlaylistURL.TabIndex = 5;
            this.labelPlaylistURL.Text = "Playlist URL:";
            // 
            // textBoxPlaylistURL
            // 
            this.textBoxPlaylistURL.Location = new System.Drawing.Point(85, 177);
            this.textBoxPlaylistURL.Name = "textBoxPlaylistURL";
            this.textBoxPlaylistURL.Size = new System.Drawing.Size(278, 20);
            this.textBoxPlaylistURL.TabIndex = 6;
            // 
            // pictureBoxAdd
            // 
            this.pictureBoxAdd.Image = global::YouTubeSubscriptionDownloader.Properties.Resources.plus_black_symbol;
            this.pictureBoxAdd.Location = new System.Drawing.Point(369, 177);
            this.pictureBoxAdd.Name = "pictureBoxAdd";
            this.pictureBoxAdd.Size = new System.Drawing.Size(20, 20);
            this.pictureBoxAdd.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxAdd.TabIndex = 7;
            this.pictureBoxAdd.TabStop = false;
            this.pictureBoxAdd.Click += new System.EventHandler(this.pictureBoxAdd_Click);
            // 
            // PlaylistManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(418, 261);
            this.Controls.Add(this.pictureBoxAdd);
            this.Controls.Add(this.textBoxPlaylistURL);
            this.Controls.Add(this.labelPlaylistURL);
            this.Controls.Add(this.labelPlaylistOverview);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.listBoxPlaylists);
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

        private System.Windows.Forms.ListBox listBoxPlaylists;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelPlaylistOverview;
        private System.Windows.Forms.Label labelPlaylistURL;
        private System.Windows.Forms.TextBox textBoxPlaylistURL;
        private System.Windows.Forms.PictureBox pictureBoxAdd;
    }
}