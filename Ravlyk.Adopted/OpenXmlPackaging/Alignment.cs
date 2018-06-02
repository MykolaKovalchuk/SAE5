using System.Xml.Linq;

namespace Ravlyk.Adopted.OpenXmlPackaging
{
	/// <summary>
	/// Represents an <alignment> tag in the Stylesheet
	/// </summary>
	public class Alignment : XElementWriter
	{
		#region Public Properties

		// Horizontal Alignment
		public HorizontalAlignment HorizontalAlignment { get; set; }

		// Vertical Alignment
		public VerticalAlignment VerticalAlignment { get; set; }

		// WrapText = true => wraps the text
		public bool WrapText { get; set; }

		// Rotation angle in degrees
		public double? Rotation { get; set; }

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public Alignment()
		{
			// Initialize alignment to default values
			HorizontalAlignment = HorizontalAlignment.None;
			VerticalAlignment = VerticalAlignment.None;
		}

		#endregion

		#region XElementWriter Members

		/// <summary>
		/// Returns the Parent node to which this element is added
		/// </summary>
		internal override string ParentNode => "xf";

		/// <summary>
		/// Returns the XElement equivalent of the node
		/// </summary>
		internal override XElement Element
		{
			get
			{
				var element = new XElement(Constants.MainXNamespace + "alignment");
				if (HorizontalAlignment != HorizontalAlignment.None)
				{
					element.Add(new XAttribute(typeof(HorizontalAlignment).GetAttribute<ElementAttribute>().Name,
						HorizontalAlignment.GetAttribute<ElementAttribute>().Name));
				}

				if (VerticalAlignment != VerticalAlignment.None)
				{
					element.Add(new XAttribute(typeof(VerticalAlignment).GetAttribute<ElementAttribute>().Name,
						VerticalAlignment.GetAttribute<ElementAttribute>().Name));
				}

				if (Rotation.HasValue)
				{
					element.Add(new XAttribute("textRotation", Rotation.ToString()));
				}

				if (WrapText)
				{
					element.Add(new XAttribute("wrapText", "1"));
				}

				return element;
			}
		}

		/// <summary>
		/// Returns true if the element has a count attribute
		/// </summary>
		internal override bool HasCount => false;

		#endregion

		#region Equality

		public override int GetHashCode()
		{
			return HorizontalAlignment.GetHashCode()
				^ VerticalAlignment.GetHashCode()
				^ (WrapText ? 1 : 0)
				^ Rotation.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			var alignmentO = obj as Alignment;
			return alignmentO != null
				&& HorizontalAlignment == alignmentO.HorizontalAlignment
				&& VerticalAlignment == alignmentO.VerticalAlignment
				&& WrapText == alignmentO.WrapText
				&& Rotation == alignmentO.Rotation;
		}

		#endregion
	}
}
