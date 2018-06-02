using System;

namespace Ravlyk.Adopted.OpenXmlPackaging {
    
    public class ElementAttribute : Attribute {
        public string Name { get; set; }

        public ElementAttribute(string name) {
            Name = name;
        }
    }
}
