using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace SBPS;

internal enum FilePageCountKind
{
    Loading,
    Known,
    AtPrintTime,
    Unavailable,
}

internal readonly record struct FilePageCountResult(FilePageCountKind Kind, int? Count = null)
{
    internal static FilePageCountResult Known(int count) =>
        new(FilePageCountKind.Known, count);

    internal static FilePageCountResult AtPrintTime =>
        new(FilePageCountKind.AtPrintTime);

    internal static FilePageCountResult Unavailable =>
        new(FilePageCountKind.Unavailable);
}

internal static partial class FilePageCountService
{
    private const long MaximumPdfInspectionBytes = 128L * 1024 * 1024;
    private const ushort VariantInt32 = 3;
    private const ushort VariantUInt32 = 19;
    private const uint BestEffortPropertyLookup = 0x40;

    private static readonly HashSet<string> SinglePageImageExtensions =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".jfif", ".png", ".bmp", ".gif", ".webp", ".svg", ".avif", ".ico", ".jxl",
        };

    private static readonly ConcurrentDictionary<PageCountCacheKey, FilePageCountResult> Cache = [];

    private static readonly PropertyKey DocumentPageCountProperty = new(
        new Guid("F29F85E0-4FF9-1068-AB91-08002B27B3D9"),
        14);

    internal static Task<FilePageCountResult> GetPageCountAsync(
        string filePath,
        CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var extension = Path.GetExtension(filePath);
            if (SinglePageImageExtensions.Contains(extension))
            {
                return Task.FromResult(FilePageCountResult.Known(1));
            }

            if (!extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(FilePageCountResult.AtPrintTime);
            }

            var file = new FileInfo(filePath);
            var cacheKey = new PageCountCacheKey(file.FullName, file.Length, file.LastWriteTimeUtc);
            if (Cache.TryGetValue(cacheKey, out var cachedResult))
            {
                return Task.FromResult(cachedResult);
            }

            return Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var result = ReadPdfPageCount(file, cancellationToken);
                Cache[cacheKey] = result;
                return result;
            }, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return Task.FromCanceled<FilePageCountResult>(cancellationToken);
        }
        catch (Exception ex) when (
            ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
        {
            return Task.FromResult(FilePageCountResult.Unavailable);
        }
    }

    private static FilePageCountResult ReadPdfPageCount(
        FileInfo file,
        CancellationToken cancellationToken)
    {
        try
        {
            if (file.Length is <= 0 or > MaximumPdfInspectionBytes)
            {
                return TryReadWindowsPageCount(file.FullName, out var shellCount)
                    ? FilePageCountResult.Known(shellCount)
                    : FilePageCountResult.Unavailable;
            }

            byte[] bytes;
            using (var stream = new FileStream(
                       file.FullName,
                       FileMode.Open,
                       FileAccess.Read,
                       FileShare.ReadWrite | FileShare.Delete))
            {
                bytes = new byte[stream.Length];
                stream.ReadExactly(bytes);
            }

            cancellationToken.ThrowIfCancellationRequested();
            var document = Encoding.Latin1.GetString(bytes);
            if (!document.AsSpan(0, Math.Min(document.Length, 1024)).Contains("%PDF-", StringComparison.Ordinal) ||
                PdfEncryptionRegex().IsMatch(document))
            {
                return FilePageCountResult.Unavailable;
            }

            var pageTreeCounts = PdfPagesNodeRegex()
                .Matches(document)
                .Select(match => ParsePageCount(match.Groups[1].Value))
                .Concat(PdfPagesNodeReverseRegex()
                    .Matches(document)
                    .Select(match => ParsePageCount(match.Groups[1].Value)))
                .Where(count => count is > 0 and <= 1_000_000)
                .ToArray();

            if (pageTreeCounts.Length > 0)
            {
                return FilePageCountResult.Known(pageTreeCounts.Max());
            }

            var directPageCount = PdfPageNodeRegex().Matches(document).Count;
            if (directPageCount > 0 && !PdfObjectStreamRegex().IsMatch(document))
            {
                return FilePageCountResult.Known(directPageCount);
            }

            return TryReadWindowsPageCount(file.FullName, out var windowsCount)
                ? FilePageCountResult.Known(windowsCount)
                : FilePageCountResult.Unavailable;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or COMException)
        {
            return FilePageCountResult.Unavailable;
        }
    }

    private static int ParsePageCount(string value) =>
        int.TryParse(value, out var count) ? count : 0;

    private static bool TryReadWindowsPageCount(string filePath, out int pageCount)
    {
        pageCount = 0;
        IPropertyStore? propertyStore = null;
        PropVariant value = default;

        try
        {
            var interfaceId = typeof(IPropertyStore).GUID;
            var result = SHGetPropertyStoreFromParsingName(
                filePath,
                IntPtr.Zero,
                BestEffortPropertyLookup,
                ref interfaceId,
                out propertyStore);
            if (result < 0 || propertyStore is null)
            {
                return false;
            }

            var propertyKey = DocumentPageCountProperty;
            result = propertyStore.GetValue(ref propertyKey, out value);
            if (result < 0)
            {
                return false;
            }

            pageCount = value.VariantType switch
            {
                VariantInt32 => value.Int32Value,
                VariantUInt32 when value.UInt32Value <= int.MaxValue => (int)value.UInt32Value,
                _ => 0,
            };
            return pageCount > 0;
        }
        finally
        {
            PropVariantClear(ref value);
            if (propertyStore is not null)
            {
                Marshal.ReleaseComObject(propertyStore);
            }
        }
    }

    [GeneratedRegex(@"/Encrypt\b", RegexOptions.CultureInvariant)]
    private static partial Regex PdfEncryptionRegex();

    [GeneratedRegex(
        @"/Type\s*/Pages\b(?:(?!endobj)[\s\S]){0,4096}?/Count\s+(\d+)",
        RegexOptions.CultureInvariant)]
    private static partial Regex PdfPagesNodeRegex();

    [GeneratedRegex(
        @"/Count\s+(\d+)(?:(?!endobj)[\s\S]){0,4096}?/Type\s*/Pages\b",
        RegexOptions.CultureInvariant)]
    private static partial Regex PdfPagesNodeReverseRegex();

    [GeneratedRegex(@"/Type\s*/Page(?!s)\b", RegexOptions.CultureInvariant)]
    private static partial Regex PdfPageNodeRegex();

    [GeneratedRegex(@"/Type\s*/ObjStm\b", RegexOptions.CultureInvariant)]
    private static partial Regex PdfObjectStreamRegex();

    [DllImport("shell32.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
    private static extern int SHGetPropertyStoreFromParsingName(
        string path,
        IntPtr bindingContext,
        uint flags,
        ref Guid interfaceId,
        [MarshalAs(UnmanagedType.Interface)] out IPropertyStore? propertyStore);

    [DllImport("ole32.dll")]
    private static extern int PropVariantClear(ref PropVariant value);

    [ComImport]
    [Guid("886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IPropertyStore
    {
        [PreserveSig]
        int GetCount(out uint propertyCount);

        [PreserveSig]
        int GetAt(uint propertyIndex, out PropertyKey propertyKey);

        [PreserveSig]
        int GetValue(ref PropertyKey propertyKey, out PropVariant value);

        [PreserveSig]
        int SetValue(ref PropertyKey propertyKey, ref PropVariant value);

        [PreserveSig]
        int Commit();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    private readonly struct PropertyKey(Guid formatId, uint propertyId)
    {
        internal readonly Guid FormatId = formatId;
        internal readonly uint PropertyId = propertyId;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct PropVariant
    {
        [FieldOffset(0)]
        internal ushort VariantType;

        [FieldOffset(8)]
        internal int Int32Value;

        [FieldOffset(8)]
        internal uint UInt32Value;
    }

    private readonly record struct PageCountCacheKey(
        string FullPath,
        long Length,
        DateTime LastWriteTimeUtc);
}
