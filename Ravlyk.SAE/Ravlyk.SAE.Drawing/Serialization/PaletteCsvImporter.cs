using System;
using System.Collections.Generic;
using Ravlyk.Drawing;
using Ravlyk.SAE.Drawing.Properties;

namespace Ravlyk.SAE.Drawing.Serialization
{
	/// <summary>
	/// Exports and imports <see cref="Palette"/> object to and from CSV.
	/// </summary>
	public static class PaletteCsvImporter
	{
		/// <summary>
		/// Imports <see cref="Palette"/> object from CSV.
		/// </summary>
		/// <param name="csv">Enumeration of CSV string lines.</param>
		/// <param name="skipFirstLine">Specifies if first line should be skipped.</param>
		/// <param name="indexCode">Index of color code value.</param>
		/// <param name="indexName">Index of color name value.</param>
		/// <param name="indexR">Index of color Red component value.</param>
		/// <param name="indexG">Index of color Green component value.</param>
		/// <param name="indexB">Index of color Blue component value.</param>
		/// <param name="name">New palette name.</param>
		/// <returns>Instance of <see cref="Palette"/> with all <see cref="Color"/> imported from CSV.</returns>
		public static CodedPalette ImportFromCsv(IEnumerable<string> csv, bool skipFirstLine = false, int indexCode = 0, int indexName = 1, int indexR = 2, int indexG = 3, int indexB = 4, string name = "")
		{
			var firstLine = !skipFirstLine;
			var palette = new CodedPalette { Name = name };
			foreach (var line in csv)
			{
				if (skipFirstLine)
				{
					skipFirstLine = false;
					continue;
				}

				try
				{
					byte r, g, b;
					string cCode, cName;
					GetLineValues(line, out cCode, out cName, out r, out g, out b, name, indexCode, indexName, indexR, indexG, indexB);

					var color = new CodedColor(r, g, b) { ColorCode = cCode, ColorName = cName };
					palette.Add(color);
				}
				catch (FormatException)
				{
					// Ignore for first line - maybe it is header
					if (!firstLine)
					{
						throw;
					}
				}

				firstLine = false;
			}

			return palette;
		}

		static void GetLineValues(string line, out string code, out string name, out byte r, out byte g, out byte b, string paletteName, int indexCode, int indexName, int indexR, int indexG, int indexB)
		{
			var maxIndex = indexCode;
			if (indexName > maxIndex) maxIndex = indexName;
			if (indexR > maxIndex) maxIndex = indexR;
			if (indexG > maxIndex) maxIndex = indexG;
			if (indexB > maxIndex) maxIndex = indexB;

			var values = line.Split(',');

			if (values.Length <= maxIndex)
			{
				var valuesTemplateList = new string[maxIndex];
				valuesTemplateList[indexCode] = Resources.PrintColumnCode;
				if (indexName >= 0)
				{
					valuesTemplateList[indexName] = Resources.PrintColumnName;
				}
				valuesTemplateList[indexR] = Resources.PrintColumnRed + " (0-255)";
				valuesTemplateList[indexG] = Resources.PrintColumnGreen + " (0-255)";
				valuesTemplateList[indexB] = Resources.PrintColumnBlue + " (0-255)";
				var template = string.Join(",", valuesTemplateList);

				throw new FormatException(Resources.ErrorIncorrectPaletteCsvFormat + Environment.NewLine + template);
			}

			code = values[indexCode].Trim(' ', '\t', '"', '\'');
			name = indexName >= 0 ? values[indexName].Trim(' ', '\t', '"', '\'') : (paletteName + " " + code).Trim();

			if (!byte.TryParse(values[indexR], out r))
			{
				throw new FormatException(string.Format(Resources.ErrorIncerroctPaletteCsvValue, values[indexR], indexR, Resources.PrintColumnRed, code));
			}

			if (!byte.TryParse(values[indexG], out g))
			{
				throw new FormatException(string.Format(Resources.ErrorIncerroctPaletteCsvValue, values[indexR], indexR, Resources.PrintColumnGreen, code));
			}

			if (!byte.TryParse(values[indexB], out b))
			{
				throw new FormatException(string.Format(Resources.ErrorIncerroctPaletteCsvValue, values[indexR], indexR, Resources.PrintColumnBlue, code));
			}
		}

		/// <summary>
		/// Exportns <see cref="Palette"/> object to CSV.
		/// </summary>
		/// <param name="palette"><see cref="Palette"/> object to be exported.</param>
		/// <param name="includeCaptions">Specifies if first line should contain values captions.</param>
		/// <returns>Enumeration of CSV string lines.</returns>
		public static IEnumerable<string> ExportToCsv(this CodedPalette palette, bool includeCaptions = false)
		{
			if (includeCaptions)
			{
				yield return "Code,Name,Reg,Green,Blue";
			}

			foreach (var color in palette)
			{
				yield return string.Join(",", color.ColorCode, color.ColorName, color.R.ToString(), color.G.ToString(), color.B.ToString());
			}
		}
	}
}
