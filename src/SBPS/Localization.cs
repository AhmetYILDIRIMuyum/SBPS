using System.Globalization;

namespace SBPS;

internal enum AppLanguage
{
    Turkish,
    English,
}

internal static class AppLocalization
{
    private static readonly IReadOnlyDictionary<string, (string Turkish, string English)> Texts =
        new Dictionary<string, (string, string)>(StringComparer.Ordinal)
        {
            ["AppTitle"] = ("Smart Batch Printing Studio", "Smart Batch Printing Studio"),
            ["AppHeading"] = ("AKILLI TOPLU YAZDIRMA STÜDYOSU", "SMART BATCH PRINTING STUDIO"),
            ["AppSubtitle"] = ("Çoklu Dosya Yönetimi ve Toplu Yazdırma", "Multi-File Management and Batch Printing"),
            ["HeaderIdentity"] = ("Ahmet YILDIRIM  ·  {0}", "Ahmet YILDIRIM  ·  {0}"),
            ["FolderSection"] = ("DOSYA KLASÖRÜ", "FILE FOLDER"),
            ["FolderPlaceholder"] = ("Klasör yolunu yazın veya klasör seçin...", "Enter a folder path or choose a folder..."),
            ["SelectFolder"] = ("KLASÖR SEÇ", "CHOOSE FOLDER"),
            ["Refresh"] = ("YENİLE", "REFRESH"),
            ["IncludeSubfolders"] = ("Alt klasörler", "Subfolders"),
            ["FolderHelp"] = ("Yolu elle yazdıktan sonra Enter'a basın. Dosyalar adına göre sıralanır.", "Press Enter after typing a path. Files are sorted by name."),
            ["FileTypes"] = ("Dosya tipleri:", "File types:"),
            ["NoFileTypes"] = ("Desteklenen dosya yok", "No supported files"),
            ["QueueSection"] = ("BASKI SIRASI", "PRINT QUEUE"),
            ["FileCount"] = ("{0} DOSYA", "{0} FILES"),
            ["SelectedFileCount"] = ("{0}/{1} SEÇİLİ", "{0}/{1} SELECTED"),
            ["TotalFiles"] = ("Toplam: {0}", "Total: {0}"),
            ["SelectedFiles"] = ("Seçilen: {0}", "Selected: {0}"),
            ["PrintableFiles"] = ("Yazdırılacak: {0}", "To print: {0}"),
            ["KnownPages"] = ("Bilinen sayfa: {0}", "Known pages: {0}"),
            ["KnownPagesPartial"] = ("Bilinen sayfa: {0} (+{1} belirsiz)", "Known pages: {0} (+{1} unknown)"),
            ["SeparatorPages"] = ("Ayırıcı: {0}", "Separators: {0}"),
            ["PrinterSection"] = ("YAZICI VE BASKI", "PRINTER & PRINT"),
            ["Duplex"] = ("Her iki yüze", "Two-sided"),
            ["ShowPrinterSettings"] = ("Önce ayarları göster", "Show settings first"),
            ["AddSeparatorPage"] = ("Aralara boş sayfa ekle", "Add blank pages between files"),
            ["RefreshPrinters"] = ("YAZICILARI YENİLE", "REFRESH PRINTERS"),
            ["SendToPrinter"] = ("YAZICIYA GÖNDER", "SEND TO PRINTER"),
            ["Stop"] = ("DURDUR", "STOP"),
            ["Help"] = ("YARDIM", "HELP"),
            ["Exit"] = ("ÇIKIŞ", "EXIT"),
            ["HelpTitle"] = ("Kullanım Yardımı", "How to Use"),
            ["HelpIntro"] = ("Klasör seçin, dosyaları işaretleyin ve sırayı yazıcıya gönderin.", "Choose a folder, select the files, and send the queue to a printer."),
            ["HelpClose"] = ("Kapat", "Close"),
            ["HelpFooter"] = ("© 2026 Ahmet YILDIRIM [dede]. Telif ve geliştirme hakları saklıdır.   ahmetyildirimuyum@gmail.com", "© 2026 Ahmet YILDIRIM [dede]. Copyright and development rights reserved.   ahmetyildirimuyum@gmail.com"),
            ["HelpFolderTitle"] = ("Klasör ve dosya seçimi", "Folder and file selection"),
            ["HelpFolderBody"] = ("Yolu yazın veya Klasör Seç kullanın. Tip işaretini kaldırmak satırı silmez, yalnızca seçimini kapatır. Liste dosya adına göredir.", "Type a path or use Choose Folder. Clearing a type keeps the row and turns off its selection. The list is ordered by file name."),
            ["HelpPageTitle"] = ("Sayfa sayısı", "Page count"),
            ["HelpPageBody"] = ("Sayfa Sayısı sütunu güvenli okunan değeri gösterir. Görseller 1 sayfadır. Metin ve web belgeleri yazdırmada belirlenir. Okunamayan dosyada Hesaplanamadı yazar.", "Page Count shows a value only when it can be read safely. Images are 1 page. Text and web files are determined at print time. Unreadable files show Unavailable."),
            ["HelpStatsTitle"] = ("Yazdırma istatistikleri", "Print statistics"),
            ["HelpStatsBody"] = ("Toplam, seçilen ve yazdırılacak sayılar anında güncellenir. Yazdırılacak sayı, seçili ve kullanılabilir dosyaların gerçek kuyruğudur.", "Total, selected, and printable counts update immediately. Printable count is the real queue of selected files that are available."),
            ["HelpSeparatorTitle"] = ("Ayırıcı sayfa", "Separator page"),
            ["HelpSeparatorBody"] = ("Aralara boş sayfa ekle açıksa belgeler arasına bir boş sayfa gider. İlk dosyadan önce ve son dosyadan sonra ayırıcı yoktur.", "When Add blank pages between files is on, one blank page is sent between documents. None is added before the first or after the last file."),
            ["HelpPrintTitle"] = ("Yazıcıya gönderme", "Sending to the printer"),
            ["HelpPrintBody"] = ("Yazıcıyı ve çift tarafı seçin. Ayrıntı için Önce ayarları göster'i açın. Hata olursa kalan dosyalar gitmez. Çıkış, yazdırma sürerken onay ister.", "Choose the printer and two-sided printing. Turn on Show settings first for the detailed dialog. After an error, remaining files are not sent. Exit asks for confirmation while printing."),
            ["HelpFormatsTitle"] = ("Desteklenen türler", "Supported types"),
            ["HelpFormatsBody"] = ("PDF, JPG/JPEG, JFIF, PNG, BMP, GIF, WEBP, SVG, AVIF, ICO, JXL, TXT, CSV, XML, HTML, HTM, XHTML, MHT ve MHTML. Yalnızca seçili ve kullanılabilir dosyalar yazdırılır.", "PDF, JPG/JPEG, JFIF, PNG, BMP, GIF, WEBP, SVG, AVIF, ICO, JXL, TXT, CSV, XML, HTML, HTM, XHTML, MHT, and MHTML. Only selected and available files are printed."),
            ["Preparing"] = ("Yazdırma altyapısı hazırlanıyor...", "Preparing the printing engine..."),
            ["Secure"] = ("SIRALI  •  GÜVENLİ", "ORDERED  •  SAFE"),
            ["ColumnOrder"] = ("Sıra", "Order"),
            ["ColumnSelect"] = ("Seç", "Select"),
            ["ColumnPdf"] = ("Dosya", "File"),
            ["ColumnType"] = ("Tür", "Type"),
            ["ColumnPageCount"] = ("Sayfa Sayısı", "Page Count"),
            ["ColumnFolder"] = ("Klasör", "Folder"),
            ["ColumnSizeValue"] = ("Değer", "Value"),
            ["ColumnSizeUnit"] = ("Birim", "Unit"),
            ["ColumnStatus"] = ("Durum", "Status"),
            ["Ready"] = ("Hazır — WebView2 {0}", "Ready — WebView2 {0}"),
            ["WebViewMissingStatus"] = ("Microsoft Edge WebView2 Runtime bulunamadı.", "Microsoft Edge WebView2 Runtime was not found."),
            ["WebViewRequiredMessage"] = ("PDF yazdırma bileşeni için Microsoft Edge WebView2 Runtime gereklidir. Lütfen WebView2 Evergreen Runtime'ı kurup uygulamayı yeniden açın.", "Microsoft Edge WebView2 Runtime is required for PDF printing. Install the Evergreen Runtime and restart the application."),
            ["WebViewRequiredTitle"] = ("WebView2 gerekli", "WebView2 required"),
            ["InfrastructureFailed"] = ("Yazdırma altyapısı başlatılamadı.", "The printing engine could not be initialized."),
            ["FolderDialogDescription"] = ("Yazdırılacak dosyaların bulunduğu klasörü seçin", "Choose the folder containing files to print"),
            ["ChooseFolderStatus"] = ("Bir dosya klasörü seçin.", "Choose a file folder."),
            ["NoFiles"] = ("Seçili tiplerde yazdırılabilir dosya bulunamadı.", "No printable files were found for the selected types."),
            ["FilesFound"] = ("{0} dosya bulundu. Dosya seçimlerini ve baskı seçeneklerini kontrol edin.", "{0} files found. Review file selections and print options."),
            ["ReadFolderFailed"] = ("Klasördeki dosyalar okunamadı.", "Files in the folder could not be read."),
            ["NoPrinters"] = ("Windows'ta kurulu yazıcı bulunamadı.", "No installed Windows printer was found."),
            ["ReadPrintersFailed"] = ("Windows yazıcı listesi okunamadı.", "The Windows printer list could not be read."),
            ["PrinterInvalidMessage"] = ("'{0}' yazıcısı artık kullanılamıyor. Yazıcı listesini yenileyin.", "The '{0}' printer is no longer available. Refresh the printer list."),
            ["PrinterInvalidTitle"] = ("Yazıcı kullanılamıyor", "Printer unavailable"),
            ["ConfirmPrintMessage"] = ("{0} dosya, '{1}' yazıcısına gönderilecek.\n\nHer iki yüze yazdırma: {2}\nKopya: {3}\nDosyalar arasında boş sayfa: {4}\n\nDevam edilsin mi?", "{0} files will be sent to '{1}'.\n\nTwo-sided printing: {2}\nCopies: {3}\nBlank pages between files: {4}\n\nContinue?"),
            ["ConfirmPrintTitle"] = ("Yazdırmayı onayla", "Confirm printing"),
            ["On"] = ("Açık", "On"),
            ["Off"] = ("Kapalı", "Off"),
            ["StatusWaiting"] = ("Bekliyor", "Waiting"),
            ["StatusNotSelected"] = ("Seçilmedi", "Not selected"),
            ["StatusUnavailable"] = ("Kullanılamıyor", "Unavailable"),
            ["StatusPrinting"] = ("Yazdırılıyor", "Printing"),
            ["StatusSent"] = ("Gönderildi", "Sent"),
            ["StatusSentSeparator"] = ("Gönderildi + ayırıcı", "Sent + separator"),
            ["StatusCancelled"] = ("İptal edildi", "Cancelled"),
            ["StatusError"] = ("Hata", "Error"),
            ["ActiveFile"] = ("Dosya: {0}", "File: {0}"),
            ["PrintingFile"] = ("Dosya yazdırılıyor ({0}/{1}): {2}", "Printing file ({0}/{1}): {2}"),
            ["ActiveSeparator"] = ("{0} sonrası boş ayırıcı sayfa", "Blank separator after {0}"),
            ["SendingSeparator"] = ("{0} sonrasında boş ayırıcı sayfa gönderiliyor...", "Sending a blank separator page after {0}..."),
            ["CompletedStatus"] = ("Tamamlandı — {0} dosya ve {1} boş ayırıcı sayfa gönderildi.", "Completed — {0} files and {1} blank separator pages sent."),
            ["CompletedMessage"] = ("Tüm dosyalar ve aralarındaki boş ayırıcı sayfalar yazıcıya başarıyla gönderildi.", "All files and blank separator pages were successfully sent to the printer."),
            ["CompletedMessageNoSeparator"] = ("Seçilen tüm dosyalar yazıcıya başarıyla gönderildi.", "All selected files were successfully sent to the printer."),
            ["CompletedTitle"] = ("Yazdırma tamamlandı", "Printing completed"),
            ["CancelledStatus"] = ("Yazdırma kullanıcı tarafından durduruldu.", "Printing was stopped by the user."),
            ["FailedStatus"] = ("Yazdırma hata nedeniyle durduruldu.", "Printing stopped because of an error."),
            ["PrintFailedMessage"] = ("Yazdırma sırasında hata oluştu. Sıra korunması için kalan dosyalar gönderilmedi.", "An error occurred while printing. Remaining files were not sent in order to preserve the queue."),
            ["Phase"] = ("Aşama: {0}", "Stage: {0}"),
            ["PdfOpenFailed"] = ("PDF açılamadı. WebView2 hata kodu: {0}", "The PDF could not be opened. WebView2 error: {0}"),
            ["FileOpenFailed"] = ("Dosya açılamadı. WebView2 hata kodu: {0}", "The file could not be opened. WebView2 error: {0}"),
            ["UnsupportedFileType"] = ("'{0}' dosya tipi yazdırma için desteklenmiyor.", "The '{0}' file type is not supported for printing."),
            ["SeparatorFailed"] = ("Boş ayırıcı sayfa hazırlanamadı. WebView2 hata kodu: {0}", "The blank separator page could not be prepared. WebView2 error: {0}"),
            ["PrinterUnavailableException"] = ("'{0}' yazıcısı bulunamadı, çevrimdışı veya hata durumunda.", "The '{0}' printer was not found, is offline, or is in an error state."),
            ["WebViewPrintStatus"] = ("WebView2 yazdırma durumu: {0}", "WebView2 print status: {0}"),
            ["PrinterRejectedSettings"] = ("'{0}' yazıcısı WebView2 yazdırma ayarlarını kabul etmedi.", "The '{0}' printer rejected the WebView2 print settings."),
            ["RetryLog"] = ("Yazdırma yeniden denenecek ({0}/{1}) — {2}", "Printing will be retried ({0}/{1}) — {2}"),
            ["RetryStatus"] = ("{0} gönderilemedi; yeniden deneniyor ({1}/{2})...", "{0} could not be sent; retrying ({1}/{2})..."),
            ["ThreeAttempts"] = ("{0} üç denemede de yazdırılamadı. Yazıcı çevrimiçi olduğu halde sorun sürüyorsa WebView2 Runtime'ı güncelleyin.", "{0} could not be printed after three attempts. If the printer is online, update the WebView2 Runtime."),
            ["StopRequested"] = ("Durdurma istendi; etkin iş bittikten sonra sıra duracak...", "Stop requested; the queue will stop after the active job finishes..."),
            ["CloseMessage"] = ("Yazdırma devam ediyor. Uygulamayı kapatıp kalan işleri durdurmak istiyor musunuz?", "Printing is in progress. Close the application and stop the remaining jobs?"),
            ["CloseTitle"] = ("Yazdırma devam ediyor", "Printing in progress"),
            ["TechnicalDetail"] = ("Teknik ayrıntı: {0}", "Technical detail: {0}"),
            ["DiagnosticLog"] = ("Tanılama kaydı: {0}", "Diagnostic log: {0}"),
            ["ErrorTitle"] = ("Hata", "Error"),
            ["PageCountLoading"] = ("Hesaplanıyor…", "Calculating…"),
            ["PageCountAtPrint"] = ("Yazdırmada belirlenir", "Determined at print time"),
            ["PageCountUnavailable"] = ("Hesaplanamadı", "Unavailable"),
        };

    internal static AppLanguage DetectLanguage()
    {
        var uiLanguage = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        var regionalLanguage = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
        return uiLanguage.Equals("tr", StringComparison.OrdinalIgnoreCase) ||
               regionalLanguage.Equals("tr", StringComparison.OrdinalIgnoreCase)
            ? AppLanguage.Turkish
            : AppLanguage.English;
    }

    internal static string Get(AppLanguage language, string key, params object?[] args)
    {
        if (!Texts.TryGetValue(key, out var values))
        {
            return key;
        }

        var value = language == AppLanguage.Turkish ? values.Turkish : values.English;
        return args.Length == 0 ? value : string.Format(CultureInfo.CurrentCulture, value, args);
    }
}
