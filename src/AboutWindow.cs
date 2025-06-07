/*
##########################################
#           TikTok Downloader            #
#           Made by Jettcodey            #
#                © 2024                  #
#           DO NOT REMOVE THIS           #
##########################################
*/
using System.Diagnostics;
using static TikTok_Downloader.MainForm;

namespace TikTok_Downloader
{
    public partial class AboutWindow : Form
    {
        private readonly BrowserUtility browserUtility;


        internal AboutWindow(BrowserUtility browserUtility)

        {
            InitializeComponent();
            this.browserUtility = browserUtility;
        }

        public AboutWindow()
        {
            InitializeComponent();
        }

        private Label titleLabel;
        private Label licenseInfo;
        private Label versionLabel;
        private Label descriptionLabel;
        private Button okButton;
        private Label label2;
        private Label label3;
        private LinkLabel linkLabel;

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutWindow));
            titleLabel = new Label();
            licenseInfo = new Label();
            versionLabel = new Label();
            descriptionLabel = new Label();
            okButton = new Button();
            linkLabel = new LinkLabel();
            label2 = new Label();
            label3 = new Label();
            SuspendLayout();
            // 
            // titleLabel
            // 
            titleLabel.AutoSize = true;
            titleLabel.BackColor = Color.Transparent;
            titleLabel.Font = new Font("Arial", 12F, FontStyle.Bold);
            titleLabel.ForeColor = Color.Aquamarine;
            titleLabel.Location = new Point(55, 20);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new Size(157, 19);
            titleLabel.TabIndex = 0;
            titleLabel.Text = "TikTok Downloader";
            // 
            // licenseInfo
            // 
            licenseInfo.AutoSize = true;
            licenseInfo.BackColor = Color.Transparent;
            licenseInfo.ForeColor = Color.Aqua;
            licenseInfo.Location = new Point(12, 120);
            licenseInfo.Name = "licenseInfo";
            licenseInfo.Size = new Size(249, 15);
            licenseInfo.TabIndex = 4;
            licenseInfo.TabStop = true;
            licenseInfo.Text = "This project is licensed under the MIT License.";
            // 
            // versionLabel
            // 
            versionLabel.AutoSize = true;
            versionLabel.BackColor = Color.Transparent;
            versionLabel.ForeColor = Color.Aqua;
            versionLabel.Location = new Point(78, 45);
            versionLabel.Name = "versionLabel";
            versionLabel.Size = new Size(117, 15);
            versionLabel.TabIndex = 1;
            versionLabel.Text = $"Version: Release {ProductVersion}";
            // 
            // descriptionLabel
            // 
            descriptionLabel.AutoSize = true;
            descriptionLabel.BackColor = Color.Transparent;
            descriptionLabel.ForeColor = Color.Aqua;
            descriptionLabel.Location = new Point(38, 155);
            descriptionLabel.Name = "descriptionLabel";
            descriptionLabel.Size = new Size(196, 15);
            descriptionLabel.TabIndex = 2;
            descriptionLabel.Text = "Contact me on Discord: @Jettcodey";
            // 
            // okButton
            // 
            okButton.BackColor = SystemColors.Control;
            okButton.Location = new Point(94, 210);
            okButton.Name = "okButton";
            okButton.Size = new Size(75, 23);
            okButton.TabIndex = 3;
            okButton.Text = "OK";
            okButton.UseVisualStyleBackColor = false;
            okButton.Click += OkButton_Click;
            // 
            // linkLabel
            // 
            linkLabel.AutoSize = true;
            linkLabel.BackColor = Color.Transparent;
            linkLabel.LinkColor = Color.Aquamarine;
            linkLabel.Location = new Point(36, 175);
            linkLabel.Name = "linkLabel";
            linkLabel.Size = new Size(199, 15);
            linkLabel.TabIndex = 5;
            linkLabel.TabStop = true;
            linkLabel.Text = "https://www.github.com/Jettcodey/";
            linkLabel.VisitedLinkColor = Color.Aquamarine;
            linkLabel.LinkClicked += LinkLabel_LinkClicked;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.Transparent;
            label2.ForeColor = Color.Aqua;
            label2.Location = new Point(78, 80);
            label2.Name = "label2";
            label2.Size = new Size(109, 15);
            label2.TabIndex = 7;
            label2.Text = "TikTok Downloader";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.Transparent;
            label3.ForeColor = Color.Aqua;
            label3.Location = new Point(14, 100);
            label3.Name = "label3";
            label3.Size = new Size(234, 15);
            label3.TabIndex = 8;
            label3.Text = "Made w/ Love by Jettcodey && Contributors";
            // 
            // AboutWindow
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(32, 32, 32);
            ClientSize = new Size(269, 251);
            Controls.Add(titleLabel);
            Controls.Add(versionLabel);
            Controls.Add(label2);
            Controls.Add(label3);
            Controls.Add(licenseInfo);
            Controls.Add(descriptionLabel);
            Controls.Add(linkLabel);
            Controls.Add(okButton);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AboutWindow";
            Text = $"TikTok Downloader v{ProductVersion}";
            ResumeLayout(false);
            PerformLayout();
        }

        private void OkButton_Click(object? sender, EventArgs e)
        {
            this.Close();
        }

        private async void LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = linkLabel.Text;

            string browserPath = await browserUtility.GetBrowserExecutablePath();

            try
            {
                Process.Start(new ProcessStartInfo(browserPath, url));
            }
            catch (Exception ex)
            {
                using (var errorDialog = new Form())
                {
                    errorDialog.Text = "Error Opening Link";
                    errorDialog.Size = new Size(400, 200);

                    var errorMessageTextBox = new TextBox();
                    errorMessageTextBox.Multiline = true;
                    errorMessageTextBox.ReadOnly = true;
                    errorMessageTextBox.ScrollBars = ScrollBars.Vertical;
                    errorMessageTextBox.Dock = DockStyle.Fill;
                    errorMessageTextBox.Text = $"An error occurred:\n\n{ex.Message}";

                    errorDialog.Controls.Add(errorMessageTextBox);

                    var okButton = new Button();
                    okButton.Text = "OK";
                    okButton.Dock = DockStyle.Bottom;
                    okButton.Click += (s, ev) => errorDialog.Close();
                    errorDialog.Controls.Add(okButton);

                    errorDialog.ShowDialog();
                }
            }
        }

    }
}
