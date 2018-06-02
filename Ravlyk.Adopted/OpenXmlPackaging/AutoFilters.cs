using System;
using System.Xml;
using System.Xml.Linq;
namespace Ravlyk.Adopted.OpenXmlPackaging {
    class AutoFilters : XElementWriter {

        #region Public Properties
        
        /// <summary>
        /// Auto filter reference
        /// </summary>
        public string Reference { get; set; } 

        #endregion

        #region XElementWriter members

        internal override string ParentNode {
            get { return "worksheet"; }
        }

        internal override XElement Element {
            get {
                return !String.IsNullOrWhiteSpace(Reference) ? new XElement(Constants.MainXNamespace + "autoFilter",
                                                                new XAttribute("ref", Reference))
                                                             : null;
            }
        } 
        #endregion

        //internal override bool Write(XmlWriter writer) {
        //    if (!String.IsNullOrWhiteSpace(Reference)) {
        //        writer.WriteStartElement("autoFilter", Constants.MainNamespace);
        //        writer.WriteAttributeString("ref", Reference);
        //        writer.WriteEndElement();
        //    }

        //    return false;
        //}
    }
}
