# SBPS Architecture

## Runtime Model

SBPS is a single-project .NET 9 Windows Forms application. The main form owns the UI state, file discovery, print-queue orchestration, WebView2 navigation, Windows printer selection, and cancellation lifecycle.

## Main Components

| Component | Responsibility |
|---|---|
| `MainForm` | UI construction, folder scanning, filters, queue selection, printer options, status, and ordered print orchestration |
| `FilePageCountService` | Asynchronous page-count lookup and cache |
| `PdfFileItem` | Bindable row model for selection, path, size, state, and page-count display |
| `Localization` | Turkish/English strings and culture-based initial language selection |
| `HelpDialog` | Fixed, DPI-aware in-app help window with six sections and no scrollbar |
| `PremiumControls` | Custom colors, cards, buttons, header, and progress control |

## File Discovery and Queue

The selected folder is enumerated for a fixed set of supported extensions. Files outside that set stay out of the queue, and the status line reports how many were listed and how many were excluded. The application creates dynamic type filters from the discovered files. The visible list is sorted by file name and each row can be included or excluded independently. A folder can be chosen by typing, by the folder dialog, or by dropping it on the window. The last loaded folder is restored from `%LocalAppData%\FilePrintStudio\last-folder.txt`.

`BuildPrintQueue()` is the shared source of truth for the printable-file summary and the actual print operation. It includes selected rows whose files still exist and whose extensions remain supported.

## Page Count

Page counts are loaded asynchronously with a maximum of four concurrent operations. Changing the folder or filters cancels the prior batch. Results are cached by full path, file length, and last-write time.

- Images are reported as one page because the current renderer submits each image as one document.
- Text and web content are reported as determined at print time.
- PDF inspection reads the file with shared access, checks for encryption, examines page-tree counts, and falls back to direct page nodes when safe.
- Windows Property System `System.Document.PageCount` is used as a best-effort fallback.
- Unsupported, encrypted, damaged, oversized, or unrecognized PDFs return an unavailable state without terminating the application.

The parser is intentionally conservative. It does not claim exact results for PDF structures it cannot verify safely.

## Printing

WebView2 renders supported files and submits each file as a separate print job. Text-like files are HTML-encoded and wrapped in a controlled printable document. If enabled, a minimal white HTML page is submitted between consecutive queue entries.

**Skip failed files** starts on. A failed file is logged and the queue continues. Turn the option off when the queue should stop after an error so later files are not sent out of order. Retry behavior is retained for transient print failures. The user can request cancellation; the current operation completes or returns before the queue stops. Optional control tips follow the active language and appear only while **Tips** is on.

## Compatibility

The assembly, root namespace, executable, project, and solution are named `SBPS`. The legacy `%LocalAppData%\FilePrintStudio` data path is intentionally retained for diagnostic-log and WebView2 profile continuity.

## Dependencies

- .NET 9 Windows Desktop runtime
- Microsoft Edge WebView2 Runtime
- NuGet: `Microsoft.Web.WebView2` 1.0.4258.31
- Windows printer drivers and Windows Property System
