using System;
using System.IO;
using System.Text;
using System.Xml;
using NUnit.Framework;

namespace Ravlyk.SAE.Drawing.Serialization.Test
{
	[TestFixture]
	public class ColorSerializerTest
	{
		[Test]
		public void TestSerialize()
		{
			var color = new CodedColor(1, 2, 3, 4) { ColorCode = "100", ColorName = "Black", SymbolChar = 'A' };

			var sb = new StringBuilder();
			using (var writer = XmlWriter.Create(sb))
			{
				color.WriteToXml(writer);
			}

			using (var reader = XmlReader.Create(new StringReader(sb.ToString())))
			{
				var reloadedColor = ColorSerializer.ReadFromXml(reader);

				Assert.AreEqual(1, reloadedColor.A);
				Assert.AreEqual(2, reloadedColor.R);
				Assert.AreEqual(3, reloadedColor.G);
				Assert.AreEqual(4, reloadedColor.B);
				Assert.AreEqual("100", reloadedColor.ColorCode);
				Assert.AreEqual("Black", reloadedColor.ColorName);
				Assert.AreEqual('A', reloadedColor.SymbolChar);
			}
		}
	}
}

