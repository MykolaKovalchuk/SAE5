using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ravlyk.Adopted.OpenXmlPackaging {
    public abstract class Writer {
        /// <summary>
        /// Gets or sets the zero based index of the element in respective file.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        public virtual int Index { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has count.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has count; otherwise, <c>false</c>.
        /// </value>
        internal virtual bool HasCount { get { return true; } }
    }
}
