using System.Xml.Linq;

namespace Ravlyk.Adopted.OpenXmlPackaging {

    /// <summary>
    /// Represents a <mergeCell> element in the Worksheet
    /// </summary>
    internal class MergeCell : DataWriter {

        public string Reference { get; set; }

        #region DataWriter Members

        internal override bool Write(System.Xml.XmlWriter writer) {

            writer.WriteStartElement("mergeCell", Constants.MainNamespace);
            writer.WriteAttributeString("ref", Reference);
            writer.WriteEndElement();

            return false;
        } 

        #endregion
    }
}
