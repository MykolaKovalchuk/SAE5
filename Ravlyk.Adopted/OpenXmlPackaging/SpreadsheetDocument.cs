using System;
using System.IO.Packaging;
using System.IO;


namespace Ravlyk.Adopted.OpenXmlPackaging {
    /// <summary>
    /// Represents the Workbook
    /// </summary>
    public sealed class SpreadsheetDocument : IDisposable {
        
        #region Private Members

        private Package _package;

        private Workbook _workbook;

        private Stylesheet _stylesheet;

        private Worksheets _worksheets;
        
        #endregion

        #region Constructor

        public SpreadsheetDocument(string path) {
            CreateSpreadsheetDocument(path, FileMode.Create);
        }
        
        #endregion
        
        #region Public Properties
        
        public Workbook Workbook {
            get { return _workbook; }
            set { _workbook = value; }
        }

        public Stylesheet Stylesheet {
            get { return _stylesheet; }
            set { _stylesheet = value; }
        }

        public Worksheets Worksheets {
            get { return _worksheets; }
            set { _worksheets = value; }
        } 

        public Package Package {
            get { return _package; }
            set { _package = value; }
        }

        #endregion
        
        #region Private Methods

        private void CreateSpreadsheetDocument(string path, FileMode mode) {
            _package = Package.Open(path, mode);
            _workbook = new Workbook(_package);
            _stylesheet = new Stylesheet(_package);
            _worksheets = new Worksheets(_package, _stylesheet);
        }
        
        #endregion
        
        #region IDisposable Member

        public void Dispose() {
            try {
                _stylesheet.Save();
                _worksheets.Save();
                _package.Flush();
                _package.Close();
            } catch {
                throw;
                // TODO :: Exception handling logic goes here
            }
        } 
        
        #endregion
    }
}
