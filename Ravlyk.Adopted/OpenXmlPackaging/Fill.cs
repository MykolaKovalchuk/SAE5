using System;
using System.Drawing;
using System.Xml.Linq;

namespace Ravlyk.Adopted.OpenXmlPackaging
{
	/// <summary>
	/// Represents a <fill> element in Stylesheet
	/// </summary>
	public class Fill : XElementWriter
	{
		#region Public Properties

		public Color Color { get; set; }

		#endregion

		#region XElementWriter Members

		internal override string ParentNode => "fills";

		internal override XElement Element => new XElement(Constants.MainXNamespace + "fill",
			new XElement(Constants.MainXNamespace + "patternFill", new XAttribute("patternType", "solid"),
				new XElement(Constants.MainXNamespace + "fgColor", new XAttribute("rgb", Utilities.GetColorInHex(Color)))));

		#endregion

		#region Constructor

		public Fill(Color color = default(Color))
		{
			Color = color;
		}

		#endregion

		#region Equality

		public override int GetHashCode()
		{
			return Color.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			var fillO = obj as Fill;
			return Color.Equals(fillO?.Color);
		}

		#endregion
	}
}
