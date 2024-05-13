using System.Xml.Serialization;
using static TikTok_Downloader.AppSettings;


namespace TikTok_Downloader
{
    public partial class SettingsDialog : Form
    {

        private string SettingsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Jettcodey", "TikTok Downloader", "appsettings.xml");
        public bool UseOldFileStructure
        {
            get { return setting3CheckBox.Checked; }
        }

        private Label descriptionLabel;
        private Button okButton;
        private Button saveButton;
        private Button exportButton;
        private Button loadButton;
        private CheckBox setting1CheckBox;
        private CheckBox setting2CheckBox;
        private CheckBox setting3CheckBox;
        private CheckBox setting4CheckBox;
        private AppSettings.Settings settings = new AppSettings.Settings();

        private MainForm mainForm;
        public SettingsDialog(MainForm mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            LoadExistingSettings();
        }

        private void Setting3CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                mainForm.SetUseOldFileStructure(checkBox.Checked);
            }
        }

        private void Setting2CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                mainForm.LogJsonCheckBox(checkBox.Checked);
            }
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsDialog));
            descriptionLabel = new Label();
            okButton = new Button();
            saveButton = new Button();
            exportButton = new Button();
            loadButton = new Button();
            setting1CheckBox = new CheckBox();
            setting2CheckBox = new CheckBox();
            setting3CheckBox = new CheckBox();
            setting4CheckBox = new CheckBox();
            SuspendLayout();
            // 
            // descriptionLabel
            // 
            descriptionLabel.AutoSize = true;
            descriptionLabel.BackColor = Color.Transparent;
            descriptionLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            descriptionLabel.ForeColor = SystemColors.Control;
            descriptionLabel.Location = new Point(12, 12);
            descriptionLabel.Name = "descriptionLabel";
            descriptionLabel.Size = new Size(224, 21);
            descriptionLabel.TabIndex = 0;
            descriptionLabel.Text = "TikTok Downloader Settings";
            descriptionLabel.TextAlign = ContentAlignment.TopCenter;
            // 
            // okButton
            // 
            okButton.BackColor = SystemColors.Control;
            okButton.Location = new Point(15, 175);
            okButton.Name = "okButton";
            okButton.Size = new Size(75, 23);
            okButton.TabIndex = 1;
            okButton.Text = "OK";
            okButton.UseVisualStyleBackColor = false;
            okButton.Click += OkButton_Click;
            // 
            // saveButton
            // 
            saveButton.BackColor = SystemColors.Control;
            saveButton.Location = new Point(107, 175);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(75, 23);
            saveButton.TabIndex = 2;
            saveButton.Text = "Save";
            saveButton.UseVisualStyleBackColor = false;
            saveButton.Click += SaveButton_Click;
            // 
            // exportButton
            // 
            exportButton.BackColor = SystemColors.Control;
            exportButton.Location = new Point(202, 175);
            exportButton.Name = "exportButton";
            exportButton.Size = new Size(75, 23);
            exportButton.TabIndex = 3;
            exportButton.Text = "Export";
            exportButton.UseVisualStyleBackColor = false;
            exportButton.Click += ExportButton_Click;
            // 
            // loadButton
            // 
            loadButton.BackColor = SystemColors.Control;
            loadButton.Location = new Point(300, 175);
            loadButton.Name = "loadButton";
            loadButton.Size = new Size(75, 23);
            loadButton.TabIndex = 4;
            loadButton.Text = "Load";
            loadButton.UseVisualStyleBackColor = false;
            loadButton.Click += LoadButton_Click;
            // 
            // setting1CheckBox
            // 
            setting1CheckBox.AutoSize = true;
            setting1CheckBox.BackColor = Color.Transparent;
            setting1CheckBox.ForeColor = SystemColors.Control;
            setting1CheckBox.Location = new Point(15, 95);
            setting1CheckBox.Name = "setting1CheckBox";
            setting1CheckBox.Size = new Size(146, 19);
            setting1CheckBox.TabIndex = 5;
            setting1CheckBox.Text = "Download Videos Only";
            setting1CheckBox.UseVisualStyleBackColor = false;
            setting1CheckBox.Visible = false;
            // 
            // setting2CheckBox
            // 
            setting2CheckBox.AutoSize = true;
            setting2CheckBox.BackColor = Color.Transparent;
            setting2CheckBox.ForeColor = SystemColors.Control;
            setting2CheckBox.Location = new Point(15, 70);
            setting2CheckBox.Name = "setting2CheckBox";
            setting2CheckBox.Size = new Size(115, 19);
            setting2CheckBox.TabIndex = 6;
            setting2CheckBox.Text = "Enable Json Logs";
            setting2CheckBox.UseVisualStyleBackColor = false;
            setting2CheckBox.CheckedChanged += Setting2CheckBox_CheckedChanged;
            // 
            // setting3CheckBox
            // 
            setting3CheckBox.AutoSize = true;
            setting3CheckBox.BackColor = Color.Transparent;
            setting3CheckBox.ForeColor = SystemColors.Control;
            setting3CheckBox.Location = new Point(15, 45);
            setting3CheckBox.Name = "setting3CheckBox";
            setting3CheckBox.Size = new Size(139, 19);
            setting3CheckBox.TabIndex = 7;
            setting3CheckBox.Text = "Use Old File Structure";
            setting3CheckBox.UseVisualStyleBackColor = false;
            setting3CheckBox.CheckedChanged += Setting3CheckBox_CheckedChanged;
            // 
            // setting4CheckBox
            // 
            setting4CheckBox.AutoSize = true;
            setting4CheckBox.BackColor = Color.Transparent;
            setting4CheckBox.ForeColor = SystemColors.Control;
            setting4CheckBox.Location = new Point(15, 120);
            setting4CheckBox.Name = "setting4CheckBox";
            setting4CheckBox.Size = new Size(149, 19);
            setting4CheckBox.TabIndex = 7;
            setting4CheckBox.Text = "Download Images Only";
            setting4CheckBox.UseVisualStyleBackColor = false;
            setting4CheckBox.Visible = false;
            // 
            // SettingsDialog
            // 
            BackColor = SystemColors.ControlDarkDark;
            BackgroundImage = Properties.Resources.bg;
            ClientSize = new Size(390, 250);
            Controls.Add(descriptionLabel);
            Controls.Add(okButton);
            Controls.Add(saveButton);
            Controls.Add(exportButton);
            Controls.Add(loadButton);
            Controls.Add(setting1CheckBox);
            Controls.Add(setting2CheckBox);
            Controls.Add(setting3CheckBox);
            Controls.Add(setting4CheckBox);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SettingsDialog";
            Text = "TikTok Downloader Settings";
            ResumeLayout(false);
            PerformLayout();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
            this.Close();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void SaveSettings()
        {
            try
            {
                settings.DownloadVideosOnly = setting1CheckBox.Checked;
                settings.DownloadImagesOnly = setting4CheckBox.Checked;
                settings.EnableJsonLogs = setting2CheckBox.Checked;
                settings.UseOldFileStructure = setting3CheckBox.Checked;

                using (StreamWriter writer = new StreamWriter(SettingsFilePath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(AppSettings.Settings));
                    serializer.Serialize(writer, settings);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving settings: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadExistingSettings()
        {
            try
            {
                if (File.Exists(SettingsFilePath))
                {
                    using (StreamReader reader = new StreamReader(SettingsFilePath))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(AppSettings.Settings));
                        settings = (AppSettings.Settings)serializer.Deserialize(reader);
                    }
                    setting1CheckBox.Checked = settings.DownloadVideosOnly;
                    setting4CheckBox.Checked = settings.DownloadImagesOnly;
                    setting2CheckBox.Checked = settings.EnableJsonLogs;
                    setting3CheckBox.Checked = settings.UseOldFileStructure;
                }
                else
                {
                    settings = new AppSettings.Settings();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading existing settings: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void LoadButton_Click(object sender, EventArgs e)
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                openFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var streamReader = new StreamReader(openFileDialog.FileName))
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(AppSettings.Settings));
                            AppSettings.Settings settings = (AppSettings.Settings)serializer.Deserialize(streamReader);
                            setting1CheckBox.Checked = settings.DownloadVideosOnly;
                            setting4CheckBox.Checked = settings.DownloadImagesOnly;
                            setting2CheckBox.Checked = settings.EnableJsonLogs;
                            setting3CheckBox.Checked = settings.UseOldFileStructure;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }


        private void ExportButton_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                saveFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        AppSettings.Settings settings = new AppSettings.Settings
                        {
                            DownloadVideosOnly = setting1CheckBox.Checked,
                            DownloadImagesOnly = setting4CheckBox.Checked,
                            EnableJsonLogs = setting2CheckBox.Checked,
                            UseOldFileStructure = setting3CheckBox.Checked
                        };

                        XmlSerializer serializer = new XmlSerializer(typeof(AppSettings.Settings));
                        using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                        {
                            serializer.Serialize(writer, settings);
                        }

                        MessageBox.Show("Settings exported successfully.", "Export Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error exporting settings: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
