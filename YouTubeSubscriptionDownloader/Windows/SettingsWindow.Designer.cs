namespace YouTubeSubscriptionDownloader
{
    partial class SettingsWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsWindow));
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.labelDownloadDirectory = new System.Windows.Forms.Label();
            this.textBoxDownloadDirectory = new System.Windows.Forms.TextBox();
            this.labelPreferredQuality = new System.Windows.Forms.Label();
            this.comboBoxPreferredQuality = new System.Windows.Forms.ComboBox();
            this.buttonFolderPicker = new System.Windows.Forms.Button();
            this.checkBoxDownloadVideos = new System.Windows.Forms.CheckBox();
            this.checkBoxAddPocket = new System.Windows.Forms.CheckBox();
            this.checkBoxCheckForMissedUploads = new System.Windows.Forms.CheckBox();
            this.labelIterationFrequency = new System.Windows.Forms.Label();
            this.numericUpDownIterationFrequency = new System.Windows.Forms.NumericUpDown();
            this.labelMinutes = new System.Windows.Forms.Label();
            this.checkBoxRunIterationsOnStartup = new System.Windows.Forms.CheckBox();
            this.checkBoxSyncSubscriptions = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageSettings = new System.Windows.Forms.TabPage();
            this.tabPageAbout = new System.Windows.Forms.TabPage();
            this.pictureBoxPayPal = new System.Windows.Forms.PictureBox();
            this.pictureBoxGoogleWallet = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPageNotifications = new System.Windows.Forms.TabPage();
            this.comboBoxNotificationClick = new System.Windows.Forms.ComboBox();
            this.labelClickNotification = new System.Windows.Forms.Label();
            this.checkBoxShowNotifications = new System.Windows.Forms.CheckBox();
            this.checkBoxShowThumbnails = new System.Windows.Forms.CheckBox();
            this.comboBoxNotifyScreen = new System.Windows.Forms.ComboBox();
            this.labelNotifyScreen = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownIterationFrequency)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPageSettings.SuspendLayout();
            this.tabPageAbout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPayPal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxGoogleWallet)).BeginInit();
            this.tabPageNotifications.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(266, 341);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 0;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(185, 341);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 1;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // labelDownloadDirectory
            // 
            this.labelDownloadDirectory.AutoSize = true;
            this.labelDownloadDirectory.Location = new System.Drawing.Point(6, 29);
            this.labelDownloadDirectory.Name = "labelDownloadDirectory";
            this.labelDownloadDirectory.Size = new System.Drawing.Size(101, 13);
            this.labelDownloadDirectory.TabIndex = 2;
            this.labelDownloadDirectory.Text = "Download directory:";
            // 
            // textBoxDownloadDirectory
            // 
            this.textBoxDownloadDirectory.Location = new System.Drawing.Point(113, 26);
            this.textBoxDownloadDirectory.Name = "textBoxDownloadDirectory";
            this.textBoxDownloadDirectory.Size = new System.Drawing.Size(182, 20);
            this.textBoxDownloadDirectory.TabIndex = 3;
            // 
            // labelPreferredQuality
            // 
            this.labelPreferredQuality.AutoSize = true;
            this.labelPreferredQuality.Location = new System.Drawing.Point(6, 59);
            this.labelPreferredQuality.Name = "labelPreferredQuality";
            this.labelPreferredQuality.Size = new System.Drawing.Size(86, 13);
            this.labelPreferredQuality.TabIndex = 4;
            this.labelPreferredQuality.Text = "Preferred quality:";
            // 
            // comboBoxPreferredQuality
            // 
            this.comboBoxPreferredQuality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPreferredQuality.FormattingEnabled = true;
            this.comboBoxPreferredQuality.Items.AddRange(new object[] {
            "Highest",
            "1080p",
            "720p",
            "480p",
            "360p",
            "240p",
            "144p"});
            this.comboBoxPreferredQuality.Location = new System.Drawing.Point(113, 56);
            this.comboBoxPreferredQuality.Name = "comboBoxPreferredQuality";
            this.comboBoxPreferredQuality.Size = new System.Drawing.Size(121, 21);
            this.comboBoxPreferredQuality.TabIndex = 5;
            // 
            // buttonFolderPicker
            // 
            this.buttonFolderPicker.Location = new System.Drawing.Point(301, 25);
            this.buttonFolderPicker.Name = "buttonFolderPicker";
            this.buttonFolderPicker.Size = new System.Drawing.Size(24, 23);
            this.buttonFolderPicker.TabIndex = 6;
            this.buttonFolderPicker.Text = "...";
            this.buttonFolderPicker.UseVisualStyleBackColor = true;
            this.buttonFolderPicker.Click += new System.EventHandler(this.buttonFolderPicker_Click);
            // 
            // checkBoxDownloadVideos
            // 
            this.checkBoxDownloadVideos.AutoSize = true;
            this.checkBoxDownloadVideos.Location = new System.Drawing.Point(6, 6);
            this.checkBoxDownloadVideos.Name = "checkBoxDownloadVideos";
            this.checkBoxDownloadVideos.Size = new System.Drawing.Size(115, 17);
            this.checkBoxDownloadVideos.TabIndex = 8;
            this.checkBoxDownloadVideos.Text = "Download Videos?";
            this.checkBoxDownloadVideos.UseVisualStyleBackColor = true;
            this.checkBoxDownloadVideos.CheckedChanged += new System.EventHandler(this.checkBoxDownloadVideos_CheckedChanged);
            // 
            // checkBoxAddPocket
            // 
            this.checkBoxAddPocket.AutoSize = true;
            this.checkBoxAddPocket.Location = new System.Drawing.Point(6, 83);
            this.checkBoxAddPocket.Name = "checkBoxAddPocket";
            this.checkBoxAddPocket.Size = new System.Drawing.Size(134, 17);
            this.checkBoxAddPocket.TabIndex = 9;
            this.checkBoxAddPocket.Text = "Add videos to Pocket?";
            this.checkBoxAddPocket.UseVisualStyleBackColor = true;
            this.checkBoxAddPocket.CheckedChanged += new System.EventHandler(this.checkBoxAddPocket_CheckedChanged);
            // 
            // checkBoxCheckForMissedUploads
            // 
            this.checkBoxCheckForMissedUploads.AutoSize = true;
            this.checkBoxCheckForMissedUploads.Location = new System.Drawing.Point(6, 107);
            this.checkBoxCheckForMissedUploads.Name = "checkBoxCheckForMissedUploads";
            this.checkBoxCheckForMissedUploads.Size = new System.Drawing.Size(291, 17);
            this.checkBoxCheckForMissedUploads.TabIndex = 10;
            this.checkBoxCheckForMissedUploads.Text = "Download all uploads since last time program was used?";
            this.checkBoxCheckForMissedUploads.UseVisualStyleBackColor = true;
            // 
            // labelIterationFrequency
            // 
            this.labelIterationFrequency.AutoSize = true;
            this.labelIterationFrequency.Location = new System.Drawing.Point(2, 202);
            this.labelIterationFrequency.Name = "labelIterationFrequency";
            this.labelIterationFrequency.Size = new System.Drawing.Size(91, 13);
            this.labelIterationFrequency.TabIndex = 11;
            this.labelIterationFrequency.Text = "Check frequency:";
            // 
            // numericUpDownIterationFrequency
            // 
            this.numericUpDownIterationFrequency.Location = new System.Drawing.Point(99, 200);
            this.numericUpDownIterationFrequency.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownIterationFrequency.Name = "numericUpDownIterationFrequency";
            this.numericUpDownIterationFrequency.Size = new System.Drawing.Size(40, 20);
            this.numericUpDownIterationFrequency.TabIndex = 12;
            this.numericUpDownIterationFrequency.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // labelMinutes
            // 
            this.labelMinutes.AutoSize = true;
            this.labelMinutes.Location = new System.Drawing.Point(145, 202);
            this.labelMinutes.Name = "labelMinutes";
            this.labelMinutes.Size = new System.Drawing.Size(43, 13);
            this.labelMinutes.TabIndex = 13;
            this.labelMinutes.Text = "minutes";
            // 
            // checkBoxRunIterationsOnStartup
            // 
            this.checkBoxRunIterationsOnStartup.AutoSize = true;
            this.checkBoxRunIterationsOnStartup.Location = new System.Drawing.Point(6, 173);
            this.checkBoxRunIterationsOnStartup.Name = "checkBoxRunIterationsOnStartup";
            this.checkBoxRunIterationsOnStartup.Size = new System.Drawing.Size(228, 17);
            this.checkBoxRunIterationsOnStartup.TabIndex = 14;
            this.checkBoxRunIterationsOnStartup.Text = "Start running iterations on program startup?";
            this.checkBoxRunIterationsOnStartup.UseVisualStyleBackColor = true;
            // 
            // checkBoxSyncSubscriptions
            // 
            this.checkBoxSyncSubscriptions.Location = new System.Drawing.Point(6, 130);
            this.checkBoxSyncSubscriptions.Name = "checkBoxSyncSubscriptions";
            this.checkBoxSyncSubscriptions.Size = new System.Drawing.Size(319, 37);
            this.checkBoxSyncSubscriptions.TabIndex = 18;
            this.checkBoxSyncSubscriptions.Text = "Sync subscriptions with YouTube? (does not affect manual playlist subscriptions)";
            this.checkBoxSyncSubscriptions.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageSettings);
            this.tabControl1.Controls.Add(this.tabPageNotifications);
            this.tabControl1.Controls.Add(this.tabPageAbout);
            this.tabControl1.Location = new System.Drawing.Point(1, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(344, 335);
            this.tabControl1.TabIndex = 19;
            // 
            // tabPageSettings
            // 
            this.tabPageSettings.Controls.Add(this.checkBoxDownloadVideos);
            this.tabPageSettings.Controls.Add(this.checkBoxSyncSubscriptions);
            this.tabPageSettings.Controls.Add(this.labelDownloadDirectory);
            this.tabPageSettings.Controls.Add(this.textBoxDownloadDirectory);
            this.tabPageSettings.Controls.Add(this.labelPreferredQuality);
            this.tabPageSettings.Controls.Add(this.comboBoxPreferredQuality);
            this.tabPageSettings.Controls.Add(this.checkBoxRunIterationsOnStartup);
            this.tabPageSettings.Controls.Add(this.buttonFolderPicker);
            this.tabPageSettings.Controls.Add(this.labelMinutes);
            this.tabPageSettings.Controls.Add(this.numericUpDownIterationFrequency);
            this.tabPageSettings.Controls.Add(this.checkBoxAddPocket);
            this.tabPageSettings.Controls.Add(this.labelIterationFrequency);
            this.tabPageSettings.Controls.Add(this.checkBoxCheckForMissedUploads);
            this.tabPageSettings.Location = new System.Drawing.Point(4, 22);
            this.tabPageSettings.Name = "tabPageSettings";
            this.tabPageSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSettings.Size = new System.Drawing.Size(336, 309);
            this.tabPageSettings.TabIndex = 0;
            this.tabPageSettings.Text = "Settings";
            this.tabPageSettings.UseVisualStyleBackColor = true;
            // 
            // tabPageAbout
            // 
            this.tabPageAbout.Controls.Add(this.pictureBoxPayPal);
            this.tabPageAbout.Controls.Add(this.pictureBoxGoogleWallet);
            this.tabPageAbout.Controls.Add(this.label1);
            this.tabPageAbout.Location = new System.Drawing.Point(4, 22);
            this.tabPageAbout.Name = "tabPageAbout";
            this.tabPageAbout.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageAbout.Size = new System.Drawing.Size(336, 309);
            this.tabPageAbout.TabIndex = 1;
            this.tabPageAbout.Text = "About";
            this.tabPageAbout.UseVisualStyleBackColor = true;
            // 
            // pictureBoxPayPal
            // 
            this.pictureBoxPayPal.Image = global::YouTubeSubscriptionDownloader.Properties.Resources.PayPalButton;
            this.pictureBoxPayPal.Location = new System.Drawing.Point(192, 134);
            this.pictureBoxPayPal.Name = "pictureBoxPayPal";
            this.pictureBoxPayPal.Size = new System.Drawing.Size(91, 35);
            this.pictureBoxPayPal.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxPayPal.TabIndex = 7;
            this.pictureBoxPayPal.TabStop = false;
            this.pictureBoxPayPal.Click += new System.EventHandler(this.PictureBoxPayPal_Click);
            // 
            // pictureBoxGoogleWallet
            // 
            this.pictureBoxGoogleWallet.Image = global::YouTubeSubscriptionDownloader.Properties.Resources.GooglePayButton;
            this.pictureBoxGoogleWallet.Location = new System.Drawing.Point(33, 133);
            this.pictureBoxGoogleWallet.Name = "pictureBoxGoogleWallet";
            this.pictureBoxGoogleWallet.Size = new System.Drawing.Size(153, 40);
            this.pictureBoxGoogleWallet.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxGoogleWallet.TabIndex = 6;
            this.pictureBoxGoogleWallet.TabStop = false;
            this.pictureBoxGoogleWallet.Click += new System.EventHandler(this.PictureBoxGoogleWallet_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(7, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(323, 70);
            this.label1.TabIndex = 0;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // tabPageNotifications
            // 
            this.tabPageNotifications.Controls.Add(this.comboBoxNotifyScreen);
            this.tabPageNotifications.Controls.Add(this.labelNotifyScreen);
            this.tabPageNotifications.Controls.Add(this.checkBoxShowThumbnails);
            this.tabPageNotifications.Controls.Add(this.comboBoxNotificationClick);
            this.tabPageNotifications.Controls.Add(this.labelClickNotification);
            this.tabPageNotifications.Controls.Add(this.checkBoxShowNotifications);
            this.tabPageNotifications.Location = new System.Drawing.Point(4, 22);
            this.tabPageNotifications.Name = "tabPageNotifications";
            this.tabPageNotifications.Size = new System.Drawing.Size(336, 309);
            this.tabPageNotifications.TabIndex = 2;
            this.tabPageNotifications.Text = "Notifications";
            this.tabPageNotifications.UseVisualStyleBackColor = true;
            // 
            // comboBoxNotificationClick
            // 
            this.comboBoxNotificationClick.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxNotificationClick.FormattingEnabled = true;
            this.comboBoxNotificationClick.Items.AddRange(new object[] {
            "Opens video",
            "Dismisses notification"});
            this.comboBoxNotificationClick.Location = new System.Drawing.Point(132, 22);
            this.comboBoxNotificationClick.Name = "comboBoxNotificationClick";
            this.comboBoxNotificationClick.Size = new System.Drawing.Size(137, 21);
            this.comboBoxNotificationClick.TabIndex = 20;
            // 
            // labelClickNotification
            // 
            this.labelClickNotification.AutoSize = true;
            this.labelClickNotification.Location = new System.Drawing.Point(25, 25);
            this.labelClickNotification.Name = "labelClickNotification";
            this.labelClickNotification.Size = new System.Drawing.Size(94, 13);
            this.labelClickNotification.TabIndex = 19;
            this.labelClickNotification.Text = "Notification click...";
            // 
            // checkBoxShowNotifications
            // 
            this.checkBoxShowNotifications.AutoSize = true;
            this.checkBoxShowNotifications.Location = new System.Drawing.Point(7, 3);
            this.checkBoxShowNotifications.Name = "checkBoxShowNotifications";
            this.checkBoxShowNotifications.Size = new System.Drawing.Size(120, 17);
            this.checkBoxShowNotifications.TabIndex = 18;
            this.checkBoxShowNotifications.Text = "Show Notifications?";
            this.checkBoxShowNotifications.UseVisualStyleBackColor = true;
            // 
            // checkBoxShowThumbnails
            // 
            this.checkBoxShowThumbnails.AutoSize = true;
            this.checkBoxShowThumbnails.Location = new System.Drawing.Point(7, 85);
            this.checkBoxShowThumbnails.Name = "checkBoxShowThumbnails";
            this.checkBoxShowThumbnails.Size = new System.Drawing.Size(206, 17);
            this.checkBoxShowThumbnails.TabIndex = 21;
            this.checkBoxShowThumbnails.Text = "Show video thumbnail in notifications?";
            this.checkBoxShowThumbnails.UseVisualStyleBackColor = true;
            // 
            // comboBoxNotifyScreen
            // 
            this.comboBoxNotifyScreen.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxNotifyScreen.FormattingEnabled = true;
            this.comboBoxNotifyScreen.Location = new System.Drawing.Point(148, 49);
            this.comboBoxNotifyScreen.Name = "comboBoxNotifyScreen";
            this.comboBoxNotifyScreen.Size = new System.Drawing.Size(121, 21);
            this.comboBoxNotifyScreen.TabIndex = 23;
            // 
            // labelNotifyScreen
            // 
            this.labelNotifyScreen.AutoSize = true;
            this.labelNotifyScreen.Location = new System.Drawing.Point(25, 52);
            this.labelNotifyScreen.Name = "labelNotifyScreen";
            this.labelNotifyScreen.Size = new System.Drawing.Size(117, 13);
            this.labelNotifyScreen.TabIndex = 22;
            this.labelNotifyScreen.Text = "Show notifications on...";
            // 
            // SettingsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(347, 369);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SettingsWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SettingsWindow";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownIterationFrequency)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPageSettings.ResumeLayout(false);
            this.tabPageSettings.PerformLayout();
            this.tabPageAbout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPayPal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxGoogleWallet)).EndInit();
            this.tabPageNotifications.ResumeLayout(false);
            this.tabPageNotifications.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Label labelDownloadDirectory;
        private System.Windows.Forms.TextBox textBoxDownloadDirectory;
        private System.Windows.Forms.Label labelPreferredQuality;
        private System.Windows.Forms.ComboBox comboBoxPreferredQuality;
        private System.Windows.Forms.Button buttonFolderPicker;
        private System.Windows.Forms.CheckBox checkBoxDownloadVideos;
        private System.Windows.Forms.CheckBox checkBoxAddPocket;
        private System.Windows.Forms.CheckBox checkBoxCheckForMissedUploads;
        private System.Windows.Forms.Label labelIterationFrequency;
        private System.Windows.Forms.NumericUpDown numericUpDownIterationFrequency;
        private System.Windows.Forms.Label labelMinutes;
        private System.Windows.Forms.CheckBox checkBoxRunIterationsOnStartup;
        private System.Windows.Forms.CheckBox checkBoxSyncSubscriptions;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageSettings;
        private System.Windows.Forms.TabPage tabPageAbout;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBoxPayPal;
        private System.Windows.Forms.PictureBox pictureBoxGoogleWallet;
        private System.Windows.Forms.TabPage tabPageNotifications;
        private System.Windows.Forms.CheckBox checkBoxShowThumbnails;
        private System.Windows.Forms.ComboBox comboBoxNotificationClick;
        private System.Windows.Forms.Label labelClickNotification;
        private System.Windows.Forms.CheckBox checkBoxShowNotifications;
        private System.Windows.Forms.ComboBox comboBoxNotifyScreen;
        private System.Windows.Forms.Label labelNotifyScreen;
    }
}