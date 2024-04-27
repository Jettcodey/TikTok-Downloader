using System.Xml.Serialization;

namespace TikTok_Downloader
{
    public class AppSettings
    {
        public class Settings
        {
            public bool DownloadVideosOnly { get; set; }
            public bool DownloadImagesOnly { get; set; }
            public bool DisableJsonLogs { get; set; }
            public bool UseOldFileStructure { get; set; }
            public string LastDownloadFolderPath { get; set; }
        }

        private string filePath = "appsettings.xml";
        public Settings CurrentSettings { get; set; }

        public AppSettings()
        {
            CurrentSettings = new Settings();
        }

        public void SaveSettings()
        {
            try
            {
                using (var streamWriter = new StreamWriter(filePath))
                {
                    var serializer = new XmlSerializer(typeof(Settings));
                    serializer.Serialize(streamWriter, CurrentSettings);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void LoadSettings()
        {
            if (!File.Exists(filePath))
            {
                CurrentSettings = new Settings
                {
                    DownloadVideosOnly = false,
                    DownloadImagesOnly = false,
                    DisableJsonLogs = false,
                    UseOldFileStructure = false,
                    LastDownloadFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TiktokDownloads")
                };
                SaveSettings();
            }
            else
            {
                try
                {
                    using (var streamReader = new StreamReader(filePath))
                    {
                        var serializer = new XmlSerializer(typeof(Settings));
                        CurrentSettings = (Settings)serializer.Deserialize(streamReader);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
