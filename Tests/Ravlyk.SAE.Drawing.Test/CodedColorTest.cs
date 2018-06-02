using System;
using NUnit.Framework;

namespace Ravlyk.SAE.Drawing.Test
{
	[TestFixture]
	public class CodedColorTest
	{
		[Test]
		public void TestConstructor()
		{
			CodedColor color = new CodedColor(0x12345678) { ColorCode = "100", ColorName = "Red", SymbolChar = 'A', Hidden = true };
			CodedColor newColor = new CodedColor(color);

			Assert.AreEqual(color, newColor);
			AssertColor(newColor, 0x12345678, "100", "Red", 'A', true);
		}

		[Test]
		public void TestClone()
		{
			CodedColor color = new CodedColor(0x12345678) { ColorCode = "100", ColorName = "Red", SymbolChar = 'A', Hidden = true };
			CodedColor clonedColor = color.Clone();

			Assert.AreEqual(typeof(CodedColor), clonedColor.GetType());
			Assert.AreEqual(color, clonedColor);
			Assert.AreNotSame(color, clonedColor);
			AssertColor(clonedColor, 0x12345678, "100", "Red", 'A', true);
		}

		#region Implementation

		void AssertColor(CodedColor color, int argb, string code, string name, char symbol, bool hidden)
		{
			Assert.AreEqual(argb, color.Argb);
			Assert.AreEqual(code, color.ColorCode);
			Assert.AreEqual(name, color.ColorName);
			Assert.AreEqual(symbol, color.SymbolChar);
			Assert.AreEqual(hidden, color.Hidden);
		}

		#endregion
	}
}

