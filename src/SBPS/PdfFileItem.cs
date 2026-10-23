using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SBPS;

internal enum PdfPrintState
{
    NotSelected,
    Unavailable,
    Waiting,
    Printing,
    Sent,
    SentWithSeparator,
    Cancelled,
    Error,
}

internal sealed class PdfFileItem : INotifyPropertyChanged
{
    private bool _isSelected = true;
    private int? _pageCount;
    private FilePageCountKind _pageCountKind = FilePageCountKind.Loading;
    private string _pageCountText = string.Empty;
    private string _status = string.Empty;

    public required int Order { get; init; }
    public required string FullPath { get; init; }
    public required long SizeBytes { get; init; }
    public PdfPrintState State { get; set; } = PdfPrintState.Waiting;

    public string FileName => Path.GetFileName(FullPath);
    public string FileType => Path.GetExtension(FullPath).TrimStart('.').ToUpperInvariant();
    public string DirectoryName => Path.GetDirectoryName(FullPath) ?? string.Empty;
    public string SizeValue => FormatSize(SizeBytes).Value;

    public string SizeUnit => FormatSize(SizeBytes).Unit;

    public int? PageCount
    {
        get => _pageCount;
        set
        {
            if (_pageCount == value)
            {
                return;
            }

            _pageCount = value;
            OnPropertyChanged();
        }
    }

    public FilePageCountKind PageCountKind
    {
        get => _pageCountKind;
        set
        {
            if (_pageCountKind == value)
            {
                return;
            }

            _pageCountKind = value;
            OnPropertyChanged();
        }
    }

    public string PageCountText
    {
        get => _pageCountText;
        set
        {
            if (_pageCountText == value)
            {
                return;
            }

            _pageCountText = value;
            OnPropertyChanged();
        }
    }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value)
            {
                return;
            }

            _isSelected = value;
            OnPropertyChanged();
        }
    }

    public string Status
    {
        get => _status;
        set
        {
            if (_status == value)
            {
                return;
            }

            _status = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private static (string Value, string Unit) FormatSize(long bytes)
    {
        string[] units = ["B", "KB", "MB", "GB"];
        var value = (double)bytes;
        var unitIndex = 0;

        while (value >= 1024 && unitIndex < units.Length - 1)
        {
            value /= 1024;
            unitIndex++;
        }

        return ($"{value:0.##}", units[unitIndex]);
    }
}
