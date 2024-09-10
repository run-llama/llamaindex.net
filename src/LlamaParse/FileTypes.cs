using System;
using System.Collections.Generic;
using System.IO;

namespace LlamaParse;

internal static class FileTypes
{
    private static readonly HashSet<string> _supportedFileTypes =
    [
        ".pdf",
        // document and presentations
        ".602",
        ".abw",
        ".cgm",
        ".cwk",
        ".doc",
        ".docx",
        ".docm",
        ".dot",
        ".dotm",
        ".hwp",
        ".key",
        ".lwp",
        ".mw",
        ".mcw",
        ".pages",
        ".pbd",
        ".ppt",
        ".pptm",
        ".pptx",
        ".pot",
        ".potm",
        ".potx",
        ".rtf",
        ".sda",
        ".sdd",
        ".sdp",
        ".sdw",
        ".sgl",
        ".sti",
        ".sxi",
        ".sxw",
        ".stw",
        ".sxg",
        ".txt",
        ".uof",
        ".uop",
        ".uot",
        ".vor",
        ".wpd",
        ".wps",
        ".xml",
        ".zabw",
        ".epub",
        // images
        ".jpg",
        ".jpeg",
        ".png",
        ".gif",
        ".bmp",
        ".svg",
        ".tiff",
        ".webp",
        // web
        ".htm",
        ".html",
        // spreadsheets
        ".xlsx",
        ".xls",
        ".xlsm",
        ".xlsb",
        ".xlw",
        ".csv",
        ".dif",
        ".sylk",
        ".slk",
        ".prn",
        ".numbers",
        ".et",
        ".ods",
        ".fods",
        ".uos1",
        ".uos2",
        ".dbf",
        ".wk1",
        ".wk2",
        ".wk3",
        ".wk4",
        ".wks",
        ".123",
        ".wq1",
        ".wq2",
        ".wb1",
        ".wb2",
        ".wb3",
        ".qpw",
        ".xlr",
        ".eth",
        ".tsv"
    ];

    public static bool IsSupported(FileInfo fileInfo)
    {
        var fileInfoName = fileInfo.Name;

        var extension = Path.GetExtension(fileInfoName);
        return IsSupported(extension);
    }

    public static bool IsSupported(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        return _supportedFileTypes.Contains(extension);
    }

    public static string GetMimeType(string fileName)
    {
        var extension = Path.GetExtension(fileName);

        if (!IsSupported(extension))
        {
            throw new ArgumentOutOfRangeException(nameof(fileName), $"Extension {extension} is not supported");
        }

        return extension switch
        {
            ".pdf" => "application/pdf",
            // Documents and Presentations
            ".602" => "application/x-t602",
            ".abw" => "application/x-abiword",
            ".cgm" => "image/cgm",
            ".cwk" => "application/x-cwk",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".docm" => "application/vnd.ms-word.document.macroEnabled.12",
            ".dot" => "application/msword",
            ".dotm" => "application/vnd.ms-word.template.macroEnabled.12",
            ".dotx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.template",
            ".hwp" => "application/x-hwp",
            ".key" => "application/x-iwork-keynote-sffkey",
            ".lwp" => "application/vnd.lotus-wordpro",
            ".mw" => "application/macwriteii",
            ".mcw" => "application/macwriteii",
            ".pages" => "application/x-iwork-pages-sffpages",
            ".pbd" => "application/x-pagemaker",
            ".ppt" => "application/vnd.ms-powerpoint",
            ".pptm" => "application/vnd.ms-powerpoint.presentation.macroEnabled.12",
            ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            ".pot" => "application/vnd.ms-powerpoint",
            ".potm" => "application/vnd.ms-powerpoint.template.macroEnabled.12",
            ".potx" => "application/vnd.openxmlformats-officedocument.presentationml.template",
            ".rtf" => "application/rtf",
            ".sda" => "application/vnd.stardivision.draw",
            ".sdd" => "application/vnd.stardivision.impress",
            ".sdp" => "application/sdp",
            ".sdw" => "application/vnd.stardivision.writer",
            ".sgl" => "application/vnd.stardivision.writer",
            ".sti" => "application/vnd.sun.xml.impress.template",
            ".sxi" => "application/vnd.sun.xml.impress",
            ".sxw" => "application/vnd.sun.xml.writer",
            ".stw" => "application/vnd.sun.xml.writer.template",
            ".sxg" => "application/vnd.sun.xml.writer.global",
            ".txt" => "text/plain",
            ".uof" => "application/vnd.uoml+xml",
            ".uop" => "application/vnd.openofficeorg.presentation",
            ".uot" => "application/x-uo",
            ".vor" => "application/vnd.stardivision.writer",
            ".wpd" => "application/wordperfect",
            ".wps" => "application/vnd.ms-works",
            ".xml" => "application/xml",
            ".zabw" => "application/x-abiword",
            // Images
            ".epub" => "application/epub+zip",
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".svg" => "image/svg+xml",
            ".tiff" => "image/tiff",
            ".webp" => "image/webp",
            // Web
            ".htm" => "text/html",
            ".html" => "text/html",
            // Spreadsheets
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".xls" => "application/vnd.ms-excel",
            ".xlsm" => "application/vnd.ms-excel.sheet.macroEnabled.12",
            ".xlsb" => "application/vnd.ms-excel.sheet.binary.macroEnabled.12",
            ".xlw" => "application/vnd.ms-excel",
            ".csv" => "text/csv",
            ".dif" => "application/x-dif",
            ".sylk" => "text/vnd.sylk",
            ".slk" => "text/vnd.sylk",
            ".prn" => "application/x-prn",
            ".numbers" => "application/x-iwork-numbers-sffnumbers",
            ".et" => "application/vnd.ms-excel",
            ".ods" => "application/vnd.oasis.opendocument.spreadsheet",
            ".fods" => "application/vnd.oasis.opendocument.spreadsheet",
            ".uos1" => "application/vnd.uoml+xml",
            ".uos2" => "application/vnd.uoml+xml",
            ".dbf" => "application/vnd.dbf",
            ".wk1" => "application/vnd.lotus-1-2-3",
            ".wk2" => "application/vnd.lotus-1-2-3",
            ".wk3" => "application/vnd.lotus-1-2-3",
            ".wk4" => "application/vnd.lotus-1-2-3",
            ".wks" => "application/vnd.lotus-1-2-3",
            ".123" => "application/vnd.lotus-1-2-3",
            ".wq1" => "application/x-lotus",
            ".wq2" => "application/x-lotus",
            ".wb1" => "application/x-quattro-pro",
            ".wb2" => "application/x-quattro-pro",
            ".wb3" => "application/x-quattro-pro",
            ".qpw" => "application/x-quattro-pro",
            ".xlr" => "application/vnd.ms-works",
            ".eth" => "application/ethos",
            ".tsv" => "text/tab-separated-values",
            _ => throw new ArgumentOutOfRangeException(nameof(fileName), $"Extension {extension} is not supported")
        };
    }

    public static string GetMimeType(FileInfo fileInfo)
    {
        var fileName = fileInfo.Name;

        return GetMimeType(fileName);
    }
}
