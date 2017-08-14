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
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.labelDownloadDirectory = new System.Windows.Forms.Label();
            this.textBoxDownloadDirectory = new System.Windows.Forms.TextBox();
            this.labelPreferredQuality = new System.Windows.Forms.Label();
            this.comboBoxPreferredQuality = new System.Windows.Forms.ComboBox();
            this.buttonFolderPicker = new System.Windows.Forms.Button();
            this.checkBoxShowNotifications = new System.Windows.Forms.CheckBox();
            this.checkBoxDownloadVideos = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(256, 150);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 0;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(175, 150);
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
            this.labelDownloadDirectory.Location = new System.Drawing.Point(12, 36);
            this.labelDownloadDirectory.Name = "labelDownloadDirectory";
            this.labelDownloadDirectory.Size = new System.Drawing.Size(101, 13);
            this.labelDownloadDirectory.TabIndex = 2;
            this.labelDownloadDirectory.Text = "Download directory:";
            // 
            // textBoxDownloadDirectory
            // 
            this.textBoxDownloadDirectory.Location = new System.Drawing.Point(119, 33);
            this.textBoxDownloadDirectory.Name = "textBoxDownloadDirectory";
            this.textBoxDownloadDirectory.Size = new System.Drawing.Size(182, 20);
            this.textBoxDownloadDirectory.TabIndex = 3;
            // 
            // labelPreferredQuality
            // 
            this.labelPreferredQuality.AutoSize = true;
            this.labelPreferredQuality.Location = new System.Drawing.Point(12, 66);
            this.labelPreferredQuality.Name = "labelPreferredQuality";
            this.labelPreferredQuality.Size = new System.Drawing.Size(86, 13);
            this.labelPreferredQuality.TabIndex = 4;
            this.labelPreferredQuality.Text = "Preferred quality:";
            // 
            // comboBoxPreferredQuality
            // 
            this.comboBoxPreferredQuality.FormattingEnabled = true;
            this.comboBoxPreferredQuality.Items.AddRange(new object[] {
            "Highest",
            "1080p",
            "720p",
            "480p",
            "360p",
            "240p",
            "144p"});
            this.comboBoxPreferredQuality.Location = new System.Drawing.Point(119, 63);
            this.comboBoxPreferredQuality.Name = "comboBoxPreferredQuality";
            this.comboBoxPreferredQuality.Size = new System.Drawing.Size(121, 21);
            this.comboBoxPreferredQuality.TabIndex = 5;
            // 
            // buttonFolderPicker
            // 
            this.buttonFolderPicker.Location = new System.Drawing.Point(307, 32);
            this.buttonFolderPicker.Name = "buttonFolderPicker";
            this.buttonFolderPicker.Size = new System.Drawing.Size(24, 23);
            this.buttonFolderPicker.TabIndex = 6;
            this.buttonFolderPicker.Text = "...";
            this.buttonFolderPicker.UseVisualStyleBackColor = true;
            this.buttonFolderPicker.Click += new System.EventHandler(this.buttonFolderPicker_Click);
            // 
            // checkBoxShowNotifications
            // 
            this.checkBoxShowNotifications.AutoSize = true;
            this.checkBoxShowNotifications.Location = new System.Drawing.Point(12, 100);
            this.checkBoxShowNotifications.Name = "checkBoxShowNotifications";
            this.checkBoxShowNotifications.Size = new System.Drawing.Size(120, 17);
            this.checkBoxShowNotifications.TabIndex = 7;
            this.checkBoxShowNotifications.Text = "Show Notifications?";
            this.checkBoxShowNotifications.UseVisualStyleBackColor = true;
            // 
            // checkBoxDownloadVideos
            // 
            this.checkBoxDownloadVideos.AutoSize = true;
            this.checkBoxDownloadVideos.Location = new System.Drawing.Point(12, 13);
            this.checkBoxDownloadVideos.Name = "checkBoxDownloadVideos";
            this.checkBoxDownloadVideos.Size = new System.Drawing.Size(115, 17);
            this.checkBoxDownloadVideos.TabIndex = 8;
            this.checkBoxDownloadVideos.Text = "Download Videos?";
            this.checkBoxDownloadVideos.UseVisualStyleBackColor = true;
            this.checkBoxDownloadVideos.CheckedChanged += new System.EventHandler(this.checkBoxDownloadVideos_CheckedChanged);
            // 
            // SettingsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(343, 185);
            this.Controls.Add(this.checkBoxDownloadVideos);
            this.Controls.Add(this.checkBoxShowNotifications);
            this.Controls.Add(this.buttonFolderPicker);
            this.Controls.Add(this.comboBoxPreferredQuality);
            this.Controls.Add(this.labelPreferredQuality);
            this.Controls.Add(this.textBoxDownloadDirectory);
            this.Controls.Add(this.labelDownloadDirectory);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SettingsWindow";
            this.Text = "SettingsWindow";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Label labelDownloadDirectory;
        private System.Windows.Forms.TextBox textBoxDownloadDirectory;
        private System.Windows.Forms.Label labelPreferredQuality;
        private System.Windows.Forms.ComboBox comboBoxPreferredQuality;
        private System.Windows.Forms.Button buttonFolderPicker;
        private System.Windows.Forms.CheckBox checkBoxShowNotifications;
        private System.Windows.Forms.CheckBox checkBoxDownloadVideos;
    }
}