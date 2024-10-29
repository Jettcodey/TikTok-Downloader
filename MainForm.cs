/*
##########################################
#           TikTok Downloader            #
#           Made by Jettcodey            #
#                © 2024                  #
#           DO NOT REMOVE THIS           #
##########################################
*/
using Microsoft.Playwright;
using Microsoft.Win32;
using System.Diagnostics;
using System.Globalization;
using System.Management;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Collections.Generic;
using System.Reflection;
using JsonSoft = Newtonsoft.Json;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Linq.Expressions;
using System.Security.Policy;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System;

namespace TikTok_Downloader
{
    public partial class MainForm : Form
    {
        private string logFolderPath;
        private string logFilePath;
        private string downloadFolderPath;
        private BrowserUtility browserUtility;
        private bool EnableDownloadLogs;
        private bool logJsonEnabled;
        private string jsonLogFilePath;
        private object jsonLock = new object();
        private bool DownloadVideosOnly;
        private bool DownloadImagesOnly;
        private readonly AppSettings settings;
        private SettingsDialog settingsDialog;
        private List<string> cachedVideoUrls = new List<string>();

        private bool isLoggingInitialized = false;
        public AppSettings AppSettings => settings;

        private Task LogSystemInformation(string logFilePath)
        {
            try
            {
                string systemInfo = "";

                systemInfo += $"\nRegion and Language: {CultureInfo.CurrentCulture.DisplayName}\n";

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
            settings = new AppSettings();
            LoadSettingsatbeginning();
            InitializeLoggingFolder();
            settings.LoadSettings();
            settingsDialog = new SettingsDialog(this);
            Directory.CreateDirectory(downloadFolderPath);

            Task.Run(async () =>
            {
                await LogSystemInformation(logFilePath);
            }).Wait();

            LogMessage(logFilePath, $"Initial download folder: {downloadFolderPath}");

            browserUtility = new BrowserUtility(this, settings);

            cmbChoice.SelectedItem = "Single Video/Image Download";

            outputTextBox.ReadOnly = true;
        }

        public class VersionInfo
        {
            public string Version { get; set; }
            public List<string> Files { get; set; }
        }

        private void InitializeLoggingFolder()
        {
            if (!isLoggingInitialized && (EnableDownloadLogs || logJsonEnabled))
            {
                string logFolderName = $"TTDownloader-Logs[{DateTime.Now:yyyy-MM-dd_HH-mm}]";
                logFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), logFolderName);

                try
                {
                    Directory.CreateDirectory(logFolderPath);
                    logFilePath = Path.Combine(logFolderPath, $"TikTokDownloaderLog_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt");
                    jsonLogFilePath = Path.Combine(logFolderPath, $"JSON_Log_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.json");

                    isLoggingInitialized = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error creating log directory: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(1);
                }
            }
        }

        private void LoadSettingsatbeginning()
        {
            settings.LoadSettings();

            var currentSettings = settings.CurrentSettings;

            if (currentSettings != null && !currentSettings.FirstRun)
            {
                DialogResult result = MessageBox.Show("It seems like this is the first time you're opening the application. Do you want to run the setup scripts now?", "First Time Setup", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        RunSetupScripts();

                        currentSettings.FirstRun = true;
                        settings.SaveSettings();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error running setup scripts: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("The feature 'Mass Download by Username' will not work if you want to use Firefox until the setup scripts are run. If you want to use Firefox as your browser, you will need to execute the scripts manually.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    currentSettings.FirstRun = true;
                    settings.SaveSettings();
                }
            }

            downloadFolderPath = currentSettings.LastDownloadFolderPath;
            if (string.IsNullOrEmpty(downloadFolderPath))
            {
                downloadFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TiktokDownloads");
            }

            EnableDownloadLogs = currentSettings.EnableDownloadLogs;
            logJsonEnabled = currentSettings.EnableJsonLogs;
        }


        private void LogMessage(string logFilePath, string message)
        {
            if (!EnableDownloadLogs)
            {
                return;
            }

            lock (logFilePath)
            {
                string redactedMessage = message.Replace(Environment.UserName, "[RedactedUsername]");
                File.AppendAllText(logFilePath, $"{DateTime.Now}: {redactedMessage}\n");
            }
        }



        private void LogDownload(string fileName, string url)
        {
            if (EnableDownloadLogs)
            {
                LogMessage(logFilePath, $"Downloaded file: {fileName}, from URL: {url}");
            }
        }

        private void LogJson(string fileName, string jsonContent)
        {
            if (logJsonEnabled)
            {
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
                        outputTextBox.AppendText($"Error: An error occurred while logging the JSON response: {ex.Message}\r\n");
                        Console.WriteLine($"Error occurred while logging the JSON response: {ex}");
                    }
                }
            }
        }

        private void LogError(string errorMessage)
        {
            if (EnableDownloadLogs)
            {
                LogMessage(logFilePath, $"ERROR: {errorMessage}");
            }
        }


        class VideoData
        {
            public string Url { get; set; } = string.Empty;
            public List<string> Images { get; set; } = new List<string>();
            public string Id { get; set; } = string.Empty;
            public List<string> AvatarUrls { get; set; }
            public List<string> GifAvatarUrls { get; set; }
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
            public Author author { get; set; } = new Author();
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
        class Author
        {
            public Avatar avatar_medium { get; set; } = new Avatar();
            public Avatar video_icon { get; set; } = new Avatar();
        }
        class Avatar
        {
            public List<string> url_list { get; set; } = new List<string>();
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
                string filePath = urlTextBox.Text.Trim();
                LogMessage(logFilePath, $"Selected file path: {filePath}");
                await DownloadFromTextFile(filePath);
            }
            else if (choice == "HD Download Single Video")
            {
                string HDUrl = urlTextBox.Text.Trim();
                await HDGetMediaID(HDUrl);
            }
            else if (choice == "HD Download´From Text File Links")
            {
                string filePath = urlTextBox.Text.Trim();
                LogMessage(logFilePath, $"Selected file path: {filePath}");
                await HDVideoDownloadFromTextFile(filePath);
            }
        }

        internal class BrowserUtility
        {
            private readonly MainForm mainForm;
            private readonly AppSettings appSettings;

            public BrowserUtility(MainForm mainForm, AppSettings appSettings)
            {
                this.mainForm = mainForm;
                this.appSettings = appSettings;
            }

            public async Task<string> GetSystemDefaultBrowser()
            {
                string executablePath = string.Empty;
                RegistryKey regKey = null;

                try
                {
                    var regDefault = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\Shell\\Associations\\UrlAssociations\\http\\UserChoice", false);
                    var stringDefault = regDefault.GetValue("ProgId");

                    regKey = Registry.ClassesRoot.OpenSubKey(stringDefault + "\\shell\\open\\command", false);
                    executablePath = regKey.GetValue(null).ToString().ToLower().Replace("\"", "");

                    if (!executablePath.EndsWith("exe"))
                        executablePath = executablePath.Substring(0, executablePath.LastIndexOf(".exe") + 4);
                    
                    if (executablePath.Contains("firefox"))
                    {
                        var playwright = await Playwright.CreateAsync();
                        executablePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"ms-playwright\firefox-1447\firefox\firefox.exe");
                    }
                }
                catch (Exception ex)
                {
                    executablePath = $"ERROR: An exception of type: {ex.GetType()} occurred in method: {ex.TargetSite} in the following module: {nameof(BrowserUtility)}";
                    mainForm.LogError(executablePath);
                }
                finally
                {
                    regKey?.Close();
                }

                return executablePath;
            }

            public async Task<string> GetBrowserExecutablePath()
            {
                if (!string.IsNullOrWhiteSpace(appSettings.CurrentSettings.CustomBrowserPath))
                {
                    if (File.Exists(appSettings.CurrentSettings.CustomBrowserPath))
                    {
                        return appSettings.CurrentSettings.CustomBrowserPath;
                    }
                    else
                    {
                        mainForm.LogError($"Custom browser path does not exist: {appSettings.CurrentSettings.CustomBrowserPath}");
                    }

                }
                return await GetSystemDefaultBrowser();
            }

            public IBrowserType GetBrowserTypeForExecutable(string executablePath, IPlaywright playwright)
            {
                if (executablePath.ToLower().Contains("firefox"))
                {
                    return playwright.Firefox;
                }
                else if (executablePath.ToLower().Contains("webkit"))
                {
                    return playwright.Webkit;
                }
                else
                {
                    return playwright.Chromium;
                }
            }
        }

        private async Task MassDownloadByUsername()
        {
            string input = urlTextBox.Text.Trim();
            string username = null;
            string baseUrl = null;
            if (string.IsNullOrWhiteSpace(input))
            {
                MessageBox.Show("Please enter a valid URL", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (input.StartsWith("https://www.tiktok.com/@"))
            {
                Uri uri = new Uri(input);
                username = uri.Segments.Last().Trim('/');
                baseUrl = input;
            }
            else
            {
                username = input;
                baseUrl = $"https://www.tiktok.com/@{username}";
            }
            LogMessage(logFilePath, $"Username selected for mass download: {username}");

            // Get system default browser executable path
            string browserExecutablePath = await browserUtility.GetBrowserExecutablePath();
            LogMessage(logFilePath, $"Browser executable path: {browserExecutablePath}");

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

                // Filter links by username using the first pattern
                var filteredUrls = combinedUrls.Where(url => url.Contains($"/@{username}/")).ToList();
                LogMessage(logFilePath, $"Filtered {filteredUrls.Count()} URLs by username (@{username})");

                // If no URLs were found, fallback to the second pattern
                if (!filteredUrls.Any())
                {
                    filteredUrls = combinedUrls.Where(url => url.Contains($"/{username}/")).ToList();
                    LogMessage(logFilePath, $"Filtered {filteredUrls.Count()} URLs by username ({username})");
                }
                else
                {
                    LogMessage(logFilePath, $"Filtered {filteredUrls.Count()} URLs by username (@{username})");
                }

                // Save all video and image post links to a single text file
                string combinedLinksFilePath = Path.Combine(downloadFolderPath, $"{username}_combined_links.txt");
                await File.WriteAllLinesAsync(combinedLinksFilePath, filteredUrls);
                LogMessage(logFilePath, $"Saved {filteredUrls.Count()} URLs to file: {combinedLinksFilePath}");

                // Close the page and context
                await page.CloseAsync();
                await context.CloseAsync();
                LogMessage(logFilePath, "Browser Page and context closed successfully");

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

                        if (withWatermarkCheckBox.Checked)
                        {
                            var dataWithWatermark = await GetMedia(trimmedUrl, true, false);

                            if (dataWithWatermark == null)
                            {
                                LogError($"Error: Media with watermark from URL {trimmedUrl} wasn´t found!");
                            }
                            else
                            {
                                await DownloadMedia(dataWithWatermark, trimmedUrl, true, false);
                            }
                        }

                        else if (noWatermarkCheckBox.Checked)
                        {
                            var dataNoWatermark = await GetMedia(trimmedUrl, false, true);

                            if (dataNoWatermark == null)
                            {
                                LogError($"Error: Media without watermark from URL {trimmedUrl} wasn´t found!");
                            }
                            else
                            {
                                await DownloadMedia(dataNoWatermark, trimmedUrl, false, true);
                            }
                        }
                        else
                        {
                            var dataNoWatermark = await GetMedia(trimmedUrl, false, true);

                            if (dataNoWatermark == null)
                            {
                                LogError($"Error: Media without watermark from URL {trimmedUrl} not found!");
                            }
                            else
                            {
                                await DownloadMedia(dataNoWatermark, trimmedUrl, false, true);
                            }
                        }
                    }

                    progressBar.Value++;
                }
            }
        }

        private async Task DownloadFromTextFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show($"Text file not found or None was Selected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogError("Text file not found or None was Selected!");
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
                        LogMessage(logFilePath, $"Downloading {trimmedUrl} ...");

                        if (withWatermarkCheckBox.Checked)
                        {
                            var dataWithWatermark = await GetMedia(trimmedUrl, true, false);

                            if (dataWithWatermark == null)
                            {
                                LogError($"Error: Media with watermark from URL {trimmedUrl} not found!");
                            }
                            else
                            {
                                await DownloadMedia(dataWithWatermark, trimmedUrl, true, false);
                            }
                        }

                        else if (noWatermarkCheckBox.Checked)
                        {
                            var dataNoWatermark = await GetMedia(trimmedUrl, false, true);

                            if (dataNoWatermark == null)
                            {
                                LogError($"Error: Media without watermark from URL {trimmedUrl} not found!");
                            }
                            else
                            {
                                await DownloadMedia(dataNoWatermark, trimmedUrl, false, true);
                            }
                        }
                        else
                        {
                            var dataNoWatermark = await GetMedia(trimmedUrl, false, true);

                            if (dataNoWatermark == null)
                            {
                                LogError($"Error: Media without watermark from URL {trimmedUrl} not found!");
                            }
                            else
                            {
                                await DownloadMedia(dataNoWatermark, trimmedUrl, false, true);
                            }
                        }
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

                using (var settingsDialog = new SettingsDialog(this))
                {
                    if (withWatermarkCheckBox.Checked)
                    {
                        var dataWithWatermark = await GetMedia(trimmedUrl, true, false);

                        if (dataWithWatermark == null)
                        {
                            LogError($"Error: Media with watermark from URL {trimmedUrl} not found!");
                        }
                        else
                        {
                            await DownloadMedia(dataWithWatermark, trimmedUrl, true, false);
                        }
                    }

                    else if (noWatermarkCheckBox.Checked)
                    {
                        var dataNoWatermark = await GetMedia(trimmedUrl, false, true);

                        if (dataNoWatermark == null)
                        {
                            LogError($"Error: Media without watermark from URL {trimmedUrl} not found!");
                        }
                        else
                        {
                            await DownloadMedia(dataNoWatermark, trimmedUrl, false, true);
                        }
                    }
                    else
                    {
                        var dataNoWatermark = await GetMedia(trimmedUrl, false, true);

                        if (dataNoWatermark == null)
                        {
                            LogError($"Error: Media without watermark from URL {trimmedUrl} not found!");
                        }
                        else
                        {
                            await DownloadMedia(dataNoWatermark, trimmedUrl, false, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError($"An error occurred: {ex.Message}");
            }
        }

        private async Task HDGetMediaID(string HDUrl)
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

                var tiktokUrl = await GetMediaUrl(trimmedUrl);

                if (string.IsNullOrEmpty(tiktokUrl))
                {
                    LogError($"Error: MediaID not found for URL {trimmedUrl}");
                    return;
                }
                else
                {
                    await HDVideoDownload(tiktokUrl);
                }
            }
            catch (Exception ex)
            {
                LogError($"An error occurred: {ex.Message}");
            }
        }

        private async Task HDVideoDownloadFromTextFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show("Error: The provided file path does not exist.", "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var urls = await File.ReadAllLinesAsync(filePath);
            LogMessage(logFilePath, $"Read {urls.Length} URLs from file: {filePath}");
            progressBar.Minimum = 0;
            progressBar.Maximum = urls.Length;
            progressBar.Value = 0;
            using (var settingsDialog = new SettingsDialog(this))
            {
                foreach (var url in urls)
                {
                    string HDUrl = url.Trim();
                    if (!string.IsNullOrEmpty(HDUrl))
                    {
                        LogMessage(logFilePath, $"Downloading HD Video {HDUrl} ...");
                        await HDGetMediaID(HDUrl);
                        progressBar.Value++;
                        await Task.Delay(1000);
                    }
                }
            }
        }

        public async Task<string> GetMediaUrl(string url)
        {
            try
            {
                if (url.Contains("/photo/"))
                {
                    outputTextBox.AppendText($"Blocked URL: Image/Photo URLs are not allowed: {url}\r\n");
                    return string.Empty;
                }

                if (url.Contains("/video/"))
                {
                    string redirectedUrl = await GetRedirectUrl(url);

                    if (redirectedUrl.Contains("/photo/"))
                    {
                        outputTextBox.AppendText($"Blocked URL: Image/Photo URLs are not allowed after redirection: {redirectedUrl}\r\n");
                        return string.Empty;
                    }

                    return redirectedUrl;
                }

                outputTextBox.AppendText($"Invalid URL: Not a Video URL! URL provided: {url}\r\n");
                return string.Empty;
            }
            catch (Exception ex)
            {
                outputTextBox.AppendText($"Error occurred while extracting MediaID: {ex.Message} - {url}\r\n");
                LogMessage(logFilePath, $"Error occurred while extracting MediaID: {ex.Message} - {url}");
                return string.Empty;
            }
        }

        public async Task<string> GetMediaID(string url)
        {
            try
            {
                if (url.Contains("/t/"))
                {
                    using (var client = new HttpClient())
                    {
                        var response = await client.GetAsync(url);
                        url = response.RequestMessage?.RequestUri.Segments.LastOrDefault()?.TrimEnd('/') ?? string.Empty;
                    }
                }
                else
                {
                    url = await GetRedirectUrl(url);
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

                var MediaID = url.Substring(startIndex, endIndex - startIndex);

                return MediaID;
            }
            catch (Exception ex)
            {
                outputTextBox.AppendText($"Error occurred while extracting MediaID: {ex.Message} - {url}\r\n");
                LogMessage(logFilePath, $"Error occurred while extracting MediaID: {ex.Message} - {url}");
                return string.Empty;
            }
        }

        private async Task<string> GetRedirectUrl(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                    return response.RequestMessage?.RequestUri?.AbsoluteUri ?? url;
                }
            }
            catch (Exception ex)
            {
                LogMessage(logFilePath, $"Error in Getting the RedirectURL: {ex.Message}");
                throw;
            }
        }

        private async Task<VideoData?> GetMedia(string url, bool withWatermark, bool noWatermark)
        {
            var MediaID = await GetMediaID(url);
            var username = await ExtractUsernameFromUrl(url);
            string userFolderPath = Path.Combine(downloadFolderPath, username);
            string indexFilePath = Path.Combine(userFolderPath, $"{username}_index.txt");

            if (File.Exists(indexFilePath))
            {
                var downloadedIds = File.ReadLines(indexFilePath).ToList();

                if (downloadAvatarsCheckBox.Checked)
                {
                    // Check for Avatar_{indexNumber} (Normal Avatars)
                    var avatarIndexNumberPattern = $"{username}_\\d+";
                    bool normalAvatarsExist = downloadedIds.Any(id => Regex.IsMatch(id, avatarIndexNumberPattern));
                    if (normalAvatarsExist)
                    {
                        outputTextBox.AppendText($"Avatars for Media {username} already downloaded. Skipping...\r\n");
                        return null;
                    }

                    // Check for GifAvatar_{indexNumber} (GIF Avatars)
                    var gifAvatarIndexNumberPattern = $"{username}_GIF_\\d+";
                    if (downloadedIds.Any(id => Regex.IsMatch(id, gifAvatarIndexNumberPattern)))
                    {
                        outputTextBox.AppendText($"GIF Avatars for Media {username} already downloaded. Skipping...\r\n");
                        return null;
                    }
                }

                if (!downloadAvatarsCheckBox.Checked)
                {
                    // Check for MediaID (Videos)
                    if (downloadedIds.Contains(MediaID))
                    {
                        outputTextBox.AppendText($"Media {MediaID} already downloaded. Skipping...\r\n");
                        return null;
                    }

                    // Check for MediaID_{indexNumber} (Images)
                    var indexNumberPattern = $"{MediaID}_\\d+";
                    if (downloadedIds.Any(id => Regex.IsMatch(id, indexNumberPattern)))
                    {
                        outputTextBox.AppendText($"Media {MediaID} already downloaded. Skipping...\r\n");
                        return null;
                    }
                }
            }

            var apiUrl = $"https://api22-normal-c-alisg.tiktokv.com/aweme/v1/feed/?aweme_id={MediaID}&iid=7238789370386695942&device_id=7238787983025079814&resolution=1080*2400&channel=googleplay&app_name=musical_ly&version_code=350103&device_platform=android&device_type=Pixel+7&os_version=13";

            using (var client = new HttpClient())
            {
                try
                {
                    var finalUrl = await GetRedirectUrl(url);
                    var request = new HttpRequestMessage(HttpMethod.Options, apiUrl);
                    var response = await client.SendAsync(request);

                    if (response.StatusCode == HttpStatusCode.TooManyRequests)
                    {
                        LogMessage(logFilePath, "Received a Http 429 error (TooManyRequests), retrying after 5 Second delay...");
                        outputTextBox.AppendText($"Small Cooldown, Continue after 5 Seconds.\r\n");
                        await Task.Delay(5000);
                        return await GetMedia(url, withWatermark, noWatermark);
                    }

                    response.EnsureSuccessStatusCode();

                    var json = await response.Content.ReadAsStringAsync();
                    LogJson($"API_Response_For_'{MediaID}'", json);

                    if (string.IsNullOrWhiteSpace(json))
                    {
                        outputTextBox.AppendText($"Error: Received empty JSON response for MediaID: {MediaID}\r\n");
                        return null;
                    }

                    var data = JsonSoft.JsonConvert.DeserializeObject<ApiData>(json);
                    if (data?.aweme_list == null || data.aweme_list.Count == 0)
                    {
                        outputTextBox.AppendText($"Error: No aweme_list found in JSON response for MediaID: {MediaID}\r\n");
                        return null;
                    }

                    var video = data.aweme_list.FirstOrDefault();
                    var urlMedia = noWatermark ? video?.video?.play_addr?.url_list.FirstOrDefault()
                                               : (withWatermark ? video?.video?.download_addr?.url_list.FirstOrDefault()
                                                                : video?.video?.play_addr?.url_list.FirstOrDefault());
                    var imageUrls = video?.image_post_info?.images?.Select(img => img.display_image.url_list.FirstOrDefault()).ToList();
                    var avatarUrls = video?.author?.avatar_medium?.url_list ?? new List<string>();
                    var gifAvatarUrls = video?.author?.video_icon?.url_list ?? new List<string>();

                    if (urlMedia == null)
                    {
                        if (noWatermark)
                        {
                            LogMessage(logFilePath, $"Skipping download link for MediaID: {MediaID} due to missing No Watermark URL.");
                            outputTextBox.AppendText($"Error: No Watermark free URL found for MediaID: {MediaID}\r\n");
                        }
                        else if (withWatermark)
                        {
                            LogMessage(logFilePath, $"Skipping download link for MediaID: {MediaID} due to missing Watermark URL.");
                            outputTextBox.AppendText($"Error: No Watermark URL found for MediaID: {MediaID}\r\n");
                        }
                        else
                        {
                            LogMessage(logFilePath, $"Skipping download link for MediaID: {MediaID} due to missing media URL.");
                            outputTextBox.AppendText($"Error: No media URL found for MediaID: {MediaID}\r\n");
                        }

                        return null;
                    }

                    return new VideoData
                    {
                        Url = urlMedia,
                        Images = imageUrls ?? new List<string>(),
                        Id = MediaID,
                        AvatarUrls = avatarUrls,
                        GifAvatarUrls = gifAvatarUrls
                    };
                }
                catch (HttpRequestException ex)
                {
                    LogError($"HTTP error while getting video: {ex.Message}");
                    return null;
                }
                catch (JsonSoft.JsonException ex)
                {
                    LogError($"JSON error while getting video: {ex.Message}");
                    return null;
                }
                catch (Exception ex)
                {
                    LogError($"Unexpected error while getting video: {ex.Message}");
                    return null;
                }
            }
        }

        private async Task HDVideoDownload(string tiktokUrl) // Third-party API endpoint!
        {
            string apiEndpoint = "https://www.tikwm.com/api/";
            using (HttpClient client = new HttpClient())
            {
                var urlWithParams = $"{apiEndpoint}?url={tiktokUrl}&hd=1"; // Set hd=1 for HD download
                HttpResponseMessage response = await client.GetAsync(urlWithParams);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    dynamic responseData = JsonSoft.JsonConvert.DeserializeObject(responseBody);
                    if (responseData.code == 0)
                    {
                        // Extract username and video ID from the TikTok URL
                        var videoURL = await GetMediaUrl(tiktokUrl);
                        string username = Regex.Match(tiktokUrl, @"@(\w+)").Groups[1].Value;
                        string videoId = Regex.Match(tiktokUrl, @"/video/(\d+)").Groups[1].Value;
                        string userFolderPath = Path.Combine(downloadFolderPath, username);
                        string indexFilePath = Path.Combine(userFolderPath, $"{username}_index.txt");

                        if (!Directory.Exists(userFolderPath))
                        {
                            Directory.CreateDirectory(userFolderPath);
                        }

                        string videosFolderPath = Path.Combine(userFolderPath, "Videos");
                        if (!Directory.Exists(videosFolderPath))
                        {
                            Directory.CreateDirectory(videosFolderPath);
                        }

                        string filename = $"{videoId}_HD.mp4";
                        string fullPath = Path.Combine(videosFolderPath, filename);
                        LogMessage(logFilePath, $"HD Video File Saved to {fullPath}.");

                        bool videoAlreadyDownloaded = false;
                        if (File.Exists(indexFilePath))
                        {
                            var downloadedIds = await File.ReadAllLinesAsync(indexFilePath);
                            var HDVideoPattern = $"{videoId}_HD";
                            if (downloadedIds.Any(id => Regex.IsMatch(id, HDVideoPattern)))
                            {
                                outputTextBox.AppendText($"Media {videoId} already downloaded. Skipping...\r\n");
                                videoAlreadyDownloaded = true;
                            }
                        }

                        if (!videoAlreadyDownloaded)
                        {
                            string videoUrl = responseData.data.hdplay; // Get HD video URL
                            byte[] videoData = await client.GetByteArrayAsync(videoUrl);
                            await File.WriteAllBytesAsync(fullPath, videoData);
                            outputTextBox.AppendText($"Downloading HD Video from User: {username}\r\nDownloaded HD Video: '{filename}' Successfully...\r\n");

                            await File.AppendAllTextAsync(indexFilePath, $"{videoId}_HD\n");
                        }
                        
                    }
                    else
                    {
                        outputTextBox.AppendText($"Error: {responseData.message}\r\n");
                    }
                }
                else
                {
                    outputTextBox.AppendText("Error: Unable to download video in HD\r\n");
                }
            }
        }

        private async Task DownloadMedia(VideoData data, string url, bool withWatermark, bool noWatermark)
        {
            const int maxRetries = 5;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    string username = await ExtractUsernameFromUrl(url);
                    string userFolderPath = Path.Combine(downloadFolderPath, username);
                    string indexFilePath = Path.Combine(userFolderPath, $"{username}_index.txt");

                    Directory.CreateDirectory(userFolderPath);

                    string videosFolderPath = Path.Combine(userFolderPath, "Videos");
                    Directory.CreateDirectory(videosFolderPath);

                    string imagesFolderPath = Path.Combine(userFolderPath, "Images");
                    Directory.CreateDirectory(imagesFolderPath);

                    if (data.Images.Count > 0)
                    {
                        foreach (var imageUrl in data.Images)
                        {
                            string imageFileName = $"{data.Id}_{data.Images.IndexOf(imageUrl)}.jpeg";
                            string imageFilePath = Path.Combine(imagesFolderPath, imageFileName);

                            if (File.Exists(imageFilePath))
                            {
                                outputTextBox.AppendText($"Image: '{imageFileName}' already exists. Skipping...\r\n");
                                continue;
                            }

                            using (var client = new HttpClient())
                            {
                                using (var stream = await client.GetStreamAsync(imageUrl))
                                using (var fileStream = File.Create(imageFilePath))
                                {
                                    await stream.CopyToAsync(fileStream);
                                }
                            }

                            outputTextBox.AppendText($"Downloaded Image: '{imageFileName}' successfully.\r\n");
                            await File.AppendAllTextAsync(indexFilePath, $"{data.Id}_{data.Images.IndexOf(imageUrl)}\n");
                        }
                    }
                    // Download the video only if no images were found/downloaded (I know this is Stupid, but it works)
                    else
                    {
                        if (!string.IsNullOrEmpty(data.Url))
                        {
                            string videoFileName = $"{data.Id}{(noWatermark ? "_Save" : (withWatermark ? "_Watermark" : string.Empty))}.mp4";
                            string videoFilePath = Path.Combine(videosFolderPath, videoFileName);

                            if (File.Exists(videoFilePath))
                            {
                                outputTextBox.AppendText($"Video: '{videoFileName}' already exists. Skipping...\r\n");
                            }
                            else
                            {
                                using (var client = new HttpClient())
                                {
                                    var response = await client.GetAsync(data.Url);
                                    response.EnsureSuccessStatusCode();

                                    using (var stream = await response.Content.ReadAsStreamAsync())
                                    using (var fileStream = File.Create(videoFilePath))
                                    {
                                        await stream.CopyToAsync(fileStream);
                                    }
                                }

                                outputTextBox.AppendText($"Downloaded Video: '{videoFileName}' successfully.\r\n");
                                await File.AppendAllTextAsync(indexFilePath, $"{data.Id}\n");
                            }
                        }
                    }

                    if (downloadAvatarsCheckBox.Checked)
                    {
                        await DownloadAvatars(data, url, username);
                        LogMessage(logFilePath, "Download Avatars Checkbox is Active.");
                    }

                    return;
                }
                catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    outputTextBox.AppendText($"Error: The media download failed with a 429 error: {url}\r\n");
                    outputTextBox.AppendText("Retrying in 5 seconds...\r\n");
                    LogMessage(logFilePath, $"Error: The media download failed with a 429 error: {ex.Message}");
                    await Task.Delay(5000);
                }
                catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    outputTextBox.AppendText($"Error: The media download failed with a 404 error: {url}\r\n");
                    LogMessage(logFilePath, $"Error: The media download failed with a 404 error: {ex.Message}");
                    break;
                }
                catch (HttpRequestException ex)
                {
                    outputTextBox.AppendText($"Error: An error occurred while downloading Media: {ex.Message}\r\n");
                    outputTextBox.AppendText("Retrying in 5 seconds...\r\n");
                    LogMessage(logFilePath, $"Error: An error occurred while downloading Media: {ex.Message}");
                    await Task.Delay(5000);
                }
                catch (TargetInvocationException ex)
                {
                    outputTextBox.AppendText($"Error: TargetInvocationException occurred: {ex.InnerException?.Message}\r\n");
                    outputTextBox.AppendText($"Inner Exception 1: {ex.InnerException?.InnerException?.Message}\r\n");
                    outputTextBox.AppendText($"Inner Exception 2: {ex.InnerException?.InnerException?.InnerException?.Message}\r\n");
                    LogMessage(logFilePath, $"Error: TargetInvocationException occurred: {ex.InnerException?.Message}");
                }
                catch (JsonSoft.JsonException ex)
                {
                    outputTextBox.AppendText($"Error: An error occurred while processing JSON response: {ex.Message}\r\n");
                    outputTextBox.AppendText("Retrying in 5 seconds...\n");
                    LogMessage(logFilePath, $"Error: An error occurred while processing JSON response: {ex.Message}");
                    await Task.Delay(5000);
                }
                catch (Exception ex)
                {
                    outputTextBox.AppendText($"Error: An unexpected error occurred: {ex.Message}\r\n");
                    LogMessage(logFilePath, $"Error: An unexpected error occurred: {ex.Message}");
                }

                if (attempt < maxRetries)
                {
                    outputTextBox.AppendText($"Retrying download (Attempt {attempt} of {maxRetries}) in 5 seconds...\r\n");
                    await Task.Delay(5000);
                }
                else
                {
                    outputTextBox.AppendText($"Error: Failed to download media from {url} after {maxRetries} attempts.\r\n");
                }
            }
        }

        private async Task DownloadAvatars(VideoData data, string url, string username)
        {
            try
            {
                string userFolderPath = Path.Combine(downloadFolderPath, username);
                string avatarsFolderPath = Path.Combine(userFolderPath, "Avatars");
                string indexFilePath = Path.Combine(userFolderPath, $"{username}_index.txt");

                if (Directory.Exists(avatarsFolderPath) && Directory.GetFiles(avatarsFolderPath).Length > 0)
                {
                    LogError($"Error: Avatars folder already exists for user: {username}");
                    return;
                }

                if (!Directory.Exists(avatarsFolderPath))
                {
                    Directory.CreateDirectory(avatarsFolderPath);
                }

                if (data.AvatarUrls != null && data.AvatarUrls.Count > 0)
                {
                    foreach (var avatarUrl in data.AvatarUrls)
                    {
                        string avatarFileName = $"{username}_{data.AvatarUrls.IndexOf(avatarUrl)}.jpeg";
                        string avatarFilePath = Path.Combine(avatarsFolderPath, avatarFileName);

                        if (File.Exists(avatarFilePath))
                        {
                            outputTextBox.AppendText($"Avatar: '{avatarFileName}' already exists. Skipping\r\n");
                            continue;
                        }

                        using (var client = new HttpClient())
                        {
                            using (var stream = await client.GetStreamAsync(avatarUrl))
                            using (var fileStream = File.Create(avatarFilePath))
                            {
                                await stream.CopyToAsync(fileStream);
                            }
                        }

                        outputTextBox.AppendText($"Downloaded Avatar From '{avatarFileName}' Successfully...\r\n");
                        await File.AppendAllTextAsync(indexFilePath, $"{username}_{data.AvatarUrls.IndexOf(avatarUrl)}\r\n");
                        LogDownload(avatarFileName, avatarUrl);
                    }
                }
                else
                {
                    outputTextBox.AppendText("No avatar images to download.\r\n");
                }

                if (data.GifAvatarUrls != null && data.GifAvatarUrls.Count > 0)
                {
                    foreach (var gifAvatarUrl in data.GifAvatarUrls)
                    {
                        string gifAvatarfileName = $"{username}_GIF_{data.GifAvatarUrls.IndexOf(gifAvatarUrl)}.gif";
                        string gifAvatarfilePath = Path.Combine(avatarsFolderPath, gifAvatarfileName);

                        if (File.Exists(gifAvatarfilePath))
                        {
                            LogError($"Error: Avatars folder already exists for user: {username}");
                            continue;
                        }

                        using (var client = new HttpClient())
                        {
                            using (var stream = await client.GetStreamAsync(gifAvatarUrl))
                            using (var fileStream = File.Create(gifAvatarfilePath))
                            {
                                await stream.CopyToAsync(fileStream);
                            }
                        }

                        outputTextBox.AppendText($"Downloaded GIF Avatar from '{gifAvatarfileName}' Successfully...\r\n");
                        await File.AppendAllTextAsync(indexFilePath, $"{username}_GIF_{data.GifAvatarUrls.IndexOf(gifAvatarUrl)}\r\n");
                        LogDownload(gifAvatarfileName, gifAvatarUrl);
                    }
                }
                else
                {
                    outputTextBox.AppendText("No GIF avatars to download.\r\n");
                    LogDownload("No GIF avatars to download", url);
                }
            }
            catch (Exception ex)
            {
                outputTextBox.AppendText($"Error: An error occurred while downloading avatars: {ex.Message}\r\n");
                LogMessage(logFilePath, $"Error: An error occurred while downloading avatars: {ex.Message}");
            }
        }

        private async Task<string> ExtractUsernameFromUrl(string url)
        {
            try
            {
                var finalUrl = await GetRedirectUrl(url);
                var segments = finalUrl.Split('/');

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
                return "Username not Found!";
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

        private void RunSetupScripts()
        {
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string scriptsDirectory = Path.Combine(appDirectory, "scripts");

            Process process1 = Process.Start(new ProcessStartInfo
            {
                FileName = Path.Combine(scriptsDirectory, "pwsh.bat"),
                WorkingDirectory = scriptsDirectory
            });

            process1.WaitForExit();

            if (process1.ExitCode == 0)
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = Path.Combine(scriptsDirectory, "playwright-ex.bat"),
                    WorkingDirectory = scriptsDirectory
                });

                MessageBox.Show("Setup scripts executed successfully.", "Setup Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Script1 failed to execute. Setup scripts cannot proceed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CheckForUpdates()
        {
            string url = "https://api.jettcodey.de/ttd/update/update.json";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var response = client.GetStringAsync(url).Result;
                    var latestVersionInfo = JsonSoft.JsonConvert.DeserializeObject<VersionInfo>(response);

                    // Compare versions
                    Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
                    Version latestVersion = new Version(latestVersionInfo.Version);

                    if (latestVersion > currentVersion)
                    {
                        string tempFolder = Path.Combine(Path.GetTempPath(), "update");
                        Directory.CreateDirectory(tempFolder);
                        DownloadFiles(latestVersionInfo.Files, tempFolder);

                        DialogResult result = MessageBox.Show("New updates are available. Would you like to restart the application to apply updates?", "Update Available", MessageBoxButtons.YesNo);
                        if (result == DialogResult.Yes)
                        {
                            CreateBatchFile(tempFolder); // Generate Update.bat
                            Application.Exit();
                        }
                    }
                    else
                    {
                        MessageBox.Show("You are already using the latest version.");
                    }
                }
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
            {
                MessageBox.Show("You´re sending Too Many Requests to The Update Server!", "Too Many Requests!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                MessageBox.Show("Update Server Not Found!, Please Check the Github Repository for further Information", "Update Server Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"An error occurred while checking for updates: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateBatchFile(string tempFolder)
        {
            string appFolder = AppDomain.CurrentDomain.BaseDirectory;
            string batchFilePath = Path.Combine(Path.GetTempPath(), "update.bat");

            using (StreamWriter writer = new StreamWriter(batchFilePath))
            {
                writer.WriteLine("@echo off");
                writer.WriteLine("timeout /t 2");

                foreach (var file in Directory.GetFiles(tempFolder))
                {
                    string fileName = Path.GetFileName(file);
                    string destinationPath = Path.Combine(appFolder, fileName);
                    writer.WriteLine($"copy /y \"{file}\" \"{destinationPath}\"");
                }

                writer.WriteLine($"rmdir /s /q \"{tempFolder}\"");
            }

            // Start the Update.bat file
            Process.Start(new ProcessStartInfo
            {
                FileName = batchFilePath,
                UseShellExecute = true,
                Verb = "runas" // Admin rights required to replace files
            });
        }

        private void DownloadFiles(List<string> fileUrls, string tempFolder)
        {
            using (HttpClient client = new HttpClient())
            {
                foreach (var fileUrl in fileUrls)
                {
                    string fileName = Path.GetFileName(fileUrl);
                    string tempFilePath = Path.Combine(tempFolder, fileName);
                    var fileBytes = client.GetByteArrayAsync(fileUrl).Result;
                    File.WriteAllBytes(tempFilePath, fileBytes);
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
                    urlTextBox.Text = openFileDialog.FileName;
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

        private void checkForUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckForUpdates();
        }

        public void LogJsonCheckBox(bool value)
        {
            logJsonEnabled = value;
            InitializeLoggingFolder();
        }

        public void EnableDownloadLogsCheckBox(bool value)
        {
            EnableDownloadLogs = value;
            InitializeLoggingFolder();
        }

        public void DownloadVideosOnlyCheckBox(bool value)
        {
            DownloadVideosOnly = value;
        }

        public void DownloadImagesOnlyCheckBox(bool value)
        {
            DownloadImagesOnly = value;
        }

        private async void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = "https://github.com/Jettcodey/TikTok-Downloader/issues";

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

        private void withWatermarkCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (withWatermarkCheckBox.Checked)
            {
                noWatermarkCheckBox.Visible = true;
            }
            else
            {
                noWatermarkCheckBox.Visible = false;
                noWatermarkCheckBox.Checked = false;
            }
        }

        private void downloadAvatarsCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}