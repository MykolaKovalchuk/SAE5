using System;
using System.Xml.Linq;
using System.Drawing;

namespace Ravlyk.Adopted.OpenXmlPackaging
{
	public class Border : XElementWriter
	{
		#region Public Properties

		public BorderType BorderType { get; set; }

		public BorderStyles BorderStyle { get; set; }

		public Color Color { get; set; }

		#endregion

		#region XElementWriter Members

		internal override string ParentNode => "borders";

		internal override XElement Element => new XElement(Constants.MainXNamespace + BorderType.GetAttribute<ElementAttribute>().Name,
			new XAttribute("style", BorderStyle.GetAttribute<ElementAttribute>().Name),
			new XElement(Constants.MainXNamespace + "color", new XAttribute("rgb", Utilities.GetColorInHex(Color))));

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="Border"/> class.
		/// </summary>
		public Border()
		{
			Color = Color.Black;
		}

		#endregion

		#region Equality

		public override int GetHashCode()
		{
			return BorderType.GetHashCode()
				^ BorderStyle.GetHashCode()
				^ Color.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			var borderO = obj as Border;
			return borderO != null
				&& BorderType == borderO.BorderType
				&& BorderStyle == borderO.BorderStyle
				&& Color == borderO.Color;
		}

		#endregion
	}
}
