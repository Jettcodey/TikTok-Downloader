/*
##########################################
#           TikTok Downloader            #
#           Made by Jettcodey            #
#                © 2024                  #
#           DO NOT REMOVE THIS           #
##########################################
*/
using System.Xml.Serialization;
using static TikTok_Downloader.MainForm;


namespace TikTok_Downloader
{
    public class AppSettings
    {
        private readonly BrowserUtility browserUtility;

        internal AppSettings(BrowserUtility browserUtility)
        {
            this.browserUtility = browserUtility;
        }

        public class Settings
        {
            public bool ToastsAllowed { get; set; }
            public bool EnableJsonLogs { get; set; }
            public bool EnableDownloadLogs { get; set; }
            public string LastDownloadFolderPath { get; set; }
            public string LastDownloadOption { get; set; }
            public string LastBrowsingPath { get; set; }
            public bool FirstRun { get; set; }
            public string CustomBrowserPath { get; set; }
        }

        private readonly string directoryPath;
        private readonly string filePath;

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
                    ToastsAllowed = false,
                    EnableJsonLogs = false,
                    EnableDownloadLogs = false,
                    LastDownloadFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TiktokDownloads"),
                    LastDownloadOption = "Single Video/Image Download",
                    LastBrowsingPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    FirstRun = false,
                    CustomBrowserPath = "",
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
                        CurrentSettings = serializer.Deserialize(streamReader) as Settings;
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
