using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Ravlyk.Adopted.OpenXmlPackaging {
    /// <summary>
    /// Represents <sheetData> element in the worksheet xml
    /// </summary>
    public class Cells : DataWriter {

        #region Private and Internal Members

        // Workbook's Stylesheet 
        private readonly Stylesheet _styleSheet;

        // All Rows
        private List<Row> Rows { get; set; }

        // The Worksheet
        private readonly Worksheet _worksheet;

        // All Cells present in the Worksheet
        internal Dictionary<int, List<Cell>> AllCells { get; set; }

        // All Colunms whose custom properties are set
        internal Columns Columns { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Cells"/> class.
        /// </summary>
        /// <param name="styleSheet">The style sheet.</param>
        /// <param name="worksheet">The Worksheet</param>
        internal Cells(Stylesheet styleSheet, Worksheet worksheet) {
            _styleSheet = styleSheet;
            _worksheet = worksheet;
            AllCells = new Dictionary<int, List<Cell>>();
            Columns = new Columns();
            Rows = new List<Row>();
        }

        #endregion

        #region Public Methods and Indexers
        
        /// <summary>
        /// Gets the <see cref="OpenXmlPackaging.Cell"/> with the specified row and column.
        /// </summary>
        public Cell this[int row, int column] {
            get {
                return GetCell(row, column);
            }
        }

        /// <summary>
        /// Gets the <see cref="OpenXmlPackaging.Cell"/> with the specified reference.
        /// </summary>
        public Cell this[string reference] {
            get {
                return GetCell(reference);
            }
        }

        /// <summary>
        /// Imports a data table
        /// </summary>
        /// <param name="table">The Data table</param>
        /// <param name="startFromReference">Start From Reference</param>
        /// <param name="includeColumnNames">Whether to include column names or not</param>
        public void ImportDataTable(DataTable table, string startFromReference, bool includeColumnNames) {
            if (table != null) {
                int startColumnIndex = Utilities.ColumnNames.IndexOf(Utilities.GetColumnName(startFromReference)) + 1;
                int startRowIndex = Utilities.GetRowIndex(startFromReference);

                int columnIndex = startColumnIndex, rowIndex = startRowIndex;

                // Write columns to Excel if includeColumnNames = true
                if (includeColumnNames) {
                    foreach (DataColumn column in table.Columns) {
                        Columns.ColumnsList.Add(new Column { Index = columnIndex });
                        AddCell(rowIndex, columnIndex++, column.Caption);
                    }
                    Rows.Add(new Row(rowIndex));
                }

                // Add all the data row values
                rowIndex++;
                columnIndex = startColumnIndex;
                foreach (DataRow row in table.Rows) {
                    Rows.Add(new Row(rowIndex));
                    for (var i = 0; i < table.Columns.Count; i++) {
                        AddCell(rowIndex, columnIndex++, row[i].ToString());
                    }
                    rowIndex++;
                    columnIndex = startColumnIndex;
                }

            } else {
                throw new ArgumentNullException("table");
            }
        }

        /// <summary>
        /// Creates the range.
        /// </summary>
        /// <param name="startRow">The start row.</param>
        /// <param name="startColumn">The start column.</param>
        /// <param name="numberOfRows">The number of rows.</param>
        /// <param name="numberOfColumns">The number of columns.</param>
        /// <returns></returns>
        public Range CreateRange(int startRow, int startColumn, int numberOfRows, int numberOfColumns) {
            return new Range(this, _worksheet) {
                StartRow = startRow,
                StartColumn = startColumn,
                RowCount = numberOfRows,
                ColumnsCount = numberOfColumns
            };
        }

        /// <summary>
        /// Creates the range.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <returns></returns>
        public Range CreateRange(string reference) {
            var references = reference.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (references.Length == 1 || references.Length == 2) {
                foreach (var t in references) {
                    Utilities.ValidateReference(t);
                }

                var first = references[0];

                var second = references.Length == 2 ? references[1] : first;

                return Range(first, second);
            }
            throw new ArgumentException("Invalid Reference");
        }

        /// <summary>
        /// Creates the range.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        public Range CreateRange(string from, string to) {
            Utilities.ValidateReference(from);
            Utilities.ValidateReference(to);
            return Range(from, to);
        } 

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates a range based on the first and second cell references
        /// </summary>
        /// <param name="first">The first cell reference.</param>
        /// <param name="second">The second cell reference.</param>
        /// <returns></returns>
        private Range Range(string first, string second) {
            // Swap the first and second references if the  firstRow (column) > secondRow (column)
            var firstCellRow = Utilities.GetRowIndex(first);

            var firstCellColumn = Utilities.ColumnNames.IndexOf(Utilities.GetColumnName(first)) + 1;

            var secondCellRow = Utilities.GetRowIndex(second);

            var secondCellColumn = Utilities.ColumnNames.IndexOf(Utilities.GetColumnName(second)) + 1;

            if (firstCellRow > secondCellRow) {
                Swap(ref firstCellRow, ref secondCellRow);
            }

            if (firstCellColumn > secondCellColumn) {
                Swap(ref firstCellColumn, ref secondCellColumn);
            }

            // Create and return a new Range
            return new Range(this, _worksheet) {
                StartRow = firstCellRow,
                StartColumn = firstCellColumn,
                RowCount = secondCellRow - firstCellRow,
                ColumnsCount = secondCellColumn - firstCellColumn
            };
        }

        /// <summary>
        /// Returns a cell at a given reference
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <returns>Cell</returns>
        private Cell GetCell(string reference) {
            var columnName = Utilities.GetColumnName(reference);
            var rowNumber = Utilities.GetRowIndex(reference);
            var columnIndex = Utilities.ColumnNames.IndexOf(columnName) + 1;

            return GetCell(rowNumber, columnIndex);
        }

        /// <summary>
        /// Returns a cell at a given row, column. 
        /// It creates a new cell if it does not exist.
        /// It will return the cell if it already exists
        /// </summary>
        /// <param name="rowNumber">Row Number</param>
        /// <param name="columnIndex">Column Index</param>
        /// <returns></returns>
        private Cell GetCell(int rowNumber, int columnIndex) {
            string reference = Utilities.BuildReference(rowNumber, columnIndex);
            var row = AllCells.FirstOrDefault(c => c.Key == rowNumber).Value;

            // Check if the cell already exists
            var cell = row == null ? null : row.FirstOrDefault(c => c.ColumnNumber == columnIndex);

            // Cell does not exist. Create one.
            if (cell == null) {
                cell = new Cell(reference) { Stylesheet = _styleSheet };

                var currentRow = Rows.FirstOrDefault(r => r.Index == rowNumber);

                // If the row already exists, get the row. Else, create a new row
                List<Cell> thisRow = null;
                if (AllCells.ContainsKey(rowNumber)) {
                    thisRow = AllCells[rowNumber];
                } else {
                    thisRow = new List<Cell>();
                    AllCells.Add(rowNumber, thisRow);
                }

                // Add cell to the current row.
                thisRow.Add(cell);

                // Add the current row to the list of rows if it does not exist in the list.
                if (currentRow == null) {
                    cell.Row = new Row(rowNumber);
                    Rows.Add(cell.Row);
                } else {
                    cell.Row = currentRow;
                }

                // Add the current column to the list of columns if it does not exist in the list.
                if (!Columns.ColumnsList.Any(c => c.Index == columnIndex)) {
                    Columns.ColumnsList.Add(new Column { Index = columnIndex });
                }
            }

            return cell;
        }

        /// <summary>
        /// Swaps the first and second elemnts.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        private static void Swap(ref int first, ref int second) {
            var tempSwap = first;
            first = second;
            second = tempSwap;
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Empties the Worksheet
        /// </summary>
        internal void ClearCells() {
            AllCells.Clear();
            Columns.ColumnsList.Clear();
            Rows.Clear();
        }

        /// <summary>
        /// Adds cells to the sheet without checking for its existance.
        /// The method is used by Importing Bulk data methods exposed by OpenXmlPackaging
        /// </summary>
        /// <param name="row">Cell Row</param>
        /// <param name="column">Cell Column</param>
        /// <param name="value">Value of the cell</param>
        internal void AddCell(int row, int column, string value) {
            // Get the current row
            var cellRow = AllCells.ContainsKey(row) ? AllCells[row] : null;

            // Add one if the row does not exist
            if (cellRow == null) {
                cellRow = new List<Cell>();
                AllCells.Add(row, cellRow);
            }

            // Create a cell
            var cell = new Cell(row, column) { Stylesheet = _styleSheet, Value = value };

            // Assign the Row property of the cell 
            if (cellRow.Count == 0) {
                cell.Row = new Row(row);
            } else {
                cell.Row = cellRow[0].Row;
            }

            // Add the cell to the row
            cellRow.Add(cell);
        }

        #endregion

        #region DataWriter Members

        internal override bool Write(XmlWriter writer) {
            writer.WriteStartElement("sheetData", Constants.MainNamespace);
            foreach (var row in AllCells.OrderBy(r => r.Key)) {
                if (row.Value.Any()) {
                    var currentRow = row.Value[0].Row;
                    var postWriteRow = currentRow.Write(writer);

                    foreach (var cell in row.Value) {
                        if (cell.Write(writer)) {
                            cell.PostWrite(writer);
                        }
                    }

                    if (postWriteRow) {
                        currentRow.PostWrite(writer);
                    }
                }
            }
            writer.WriteEndElement();

            return false;
        }

        #endregion
    }
}
