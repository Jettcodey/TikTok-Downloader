/*
##########################################
#           TikTok Downloader            #
#           Made by Jettcodey            #
#                © 2024                  #
#           DO NOT REMOVE THIS           #
##########################################
*/
using System.Xml.Serialization;
using Microsoft.Win32;
using static TikTok_Downloader.AppSettings;
using static TikTok_Downloader.MainForm;

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
        private CheckBox setting5CheckBox;
        private ComboBox browserComboBox;
        private AppSettings.Settings settings = new AppSettings.Settings();
        private Label BrowserSelect;
        private MainForm mainForm;
        public SettingsDialog(MainForm mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            LoadExistingSettings();
            browserComboBox.SelectedItem = "System Default";
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

        private void Setting5CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                mainForm.EnableDownloadLogsCheckBox(checkBox.Checked);
            }
        }

        private void Setting1CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                mainForm.DownloadVideosOnlyCheckBox(checkBox.Checked);
            }
        }

        private void Setting4CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                mainForm.DownloadImagesOnlyCheckBox(checkBox.Checked);
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
            setting5CheckBox = new CheckBox();
            browserComboBox = new ComboBox();
            BrowserSelect = new Label();
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
            setting1CheckBox.CheckedChanged += Setting1CheckBox_CheckedChanged;
            // 
            // setting2CheckBox
            // 
            setting2CheckBox.AutoSize = true;
            setting2CheckBox.BackColor = Color.Transparent;
            setting2CheckBox.ForeColor = SystemColors.Control;
            setting2CheckBox.Location = new Point(167, 70);
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
            setting4CheckBox.Location = new Point(166, 95);
            setting4CheckBox.Name = "setting4CheckBox";
            setting4CheckBox.Size = new Size(149, 19);
            setting4CheckBox.TabIndex = 7;
            setting4CheckBox.Text = "Download Images Only";
            setting4CheckBox.UseVisualStyleBackColor = false;
            setting4CheckBox.Visible = false;
            setting4CheckBox.CheckedChanged += Setting4CheckBox_CheckedChanged;
            // 
            // setting5CheckBox
            // 
            setting5CheckBox.AutoSize = true;
            setting5CheckBox.BackColor = Color.Transparent;
            setting5CheckBox.ForeColor = SystemColors.Control;
            setting5CheckBox.Location = new Point(15, 70);
            setting5CheckBox.Name = "setting5CheckBox";
            setting5CheckBox.Size = new Size(146, 19);
            setting5CheckBox.TabIndex = 7;
            setting5CheckBox.Text = "Enable Download Logs";
            setting5CheckBox.UseVisualStyleBackColor = false;
            setting5CheckBox.CheckedChanged += Setting5CheckBox_CheckedChanged;
            // 
            // browserComboBox
            // 
            browserComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            browserComboBox.FormattingEnabled = true;
            browserComboBox.Items.AddRange(new object[] { "System Default", "Google Chrome", "Mozilla Firefox", "Microsoft Edge", "Chromium", "Brave Browser", "Opera GX (Not Working!)" });
            browserComboBox.Location = new Point(12, 134);
            browserComboBox.Name = "browserComboBox";
            browserComboBox.Size = new Size(300, 23);
            browserComboBox.TabIndex = 8;
            browserComboBox.SelectedIndexChanged += BrowserComboBox_SelectedIndexChanged;
            // 
            // BrowserSelect
            // 
            BrowserSelect.AutoSize = true;
            BrowserSelect.BackColor = Color.Transparent;
            BrowserSelect.ForeColor = SystemColors.Control;
            BrowserSelect.Location = new Point(11, 117);
            BrowserSelect.Name = "BrowserSelect";
            BrowserSelect.Size = new Size(143, 15);
            BrowserSelect.TabIndex = 9;
            BrowserSelect.Text = "Select a Custom Browser: ";
            // 
            // SettingsDialog
            // 
            BackColor = SystemColors.ControlDarkDark;
            BackgroundImage = Properties.Resources.bg;
            ClientSize = new Size(400, 250);
            Controls.Add(BrowserSelect);
            Controls.Add(descriptionLabel);
            Controls.Add(okButton);
            Controls.Add(saveButton);
            Controls.Add(exportButton);
            Controls.Add(loadButton);
            Controls.Add(setting1CheckBox);
            Controls.Add(setting2CheckBox);
            Controls.Add(setting3CheckBox);
            Controls.Add(setting4CheckBox);
            Controls.Add(setting5CheckBox);
            Controls.Add(browserComboBox);
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
                settings.EnableDownloadLogs = setting5CheckBox.Checked;
                settings.UseOldFileStructure = setting3CheckBox.Checked;

                string selectedBrowser = browserComboBox.SelectedItem.ToString();
                settings.CustomBrowserPath = GetBrowserPath(selectedBrowser);

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
                    setting5CheckBox.Checked = settings.EnableDownloadLogs;
                    setting3CheckBox.Checked = settings.UseOldFileStructure;

                    string selectedBrowser = settings.CustomBrowserPath;
                    if (!string.IsNullOrWhiteSpace(selectedBrowser))
                    {
                        foreach (var item in browserComboBox.Items)
                        {
                            if (selectedBrowser.Contains(item.ToString()))
                            {
                                browserComboBox.SelectedItem = item;
                                break;
                            }
                        }
                    }
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
                            setting5CheckBox.Checked = settings.EnableDownloadLogs;
                            setting3CheckBox.Checked = settings.UseOldFileStructure;
                            browserComboBox.SelectedItem = GetBrowserNameFromPath(settings.CustomBrowserPath);
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
                            EnableDownloadLogs = setting5CheckBox.Checked,
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

        private string GetBrowserPath(string browserName)
        {
            string browserPath = string.Empty;
            try
            {
                if (string.Equals(browserName, "System Default", StringComparison.OrdinalIgnoreCase))
                {
                    var browserUtility = new BrowserUtility(mainForm, mainForm.AppSettings);
                    browserPath = Task.Run(() => browserUtility.GetSystemDefaultBrowser()).Result;
                }
                else if (string.Equals(browserName, "Mozilla Firefox", StringComparison.OrdinalIgnoreCase))
                {
                    browserPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"ms-playwright\firefox-1447\firefox\firefox.exe");
                }
                else if (string.Equals(browserName, "Chromium", StringComparison.OrdinalIgnoreCase))
                {
                    browserPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Chromium\Application\chrome.exe");
                }
                else
                {
                    string registryKeyPath = string.Empty;
                    switch (browserName)
                    {
                        case "Google Chrome":
                            registryKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe";
                            break;
                        case "Microsoft Edge":
                            registryKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\msedge.exe";
                            break;
                        case "Brave Browser":
                            registryKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\brave.exe";
                            break;
                        case "Opera GX":
                            registryKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\opera.exe";
                            break;
                        default:
                            throw new Exception("Unsupported browser selected.");
                    }

                    using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKeyPath))
                    {
                        if (key != null)
                        {
                            browserPath = key.GetValue("") as string;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting browser path: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return browserPath;
        }

        private void BrowserComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (browserComboBox.SelectedItem != null)
            {
                string browserName = browserComboBox.SelectedItem.ToString();
                settings.CustomBrowserPath = GetBrowserPath(browserName);

                mainForm.AppSettings.CurrentSettings.CustomBrowserPath = settings.CustomBrowserPath;
                mainForm.AppSettings.SaveSettings();

            }
        }

        private string GetBrowserNameFromPath(string browserPath)
        {
            if (browserPath.Contains("chrome.exe"))
            {
                if (browserPath.Contains("Chromium"))
                {
                    return "Chromium";
                }
                return "Google Chrome";
            }
            if (browserPath.Contains("firefox.exe")) return "Mozilla Firefox";
            if (browserPath.Contains("msedge.exe")) return "Microsoft Edge";
            if (browserPath.Contains("brave.exe")) return "Brave Browser";
            if (browserPath.Contains("opera.exe")) return "Opera GX";
            return "System Default";
        }
    }
}
