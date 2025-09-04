<!--
##########################################
#           TikTok Downloader            #
#           Made by Jettcodey            #
#                © 2024                  #
#           DO NOT REMOVE THIS           #
##########################################
-->
# TikTok Downloader
<!-- # ⚠ THE APPLICATION ISN'T WORKING RIGHT NOW DUE TO RECENT TIKTOK API UPDATES ⚠ -->

<!-- # ⚠ The Update Server is currently down for Maintenance. I´m working hard to bring it back online. ⚠ -->

<!-- > [!NOTE]
> Some features of the application are not always working 100% right now due to changes in the TikTok API. -->

<!-- > [!NOTE]
> The application is currently undergoing refactoring and a small rewrite. -->

> [!NOTE] 
> As of [Release v1.3.9](https://github.com/Jettcodey/TikTok-Downloader/releases/tag/Release-1.3.9), this project is not being actively maintained.\
While I **may** answer questions, updates of any kind (new features, bug fixes, etc.) will not be made until I return to the project.

<!-- TABLE OF CONTENTS -->
<summary>Table of Contents</summary>
 <ol>
   <li><a href="#-about-the-project">About The Project</a></li>
   <li><a href="#-features">Features</a></li>
   <li><a href="#%EF%B8%8F-installation">Installation</a></li>
   <li><a href="#-usage">Usage</a></li>
   <li><a href="#-contribute">Contribute</a></li>
   <li><a href="#-report-a-bug--request-a-feature">Report a Bug/Request a Feature</a></li>
 </ol>

### Built With
C# and Microsoft Playwright

<img src="https://github.com/Jettcodey/TikTok-Downloader/assets/163922510/aca578ae-4c24-490f-96f2-4c19a16fe9e6" width="48" height="48">
<img src="https://github.com/Jettcodey/TikTok-Downloader/assets/163922510/e36d2e7e-689f-4927-aadb-42b8a7d1de2d" width="48" height="48">

<!--![csharpIcon](https://github.com/Jettcodey/TikTok-Downloader/assets/163922510/aca578ae-4c24-490f-96f2-4c19a16fe9e6)
![Playwright](https://github.com/Jettcodey/TikTok-Downloader/assets/163922510/e36d2e7e-689f-4927-aadb-42b8a7d1de2d)-->

<p align="right"><a href="#tiktok-downloader">Back to top</a></p>

<!-- ABOUT THE PROJECT -->
## 📁 About The Project

**TikTok Downloader** is a Simple application that allows you to:

- **Download individual Video or Image Posts**
- **Download Videos in HD Quality**
- **Mass Download Content** using Links from a Text file or by Username

### 💾 App Information
- **Install Location**: TikTok Downloader will be Installed at `C:\Program Files\Jettcodey\TikTok Downloader\TikTok Downloader`
- **Default Location**: Downloads are saved in a folder on your desktop named `TikTokDownloads\{@Username}\`, organized into **Videos**, **Images**, and **Avatars**.
- **Settings File**: Configuration is stored in the user's Documents folder at `Jettcodey\TikTok Downloader\appsettings.xml`.

### ❓ Questions?  
If you have any questions, feel free to [open an issue](https://github.com/Jettcodey/TikTok-Downloader/issues) or reach out to me on Discord: **`jettcodey`**.  
Please note that it may take me 1-2 days to respond—I'm just one person with a life outside of coding.

<p align="right"><a href="#tiktok-downloader">Back to top</a></p>

<!-- Features -->
## 🚀 Features

- ✔ **Download Single Video & Images**
- ✔ **Download Videos With or Without Watermark** **\***
- ✔ **Download Images Without Watermark**
- ✔ **Download Videos & Images from Links in a Text file**
- ✔ **Download Videos in HD Quality** (Third-Party API)
- ✔ **Download Profile Avatars**
- ✔ **Load & Download all Videos & Images from a User**
- ✔ **Mobile Links Support** ('vm.tiktok.com')
- ✔ **System Default Browser Support**
- ✔ **Built-in App-Updater**

### 🌐 **Multi-browser support**:
  - ✔ **Google Chrome**
  - ✔ **Microsoft Edge**
  - ✔ **Chromium**
  - ✔ **Brave**
  - ✔ **Mozilla Firefox** **\*\***

**\*** Downloading Videos With a Watermark is **not** working 100% of the time due to changes in the TikTok API.

**\*\*** Firefox support is only available using the Microsoft Playwright Firefox build ([More info](https://playwright.dev/dotnet/docs/browsers#firefox)).

<p align="right"><a href="#tiktok-downloader">Back to top</a></p>

<!-- Installation -->
## 🛠️ Installation

**Download the Latest Setup File**  
   Get the latest version of TikTok Downloader from the [Releases page](https://github.com/Jettcodey/TikTok-Downloader/releases/latest).

**Install .NET 8.0 Desktop Runtime (if needed)**  
   If the installer fails, you may need to install the [.NET 8.0 Desktop Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) manually.

>.NET 8.0 Desktop Runtime is required for TikTok Downloader to run.

<p align="right"><a href="#tiktok-downloader">Back to top</a></p>

<!-- Usage -->
## 📖 Usage

After installing TikTok Downloader, open the application and complete the First-Time Setup. You will then have the following options:

- **Single Video/Image Download**
- **Mass Download by Username**
- **Mass Download from Text File Links**
- **HD Download Video/Image**
- **HD Download From Text File Links**
- **HD Download By Username**

### 🛠️ How to Use Each Option:

#### 1. Single Video/Image Download
- Select this option, copy & paste the TikTok link* into the input field, and click **Download**.

#### 2. Mass Download by Username
- Choose this option, and copy & paste the TikTok profile link** or username into the text field.  
- Click **Download**, and your default browser will open. 
- Solve a simple puzzle to verify you're human, and accept/deny cookies (these will be deleted post-download). 
- When prompted with a Login Popup, select **"Continue as Guest"**. 
- The browser will scroll through the profile, saving links to a text file (`Username_combined_links.txt`) in the default storage location. 
- The downloader will then retrieve all video/image posts from the saved links and begin Downloading.

#### 3. Mass Download from Text File Links
- Create a `.txt` file with each TikTok link* on a new line. 
- Select the **"Browse"** button to locate and choose your text file. 
- Click **Download** to start the process.

#### 4. The HD Variants
The HD Variants are the Same but download **Videos** and **Images** (if Possible) in HD quality.

\* TikTok Links format: `https://www.tiktok.com/@user/video|photo/123456789` or `https://vm|vt.tiktok.com/a1b2c3d4`.  
**TikTok Profile Links format: `https://www.tiktok.com/@user`.

<p align="right"><a href="#tiktok-downloader">Back to top</a></p>

<!-- Contribute -->
## 🤝 Contribute

We welcome contributions from everyone! If you’re interested in helping improve TikTok Downloader, feel free to get involved.

<p align="right"><a href="#tiktok-downloader">Back to top</a></p>

<!-- Report a bug -->
## 🐞 Report a Bug / Request a Feature

Have you encountered a bug or have a feature in mind? Check out the [open issues](https://github.com/Jettcodey/TikTok-Downloader/issues) for a complete list of proposed features and known issues.

### **Important:**  
Please report bugs and issues in **English**. Submissions in other languages may be ignored and closed.

<p align="right"><a href="#tiktok-downloader">Back to top</a></p>
