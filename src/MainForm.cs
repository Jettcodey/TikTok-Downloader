﻿/*
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
using System;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace TikTok_Downloader
{
    public partial class MainForm : Form
    {
        private string logFolderPath;
        private string logFilePath;
        private string downloadFolderPath;
        private string lastBrowsingPath;
        private readonly BrowserUtility browserUtility;
        private bool EnableDownloadLogs;
        private bool logJsonEnabled;
        private string jsonLogFilePath;
        private readonly object jsonLock = new object();
        private readonly bool ToastsAllowed;
        public bool firefoxfound;
        private readonly AppSettings settings;
        private readonly SettingsDialog settingsDialog;
        private List<string> cachedVideoUrls = new List<string>();
        private bool _stopLoggingForCooldown;
        private bool isLoggingInitialized = false;
        private bool _isPaused = false;
        private CancellationTokenSource _cancellationTokenSource;
        private TaskCompletionSource<bool> _pauseTaskCompletionSource;
        private readonly StringBuilder _pausedTextBuffer = new StringBuilder();
        public AppSettings AppSettings => settings;
        private readonly CacheManager cacheManager;

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
            cacheManager = new CacheManager();
            settings = new AppSettings();
            LoadSettingsatbeginning();
            Directory.CreateDirectory(downloadFolderPath);
            InitializeLoggingFolder();
            settings.LoadSettings();
            settingsDialog = new SettingsDialog(this);

            Task.Run(async () =>
            {
                await LogSystemInformation(logFilePath);
            }).Wait();

            LogMessage(logFilePath, $"Initial download folder: {downloadFolderPath}");

            browserUtility = new BrowserUtility(this, settings);

            outputTextBox.ReadOnly = true;
        }

        public class VersionInfo
        {
            public string Version { get; set; }
            public string Message { get; set; }
            public List<string> Files { get; set; }
        }

        public void InitializeLoggingFolder()
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
                DialogResult result = MessageBox.Show("It seems this is your first time opening the application. If you want to use Firefox for mass downloading, you need to run the Firefox script first! Make sure Powershell 7 is Already Installed!", "Firefox Install Script", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        RunFirefoxScript();

                        currentSettings.FirstRun = true;
                        settings.SaveSettings();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error running Firefox Install Script: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("The feature '(HD)Mass Download by Username' will not work with Firefox until the script has been run. If you want to use Firefox as your browser, you must execute the script manually.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    currentSettings.FirstRun = true;
                    settings.SaveSettings();
                }
            }

            downloadFolderPath = currentSettings.LastDownloadFolderPath;
            if (string.IsNullOrEmpty(downloadFolderPath))
            {
                downloadFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TiktokDownloads");
            }

            lastBrowsingPath = currentSettings.LastBrowsingPath;
            if (string.IsNullOrEmpty(lastBrowsingPath))
            {
                lastBrowsingPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }

            if (cmbChoice.Items.Contains(currentSettings.LastDownloadOption))
            {
                cmbChoice.SelectedItem = currentSettings.LastDownloadOption;
            }
            else
            {
                cmbChoice.SelectedIndex = 0;
            }

            EnableDownloadLogs = currentSettings.EnableDownloadLogs;
            logJsonEnabled = currentSettings.EnableJsonLogs;
        }

        public void LogMessage(string logFilePath, string message)
        {
            if (!EnableDownloadLogs || _stopLoggingForCooldown)
            {
                return;
            }

            lock (logFilePath)
            {
                string redactedMessage = message.Replace(Environment.UserName, "[RedactedUsername]");
                File.AppendAllText(logFilePath, $"{DateTime.Now}: {redactedMessage}\n");
            }
        }

        public void LogDownload(string fileName, string url)
        {
            if (EnableDownloadLogs)
            {
                LogMessage(logFilePath, $"Downloaded file: {fileName}, from URL: {url}");
            }
        }

        public void LogJson(string fileName, string jsonContent)
        {
            if (logJsonEnabled)
            {
                lock (jsonLock)
                {
                    try
                    {
                        using StreamWriter writer = File.AppendText(jsonLogFilePath);
                        writer.WriteLine($"[{DateTime.Now}] File: {fileName}");
                        writer.WriteLine(jsonContent);
                        writer.WriteLine();
                    }
                    catch (Exception ex)
                    {
                        outputTextBox.AppendText($"Error: An error occurred while logging the JSON response: {ex.Message}\r\n");
                        LogError($"An error occurred while logging the JSON response: {ex.Message}");
                    }
                }
            }
        }

        public void LogError(string errorMessage)
        {
            if (EnableDownloadLogs)
            {
                LogMessage(logFilePath, $"Error: {errorMessage}");
            }
        }

        class VideoData
        {
            public string Url { get; set; } = string.Empty;
            public List<string> Images { get; set; } = new List<string>();
            public string Id { get; set; } = string.Empty;
            public List<string> AvatarUrls { get; set; }
            public List<string> GifAvatarUrls { get; set; }
            public string Name { get; set; } = string.Empty;
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
            public string unique_Id { get; set; } = string.Empty;
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

        private async void DownloadButton_Click(object sender, EventArgs e)
        {
            outputTextBox.Clear();
            progressBar.Value = 0;
            string choice = cmbChoice.SelectedItem.ToString();
            LogMessage(logFilePath, $"Selected option: {choice}");
            _cancellationTokenSource = new CancellationTokenSource();

            if (choice == "Single Video/Image Download")
            {
                await SingleMediaDownload();
            }
            else if (choice == "Mass Download by Username")
            {
                await MassDownloadByUsername(_cancellationTokenSource.Token);
            }
            else if (choice == "Mass Download from Text File Links")
            {
                string filePath = urlTextBox.Text.Trim();
                LogMessage(logFilePath, $"Selected file path: {filePath}");
                await DownloadFromTextFile(filePath, _cancellationTokenSource.Token);
            }
            else if (choice == "HD Download Video/Image")
            {
                string HDUrl = urlTextBox.Text.Trim();
                await HDSingleMediaDownload(HDUrl);
            }
            else if (choice == "HD Download From Text File Links")
            {
                string filePath = urlTextBox.Text.Trim();
                LogMessage(logFilePath, $"Selected file path: {filePath}");
                await HDDownloadFromTextFile(filePath, _cancellationTokenSource.Token);
            }
            else if (choice == "HD Mass Download by Username")
            {
                await MassDownloadByUsername(_cancellationTokenSource.Token);
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
                await Task.Yield();
                string executablePath = string.Empty;
                mainForm.firefoxfound = false;
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
                        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                        var msPlaywrightPath = Path.Combine(localAppData, "ms-playwright");

                        var firefoxDirectories = Directory.GetDirectories(msPlaywrightPath, "firefox-*");

                        foreach (var dir in firefoxDirectories)
                        {
                            var candidatePath = Path.Combine(dir, "firefox", "firefox.exe");
                            if (File.Exists(candidatePath))
                            {
                                executablePath = candidatePath;
                                mainForm.firefoxfound = true;
                                break;
                            }
                        }

                        if (!mainForm.firefoxfound)
                        {
                            mainForm.LogError("Firefox executable was not found in the expected ms-playwright folder structure.");
                        }
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

            public static IBrowserType GetBrowserTypeForExecutable(string executablePath, IPlaywright playwright)
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

        private async Task CheckPauseStatusAsync()
        {
            if (_isPaused)
            {
                await _pauseTaskCompletionSource.Task;
            }
        }

        private async Task MassDownloadByUsername(CancellationToken token)
        {
            outputTextBox.Clear();
            downloadButton.Enabled = false;
            cmbChoice.Enabled = false;
            string input = urlTextBox.Text.Trim();
            string username = null;
            string baseUrl = null;
            if (string.IsNullOrWhiteSpace(input))
            {
                MessageBox.Show("Please enter a valid URL", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                downloadButton.Enabled = true;
                cmbChoice.Enabled = true;
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


            token.ThrowIfCancellationRequested();

            // Get Selected browser executable path
            string browserExecutablePath = await browserUtility.GetBrowserExecutablePath();
            LogMessage(logFilePath, $"Browser executable path: {browserExecutablePath}");

            // Launch Playwright with the appropriate browser type
            using var playwright = await Playwright.CreateAsync();
            var browserType = BrowserUtility.GetBrowserTypeForExecutable(browserExecutablePath, playwright);

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
                    token.ThrowIfCancellationRequested();

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
                LogMessage(logFilePath, $"Filtered {filteredUrls.Count} URLs by username (@{username})");

                // If no URLs were found, fallback to the second pattern
                if (filteredUrls.Count == 0)
                {
                    filteredUrls = combinedUrls.Where(url => url.Contains($"/{username}/")).ToList();
                    LogMessage(logFilePath, $"Filtered {filteredUrls.Count} URLs by username ({username})");
                }
                else
                {
                    LogMessage(logFilePath, $"Filtered {filteredUrls.Count} URLs by username (@{username})");
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
                string choice = cmbChoice.SelectedItem.ToString();
                if (choice == "Mass Download by Username")
                {
                    await DownloadFromCombinedLinksFile(combinedLinksFilePath, _cancellationTokenSource.Token);
                }
                else if (choice == "HD Mass Download by Username")
                {
                    await HDDownloadFromTextFile(combinedLinksFilePath, _cancellationTokenSource.Token);
                }
            }
            catch (TaskCanceledException)
            {
                await browser.CloseAsync();
                return;
            }
            catch (Exception ex)
            {
                LogError($"{ex.Message}");
            }
            finally
            {
                await browser.CloseAsync();
                LogMessage(logFilePath, "Browser closed successfully");
                downloadButton.Enabled = true;
                cmbChoice.Enabled = true;
                string choice = cmbChoice.SelectedItem.ToString();
                if (choice == "Mass Download by Username")
                {
                    ToastNotification.ShowToast($"Mass Download by Username Completed!", $"Finished downloading all Images/Videos from {username}.", _cancellationTokenSource);
                }
                else if (choice == "HD Mass Download by Username")
                {
                    ToastNotification.ShowToast($"HD Mass Download by Username Completed!", $"Finished downloading all Images/Videos from {username}.", _cancellationTokenSource);
                }
                outputTextBox.AppendText("Download Completed!\r\n");
            }
        }

        private async Task DownloadFromCombinedLinksFile(string filePath, CancellationToken token)
        {
            outputTextBox.Clear();
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

            {
                foreach (var url in urls)
                {
                    var trimmedUrl = url.Trim();
                    if (!string.IsNullOrWhiteSpace(trimmedUrl))
                    {
                        if (withWatermarkCheckBox.Checked)
                        {
                            var dataWithWatermark = await GetMedia(trimmedUrl, true, false, _cancellationTokenSource.Token);

                            if (dataWithWatermark != null)
                            {
                                await DownloadMedia(dataWithWatermark, trimmedUrl, true, false, _cancellationTokenSource.Token);
                            }
                        }

                        else if (noWatermarkCheckBox.Checked)
                        {
                            var dataNoWatermark = await GetMedia(trimmedUrl, false, true, _cancellationTokenSource.Token);

                            if (dataNoWatermark != null)
                            {
                                await DownloadMedia(dataNoWatermark, trimmedUrl, false, true, _cancellationTokenSource.Token);
                            }
                        }
                        else
                        {
                            var dataNoWatermark = await GetMedia(trimmedUrl, false, true, _cancellationTokenSource.Token);

                            if (dataNoWatermark != null)
                            {
                                await DownloadMedia(dataNoWatermark, trimmedUrl, false, true, _cancellationTokenSource.Token);
                            }
                        }
                    }

                    progressBar.Value++;
                }
            }
        }

        private async Task DownloadFromTextFile(string filePath, CancellationToken token)
        {
            outputTextBox.Clear();
            downloadButton.Enabled = false;
            browseFileButton.Enabled = false;
            cmbChoice.Enabled = false;
            if (!File.Exists(filePath))
            {
                MessageBox.Show($"Text file not found or None was Selected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogError("Text file not found or None was Selected!");
                downloadButton.Enabled = true;
                browseFileButton.Enabled = true;
                cmbChoice.Enabled = true;
                return;
            }

            var urls = File.ReadAllLines(filePath);
            LogMessage(logFilePath, $"Read {urls.Length} URLs from file: {filePath}");

            progressBar.Minimum = 0;
            progressBar.Maximum = urls.Length;
            progressBar.Value = 0;

            {

                foreach (var url in urls)
                {
                    var trimmedUrl = url.Trim();
                    if (!string.IsNullOrWhiteSpace(trimmedUrl))
                    {
                        if (withWatermarkCheckBox.Checked)
                        {
                            var dataWithWatermark = await GetMedia(trimmedUrl, true, false, _cancellationTokenSource.Token);

                            if (dataWithWatermark != null)
                            {
                                await DownloadMedia(dataWithWatermark, trimmedUrl, true, false, _cancellationTokenSource.Token);
                            }
                        }

                        else if (noWatermarkCheckBox.Checked)
                        {
                            var dataNoWatermark = await GetMedia(trimmedUrl, false, true, _cancellationTokenSource.Token);

                            if (dataNoWatermark != null)
                            {
                                await DownloadMedia(dataNoWatermark, trimmedUrl, false, true, _cancellationTokenSource.Token);
                            }
                        }
                        else
                        {
                            var dataNoWatermark = await GetMedia(trimmedUrl, false, true, _cancellationTokenSource.Token);

                            if (dataNoWatermark != null)
                            {
                                await DownloadMedia(dataNoWatermark, trimmedUrl, false, true, _cancellationTokenSource.Token);
                            }
                        }
                    }

                    progressBar.Value++;
                }
            }
            downloadButton.Enabled = true;
            browseFileButton.Enabled = true;
            cmbChoice.Enabled = true;
            ToastNotification.ShowToast($"Mass Download from Text File Completed!", $"Finished downloading all {urls.Length} Images/Videos from your Text File.", _cancellationTokenSource);
            outputTextBox.AppendText("Download Completed!\r\n");
        }

        private async Task SingleMediaDownload()
        {
            outputTextBox.Clear();
            downloadButton.Enabled = false;
            cmbChoice.Enabled = false;

            var url = urlTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(url))
            {
                MessageBox.Show("Please enter a valid URL", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                downloadButton.Enabled = true;
                cmbChoice.Enabled = true;
                return;
            }

            try
            {
                progressBar.Minimum = 0;
                progressBar.Maximum = 100;
                progressBar.Value = 0;

                var trimmedUrl = url.Trim();

                {
                    if (withWatermarkCheckBox.Checked)
                    {
                        var dataWithWatermark = await GetMedia(trimmedUrl, true, false, _cancellationTokenSource.Token);

                        if (dataWithWatermark != null)
                        {
                            await DownloadMedia(dataWithWatermark, trimmedUrl, true, false, _cancellationTokenSource.Token);
                        }
                    }

                    else if (noWatermarkCheckBox.Checked)
                    {
                        var dataNoWatermark = await GetMedia(trimmedUrl, false, true, _cancellationTokenSource.Token);

                        if (dataNoWatermark != null)
                        {
                            await DownloadMedia(dataNoWatermark, trimmedUrl, false, true, _cancellationTokenSource.Token);
                        }
                    }
                    else
                    {
                        var dataNoWatermark = await GetMedia(trimmedUrl, false, true, _cancellationTokenSource.Token);

                        if (dataNoWatermark != null)
                        {
                            await DownloadMedia(dataNoWatermark, trimmedUrl, false, true, _cancellationTokenSource.Token);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError($"An error occurred: {ex.Message}");
            }
            finally
            {
                progressBar.Value = 100;
                downloadButton.Enabled = true;
                cmbChoice.Enabled = true;
                outputTextBox.AppendText("Download Completed!\r\n");
            }
        }

        private async Task HDSingleMediaDownload(string HDUrl)
        {
            outputTextBox.Clear();
            downloadButton.Enabled = false;
            cmbChoice.Enabled = false;

            var url = urlTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(url))
            {
                MessageBox.Show("Please enter a valid URL", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                downloadButton.Enabled = true;
                cmbChoice.Enabled = true;
                return;
            }

            try
            {
                progressBar.Minimum = 0;
                progressBar.Maximum = 100;
                progressBar.Value = 0;

                var trimmedUrl = url.Trim();

                if (string.IsNullOrEmpty(trimmedUrl))
                {
                    LogError($"MediaID not found for URL {trimmedUrl}");
                    return;
                }
                else
                {
                    if (trimmedUrl.Contains("/photo/"))
                    {
                        outputTextBox.AppendText($"Photo URL detected: {trimmedUrl}.\r\n");
                        await HDImageDownload(trimmedUrl, _cancellationTokenSource.Token);
                    }
                    else if (trimmedUrl.Contains("/video/"))
                    {
                        outputTextBox.AppendText($"Video URL detected: {trimmedUrl}.\r\n");
                        await HDVideoDownload(trimmedUrl, _cancellationTokenSource.Token);
                    }
                    else if (trimmedUrl.Contains("vm.tiktok.com") || trimmedUrl.Contains("vt.tiktok.com"))
                    {
                        var redirectedUrl = await GetRedirectUrl(trimmedUrl, _cancellationTokenSource.Token);
                        if (string.IsNullOrEmpty(redirectedUrl))
                        {
                            outputTextBox.AppendText($"Could not resolve shorturl {trimmedUrl}\r\n");
                            LogError($"MediaID not found for URL {trimmedUrl}");
                            return;
                        }
                        if (redirectedUrl.Contains("/photo/"))
                        {
                            outputTextBox.AppendText($"Photo URL from shorturl detected: {redirectedUrl}.\r\n");
                            await HDImageDownload(redirectedUrl, _cancellationTokenSource.Token);
                        }
                        else if (redirectedUrl.Contains("/video/"))
                        {
                            outputTextBox.AppendText($"Video URL from shorturl detected: {redirectedUrl}.\r\n");
                            await HDVideoDownload(redirectedUrl, _cancellationTokenSource.Token);
                        }
                    }
                    else
                    {
                        outputTextBox.AppendText($"Unsupported URL format: {trimmedUrl}\r\n");
                        LogError($"Unsupported URL format: {trimmedUrl}");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError($"An error occurred: {ex.Message}");
            }
            finally
            {
                progressBar.Value = 100;
                downloadButton.Enabled = true;
                cmbChoice.Enabled = true;
                outputTextBox.AppendText("Download Completed!\r\n");
            }
        }

        private async Task HDDownloadFromTextFile(string filePath, CancellationToken token)
        {
            outputTextBox.Clear();
            downloadButton.Enabled = false;
            browseFileButton.Enabled = false;
            cmbChoice.Enabled = false;
            if (!File.Exists(filePath))
            {
                MessageBox.Show($"Text file not found or None was Selected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogError("Text file not found or None was Selected!");
                downloadButton.Enabled = true;
                browseFileButton.Enabled = true;
                cmbChoice.Enabled = true;
                return;
            }

            var urls = await File.ReadAllLinesAsync(filePath, token);
            LogMessage(logFilePath, $"Read {urls.Length} URLs from file: {filePath}");

            progressBar.Minimum = 0;
            progressBar.Maximum = urls.Length;
            progressBar.Value = 0;

            {
                foreach (var url in urls)
                {
                    string trimmedUrl = url.Trim();
                    if (!string.IsNullOrEmpty(trimmedUrl))
                    {
                        if (trimmedUrl.Contains("/photo/"))
                        {
                            await HDImageDownload(trimmedUrl, _cancellationTokenSource.Token);
                        }
                        else if (trimmedUrl.Contains("/video/"))
                        {
                            await HDVideoDownload(trimmedUrl, _cancellationTokenSource.Token);
                        }
                        else if (trimmedUrl.Contains("vm.tiktok.com") || trimmedUrl.Contains ("vt.tiktok.com"))
                        {
                            var redirectedUrl = await GetRedirectUrl(trimmedUrl, _cancellationTokenSource.Token);
                            if (string.IsNullOrEmpty(redirectedUrl))
                            {
                                LogError($"MediaID not found for URL {trimmedUrl}");
                                continue;
                            }
                            if (redirectedUrl.Contains("/photo/"))
                            {
                                await HDImageDownload(redirectedUrl, _cancellationTokenSource.Token);
                            }
                            else if (redirectedUrl.Contains("/video/"))
                            {
                                await HDVideoDownload(redirectedUrl, _cancellationTokenSource.Token);
                            }
                        }
                        else
                        {
                            LogError($"Unsupported URL format: {trimmedUrl}");
                            return;
                        }

                        progressBar.Value++;
                    }
                }
            }
            downloadButton.Enabled = true;
            browseFileButton.Enabled = true;
            cmbChoice.Enabled = true;
            string choice = cmbChoice.SelectedItem.ToString();
            if (choice == "HD Download From Text File Links")
            {
                ToastNotification.ShowToast($"HD Mass Download from Text File Completed!", $"Finished downloading all {urls.Length} Images/Videos from your Text File.", _cancellationTokenSource);
                outputTextBox.AppendText("Download Completed!\r\n");
            }
        }

        public async Task<string> GetMediaUrl(string url, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                string redirectedUrl = url;
                string videoId = string.Empty;

                if (url.Contains("vm.tiktok.com") || (url.Contains("vt.tiktok.com")))
                {
                    redirectedUrl = await GetRedirectUrl(url, token);
                }

                token.ThrowIfCancellationRequested();

                if (redirectedUrl.Contains("/photo/"))
                {
                    var match = Regex.Match(redirectedUrl, @"/photo/(\d+)");
                    if (match.Success)
                    {
                        videoId = match.Groups[1].Value;
                    }
                }

                token.ThrowIfCancellationRequested();

                if (redirectedUrl.Contains("/video/"))
                {
                    var match = Regex.Match(redirectedUrl, @"/video/(\d+)");
                    if (match.Success)
                    {
                        videoId = match.Groups[1].Value;
                    }
                }

                if (!string.IsNullOrEmpty(videoId))
                {
                    return videoId;
                }

                // Returning the redirected URL as a fallback if a direct video ID isn't found.
                return redirectedUrl;
            }
            catch (TaskCanceledException)
            {
                return string.Empty;
            }
            catch (Exception ex)
            {
                if (!token.IsCancellationRequested)
                {
                    outputTextBox.AppendText($"Error occurred while extracting MediaID: {ex.Message} - {url}\r\n");
                    LogError($"An error occurred while extracting MediaID: {ex.Message} - {url}");
                }
                return string.Empty;
            }
        }

        public async Task<string> GetMediaID(string url, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                if (url.Contains("/t/"))
                {
                    using (var client = new HttpClient())
                    {
                        var response = await client.GetAsync(url, token);
                        url = response.RequestMessage?.RequestUri.Segments.LastOrDefault()?.TrimEnd('/') ?? string.Empty;
                    }
                }
                else
                {
                    url = await GetRedirectUrl(url, token);
                }

                token.ThrowIfCancellationRequested();

                var matching = url.Contains("/video/");
                var matchingPhoto = url.Contains("/photo/");
                var startIndex = url.IndexOf("/video/") + 7;
                var endIndex = startIndex + 19;

                if (matchingPhoto)
                {
                    startIndex = url.IndexOf("/photo/") + 7;
                    endIndex = startIndex + 19;
                }
                else if (!matching)
                {
                    if (!token.IsCancellationRequested)
                    {
                        outputTextBox.AppendText($"Error: URL not found - {url}\r\n");
                        LogError($"URL not found - {url}");
                    }
                    return string.Empty;
                }

                if (endIndex > url.Length)
                {
                    endIndex = startIndex + 18;
                    if (endIndex > url.Length || endIndex <= startIndex)
                    {
                        if (!token.IsCancellationRequested)
                        {
                            outputTextBox.AppendText($"Error: Invalid URL format or insufficient length - {url}\r\n");
                            LogError($"Invalid URL format or insufficient length - {url}");
                        }
                        return string.Empty;
                    }
                }

                if (startIndex < 0 || endIndex < startIndex)
                {
                    if (!token.IsCancellationRequested)
                    {
                        outputTextBox.AppendText($"Error: Invalid URL format - {url}\r\n");
                        LogError($"Invalid URL format - {url}");
                    }
                    return string.Empty;
                }

                var MediaID = url.Substring(startIndex, endIndex - startIndex);

                return MediaID;
            }
            catch (TaskCanceledException)
            {
                return string.Empty;
            }
            catch (Exception ex)
            {
                if (!token.IsCancellationRequested)
                {
                    outputTextBox.AppendText($"Error occurred while extracting MediaID: {ex.Message} - {url}\r\n");
                    LogError($"An erro occurred while extracting MediaID: {ex.Message} - {url}");
                }
                return string.Empty;
            }
        }

        private async Task<string> GetRedirectUrl(string url, CancellationToken token)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token);
                    return response.RequestMessage?.RequestUri?.AbsoluteUri ?? url;
                }
            }
            catch (TaskCanceledException)
            {
                return string.Empty;
            }
            catch (Exception ex)
            {
                outputTextBox.AppendText($"An error occurred while trying to get the Redirect-URL: {ex.Message}\r\n");
                LogError($"An error occurred while trying to get the Redirect-URL: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// The method which makes requests to an official TikTok API endpoint, which is capped at SD quality and only allows 1 request every 1-30 seconds.
        /// </summary>
        private static string lastCheckedUsername = string.Empty;

        private async Task<VideoData?> GetMedia(string url, bool withWatermark, bool noWatermark, CancellationToken token)
        {
            var MediaID = await GetMediaID(url, token);
            var username = await ExtractUsernameFromUrl(url, token);

            string userFolderPath = Path.Combine(downloadFolderPath, username);
            string indexFilePath = Path.Combine(userFolderPath, $"{username}_index.txt");

            if (username != lastCheckedUsername)
            {
                lastCheckedUsername = username;
            }

            bool videoAlreadyDownloaded = false;

            if (File.Exists(indexFilePath))
            {
                var downloadedIds = await ReadDownloadedIdsInChunks(indexFilePath, token);

                if (downloadAvatarsCheckBox.Checked)
                {
                    var avatarIndexNumberPattern = $"{username}_\\d+";
                    bool normalAvatarsExist = downloadedIds.Any(id => id.StartsWith($"{username}_") && !id.Contains("GIF"));
                    if (normalAvatarsExist)
                    {
                        outputTextBox.AppendText($"Avatars for Media {username} already downloaded. Skipping...\r\n");
                        return null;
                    }

                    var gifAvatarIndexNumberPattern = $"{username}_GIF_\\d+";
                    if (downloadedIds.Any(id => id.StartsWith($"{username}_GIF_") && !id.Contains("Watermark")))
                    {
                        outputTextBox.AppendText($"GIF Avatars for Media {username} already downloaded. Skipping...\r\n");
                        return null;
                    }
                }

                if (!downloadAvatarsCheckBox.Checked)
                {
                    if (downloadedIds.Contains(MediaID))
                    {
                        outputTextBox.AppendText($"Media {MediaID} already downloaded. Skipping...\r\n");
                        videoAlreadyDownloaded = true;
                    }
                    else
                    {
                        var indexNumberPattern = $"{MediaID}_";
                        if (downloadedIds.Any(id => id.StartsWith(indexNumberPattern) && !id.Contains("_HD")))
                        {
                            outputTextBox.AppendText($"Media {MediaID} already downloaded. Skipping...\r\n");
                            videoAlreadyDownloaded = true;
                        }
                    }
                }
            }
            if (videoAlreadyDownloaded)
            {
                LogMessage(logFilePath, $"Media {MediaID} already exists.");
                return null;
            }

            var apiUrl = $"https://api22-normal-c-alisg.tiktokv.com/aweme/v1/feed/?aweme_id={MediaID}&iid=7238789370386695942&device_id=7238787983025079814&resolution=1080*2400&channel=googleplay&app_name=musical_ly&version_code=350103&device_platform=android&device_type=Pixel+7&os_version=13";

            using (var client = new HttpClient())
            {
                try
                {
                    var finalUrl = await GetRedirectUrl(url, token);
                    var request = new HttpRequestMessage(HttpMethod.Options, apiUrl);
                    var response = await client.SendAsync(request, token);

                    if (response.StatusCode == HttpStatusCode.TooManyRequests)
                    {
                        LogMessage(logFilePath, "Received a Http 429 error (TooManyRequests), retrying after 5 Second delay...");
                        outputTextBox.AppendText($"TikTok API received too many requests (Http Code 429). Waiting 5 seconds before retrying the download...\r\n");
                        await Task.Delay(5000, token);
                        return await GetMedia(url, withWatermark, noWatermark, _cancellationTokenSource.Token);
                    }

                    response.EnsureSuccessStatusCode();

                    var json = await response.Content.ReadAsStringAsync();
                    LogJson($"API_Response_For_'{MediaID}'", json);

                    if (string.IsNullOrWhiteSpace(json))
                    {
                        outputTextBox.AppendText($"Error: Received empty JSON response for MediaID: {MediaID}\r\n");
                        return null;
                    }

                    token.ThrowIfCancellationRequested();

                    var data = JsonSoft.JsonConvert.DeserializeObject<ApiData>(json);
                    if (data?.aweme_list == null || data.aweme_list.Count == 0)
                    {
                        LogError($"No aweme_list found in JSON response for MediaID: {MediaID}");
                        outputTextBox.AppendText($"Error: No aweme_list found in JSON response for MediaID: {MediaID}\r\n");
                        return null;
                    }

                    var video = data.aweme_list.FirstOrDefault();

                    if (video?.aweme_id != MediaID)
                    {
                        LogError($"MediaID mismatch in JSON response for MediaID: {MediaID}");
                        outputTextBox.AppendText($"Error: MediaID mismatch in JSON response for MediaID: {MediaID}\r\n");
                        return null;
                    }

                    var urlMedia = noWatermark ? video?.video?.play_addr?.url_list.FirstOrDefault()
                                               : (withWatermark ? video?.video?.download_addr?.url_list.FirstOrDefault()
                                                                : video?.video?.play_addr?.url_list.FirstOrDefault());
                    var imageUrls = video?.image_post_info?.images?.Select(img => img.display_image.url_list.FirstOrDefault()).ToList();
                    var avatarUrls = video?.author?.avatar_medium?.url_list ?? new List<string>();
                    var gifAvatarUrls = video?.author?.video_icon?.url_list ?? new List<string>();
                    var uniqueId = video?.author?.unique_Id;

                    if (urlMedia == null)
                    {
                        if (noWatermark)
                        {
                            LogError($"No Watermark free URL found for MediaID: {MediaID}.");
                            outputTextBox.AppendText($"Skipping download link for MediaID: {MediaID} due to missing No Watermark URL.\r\n");
                        }
                        else if (withWatermark)
                        {
                            LogError($"No Watermark URL found for MediaID: {MediaID}.");
                            outputTextBox.AppendText($"Skipping download link for MediaID: {MediaID} due to missing Watermark URL.\r\n");
                        }
                        else
                        {
                            LogError($"No media URL found for MediaID: {MediaID}.");
                            outputTextBox.AppendText($"Skipping download link for MediaID: {MediaID} due to missing media URL.\r\n");
                        }

                        return null;
                    }

                    return new VideoData 
                    {
                        Url = urlMedia,
                        Images = imageUrls ?? new List<string>(),
                        Id = MediaID,
                        AvatarUrls = avatarUrls,
                        GifAvatarUrls = gifAvatarUrls,
                        Name = uniqueId?.ToString() ?? string.Empty,
                    };
                }
                catch (TaskCanceledException)
                {
                    return null;
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

        /// <summary>
        /// The following 2 HD Download Methods are temporary and will be removed as soon as the Tikwm.com default API supports HD Images and high-bitrate HD Videos again.
        /// Or when I find a better solution for HD Downloads and/or when I'm done with the rewrite.
        /// </summary>
        private async Task HDImageDownload(string imgtikokUrl, CancellationToken token, int retryDepth = 0)
        {
            string usernameFromUrl = null;
            string img = null;
            var photoIdPattern = @"https:\/\/www\.tiktok\.com\/@([\w\.]+)\/photo\/(\d+)";
            var match = Regex.Match(imgtikokUrl, photoIdPattern);
            if (match.Success)
            {
                usernameFromUrl = match.Groups[1].Value;
                img = match.Groups[2].Value;
                string userFolderPath = Path.Combine(downloadFolderPath, usernameFromUrl);
                string indexFilePath = Path.Combine(userFolderPath, $"{usernameFromUrl}_index.txt");
                if (File.Exists(indexFilePath))
                {
                    var downloadedIds = await ReadDownloadedIdsInChunks(indexFilePath, token);
                    if (downloadedIds.Any(id => id.Contains($"{img}_HD")))
                    {
                        outputTextBox.AppendText($"Media {img}_HD.jpg already downloaded. Skipping...\r\n");
                        LogMessage(logFilePath, $"Media {img}_HD.jpg already exists.");
                        return;
                    }
                }
            }

            string mediaId = await GetMediaUrl(imgtikokUrl, token);
            if (string.IsNullOrEmpty(mediaId))
            {
                return; // URL was invalid or cancelled
            }
            if (retryDepth >= 5)
            {
                outputTextBox.AppendText($"Failed 5 retries... skipping media {mediaId}.\r\n");
                LogError($"Failed 5 retries... skipping media {mediaId}.");
                return;
            }

            string apiEndpoint = "https://www.tikwm.com/api/";      // <-- we still need to use the "default" api Endpoint of Tikwm.com for HD Images.
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var urlWithParams = $"{apiEndpoint}?url={mediaId}&hd=1";    // Set hd=1 for HD download
                    HttpResponseMessage response = await client.GetAsync(urlWithParams, token);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        dynamic responseData = JsonSoft.JsonConvert.DeserializeObject(responseBody);
                        LogJson($"API_Response_For_{mediaId}", responseBody);

                        if (responseData.code == 0)
                        {
                            // get Username from response otherwise fallback to URL
                            string username = responseData.data.author.unique_id;
                            if (string.IsNullOrEmpty(username) && usernameFromUrl != null)
                            {
                                username = usernameFromUrl;
                            }

                            string userFolderPath = Path.Combine(downloadFolderPath, username);
                            Directory.CreateDirectory(userFolderPath);
                            // Download the Images
                            if (responseData.data.images != null)
                            {
                                string imagesFolderPath = Path.Combine(userFolderPath, "Images");
                                Directory.CreateDirectory(imagesFolderPath);

                                string indexFilePath = Path.Combine(userFolderPath, $"{username}_index.txt");
                                bool alreadyDownloaded = false;
                                if (File.Exists(indexFilePath))
                                {
                                    var images = responseData.data.images;
                                    int imageCount = images.Count;
                                    var downloadedIds = await ReadDownloadedIdsInChunks(indexFilePath, token);
                                    bool allImagesDownloaded = true;
                                    for (int i = 0; i < imageCount; i++)
                                    {
                                        if (!downloadedIds.Contains($"{mediaId}_{i + 1}.jpg"))
                                        {
                                            allImagesDownloaded = false;
                                            break;
                                        }
                                    }
                                    if (allImagesDownloaded)
                                    {
                                        outputTextBox.AppendText($"Media set {mediaId} already downloaded. Skipping...\r\n");
                                        LogMessage(logFilePath, $"Media set {mediaId} already exists.");
                                        alreadyDownloaded = true;
                                    }
                                }

                                if (!alreadyDownloaded)
                                {
                                    var images = responseData.data.images;
                                    int imageCount = images.Count;
                                    AppendTextToOutput($"Downloading {imageCount} Images from User: {username}\r\n");
                                    LogMessage(logFilePath, $"Downloading {imageCount} Images from User: {username}");

                                    for (int i = 0; i < imageCount; i++)
                                    {
                                        string imageUrl = images[i].ToString();
                                        string filename = $"{mediaId}_{i + 1}.jpg";
                                        string fullPath = Path.Combine(imagesFolderPath, filename);

                                        bool success = false;
                                        int retryCount = 5;
                                        while (!success && retryCount > 0)
                                        {
                                            try
                                            {
                                                outputTextBox.AppendText($"Downloading Image {filename}\r\n");
                                                LogMessage(logFilePath, $"Downloading {filename}");
                                                await DownloadVideoWithBufferedWrite(client, imageUrl, fullPath, token);
                                                success = true;
                                                await File.AppendAllTextAsync(indexFilePath, $"{filename}\n");
                                            }
                                            catch (TaskCanceledException)
                                            {
                                                return;
                                            }
                                            catch (OperationCanceledException)
                                            {
                                                retryCount--;
                                                if (retryCount > 0)
                                                {
                                                    outputTextBox.AppendText("Connection lost. Retrying download...\r\n");
                                                    LogError($"Connection lost. Retrying download: {retryDepth}");
                                                }
                                                else
                                                {
                                                    outputTextBox.AppendText($"Failed to Download {mediaId}\r\n");
                                                    LogError($"Failed to Download {mediaId}");
                                                    return;
                                                }
                                            }
                                            catch (IOException ex) when (ex.InnerException is SocketException)
                                            {
                                                retryCount--;
                                                if (retryCount > 0)
                                                {
                                                    outputTextBox.AppendText("Connection lost. Retrying download...\r\n");
                                                    LogError($"Connection lost. Retrying download: attempt {retryDepth} of {retryCount}");
                                                }
                                                else
                                                {
                                                    outputTextBox.AppendText("Failed to resume download after multiple attempts.\r\n");
                                                    LogError($"Failed to resume download after {retryCount} attempts.");
                                                    return;
                                                }
                                            }
                                        }
                                    }
                                    LogMessage(logFilePath, $"Images saved to {imagesFolderPath}");
                                    outputTextBox.AppendText($"Downloaded {imageCount} Images from {username} Successfully...\r\n");
                                    string choice = cmbChoice.SelectedItem.ToString();
                                    if (choice == "HD Download Video/Image")
                                    {
                                        ToastNotification.ShowToast($"HD Image(s) from @{username} Downloaded", $"Download of {imageCount} Image(s) completed successfully!");
                                    }
                                }
                            }
                            else
                            {
                                outputTextBox.AppendText($"No downloadable media found for {imgtikokUrl}\r\n");
                                LogError($"No downloadable media found for {imgtikokUrl}");
                            }
                        }
                        else
                        {
                            // If the response is not successful, check again if the media is already downloaded
                            if (!string.IsNullOrEmpty(img) && !string.IsNullOrEmpty(usernameFromUrl))
                            {
                                string userFolderPath = Path.Combine(downloadFolderPath, usernameFromUrl);
                                string indexFilePath = Path.Combine(userFolderPath, $"{usernameFromUrl}_index.txt");
                                if (File.Exists(indexFilePath))
                                {
                                    var downloadedIds = await ReadDownloadedIdsInChunks(indexFilePath, token);
                                    if (downloadedIds.Any(id => id.Contains($"{img}_HD")))
                                    {
                                        outputTextBox.AppendText($"Media {img}_HD.jpg already downloaded. Skipping...\r\n");
                                        return;
                                    }
                                }
                            }

                            outputTextBox.AppendText($"Media {mediaId} does not exist or token is rate limited, trying again {5 - retryDepth} times...\r\n");
                            LogError($"Media {mediaId} does not exist or token is rate limited.");
                            await Task.Delay(2000, token);
                            await HDImageDownload(imgtikokUrl, token, retryDepth + 1);
                        }
                    }
                    else
                    {
                        outputTextBox.AppendText("Error: Download of HD Media failed!\r\n");
                        LogError($"Download of HD Media failed! Status Response: {response.StatusCode}");
                    }
                }
                catch (HttpRequestException)
                {
                    outputTextBox.AppendText("Small Cooldown, Continue after 5 Seconds.\r\n");
                    await Task.Delay(5000, token);
                }
                catch (TaskCanceledException)
                {
                    return;
                }
            }
        }

        private async Task HDVideoDownload(string tiktokUrl, CancellationToken token, int retryDepth = 0)
        {
            // Extract username and video ID from the TikTok URL for an early check
            string usernameFromUrl = null;
            string vid = null;
            var videoIdPattern = @"https:\/\/www\.tiktok\.com\/@([\w\.]+)\/video\/(\d+)";
            var match = Regex.Match(tiktokUrl, videoIdPattern);
            if (match.Success)
            {
                usernameFromUrl = match.Groups[1].Value;
                vid = match.Groups[2].Value;
                string userFolderPath = Path.Combine(downloadFolderPath, usernameFromUrl);
                string indexFilePath = Path.Combine(userFolderPath, $"{usernameFromUrl}_index.txt");
                if (File.Exists(indexFilePath))
                {
                    var downloadedIds = await ReadDownloadedIdsInChunks(indexFilePath, token);
                    if (downloadedIds.Any(id => id.Contains($"{vid}_HD")))
                    {
                        outputTextBox.AppendText($"Media {vid}_HD.mp4 already downloaded. Skipping...\r\n");
                        LogMessage(logFilePath, $"Media {vid}_HD.mp4 already exists.");
                        return;
                    }
                }
            }

            string mediaId = await GetMediaUrl(tiktokUrl, token);
            if (string.IsNullOrEmpty(mediaId))
            {
                return; // URL was invalid or cancelled
            }

            if (retryDepth >= 5)
            {
                outputTextBox.AppendText($"Failed 5 retries... skipping video {mediaId}.\r\n");
                LogError($"Failed 5 retries... skipping video {mediaId}.");
                return;
            }
            /// <summary>
            /// This is the "new" Tikwm.com API Endpoint for high-bitrate HD Video Downloads which isn't capable of downloading HD Images (yet?).
            /// It is also not as reliable as the old API Endpoint and will make you hit the 5,000 daily request cap faster.
            /// </summary>
            string submitEndpoint = "https://www.tikwm.com/api/video/task/submit";
            string resultEndpointTemplate = "https://www.tikwm.com/api/video/task/result?task_id={0}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var submitcontent = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "url", mediaId },
                        { "web", "1" }
                    });

                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri(submitEndpoint),
                        Content = submitcontent
                    };

                    HttpResponseMessage submitResponse = await client.SendAsync(request, token);
                    submitResponse.EnsureSuccessStatusCode();

                    string submitResponseBody = await submitResponse.Content.ReadAsStringAsync();
                    dynamic submitData = JsonSoft.JsonConvert.DeserializeObject(submitResponseBody);
                    LogJson($"Submit_Endpoint_Response_For_{mediaId}", submitResponseBody);

                    if (submitData.code != 0 || submitData.data.task_id == null)
                    {
                        outputTextBox.AppendText($"Failed to submit task for {mediaId}. Response: {submitResponseBody}\r\n");
                        LogError($"Failed to submit task for {mediaId}. Response: {submitResponseBody}");
                        return;
                    }

                    string taskId = submitData.data.task_id;
                    string resultEndpoint = string.Format(resultEndpointTemplate, taskId);
                    bool isReady = false;
                    int maxRetries = 15;
                    int delayMs = 500;
                    dynamic responseData = null;

                    for (int attempt = 0; attempt < maxRetries; attempt++)
                    {
                        HttpResponseMessage resultResponse = await client.GetAsync(resultEndpoint, token);
                        resultResponse.EnsureSuccessStatusCode();
                        string resultResponseBody = await resultResponse.Content.ReadAsStringAsync();
                        responseData = JsonSoft.JsonConvert.DeserializeObject(resultResponseBody);
                        LogJson($"Result_Endpoint_Response_For_{mediaId}", resultResponseBody);

                        if (responseData.code == 0 && responseData.data != null)
                        {
                            int status = responseData.data.status;
                            long size = responseData.data.detail.size;

                            if (status == 2 && size > 0)
                            {
                                outputTextBox.AppendText($"Download Task for {mediaId} is ready. Size: {size} bytes.\r\n");
                                LogMessage(logFilePath, $"Download Task for {mediaId} is ready. Size: {size} bytes.");
                                isReady = true;
                                break;
                            }
                        }

                        await Task.Delay(delayMs, token);
                    }

                    if (!isReady)
                    {
                        outputTextBox.AppendText($"Download Task for {mediaId} is not ready after {maxRetries} attempts.\r\n");
                        LogError($"Download Task for {mediaId} is not ready after {maxRetries} attempts.");
                        return;
                    }

                    if (responseData.code == 0)
                    {
                        // get Username from response otherwise fallback to URL
                        string username = responseData.data.detail.author.unique_id;
                        if (string.IsNullOrEmpty(username) && usernameFromUrl != null)
                        {
                            username = usernameFromUrl;
                        }

                        string userFolderPath = Path.Combine(downloadFolderPath, username);
                        Directory.CreateDirectory(userFolderPath);

                        // Check whether the response contains an HD video
                        if (responseData.data.detail.play_url != null && (responseData.data.detail.size != "0" || responseData.data.detail.id != null))
                        {
                            string videosFolderPath = Path.Combine(userFolderPath, "Videos");
                            Directory.CreateDirectory(videosFolderPath);

                            if (string.IsNullOrEmpty(vid))
                            {
                                var videoIdPatternFallback = @"(?:https:\/\/www\.tiktok\.com\/(?:@[\w\.]+\/video\/))(\d+)";
                                var matchFallback = Regex.Match(mediaId, videoIdPatternFallback);
                                vid = matchFallback.Success ? matchFallback.Groups[1].Value : mediaId;
                            }
                            string filename = $"{vid}_HD.mp4";
                            string fullPath = Path.Combine(videosFolderPath, filename);

                            // Check if the video has already been downloaded
                            string indexFilePath = Path.Combine(userFolderPath, $"{username}_index.txt");
                            if (File.Exists(indexFilePath))
                            {
                                var downloadedIds = await ReadDownloadedIdsInChunks(indexFilePath, token);
                                if (downloadedIds.Any(id => id.Contains($"{vid}_HD")))
                                {
                                    outputTextBox.AppendText($"Media {filename} already downloaded. Skipping...\r\n");
                                    LogMessage(logFilePath, $"Media {filename} already exists.");
                                    return;
                                }
                            }

                            // double check response data for completeness
                            string videoUrl = responseData.data.detail.play_url;
                            string size = responseData.data.detail.size.ToString();
                            if (string.IsNullOrEmpty(videoUrl) || size == "0")
                            {
                                await Task.Delay(2000, token);
                                await HDVideoDownload(tiktokUrl, token, retryDepth + 1);
                                return;
                            }

                            AppendTextToOutput($"Downloading HD Video from User: {username}\r\n");
                            LogMessage(logFilePath, $"Downloading HD Video from User: {username}");

                            // Download the video
                            bool success = false;
                            int retryCount = 5;
                            while (!success && retryCount > 0)
                            {
                                try
                                {
                                    LogMessage(logFilePath, $"Downloading {vid}");
                                    await DownloadVideoWithBufferedWrite(client, videoUrl, fullPath, token);
                                    success = true;
                                }
                                catch (TaskCanceledException)
                                {
                                    if (File.Exists(fullPath))
                                        File.Delete(fullPath);
                                    return;
                                }
                                catch (OperationCanceledException)
                                {
                                    retryCount--;
                                    if (retryCount > 0)
                                    {
                                        outputTextBox.AppendText("Connection lost. Retrying download...\r\n");
                                        LogMessage(logFilePath, "Connection lost. Retrying download...");
                                        if (File.Exists(fullPath))
                                        {
                                            File.Delete(fullPath);
                                            LogMessage(logFilePath, $"Deleted {fullPath} due to connection loss.");
                                        }
                                    }
                                    else
                                    {
                                        outputTextBox.AppendText($"Failed to Download {vid}\r\n");
                                        LogError($"Failed to Download {vid}");
                                        if (File.Exists(fullPath))
                                        {
                                            File.Delete(fullPath);
                                            LogMessage(logFilePath, $"Deleted {fullPath} due to unknown download error.");
                                        }
                                        return;
                                    }
                                }
                                catch (IOException ex) when (ex.InnerException is SocketException)
                                {
                                    retryCount--;
                                    if (retryCount > 0)
                                    {
                                        outputTextBox.AppendText("Connection lost. Retrying download...\r\n");
                                        if (File.Exists(fullPath))
                                        {
                                            File.Delete(fullPath);
                                            LogMessage(logFilePath, $"Deleted {fullPath} due to connection loss.");
                                        }
                                    }
                                    else
                                    {
                                        outputTextBox.AppendText("Failed to resume download after multiple attempts.\r\n");
                                        LogError("Failed to resume download after multiple attempts.");
                                        if (File.Exists(fullPath))
                                        {
                                            File.Delete(fullPath);
                                            LogMessage(logFilePath, $"Deleted {fullPath} due to reconnection issues.");
                                        }
                                        return;
                                    }
                                }
                            }
                            await File.AppendAllTextAsync(indexFilePath, $"{vid}_HD\n");
                            LogMessage(logFilePath, $"HD Video File Saved to {fullPath}");
                            string choice = cmbChoice.SelectedItem.ToString();
                            if (choice == "HD Download Video/Image")
                            {
                                ToastNotification.ShowToast($"HD Video from @{username} Downloaded", $"Download of {vid}_HD.mp4 completed successfully!");
                            }
                            outputTextBox.AppendText($"Downloaded HD Video: '{filename}' Successfully...\r\n");
                        }
                        else
                        {
                            outputTextBox.AppendText($"No downloadable media found for {tiktokUrl}\r\n");
                            LogError($"No downloadable media found for {tiktokUrl}");
                        }
                    }
                    else
                    {
                        // If the response is not successful, check again if the media is already downloaded
                        if (!string.IsNullOrEmpty(vid) && !string.IsNullOrEmpty(usernameFromUrl))
                        {
                            string userFolderPath = Path.Combine(downloadFolderPath, usernameFromUrl);
                            string indexFilePath = Path.Combine(userFolderPath, $"{usernameFromUrl}_index.txt");
                            if (File.Exists(indexFilePath))
                            {
                                var downloadedIds = await ReadDownloadedIdsInChunks(indexFilePath, token);
                                if (downloadedIds.Any(id => id.Contains($"{vid}_HD")))
                                {
                                    outputTextBox.AppendText($"Media {vid}_HD.mp4 already downloaded. Skipping...\r\n");
                                    return;
                                }
                            }
                        }

                        outputTextBox.AppendText($"Media {mediaId} does not exist or token is rate limited, trying again {5 - retryDepth} times...\r\n");
                        LogError($"Media {mediaId} does not exist or token is rate limited.");
                        await Task.Delay(2000, token);
                        await HDVideoDownload(tiktokUrl, token, retryDepth + 1);
                    }
                }
                catch (HttpRequestException)
                {
                    outputTextBox.AppendText("Small Cooldown, Continue after 5 Seconds.\r\n");
                    await Task.Delay(5000, token);
                }
                catch (TaskCanceledException)
                {
                    return;
                }
            }
        }

        private async Task<HashSet<string>> ReadDownloadedIdsInChunks(string indexFilePath, CancellationToken token)
        {
            var downloadedIds = new HashSet<string>();

            using (var reader = new StreamReader(indexFilePath, Encoding.UTF8, true, 8192))
            {
                char[] buffer = new char[8192];
                int charsRead;

                while ((charsRead = await reader.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    string chunk = new string(buffer, 0, charsRead);
                    var lines = chunk.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            downloadedIds.Add(line.Trim());
                            //outputTextBox.AppendText($"Downloaded ID: {line}\r\n");
                        }
                    }
                }
            }
            return downloadedIds;
        }
        /// <summary>
        /// SD download method, which uses the response from "GetMedia".
        /// </summary>

        private async Task DownloadMedia(VideoData data, string url, bool withWatermark, bool noWatermark, CancellationToken cancellationToken)
        {
            const int maxRetries = 5;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    string username = data.Name;
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

                            AppendTextToOutput($"Downloading Images from User: {username}\r\n");
                            LogMessage(logFilePath, $"Downloading Images from User: {username}");
                            using (var client = new HttpClient())
                            {
                                using (var stream = await client.GetStreamAsync(imageUrl, cancellationToken))
                                using (var fileStream = File.Create(imageFilePath))
                                {
                                    await stream.CopyToAsync(fileStream, cancellationToken);
                                }
                            }

                            outputTextBox.AppendText($"Downloaded Image: '{imageFileName}' successfully.\r\n");
                            await File.AppendAllTextAsync(indexFilePath, $"{data.Id}_{data.Images.IndexOf(imageUrl)}\n");
                        }
                        string choice = cmbChoice.SelectedItem.ToString();
                        if (choice == "Single Video/Image Download")
                        {
                            ToastNotification.ShowToast($"SD Image(s) from @{username} Downloaded", $"Download of {data.Images.Count} Image(s) completed successfully!");
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
                                AppendTextToOutput($"Downloading Video from User: {username}\r\n");
                                LogMessage(logFilePath, $"Downloading Video from User: {username}");
                                LogMessage(logFilePath, $"Downloading Video: {url}");
                                using (var client = new HttpClient())
                                {
                                    await DownloadVideoWithBufferedWrite(client, data.Url, videoFilePath, cancellationToken);
                                }
                                outputTextBox.AppendText($"Downloaded Video: '{videoFileName}' successfully.\r\n");
                                await File.AppendAllTextAsync(indexFilePath, $"{data.Id}\n");
                            }
                            string choice = cmbChoice.SelectedItem.ToString();
                            if (choice == "Single Video/Image Download")
                            {
                                ToastNotification.ShowToast($"SD Video from @{username} Downloaded", $"Download of {data.Id}.mp4 completed successfully!");
                            }
                        }
                    }

                    if (downloadAvatarsCheckBox.Checked)
                    {
                        await DownloadAvatars(data, url, username, cancellationToken);
                        LogMessage(logFilePath, "Download Avatars Checkbox is Checked.");
                    }

                    return;
                }
                catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    outputTextBox.AppendText($"Error: The media download failed with a 429 error: {url}\r\n");
                    outputTextBox.AppendText("Retrying in 5 seconds...\r\n");
                    LogError($"The media download failed with a 429 error: {ex.Message}");
                    await Task.Delay(5000, cancellationToken);
                }
                catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Forbidden)
                {
                    outputTextBox.AppendText($"Error: The download request was blocked by the API. This might be because your IP address is restricted. To resolve this, try connecting through a VPN and retrying the download.\r\n");
                    LogError($"The media download failed with a 403 error: {ex.Message}");
                    break;
                }
                catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    outputTextBox.AppendText($"Error: The media download failed with a 404 error: {url}\r\n");
                    LogError($"The media download failed with a 404 error: {ex.Message}");
                    break;
                }
                catch (HttpRequestException ex)
                {
                    outputTextBox.AppendText($"Error: An error occurred while downloading Media: {ex.Message}\r\n");
                    outputTextBox.AppendText("Retrying in 5 seconds...\r\n");
                    LogError($"An error occurred while downloading Media: {ex.Message}");
                    await Task.Delay(5000, cancellationToken);
                }
                catch (TargetInvocationException ex)
                {
                    outputTextBox.AppendText($"Error: TargetInvocationException occurred: {ex.InnerException?.Message}\r\n");
                    outputTextBox.AppendText($"Inner Exception 1: {ex.InnerException?.InnerException?.Message}\r\n");
                    outputTextBox.AppendText($"Inner Exception 2: {ex.InnerException?.InnerException?.InnerException?.Message}\r\n");
                    LogError($"TargetInvocationException occurred: {ex.InnerException?.Message}");
                }
                catch (JsonSoft.JsonException ex)
                {
                    outputTextBox.AppendText($"Error: An error occurred while processing JSON response: {ex.Message}\r\n");
                    outputTextBox.AppendText("Retrying in 5 seconds...\n");
                    LogError($"An error occurred while processing JSON response: {ex.Message}");
                    await Task.Delay(5000, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    return;
                }

                if (attempt < maxRetries)
                {
                    outputTextBox.AppendText($"Retrying download (Attempt {attempt} of {maxRetries}) in 5 seconds...\r\n");
                    LogError($"Retrying download (Attempt {attempt} of {maxRetries}) for URL: {url}");
                    await Task.Delay(5000, cancellationToken);
                }
                else
                {
                    outputTextBox.AppendText($"Error: Failed to download media from {url} after {maxRetries} attempts.\r\n");
                }
            }
        }

        private async Task DownloadVideoWithBufferedWrite(HttpClient client, string videoUrl, string fullPath, CancellationToken token)
        {
            int retryCount = 5;
            while (retryCount > 0)
            {
                try
                {
                    long totalBytesRead = 0;
                    if (File.Exists(fullPath))
                    {
                        totalBytesRead = new FileInfo(fullPath).Length;
                    }
                    else
                    {
                        //AppendTextToOutput($"Delay 1900, token Triggered\r\n");
                        await Task.Delay(1900, token);
                    }

                    token.ThrowIfCancellationRequested();

                    using (var response = await client.GetAsync(videoUrl, HttpCompletionOption.ResponseHeadersRead, token))
                    {
                        response.EnsureSuccessStatusCode();

                        using (var responseStream = await response.Content.ReadAsStreamAsync(token))
                        using (var fileStream = new FileStream(fullPath, FileMode.Append, FileAccess.Write, FileShare.None, 8192, true))
                        {
                            byte[] buffer = new byte[8192];
                            int bytesRead;

                            while ((bytesRead = await responseStream.ReadAsync(buffer, token)) > 0)
                            {
                                await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), token);
                                totalBytesRead += bytesRead;

                                await CheckPauseStatusAsync();
                            }
                        }
                    }
                    return;
                }
                catch (TaskCanceledException)
                {
                    return;
                }
                catch (IOException ex) when (ex.InnerException is SocketException)
                {
                    retryCount--;
                    if (retryCount > 0)
                    {
                        outputTextBox.AppendText("Connection lost. Retrying download...\r\n");
                        LogError($"Connection lost. Retrying download... Error: {ex.Message}");
                    }
                    else
                    {
                        outputTextBox.AppendText("Failed to resume download after multiple attempts.\r\n");
                        LogError($"Failed to resume download after multiple attempts. Error: {ex.Message}");
                        return;
                    }
                }
                catch (IOException ex)
                {

                    if (!ex.Message.StartsWith("The response ended prematurely"))
                    {
                        retryCount--;
                        if (retryCount > 0)
                        {
                            outputTextBox.AppendText($"The API response ended prematurely. Retrying download...\r\n");
                            LogError($"{ex.Message}");
                            return;
                        }
                        else
                        {
                            outputTextBox.AppendText("Failed to resume download after multiple attempts.\r\n");
                            LogError($"Failed to resume download after multiple attempts. Error: {ex.Message}");
                            return;
                        }
                    }
                    else
                    {
                        outputTextBox.AppendText("Failed to resume download after multiple attempts.\r\n");
                        LogError($"Failed to resume download after multiple attempts. Error: {ex.Message}");
                        return;
                    }
                }
            }
        }

        private async Task DownloadAvatars(VideoData data, string url, string username, CancellationToken cancellationToken)
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

                        using (var client = new HttpClient())
                        {
                            using (var stream = await client.GetStreamAsync(avatarUrl, cancellationToken))
                            using (var fileStream = File.Create(avatarFilePath))
                            {
                                await stream.CopyToAsync(fileStream, cancellationToken);
                            }
                        }

                        outputTextBox.AppendText($"Downloaded Avatar From '{avatarFileName}' Successfully...\r\n");
                        await File.AppendAllTextAsync(indexFilePath, $"{username}_{data.AvatarUrls.IndexOf(avatarUrl)}\r\n");
                        LogDownload(avatarFileName, avatarUrl);
                    }
                }
                else
                {
                    LogMessage(logFilePath, "No Avatar images to download.");
                    outputTextBox.AppendText("No Avatar images to download.\r\n");
                }

                if (data.GifAvatarUrls != null && data.GifAvatarUrls.Count > 0)
                {
                    foreach (var gifAvatarUrl in data.GifAvatarUrls)
                    {
                        string gifAvatarfileName = $"{username}_GIF_{data.GifAvatarUrls.IndexOf(gifAvatarUrl)}.gif";
                        string gifAvatarfilePath = Path.Combine(avatarsFolderPath, gifAvatarfileName);

                        using (var client = new HttpClient())
                        {
                            using (var stream = await client.GetStreamAsync(gifAvatarUrl, cancellationToken))
                            using (var fileStream = File.Create(gifAvatarfilePath))
                            {
                                await stream.CopyToAsync(fileStream, cancellationToken);
                            }
                        }

                        AppendTextToOutput($"Downloaded GIF Avatar from '{gifAvatarfileName}' Successfully...\r\n");
                        await File.AppendAllTextAsync(indexFilePath, $"{username}_GIF_{data.GifAvatarUrls.IndexOf(gifAvatarUrl)}\r\n");
                        LogDownload(gifAvatarfileName, gifAvatarUrl);
                    }
                }
                else
                {
                    LogMessage(logFilePath, "No GIF Avatar images to download.");
                    outputTextBox.AppendText("No GIF Avatar images to download.\r\n");
                }
            }
            catch (TaskCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                outputTextBox.AppendText($"Error downloading avatars: {ex.Message}\r\n");
                LogError($"Error: {ex.Message}");
            }
        }

        private async Task<string> ExtractUsernameFromUrl(string url, CancellationToken token)
        {
            try
            {
                var finalUrl = await GetRedirectUrl(url, token);
                var segments = finalUrl.Split('/');

                var usernameSegment = segments.FirstOrDefault(s => s.StartsWith("@"));

                if (usernameSegment == null)
                {
                    throw new ArgumentException("Invalid TikTok URL");
                }
                var username = usernameSegment.TrimStart('@');

                return username;
            }
            catch (TaskCanceledException)
            {
                return string.Empty;
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

        private void BrowseFileButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.InitialDirectory = lastBrowsingPath;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    urlTextBox.Text = openFileDialog.FileName;
                    lastBrowsingPath = Path.GetDirectoryName(openFileDialog.FileName);
                    settings.CurrentSettings.LastBrowsingPath = lastBrowsingPath;
                    settings.SaveSettings();
                }
            }
        }

        private void SaveLastDownloadOption()
        {
            if (settings?.CurrentSettings != null && cmbChoice.SelectedItem != null)
            {
                settings.CurrentSettings.LastDownloadOption = cmbChoice.SelectedItem.ToString();
                settings.SaveSettings();
            }
        }

        public static void RunFirefoxScript()
        {
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string scriptPath = Path.Combine(appDirectory, "playwright.ps1");
            string arguments = $"-Command \"& {{ pwsh -File '{scriptPath}' install firefox; Read-Host -Prompt 'Press Enter to exit' }}\"";

            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "powershell",
                    Arguments = arguments,
                    WorkingDirectory = appDirectory,
                    UseShellExecute = true,
                    CreateNoWindow = false
                }
            };

            process.Start();
            process.WaitForExit();

            if (process.ExitCode == 0)
            {
                MessageBox.Show("The Firefox script executed successfully.", "Setup Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("The Firefox script failed to execute. Please check if PowerShell 7 is installed and try running the script again. If the issue persists, open an issue on GitHub.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void CheckForUpdates()
        {
            string url = "https://api.jettcodey.de/ttd/update/update.json";
            //string url = "https://api.jettcodey.de/ttd/dev_update/dev_update.json"; //  Dev Update Server URL

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

                        DialogResult result = MessageBox.Show(
                            $"New Version {latestVersionInfo.Version} available. Would you like to restart the application to apply updates?\nIf you don't trust this Updater, you can always install the latest version from the Github Repository",
                            "Update Available",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Information);

                        if (result == DialogResult.Yes)
                        {
                            // Check if there is a Warning message associated with the Update!
                            if (!string.IsNullOrWhiteSpace(latestVersionInfo.Message))
                            {
                                DialogResult confirm = MessageBox.Show(
                                    latestVersionInfo.Message + "\n\nDo you want to continue with the update?",
                                    "Update Warning",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Warning);

                                if (confirm != DialogResult.Yes)
                                {
                                    // Cancel the update
                                    return;
                                }
                            }

                            DownloadFiles(latestVersionInfo.Files, tempFolder);
                            CreateBatchFile(tempFolder, latestVersionInfo.Version); // Generate Update.bat
                            Application.Exit();
                        }
                    }
                    else
                    {
                        MessageBox.Show($"You are already using the latest Version {latestVersionInfo.Version}.", "No Updates Available", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private static void CreateBatchFile(string tempFolder, string newVersion)
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

                writer.WriteLine($"reg add \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Jettcodey\\TikTok Downloader\" /v Version /t REG_SZ /d \"{newVersion}\" /f");

                writer.WriteLine($"rmdir /s /q \"{tempFolder}\"");

                writer.WriteLine($"start \"\" \"{Path.Combine(appFolder, "TikTok Downloader.exe")}\"");
            }

            // Start the Update.bat file
            Process.Start(new ProcessStartInfo
            {
                FileName = batchFilePath,
                UseShellExecute = true,
                Verb = "runas" // Admin rights required to replace files
            });
        }

        private static void DownloadFiles(List<string> fileUrls, string tempFolder)
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

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow(browserUtility);
            aboutWindow.ShowDialog();
        }

        public void StopButton_Click(object sender, EventArgs e)
        {
            if (_isPaused)
            {
                PauseButton_Click(sender, e);
            }
            _cancellationTokenSource?.Cancel();
            ToastNotification.ShowToast("Download Stopped!", "You stopped the download process.");
            outputTextBox.AppendText("Download Stopped!\r\n");
            LogMessage(logFilePath, "Download got Stopped by User");
            _stopLoggingForCooldown = true;
            Task.Delay(1000).ContinueWith(_ => _stopLoggingForCooldown = false);
        }

        public void PauseButton_Click(object sender, EventArgs e)
        {
            if (_isPaused)
            {
                // Resuming download
                _pauseTaskCompletionSource?.SetResult(true);
                _isPaused = false;
                outputTextBox.AppendText($"Download Continues...\r\n");
                LogMessage(logFilePath, "Download got Resumed by User");
                // Append all buffered text
                if (_pausedTextBuffer.Length > 0)
                {
                    outputTextBox.AppendText(_pausedTextBuffer.ToString());
                    _pausedTextBuffer.Clear();
                }
                pauseButton.Text = "||";
                pauseButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            }
            else
            {
                // Pausing download
                _pauseTaskCompletionSource = new TaskCompletionSource<bool>();
                _isPaused = true;
                outputTextBox.AppendText($"Download Paused!\r\n");
                LogMessage(logFilePath, "Download got Paused by User");
                pauseButton.Text = "▶";
                pauseButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            }
        }

        private void AppendTextToOutput(string text)
        {
            if (_isPaused)
            {
                _pausedTextBuffer.Append(text);
            }
            else
            {
                outputTextBox.AppendText(text);
            }
        }

        private void SettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var settingsDialog = new SettingsDialog(this))
            {
                settingsDialog.ShowDialog();
            }
        }

        private void CheckForUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckForUpdates();
        }

        private void TikTokSigninToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"This feature is still in development and is therefore not available.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            /*TikTokSigninDialog tikTokSigninDialog = new TikTokSigninDialog(browserUtility);
            tikTokSigninDialog.ShowDialog();*/
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

        private async void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = "https://github.com/Jettcodey/TikTok-Downloader/issues";

            string browserPath = await browserUtility.GetBrowserExecutablePath();

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

        private void WithWatermarkCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (withWatermarkCheckBox.Checked)
            {
                noWatermarkCheckBox.Visible = true;
                LogMessage(logFilePath, "With Watermark Checkbox is Checked.");
            }
            else
            {
                noWatermarkCheckBox.Visible = false;
                noWatermarkCheckBox.Checked = false;
                LogMessage(logFilePath, "With Watermark Checkbox is Un-Checked.");
            }
        }

        private void NoWatermarkCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (noWatermarkCheckBox.Checked)
            {
                LogMessage(logFilePath, "No Watermark Checkbox is Checked.");
            }
            else
            {
                LogMessage(logFilePath, "No Watermark Checkbox is Not Checked.");
            }
        }

        private void DownloadAvatarsCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}