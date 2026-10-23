using System.ComponentModel;
using System.Drawing.Printing;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace SBPS;

internal sealed class MainForm : Form
{
    private static readonly TimeSpan PdfViewerReadyDelay = TimeSpan.FromSeconds(2);
    private static readonly TimeSpan PrintRetryDelay = TimeSpan.FromSeconds(2);
    private static readonly HashSet<string> SupportedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf", ".jpg", ".jpeg", ".jfif", ".png", ".bmp", ".gif", ".webp", ".svg", ".avif", ".ico", ".jxl",
        ".txt", ".csv", ".xml", ".html", ".htm", ".xhtml", ".mht", ".mhtml",
    };
    private static readonly HashSet<string> TextExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".txt", ".csv", ".xml",
    };

    private sealed record BatchPrintOptions(
        string PrinterName,
        bool Duplex,
        int Copies,
        bool Collate,
        bool Color,
        bool Landscape,
        double PageWidth,
        double PageHeight);

    private sealed record FileTypeOption(string Extension, int Count);

    private const string BlankPageHtml = """
        <!doctype html>
        <html>
        <head>
          <meta charset="utf-8">
          <style>
            @page { margin: 0; }
            html, body { width: 100%; height: 100%; margin: 0; background: white; }
            /* İçerik fiziksel olarak görünmez; bazı sürücülerin boş işi yok saymasını azaltır. */
            body::before { content: '\00a0'; color: white; font-size: 1px; }
          </style>
        </head>
        <body></body>
        </html>
        """;

    private readonly TextBox _folderTextBox = new();
    private readonly Label _appTitleLabel = new();
    private readonly Label _appSubtitleLabel = new();
    private readonly Label _headerIdentityLabel = new();
    private readonly ComboBox _languageComboBox = new();
    private readonly Label _folderSectionLabel = new();
    private readonly Label _folderHelpLabel = new();
    private readonly Label _fileTypesLabel = new();
    private readonly FlowLayoutPanel _fileTypesPanel = new();
    private readonly Label _queueSectionLabel = new();
    private readonly Label _printerSectionLabel = new();
    private readonly Label _secureLabel = new();
    private readonly PremiumButton _selectFolderButton = new();
    private readonly PremiumButton _refreshFilesButton = new();
    private readonly CheckBox _includeSubfoldersCheckBox = new();
    private readonly DataGridView _filesGrid = new();
    private readonly Label _totalFilesLabel = new();
    private readonly Label _selectedFilesLabel = new();
    private readonly Label _printQueueFilesLabel = new();
    private readonly Label _knownPagesLabel = new();
    private readonly Label _separatorPagesLabel = new();
    private readonly ComboBox _printersComboBox = new();
    private readonly CheckBox _duplexCheckBox = new();
    private readonly CheckBox _showPrinterSettingsCheckBox = new();
    private readonly CheckBox _addSeparatorPageCheckBox = new();
    private readonly CheckBox _skipFailedFileCheckBox = new();
    private readonly PremiumButton _refreshPrintersButton = new();
    private readonly PremiumButton _printButton = new();
    private readonly PremiumButton _cancelButton = new();
    private readonly PremiumButton _helpButton = new();
    private readonly PremiumButton _exitButton = new();
    private readonly PremiumProgressBar _progressBar = new();
    private readonly Label _statusLabel = new();
    private readonly WebView2 _webView = new();
    private readonly BindingList<PdfFileItem> _pdfFiles = [];

    private CancellationTokenSource? _printCancellation;
    private string? _sortProperty;
    private bool _sortAscending = true;
    private CancellationTokenSource? _pageCountCancellation;
    private bool _webViewReady;
    private bool _updatingLanguage;
    private bool _updatingFileTypes;
    private string? _webViewRuntimeVersion;
    private string? _loadedFolder;
    private readonly Dictionary<string, CheckBox> _fileTypeCheckBoxes =
        new(StringComparer.OrdinalIgnoreCase);
    private AppLanguage _language = AppLocalization.DetectLanguage();

    public MainForm()
    {
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        Text = "Smart Batch Printing Studio";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(1000, 720);
        Size = new Size(1368, 882);
        Font = new Font("Segoe UI", 9.5F);
        BackColor = PremiumPalette.Canvas;
        ForeColor = PremiumPalette.Text;

        BuildInterface();
        ApplyLanguage();
        WireEvents();
        EnableFolderDrop(this);
    }

    private static string AppDataFolder => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "FilePrintStudio");

    private void BuildInterface()
    {
        SuspendLayout();

        var shell = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = PremiumPalette.Canvas,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
        };
        shell.RowStyles.Add(new RowStyle(SizeType.Absolute, 124));
        shell.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var header = new GradientHeaderPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(30, 12, 30, 10),
            Margin = Padding.Empty,
        };
        var headerLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            ColumnCount = 2,
            RowCount = 1,
        };
        headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        var brandLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            ColumnCount = 1,
            RowCount = 2,
        };
        brandLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 62));
        brandLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 38));
        _appTitleLabel.Dock = DockStyle.Fill;
        _appTitleLabel.Font = new Font("Segoe UI Semibold", 20F);
        _appTitleLabel.ForeColor = Color.White;
        _appTitleLabel.TextAlign = ContentAlignment.BottomLeft;
        _appSubtitleLabel.Dock = DockStyle.Fill;
        _appSubtitleLabel.Font = new Font("Segoe UI", 9.5F);
        _appSubtitleLabel.ForeColor = Color.FromArgb(195, 215, 244);
        _appSubtitleLabel.TextAlign = ContentAlignment.TopLeft;
        brandLayout.Controls.Add(_appTitleLabel, 0, 0);
        brandLayout.Controls.Add(_appSubtitleLabel, 0, 1);

        var headerActions = new TableLayoutPanel
        {
            AutoSize = true,
            Anchor = AnchorStyles.Right,
            BackColor = Color.Transparent,
            ColumnCount = 1,
            Margin = Padding.Empty,
            RowCount = 2,
        };
        headerActions.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        headerActions.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _headerIdentityLabel.AutoSize = true;
        _headerIdentityLabel.Anchor = AnchorStyles.Right;
        _headerIdentityLabel.BackColor = Color.Transparent;
        _headerIdentityLabel.Font = new Font("Segoe UI", 9F);
        _headerIdentityLabel.ForeColor = Color.FromArgb(214, 226, 244);
        _headerIdentityLabel.Margin = new Padding(0, 2, 0, 8);
        _headerIdentityLabel.TextAlign = ContentAlignment.MiddleRight;

        var actionLayout = new FlowLayoutPanel
        {
            AutoSize = true,
            Anchor = AnchorStyles.Right,
            BackColor = Color.Transparent,
            FlowDirection = FlowDirection.RightToLeft,
            Margin = Padding.Empty,
            WrapContents = false,
        };
        _languageComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _languageComboBox.FlatStyle = FlatStyle.Flat;
        _languageComboBox.Font = new Font("Segoe UI Semibold", 9.5F);
        _languageComboBox.Margin = new Padding(0, 4, 0, 0);
        _languageComboBox.Width = 118;
        _languageComboBox.Items.AddRange(["Türkçe", "English"]);
        _languageComboBox.SelectedIndex = _language == AppLanguage.Turkish ? 0 : 1;
        ConfigureButton(_helpButton, string.Empty, PremiumButtonVariant.Secondary);
        _helpButton.MinimumSize = new Size(108, 36);
        _helpButton.Height = 36;
        _helpButton.Margin = new Padding(0, 0, 8, 0);
        _helpButton.Font = new Font("Segoe UI Semibold", 9.5F);
        ConfigureButton(_exitButton, string.Empty, PremiumButtonVariant.Danger);
        _exitButton.MinimumSize = new Size(108, 36);
        _exitButton.Height = 36;
        _exitButton.Margin = Padding.Empty;
        _exitButton.Font = new Font("Segoe UI Semibold", 9.5F);
        var exitGap = new Panel
        {
            BackColor = Color.Transparent,
            Margin = Padding.Empty,
            Size = new Size(18, 36),
        };
        actionLayout.Controls.Add(_exitButton);
        actionLayout.Controls.Add(exitGap);
        actionLayout.Controls.Add(_languageComboBox);
        actionLayout.Controls.Add(_helpButton);
        headerActions.Controls.Add(_headerIdentityLabel, 0, 0);
        headerActions.Controls.Add(actionLayout, 0, 1);
        headerLayout.Controls.Add(brandLayout, 0, 0);
        headerLayout.Controls.Add(headerActions, 1, 0);
        header.Controls.Add(headerLayout);

        var content = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            BackColor = PremiumPalette.Canvas,
            Padding = new Padding(24, 18, 24, 18),
        };
        content.RowStyles.Add(new RowStyle(SizeType.Absolute, 170));
        content.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        content.RowStyles.Add(new RowStyle(SizeType.Absolute, 198));
        content.RowStyles.Add(new RowStyle(SizeType.Absolute, 62));

        var folderCard = new PremiumCardPanel
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 0, 0, 12),
        };
        var folderCardLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            BackColor = Color.Transparent,
        };
        folderCardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
        folderCardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        folderCardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
        folderCardLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        ConfigureSectionTitle(_folderSectionLabel, "01");
        folderCardLayout.Controls.Add(_folderSectionLabel, 0, 0);

        var folderInputLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 1,
            BackColor = Color.Transparent,
            Margin = new Padding(0, 2, 0, 0),
        };
        folderInputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        folderInputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        folderInputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        folderInputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        _folderTextBox.Dock = DockStyle.Fill;
        _folderTextBox.AutoSize = false;
        _folderTextBox.Height = 38;
        _folderTextBox.BorderStyle = BorderStyle.FixedSingle;
        _folderTextBox.BackColor = Color.White;
        _folderTextBox.ForeColor = PremiumPalette.Text;
        _folderTextBox.Font = new Font("Segoe UI", 10F);
        _folderTextBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        _folderTextBox.AutoCompleteSource = AutoCompleteSource.FileSystemDirectories;
        _folderTextBox.Margin = new Padding(0, 0, 10, 0);

        ConfigureButton(_selectFolderButton, string.Empty, PremiumButtonVariant.Primary);
        ConfigureButton(_refreshFilesButton, string.Empty, PremiumButtonVariant.Secondary);
        ConfigureCheckBox(_includeSubfoldersCheckBox, string.Empty, isChecked: false);

        folderInputLayout.Controls.Add(_folderTextBox, 0, 0);
        folderInputLayout.Controls.Add(_selectFolderButton, 1, 0);
        folderInputLayout.Controls.Add(_refreshFilesButton, 2, 0);
        folderInputLayout.Controls.Add(_includeSubfoldersCheckBox, 3, 0);
        folderCardLayout.Controls.Add(folderInputLayout, 0, 1);
        _folderHelpLabel.Dock = DockStyle.Fill;
        _folderHelpLabel.ForeColor = PremiumPalette.Muted;
        _folderHelpLabel.Font = new Font("Segoe UI", 8.5F);
        _folderHelpLabel.TextAlign = ContentAlignment.MiddleLeft;
        folderCardLayout.Controls.Add(_folderHelpLabel, 0, 2);

        var fileTypesLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = Color.Transparent,
            Margin = Padding.Empty,
        };
        fileTypesLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        fileTypesLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        _fileTypesLabel.AutoSize = true;
        _fileTypesLabel.Anchor = AnchorStyles.Left;
        _fileTypesLabel.Font = new Font("Segoe UI Semibold", 9F);
        _fileTypesLabel.ForeColor = PremiumPalette.Navy;
        _fileTypesLabel.Margin = new Padding(0, 6, 10, 0);
        _fileTypesPanel.Dock = DockStyle.Fill;
        _fileTypesPanel.AutoScroll = true;
        _fileTypesPanel.WrapContents = false;
        _fileTypesPanel.FlowDirection = FlowDirection.LeftToRight;
        _fileTypesPanel.BackColor = Color.Transparent;
        _fileTypesPanel.Margin = Padding.Empty;
        fileTypesLayout.Controls.Add(_fileTypesLabel, 0, 0);
        fileTypesLayout.Controls.Add(_fileTypesPanel, 1, 0);
        folderCardLayout.Controls.Add(fileTypesLayout, 0, 3);
        folderCard.Controls.Add(folderCardLayout);

        ConfigureGrid();
        var filesCard = new PremiumCardPanel
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 0, 0, 12),
            Padding = new Padding(18, 14, 18, 16),
        };
        var filesCardLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = Color.Transparent,
        };
        filesCardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
        filesCardLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        var filesHeader = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = Color.Transparent,
        };
        filesHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        filesHeader.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        ConfigureSectionTitle(_queueSectionLabel, "02");
        filesHeader.Controls.Add(_queueSectionLabel, 0, 0);
        var statisticsPanel = new FlowLayoutPanel
        {
            AutoSize = true,
            Anchor = AnchorStyles.Right,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            BackColor = Color.Transparent,
            Margin = Padding.Empty,
        };
        ConfigureStatisticLabel(_totalFilesLabel, PremiumPalette.SoftBlue, PremiumPalette.Primary);
        ConfigureStatisticLabel(_selectedFilesLabel, Color.FromArgb(231, 248, 244), Color.FromArgb(9, 125, 104));
        ConfigureStatisticLabel(_printQueueFilesLabel, Color.FromArgb(235, 242, 255), PremiumPalette.Navy);
        ConfigureStatisticLabel(_knownPagesLabel, Color.FromArgb(247, 240, 255), Color.FromArgb(108, 70, 160));
        ConfigureStatisticLabel(_separatorPagesLabel, Color.FromArgb(255, 246, 225), Color.FromArgb(156, 101, 12));
        statisticsPanel.Controls.Add(_totalFilesLabel);
        statisticsPanel.Controls.Add(_selectedFilesLabel);
        statisticsPanel.Controls.Add(_printQueueFilesLabel);
        statisticsPanel.Controls.Add(_knownPagesLabel);
        statisticsPanel.Controls.Add(_separatorPagesLabel);
        filesHeader.Controls.Add(statisticsPanel, 1, 0);
        filesCardLayout.Controls.Add(filesHeader, 0, 0);
        filesCardLayout.Controls.Add(_filesGrid, 0, 1);
        filesCard.Controls.Add(filesCardLayout);

        var printerCard = new PremiumCardPanel
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 0, 0, 12),
        };
        var printerCardLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            BackColor = Color.Transparent,
        };
        printerCardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
        printerCardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        printerCardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
        printerCardLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        ConfigureSectionTitle(_printerSectionLabel, "03");
        printerCardLayout.Controls.Add(_printerSectionLabel, 0, 0);

        var printerLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 1,
            BackColor = Color.Transparent,
        };
        printerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        _printersComboBox.Dock = DockStyle.Fill;
        _printersComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _printersComboBox.FlatStyle = FlatStyle.Flat;
        _printersComboBox.Font = new Font("Segoe UI", 10F);
        _printersComboBox.BackColor = Color.White;
        _printersComboBox.ForeColor = PremiumPalette.Text;
        _printersComboBox.Margin = new Padding(0, 3, 0, 3);

        ConfigureCheckBox(_duplexCheckBox, string.Empty, isChecked: true);
        ConfigureCheckBox(_showPrinterSettingsCheckBox, string.Empty, isChecked: false);
        ConfigureCheckBox(_addSeparatorPageCheckBox, string.Empty, isChecked: true);
        ConfigureCheckBox(_skipFailedFileCheckBox, string.Empty, isChecked: false);
        ConfigureButton(_refreshPrintersButton, string.Empty, PremiumButtonVariant.Secondary);
        ConfigureButton(_printButton, string.Empty, PremiumButtonVariant.Primary);
        _printButton.MinimumSize = new Size(154, 40);
        _printButton.Enabled = false;
        ConfigureButton(_cancelButton, string.Empty, PremiumButtonVariant.Danger);
        _cancelButton.Enabled = false;

        printerLayout.Controls.Add(_printersComboBox, 0, 0);
        printerCardLayout.Controls.Add(printerLayout, 0, 1);

        var printerOptionsLayout = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoScroll = false,
            BackColor = Color.Transparent,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
        };
        printerOptionsLayout.Controls.Add(_duplexCheckBox);
        printerOptionsLayout.Controls.Add(_showPrinterSettingsCheckBox);
        printerOptionsLayout.Controls.Add(_addSeparatorPageCheckBox);
        printerOptionsLayout.Controls.Add(_skipFailedFileCheckBox);
        printerCardLayout.Controls.Add(printerOptionsLayout, 0, 2);

        var printerActionsLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 1,
            BackColor = Color.Transparent,
            Margin = Padding.Empty,
        };
        printerActionsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        printerActionsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        printerActionsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        printerActionsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        printerActionsLayout.Controls.Add(_refreshPrintersButton, 1, 0);
        printerActionsLayout.Controls.Add(_printButton, 2, 0);
        printerActionsLayout.Controls.Add(_cancelButton, 3, 0);
        printerCardLayout.Controls.Add(printerActionsLayout, 0, 3);
        printerCard.Controls.Add(printerCardLayout);

        var statusCard = new PremiumCardPanel
        {
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            Padding = new Padding(18, 10, 18, 10),
        };
        var statusLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 2,
            BackColor = Color.Transparent,
        };
        statusLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        statusLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        statusLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        statusLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 8));
        _statusLabel.AutoSize = true;
        _statusLabel.ForeColor = PremiumPalette.Muted;
        _statusLabel.Font = new Font("Segoe UI", 9F);
        _statusLabel.Anchor = AnchorStyles.Left;
        _secureLabel.AutoSize = true;
        _secureLabel.Anchor = AnchorStyles.Right;
        _secureLabel.Font = new Font("Segoe UI Semibold", 8F);
        _secureLabel.ForeColor = PremiumPalette.Cyan;
        _progressBar.Dock = DockStyle.Fill;
        _progressBar.Minimum = 0;
        _progressBar.Maximum = 1;
        statusLayout.Controls.Add(_statusLabel, 0, 0);
        statusLayout.Controls.Add(_secureLabel, 1, 0);
        statusLayout.Controls.Add(_progressBar, 0, 1);
        statusLayout.SetColumnSpan(_progressBar, 2);
        statusCard.Controls.Add(statusLayout);

        // PDF görüntüleyici gerçek bir çizim alanına sahip olmazsa bazı WebView2
        // sürümleri PrintAsync çağrısında E_FAIL döndürebiliyor. Kontrol formu
        // kaplar ancak ana arayüzün arkasında tutulur.
        _webView.Dock = DockStyle.Fill;
        _webView.TabStop = false;
        _webView.DefaultBackgroundColor = Color.White;

        content.Controls.Add(folderCard, 0, 0);
        content.Controls.Add(filesCard, 0, 1);
        content.Controls.Add(printerCard, 0, 2);
        content.Controls.Add(statusCard, 0, 3);
        shell.Controls.Add(header, 0, 0);
        shell.Controls.Add(content, 0, 1);

        Controls.Add(shell);
        Controls.Add(_webView);
        _webView.SendToBack();
        shell.BringToFront();
        ResumeLayout(performLayout: true);
    }

    private static void ConfigureSectionTitle(Label label, string number)
    {
        label.Tag = number;
        label.Dock = DockStyle.Fill;
        label.Font = new Font("Segoe UI Semibold", 9F);
        label.ForeColor = PremiumPalette.Text;
        label.TextAlign = ContentAlignment.MiddleLeft;
    }

    private static void ConfigureButton(
        PremiumButton button,
        string text,
        PremiumButtonVariant variant)
    {
        button.Text = text;
        button.Variant = variant;
        button.AutoSize = true;
        button.Height = 40;
        button.Margin = new Padding(0, 0, 8, 0);
    }

    private static void ConfigureCheckBox(CheckBox checkBox, string text, bool isChecked)
    {
        checkBox.Text = text;
        checkBox.Checked = isChecked;
        checkBox.AutoSize = true;
        checkBox.Anchor = AnchorStyles.Left;
        checkBox.FlatStyle = FlatStyle.Flat;
        checkBox.Font = new Font("Segoe UI Semibold", 9F);
        checkBox.ForeColor = PremiumPalette.Text;
        checkBox.Margin = new Padding(8, 0, 14, 0);
    }

    private static void ConfigureStatisticLabel(Label label, Color background, Color foreground)
    {
        label.AutoSize = true;
        label.Anchor = AnchorStyles.Right;
        label.Font = new Font("Segoe UI Semibold", 7.7F);
        label.ForeColor = foreground;
        label.BackColor = background;
        label.Padding = new Padding(7, 5, 7, 5);
        label.Margin = new Padding(4, 0, 0, 0);
    }

    private string T(string key, params object?[] args) => AppLocalization.Get(_language, key, args);

    private void ApplyLanguage()
    {
        _updatingLanguage = true;
        try
        {
            Text = T("AppTitle");
            _appTitleLabel.Text = T("AppHeading");
            _appSubtitleLabel.Text = T("AppSubtitle");
            _headerIdentityLabel.Text = T(
                "HeaderIdentity",
                DateTime.Now.ToString("d", System.Globalization.CultureInfo.CurrentCulture));

            _folderSectionLabel.Text = $"01   {T("FolderSection")}";
            _queueSectionLabel.Text = $"02   {T("QueueSection")}";
            _printerSectionLabel.Text = $"03   {T("PrinterSection")}";
            _folderTextBox.PlaceholderText = T("FolderPlaceholder");
            _folderHelpLabel.Text = T("FolderHelp");
            _fileTypesLabel.Text = T("FileTypes");
            _selectFolderButton.Text = T("SelectFolder");
            _refreshFilesButton.Text = T("Refresh");
            _includeSubfoldersCheckBox.Text = T("IncludeSubfolders");
            _duplexCheckBox.Text = T("Duplex");
            _showPrinterSettingsCheckBox.Text = T("ShowPrinterSettings");
            _addSeparatorPageCheckBox.Text = T("AddSeparatorPage");
            _skipFailedFileCheckBox.Text = T("SkipFailedFile");
            _refreshPrintersButton.Text = T("RefreshPrinters");
            _printButton.Text = T("SendToPrinter");
            _cancelButton.Text = T("Stop");
            _helpButton.Text = T("Help");
            _exitButton.Text = T("Exit");
            _secureLabel.Text = T("Secure");
            UpdateSelectionSummary();
            UpdateFileTypeTexts();

            if (_filesGrid.Columns.Count >= 9)
            {
                _filesGrid.Columns[0].HeaderText = T("ColumnStatus");
                _filesGrid.Columns[1].HeaderText = T("ColumnSelect");
                _filesGrid.Columns[2].HeaderText = T("ColumnOrder");
                _filesGrid.Columns[3].HeaderText = T("ColumnPdf");
                _filesGrid.Columns[4].HeaderText = T("ColumnType");
                _filesGrid.Columns[5].HeaderText = T("ColumnPageCount");
                _filesGrid.Columns[6].HeaderText = T("ColumnFolder");
                _filesGrid.Columns[7].HeaderText = T("ColumnSizeValue");
                _filesGrid.Columns[8].HeaderText = T("ColumnSizeUnit");
            }

            foreach (var item in _pdfFiles)
            {
                SetItemState(item, item.State);
                UpdatePageCountText(item);
            }

            _languageComboBox.SelectedIndex = _language == AppLanguage.Turkish ? 0 : 1;

            if (_printCancellation is null)
            {
                if (!_webViewReady)
                {
                    _statusLabel.Text = T("Preparing");
                }
                else if (_pdfFiles.Count == 0)
                {
                    _statusLabel.Text = T("ChooseFolderStatus");
                }
                else
                {
                    _statusLabel.Text = T("FilesFound", _pdfFiles.Count);
                }
            }
        }
        finally
        {
            _updatingLanguage = false;
        }
    }

    private void LanguageComboBoxOnSelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_updatingLanguage || _languageComboBox.SelectedIndex < 0)
        {
            return;
        }

        _language = _languageComboBox.SelectedIndex == 0
            ? AppLanguage.Turkish
            : AppLanguage.English;
        ApplyLanguage();
    }

    private void SetItemState(PdfFileItem item, PdfPrintState state)
    {
        item.State = state;
        item.Status = T(state switch
        {
            PdfPrintState.Printing => "StatusPrinting",
            PdfPrintState.Sent => "StatusSent",
            PdfPrintState.SentWithSeparator => "StatusSentSeparator",
            PdfPrintState.Cancelled => "StatusCancelled",
            PdfPrintState.Error => "StatusError",
            PdfPrintState.NotSelected => "StatusNotSelected",
            PdfPrintState.Unavailable => "StatusUnavailable",
            _ => "StatusWaiting",
        });
    }

    private void ConfigureGrid()
    {
        _filesGrid.Dock = DockStyle.Fill;
        _filesGrid.Margin = Padding.Empty;
        _filesGrid.AutoGenerateColumns = false;
        _filesGrid.AllowUserToAddRows = false;
        _filesGrid.AllowUserToDeleteRows = false;
        _filesGrid.AllowUserToResizeRows = false;
        _filesGrid.AllowUserToOrderColumns = false;
        _filesGrid.ReadOnly = false;
        _filesGrid.MultiSelect = false;
        _filesGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _filesGrid.RowHeadersVisible = false;
        _filesGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _filesGrid.BackgroundColor = Color.White;
        _filesGrid.BorderStyle = BorderStyle.None;
        _filesGrid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        _filesGrid.GridColor = Color.FromArgb(235, 239, 246);
        _filesGrid.EnableHeadersVisualStyles = false;
        _filesGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        _filesGrid.ColumnHeadersHeight = 42;
        _filesGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        _filesGrid.RowTemplate.Height = 38;
        _filesGrid.DefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = Color.White,
            ForeColor = PremiumPalette.Text,
            SelectionBackColor = PremiumPalette.SoftBlue,
            SelectionForeColor = PremiumPalette.Navy,
            Font = new Font("Segoe UI", 9.5F),
            Padding = new Padding(8, 0, 8, 0),
        };
        _filesGrid.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = Color.FromArgb(250, 252, 255),
            ForeColor = PremiumPalette.Text,
            SelectionBackColor = PremiumPalette.SoftBlue,
            SelectionForeColor = PremiumPalette.Navy,
        };
        _filesGrid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = PremiumPalette.NavySoft,
            ForeColor = Color.White,
            SelectionBackColor = PremiumPalette.NavySoft,
            SelectionForeColor = Color.White,
            Font = new Font("Segoe UI Semibold", 9F),
            Padding = new Padding(8, 0, 8, 0),
        };
        _filesGrid.DataSource = _pdfFiles;

        _filesGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(PdfFileItem.Status),
            HeaderText = "Durum",
            FillWeight = 14,
            MinimumWidth = 108,
            ReadOnly = true,
        });
        _filesGrid.Columns.Add(new DataGridViewCheckBoxColumn
        {
            DataPropertyName = nameof(PdfFileItem.IsSelected),
            HeaderText = "Seç",
            FillWeight = 8,
            MinimumWidth = 48,
            ReadOnly = false,
            SortMode = DataGridViewColumnSortMode.Programmatic,
        });
        _filesGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(PdfFileItem.Order),
            HeaderText = "Sıra",
            FillWeight = 6,
            MinimumWidth = 46,
            ReadOnly = true,
        });
        _filesGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(PdfFileItem.FileName),
            HeaderText = "Dosya",
            FillWeight = 62,
            MinimumWidth = 220,
            ReadOnly = true,
        });
        _filesGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(PdfFileItem.FileType),
            HeaderText = "Tür",
            FillWeight = 8,
            MinimumWidth = 56,
            ReadOnly = true,
        });
        _filesGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(PdfFileItem.PageCountText),
            HeaderText = "Sayfa Sayısı",
            FillWeight = 12,
            MinimumWidth = 108,
            ReadOnly = true,
            DefaultCellStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleRight,
                Padding = new Padding(8, 0, 8, 0),
            },
        });
        _filesGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(PdfFileItem.DirectoryName),
            HeaderText = "Klasör",
            FillWeight = 46,
            MinimumWidth = 180,
            ReadOnly = true,
        });
        _filesGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(PdfFileItem.SizeValue),
            HeaderText = "Değer",
            FillWeight = 8,
            MinimumWidth = 64,
            ReadOnly = true,
            DefaultCellStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleRight,
                Padding = new Padding(8, 0, 8, 0),
            },
        });
        _filesGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(PdfFileItem.SizeUnit),
            HeaderText = "Birim",
            FillWeight = 6,
            MinimumWidth = 52,
            ReadOnly = true,
        });
        _filesGrid.CellFormatting += FilesGridOnCellFormatting;
        foreach (DataGridViewColumn column in _filesGrid.Columns)
        {
            column.SortMode = DataGridViewColumnSortMode.Programmatic;
        }
    }

    private void WireEvents()
    {
        Shown += async (_, _) => await InitializeAsync();
        _languageComboBox.SelectedIndexChanged += LanguageComboBoxOnSelectedIndexChanged;
        _helpButton.Click += (_, _) => ShowHelp();
        _exitButton.Click += (_, _) => Close();
        _folderTextBox.KeyDown += FolderTextBoxOnKeyDown;
        _selectFolderButton.Click += (_, _) => SelectFolder();
        _refreshFilesButton.Click += (_, _) => LoadFiles();
        _includeSubfoldersCheckBox.CheckedChanged += (_, _) => LoadFiles();
        _filesGrid.ColumnHeaderMouseClick += FilesGridOnColumnHeaderMouseClick;
        _filesGrid.CurrentCellDirtyStateChanged += FilesGridOnCurrentCellDirtyStateChanged;
        _filesGrid.CellValueChanged += FilesGridOnCellValueChanged;
        _refreshPrintersButton.Click += (_, _) => LoadPrinters();
        _printersComboBox.SelectedIndexChanged += (_, _) => UpdatePrintButtonState();
        _addSeparatorPageCheckBox.CheckedChanged += (_, _) => UpdateSelectionSummary();
        _printButton.Click += async (_, _) => await PrintAllAsync();
        _cancelButton.Click += (_, _) => CancelPrinting();
        FormClosing += OnFormClosing;
    }

    private void FilesGridOnColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
    {
        if (e.ColumnIndex < 0 || _printCancellation is not null)
        {
            return;
        }

        var propertyName = _filesGrid.Columns[e.ColumnIndex].DataPropertyName;
        if (string.IsNullOrEmpty(propertyName))
        {
            return;
        }

        _sortAscending = _sortProperty == propertyName ? !_sortAscending : true;
        _sortProperty = propertyName;

        var items = _pdfFiles.ToList();
        items.Sort(CompareSortedItems);
        _pdfFiles.RaiseListChangedEvents = false;
        _pdfFiles.Clear();
        foreach (var item in items)
        {
            _pdfFiles.Add(item);
        }

        _pdfFiles.RaiseListChangedEvents = true;
        _pdfFiles.ResetBindings();
        foreach (DataGridViewColumn column in _filesGrid.Columns)
        {
            column.HeaderCell.SortGlyphDirection = column.DataPropertyName == _sortProperty
                ? _sortAscending
                    ? SortOrder.Ascending
                    : SortOrder.Descending
                : SortOrder.None;
        }
    }

    private void ClearColumnSort()
    {
        _sortProperty = null;
        _sortAscending = true;
        if (_filesGrid.Columns.Count == 0)
        {
            return;
        }

        foreach (DataGridViewColumn column in _filesGrid.Columns)
        {
            column.HeaderCell.SortGlyphDirection = SortOrder.None;
        }
    }

    private int CompareSortedItems(PdfFileItem left, PdfFileItem right)
    {
        var result = _sortProperty switch
        {
            nameof(PdfFileItem.Order) => left.Order.CompareTo(right.Order),
            nameof(PdfFileItem.IsSelected) => left.IsSelected.CompareTo(right.IsSelected),
            nameof(PdfFileItem.FileName) => StringComparer.CurrentCultureIgnoreCase.Compare(left.FileName, right.FileName),
            nameof(PdfFileItem.FileType) => StringComparer.CurrentCultureIgnoreCase.Compare(left.FileType, right.FileType),
            nameof(PdfFileItem.DirectoryName) => StringComparer.CurrentCultureIgnoreCase.Compare(left.DirectoryName, right.DirectoryName),
            nameof(PdfFileItem.Status) => StringComparer.CurrentCultureIgnoreCase.Compare(left.Status, right.Status),
            nameof(PdfFileItem.PageCountText) => CompareKnownNumber(left.PageCount, right.PageCount),
            nameof(PdfFileItem.SizeValue) => left.SizeBytes.CompareTo(right.SizeBytes),
            nameof(PdfFileItem.SizeUnit) => CompareSizeUnit(left, right),
            _ => 0,
        };

        if (_sortProperty is nameof(PdfFileItem.PageCountText))
        {
            return result;
        }

        return _sortAscending ? result : -result;
    }

    private int CompareKnownNumber(int? left, int? right)
    {
        if (left is null && right is null)
        {
            return 0;
        }

        if (left is null)
        {
            return 1;
        }

        if (right is null)
        {
            return -1;
        }

        var result = left.Value.CompareTo(right.Value);
        return _sortAscending ? result : -result;
    }

    private static int CompareSizeUnit(PdfFileItem left, PdfFileItem right)
    {
        var unitCompare = SizeUnitRank(left.SizeUnit).CompareTo(SizeUnitRank(right.SizeUnit));
        return unitCompare != 0 ? unitCompare : left.SizeBytes.CompareTo(right.SizeBytes);
    }

    private static int SizeUnitRank(string unit) => unit switch
    {
        "B" => 0,
        "KB" => 1,
        "MB" => 2,
        "GB" => 3,
        _ => 4,
    };

    private void FilesGridOnCurrentCellDirtyStateChanged(object? sender, EventArgs e)
    {
        if (_filesGrid.IsCurrentCellDirty &&
            _filesGrid.CurrentCell is DataGridViewCheckBoxCell)
        {
            _filesGrid.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }
    }

    private void FilesGridOnCellValueChanged(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 ||
            e.ColumnIndex < 0 ||
            _printCancellation is not null ||
            _filesGrid.Columns[e.ColumnIndex].DataPropertyName != nameof(PdfFileItem.IsSelected))
        {
            return;
        }

        if (_filesGrid.Rows[e.RowIndex].DataBoundItem is PdfFileItem item)
        {
            SetItemState(
                item,
                !item.IsSelected
                    ? PdfPrintState.NotSelected
                    : IsItemPrintable(item)
                        ? PdfPrintState.Waiting
                        : PdfPrintState.Unavailable);
        }

        UpdateSelectionSummary();
        UpdatePrintButtonState();
    }

    private void FolderTextBoxOnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Enter)
        {
            return;
        }

        e.Handled = true;
        e.SuppressKeyPress = true;
        LoadFiles();
    }

    private async Task InitializeAsync()
    {
        LoadPrinters();

        try
        {
            var runtimeVersion = CoreWebView2Environment.GetAvailableBrowserVersionString();
            var userDataFolder = Path.Combine(AppDataFolder, "WebView2");
            Directory.CreateDirectory(userDataFolder);

            var environment = await CoreWebView2Environment.CreateAsync(userDataFolder: userDataFolder);
            await _webView.EnsureCoreWebView2Async(environment);

            _webView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
            _webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            _webView.CoreWebView2.Settings.AreDevToolsEnabled = false;
            _webView.CoreWebView2.Settings.IsStatusBarEnabled = false;

            _webViewReady = true;
            _webViewRuntimeVersion = runtimeVersion;
            if (!RestoreLastFolder())
            {
                _statusLabel.Text = T("Ready", runtimeVersion);
            }

            UpdatePrintButtonState();
        }
        catch (WebView2RuntimeNotFoundException)
        {
            _statusLabel.Text = T("WebViewMissingStatus");
            MessageBox.Show(
                this,
                T("WebViewRequiredMessage"),
                T("WebViewRequiredTitle"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            _statusLabel.Text = T("InfrastructureFailed");
            ShowError(T("InfrastructureFailed"), ex);
        }
    }

    private void EnableFolderDrop(Control control)
    {
        control.AllowDrop = true;
        control.DragEnter -= FolderOnDragEnter;
        control.DragOver -= FolderOnDragEnter;
        control.DragDrop -= FolderOnDragDrop;
        control.DragEnter += FolderOnDragEnter;
        control.DragOver += FolderOnDragEnter;
        control.DragDrop += FolderOnDragDrop;
        control.ControlAdded -= FolderDropControlAdded;
        control.ControlAdded += FolderDropControlAdded;

        foreach (Control child in control.Controls)
        {
            EnableFolderDrop(child);
        }
    }

    private void FolderDropControlAdded(object? sender, ControlEventArgs e)
    {
        if (e.Control is not null)
        {
            EnableFolderDrop(e.Control);
        }
    }

    private void FolderOnDragEnter(object? sender, DragEventArgs e)
    {
        e.Effect = _printCancellation is null && FirstDroppedFolder(e) is not null
            ? DragDropEffects.Copy
            : DragDropEffects.None;
    }

    private void FolderOnDragDrop(object? sender, DragEventArgs e)
    {
        if (_printCancellation is not null)
        {
            return;
        }

        var folder = FirstDroppedFolder(e);
        if (folder is null)
        {
            return;
        }

        _folderTextBox.Text = folder;
        LoadFiles();
    }

    private static string? FirstDroppedFolder(DragEventArgs e)
    {
        if (e.Data?.GetData(DataFormats.FileDrop) is not string[] paths)
        {
            return null;
        }

        return paths.FirstOrDefault(Directory.Exists);
    }

    private bool RestoreLastFolder()
    {
        var folder = ReadLastFolder();
        if (folder is null || !Directory.Exists(folder))
        {
            return false;
        }

        _folderTextBox.Text = folder;
        LoadFiles();
        return true;
    }

    private static string LastFolderFilePath => Path.Combine(AppDataFolder, "last-folder.txt");

    private static string? ReadLastFolder()
    {
        try
        {
            if (!File.Exists(LastFolderFilePath))
            {
                return null;
            }

            var folder = File.ReadAllText(LastFolderFilePath).Trim();
            return string.IsNullOrWhiteSpace(folder) ? null : folder;
        }
        catch (IOException)
        {
            return null;
        }
        catch (UnauthorizedAccessException)
        {
            return null;
        }
    }

    private static void SaveLastFolder(string folder)
    {
        try
        {
            Directory.CreateDirectory(AppDataFolder);
            File.WriteAllText(LastFolderFilePath, folder);
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }

    private void SelectFolder()
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = T("FolderDialogDescription"),
            UseDescriptionForTitle = true,
            ShowNewFolderButton = false,
        };

        if (Directory.Exists(_folderTextBox.Text))
        {
            dialog.InitialDirectory = _folderTextBox.Text;
        }

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        _folderTextBox.Text = dialog.SelectedPath;
        LoadFiles();
    }

    private void LoadFiles(bool rebuildFileTypes = true)
    {
        if (_printCancellation is not null)
        {
            return;
        }

        CancelPageCountLoading();
        _pdfFiles.Clear();
        UpdateSelectionSummary();
        var folder = _folderTextBox.Text.Trim();

        if (!Directory.Exists(folder))
        {
            ClearFileTypeFilters();
            _statusLabel.Text = _webViewReady ? T("ChooseFolderStatus") : _statusLabel.Text;
            UpdatePrintButtonState();
            return;
        }

        try
        {
            var searchOption = _includeSubfoldersCheckBox.Checked
                ? SearchOption.AllDirectories
                : SearchOption.TopDirectoryOnly;

            var discoveredPaths = Directory
                .EnumerateFiles(folder, "*.*", searchOption)
                .ToArray();
            var allPaths = discoveredPaths
                .Where(path => SupportedExtensions.Contains(Path.GetExtension(path)))
                .OrderBy(path => Path.GetFileName(path), StringComparer.CurrentCultureIgnoreCase)
                .ThenBy(path => path, StringComparer.CurrentCultureIgnoreCase)
                .ToArray();
            var excludedCount = discoveredPaths.Length - allPaths.Length;

            var sourceKey = $"{Path.GetFullPath(folder)}|{searchOption}";
            if (rebuildFileTypes)
            {
                RebuildFileTypeFilters(sourceKey, allPaths);
            }

            var selectedExtensions = SelectedFileTypeExtensions();

            for (var index = 0; index < allPaths.Length; index++)
            {
                var file = new FileInfo(allPaths[index]);
                var included = selectedExtensions.Contains(file.Extension);
                var item = new PdfFileItem
                {
                    Order = index + 1,
                    FullPath = file.FullName,
                    SizeBytes = file.Length,
                    IsSelected = included,
                };
                SetItemState(item, included ? PdfPrintState.Waiting : PdfPrintState.NotSelected);
                _pdfFiles.Add(item);
            }

            UpdateSelectionSummary();
            ClearColumnSort();

            _statusLabel.Text = T("FilesScanned", allPaths.Length, excludedCount);
            SaveLastFolder(Path.GetFullPath(folder));

            StartPageCountLoading();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            ShowError(T("ReadFolderFailed"), ex);
        }

        UpdatePrintButtonState();
    }

    private void StartPageCountLoading()
    {
        if (_pdfFiles.Count == 0)
        {
            return;
        }

        foreach (var item in _pdfFiles)
        {
            item.PageCount = null;
            item.PageCountKind = FilePageCountKind.Loading;
            UpdatePageCountText(item);
        }

        _pageCountCancellation = new CancellationTokenSource();
        var owner = _pageCountCancellation;
        var items = _pdfFiles.ToArray();
        _ = LoadPageCountsAsync(items, owner);
    }

    private async Task LoadPageCountsAsync(
        IReadOnlyList<PdfFileItem> items,
        CancellationTokenSource owner)
    {
        using var concurrency = new SemaphoreSlim(4);

        try
        {
            var tasks = items.Select(async item =>
            {
                await concurrency.WaitAsync(owner.Token).ConfigureAwait(false);
                try
                {
                    FilePageCountResult result;
                    try
                    {
                        result = await FilePageCountService
                            .GetPageCountAsync(item.FullPath, owner.Token)
                            .ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch
                    {
                        result = FilePageCountResult.Unavailable;
                    }

                    return (Item: item, Result: result);
                }
                finally
                {
                    concurrency.Release();
                }
            });

            var results = await Task.WhenAll(tasks);
            if (owner.IsCancellationRequested || !ReferenceEquals(owner, _pageCountCancellation))
            {
                return;
            }

            foreach (var (item, result) in results)
            {
                if (!_pdfFiles.Contains(item))
                {
                    continue;
                }

                item.PageCount = result.Count;
                item.PageCountKind = result.Kind;
                UpdatePageCountText(item);
            }

            UpdateSelectionSummary();
        }
        catch (OperationCanceledException)
        {
            // Klasör veya filtre değişti; eski sonuçlar yeni listeye uygulanmaz.
        }
        finally
        {
            if (ReferenceEquals(owner, _pageCountCancellation))
            {
                _pageCountCancellation = null;
            }

            owner.Dispose();
        }
    }

    private void CancelPageCountLoading()
    {
        var cancellation = _pageCountCancellation;
        _pageCountCancellation = null;
        cancellation?.Cancel();
    }

    private void UpdatePageCountText(PdfFileItem item)
    {
        item.PageCountText = item.PageCountKind switch
        {
            FilePageCountKind.Known when item.PageCount is > 0 =>
                item.PageCount.Value.ToString(System.Globalization.CultureInfo.CurrentCulture),
            FilePageCountKind.AtPrintTime => T("PageCountAtPrint"),
            FilePageCountKind.Unavailable => T("PageCountUnavailable"),
            _ => T("PageCountLoading"),
        };
    }

    private void RebuildFileTypeFilters(string sourceKey, IReadOnlyCollection<string> paths)
    {
        var preserveSelection = string.Equals(_loadedFolder, sourceKey, StringComparison.OrdinalIgnoreCase);
        var previouslySelected = preserveSelection
            ? _fileTypeCheckBoxes
                .Where(pair => pair.Value.Checked)
                .Select(pair => pair.Key)
                .ToHashSet(StringComparer.OrdinalIgnoreCase)
            : [];

        var groups = paths
            .GroupBy(path => Path.GetExtension(path).ToLowerInvariant(), StringComparer.OrdinalIgnoreCase)
            .Select(group => new FileTypeOption(group.Key, group.Count()))
            .OrderBy(option => option.Extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase) ? 0 : 1)
            .ThenBy(option => option.Extension, StringComparer.CurrentCultureIgnoreCase)
            .ToArray();

        _updatingFileTypes = true;
        try
        {
            _fileTypesPanel.Controls.Clear();
            _fileTypeCheckBoxes.Clear();

            foreach (var option in groups)
            {
                var checkBox = new CheckBox
                {
                    AutoSize = true,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI Semibold", 8.5F),
                    ForeColor = PremiumPalette.Text,
                    Margin = new Padding(0, 4, 14, 0),
                    Tag = option,
                    Checked = preserveSelection
                        ? previouslySelected.Contains(option.Extension)
                        : option.Extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase),
                };
                checkBox.CheckedChanged += FileTypeCheckBoxOnCheckedChanged;
                _fileTypeCheckBoxes.Add(option.Extension, checkBox);
                _fileTypesPanel.Controls.Add(checkBox);
            }

            if (groups.Length > 0 && !_fileTypeCheckBoxes.Values.Any(checkBox => checkBox.Checked))
            {
                _fileTypeCheckBoxes[groups[0].Extension].Checked = true;
            }

            _loadedFolder = sourceKey;
            UpdateFileTypeTexts();
        }
        finally
        {
            _updatingFileTypes = false;
        }
    }

    private void ClearFileTypeFilters()
    {
        _updatingFileTypes = true;
        try
        {
            _fileTypesPanel.Controls.Clear();
            _fileTypeCheckBoxes.Clear();
            _loadedFolder = null;
            UpdateFileTypeTexts();
        }
        finally
        {
            _updatingFileTypes = false;
        }
    }

    private void UpdateFileTypeTexts()
    {
        foreach (var checkBox in _fileTypeCheckBoxes.Values)
        {
            if (checkBox.Tag is FileTypeOption option)
            {
                checkBox.Text = $"{option.Extension.TrimStart('.').ToUpperInvariant()} ({option.Count})";
            }
        }

        if (_fileTypeCheckBoxes.Count > 0)
        {
            return;
        }

        _fileTypesPanel.Controls.Clear();
        _fileTypesPanel.Controls.Add(new Label
        {
            AutoSize = true,
            Text = T("NoFileTypes"),
            Font = new Font("Segoe UI", 8.5F),
            ForeColor = PremiumPalette.Muted,
            Margin = new Padding(0, 6, 0, 0),
        });
    }

    private void FileTypeCheckBoxOnCheckedChanged(object? sender, EventArgs e)
    {
        if (!_updatingFileTypes)
        {
            ApplyFileTypeSelection();
        }
    }

    private HashSet<string> SelectedFileTypeExtensions() =>
        _fileTypeCheckBoxes
            .Where(pair => pair.Value.Checked)
            .Select(pair => pair.Key)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

    private void ApplyFileTypeSelection()
    {
        if (_printCancellation is not null)
        {
            return;
        }

        var selectedExtensions = SelectedFileTypeExtensions();
        foreach (var item in _pdfFiles)
        {
            var included = selectedExtensions.Contains(Path.GetExtension(item.FullPath));
            item.IsSelected = included;
            SetItemState(
                item,
                !included
                    ? PdfPrintState.NotSelected
                    : IsItemPrintable(item)
                        ? PdfPrintState.Waiting
                        : PdfPrintState.Unavailable);
        }

        _filesGrid.Invalidate();
        UpdateSelectionSummary();
        UpdatePrintButtonState();
    }

    private void FilesGridOnCellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 ||
            _filesGrid.Rows[e.RowIndex].DataBoundItem is not PdfFileItem item ||
            e.CellStyle is null)
        {
            return;
        }

        if (item.IsSelected)
        {
            return;
        }

        e.CellStyle.ForeColor = PremiumPalette.Muted;
        e.CellStyle.SelectionForeColor = PremiumPalette.Muted;
    }

    private void ShowHelp()
    {
        using var dialog = new HelpDialog(
            T("HelpTitle"),
            T("HelpIntro"),
            T("HelpClose"),
            T("HelpFooter"),
            [
                (T("HelpFolderTitle"), T("HelpFolderBody")),
                (T("HelpPageTitle"), T("HelpPageBody")),
                (T("HelpStatsTitle"), T("HelpStatsBody")),
                (T("HelpSeparatorTitle"), T("HelpSeparatorBody")),
                (T("HelpPrintTitle"), T("HelpPrintBody")),
                (T("HelpFormatsTitle"), T("HelpFormatsBody")),
            ]);
        dialog.ShowDialog(this);
    }

    private void LoadPrinters()
    {
        if (_printCancellation is not null)
        {
            return;
        }

        var previousSelection = _printersComboBox.SelectedItem?.ToString();
        _printersComboBox.Items.Clear();

        try
        {
            foreach (string printerName in PrinterSettings.InstalledPrinters)
            {
                _printersComboBox.Items.Add(printerName);
            }

            var defaultPrinter = new PrinterSettings().PrinterName;
            var preferred = previousSelection ?? defaultPrinter;
            var preferredIndex = _printersComboBox.FindStringExact(preferred);

            if (preferredIndex >= 0)
            {
                _printersComboBox.SelectedIndex = preferredIndex;
            }
            else if (_printersComboBox.Items.Count > 0)
            {
                _printersComboBox.SelectedIndex = 0;
            }

            if (_printersComboBox.Items.Count == 0)
            {
                _statusLabel.Text = T("NoPrinters");
            }

            UpdatePrinterDropDownWidth();
        }
        catch (Win32Exception ex)
        {
            ShowError(T("ReadPrintersFailed"), ex);
        }

        UpdatePrintButtonState();
    }

    private void UpdatePrinterDropDownWidth()
    {
        var widestItem = _printersComboBox.Items
            .Cast<object>()
            .Select(item => TextRenderer.MeasureText(item.ToString(), _printersComboBox.Font).Width)
            .DefaultIfEmpty(_printersComboBox.Width)
            .Max();
        _printersComboBox.DropDownWidth = Math.Max(
            _printersComboBox.Width,
            Math.Min(900, widestItem + SystemInformation.VerticalScrollBarWidth + 24));
    }

    private BatchPrintOptions? GetPrintOptions(string printerName, bool duplex)
    {
        var initial = new BatchPrintOptions(
            printerName,
            duplex,
            Copies: 1,
            Collate: false,
            Color: true,
            Landscape: false,
            PageWidth: 0,
            PageHeight: 0);

        if (!_showPrinterSettingsCheckBox.Checked)
        {
            return initial;
        }

        using var document = new PrintDocument();
        document.PrinterSettings.PrinterName = printerName;
        document.PrinterSettings.Copies = 1;
        if (document.PrinterSettings.CanDuplex)
        {
            document.PrinterSettings.Duplex = duplex
                ? Duplex.Vertical
                : Duplex.Simplex;
        }

        using var dialog = new PrintDialog
        {
            Document = document,
            UseEXDialog = true,
            AllowCurrentPage = false,
            AllowSelection = false,
            AllowSomePages = false,
            AllowPrintToFile = false,
            PrintToFile = false,
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return null;
        }

        var printerSettings = document.PrinterSettings;
        var pageSettings = document.DefaultPageSettings;
        var paperSize = pageSettings.PaperSize;
        var chosenPrinterIndex = _printersComboBox.FindStringExact(printerSettings.PrinterName);
        if (chosenPrinterIndex >= 0)
        {
            _printersComboBox.SelectedIndex = chosenPrinterIndex;
        }

        var chosenDuplex = printerSettings.CanDuplex &&
                           printerSettings.Duplex is Duplex.Horizontal or Duplex.Vertical;
        _duplexCheckBox.Checked = chosenDuplex;

        return new BatchPrintOptions(
            printerSettings.PrinterName,
            chosenDuplex,
            Math.Clamp((int)printerSettings.Copies, 1, 999),
            printerSettings.Collate,
            pageSettings.Color,
            pageSettings.Landscape,
            paperSize.Width > 0 ? paperSize.Width / 100D : 0,
            paperSize.Height > 0 ? paperSize.Height / 100D : 0);
    }

    private async Task PrintAllAsync()
    {
        _filesGrid.EndEdit();
        var printQueue = BuildPrintQueue();
        if (!_webViewReady || printQueue.Count == 0 || _printersComboBox.SelectedItem is not string printerName)
        {
            return;
        }

        var addSeparatorPages = _addSeparatorPageCheckBox.Checked;

        var printOptions = GetPrintOptions(printerName, _duplexCheckBox.Checked);
        if (printOptions is null)
        {
            return;
        }

        printerName = printOptions.PrinterName;
        var selectedPrinter = new PrinterSettings { PrinterName = printerName };
        if (!selectedPrinter.IsValid)
        {
            MessageBox.Show(
                this,
                T("PrinterInvalidMessage", printerName),
                T("PrinterInvalidTitle"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        var answer = MessageBox.Show(
            this,
            T(
                "ConfirmPrintMessage",
                printQueue.Count,
                printerName,
                printOptions.Duplex ? T("On") : T("Off"),
                printOptions.Copies,
                addSeparatorPages ? T("On") : T("Off"),
                _skipFailedFileCheckBox.Checked ? T("On") : T("Off")),
            T("ConfirmPrintTitle"),
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question,
            MessageBoxDefaultButton.Button2);

        if (answer != DialogResult.Yes)
        {
            return;
        }

        _printCancellation = new CancellationTokenSource();
        var cancellationToken = _printCancellation.Token;
        SetPrintingState(isPrinting: true);

        var separatorCount = addSeparatorPages ? Math.Max(0, printQueue.Count - 1) : 0;
        var totalJobs = printQueue.Count + separatorCount;
        var completedJobs = 0;
        _progressBar.Maximum = Math.Max(1, totalJobs);
        _progressBar.Value = 0;

        foreach (var item in _pdfFiles)
        {
            SetItemState(
                item,
                !item.IsSelected
                    ? PdfPrintState.NotSelected
                    : printQueue.Contains(item)
                        ? PdfPrintState.Waiting
                        : PdfPrintState.Unavailable);
        }

        PdfFileItem? activeItem = null;
        var activePhase = string.Empty;
        var skippedFiles = 0;

        try
        {
            for (var index = 0; index < printQueue.Count; index++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var item = printQueue[index];
                activeItem = item;
                activePhase = T("ActiveFile", item.FileName);
                SetItemState(item, PdfPrintState.Printing);
                _filesGrid.FirstDisplayedScrollingRowIndex = Math.Max(0, _pdfFiles.IndexOf(item));
                _statusLabel.Text = T("PrintingFile", index + 1, printQueue.Count, item.FileName);

                try
                {
                    await LoadPrintableFileAsync(item.FullPath, cancellationToken);
                    await PrintCurrentDocumentAsync(printOptions, activePhase, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex) when (_skipFailedFileCheckBox.Checked)
                {
                    SetItemState(item, PdfPrintState.Error);
                    TryAppendDiagnosticLog(T("SkippedFileLog", item.FileName), ex);
                    skippedFiles++;
                    completedJobs++;
                    _progressBar.Value = Math.Min(_progressBar.Maximum, completedJobs);
                    continue;
                }

                SetItemState(item, PdfPrintState.Sent);
                completedJobs++;
                _progressBar.Value = completedJobs;

                if (!addSeparatorPages || index >= printQueue.Count - 1)
                {
                    continue;
                }

                cancellationToken.ThrowIfCancellationRequested();
                SetItemState(item, PdfPrintState.SentWithSeparator);
                activePhase = T("ActiveSeparator", item.FileName);
                _statusLabel.Text = T("SendingSeparator", item.FileName);

                try
                {
                    await NavigateToHtmlAsync(BlankPageHtml, cancellationToken);
                    await PrintCurrentDocumentAsync(printOptions, activePhase, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex) when (_skipFailedFileCheckBox.Checked)
                {
                    SetItemState(item, PdfPrintState.Sent);
                    TryAppendDiagnosticLog(T("SkippedSeparatorLog", item.FileName), ex);
                    continue;
                }

                completedJobs++;
                _progressBar.Value = completedJobs;
            }

            var sentCount = printQueue.Count - skippedFiles;
            if (skippedFiles > 0)
            {
                _statusLabel.Text = T("CompletedWithSkipsStatus", sentCount, skippedFiles);
                MessageBox.Show(
                    this,
                    T("CompletedWithSkipsMessage", sentCount, skippedFiles),
                    T("CompletedTitle"),
                    MessageBoxButtons.OK,
                    sentCount == 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
            }
            else
            {
                _statusLabel.Text = T("CompletedStatus", printQueue.Count, separatorCount);
                MessageBox.Show(
                    this,
                    T(addSeparatorPages && separatorCount > 0
                        ? "CompletedMessage"
                        : "CompletedMessageNoSeparator"),
                    T("CompletedTitle"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }
        catch (OperationCanceledException)
        {
            _statusLabel.Text = T("CancelledStatus");
            MarkWaitingItemsAs(PdfPrintState.Cancelled);
        }
        catch (Exception ex)
        {
            if (activeItem is not null)
            {
                SetItemState(activeItem, PdfPrintState.Error);
            }

            _statusLabel.Text = T("FailedStatus");
            ShowError(
                T("PrintFailedMessage") +
                (string.IsNullOrWhiteSpace(activePhase) ? string.Empty : $"\n\n{T("Phase", activePhase)}"),
                ex);
        }
        finally
        {
            _printCancellation.Dispose();
            _printCancellation = null;
            SetPrintingState(isPrinting: false);

            if (_webViewReady)
            {
                _webView.CoreWebView2.Navigate("about:blank");
            }
        }
    }

    private async Task LoadPrintableFileAsync(string filePath, CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(filePath);
        if (!SupportedExtensions.Contains(extension))
        {
            throw new NotSupportedException(T("UnsupportedFileType", extension));
        }

        if (TextExtensions.Contains(extension))
        {
            var content = await File.ReadAllTextAsync(filePath, cancellationToken);
            var title = WebUtility.HtmlEncode(Path.GetFileName(filePath));
            var encodedContent = WebUtility.HtmlEncode(content);
            var html = $$"""
                <!doctype html>
                <html>
                <head>
                  <meta charset="utf-8">
                  <title>{{title}}</title>
                  <style>
                    @page { margin: 15mm; }
                    body { margin: 0; color: #111827; background: white; }
                    pre { margin: 0; white-space: pre-wrap; overflow-wrap: anywhere; font: 10pt Consolas, monospace; }
                  </style>
                </head>
                <body><pre>{{encodedContent}}</pre></body>
                </html>
                """;
            await NavigateToHtmlAsync(html, cancellationToken, "FileOpenFailed");
            return;
        }

        await NavigateAsync(
            new Uri(filePath),
            isPdf: extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase),
            cancellationToken);
    }

    private async Task NavigateAsync(
        Uri fileUri,
        bool isPdf,
        CancellationToken cancellationToken)
    {
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        void OnNavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            _webView.CoreWebView2.NavigationCompleted -= OnNavigationCompleted;

            if (args.IsSuccess)
            {
                completion.TrySetResult();
            }
            else
            {
                completion.TrySetException(new IOException(
                    T("FileOpenFailed", args.WebErrorStatus)));
            }
        }

        _webView.CoreWebView2.NavigationCompleted += OnNavigationCompleted;
        using var registration = cancellationToken.Register(() =>
        {
            _webView.CoreWebView2.NavigationCompleted -= OnNavigationCompleted;
            completion.TrySetCanceled(cancellationToken);
        });

        _webView.CoreWebView2.Navigate(fileUri.AbsoluteUri);
        await completion.Task;
        await WaitForDocumentReadyAsync(isPdf, cancellationToken);
    }

    private async Task NavigateToHtmlAsync(
        string html,
        CancellationToken cancellationToken,
        string failureMessageKey = "SeparatorFailed")
    {
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        void OnNavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            _webView.CoreWebView2.NavigationCompleted -= OnNavigationCompleted;

            if (args.IsSuccess)
            {
                completion.TrySetResult();
            }
            else
            {
                completion.TrySetException(new IOException(
                    T(failureMessageKey, args.WebErrorStatus)));
            }
        }

        _webView.CoreWebView2.NavigationCompleted += OnNavigationCompleted;
        using var registration = cancellationToken.Register(() =>
        {
            _webView.CoreWebView2.NavigationCompleted -= OnNavigationCompleted;
            completion.TrySetCanceled(cancellationToken);
        });

        _webView.NavigateToString(html);
        await completion.Task;
        await WaitForDocumentReadyAsync(isPdf: false, cancellationToken);
    }

    private async Task WaitForDocumentReadyAsync(bool isPdf, CancellationToken cancellationToken)
    {
        // NavigationCompleted, dahili PDF görüntüleyicinin bütün sayfaları baskıya
        // hazırladığı anlamına gelmeyebilir. Önce DOM'u, ardından PDF işleyicisini bekle.
        for (var attempt = 0; attempt < 20; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var readyState = await _webView.CoreWebView2.ExecuteScriptAsync("document.readyState");
                if (readyState.Contains("complete", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
            }
            catch (Exception ex) when (ex is COMException or InvalidOperationException)
            {
                // Bazı WebView2 sürümleri dahili PDF eklentisinde betik çalıştırmaya
                // izin vermez. Sabit hazırlık beklemesi yine uygulanır.
                break;
            }

            await Task.Delay(100, cancellationToken);
        }

        await Task.Delay(isPdf ? PdfViewerReadyDelay : TimeSpan.FromMilliseconds(200), cancellationToken);
    }

    private async Task PrintCurrentDocumentAsync(
        BatchPrintOptions printOptions,
        string documentDescription,
        CancellationToken cancellationToken)
    {
        const int maximumAttempts = 3;
        Exception? lastException = null;

        for (var attempt = 1; attempt <= maximumAttempts; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                // Sürücü uyumsuzluğunu azaltmak için yalnızca gerekli ayarlar
                // gönderilir. Kenar boşluğu, ölçek ve kâğıt sürücü varsayılanındadır.
                var settings = _webView.CoreWebView2.Environment.CreatePrintSettings();
                settings.PrinterName = printOptions.PrinterName;
                settings.ShouldPrintBackgrounds = true;
                settings.ShouldPrintHeaderAndFooter = false;
                settings.Duplex = printOptions.Duplex
                    ? CoreWebView2PrintDuplex.TwoSidedLongEdge
                    : CoreWebView2PrintDuplex.OneSided;
                settings.Copies = printOptions.Copies;
                settings.Collation = printOptions.Collate
                    ? CoreWebView2PrintCollation.Collated
                    : CoreWebView2PrintCollation.Uncollated;
                settings.ColorMode = printOptions.Color
                    ? CoreWebView2PrintColorMode.Color
                    : CoreWebView2PrintColorMode.Grayscale;
                settings.Orientation = printOptions.Landscape
                    ? CoreWebView2PrintOrientation.Landscape
                    : CoreWebView2PrintOrientation.Portrait;

                if (printOptions.PageWidth > 0 && printOptions.PageHeight > 0)
                {
                    settings.MediaSize = CoreWebView2PrintMediaSize.Custom;
                    settings.PageWidth = printOptions.PageWidth;
                    settings.PageHeight = printOptions.PageHeight;
                }

                var status = await _webView.CoreWebView2.PrintAsync(settings);

                if (status == CoreWebView2PrintStatus.Succeeded)
                {
                    return;
                }

                if (status == CoreWebView2PrintStatus.PrinterUnavailable)
                {
                    throw new ApplicationException(
                        T("PrinterUnavailableException", printOptions.PrinterName));
                }

                lastException = new InvalidOperationException(
                    T("WebViewPrintStatus", status));
            }
            catch (ArgumentException ex)
            {
                throw new InvalidOperationException(
                    T("PrinterRejectedSettings", printOptions.PrinterName),
                    ex);
            }
            catch (ApplicationException)
            {
                throw;
            }
            catch (Exception ex) when (ex is ExternalException or InvalidOperationException)
            {
                lastException = ex;
            }

            if (attempt >= maximumAttempts)
            {
                break;
            }

            TryAppendDiagnosticLog(
                T("RetryLog", attempt, maximumAttempts, documentDescription),
                lastException);
            _statusLabel.Text = T(
                "RetryStatus",
                documentDescription,
                attempt + 1,
                maximumAttempts);
            await Task.Delay(PrintRetryDelay * attempt, cancellationToken);
        }

        throw new InvalidOperationException(
            T("ThreeAttempts", documentDescription),
            lastException);
    }

    private void CancelPrinting()
    {
        if (_printCancellation is null)
        {
            return;
        }

        _cancelButton.Enabled = false;
        _statusLabel.Text = T("StopRequested");
        _printCancellation.Cancel();
    }

    private void MarkWaitingItemsAs(PdfPrintState state)
    {
        foreach (var item in _pdfFiles.Where(item =>
                     item.State is PdfPrintState.Waiting or PdfPrintState.Printing))
        {
            SetItemState(item, state);
        }
    }

    private void SetPrintingState(bool isPrinting)
    {
        _selectFolderButton.Enabled = !isPrinting;
        _refreshFilesButton.Enabled = !isPrinting;
        _includeSubfoldersCheckBox.Enabled = !isPrinting;
        _filesGrid.ReadOnly = isPrinting;
        _printersComboBox.Enabled = !isPrinting;
        _duplexCheckBox.Enabled = !isPrinting;
        _showPrinterSettingsCheckBox.Enabled = !isPrinting;
        _addSeparatorPageCheckBox.Enabled = !isPrinting;
        _skipFailedFileCheckBox.Enabled = !isPrinting;
        _refreshPrintersButton.Enabled = !isPrinting;
        _printButton.Enabled = !isPrinting && CanPrint();
        _cancelButton.Enabled = isPrinting;
    }

    private void UpdatePrintButtonState() => _printButton.Enabled = _printCancellation is null && CanPrint();

    private void UpdateSelectionSummary()
    {
        var selectedCount = _pdfFiles.Count(item => item.IsSelected);
        var printQueue = BuildPrintQueue();
        var knownPageCount = printQueue
            .Where(item => item.PageCount is > 0)
            .Sum(item => item.PageCount!.Value);
        var unknownPageCount = printQueue.Count(item => item.PageCount is null);
        var separatorPageCount = _addSeparatorPageCheckBox.Checked
            ? Math.Max(0, printQueue.Count - 1)
            : 0;

        _totalFilesLabel.Text = T("TotalFiles", _pdfFiles.Count);
        _selectedFilesLabel.Text = T("SelectedFiles", selectedCount);
        _printQueueFilesLabel.Text = T("PrintableFiles", printQueue.Count);
        _knownPagesLabel.Text = unknownPageCount > 0
            ? T("KnownPagesPartial", knownPageCount, unknownPageCount)
            : T("KnownPages", knownPageCount);
        _separatorPagesLabel.Text = T("SeparatorPages", separatorPageCount);
    }

    private List<PdfFileItem> BuildPrintQueue() =>
        _pdfFiles.Where(item => item.IsSelected && IsItemPrintable(item)).ToList();

    private static bool IsItemPrintable(PdfFileItem item) =>
        File.Exists(item.FullPath) &&
        SupportedExtensions.Contains(Path.GetExtension(item.FullPath));

    private bool CanPrint() =>
        _webViewReady &&
        BuildPrintQueue().Count > 0 &&
        _printersComboBox.SelectedIndex >= 0;

    private void OnFormClosing(object? sender, FormClosingEventArgs e)
    {
        CancelPageCountLoading();

        if (_printCancellation is null)
        {
            return;
        }

        var answer = MessageBox.Show(
            this,
            T("CloseMessage"),
            T("CloseTitle"),
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2);

        if (answer == DialogResult.No)
        {
            e.Cancel = true;
            return;
        }

        _printCancellation.Cancel();
    }

    private void ShowError(string message, Exception exception)
    {
        var rootException = exception.GetBaseException();
        var logPath = TryAppendDiagnosticLog(message, exception);
        var technicalDetail =
            $"{rootException.GetType().Name} (0x{rootException.HResult:X8}): {rootException.Message}";

        MessageBox.Show(
            this,
            $"{message}\n\n{T("TechnicalDetail", technicalDetail)}" +
            (logPath is null ? string.Empty : $"\n\n{T("DiagnosticLog", logPath)}"),
            T("ErrorTitle"),
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }

    private void InitializeComponent()
    {
        SuspendLayout();
        // 
        // MainForm
        // 
        Size = new Size(1368, 882);
        Name = "MainForm";
        ResumeLayout(false);

    }

    private static string? TryAppendDiagnosticLog(string context, Exception? exception)
    {
        try
        {
            var logDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "FilePrintStudio",
                "Logs");
            Directory.CreateDirectory(logDirectory);

            var logPath = Path.Combine(logDirectory, $"yazdirma-{DateTime.Now:yyyy-MM-dd}.log");
            var builder = new StringBuilder()
                .AppendLine(new string('-', 80))
                .AppendLine($"Tarih: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff zzz}")
                .AppendLine($"Açıklama: {context}");

            var currentException = exception;
            var level = 0;
            while (currentException is not null)
            {
                builder
                    .AppendLine($"Hata[{level}]: {currentException.GetType().FullName}")
                    .AppendLine($"HRESULT[{level}]: 0x{currentException.HResult:X8}")
                    .AppendLine($"Mesaj[{level}]: {currentException.Message}")
                    .AppendLine($"Stack[{level}]: {currentException.StackTrace}");

                currentException = currentException.InnerException;
                level++;
            }

            File.AppendAllText(logPath, builder.ToString(), Encoding.UTF8);
            return logPath;
        }
        catch
        {
            // Tanılama kaydının yazılamaması asıl hatayı gölgelememeli.
            return null;
        }
    }
}
