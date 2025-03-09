using Spectre.Console;
using System.Management;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Diagnostics;
using SixLabors.ImageSharp.Processing;

namespace FF
{
    class Program
    {
        static void Main(string[] args)
        {
            ParseCommandLineArguments(args);
            Console.Clear();
            var sysInfo = new SystemInfo();
            sysInfo.Display();
        }
        
        private static void ParseCommandLineArguments(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLower())
                {
                    case "-h":
                    case "--help":
                        DisplayHelp();
                        Environment.Exit(0);
                        break;
                    case "-v":
                    case "--version":
                        DisplayVersion();
                        Environment.Exit(0);
                        break;
                }
            }
        }
        
        private static void DisplayHelp()
        {
            AnsiConsole.Write(
                new FigletText("FF")
                    .LeftJustified()
                    .Color(Color.Blue));
            AnsiConsole.WriteLine("A Windows system information fetcher");
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine("Usage: ff [OPTIONS]");
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine("Options:");
            AnsiConsole.WriteLine("  -h, --help           Show help information");
            AnsiConsole.WriteLine("  -v, --version        Show version information");
        }
        
        private static void DisplayVersion()
        {
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            AnsiConsole.WriteLine($"FF v{version.Major}.{version.Minor}.{version.Build}");
        }
    }

    class SystemInfo
    {
        private readonly Dictionary<string, string> _cachedInfo;

        public SystemInfo()
        {
            _cachedInfo = new Dictionary<string, string>();

            Parallel.Invoke(
                () => _cachedInfo["cpu"] = GetCPUInfo(),
                () => _cachedInfo["gpu"] = GetGPUInfo(),
                () => _cachedInfo["ram"] = GetRAMInfo(),
                () => _cachedInfo["kernel"] = GetKernelInfo(),
                () => _cachedInfo["host"] = GetHostInfo(),
                () => _cachedInfo["packages"] = GetPackagesInfo(),
                () => _cachedInfo["wmtheme"] = GetWMTheme(),
                () => _cachedInfo["disk"] = GetDiskInfo(),
                () => _cachedInfo["uptime"] = GetUptime()
            );
        }

        public void Display()
        {
            var layout = new Layout("Root")
                .SplitColumns(
                    new Layout("Left"),
                    new Layout("Right")
                );

            layout["Left"].Size = 64;

            var imagePath = "C:\\ff\\image\\bg.jpg";
            if (File.Exists(imagePath))
            {
                var image = new CanvasImage(imagePath);
                image.MaxWidth = 120;
                image.Resampler = KnownResamplers.Triangle;

                var imagePanel = new Panel(image)
                {
                    Border = BoxBorder.None,
                    Padding = new Padding(2, 2, 0, 0)
                };
                
                layout["Left"].Update(imagePanel);
            }
            else
            {
                var asciiArt = GetWindowsAsciiArt();
                var asciiPanel = new Panel(new Markup($"[blue]{asciiArt}[/]"))
                {
                    Border = BoxBorder.None,
                    Padding = new Padding(2, 2, 0, 0)
                };
                
                layout["Left"].Update(asciiPanel);
            }

            var infoTable = new Table()
                .Border(TableBorder.None)
                .BorderStyle(new Style(Color.Blue))
                .AddColumns(
                    new TableColumn(string.Empty),
                    new TableColumn(string.Empty).LeftAligned()
                );

            infoTable.AddRow(
                new Markup($"[blue]{Environment.UserName}@{Environment.MachineName}[/]"),
                new Markup(string.Empty)
            );

            infoTable.AddRow(
                new Markup("[blue]------------------[/]"),
                new Markup(string.Empty)
            );

            infoTable.AddRow(new Markup("[cyan]OS[/]"), new Markup(RuntimeInformation.OSDescription));
            infoTable.AddRow(new Markup("[yellow]Host[/]"), new Markup(_cachedInfo["host"]));
            infoTable.AddRow(new Markup("[magenta]Kernel[/]"), new Markup(_cachedInfo["kernel"]));
            infoTable.AddRow(new Markup("[green]Uptime[/]"), new Markup(_cachedInfo["uptime"]));
            infoTable.AddRow(new Markup("[blue]Packages[/]"), new Markup(_cachedInfo["packages"]));
            infoTable.AddRow(new Markup("[red]Shell[/]"), new Markup(Environment.GetEnvironmentVariable("COMSPEC") ?? "Unknown"));
            infoTable.AddRow(new Markup("[cyan]Resolution[/]"), new Markup(GetScreenResolution()));
            infoTable.AddRow(new Markup("[yellow]DE[/]"), new Markup("Windows Explorer"));
            infoTable.AddRow(new Markup("[magenta]WM Theme[/]"), new Markup(_cachedInfo["wmtheme"]));
            infoTable.AddRow(new Markup("[green]Terminal[/]"), new Markup(GetTerminalInfo()));
            infoTable.AddRow(new Markup("[blue]Terminal Font[/]"), new Markup("Cascadia Code"));
            infoTable.AddRow(new Markup("[red]CPU[/]"), new Markup(_cachedInfo["cpu"]));
            infoTable.AddRow(new Markup("[cyan]GPU[/]"), new Markup(_cachedInfo["gpu"]));
            infoTable.AddRow(new Markup("[yellow]Memory[/]"), new Markup(_cachedInfo["ram"]));
            infoTable.AddRow(new Markup("[magenta]Disk[/]"), new Markup(_cachedInfo["disk"]));

            var infoPanel = new Panel(infoTable)
            {
                Border = BoxBorder.None,
                Padding = new Padding(4, 8, 0, 0)
            };

            layout["Right"].Update(infoPanel);

            AnsiConsole.Write(layout);
        }

        private string GetCPUInfo()
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("root\\CIMV2",
                    "SELECT Name FROM Win32_Processor");
                using var results = searcher.Get();
                return results.Cast<ManagementObject>()
                    .FirstOrDefault()?["Name"]?.ToString()?.Trim()
                    ?? "[red]Unknown CPU[/]";
            }
            catch
            {
                return "[red]CPU information unavailable[/]";
            }
        }

        private string GetRAMInfo()
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("root\\CIMV2",
                    "SELECT TotalPhysicalMemory FROM Win32_ComputerSystem");
                using var results = searcher.Get();
                var ram = results.Cast<ManagementObject>()
                    .FirstOrDefault()?["TotalPhysicalMemory"];
                if (ram != null)
                {
                    ulong totalMemory = Convert.ToUInt64(ram);
                    return $"{totalMemory / (1024 * 1024 * 1024)} GB";
                }
            }
            catch { }
            return "[red]RAM information unavailable[/]";
        }

        private string GetGPUInfo()
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("root\\CIMV2",
                    "SELECT Name FROM Win32_VideoController");
                using var results = searcher.Get();
                return results.Cast<ManagementObject>()
                    .FirstOrDefault()?["Name"]?.ToString()?.Trim()
                    ?? "[red]Unknown GPU[/]";
            }
            catch
            {
                return "[red]GPU information unavailable[/]";
            }
        }

        private string GetDiskInfo()
        {
            try
            {
                var drives = DriveInfo.GetDrives()
                    .Where(d => d.IsReady && d.DriveType == DriveType.Fixed)
                    .Select(d => $"{d.Name.Replace("\\", "")} {d.TotalSize / (1024 * 1024 * 1024)} GB");
                return string.Join(", ", drives);
            }
            catch
            {
                return "[red]Disk information unavailable[/]";
            }
        }

        private string GetUptime()
        {
            try
            {
                using var uptime = new PerformanceCounter("System", "System Up Time");
                uptime.NextValue();
                TimeSpan ts = TimeSpan.FromSeconds(uptime.NextValue());
                return $"{ts.Days}d {ts.Hours}h {ts.Minutes}m";
            }
            catch
            {
                return "[red]Uptime information unavailable[/]";
            }
        }

        private string GetHostInfo()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\OEMInformation");
                if (key != null)
                {
                    var manufacturer = key.GetValue("Manufacturer")?.ToString() ?? "";
                    var model = key.GetValue("Model")?.ToString() ?? "";
                    return $"{manufacturer} {model}".Trim();
                }
            }
            catch { }
            return "Unknown";
        }

        private string GetKernelInfo()
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("root\\CIMV2",
                    "SELECT Version FROM Win32_OperatingSystem");
                using var results = searcher.Get();
                return results.Cast<ManagementObject>()
                    .FirstOrDefault()?["Version"]?.ToString()
                    ?? "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }

        private string GetPackagesInfo()
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
                return key?.GetSubKeyNames().Length.ToString() ?? "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }

        private string GetWMTheme()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
                if (key?.GetValue("AppsUseLightTheme") is int theme)
                {
                    return theme == 0 ? "Dark" : "Light";
                }
            }
            catch { }
            return "Unknown";
        }
        
        private string GetScreenResolution()
        {
            try
            {
                return "Primary Display";
            }
            catch
            {
                return "Unknown";
            }
        }
        
        private string GetTerminalInfo()
        {
            var terminalEnv = Environment.GetEnvironmentVariable("WT_SESSION");
            if (!string.IsNullOrEmpty(terminalEnv))
            {
                return "Windows Terminal";
            }
            
            if (Environment.GetEnvironmentVariable("TERM_PROGRAM") == "vscode")
            {
                return "VS Code Terminal";
            }
            
            return "Windows Console";
        }
        
        private string GetWindowsAsciiArt()
        {
            return @"                                 ..,
                     ....,,:;+ccllll
        ...,,+:;  cllllllllllllllllll
  ,cclllllllllll  lllllllllllllllllll
  llllllllllllll  lllllllllllllllllll
  llllllllllllll  lllllllllllllllllll
  llllllllllllll  lllllllllllllllllll
  llllllllllllll  lllllllllllllllllll
  llllllllllllll  lllllllllllllllllll
                                      
  llllllllllllll  lllllllllllllllllll
  llllllllllllll  lllllllllllllllllll
  llllllllllllll  lllllllllllllllllll
  llllllllllllll  lllllllllllllllllll
  llllllllllllll  lllllllllllllllllll
  `'ccllllllllll  lllllllllllllllllll
         `' \\*::  :ccllllllllllllllll
                        ````''*::cll
                                 ``";
        }
    }
}