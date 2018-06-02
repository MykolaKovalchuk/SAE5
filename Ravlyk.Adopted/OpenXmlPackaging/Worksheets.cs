using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.IO.Packaging;
using System.Xml;
using System.Xml.Linq;

namespace Ravlyk.Adopted.OpenXmlPackaging {
	/// <summary>
	/// Handles all the Worksheets in the Workbook
	/// </summary>
	public class Worksheets : IEnumerable {
		
		#region Private Variables
		
		private readonly Package _package;
		private readonly PackagePart _workbookPart;
		private readonly List<Worksheet> _worksheet;
		private readonly Stylesheet _stylesheet;

		#endregion

		#region Private Methods
		
		private Worksheet AddWorksheet(string name) {
			try {
				PackageRelationship sheetRelationship = null;
				using (Stream workbookStream = _workbookPart.GetStream(FileMode.Open, FileAccess.ReadWrite)) {
					Uri sheetUri = null;
					using (var writer = XmlWriter.Create(workbookStream)) {
						var document = XDocument.Load(workbookStream);

						XElement sheets = document.Descendants().FirstOrDefault(d => d.Name.Equals(Constants.MainXNamespace + "sheets"));

						var sheetElements = sheets.Descendants(Constants.MainXNamespace + "sheet");
						int sheetId;

						if (sheetElements.Any()) {

							if (sheetElements.Any(se => se.Attribute("name").Value == name)) {
								throw new Exception(String.Format("Sheet with name {0} already exists", name));
							}

							Int32.TryParse(sheetElements.Last().Attribute("sheetId").Value, out sheetId);
							sheetId++;
						}
						else {
							
							sheetId = 1;
						}
						sheetUri = new Uri(String.Format(Constants.SheetUriFormatPath, sheetId), UriKind.Relative);
						sheetRelationship = _workbookPart.CreateRelationship(sheetUri, TargetMode.Internal, Constants.RelationshipNamespace + "/worksheet");

						sheets.Add(new XElement(Constants.MainXNamespace + "sheet"
													, new XAttribute("name", name)
													, new XAttribute("sheetId", sheetId)
													, new XAttribute(Constants.RelationshipXNamespace + "id", sheetRelationship.Id)));

						// Clear the workbook xml file
						workbookStream.SetLength(0);

						document.WriteTo(writer);

						
					}


					PackagePart worksheetPart = _package.CreatePart(sheetUri, Constants.WorksheetContentType);

					var worksheet = new Worksheet(_stylesheet, worksheetPart) {
																				Name = name,
																				RelationshipId = sheetRelationship.Id,
																			};

					using (var writer = XmlWriter.Create(worksheetPart.GetStream(FileMode.Create, FileAccess.Write))) {
						worksheet.WorksheetXml = new XDocument(new XElement(Constants.MainXNamespace + "worksheet",
													new XAttribute(XNamespace.Xmlns + "r", Constants.RelationshipNamespace)));

						worksheet.WorksheetXml.WriteTo(writer);
					}

					return worksheet;
				}
			}
			catch {
				// TODO :: Add exception handling logic
				throw;
			}
		} 

		#endregion

		#region Constructor
		
		public Worksheets(Package package, Stylesheet styleSheet) {
			_package = package;
			_stylesheet = styleSheet;

			if (package.PartExists(Constants.WorkbookUri)) {
				_workbookPart = package.GetPart(Constants.WorkbookUri);
			}
			else {
				throw new Exception("Please add a workbook before instantiating a Sheet");
			}

			_worksheet = new List<Worksheet>();
		} 

		#endregion

		#region IEnumerable

		public IEnumerator GetEnumerator() {
			return _worksheet.GetEnumerator();
		}

		#endregion

		#region Manage worksheets

		public Worksheet Add(string name) {
			if (String.IsNullOrWhiteSpace(name)) {
				throw new NullReferenceException("item cannot be null");
			}

			Worksheet sheet = AddWorksheet(name);
			_worksheet.Add(sheet);
			return sheet;
		}

		public void Clear() {
			_worksheet.Clear();
		}

		public int Count {
			get { return _worksheet.Count; }
		}

		public bool Remove(Worksheet item) {
			return _worksheet.Remove(item);
		}

		public Worksheet this[int index] {
			get { return _worksheet[index]; }
		}

		public Worksheet this[string name] {
			get { return _worksheet.FirstOrDefault(w => w.Name == name); }
		}

		public void Save() {
			foreach (var worksheet in _worksheet) {
				worksheet.Save();
			}
		}

		#endregion

	}
}
