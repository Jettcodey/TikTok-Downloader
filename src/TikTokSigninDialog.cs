/*
##########################################
#           TikTok Downloader            #
#           Made by Jettcodey            #
#                © 2024                  #
#           DO NOT REMOVE THIS           #
##########################################
*/
using static TikTok_Downloader.MainForm;

namespace TikTok_Downloader
{
    public partial class TikTokSigninDialog : Form
    {
        private readonly BrowserUtility browserUtility;
        internal TikTokSigninDialog(BrowserUtility browserUtility)
        {
            InitializeComponent();
            this.browserUtility = browserUtility;
        }
        public TikTokSigninDialog()
        {
            InitializeComponent();
        }
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TikTokSigninDialog));
            titleLabel = new Label();
            textLabel = new Label();
            closeButton = new Button();
            SuspendLayout();
            // 
            // titleLabel
            // 
            titleLabel.AutoSize = true;
            titleLabel.BackColor = Color.Transparent;
            titleLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            titleLabel.ForeColor = SystemColors.Control;
            titleLabel.Location = new Point(80, 9);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new Size(119, 21);
            titleLabel.TabIndex = 0;
            titleLabel.Text = "TikTok Sign-in";
            // 
            // textLabel
            // 
            textLabel.AutoSize = true;
            textLabel.BackColor = Color.Transparent;
            textLabel.Font = new Font("Segoe UI", 9F);
            textLabel.ForeColor = SystemColors.Control;
            textLabel.Location = new Point(42, 42);
            textLabel.Name = "textLabel";
            textLabel.Size = new Size(190, 30);
            textLabel.TabIndex = 1;
            textLabel.Text = "This Feature is still in Development\n and therefore not Available.";
            textLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // closeButton
            // 
            closeButton.ForeColor = SystemColors.ControlText;
            closeButton.Location = new Point(109, 226);
            closeButton.Name = "closeButton";
            closeButton.Size = new Size(75, 23);
            closeButton.TabIndex = 2;
            closeButton.Text = "Close";
            closeButton.UseVisualStyleBackColor = true;
            closeButton.Click += closeButton_Click;
            // 
            // TikTokSigninDialog
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(32, 32, 32);
            ClientSize = new Size(284, 261);
            Controls.Add(titleLabel);
            Controls.Add(textLabel);
            Controls.Add(closeButton);
            ForeColor = SystemColors.Control;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "TikTokSigninDialog";
            Text = $"TikTok Downloader v{ProductVersion}";
            ResumeLayout(false);
            PerformLayout();
        }
        private Label titleLabel;
        private Label textLabel;
        private Button closeButton;

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}