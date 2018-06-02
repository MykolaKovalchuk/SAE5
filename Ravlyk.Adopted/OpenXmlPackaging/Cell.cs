using System;
using System.Xml;
using System.Xml.Linq;

namespace Ravlyk.Adopted.OpenXmlPackaging
{
	/// <summary>
	/// Represents a <c> element in the Worksheet
	/// </summary>
	public class Cell : DataWriter
	{

		#region Private Members

		private Style _style;

		internal Stylesheet Stylesheet { get; set; }

		internal int RowNumber { get; private set; }

		internal int ColumnNumber { get; private set; }

		#endregion

		#region Public Properties

		public object Value { get; set; }

		public string Reference { get; set; }

		public string Formula { get; set; }

		public Row Row { get; internal set; }

		/// <summary>
		/// Gets or sets the style.
		/// </summary>
		/// <value>
		/// The style.
		/// </value>
		public Style Style
		{
			get { return _style; }
			set
			{
				_style = value;
				if (Stylesheet != null) {
					Stylesheet.AddStyle(value);
				}
			}
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="Cell"/> class.
		/// </summary>
		/// <param name="reference">The reference.</param>
		public Cell(string reference)
		{
			InitializeCell(reference, null);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Cell"/> class.
		/// </summary>
		/// <param name="reference">The reference.</param>
		/// <param name="value">The value.</param>
		public Cell(string reference, string value)
		{
			InitializeCell(reference, value);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Cell"/> class.
		/// </summary>
		/// <param name="row">The row.</param>
		/// <param name="column">The column.</param>
		public Cell(int row, int column)
		{
			if (row < 1 || row > Constants.MaxRowNumber)
			{
				throw new ArgumentException("Invalid Row");
			}

			if (column < 1 || column > Constants.MaxColumnNumber)
			{
				throw new ArgumentException("Invalid Column");
			}

			Reference = Utilities.BuildReference(row, column);

			RowNumber = row;
			ColumnNumber = column;
		}

		#endregion

		#region Public Methods

		internal double? ColumnWidth
		{
			get
			{
				string font = Style != null && Style.Font != null ? Style.Font.Name : "Calibri";
				decimal fontSize = Style != null && Style.Font != null ? Style.Font.Size : 11;
				return Value == null ? 8 : Utilities.GetColumnWidth(Value.ToString(), font, (float)fontSize);
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Initializes the cell.
		/// </summary>
		/// <param name="reference">The reference.</param>
		/// <param name="value">The value.</param>
		private void InitializeCell(string reference, string value)
		{
			// Validate reference
			Utilities.ValidateReference(reference);
			reference = reference.ToUpper();

			// Set Value
			if (!String.IsNullOrWhiteSpace(value))
			{
				Value = value;
			}

			// Set Reference
			if (!String.IsNullOrWhiteSpace(reference))
			{
				Reference = reference;
			}

			// Set Row and Column numbers
			RowNumber = Utilities.GetRowIndex(reference);
			ColumnNumber = Utilities.ColumnNames.IndexOf(Utilities.GetColumnName(reference)) + 1;
		}

		#endregion

		#region DataWriter Members
		
		/// <summary>
		/// Writes the cell to the XmlWriter
		/// </summary>
		/// <param name="writer">XmlWriter</param>
		/// <returns>Whether to call PostWrite Method or not</returns>
		internal override bool Write(XmlWriter writer) {

			writer.WriteStartElement("c", Constants.MainNamespace);
			writer.WriteAttributeString("r", Reference);

			if (Style != null && Style.Index >= 0) {
				writer.WriteAttributeString("s", Style.Index.ToString());
			}

			// Add formula if available
			if (!String.IsNullOrWhiteSpace(Formula)) {
				writer.WriteStartElement("f", Constants.MainNamespace);
				writer.WriteString(Formula);
				writer.WriteEndElement();
			}

			// Add value if available
			if (Value != null) {

				//TypeCode code = Value.GetType().TypeCode

				decimal tempDouble;
				string value = Value.ToString();
				if (!Decimal.TryParse(value, out tempDouble)) {
					writer.WriteAttributeString("t", "str");
				}

				writer.WriteStartElement("v", Constants.MainNamespace);
				writer.WriteString(value);
				writer.WriteEndElement();
				//cell.Add(new XElement(Constants.MainXNamespace + "v") { Value = Value.ToString() });
			}

			writer.WriteEndElement();
			return false;
		} 

		#endregion
	}
}
