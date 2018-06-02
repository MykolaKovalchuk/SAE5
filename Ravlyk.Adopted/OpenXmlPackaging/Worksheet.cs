using System;
using System.Data;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Ravlyk.Adopted.OpenXmlPackaging {
    /// <summary>
    /// Represents a Worksheet
    /// </summary>
    public class Worksheet {

        #region Private Members
        
        private readonly Stylesheet _styleSheet;
        private readonly PackagePart _worksheetPart; 

        #endregion

        #region Public and Internal Properties
        
        public string Name { get; set; }
        public string RelationshipId { get; set; }
        public XDocument WorksheetXml { get; internal set; }
        public MergedCells MergedCells { get; set; }

        /// <summary>
        /// Indexer for all the cells.
        /// </summary>
        public Cells Cells { get; internal set; }
        
        internal AutoFilters AutoFilter { get; set; } 

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Worksheet"/> class.
        /// </summary>
        /// <param name="styleSheet">The style sheet.</param>
        /// <param name="worksheetPart">The worksheet part.</param>
        internal Worksheet(Stylesheet styleSheet, PackagePart worksheetPart) {
            _styleSheet = styleSheet;
            _worksheetPart = worksheetPart;
            MergedCells = new MergedCells();
            Cells = new Cells(_styleSheet, this);
            AutoFilter = new AutoFilters();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Saves this worksheet file.
        /// </summary>
        public void Save() {
            //SaveXElement();
            SaveWriter();
        }

        /// <summary>
        /// Autofits all the columns
        /// </summary>
        public void AutoFitColumns() {
            Cells.Columns.ColumnsList.ForEach(c => {
                // TODO: Implement autofit
                // c.Width = Cells.CellsList.Where(cell => cell.ColumnNumber == c.Index).Max(cell => cell.ColumnWidth);
            });
        }

        public void SetColumnWidth(int index, double? width) {
            var column = Cells.Columns.ColumnsList.FirstOrDefault(c => c.Index == index);
            if (column == null) {
                column = new Column { Index = index, Width = width };
                Cells.Columns.ColumnsList.Add(column);
            } else {
                column.Width = width;
            }
        }

        /// <summary>
        /// Imports the table given by the tableName in the dataset into the sheet.
        /// WARNING: This method will delete all existing data in the sheet
        /// </summary>
        /// <param name="ds">Dataset to import</param>
        /// <param name="tableName">Table name to import</param>
        /// <param name="startFromReference">Starting cell for importing data</param>
        /// <param name="includeColumnNames">Whether to include column names in the first row</param>
        public void ImportDataTable(DataTable table, string startFromReference, bool includeColumnNames) {
            if (table == null) throw new ArgumentNullException("table");
            Utilities.ValidateReference(startFromReference);

            Cells.ClearCells();
            MergedCells.Cells.Clear();
            AutoFilter.Reference = String.Empty;

            Cells.ImportDataTable(table, startFromReference, includeColumnNames);

        }

        #endregion

        #region Private Methods

        private void SaveWriter() {

            try {
                using (Stream stream = _worksheetPart.GetStream(FileMode.Create, FileAccess.Write)) {
                    using (var writer = XmlWriter.Create(stream)) {
                        writer.WriteStartElement("worksheet", Constants.MainNamespace);
                        writer.WriteAttributeString("xmlns", "r", null, Constants.RelationshipNamespace);

                        if (Cells.Columns.ColumnsList.Count > 0) {
                            Cells.Columns.Write(writer);
                        }

                        Cells.Write(writer);

                        if (MergedCells.Cells.Count > 0) {
                            MergedCells.Write(writer);
                        }

                        AutoFilter.Write(writer);

                        writer.WriteEndElement();
                    }
                }
            } catch (ObjectDisposedException) {
                //Suppress any ObjectDisposedException thrown by multiple using statements
            } catch {
                throw;
            }
        }

        #endregion
        //private void SaveXElement() {
        //    if (Cells.Columns.ColumnsList.Count > 0) {
        //        WorksheetXml.Add<Columns>(Cells.Columns);
        //    }

        //    WorksheetXml.Add<Cells>(Cells);

        //    if (MergedCells.Cells.Count > 0) {
        //        WorksheetXml.Add<MergedCells>(MergedCells);
        //    }

        //    WorksheetXml.Add<AutoFilters>(AutoFilter);

        //    using (Stream stream = _worksheetPart.GetStream(FileMode.Create, FileAccess.Write)) {
        //        using (var writer = XmlWriter.Create(stream)) {
        //            stream.SetLength(0);
        //            WorksheetXml.WriteTo(writer);
        //        }
        //    }
        //}
    }
}
