using System;

namespace Ravlyk.Adopted.OpenXmlPackaging {

    /// <summary>
    /// Represents a range of cells
    /// </summary>
    public class Range {

        #region Private Members
        
        private readonly Cells _cells;

        private readonly Worksheet _worksheet; 

        #endregion

        #region Public Properties
        
        public int StartRow { get; set; }

        public int StartColumn { get; set; }

        public int RowCount { get; set; }

        public int ColumnsCount { get; set; } 

        #endregion

        #region Constructor
        
        internal Range(Cells cells, Worksheet worksheet) {
            _cells = cells;
            _worksheet = worksheet;
        }

        #endregion
        #region Public Methods

        /// <summary>
        /// Merges the cells in the given range
        /// </summary>
        public void MergeCells() {
            string first = Utilities.BuildReference(StartRow, StartColumn);
            string second = Utilities.BuildReference(StartRow + RowCount, StartColumn + ColumnsCount);

            _worksheet.MergedCells.Cells.Add(new MergeCell {
                Reference = String.Format("{0}:{1}", first, second)
            });
        }

        /// <summary>
        /// Adds auto filters to the sheet. Overwrites the previously added auto filter if called more than once
        /// </summary>
        public void AutoFilter() {
            string first = Utilities.BuildReference(StartRow, StartColumn);
            string second = Utilities.BuildReference(StartRow + RowCount, StartColumn + ColumnsCount);
            _worksheet.AutoFilter.Reference = String.Format("{0}:{1}", first, second);
        } 
        
        #endregion
    }
}
