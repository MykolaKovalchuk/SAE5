using System;
using System.Xml.Linq;

namespace Ravlyk.Adopted.OpenXmlPackaging
{

	/// <summary>
	/// Represents a <numFmt> element in the Stylesheet
	/// </summary>
	public class NumberFormat : XElementWriter
	{

		#region Private Members

		int _formatId;

		#endregion

		#region Public Properties

		public string FormatCode { get; set; }

		#endregion

		#region XElementWriter Members

		internal override string ParentNode => "numFmts";

		internal override XElement Element
		{
			get
			{
				XElement numFmt = new XElement(Constants.MainXNamespace + "numFmt");
				if (!String.IsNullOrWhiteSpace(FormatCode))
				{
					numFmt.Add(new XAttribute("formatCode", FormatCode));
				}
				return numFmt;
			}
		}

		internal override void PostProcess(XElement element)
		{
			element.Add(new XAttribute("numFmtId", Index));
		}

		public override int Index
		{
			get { return _formatId; }
			set { _formatId = value == -1 ? value : value + 164; }
		}

		#endregion

		#region Constructor

		public NumberFormat(string formatCode)
		{
			// TODO :: Format ID??
			FormatCode = formatCode;
		}

		#endregion

		#region Equality

		public override int GetHashCode()
		{
			return FormatCode.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			var numberFormatO = obj as NumberFormat;
			return numberFormatO != null && FormatCode == numberFormatO.FormatCode;
		}

		#endregion
	}
}
