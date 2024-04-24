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
        private Label versionLabel;
        private Label descriptionLabel;
        private Button okButton;
        private Label label1;
        private LinkLabel linkLabel;

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutWindow));
            titleLabel = new Label();
            versionLabel = new Label();
            descriptionLabel = new Label();
            okButton = new Button();
            linkLabel = new LinkLabel();
            label1 = new Label();
            SuspendLayout();
            // 
            // titleLabel
            // 
            titleLabel.AutoSize = true;
            titleLabel.BackColor = Color.Transparent;
            titleLabel.Font = new Font("Arial", 12F, FontStyle.Bold);
            titleLabel.ForeColor = Color.Aquamarine;
            titleLabel.Location = new Point(45, 27);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new Size(157, 19);
            titleLabel.TabIndex = 0;
            titleLabel.Text = "TikTok Downloader";
            // 
            // versionLabel
            // 
            versionLabel.AutoSize = true;
            versionLabel.BackColor = Color.Transparent;
            versionLabel.ForeColor = Color.Aquamarine;
            versionLabel.Location = new Point(65, 65);
            versionLabel.Name = "versionLabel";
            versionLabel.Size = new Size(117, 15);
            versionLabel.TabIndex = 1;
            versionLabel.Text = "Version: Release 1.1.8";
            // 
            // descriptionLabel
            // 
            descriptionLabel.AutoSize = true;
            descriptionLabel.BackColor = Color.Transparent;
            descriptionLabel.ForeColor = Color.Aqua;
            descriptionLabel.Location = new Point(45, 109);
            descriptionLabel.Name = "descriptionLabel";
            descriptionLabel.Size = new Size(149, 15);
            descriptionLabel.TabIndex = 2;
            descriptionLabel.Text = "Simple TikTok Downloader.";
            // 
            // okButton
            // 
            okButton.BackColor = SystemColors.Control;
            okButton.Location = new Point(80, 175);
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
            linkLabel.Location = new Point(20, 155);
            linkLabel.Name = "linkLabel";
            linkLabel.Size = new Size(199, 15);
            linkLabel.TabIndex = 5;
            linkLabel.TabStop = true;
            linkLabel.Text = "https://www.github.com/Jettcodey/";
            linkLabel.VisitedLinkColor = Color.Aquamarine;
            linkLabel.LinkClicked += LinkLabel_LinkClicked;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.ForeColor = Color.Aqua;
            label1.Location = new Point(60, 140);
            label1.Name = "label1";
            label1.Size = new Size(117, 15);
            label1.TabIndex = 6;
            label1.Text = "Made by @Jettcodey";
            // 
            // AboutWindow
            // 
            BackColor = SystemColors.ControlDarkDark;
            BackgroundImage = Properties.Resources.bg;
            ClientSize = new Size(240, 217);
            Controls.Add(label1);
            Controls.Add(linkLabel);
            Controls.Add(titleLabel);
            Controls.Add(versionLabel);
            Controls.Add(descriptionLabel);
            Controls.Add(okButton);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AboutWindow";
            Text = "TikTok Downloader by Jettcodey";
            ResumeLayout(false);
            PerformLayout();
        }

        private void OkButton_Click(object? sender, EventArgs e)
        {
            this.Close();
        }

        private void LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = linkLabel.Text;

            string browserPath = browserUtility.GetSystemDefaultBrowser();

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
