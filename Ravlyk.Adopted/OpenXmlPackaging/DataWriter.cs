using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Ravlyk.Adopted.OpenXmlPackaging {

    /// <summary>
    /// Writes Xml using XmlWriter
    /// </summary>
    public abstract class DataWriter : Writer {

        /// <summary>
        /// Write the element using XmlWriter
        /// </summary>
        /// <param name="writer">XmlWriter instance</param>
        /// <returns>true if the PostWrite method should be called after this method returns</returns>
        internal abstract bool Write(XmlWriter writer);

        /// <summary>
        /// Do operations post writing to the writer
        /// </summary>
        /// <param name="writer">XmlWriter instance</param>
        internal virtual void PostWrite(XmlWriter writer) {
            return;
        }
    }
}
