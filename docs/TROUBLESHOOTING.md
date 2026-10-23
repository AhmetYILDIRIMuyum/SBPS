# Troubleshooting

## WebView2 Runtime Is Missing

**Symptom:** The printing engine cannot initialize.

Install or repair the Microsoft Edge WebView2 Evergreen Runtime, then restart SBPS. The application requires WebView2 to render and print supported files.

## A Printer Is Missing or Invalid

Verify the printer in Windows Settings and confirm that its driver is installed. Select **Refresh Printers** in SBPS. Network or redirected printers may disappear when the corresponding connection is unavailable.

## Page Count Shows Unavailable

This does not necessarily mean that printing will fail. Page inspection is deliberately conservative. Common causes include:

- encrypted or damaged PDF
- compressed object structures that cannot be verified safely
- PDF larger than the inspection safety limit
- file permissions or a transient file lock
- missing Windows metadata fallback

Text and HTML files correctly show that pagination is determined at print time.

## Printing Stops on an Error

SBPS stops the remaining queue after a failed item to preserve submission order. Confirm:

1. The printer is online and not paused.
2. Paper and consumables are available.
3. The selected file still exists and opens normally.
4. WebView2 Runtime and the printer driver are current.

Diagnostic logs are written under:

```text
%LocalAppData%\FilePrintStudio\Logs
```

The legacy folder name is retained intentionally for upgrade compatibility.

## Duplex or Separator Output Is Unexpected

Each document and separator is submitted as a separate print job. Printer drivers can reset duplex state at job boundaries or suppress apparently blank pages. Review the printer's own defaults and test with non-sensitive documents under operator supervision.

## The Window Appears Too Small or Crowded

SBPS opens at 1368 × 882 logical pixels and uses Windows DPI scaling. Check Windows display scaling and resolution. The form has a minimum size of 1000 × 720 and can be enlarged.

## Collecting a Useful Error Report

Include:

- SBPS build or commit identifier
- Windows version and display scale
- WebView2 Runtime version shown by the application
- printer model and driver version
- file type involved (do not send confidential documents)
- diagnostic log with secrets or private paths removed
