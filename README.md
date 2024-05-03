# TikTok Downloader
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
    <li><a href="#installation">Installation</a></li>
    <li><a href="#usage">Usage</a></li>
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

The default storage location (if not Changed in the Menu) is the user's desktop in a folder named "TikTokDownloads\{@Username}\Videos\ & \Images\ folders".

To download all videos from a TikTok user, the Application opens the specified TikTok user in the browser. Then, loads all Video/Image links of the user by Auto scrolling through the browser. After that, those Links get saved in a separate text file named "{@username}_combined_links.txt" in the default storage location. Finally the Application works through the Saved links.

The settings file is saved in the user's Documents folder at "Jettcodey\TikTok Downloader\appsettings.xml". The Settings Menu is still in development, so there aren't many options to choose from yet.
### Important: 
To make the TikTok User download work, you need to have a Chromium-based browser installed, because others like Firefox and Opera GX don't work for some reason, I'm still investigating.                                       

<p align="right"><a href="#readme-top">Back to top</a></p>

<!-- Installation -->
## Installation

To install TikTok Downloader, Download the Latest Setup file from [Releases](https://github.com/Jettcodey/TikTok-Downloader/releases/latest).

If the Installer fails to Install it, u need to Install [Microsoft .Net 8.0 Desktop Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) manually.

<p align="right"><a href="#readme-top">Back to top</a></p>

<!-- Usage -->
## Usage

After the installation is finished, simply open the TikTok Downloader, and you will be able to select from the following options:

- Single Video/Image Download
- Mass Download by Username
- Mass Download from Text File Links

### How to use those options:

#### Single Video/Image Download:
If you select "Single Video/Image Download," you can simply copy and paste every TikTok browser link and press Download. The downloaded media is saved to the default storage location. 
> TikTok browser links look like this: "https://www.tiktok.com/@tiktok/video/Numeric_ID", Video links from the TikTok App **Won´t Work**

#### Mass Download by Username:
If you select "Mass Download by Username," you need to type (or copy & paste) the username of the account (without the @) into the text field. After pressing download, your system's default browser will open, automatically navigating to the account's TikTok page. You'll then need to solve a simple puzzle slide at the beginning to verify that you are human and accept/deny cookies (which will be deleted after the download is finished). When the login window from TikTok shows up, just press "Continue as Guest." The browser will begin auto-scrolling the entire account page until it can't find any new posts. It will then filter and save any found links from the account's posts to a text file in the default storage location (`Username_combined_links.txt`). Once all links are saved to the text file, it will begin downloading all video/image posts from those saved links. The downloaded media is saved to the default storage location.

#### Mass Download from Text File Links:
If you select "Mass Download from Text File Links," you just need (if not already done) to create a `.txt` file anywhere on your PC in which you put all TikTok browser links. Every link needs to be on its own line. After that, simply select the "Browse" button and navigate to your text file with the links in it. Once that's done, you can press Download. The downloaded media is saved to the default storage location.

<p align="right"><a href="#readme-top">Back to top</a></p>

<!-- ROADMAP -->
## Roadmap

- [x] Download Single Videos/Images.
- [x] Download Videos With or Without Watermark.
- [x] Download Images Without Watermark.
- [x] Download Videos & Images from Links in a Text File.
- [x] Download All Videos from a User-Profile.
- [x] Download All Images from a User-Profile Without Watermark.
- [x] System Default Browser Support <--(only Chromium-based are Currently supported)
- [ ] Multi-Webbrowser Support
    - [x] Google Chrome
    - [x] Microsoft Edge
    - [x] Chromium
    - [x] Brave <-- Not tested yet, but could work.
    - [ ] Mozilla Firefox <-- Doesn't work for some reason, still investigating.
    - [ ] Opera GX <-- <-- Doesn't work for some reason, still investigating.
- [ ] Make the GUI Better
    - [ ] Add more Options(Settings Menu)
    - [ ] Make the GUI more appealing/better looking



See the [open issues](https://github.com/Jettcodey/TikTok-Downloader/issues) for a full list of proposed features (and known issues).

<p align="right"><a href="#readme-top">Back to top</a></p>
