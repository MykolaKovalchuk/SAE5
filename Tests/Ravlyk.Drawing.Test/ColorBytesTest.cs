using System;
using NUnit.Framework;

namespace Ravlyk.Drawing.Test
{
	[TestFixture]
	public class ColorBytesTest
	{
		[Test]
		public void TestComponentsExtraction()
		{
			Assert.AreEqual(0x12, 0x12345678.Alpha());
			Assert.AreEqual(0x34, 0x12345678.Red());
			Assert.AreEqual(0x56, 0x12345678.Green());
			Assert.AreEqual(0x78, 0x12345678.Blue());
		}

		[Test]
		public void TestComponentsComposition()
		{
			Assert.AreEqual(0x12345678, ColorBytes.ToArgb(0x12, 0x34, 0x56, 0x78));
		}

		[Test]
		public void TestHalfToneHight()
		{
			Assert.AreEqual(0x00808080, 0x00000000.HalfToneHigh());
			Assert.AreEqual(unchecked((int)0xffffffff), unchecked((int)0xffffffff).HalfToneHigh());
			Assert.AreEqual(unchecked((int)0xc0c0c0c0), unchecked((int)0x80808080).HalfToneHigh());
			Assert.AreEqual(0x099aabbc, 0x12345678.HalfToneHigh());
			Assert.AreEqual(0x009aabbc, 0x00345678.HalfToneHigh());
		}

		[Test]
		public void TestHalfToneLow()
		{
			Assert.AreEqual(0x00000000, 0x00000000.HalfToneLow());
			Assert.AreEqual(unchecked((int)0x7f7f7f7f), unchecked((int)0xffffffff).HalfToneLow());
			Assert.AreEqual(0x40404040, unchecked((int)0x80808080).HalfToneLow());
			Assert.AreEqual(0x091a2b3c, 0x12345678.HalfToneLow());
			Assert.AreEqual(0x001a2b3c, 0x00345678.HalfToneLow());
		}

		[Test]
		public void TestThreeQuarterToneHigh()
		{
			Assert.AreEqual(0x00c0c0c0, 0x00000000.ThreeQuarterToneHigh());
			Assert.AreEqual(unchecked((int)0xffffffff), unchecked((int)0xffffffff).ThreeQuarterToneHigh());
			Assert.AreEqual(unchecked((int)0xe0e0e0e0), unchecked((int)0x80808080).ThreeQuarterToneHigh());
			Assert.AreEqual(0x04cdd5de, 0x12345678.ThreeQuarterToneHigh());
			Assert.AreEqual(0x00cdd5de, 0x00345678.ThreeQuarterToneHigh());
			Assert.AreEqual(0x12345678.HalfToneHigh().HalfToneHigh(), 0x12345678.ThreeQuarterToneHigh(), "3/4 tone high should be same as 2x 1/2 tone high.");
		}

		[Test]
		public void TestComposeTwoColors()
		{
			Assert.AreEqual(0x00000000, ColorBytes.ComposeTwoColors(0x003377ff, 0x00000000, 255));
			Assert.AreEqual(0x003377ff, ColorBytes.ComposeTwoColors(0x00000000, 0x003377ff, 255));
			Assert.AreEqual(0x003377ff, ColorBytes.ComposeTwoColors(0x003377ff, 0x00000000, 0));
			Assert.AreEqual(0x00000000, ColorBytes.ComposeTwoColors(0x00000000, 0x003377ff, 0));

			Assert.AreEqual(0x11662266, ColorBytes.ComposeTwoColors(0x11880088, 0x22008800, 63));
			Assert.AreEqual(0x11662266, ColorBytes.ComposeTwoColors(0x11880088, 0x22008800, 64)); // Same due to rounding
		}

		[Test]
		public void TestInvertBytes()
		{
			Assert.AreEqual(0x04030201, 0x01020304.InvertBytes());
		}
	}
}

