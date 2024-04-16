using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace TikTok_Downloader
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            urlTextBox = new TextBox();
            downloadButton = new Button();
            outputTextBox = new TextBox();
            progressBar = new ProgressBar();
            cmbChoice = new ComboBox();
            txtUsername = new TextBox();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            ChangeDownloadFolderToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            filePathLabel = new Label();
            filePathTextBox = new TextBox();
            browseFileButton = new Button();
            label1 = new Label();
            label2 = new Label();
            withWatermarkCheckBox = new CheckBox();
            label3 = new Label();
            label4 = new Label();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // urlTextBox
            // 
            urlTextBox.Location = new Point(149, 62);
            urlTextBox.Name = "urlTextBox";
            urlTextBox.PlaceholderText = "Enter TikTok Video/Image Link";
            urlTextBox.Size = new Size(281, 23);
            urlTextBox.TabIndex = 0;
            // 
            // downloadButton
            // 
            downloadButton.BackColor = SystemColors.Control;
            downloadButton.Location = new Point(436, 174);
            downloadButton.Name = "downloadButton";
            downloadButton.Size = new Size(88, 23);
            downloadButton.TabIndex = 1;
            downloadButton.Text = "Download";
            downloadButton.UseVisualStyleBackColor = false;
            downloadButton.Click += btnDownload_Click;
            // 
            // outputTextBox
            // 
            outputTextBox.BackColor = SystemColors.WindowText;
            outputTextBox.Cursor = Cursors.IBeam;
            outputTextBox.ForeColor = Color.FromArgb(0, 192, 0);
            outputTextBox.Location = new Point(12, 246);
            outputTextBox.Multiline = true;
            outputTextBox.Name = "outputTextBox";
            outputTextBox.ScrollBars = ScrollBars.Vertical;
            outputTextBox.Size = new Size(512, 234);
            outputTextBox.TabIndex = 2;
            outputTextBox.TextChanged += outputTextBox_TextChanged;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(12, 203);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(512, 22);
            progressBar.TabIndex = 3;
            progressBar.Click += progressBar_Click;
            // 
            // cmbChoice
            // 
            cmbChoice.FormattingEnabled = true;
            cmbChoice.Items.AddRange(new object[] { "Single Video/Image Download", "Mass Download by Username", "Mass Download from Text File Links" });
            cmbChoice.Location = new Point(149, 33);
            cmbChoice.Name = "cmbChoice";
            cmbChoice.Size = new Size(281, 23);
            cmbChoice.TabIndex = 4;
            // 
            // txtUsername
            // 
            txtUsername.Location = new Point(149, 96);
            txtUsername.Name = "txtUsername";
            txtUsername.PlaceholderText = "Enter TikTok Username";
            txtUsername.Size = new Size(281, 23);
            txtUsername.TabIndex = 5;
            // 
            // menuStrip1
            // 
            menuStrip1.BackColor = SystemColors.Menu;
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(532, 24);
            menuStrip1.TabIndex = 6;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { ChangeDownloadFolderToolStripMenuItem, aboutToolStripMenuItem });
            fileToolStripMenuItem.ForeColor = SystemColors.MenuText;
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(61, 20);
            fileToolStripMenuItem.Text = "Options";
            fileToolStripMenuItem.Click += fileToolStripMenuItem_Click;
            // 
            // ChangeDownloadFolderToolStripMenuItem
            // 
            ChangeDownloadFolderToolStripMenuItem.BackColor = Color.Transparent;
            ChangeDownloadFolderToolStripMenuItem.Name = "ChangeDownloadFolderToolStripMenuItem";
            ChangeDownloadFolderToolStripMenuItem.Size = new Size(208, 22);
            ChangeDownloadFolderToolStripMenuItem.Text = "Change Download Folder";
            ChangeDownloadFolderToolStripMenuItem.Click += ChangeDownloadFolderToolStripMenuItem_Click;
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.BackColor = Color.Transparent;
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(208, 22);
            aboutToolStripMenuItem.Text = "About";
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            // 
            // filePathLabel
            // 
            filePathLabel.AutoSize = true;
            filePathLabel.BackColor = Color.Transparent;
            filePathLabel.ForeColor = SystemColors.Control;
            filePathLabel.Location = new Point(12, 136);
            filePathLabel.Name = "filePathLabel";
            filePathLabel.Size = new Size(136, 15);
            filePathLabel.TabIndex = 8;
            filePathLabel.Text = "Download from Text file:";
            // 
            // filePathTextBox
            // 
            filePathTextBox.Location = new Point(149, 132);
            filePathTextBox.Name = "filePathTextBox";
            filePathTextBox.PlaceholderText = "Enter/Select Path to Text File";
            filePathTextBox.Size = new Size(281, 23);
            filePathTextBox.TabIndex = 9;
            filePathTextBox.TextChanged += filePathTextBox_TextChanged;
            // 
            // browseFileButton
            // 
            browseFileButton.Location = new Point(436, 132);
            browseFileButton.Name = "browseFileButton";
            browseFileButton.Size = new Size(88, 23);
            browseFileButton.TabIndex = 7;
            browseFileButton.Text = "Browse";
            browseFileButton.UseVisualStyleBackColor = true;
            browseFileButton.Click += browseFileButton_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.ForeColor = SystemColors.Control;
            label1.Location = new Point(12, 36);
            label1.Name = "label1";
            label1.Size = new Size(109, 15);
            label1.TabIndex = 6;
            label1.Text = "Download Options:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.Transparent;
            label2.ForeColor = SystemColors.Control;
            label2.Location = new Point(12, 99);
            label2.Name = "label2";
            label2.Size = new Size(130, 15);
            label2.TabIndex = 7;
            label2.Text = "Downlod by Username:";
            label2.Click += label2_Click;
            // 
            // withWatermarkCheckBox
            // 
            withWatermarkCheckBox.AutoSize = true;
            withWatermarkCheckBox.BackColor = Color.Transparent;
            withWatermarkCheckBox.ForeColor = SystemColors.Control;
            withWatermarkCheckBox.Location = new Point(12, 178);
            withWatermarkCheckBox.Name = "withWatermarkCheckBox";
            withWatermarkCheckBox.Size = new Size(112, 19);
            withWatermarkCheckBox.TabIndex = 8;
            withWatermarkCheckBox.Text = "With Watermark";
            withWatermarkCheckBox.UseVisualStyleBackColor = false;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.Transparent;
            label3.ForeColor = SystemColors.Control;
            label3.Location = new Point(12, 65);
            label3.Name = "label3";
            label3.Size = new Size(129, 15);
            label3.TabIndex = 9;
            label3.Text = "Download Single Links:";
            label3.Click += label3_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.BackColor = Color.Transparent;
            label4.ForeColor = SystemColors.Control;
            label4.Location = new Point(12, 228);
            label4.Name = "label4";
            label4.Size = new Size(133, 15);
            label4.TabIndex = 10;
            label4.Text = "Latest Download Status:";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            BackgroundImage = (System.Drawing.Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(532, 499);
            Controls.Add(menuStrip1);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(withWatermarkCheckBox);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(filePathTextBox);
            Controls.Add(filePathLabel);
            Controls.Add(browseFileButton);
            Controls.Add(txtUsername);
            Controls.Add(cmbChoice);
            Controls.Add(progressBar);
            Controls.Add(outputTextBox);
            Controls.Add(downloadButton);
            Controls.Add(urlTextBox);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            MaximizeBox = false;
            Name = "MainForm";
            Text = "TikTok Downloader";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private TextBox urlTextBox;
        private Button downloadButton;
        private TextBox outputTextBox;
        private ProgressBar progressBar;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem ChangeDownloadFolderToolStripMenuItem;
        private ComboBox cmbChoice;
        private TextBox txtUsername;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private Button browseFileButton;
        private Label filePathLabel;
        private TextBox filePathTextBox;
        private Label label1;
        private Label label2;
        private CheckBox withWatermarkCheckBox;
        private Label label3;
        private Label label4;

    }
}
