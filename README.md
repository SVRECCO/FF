# FF

<p align="center">
  <img src="https://techstarwebsolutions.com/images/FF.png" alt="FF Logo" width="200"/>
</p>

<p align="center">
  <a href="https://github.com/SVRECCO/FF/releases">
    <img src="https://img.shields.io/github/v/release/SVRECCO/FF?include_prereleases&style=flat-square" alt="GitHub release">
  </a>
  <a href="https://github.com/SVRECCO/FF/blob/main/LICENSE">
    <img src="https://img.shields.io/github/license/SVRECCO/FF?style=flat-square" alt="License">
  </a>
  <a href="https://github.com/SVRECCO/FF/stargazers">
    <img src="https://img.shields.io/github/stars/SVRECCO/FF?style=flat-square" alt="Stars">
  </a>
  <a href="https://github.com/SVRECCO/FF/network/members">
    <img src="https://img.shields.io/github/forks/SVRECCO/FF?style=flat-square" alt="Forks">
  </a>
  <a href="https://github.com/SVRECCO/FF/issues">
    <img src="https://img.shields.io/github/issues/SVRECCO/FF?style=flat-square" alt="Issues">
  </a>
  <a href="https://dotnet.microsoft.com/">
    <img src="https://img.shields.io/badge/.NET-%3E%3D9.0-blueviolet?style=flat-square" alt=".NET Version">
  </a>
</p>

<p align="center">
  <img src="docs/screenshots/preview.png" alt="FF Preview" width="600"/>
</p>

## About FF

FF is a visually appealing, feature-rich system information tool for Windows, inspired by [neofetch](https://github.com/dylanaraps/neofetch). Display your system specs with style in your terminal.

## Features

- 🖥️ **Beautiful Display**: Rich visual presentation of system information with Spectre.Console
- 🎨 **Custom Image Support**: Use your own background image for a personalized experience
- 🚀 **Parallel Processing**: Multi-threaded data collection for optimal performance 
- 🔍 **Comprehensive System Information**: 
  - CPU, GPU, RAM specs
  - Host information
  - Windows theme detection
  - Disk usage
  - Terminal info
  - Package count
  - And more!
- 🌈 **ANSI Color Support**: Vibrant, colorful output for modern terminals

## Installation

### Prerequisites

- .NET 6.0 SDK or newer
- Windows 10/11

### Option 1: Download Release

1. Download the latest release from the [Releases page](https://github.com/SVRECCO/FF/releases)
2. Extract the zip file
3. Run `FF.exe` from a command prompt or PowerShell

### Option 2: Build from Source

```powershell
# Clone the repository
git clone https://github.com/SVRECCO/FF.git
cd FF

# Build the project
dotnet build -c Release

# Run the application
.\bin\Release\net6.0\FF.exe
```

## Custom Image Setup

1. Create a directory at `C:\ff\image\`
2. Place your desired background image as `bg.jpg` in this directory

If no image is found, FF will display the default Windows ASCII logo.

## Usage

```
ff [OPTIONS]

Options:
  -h, --help           Show help information
  -v, --version        Show version information
```

## Example Output

```
username@DESKTOP
------------------
OS: Microsoft Windows 10.0.19042
Host: Dell Inc. XPS 15 9500
Kernel: 10.0.19042
Uptime: 4d 12h 35m
Packages: 342
Shell: C:\WINDOWS\system32\cmd.exe
Resolution: 1920x1080
DE: Windows Explorer
WM Theme: Dark
Terminal: Windows Terminal
Terminal Font: Cascadia Code
CPU: Intel(R) Core(TM) i7-10750H CPU @ 2.60GHz
GPU: NVIDIA GeForce RTX 2060
Memory: 32 GB
Disk: C: 953 GB, D: 953 GB
```

## Planned Features

- [ ] Command-line options for customization
- [ ] Support for custom ASCII art
- [ ] Export options (PNG, JSON)
- [ ] Config file for persistent settings
- [ ] Color theme customization

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Inspired by [neofetch](https://github.com/dylanaraps/neofetch)
- Built with [Spectre.Console](https://spectreconsole.net/) for terminal UI
- Uses [ImageSharp](https://sixlabors.com/products/imagesharp/) for image processing

---

<p align="center">
  Made with ❤️ by <a href="https://github.com/SVRECCO">SethV</a>
</p>
