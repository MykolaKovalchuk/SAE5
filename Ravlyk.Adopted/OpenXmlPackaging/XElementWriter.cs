using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Ravlyk.Adopted.OpenXmlPackaging {
    public abstract class XElementWriter : Writer {       
        /// <summary>
        /// Gets the parent node of the element.
        /// </summary>
        internal abstract string ParentNode { get; }

        /// <summary>
        /// Gets the element as XElement.
        /// </summary>
        internal abstract XElement Element { get; }       
        
        /// <summary>
        /// Writes the element using Xml Writer
        /// </summary>
        /// <param name="writer">XmlWriter</param>
        internal virtual bool Write(XmlWriter writer) { return false; }

        /// <summary>
        /// Process the element post addition to the file (like calling WriteEndElement).
        /// </summary>
        /// <param name="writer">The XmlWriter.</param>
        internal virtual void PostWrite(XmlWriter writer) {
            return;
        }

        /// <summary>
        /// Process the element post addition to the file.
        /// </summary>
        /// <param name="element">The element.</param>
        internal virtual void PostProcess(XElement element) {
            return;
        }

        /// <summary>
        /// Initializing the Index value
        /// </summary>
        protected XElementWriter() {
            Index = -1;
        }
    }
}
