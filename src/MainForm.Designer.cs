/*
##########################################
#           TikTok Downloader            #
#           Made by Jettcodey            #
#                © 2024                  #
#           DO NOT REMOVE THIS           #
##########################################
*/
using System.Windows.Forms;

namespace TikTok_Downloader
{
    partial class MainForm
    {
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            menuStrip1 = new MenuStrip();
            // menuStrip1.Renderer = new MyRenderer();  <-- Cause VS Removes this Line everytime
            menuStrip1.Renderer = new MyRenderer();
            fileToolStripMenuItem = new ToolStripMenuItem();
            ChangeDownloadFolderToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            settingsToolStripMenuItem = new ToolStripMenuItem();
            checkForUpdateToolStripMenuItem = new ToolStripMenuItem();
            cmbChoice = new ComboBox();
            urlTextBox = new TextBox();
            downloadButton = new Button();
            browseFileButton = new Button();
            noWatermarkCheckBox = new CheckBox();
            withWatermarkCheckBox = new CheckBox();
            downloadAvatarsCheckBox = new CheckBox();
            progressBar = new ProgressBar();
            pauseButton = new Button();
            stopButton = new Button();
            outputTextBox = new TextBox();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            linkLabel1 = new LinkLabel();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.BackColor = Color.FromArgb(32, 32, 32);
            // menuStrip1.Renderer = new MyRenderer();  <-- Cause VS Removes this Line everytime
            menuStrip1.Renderer = new MyRenderer();
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(534, 24);
            menuStrip1.TabIndex = 6;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.BackColor = Color.FromArgb(58, 58, 58);
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { ChangeDownloadFolderToolStripMenuItem, aboutToolStripMenuItem, settingsToolStripMenuItem, checkForUpdateToolStripMenuItem });
            fileToolStripMenuItem.ForeColor = SystemColors.ButtonFace;
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(61, 20);
            fileToolStripMenuItem.Text = "Options";
            // 
            // ChangeDownloadFolderToolStripMenuItem
            // 
            ChangeDownloadFolderToolStripMenuItem.BackColor = Color.FromArgb(32, 32, 32);
            ChangeDownloadFolderToolStripMenuItem.ForeColor = SystemColors.ButtonFace;
            ChangeDownloadFolderToolStripMenuItem.Name = "ChangeDownloadFolderToolStripMenuItem";
            ChangeDownloadFolderToolStripMenuItem.Size = new Size(208, 22);
            ChangeDownloadFolderToolStripMenuItem.Text = "Change Download Folder";
            ChangeDownloadFolderToolStripMenuItem.Click += ChangeDownloadFolderToolStripMenuItem_Click;
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.BackColor = Color.FromArgb(32, 32, 32);
            aboutToolStripMenuItem.ForeColor = SystemColors.ButtonFace;
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(208, 22);
            aboutToolStripMenuItem.Text = "About";
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            // 
            // settingsToolStripMenuItem
            // 
            settingsToolStripMenuItem.BackColor = Color.FromArgb(32, 32, 32);
            settingsToolStripMenuItem.ForeColor = SystemColors.ButtonFace;
            settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            settingsToolStripMenuItem.Size = new Size(208, 22);
            settingsToolStripMenuItem.Text = "Settings";
            settingsToolStripMenuItem.Click += settingsToolStripMenuItem_Click;
            // 
            // checkForUpdateToolStripMenuItem
            // 
            checkForUpdateToolStripMenuItem.BackColor = Color.FromArgb(32, 32, 32);
            checkForUpdateToolStripMenuItem.ForeColor = SystemColors.Control;
            checkForUpdateToolStripMenuItem.Name = "checkForUpdateToolStripMenuItem";
            checkForUpdateToolStripMenuItem.Size = new Size(208, 22);
            checkForUpdateToolStripMenuItem.Text = "Check for Update";
            checkForUpdateToolStripMenuItem.Click += checkForUpdateToolStripMenuItem_Click;
            // 
            // cmbChoice
            // 
            cmbChoice.FormattingEnabled = true;
            cmbChoice.Items.AddRange(new object[] { "Single Video/Image Download", "Mass Download from Text File Links", "Mass Download by Username", "HD Download Video/Image", "HD Download From Text File Links", "HD Mass Download by Username" });
            cmbChoice.Location = new Point(149, 36);
            cmbChoice.Name = "cmbChoice";
            cmbChoice.Size = new Size(275, 23);
            cmbChoice.TabIndex = 4;
            cmbChoice.SelectedIndexChanged += cmbChoice_SelectedIndexChanged;
            // 
            // urlTextBox
            // 
            urlTextBox.BackColor = SystemColors.Window;
            urlTextBox.Location = new Point(149, 73);
            urlTextBox.Name = "urlTextBox";
            urlTextBox.PlaceholderText = "Enter TikTok Video/Image Link";
            urlTextBox.Size = new Size(275, 23);
            urlTextBox.TabIndex = 0;
            // 
            // downloadButton
            // 
            downloadButton.BackColor = SystemColors.Control;
            downloadButton.Location = new Point(434, 36);
            downloadButton.Name = "downloadButton";
            downloadButton.Size = new Size(88, 23);
            downloadButton.TabIndex = 1;
            downloadButton.Text = "Download";
            downloadButton.UseVisualStyleBackColor = false;
            downloadButton.Click += btnDownload_Click;
            // 
            // browseFileButton
            // 
            browseFileButton.Location = new Point(434, 73);
            browseFileButton.Name = "browseFileButton";
            browseFileButton.Size = new Size(88, 23);
            browseFileButton.TabIndex = 7;
            browseFileButton.Text = "Browse";
            browseFileButton.UseVisualStyleBackColor = true;
            browseFileButton.Visible = false;
            browseFileButton.Click += browseFileButton_Click;
            // 
            // noWatermarkCheckBox
            // 
            noWatermarkCheckBox.AutoSize = true;
            noWatermarkCheckBox.BackColor = Color.Transparent;
            noWatermarkCheckBox.ForeColor = SystemColors.Control;
            noWatermarkCheckBox.ImageAlign = ContentAlignment.MiddleLeft;
            noWatermarkCheckBox.Location = new Point(258, 106);
            noWatermarkCheckBox.Name = "noWatermarkCheckBox";
            noWatermarkCheckBox.Size = new Size(103, 19);
            noWatermarkCheckBox.TabIndex = 9;
            noWatermarkCheckBox.Text = "No Watermark";
            noWatermarkCheckBox.TextAlign = ContentAlignment.TopLeft;
            noWatermarkCheckBox.UseVisualStyleBackColor = false;
            noWatermarkCheckBox.Visible = false;
            // 
            // withWatermarkCheckBox
            // 
            withWatermarkCheckBox.AutoSize = true;
            withWatermarkCheckBox.BackColor = Color.Transparent;
            withWatermarkCheckBox.ForeColor = SystemColors.Control;
            withWatermarkCheckBox.ImageAlign = ContentAlignment.MiddleLeft;
            withWatermarkCheckBox.Location = new Point(12, 106);
            withWatermarkCheckBox.Name = "withWatermarkCheckBox";
            withWatermarkCheckBox.Size = new Size(112, 19);
            withWatermarkCheckBox.TabIndex = 8;
            withWatermarkCheckBox.Text = "With Watermark";
            withWatermarkCheckBox.TextAlign = ContentAlignment.TopLeft;
            withWatermarkCheckBox.UseVisualStyleBackColor = false;
            withWatermarkCheckBox.CheckedChanged += withWatermarkCheckBox_CheckedChanged;
            // 
            // downloadAvatarsCheckBox
            // 
            downloadAvatarsCheckBox.AutoSize = true;
            downloadAvatarsCheckBox.BackColor = Color.Transparent;
            downloadAvatarsCheckBox.ForeColor = SystemColors.Control;
            downloadAvatarsCheckBox.ImageAlign = ContentAlignment.MiddleLeft;
            downloadAvatarsCheckBox.Location = new Point(130, 106);
            downloadAvatarsCheckBox.Name = "downloadAvatarsCheckBox";
            downloadAvatarsCheckBox.Size = new Size(122, 19);
            downloadAvatarsCheckBox.TabIndex = 10;
            downloadAvatarsCheckBox.Text = "Download Avatars";
            downloadAvatarsCheckBox.TextAlign = ContentAlignment.TopLeft;
            downloadAvatarsCheckBox.UseVisualStyleBackColor = false;
            downloadAvatarsCheckBox.CheckedChanged += downloadAvatarsCheckBox_CheckedChanged;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(12, 133);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(412, 25);
            progressBar.TabIndex = 3;
            // 
            // pauseButton
            // 
            pauseButton.BackColor = SystemColors.Control;
            pauseButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            pauseButton.Location = new Point(430, 133);
            pauseButton.Name = "pauseButton";
            pauseButton.RightToLeft = RightToLeft.Yes;
            pauseButton.Size = new Size(30, 25);
            pauseButton.TabIndex = 5;
            pauseButton.Text = "||";
            pauseButton.TextImageRelation = TextImageRelation.TextBeforeImage;
            pauseButton.UseVisualStyleBackColor = true;
            pauseButton.Click += pauseButton_Click;
            // 
            // stopButton
            // 
            stopButton.BackColor = SystemColors.Control;
            stopButton.Location = new Point(466, 133);
            stopButton.Name = "stopButton";
            stopButton.Size = new Size(56, 25);
            stopButton.TabIndex = 5;
            stopButton.Text = "Stop";
            stopButton.UseVisualStyleBackColor = true;
            stopButton.Click += stopButton_Click;
            // 
            // outputTextBox
            // 
            outputTextBox.BackColor = SystemColors.WindowText;
            outputTextBox.Cursor = Cursors.IBeam;
            outputTextBox.ForeColor = Color.FromArgb(0, 192, 0);
            outputTextBox.Location = new Point(12, 185);
            outputTextBox.Multiline = true;
            outputTextBox.Name = "outputTextBox";
            outputTextBox.ScrollBars = ScrollBars.Vertical;
            outputTextBox.Size = new Size(510, 234);
            outputTextBox.TabIndex = 2;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.Transparent;
            label3.ForeColor = SystemColors.Control;
            label3.Location = new Point(12, 76);
            label3.Name = "label3";
            label3.Size = new Size(129, 15);
            label3.TabIndex = 12;
            label3.Text = "Download Single Links:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.Transparent;
            label2.ForeColor = SystemColors.Control;
            label2.Location = new Point(12, 165);
            label2.Name = "label2";
            label2.Size = new Size(133, 15);
            label2.TabIndex = 13;
            label2.Text = "Latest Download Status:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.ForeColor = SystemColors.Control;
            label1.Location = new Point(12, 39);
            label1.Name = "label1";
            label1.Size = new Size(109, 15);
            label1.TabIndex = 14;
            label1.Text = "Download Options:";
            // 
            // linkLabel1
            // 
            linkLabel1.ActiveLinkColor = Color.Teal;
            linkLabel1.AutoSize = true;
            linkLabel1.BackColor = Color.Transparent;
            linkLabel1.Font = new Font("Segoe UI Emoji", 9F, FontStyle.Italic, GraphicsUnit.Point, 0);
            linkLabel1.LinkColor = Color.Aquamarine;
            linkLabel1.Location = new Point(425, 106);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(97, 16);
            linkLabel1.TabIndex = 11;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "Report Bug/Issue";
            linkLabel1.VisitedLinkColor = Color.Aquamarine;
            linkLabel1.LinkClicked += LinkLabel1_LinkClicked;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = SystemColors.ControlDarkDark;
            BackgroundImage = Properties.Resources.bg;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(534, 431);
            Controls.Add(menuStrip1);
            Controls.Add(cmbChoice);
            Controls.Add(urlTextBox);
            Controls.Add(downloadButton);
            Controls.Add(browseFileButton);
            Controls.Add(noWatermarkCheckBox);
            Controls.Add(withWatermarkCheckBox);
            Controls.Add(downloadAvatarsCheckBox);
            Controls.Add(progressBar);
            Controls.Add(pauseButton);
            Controls.Add(stopButton);
            Controls.Add(outputTextBox);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(linkLabel1);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            MaximizeBox = false;
            Name = "MainForm";
            Text = "TikTok Downloader v1.3.7";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem ChangeDownloadFolderToolStripMenuItem;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem checkForUpdateToolStripMenuItem;
        private ComboBox cmbChoice;
        private TextBox urlTextBox;
        private Button downloadButton;
        private Button browseFileButton;
        private CheckBox noWatermarkCheckBox;
        private CheckBox withWatermarkCheckBox;
        private CheckBox downloadAvatarsCheckBox;
        private ProgressBar progressBar;
        private Button pauseButton;
        private Button stopButton;
        private TextBox outputTextBox;
        private Label label3;
        private Label label2;
        private Label label1;
        private LinkLabel linkLabel1;

        private class MyRenderer : ToolStripProfessionalRenderer
        {

            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {

                if (e.Item.Selected)
                {
                    Rectangle menuRectangle = new Rectangle(Point.Empty, e.Item.Size);

                    e.Graphics.FillRectangle(new SolidBrush(ColorTranslator.FromHtml("#343434")), menuRectangle);

                }
                else if (e.Item.Pressed)
                {
                    Rectangle menuRectangle = new Rectangle(Point.Empty, e.Item.Size);

                    e.Graphics.FillRectangle(new SolidBrush(ColorTranslator.FromHtml("#202020")), menuRectangle);
                }
                else
                {
                    base.OnRenderMenuItemBackground(e);
                }
            }
        }

        private void cmbChoice_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbChoice.SelectedIndex)
            {
                case 0:
                    urlTextBox.PlaceholderText = "Enter TikTok Video/Image Link";
                    label3.Text = "Download Single Links:";
                    browseFileButton.Visible = false;
                    withWatermarkCheckBox.Visible = true;
                    downloadAvatarsCheckBox.Visible = true;
                    break;
                case 1:
                    urlTextBox.PlaceholderText = "Enter/Select Path to Text File";
                    label3.Text = "Download from Text file:";
                    browseFileButton.Visible = true;
                    withWatermarkCheckBox.Visible = true;
                    downloadAvatarsCheckBox.Visible = true;
                    break;
                case 2:
                    urlTextBox.PlaceholderText = "Enter TikTok link/Username";
                    label3.Text = "Download by Username:";
                    browseFileButton.Visible = false;
                    withWatermarkCheckBox.Visible = true;
                    downloadAvatarsCheckBox.Visible = true;
                    break;
                case 3:
                    urlTextBox.PlaceholderText = "Enter TikTok Video/Image Link";
                    label3.Text = "HD Single Video/Image:";
                    browseFileButton.Visible = false;
                    withWatermarkCheckBox.Visible = false;
                    downloadAvatarsCheckBox.Visible = false;
                    break;
                case 4:
                    urlTextBox.PlaceholderText = "Enter/Select Path to Text File";
                    label3.Text = "HD Download Text file:";
                    browseFileButton.Visible = true;
                    withWatermarkCheckBox.Visible = false;
                    downloadAvatarsCheckBox.Visible = false;
                    break;
                case 5:
                    urlTextBox.PlaceholderText = "Enter TikTok link/Username";
                    label3.Text = "HD Download Username:";
                    browseFileButton.Visible = false;
                    withWatermarkCheckBox.Visible = false;
                    downloadAvatarsCheckBox.Visible = false;
                    break;
                default:
                    urlTextBox.PlaceholderText = "Enter TikTok Video/Image Link";
                    label3.Text = "Download Single Links:";
                    browseFileButton.Visible = false;
                    withWatermarkCheckBox.Visible = true;
                    downloadAvatarsCheckBox.Visible = true;
                    break;
            }

            urlTextBox.Text = "";
        }
    }
}
