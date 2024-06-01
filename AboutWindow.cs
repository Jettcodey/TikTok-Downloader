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
        private Label label2;
        private LinkLabel linkLabel;

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutWindow));
            titleLabel = new Label();
            versionLabel = new Label();
            descriptionLabel = new Label();
            okButton = new Button();
            linkLabel = new LinkLabel();
            label2 = new Label();
            SuspendLayout();
            // 
            // titleLabel
            // 
            titleLabel.AutoSize = true;
            titleLabel.BackColor = Color.Transparent;
            titleLabel.Font = new Font("Arial", 12F, FontStyle.Bold);
            titleLabel.ForeColor = Color.Aquamarine;
            titleLabel.Location = new Point(40, 27);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new Size(157, 19);
            titleLabel.TabIndex = 0;
            titleLabel.Text = "TikTok Downloader";
            // 
            // versionLabel
            // 
            versionLabel.AutoSize = true;
            versionLabel.BackColor = Color.Transparent;
            versionLabel.ForeColor = Color.Aqua;
            versionLabel.Location = new Point(60, 65);
            versionLabel.Name = "versionLabel";
            versionLabel.Size = new Size(117, 15);
            versionLabel.TabIndex = 1;
            versionLabel.Text = "Version: Release 1.2.6";
            // 
            // descriptionLabel
            // 
            descriptionLabel.AutoSize = true;
            descriptionLabel.BackColor = Color.Transparent;
            descriptionLabel.ForeColor = Color.Aqua;
            descriptionLabel.Location = new Point(21, 128);
            descriptionLabel.Name = "descriptionLabel";
            descriptionLabel.Size = new Size(196, 15);
            descriptionLabel.TabIndex = 2;
            descriptionLabel.Text = "Contact me on Discord: @Jettcodey";
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
            linkLabel.Location = new Point(20, 145);
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
            label2.Location = new Point(65, 80);
            label2.Name = "label2";
            label2.Size = new Size(107, 15);
            label2.TabIndex = 7;
            label2.Text = "TikTok Downloader";
            // 
            // AboutWindow
            // 
            BackColor = SystemColors.ControlDarkDark;
            BackgroundImage = Properties.Resources.bg;
            ClientSize = new Size(240, 217);
            Controls.Add(label2);
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

        private async void LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = linkLabel.Text;

            string browserPath = await browserUtility.GetSystemDefaultBrowser();

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
