using System;
using NUnit.Framework;
using System.Text;
using System.Xml;
using System.IO;

namespace Ravlyk.SAE.Drawing.Serialization.Test
{
	[TestFixture]
	public class PaletteSerializerTest
	{
		[Test]
		public void TestSerialize()
		{
			var palette = new CodedPalette { Name = "Palette 1", SymbolsFont = "Times New Roman" };
			palette.Add(new CodedColor(1));
			palette.Add(new CodedColor(2));

			var sb = new StringBuilder();
			using (var writer = XmlWriter.Create(sb))
			{
				palette.WriteToXml(writer);
			}

			using (var reader = XmlReader.Create(new StringReader(sb.ToString())))
			{
				var reloadedPalette = PaletteSerializer.ReadFromXml(reader);

				Assert.AreEqual("Palette 1", reloadedPalette.Name);
				Assert.AreEqual("Times New Roman", reloadedPalette.SymbolsFont);
				Assert.AreEqual(2, reloadedPalette.Count);
				Assert.IsTrue(reloadedPalette[1] is CodedColor);
				Assert.IsTrue(reloadedPalette[2] is CodedColor);
			}
		}
	}
}

