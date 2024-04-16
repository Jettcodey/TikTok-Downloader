using System.Text.Json;
using System.Windows.Forms;
using PuppeteerSharp;

namespace TikTok_Downloader
{
    public partial class MainForm : Form
    {
        private string? logFilePath;
        private string downloadFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TiktokDownloads");

        private List<string> cachedVideoUrls = new List<string>();

        public MainForm()
        {
            InitializeComponent();
            logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"TiktokDownloaderLog_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt");

            Directory.CreateDirectory(downloadFolderPath);

            LogMessage($"Initial download folder: {downloadFolderPath}");

            cmbChoice.SelectedItem = "Single Video/Image Download";

            outputTextBox.ReadOnly = true;
        }

        private void LogMessage(string message)
        {
            File.AppendAllText(logFilePath, $"{DateTime.Now}: {message}\n");
        }

        private void LogDownload(string fileName, string url)
        {
            LogMessage($"Downloaded file: {fileName}, from URL: {url}");
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

        class Video
        {
            public DownloadAddr download_addr { get; set; } = new DownloadAddr();
            public PlayAddr play_addr { get; set; } = new PlayAddr();
        }

        class DownloadAddr
        {
            public List<string> url_list { get; set; } = new List<string>();
        }

        class PlayAddr
        {
            public List<string> url_list { get; set; } = new List<string>();
        }

        private async void btnDownload_Click(object sender, EventArgs e)
        {
            string choice = cmbChoice.SelectedItem.ToString();

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
                await DownloadFromTextFile(filePath);
            }
        }

        private async Task MassDownloadByUsername()
        {
            try
            {
                string username = txtUsername.Text.Trim();
                string baseUrl = $"https://www.tiktok.com/@{username}";

                string chromeExecutablePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";  //<-- Change this path to ur preferred Webbrowser.

                var options = new LaunchOptions
                {
                    Headless = false,
                    ExecutablePath = chromeExecutablePath
                };

                var browser = await Puppeteer.LaunchAsync(options);
                var page = await browser.NewPageAsync();

                var navigationOptions = new NavigationOptions
                {
                    Timeout = 120000
                };

                // Navigate to the TikTok page
                await page.GoToAsync(baseUrl, navigationOptions);

                // Scroll to the bottom of the page repeatedly until no more videos are loaded
                while (true)
                {
                    long initialHeight = await page.EvaluateFunctionAsync<long>("() => document.body.scrollHeight");
                    await page.EvaluateExpressionAsync("window.scrollTo(0, document.body.scrollHeight)");
                    await page.WaitForTimeoutAsync(10000);
                    long newHeight = await page.EvaluateFunctionAsync<long>("() => document.body.scrollHeight");

                    if (newHeight == initialHeight)
                    {
                        break;
                    }
                }

                // Extract all video URLs
                var videoUrls = await page.EvaluateFunctionAsync<string[]>("() => { var videos = document.querySelectorAll('a'); var videoUrls = []; videos.forEach(video => { if (video.href.includes('/video/')) { videoUrls.push(video.href); } }); return videoUrls; }");

                await browser.CloseAsync();

                // Save all video links to a text file
                string videoLinksFilePath = Path.Combine(downloadFolderPath, $"{username}_video_links.txt");
                await File.WriteAllLinesAsync(videoLinksFilePath, videoUrls);

                // Download all videos from the text file
                List<string> videoLinksList = videoUrls.ToList();
                await DownloadVideos(videoLinksList);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine($"TimeoutException occurred: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
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
                }
            }
        }

        private async Task DownloadFromTextFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                outputTextBox.AppendText("Error: Text file not found!\r\n");
                return;
            }

            var urls = File.ReadAllLines(filePath);

            progressBar.Minimum = 0;
            progressBar.Maximum = urls.Length;
            progressBar.Value = 0;

            foreach (var url in urls)
            {
                var trimmedUrl = url.Trim();
                if (!string.IsNullOrWhiteSpace(trimmedUrl))
                {
                    outputTextBox.AppendText($"Downloading {trimmedUrl} ...\r\n");

                    var data = await GetVideo(trimmedUrl, withWatermarkCheckBox.Checked);

                    if (data == null)
                    {
                        outputTextBox.AppendText($"Error: Video from URL {trimmedUrl} was deleted!\r\n");
                        continue;
                    }

                    await DownloadMedia(data);
                }

                progressBar.Value++;
            }
        }

        private async Task DownloadVideos(List<string> videoUrls)
        {
            if (videoUrls.Count == 0)
            {
                MessageBox.Show("No videos found.");
                return;
            }

            string folder = (downloadFolderPath);
            Directory.CreateDirectory(folder);

            // Filter out duplicate and incorrect username links, bc filtering them before caused some weird Behavior in the Browser(at least in Google Chrome).
            HashSet<string> uniqueVideoUrls = new HashSet<string>();
            foreach (var url in videoUrls)
            {
                if (url.Contains($"@{txtUsername.Text}/video/") && !uniqueVideoUrls.Contains(url))
                {
                    uniqueVideoUrls.Add(url);
                }
            }

            progressBar.Minimum = 0;
            progressBar.Maximum = uniqueVideoUrls.Count;
            progressBar.Value = 0;

            foreach (string videoUrl in uniqueVideoUrls)
            {
                var videoData = await GetVideo(videoUrl, withWatermarkCheckBox.Checked);
                if (videoData != null)
                {
                    string downloadFolderPath = Path.Combine(folder, videoData.Id);
                    Directory.CreateDirectory(downloadFolderPath);

                    if (videoData.Images.Count > 0)
                    {
                        foreach (string imageUrl in videoData.Images)
                        {
                            string fileName = $"{videoData.Id}_{videoData.Images.IndexOf(imageUrl)}.jpeg";
                            string filePath = Path.Combine(downloadFolderPath, fileName);

                            if (File.Exists(filePath))
                            {
                                outputTextBox.AppendText($"File '{fileName}' already exists. Skipping\r\n");
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

                            outputTextBox.AppendText($"Downloading {videoUrl} ...\r\n");
                            LogDownload(fileName, imageUrl);
                        }
                    }
                    else
                    {
                        string fileName = $"{videoData.Id}";

                        if (withWatermarkCheckBox.Checked)
                        {
                            fileName += "_Watermark.mp4";
                        }
                        else
                        {
                            fileName += "_Save.mp4";
                        }

                        string filePath = Path.Combine(downloadFolderPath, fileName);

                        if (File.Exists(filePath))
                        {
                            outputTextBox.AppendText($"File '{fileName}' already exists. Skipping\r\n");
                            continue;
                        }

                        using (var client = new HttpClient())
                        {
                            using (var stream = await client.GetStreamAsync(videoData.Url))
                            using (var fileStream = File.Create(filePath))
                            {
                                await stream.CopyToAsync(fileStream);
                            }
                        }

                        outputTextBox.AppendText($"Downloading {videoUrl} ...\r\n");
                        LogDownload(fileName, videoData.Url);
                    }
                }

                progressBar.Value++;
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
                outputTextBox.AppendText($"Downloading {trimmedUrl} ...\r\n");

                var data = await GetVideo(trimmedUrl, withWatermarkCheckBox.Checked);

                if (data == null)
                {
                    outputTextBox.AppendText($"Error: Video from URL {trimmedUrl} was deleted!\r\n");
                    return;
                }

                await DownloadMedia(data);
            }
            catch (Exception ex)
            {
                outputTextBox.AppendText($"An error occurred: {ex.Message}\r\n");
            }
        }

        private async Task<VideoData?> GetVideo(string url, bool withWatermark)
        {
            var idVideo = await GetIdVideo(url);
            var apiUrl = $"https://api22-normal-c-alisg.tiktokv.com/aweme/v1/feed/?aweme_id={idVideo}&iid=7318518857994389254&device_id=7318517321748022790&channel=googleplay&app_name=musical_ly&version_code=300904&device_platform=android&device_type=ASUS_Z01QD&version=9";
            //I stumbled upon the API URL by accident while scrolling through Reddit; however, it took me 2 - 3 days to fully understand how to work with it.

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<ApiData>(json);

                if (data?.aweme_list == null || data.aweme_list.Count == 0)
                {
                    return null;
                }

                var video = data.aweme_list.FirstOrDefault();
                var urlMedia = withWatermark ? video?.video?.download_addr?.url_list.FirstOrDefault() : video?.video?.play_addr?.url_list.FirstOrDefault();
                var imageUrls = video?.image_post_info?.images?.Select(img => img.display_image.url_list.FirstOrDefault()).ToList();

                return new VideoData
                {
                    Url = urlMedia ?? string.Empty,
                    Images = imageUrls ?? new List<string>(),
                    Id = idVideo
                };
            }
        }

        private async Task<string> GetIdVideo(string url)
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
                throw new Exception("Error: URL not found");
            }

            if (endIndex > url.Length || startIndex < 0 || endIndex < startIndex)
            {
                throw new Exception("Error: Invalid URL format");
            }

            var idVideo = url.Substring(startIndex, endIndex - startIndex);

            return idVideo;
        }

        private async Task DownloadMedia(VideoData data)
        {

            if (data.Images.Count > 0)
            {
                outputTextBox.AppendText("Downloading Image(s)\r\n");

                for (int i = 0; i < data.Images.Count; i++)
                {
                    var fileName = $"{data.Id}_{i}.jpeg";
                    var filePath = Path.Combine(downloadFolderPath, fileName);

                    if (File.Exists(filePath))
                    {
                        outputTextBox.AppendText($"File '{fileName}' already exists. Skipping\r\n");
                        continue;
                    }

                    using (var client = new HttpClient())
                    {
                        using (var stream = await client.GetStreamAsync(data.Images[i]))
                        using (var fileStream = File.Create(filePath))
                        {
                            await stream.CopyToAsync(fileStream);
                        }
                    }

                    outputTextBox.AppendText("Downloaded successfully\r\n");
                    LogDownload(fileName, data.Images[i]);
                }
            }
            else
            {
                var fileName = $"{data.Id}";

                if (withWatermarkCheckBox.Checked)
                {
                    fileName += "_Watermark.mp4";
                }
                else
                {
                    fileName += "_Save.mp4";
                }

                var filePath = Path.Combine(downloadFolderPath, fileName);

                if (File.Exists(filePath))
                {
                    outputTextBox.AppendText($"File '{fileName}' already exists. Skipping\r\n");
                    return;
                }

                using (var client = new HttpClient())
                {
                    using (var stream = await client.GetStreamAsync(data.Url))
                    using (var fileStream = File.Create(filePath))
                    {
                        await stream.CopyToAsync(fileStream);
                    }
                }

                outputTextBox.AppendText("Downloaded successfully\r\n");
                LogDownload(fileName, data.Url);
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
                    LogMessage($"Changed download folder: {downloadFolderPath}");
                }
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
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
