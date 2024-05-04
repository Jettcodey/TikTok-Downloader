using System.Xml.Serialization;

namespace TikTok_Downloader
{
    public class AppSettings
    {
        public class Settings
        {
            public bool DownloadVideosOnly { get; set; }
            public bool DownloadImagesOnly { get; set; }
            public bool EnableJsonLogs { get; set; }
            public bool UseOldFileStructure { get; set; }
            public string LastDownloadFolderPath { get; set; }
        }

        private string directoryPath;
        private string filePath;

        public Settings CurrentSettings { get; set; }

        public AppSettings()
        {
            directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Jettcodey\\TikTok Downloader");
            filePath = Path.Combine(directoryPath, "appsettings.xml");

            CurrentSettings = new Settings();
        }

        public void SaveSettings()
        {
            try
            {
                // Create the directory if it doesn't exist
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

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
                    EnableJsonLogs = false,
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
