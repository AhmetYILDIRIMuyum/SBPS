<div align="center">

# SBPS — Smart Batch Printing Studio

**A Windows desktop application for organizing and batch-printing supported files.**

[Türkçe](README.tr.md) · [Installation](docs/INSTALLATION.md) · [User Guide](docs/USER_GUIDE.md) · [Troubleshooting](docs/TROUBLESHOOTING.md)

</div>

## Overview

SBPS turns a folder containing mixed document types into a controlled print queue. Users can filter by the file types found in the folder, include or exclude individual files, choose an installed Windows printer, review page-count information where it can be determined safely, and send the resulting queue in file-name order.

The application is designed for Windows operators who need repeatable multi-file printing without opening each file manually. It keeps each source file as a separate print job and can insert an optional blank separator page between consecutive files.

## Key Features

- Manual folder-path entry or Windows folder selection, with optional subfolder scanning
- Dynamic file-type filters generated from the selected folder
- Per-file selection in the print queue
- Real-time total, selected, and printable file counts
- Per-file page-count information without opening a print job
- Known content-page and planned separator-page summaries
- Installed-printer selection with printer-list refresh
- Two-sided printing enabled by default when supported by the selected printer
- Optional Windows printer settings dialog before submission
- Optional blank separator pages between files; enabled by default
- Ordered, cancellable processing with per-file status and diagnostic logging
- Turkish and English UI selected from Windows regional settings and changeable at runtime
- High-DPI-aware Windows Forms interface and a sectioned in-app help window
- On-screen Exit button; closing during an active print asks for confirmation

## Supported File Formats

| Category | Extensions | Page-count display |
|---|---|---|
| PDF | `.pdf` | Read from the document when safely detectable; otherwise unavailable |
| Images | `.jpg`, `.jpeg`, `.png`, `.bmp`, `.gif`, `.webp`, `.svg` | 1 page, matching the current print flow |
| Text | `.txt`, `.csv`, `.xml` | Determined at print time |
| Web | `.html`, `.htm` | Determined at print time |

SBPS does not claim native DOCX or XLSX support.

## Screenshots

![SBPS main window](docs/images/main-window.png)

## Getting Started

### Requirements

- Windows 10 or Windows 11
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) for development
- Visual Studio 2022 17.12 or later with the .NET desktop development workload, or the .NET CLI
- Microsoft Edge WebView2 Runtime
- At least one installed Windows printer for printing

### Build and run

```powershell
git clone https://github.com/AhmetYILDIRIMuyum/SBPS.git
cd SBPS
dotnet restore .\SBPS.sln
dotnet build .\SBPS.sln -c Debug
dotnet run --project .\src\SBPS\SBPS.csproj
```

For a Release build:

```powershell
dotnet build .\SBPS.sln -c Release
```

See [Installation](docs/INSTALLATION.md) for development and runtime details.

## How to Use

1. Enter a folder path or choose a folder.
2. Optionally include subfolders, then refresh the list.
3. Select the file types to include. PDF is selected initially when it exists.
4. Use the per-file checkboxes to refine the queue.
5. Review page-count and queue statistics.
6. Choose a printer and confirm two-sided, separator-page, and advanced-settings options.
7. Select **Send to Printer** and approve the confirmation dialog.
8. Select **Help** for the sectioned guide, or **Exit** to close the application.

No separator is added before the first file or after the last file. A queue of *N* files creates at most *N − 1* separator pages.

## Technology Stack

- C# and .NET 9
- Windows Forms
- Microsoft Edge WebView2 for document rendering and printing
- Windows printing APIs through `System.Drawing.Printing`
- Windows Property System as a best-effort PDF page-count fallback

The only NuGet dependency is `Microsoft.Web.WebView2`.

## Project Structure

```text
SBPS.sln
src/
  SBPS/
    SBPS.csproj
    MainForm.cs
    FilePageCountService.cs
    PdfFileItem.cs
    Localization.cs
    HelpDialog.cs
    PremiumControls.cs
    Program.cs
    app.manifest
docs/
  ARCHITECTURE.md
  INSTALLATION.md
  USER_GUIDE.md
  TROUBLESHOOTING.md
```

## Configuration and Data

SBPS reads installed printers from Windows. Printer capabilities and final duplex behavior are controlled by the selected printer and its driver.

The current build does not persist UI choices between sessions. The separator option starts enabled, two-sided printing starts enabled, and **Show settings first** starts disabled. Diagnostic logs continue to use `%LocalAppData%\FilePrintStudio\Logs` to preserve compatibility with earlier installations.

## Build and Development

The solution targets `net9.0-windows`. Build artifacts under `bin/`, `obj/`, `.vs/`, and `publish/` are intentionally excluded from source control.

There is currently no automated test project. Changes should be validated with Debug and Release builds plus non-printing startup and UI checks. Do not send physical test pages without an operator's explicit approval.

## Troubleshooting

- If WebView2 cannot initialize, install or repair the Microsoft Edge WebView2 Evergreen Runtime.
- If a printer is missing, verify it in Windows Settings and use **Refresh Printers**.
- If a PDF page count is unavailable, the file may be encrypted, damaged, unusually encoded, or outside the safe inspection limit. Printing remains a separate operation.
- If printing fails, SBPS stops the remaining queue to preserve order and writes a diagnostic log.

See [Troubleshooting](docs/TROUBLESHOOTING.md) for more details.

## Limitations

- Page count is not a guarantee of physical sheet count; copies, duplex mode, printer-driver behavior, and separator jobs affect output.
- Text and HTML pagination depends on the print engine, paper, margins, and printer settings, so it is reported only at print time.
- Some compressed, encrypted, malformed, or very large PDFs may show an unavailable page count.
- Separate print jobs cannot guarantee how every printer driver handles duplex boundaries or blank separator sheets.
- Windows and Microsoft Edge WebView2 Runtime are required.

## License

The source is publicly visible but is **not open source**. No permission to use, copy, modify, redistribute, sublicense, or sell the software is granted without prior written authorization. See [LICENSE](LICENSE).

## Author

**Ahmet YILDIRIM [dede]**  
[GitHub](https://github.com/AhmetYILDIRIMuyum) · [ahmetyildirimuyum@gmail.com](mailto:ahmetyildirimuyum@gmail.com)

## Contributing

No public contribution policy has been established. Open an issue or contact the author before preparing a contribution.

## Project Status

SBPS is under active development. Debug and Release build results are documented when changes are published; no production-readiness or release-support guarantee is implied.
