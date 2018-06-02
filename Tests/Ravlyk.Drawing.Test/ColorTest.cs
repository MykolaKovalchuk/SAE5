using System;
using NUnit.Framework;

namespace Ravlyk.Drawing.Test
{
	[TestFixture]
	public class ColorTest
	{
		[Test]
		public void TestInitialization()
		{
			AssertColor(new Color(0x12345678), 0x12, 0x34, 0x56, 0x78, 0x12345678);
			AssertColor(new Color(0x12, 0x34, 0x56, 0x78), 0x12, 0x34, 0x56, 0x78, 0x12345678);
			AssertColor(new Color(0x34, 0x56, 0x78), 0, 0x34, 0x56, 0x78, 0x00345678);
			AssertColor(new Color(new Color(0x12345678)), 0x12, 0x34, 0x56, 0x78, 0x12345678);
		}

		[Test]
		public void TestHalfToneColors()
		{
			Assert.AreEqual(0x12345678.HalfToneHigh(), new Color(0x12345678).HalfToneHighColor.Argb);
			Assert.AreEqual(0x12345678.HalfToneLow(), new Color(0x12345678).HalfToneLowColor.Argb);

			Color testColor = new Color(0x12345678);
			Assert.AreSame(testColor.HalfToneHighColor, testColor.HalfToneHighColor);
			Assert.AreSame(testColor.HalfToneLowColor, testColor.HalfToneLowColor);
		}

		[Test]
		public void TestDarkness()
		{
			Assert.AreEqual(20 + 30 + 40, new Color(10, 20, 30, 40).Darkness);
			Assert.AreEqual(0, new Color(0).Darkness);
			Assert.AreEqual(255 * 3, new Color(255, 255, 255).Darkness);
		}

		[Test]
		public void TestHSB()
		{
			Assert.Inconclusive("Implemen the functionality and test.");
		}

		[Test]
		public void TestEquals()
		{
			Color testColor = new Color(0x12345678);

			Assert.IsTrue(testColor.Equals(testColor));
			Assert.IsTrue(testColor.Equals(new Color(0x12345678)));
			Assert.IsTrue(testColor.Equals(new Color(0x12, 0x34, 0x56, 0x78)));
			Assert.IsTrue(testColor.Equals(new Color(0xff, 0x34, 0x56, 0x78)));

			Assert.IsFalse(testColor.Equals(new Color(0)));
			Assert.IsFalse(testColor.Equals(0x12345678));
			Assert.IsFalse(testColor.Equals(null));
		}

		[Test]
		public void TestGetHashCode()
		{
			Assert.AreEqual(0, new Color(0).GetHashCode());
			Assert.AreEqual(unchecked((int)0xffffffff), new Color(unchecked((int)0xffffffff)).GetHashCode());
			Assert.AreEqual(0x12345678, new Color(0x12345678).GetHashCode());
		}

		[Test]
		public void TestUsageOccurrences()
		{
			Color testColor = new Color(0x12345678);
			Assert.AreEqual(0, testColor.OccurrencesCount);

			testColor.AddOccurrence(1);
			testColor.AddOccurrence(2);
			testColor.AddOccurrence(1);
			Assert.AreEqual(3, testColor.OccurrencesCount);

			testColor.RemoveOccurrence(1);
			Assert.AreEqual(2, testColor.OccurrencesCount);

			testColor.ClearOccurrences();
			Assert.AreEqual(0, testColor.OccurrencesCount);
		}

		[Test]
		public void TestClone()
		{
			AssertColor(new Color(0x12345678).Clone(), 0x12, 0x34, 0x56, 0x78, 0x12345678);

			Color color = new Color(1);
			color.AddOccurrence(0);
			color.AddOccurrence(1);

			Color clonedColor = color.Clone();

			Assert.AreEqual(2, color.OccurrencesCount);
			Assert.AreEqual(0, clonedColor.OccurrencesCount);
		}

		[Test]
		public void TestToString()
		{
			Assert.AreEqual("(1, 2, 3)", new Color(1, 2, 3).ToString());
		}

		#region Implementation

		static void AssertColor(Color color, byte a, byte r, byte g, byte b, int argb)
		{
			Assert.AreEqual(a, color.A);
			Assert.AreEqual(r, color.R);
			Assert.AreEqual(g, color.G);
			Assert.AreEqual(b, color.B);
			Assert.AreEqual(argb, color.Argb);
		}

		#endregion
	}
}

