using System;
using NUnit.Framework;

namespace Ravlyk.Drawing.ImageProcessor.Utilities.Test
{
	[TestFixture]
	public class ColorDistanceTest
	{
		[Test]
		public void TestInitialCoeffs()
		{
			Assert.AreEqual(30, ColorDistance.CoeffRed);
			Assert.AreEqual(59, ColorDistance.CoeffGreen);
			Assert.AreEqual(11, ColorDistance.CoeffBlue);
			Assert.AreEqual(0, ColorDistance.CoeffHue);
			Assert.AreEqual(0, ColorDistance.CoeffSaturation);
			Assert.AreEqual(0, ColorDistance.CoeffBrightness);
		}

		[Test]
		public void TestSquareDistance()
		{
			Assert.AreEqual(0, ColorDistance.GetSquareDistance(new Color(1, 2, 3), new Color(1, 2, 3)));
			Assert.AreEqual(9 * 9 + 18 * 18 + 27 * 27, ColorDistance.GetSquareDistance(new Color(1, 2, 3), new Color(10, 20, 30)));
			Assert.AreEqual(1 * 1 + 2 * 2 + 3 * 3, ColorDistance.GetSquareDistance(new Color(1, 2, 3), new Color(0)));
			Assert.AreEqual(new Color(123).GetSquareDistance(new Color(456)), new Color(456).GetSquareDistance(new Color(123)));
		}

		[Test]
		public void TestGetVisualDistance()
		{
			Assert.AreEqual(0, ColorDistance.GetVisualDistance(new Color(1, 2, 3), new Color(1, 2, 3)));
			Assert.AreEqual(30 * 9 * 9 + 59 * 18 * 18 + 11 * 27 * 27, ColorDistance.GetVisualDistance(new Color(1, 2, 3), new Color(10, 20, 30)));

			Assert.AreEqual(30 * 1 * 1 + 59 * 2 * 2 + 11 * 3 * 3, ColorDistance.GetVisualDistance(new Color(1, 2, 3), new Color(0)));
			using (ColorDistance.UseCoeffs(5, 7, 13))
			{
				Assert.AreEqual(5 * 1 * 1 + 7 * 2 * 2 + 13 * 3 * 3, ColorDistance.GetVisualDistance(new Color(1, 2, 3), new Color(0)));
			}
			Assert.AreEqual(30 * 1 * 1 + 59 * 2 * 2 + 11 * 3 * 3, ColorDistance.GetVisualDistance(new Color(1, 2, 3), new Color(0)));

			Assert.AreEqual(new Color(123).GetVisualDistance(new Color(456)), new Color(456).GetVisualDistance(new Color(123)));
		}

		[Test]
		public void TestGetVisualDistanceEnhanced()
		{
			Assert.Inconclusive("HSB is not implemented on Color. Also it will be difficult to test with it because of its float point calculation.");
		}

		[Test]
		public void TestColorCoeffs()
		{
			Assert.AreEqual(30, ColorDistance.CoeffRed);
			Assert.AreEqual(59, ColorDistance.CoeffGreen);
			Assert.AreEqual(11, ColorDistance.CoeffBlue);
			Assert.AreEqual(0, ColorDistance.CoeffHue);
			Assert.AreEqual(0, ColorDistance.CoeffSaturation);
			Assert.AreEqual(0, ColorDistance.CoeffBrightness);

			using (ColorDistance.UseCoeffs(1, 2, 3, 4, 5, 6))
			{
				Assert.AreEqual(1, ColorDistance.CoeffRed);
				Assert.AreEqual(2, ColorDistance.CoeffGreen);
				Assert.AreEqual(3, ColorDistance.CoeffBlue);
				Assert.AreEqual(4, ColorDistance.CoeffHue);
				Assert.AreEqual(5, ColorDistance.CoeffSaturation);
				Assert.AreEqual(6, ColorDistance.CoeffBrightness);
			}

			Assert.AreEqual(30, ColorDistance.CoeffRed);
			Assert.AreEqual(59, ColorDistance.CoeffGreen);
			Assert.AreEqual(11, ColorDistance.CoeffBlue);
			Assert.AreEqual(0, ColorDistance.CoeffHue);
			Assert.AreEqual(0, ColorDistance.CoeffSaturation);
			Assert.AreEqual(0, ColorDistance.CoeffBrightness);
		}
	}
}

