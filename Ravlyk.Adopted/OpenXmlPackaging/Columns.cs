using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Ravlyk.Adopted.OpenXmlPackaging {
    internal class Columns : DataWriter {

        #region Internal Members
        
        internal List<Column> ColumnsList { get; set; } 

        #endregion
        
        #region Constructor
        
        /// <summary>
        /// Constructor
        /// </summary>
        public Columns() {
            ColumnsList = new List<Column>();
        }
        
        #endregion
        
        #region DataWriter Members

        internal override bool Write(XmlWriter writer) {

            var columnsWithValue = ColumnsList.Where(c => c.Width.HasValue).OrderBy(c => c.Index).ToList();

            if (columnsWithValue.Count > 0) {
                writer.WriteStartElement("cols", Constants.MainNamespace);

                foreach (var column in columnsWithValue) {
                    writer.WriteStartElement("col", Constants.MainNamespace);
                    writer.WriteAttributeString("min", column.Index.ToString());
                    writer.WriteAttributeString("max", column.Index.ToString());
                    writer.WriteAttributeString("width", column.Width.ToString());
                    writer.WriteAttributeString("customWidth", "1");
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            return false;
        }

        #endregion
    }
}
