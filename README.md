# Tiktok Downloader
<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
    </li>
    <li><a href="#Installation">Installation</li>
    <li><a href="#roadmap">Roadmap</a></li>
  </ol>
</details>

### Built With
C# and Microsoft Playwright

<img src="https://github.com/Jettcodey/TikTok-Downloader/assets/163922510/aca578ae-4c24-490f-96f2-4c19a16fe9e6" width="64" height="64">
<img src="https://github.com/Jettcodey/TikTok-Downloader/assets/163922510/e36d2e7e-689f-4927-aadb-42b8a7d1de2d" width="64" height="64">

<!--![csharpIcon](https://github.com/Jettcodey/TikTok-Downloader/assets/163922510/aca578ae-4c24-490f-96f2-4c19a16fe9e6)
![Playwright](https://github.com/Jettcodey/TikTok-Downloader/assets/163922510/e36d2e7e-689f-4927-aadb-42b8a7d1de2d)-->


<p align="right"><a href="#readme-top">Back to top</a></p>

<!-- ABOUT THE PROJECT -->
## About The Project
### For educational purposes only!

Just a simple TikTok Downloader capable of downloading single videos/images, Mass Downloading from Links in a Text file or Mass downloading from a user profile.

Completed downloaded Single videos/photos or from Links in a Text file are stored individually in the Download Directory.

When downloading all videos from a user, The default storage location is the user's desktop in a folder named "TikTokDownloads\{@Username}\Videos&Images folders".

To download all videos from a TikTok user, the Application opens the specified TikTok user in the browser. Then, loads all video links of the user by Auto scrolling through the browser. After that, those Links get saved in a separate text file named "{@username}_video_links.txt" in the TikTokDowloads Folder. Finally the Application works through the Saved links.

### Important: 
To make the TikTok User download work, you need to have a Chromium-based browser installed, because others like Firefox and Opera GX don't work for some reason, I'm still investigating.                                       

<p align="right"><a href="#readme-top">Back to top</a></p>

<!-- Installation -->
## Installation

To install TikTok Downloader, Download the Latest Setup file from [Releases](https://github.com/Jettcodey/TikTok-Downloader/releases/latest).

If the Installer fails to Install it, u need to Install [Microsoft .Net 8.0 Desktop Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) manually.

<p align="right"><a href="#readme-top">Back to top</a></p>

<!-- ROADMAP -->
## Roadmap

- [x] Download Single Videos/Images.
- [x] Download Videos With or Without Watermark.
- [x] Download Images Without Watermark.
- [x] Download Videos & Images from Links in a Text File.
- [x] Download All Videos from a User-Profile.
- [x] Download All Images from a User-Profile Without Watermark.
- [x] System Default Browser Support
- [ ] Multi-Webbrowser Support
    - [x] Google Chrome
    - [x] Microsoft Edge <-- Does the Job but the Browser doesn't Fully close after the download is finished,
thats Microsofts fault.
    - [ ] Mozilla Firefox <-- Doesn't work for some reason, still investigating.
    - [ ] Opera GX <-- <-- Doesn't work for some reason, still investigating.
    - [x] Brave <-- Not tested yet, but could work.
    - [x] Chromium <-- Not tested yet, but should work.
- [ ] Make the GUI Better.



See the [open issues](https://github.com/Jettcodey/TikTok-Downloader/issues) for a full list of proposed features (and known issues).

<p align="right"><a href="#readme-top">Back to top</a></p>
