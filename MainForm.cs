using Microsoft.Playwright;
using Microsoft.Win32;
using System.Globalization;
using System.Management;
using System.Net;
using System.Reflection;
using System.Security.Policy;
using System.Text.Json;

namespace TikTok_Downloader
{
    public partial class MainForm : Form
    {
        private readonly string logFolderPath;
        private readonly string logFilePath;
        string downloadFolderPath;
        private readonly BrowserUtility browserUtility;
        private bool logJsonEnabled;
        private readonly string jsonLogFilePath;
        private readonly object jsonLock = new object();
        private bool useOldFileStructure;      
        private readonly AppSettings settings;
        private SettingsDialog settingsDialog;
        private List<string> cachedVideoUrls = new List<string>();



        private Task LogSystemInformation(string logFilePath)
        {
            try
            {
                string systemInfo = "";

                systemInfo += $"Region and Language: {CultureInfo.CurrentCulture.DisplayName}\n";

                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor"))
                {
                    foreach (var queryObj in searcher.Get())
                    {
                        systemInfo += $"CPU: {queryObj["Name"]}\n";
                    }
                }

                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController"))
                {
                    foreach (var queryObj in searcher.Get())
                    {
                        systemInfo += $"GPU: {queryObj["Name"]}\n";
                    }
                }

                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem"))
                {
                    foreach (var queryObj in searcher.Get())
                    {
                        double ramBytes = Convert.ToDouble(queryObj["TotalPhysicalMemory"]);
                        double ramGB = ramBytes / (1024 * 1024 * 1024);
                        ramGB = Math.Ceiling(ramGB);
                        systemInfo += $"RAM: {ramGB} GB\n";
                    }
                }

                var driveInfo = new DriveInfo(Path.GetPathRoot(downloadFolderPath));
                double freeSpaceGB = driveInfo.AvailableFreeSpace / (1024 * 1024 * 1024);
                systemInfo += $"Free Storage on Download Drive: {freeSpaceGB} GB\n";

                LogMessage(logFilePath, systemInfo);
            }
            catch (Exception ex)
            {
                LogError($"Error getting system information: {ex.Message}");
            }

            return Task.CompletedTask;
        }

        public MainForm()
        {
            InitializeComponent();
            LoadDownloadFolderPath();
            settings = new AppSettings();
            settings.LoadSettings();
            this.settingsDialog = settingsDialog;
            string logFolderName = $"TTDownloader-Logs[{DateTime.Now:yyyy-MM-dd_HH-mm}]-Logs";
            string logFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), logFolderName);


            try
            {
                Directory.CreateDirectory(logFolderPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating log directory: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            logFilePath = Path.Combine(logFolderPath, $"TikTokDownloaderLog_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt");
            jsonLogFilePath = Path.Combine(logFolderPath, $"JSON_Log_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.json");

            try
            {
                Directory.CreateDirectory(logFolderPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating log directory: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            Directory.CreateDirectory(downloadFolderPath);

            Task.Run(async () =>
            {
                await LogSystemInformation(logFilePath);
            }).Wait();

            LogMessage(logFilePath, $"Initial download folder: {downloadFolderPath}");

            browserUtility = new BrowserUtility(this);

            cmbChoice.SelectedItem = "Single Video/Image Download";

            outputTextBox.ReadOnly = true;
        }


        private void LoadDownloadFolderPath()
        {
            AppSettings appSettings = new AppSettings();
            appSettings.LoadSettings();
            downloadFolderPath = appSettings.CurrentSettings.LastDownloadFolderPath;
            if (string.IsNullOrEmpty(downloadFolderPath))
            {
                downloadFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TiktokDownloads");
            }
        }


        private void LogMessage(string logFilePath, string message)
        {
            lock (logFilePath)
            {
                string redactedMessage = message.Replace(Environment.UserName, "[RedactedUsername]");
                File.AppendAllText(logFilePath, $"{DateTime.Now}: {redactedMessage}\n");
            }
        }

        private void LogDownload(string fileName, string url)
        {
            LogMessage(logFilePath, $"Downloaded file: {fileName}, from URL: {url}");
        }

        
        private void LogJson(string fileName, string jsonContent, bool logJsonEnabled)
        {
            if (!logJsonEnabled)
            {
                return;
            }

            lock (jsonLock)
            {
                try
                {
                    using (StreamWriter writer = File.AppendText(jsonLogFilePath))
                    {
                        writer.WriteLine($"[{DateTime.Now}] File: {fileName}");
                        writer.WriteLine(jsonContent);
                        writer.WriteLine();
                    }
                }
                catch (Exception ex)
                {
                    outputTextBox.AppendText($"ERROR: An error occurred while logging JSON response: {ex.Message}\r\n");
                    Console.WriteLine($"Error occurred while logging JSON response: {ex}");
                }
            }
        }

        private void LogError(string errorMessage)
        {
            LogMessage(logFilePath, $"ERROR: {errorMessage}");
        }

        class VideoData
        {
            public string Url { get; set; } = string.Empty;
            public List<string> Images { get; set; } = new List<string>();
            public string Id { get; set; } = string.Empty;
        }

        class ApiData
        {
            public List<Aweme> aweme_list { get; set; } = new List<Aweme>();
        }

        class Aweme
        {
            public string aweme_id { get; set; } = string.Empty;
            public ImagePostInfo image_post_info { get; set; } = new ImagePostInfo();
            public Video video { get; set; } = new Video();
        }

        class ImagePostInfo
        {
            public List<Image> images { get; set; } = new List<Image>();
        }

        class Image
        {
            public DisplayImage display_image { get; set; } = new DisplayImage();
        }

        class DisplayImage
        {
            public List<string> url_list { get; set; } = new List<string>();
        }

        public class Video
        {
            public DownloadAddr download_addr { get; set; } = new DownloadAddr();
            public PlayAddr play_addr { get; set; } = new PlayAddr();

        }

        public class DownloadAddr
        {
            public List<string> url_list { get; set; } = new List<string>();
        }

        public class PlayAddr
        {
            public List<string> url_list { get; set; } = new List<string>();
        }

        private async void btnDownload_Click(object sender, EventArgs e)
        {
            outputTextBox.Clear();
            string choice = cmbChoice.SelectedItem.ToString();
            LogMessage(logFilePath, $"Selected option: {choice}");

            if (choice == "Single Video/Image Download")
            {
                await SingleVideoDownload();
            }
            else if (choice == "Mass Download by Username")
            {
                await MassDownloadByUsername();
            }
            else if (choice == "Mass Download from Text File Links")
            {
                string filePath = filePathTextBox.Text.Trim();
                LogMessage(logFilePath, $"Selected file path: {filePath}");
                await DownloadFromTextFile(filePath);
            }
        }

        internal class BrowserUtility
        {
            private readonly MainForm mainForm;

            public BrowserUtility(MainForm mainForm)
            {
                this.mainForm = mainForm;
            }

            public string GetSystemDefaultBrowser()
            {
                string name = string.Empty;
                RegistryKey regKey = null;

                try
                {
                    var regDefault = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\.htm\\UserChoice", false);
                    var stringDefault = regDefault.GetValue("ProgId");

                    regKey = Registry.ClassesRoot.OpenSubKey(stringDefault + "\\shell\\open\\command", false);
                    name = regKey.GetValue(null).ToString().ToLower().Replace("" + (char)34, "");

                    if (!name.EndsWith("exe"))
                        name = name.Substring(0, name.LastIndexOf(".exe") + 4);
                }
                catch (Exception ex)
                {
                    name = string.Format("ERROR: An exception of type: {0} occurred in method: {1} in the following module: {2}", ex.GetType(), ex.TargetSite, nameof(BrowserUtility));
                    mainForm.LogError(name);
                }
                finally
                {
                    if (regKey != null)
                        regKey.Close();
                }

                return name;
            }

            public IBrowserType GetBrowserTypeForExecutable(string executablePath, IPlaywright playwright)
            {
                if (executablePath.ToLower().Contains("firefox"))
                {
                    return playwright.Firefox;  // Firefox doesn´t seem to work with my current playwright configuration for now.
                }
                else if (executablePath.ToLower().Contains("webkit"))
                {
                    return playwright.Webkit;   // Webkit doesn´t seem to work with my current playwright configuration for now.
                }
                else
                {
                    return playwright.Chromium; // Chromium-based Browsers work with my playwright configuration.
                }
            }
        }

        private async Task MassDownloadByUsername()
        {
            string username = txtUsername.Text.Trim();
            LogMessage(logFilePath, $"Username selected for mass download: {username}");
            string baseUrl = $"https://www.tiktok.com/@{username}";

            // Get system default browser executable path
            string browserExecutablePath = browserUtility.GetSystemDefaultBrowser();
            LogMessage(logFilePath, $"System default browser executable path: {browserExecutablePath}");

            // Launch Playwright with the appropriate browser type
            using var playwright = await Playwright.CreateAsync();
            var browserType = browserUtility.GetBrowserTypeForExecutable(browserExecutablePath, playwright);

            var browser = await browserType.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
                ExecutablePath = browserExecutablePath
            });

            try
            {
                var context = await browser.NewContextAsync();
                var page = await context.NewPageAsync();

                // Navigate to the TikTok page
                await page.GotoAsync(baseUrl, new PageGotoOptions { Timeout = 120000 });
                LogMessage(logFilePath, $"Navigated to TikTok page: {baseUrl}");

                // Scroll to the bottom of the page repeatedly until no more videos are loaded
                while (true)
                {
                    long initialHeight = await page.EvaluateAsync<long>("() => document.body.scrollHeight");
                    await page.EvaluateAsync("window.scrollTo(0, document.body.scrollHeight)");
                    await page.WaitForTimeoutAsync(10000);
                    long newHeight = await page.EvaluateAsync<long>("() => document.body.scrollHeight");

                    if (newHeight == initialHeight)
                    {
                        break;
                    }
                }

                // Extract all video URLs
                var videoUrls = await page.EvaluateAsync<string[]>("() => { var videos = document.querySelectorAll('a'); var videoUrls = []; videos.forEach(video => { if (video.href.includes('/video/')) { videoUrls.push(video.href); } }); return videoUrls; }");
                LogMessage(logFilePath, $"Extracted {videoUrls.Length} video URLs");

                // Extract all image post URLs
                var imagePostUrls = await page.EvaluateAsync<string[]>("() => { var images = document.querySelectorAll('a'); var imageUrls = []; images.forEach(image => { if (image.href.includes('/photo/')) { imageUrls.push(image.href); } }); return imageUrls; }");
                LogMessage(logFilePath, $"Extracted {imagePostUrls.Length} image post URLs");

                // Combine video and image post URLs
                var combinedUrls = videoUrls.Concat(imagePostUrls).Distinct();
                LogMessage(logFilePath, $"Combined {combinedUrls.Count()} URLs");

                // Filter links by username
                var filteredUrls = combinedUrls.Where(url => url.Contains($"/@{username}/"));
                LogMessage(logFilePath, $"Filtered {filteredUrls.Count()} URLs by username");

                // Save all video and image post links to a single text file
                string combinedLinksFilePath = Path.Combine(downloadFolderPath, $"{username}_combined_links.txt");
                await File.WriteAllLinesAsync(combinedLinksFilePath, filteredUrls);
                LogMessage(logFilePath, $"Saved {filteredUrls.Count()} URLs to file: {combinedLinksFilePath}");

                // Close the page and context
                await page.CloseAsync();
                await context.CloseAsync();
                LogMessage(logFilePath, "Page and context closed successfully");

                // Download all Videos and Images from the combined links file
                await DownloadFromCombinedLinksFile(combinedLinksFilePath);
            }
            catch (Exception ex)
            {
                LogError($"An error occurred: {ex.Message}");
            }
            finally
            {
                await browser.CloseAsync();
                LogMessage(logFilePath, "Browser closed successfully");
            }
        }


        private async Task DownloadFromCombinedLinksFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                LogError("Combined links file not found!");
                return;
            }

            var urls = File.ReadAllLines(filePath);
            LogMessage(logFilePath, $"Read {urls.Length} URLs from file: {filePath}");

            progressBar.Minimum = 0;
            progressBar.Maximum = urls.Length;
            progressBar.Value = 0;

            using (var settingsDialog = new SettingsDialog(this))
            {
                foreach (var url in urls)
                {
                    var trimmedUrl = url.Trim();
                    if (!string.IsNullOrWhiteSpace(trimmedUrl))
                    {
                        LogMessage(logFilePath, $"Downloading {trimmedUrl}...");

                        var data = await GetVideo(trimmedUrl, withWatermarkCheckBox.Checked);

                        if (data == null)
                        {
                            LogError($"Error: Media from URL {trimmedUrl} was deleted!");
                            progressBar.Value++;
                            continue;
                        }

                        await DownloadMedia(data, trimmedUrl, settingsDialog.UseOldFileStructure);
                    }

                    progressBar.Value++;
                }
            }
        }


        private async Task DownloadFromTextFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                LogError("Text file not found!");
                return;
            }

            var urls = File.ReadAllLines(filePath);
            LogMessage(logFilePath, $"Read {urls.Length} URLs from file: {filePath}");

            progressBar.Minimum = 0;
            progressBar.Maximum = urls.Length;
            progressBar.Value = 0;

            using (var settingsDialog = new SettingsDialog(this))
            {
                bool useOldFileStructure = settingsDialog.UseOldFileStructure;

                foreach (var url in urls)
                {
                    var trimmedUrl = url.Trim();
                    if (!string.IsNullOrWhiteSpace(trimmedUrl))
                    {
                        LogMessage(logFilePath, $"Downloading {trimmedUrl} ...");

                        var data = await GetVideo(trimmedUrl, withWatermarkCheckBox.Checked);

                        if (data == null)
                        {
                            LogError($"Error: Media from URL {trimmedUrl} was deleted!");
                            progressBar.Value++;
                            continue;
                        }

                        await DownloadMedia(data, trimmedUrl, useOldFileStructure);
                    }

                    progressBar.Value++;
                }
            }
        }


        private async Task SingleVideoDownload()
        {
            outputTextBox.Clear();

            try
            {
                var url = urlTextBox.Text.Trim();
                if (string.IsNullOrWhiteSpace(url))
                {
                    MessageBox.Show("Please enter a valid URL", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var trimmedUrl = url.Trim();
                LogMessage(logFilePath, $"Downloading {trimmedUrl}...");

                var data = await GetVideo(trimmedUrl, withWatermarkCheckBox.Checked);

                if (data == null)
                {
                    LogError($"Error: Media from URL {trimmedUrl} was deleted!");
                    return;
                }

                using (var settingsDialog = new SettingsDialog(this))
                {
                    await DownloadMedia(data, trimmedUrl, settingsDialog.UseOldFileStructure);
                }
            }
            catch (Exception ex)
            {
                LogError($"An error occurred: {ex.Message}");
            }
        }

        private async Task<VideoData?> GetVideo(string url, bool withWatermark)
        {
            var idVideo = await GetIdVideo(url);
            var apiUrl = $"https://api22-normal-c-alisg.tiktokv.com/aweme/v1/feed/?aweme_id={idVideo}&iid=7318518857994389254&device_id=7318517321748022790&channel=googleplay&app_name=musical_ly&version_code=300904&device_platform=android&device_type=ASUS_Z01QD&version=9";

            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync(apiUrl);
                    response.EnsureSuccessStatusCode();
                    var json = await response.Content.ReadAsStringAsync();
                    LogJson($"API_Response_For_'{idVideo}'", json, logJsonEnabled);
                    var data = JsonSerializer.Deserialize<ApiData>(json);

                    if (data?.aweme_list == null || data.aweme_list.Count == 0)
                    {
                        return null;
                    }

                    var video = data.aweme_list.FirstOrDefault();

                    var urlMedia = withWatermark ? video?.video?.download_addr?.url_list.FirstOrDefault() : video?.video?.play_addr?.url_list.FirstOrDefault();
                    var imageUrls = video?.image_post_info?.images?.Select(img => img.display_image.url_list.FirstOrDefault()).ToList();

                    if (urlMedia == null)
                    {
                        LogMessage(logFilePath, $"Skipping download link for video ID: {idVideo} due to missing media URL.");
                        return null;
                    }

                    return new VideoData
                    {
                        Url = urlMedia,
                        Images = imageUrls ?? new List<string>(),
                        Id = idVideo
                    };
                }
                catch (HttpRequestException ex) when ((int)ex.StatusCode == 404)
                {
                    LogMessage(logFilePath, $"Skipping download link for video ID: {idVideo} due to 404 error.");
                    return null;
                }
                catch (Exception ex)
                {
                    LogError($"Error getting video: {ex.Message}");
                    return null;
                }
            }
        }


        private async Task<string> GetIdVideo(string url)
        {
            try
            {
                if (url.Contains("/t/"))
                {
                    using (var client = new HttpClient())
                    {
                        var response = await client.GetAsync(url);
                        return response.RequestMessage?.RequestUri.Segments.LastOrDefault()?.TrimEnd('/') ?? string.Empty;
                    }
                }

                var matching = url.Contains("/video/");
                var matchingPhoto = url.Contains("/photo/");
                var startIndex = url.IndexOf("/video/") + 7;
                var endIndex = url.IndexOf("/video/") + 26;

                if (matchingPhoto)
                {
                    startIndex = url.IndexOf("/photo/") + 7;
                    endIndex = url.IndexOf("/photo/") + 26;
                }
                else if (!matching)
                {
                    outputTextBox.AppendText($"Error: URL not found - {url}\r\n");
                    LogMessage(logFilePath, $"Error: URL not found - {url}");
                    return string.Empty;
                }

                if (endIndex > url.Length || startIndex < 0 || endIndex < startIndex)
                {
                    outputTextBox.AppendText($"Error: Invalid URL format - {url}\r\n");
                    LogMessage(logFilePath, $"Error: Invalid URL format - {url}");
                    return string.Empty;
                }

                var idVideo = url.Substring(startIndex, endIndex - startIndex);

                return idVideo;
            }
            catch (Exception ex)
            {
                outputTextBox.AppendText($"Error occurred while extracting MediaID: {ex.Message} - {url}\r\n");
                LogMessage(logFilePath, $"Error occurred while extracting MediaID: {ex.Message} - {url}");
                return string.Empty;
            }
        }


        private async Task DownloadMedia(VideoData data, string url, bool useOldFileStructure)
        {
            try
            {
                string username = ExtractUsernameFromUrl(url);

                string userFolderPath = Path.Combine(downloadFolderPath, username);
                if (!Directory.Exists(userFolderPath))
                {
                    Directory.CreateDirectory(userFolderPath);
                }

                string videosFolderPath = Path.Combine(userFolderPath, "Videos");
                if (!Directory.Exists(videosFolderPath))
                {
                    Directory.CreateDirectory(videosFolderPath);
                }

                string imagesFolderPath = Path.Combine(userFolderPath, "Images");
                if (!Directory.Exists(imagesFolderPath))
                {
                    Directory.CreateDirectory(imagesFolderPath);
                }

                string folderName = useOldFileStructure ? userFolderPath : videosFolderPath;

                string fileName;

                if (data.Images.Count > 0)
                {
                    folderName = useOldFileStructure ? Path.Combine(userFolderPath, "Images", $"{data.Id}_Images") : imagesFolderPath;

                    if (!Directory.Exists(folderName))
                    {
                        Directory.CreateDirectory(folderName);
                    }

                    foreach (var imageUrl in data.Images)
                    {
                        fileName = $"{data.Id}_{data.Images.IndexOf(imageUrl)}.jpeg";
                        string filePath = Path.Combine(folderName, fileName);

                        if (File.Exists(filePath))
                        {
                            outputTextBox.AppendText($"Image: '{fileName}' already exists. Skipping\r\n");
                            continue;
                        }

                        using (var client = new HttpClient())
                        {
                            using (var stream = await client.GetStreamAsync(imageUrl))
                            using (var fileStream = File.Create(filePath))
                            {
                                await stream.CopyToAsync(fileStream);
                            }
                        }

                        outputTextBox.AppendText($"Downloaded Image:'{fileName}' Successfully...\r\n");
                        LogDownload(fileName, imageUrl);
                    }
                }
                else
                {
                    folderName = useOldFileStructure ? Path.Combine(userFolderPath, "Videos", $"{data.Id}_Video") : videosFolderPath;

                    if (!Directory.Exists(folderName))
                    {
                        Directory.CreateDirectory(folderName);
                    }

                    fileName = $"{data.Id}";

                    if (withWatermarkCheckBox.Checked)
                    {
                        fileName += "_Watermark.mp4";
                    }
                    else
                    {
                        fileName += "_Save.mp4";
                    }

                    string filePath = Path.Combine(folderName, fileName);

                    if (File.Exists(filePath))
                    {
                        outputTextBox.AppendText($"Video: '{fileName}' already exists. Skipping\r\n");
                        return;
                    }

                    using (var client = new HttpClient())
                    {
                        var response = await client.GetAsync(data.Url);

                        if (!response.IsSuccessStatusCode)
                        {
                            if (response.StatusCode == HttpStatusCode.NotFound)
                            {
                                outputTextBox.AppendText($"Download failed because of an error in the Media file hosted on the Server. Link: {url}. Skipping...\r\n");
                                return;
                            }
                            else
                            {
                                throw new HttpRequestException($"HTTP error: {response.StatusCode}");
                            }
                        }

                        using (var stream = await response.Content.ReadAsStreamAsync())
                        using (var fileStream = File.Create(filePath))
                        {
                            await stream.CopyToAsync(fileStream);
                        }
                    }

                    outputTextBox.AppendText($"Downloaded Video: '{fileName}' Successfully...\r\n");
                    LogDownload(fileName, data.Url);
                }

            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                outputTextBox.AppendText($"Error: The video download failed with a 404 error: {url}\r\n");
                LogMessage(logFilePath, $"Error: The video download failed with a 404 error: {ex.Message}");
                await DownloadMedia(data, url, settingsDialog.UseOldFileStructure);
            }
            catch (HttpRequestException ex)
            {
                outputTextBox.AppendText($"Error: An error occurred while downloading Media: {ex.Message}\r\n");
                outputTextBox.AppendText("Retry continue download in 5 seconds...\r\n");
                LogMessage(logFilePath, $"Error: An error occurred while downloading Media: {ex.Message}");
                await Task.Delay(5000);
                await DownloadMedia(data, url, settingsDialog.UseOldFileStructure);
            }
            catch (TargetInvocationException ex)
            {
                outputTextBox.AppendText($"Error: TargetInvocationException occurred: {ex.InnerException?.Message}\r\n");
                outputTextBox.AppendText($"Inner Exception 1: {ex.InnerException?.InnerException?.Message}\r\n");
                outputTextBox.AppendText($"Inner Exception 2: {ex.InnerException?.InnerException?.InnerException?.Message}\r\n");
                LogMessage(logFilePath, $"Error: TargetInvocationException occurred: {ex.InnerException?.Message}");
            }
            catch (JsonException ex)
            {
                outputTextBox.AppendText($"Error: An error occurred while processing JSON response: {ex.Message}\r\n");
                outputTextBox.AppendText("Retry continue download in 5 seconds...\r\n");
                LogMessage(logFilePath, $"Error: An error occurred while processing JSON response: {ex.Message}");
                await Task.Delay(5000);
                await DownloadMedia(data, url, settingsDialog.UseOldFileStructure);
            }
            catch (Exception ex)
            {
                outputTextBox.AppendText($"Error: An unexpected error occurred: {ex.Message}\r\n");
                LogMessage(logFilePath, $"Error: An unexpected error occurred: {ex.Message}");
            }
        }



        private string ExtractUsernameFromUrl(string url)
        {
            try
            {
                var segments = url.Split('/');

                var usernameSegment = segments.FirstOrDefault(s => s.StartsWith("@"));

                if (usernameSegment == null)
                {
                    throw new ArgumentException("Invalid TikTok URL");
                }
                var username = usernameSegment.TrimStart('@');

                return username;
            }
            catch (Exception ex)
            {
                LogError($"Error extracting username from URL: {ex.Message}");
                return "default_username";
            }
        }

        private void ChangeDownloadFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select Download Folder";
                folderDialog.SelectedPath = downloadFolderPath;
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    downloadFolderPath = folderDialog.SelectedPath;
                    LogMessage(logFilePath, $"Changed download folder: {downloadFolderPath}");


                    settings.CurrentSettings.LastDownloadFolderPath = downloadFolderPath;
                    settings.SaveSettings();
                }
            }
        }


        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow(browserUtility);
            aboutWindow.ShowDialog();
        }

        private void browseFileButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePathTextBox.Text = openFileDialog.FileName;
                    LogMessage(logFilePath, $"Selected file path: {openFileDialog.FileName}");
                }
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var settingsDialog = new SettingsDialog(this))
            {
                settingsDialog.ShowDialog();
            }
        }

        public void SetUseOldFileStructure(bool value)
        {
            useOldFileStructure = value;
        }

        public void LogJsonCheckBox(bool value)
        {
            logJsonEnabled = value;
        }

        private void withWatermarkRadioButton_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void urlLabel_Click(object sender, EventArgs e)
        {

        }

        private void withWatermarkRadioButton_CheckedChanged_1(object sender, EventArgs e)
        {

        }

        private void outputTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void progressBar_Click(object sender, EventArgs e)
        {

        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void filePathTextBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}