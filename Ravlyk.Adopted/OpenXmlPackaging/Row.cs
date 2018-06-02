using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Ravlyk.Adopted.OpenXmlPackaging {

    /// <summary>
    /// Represents a row in Excel sheet
    /// Renders a <row> element
    /// </summary>
    public class Row : DataWriter {
    
        #region Public Properties

        public decimal? Height { get; set; }

        public int Reference { get; set; } 
        
        #endregion
        
        #region Constructor

        internal Row(int reference) {
            if (reference < 0) throw new ArgumentException("reference");
            Reference = reference;
        }
        
        #endregion

        #region DataWriter Members
        
        internal override bool Write(XmlWriter writer) {
            writer.WriteStartElement("row", Constants.MainNamespace);
            writer.WriteAttributeString("r", Reference.ToString());
            if (Height.HasValue) {
                writer.WriteAttributeString("ht", Height.ToString());
            }

            return true;
        }

        internal override void PostWrite(XmlWriter writer) {
            writer.WriteEndElement();
        } 

        #endregion
    }
}
