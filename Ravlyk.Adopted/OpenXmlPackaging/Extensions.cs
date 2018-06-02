using System;
using System.Linq;
using System.Xml.Linq;
using System.Reflection;

namespace Ravlyk.Adopted.OpenXmlPackaging {
    public static class Extensions {
        public static void Add<T>(this XDocument document, T element) where T : XElementWriter {
            if (element.Element != null) {
                var parentNode = document.Descendants(Constants.MainXNamespace + element.ParentNode).FirstOrDefault() ??
                                 new XElement(Constants.MainXNamespace + element.ParentNode);

                // Add the element
                parentNode.Add(element.Element);

                if (element.HasCount) {
                    if (parentNode.Attribute("count") == null) {
                        parentNode.Add(new XAttribute("count", "0"));
                    }

                    // Get the count
                    int count = Int32.Parse(parentNode.Attribute("count").Value);
                    count++;

                    // update count value
                    parentNode.Attribute("count").SetValue(count);

                    // Set the index of element in the list
                    element.Index = count - 1;
                } 
            }
        }

        public static T GetAttribute<T>(this object member) {
            object[] attributes = null;
            if (member is Type) {
                attributes = ((Type)member).GetCustomAttributes(typeof(T), false);
            } else {
                MemberInfo[] memberInfo = member.GetType().GetMember(member.ToString());

                if (memberInfo.Any()) {
                    attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
                }
            }

            if (attributes != null && attributes.Any()) {
                return (T)attributes[0];
            }

            return default(T);
        }
    }
}
