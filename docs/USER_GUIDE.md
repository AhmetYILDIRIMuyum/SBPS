# SBPS User Guide

## 1. Select a Folder

Type a full path into the folder field and press **Enter**, or select **Choose Folder**. Enable **Subfolders** before refreshing when files below the selected directory should also be discovered.

## 2. Filter File Types

SBPS creates filter choices from supported extensions actually present in the folder. PDF is initially selected when found; otherwise the available types are selected so the list is not empty.

Changing a type filter rebuilds the visible list and restarts background page-count work.

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

After confirmation, progress and row statuses are updated in order. If a file fails after the built-in retries, the remaining queue is not submitted. Use **Stop** to request cancellation.

## 7. Language, Help, and Exit

The initial language follows Windows culture settings. Use the language selector to switch between Turkish and English at runtime.

Select **Help** to open the sectioned guide: folder selection, page count, print statistics, separator pages, sending to the printer, and supported formats. **Close** or Esc dismisses it.

Select **Exit** to close the application. If printing is still running, SBPS asks for confirmation before stopping the remaining jobs.

## Defaults and Persistence

The current build starts with two-sided and separator options enabled and printer-settings display disabled. These UI choices are not persisted when the application closes.
