using System;
using System.Xml.Linq;

namespace Ravlyk.Adopted.OpenXmlPackaging {
    public class Constants {
        public static XNamespace MainXNamespace = @"http://schemas.openxmlformats.org/spreadsheetml/2006/main";
        public static XNamespace RelationshipXNamespace = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships";
        public static XNamespace McIgnorableXNamespace = @"http://schemas.openxmlformats.org/markup-compatibility/2006";
        public static XNamespace X14AcXNamespace = @"http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac";
        public static Uri WorkbookUri = new Uri("/xl/workbook.xml", UriKind.Relative);
        public static Uri StylesUri = new Uri("/xl/styles.xml", UriKind.Relative);

        public const string MainNamespace = @"http://schemas.openxmlformats.org/spreadsheetml/2006/main";
        public const string RelationshipNamespace = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships";
        public const string McIgnorableNamespace = @"http://schemas.openxmlformats.org/markup-compatibility/2006";
        public const string X14AcNamespace = @"http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac";

        public const string SheetUriFormatPath = "/xl/worksheets/sheet{0}.xml";
        public const string WorksheetContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml";

        public const int MaxRowNumber = 1048576;
        public const int MaxColumnNumber = 1048576;
        public const string MaxColumnName = "XFD";
    }
}
