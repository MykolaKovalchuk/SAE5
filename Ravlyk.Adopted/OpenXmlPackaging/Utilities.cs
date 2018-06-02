using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using D = System.Drawing;

namespace Ravlyk.Adopted.OpenXmlPackaging {
    public class Utilities {
        public static List<string> GetColumnNames(string lastColumn = Constants.MaxColumnName) {
            List<string> columns = new List<string>();
            string[] columnChars = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

            foreach (string column in columnChars) {
                columns.Add(column);
                if ((column.Equals(lastColumn, StringComparison.OrdinalIgnoreCase))) {
                    return columns;
                }
            }

            for (int i = 0; i <= columnChars.Length - 1; i++) {
                for (int j = 0; j <= columnChars.Length - 1; j++) {
                    columns.Add(string.Format("{0}{1}", columnChars[i], columnChars[j]));

                    if ((columns[columns.Count - 1].Equals(lastColumn, StringComparison.OrdinalIgnoreCase))) {
                        return columns;
                    }
                }
            }

            for (int i = 26; i <= columns.Count - 1; i++) {
                for (int j = 0; j <= columnChars.Length - 1; j++) {
                    columns.Add(string.Format("{0}{1}", columns[i], columns[j]));

                    if ((columns[columns.Count - 1].Equals(lastColumn, StringComparison.OrdinalIgnoreCase))) {
                        return columns;
                    }
                }
            }

            return columns;
        }

        public static string GetColumnName(string cellName) {
            if (!String.IsNullOrWhiteSpace(cellName)) {
                Match match = Regex.Match(cellName, "[A-Za-z]+");
                return match.Value;
            }
            return null;
        }

        public static string BuildReference(int row, int column) {
            if (row < 1 || row > Constants.MaxRowNumber) {
                throw new ArgumentException("Invalid Row");
            }

            if (column < 1 || column > Constants.MaxColumnNumber) {
                throw new ArgumentException("Invalid Column");
            }
            return string.Format("{0}{1}", ColumnNames[column - 1], row);
        }

        public static int GetRowIndex(string cellName) {
            Match match = Regex.Match(cellName, "\\d+");

            int rowIndex = 0;
            return int.TryParse(match.Value, out rowIndex) ? rowIndex : -1;
        }

        public static void SetAttribute(XElement element, string attribute, object value) {
            var attributeElement = element.Attribute(attribute);
            if (attributeElement == null) {
                attributeElement = new XAttribute(attribute, String.Empty);
                element.Add(attributeElement);
            }
            attributeElement.Value = value != null ? value.ToString() : null;
        }

        public static string GetColorInHex(D.Color color) {
            return String.Format("{0:X2}{1:X2}{2:X2}{3:X2}", color.A, color.R, color.G, color.B);
        }

        private static List<String> _columnNames;

        public static List<String> ColumnNames {
            get { return _columnNames ?? (_columnNames = GetColumnNames()); }
        }

        public static void ValidateReference(string reference) {
            if (!String.IsNullOrWhiteSpace(reference)) {
                string columnName = Utilities.GetColumnName(reference);
                int rowIndex = Utilities.GetRowIndex(reference);

                if (ColumnNames.IndexOf(columnName.ToUpper()) == -1) {
                    throw new ArgumentException("Invalid Column Name");
                }

                if (rowIndex < 0 || rowIndex > Constants.MaxRowNumber) {
                    throw new ArgumentException("Invalid Row Index");
                }

                if (!Regex.IsMatch(reference, "^[A-Za-z]{1,3}\\d+$")) {
                    throw new ArgumentException("CellReference is not in proper format");
                }
            }
        }

        public static double GetColumnWidth(string value, string font, float fontSize) {
            D.Graphics g = D.Graphics.FromImage(new D.Bitmap(200, 200));
            return (g.MeasureString(value, new D.Font(font, fontSize)).Width + 5)/7.2;            
        }
    }
}
