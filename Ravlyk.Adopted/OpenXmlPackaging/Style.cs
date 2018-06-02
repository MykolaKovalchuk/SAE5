using System.Xml.Linq;

namespace Ravlyk.Adopted.OpenXmlPackaging
{
	/// <summary>
	/// Represents a style in the Stylesheet.
	/// Renders as <xf>
	/// </summary>
	public class Style : XElementWriter
	{
		#region Public Properties

		public NumberFormat NumberFormat { get; set; }

		public Fill Fill { get; set; }

		public Font Font { get; set; }

		public Borders Borders { get; set; }

		public Alignment Alignment { get; set; }

		#endregion

		#region XElementWriter Members

		internal override string ParentNode => "cellXfs";

		internal override XElement Element => ApplyStyle();

		#endregion

		#region Private Methods

		XElement ApplyStyle()
		{
			var element = new XElement(Constants.MainXNamespace + "xf");

			if (NumberFormat != null)
			{
				element.Add(new XAttribute("numFmtId", NumberFormat.Index));
			}

			if (Fill != null)
			{
				element.Add(new XAttribute("fillId", Fill.Index));
			}

			if (Font != null)
			{
				element.Add(new XAttribute("fontId", Font.Index));
			}

			if (Borders != null)
			{
				element.Add(new XAttribute("borderId", Borders.Index));
			}

			if (Alignment != null)
			{
				element.Add(Alignment.Element);
			}

			return element;
		}

		#endregion

		#region Equality

		public override int GetHashCode()
		{
			return NumberFormat?.GetHashCode() ?? 0
				^ Fill?.GetHashCode() ?? 0
				^ Font?.GetHashCode() ?? 0
				^ Borders?.GetHashCode() ?? 0
				^ Alignment?.GetHashCode() ?? 0;
		}

		public override bool Equals(object obj)
		{
			var styleO = obj as Style;

			return styleO != null
				&& (NumberFormat == null && styleO.NumberFormat == null || NumberFormat.Equals(styleO.NumberFormat))
				&& (Fill == null && styleO.Fill == null || Fill.Equals(styleO.Fill))
				&& (Font == null && styleO.Font == null || Font.Equals(styleO.Font))
				&& (Borders == null && styleO.Borders == null || Borders.Equals(styleO.Borders))
				&& (Alignment == null && styleO.Alignment == null || Alignment.Equals(styleO.Alignment));
		}

		#endregion
	}
}
