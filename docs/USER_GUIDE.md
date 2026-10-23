# SBPS User Guide

## 1. Select a Folder

Type a full path into the folder field and press **Enter**, select **Choose Folder**, or drop a folder onto the window. Enable **Subfolders** before refreshing when files below the selected directory should also be discovered. The last folder that was loaded successfully opens again next time.

## 2. Filter File Types

SBPS creates filter choices from supported extensions actually present in the folder. PDF is initially selected when found; otherwise the available types are selected so the list is not empty.

Clearing a file type keeps those rows on screen and turns off their selection. Checking the type selects them again. The list itself is not rebuilt.

## 3. Refine the Queue

Use the checkbox in each row to include or exclude that file. The summary area updates immediately:

- **Total**: rows currently listed
- **Selected**: rows checked by the user
- **To print**: selected rows that still exist and have a supported extension
- **Known pages**: reliable content-page counts from printable rows; unknown rows are shown separately
- **Separators**: planned blank pages when the option is enabled

## 4. Understand Page Counts

- A number is shown when SBPS can determine the count reliably.
- **Determined at print time** is used for content whose pagination depends on the renderer and printer settings.
- **Unavailable** is used when a document cannot be inspected safely.

Page count does not equal physical sheet count. Copies, duplex printing, driver behavior, and separator jobs change the final output.

## 5. Choose Print Options

Select an installed printer. **Two-sided** and **Add blank pages between files** are enabled by default.

Enable **Show settings first** when Windows printer settings should be displayed before the batch confirmation. This option starts disabled.

When separators are enabled, SBPS inserts one blank job only between printable files. It never inserts a separator before the first or after the last file.

## 6. Submit the Queue

Select **Send to Printer**. The confirmation dialog shows the printable-file count, printer, two-sided state, copy count, and separator state.

After confirmation, progress and row statuses are updated in order. **Skip failed files** starts on. While it is on, a file that fails after the built-in retries is marked as an error, written to the diagnostic log, and the queue continues. Turn it off when a failure should stop the remaining queue. The two modes cannot be active together. Use **Stop** to request cancellation.

**Tips**, next to the language selector, starts off. Turn it on to show Turkish or English guidance for the folder, printer, and queue controls. Turn it off to hide that guidance.

## 7. Language, Help, and Exit

The initial language follows Windows culture settings. Use the language selector to switch between Turkish and English at runtime.

Select **Help** to open the sectioned guide: folder selection, page count, print statistics, separator pages, sending to the printer, and supported formats. **Close** or Esc dismisses it.

Select **Exit** to close the application. If printing is still running, SBPS asks for confirmation before stopping the remaining jobs.

## Defaults and Persistence

The current build starts with two-sided printing, separator pages, and **Skip failed files** enabled. **Show settings first** and **Tips** start disabled. Those choices are not persisted. The last successfully loaded folder is stored in `%LocalAppData%\FilePrintStudio\last-folder.txt`. The WebView2 cache remains `%LocalAppData%\FilePrintStudio\WebView2`.
