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
C# and PuppeteerSharp

<img src="https://github.com/Jettcodey/TikTok-Downloader/assets/163922510/aca578ae-4c24-490f-96f2-4c19a16fe9e6" width="64" height="64">
<img src="https://github.com/Jettcodey/TikTok-Downloader/assets/163922510/08a6f2f6-ecff-41fa-8f9e-4361ad178902" width="64" height="64">

<!--![csharpIcon](https://github.com/Jettcodey/TikTok-Downloader/assets/163922510/aca578ae-4c24-490f-96f2-4c19a16fe9e6)
![logo](https://github.com/Jettcodey/TikTok-Downloader/assets/163922510/08a6f2f6-ecff-41fa-8f9e-4361ad178902)-->


<p align="right"><a href="#readme-top">Back to top</a></p>

<!-- ABOUT THE PROJECT -->
## About The Project
### For educational purposes only!

Just a simple TikTok Downloader capable of downloading single videos/images, Mass Downloading from Links in a Text file or Mass downloading videos from a user profile.

Completed downloaded Single videos/photos or from Links in a Text file are stored individually in the Download Directory.

When downloading all videos from a user, the files are saved into a folder with the video ID, one folder per video ID. The default storage location is the user's desktop in a folder named "TikTokDownloads".

To download all videos from a TikTok user, the Application opens the specified TikTok user in the browser. Then, loads all video links of the user by Auto scrolling through the browser. After that, those Links get saved in a separate text file named "{@username}_video_links.txt" in the TikTokDowloads Folder. Finally the Application works through the Saved links.

### Important: 
To make the TikTok User download work, Google Chrome must be installed since I haven't had the chance to make the app use the system default browser yet, Sry Firefox users.

<p align="right"><a href="#readme-top">Back to top</a></p>

<!-- Installation -->
## Installation

To install TikTok Downloader, Download the Latest Setup file from [Releases](https://github.com/Jettcodey/TikTok-Downloader/releases/latest).

Also make sure u have [Microsoft .Net 6.0 Desktop Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) Installed.

<p align="right"><a href="#readme-top">Back to top</a></p>

<!-- ROADMAP -->
## Roadmap

- [x] Download Single Videos/Images.
- [x] Download Videos With or Without Watermark.
- [x] Download Images Without Watermark.
- [x] Download Videos & Images from Links in a Text File.
- [x] Download All Videos from a User-Profile.
- [ ] Download All Images from a User-Profile Without Watermark.
- [ ] Make the GUI Better.
- [ ] Multi-Webbrowser Support(Systems Default).
- [ ] Multi-OS Support
  - [x] Windows Support
  - [ ] Linux Support 
  - [ ] MacOS Support <-- Currently working on


See the [open issues](https://github.com/Jettcodey/TikTok-Downloader/issues) for a full list of proposed features (and known issues).

<p align="right"><a href="#readme-top">Back to top</a></p>