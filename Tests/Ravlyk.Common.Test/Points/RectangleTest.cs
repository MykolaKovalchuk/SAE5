using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Ravlyk.Common.Test
{
	[TestFixture]
	public class RectangleTest
	{
		[Test]
		public void TestRectangle()
		{
			var rectangle = new Rectangle(1, 2, 3, 4);
			Assert.AreEqual(1, rectangle.Left);
			Assert.AreEqual(2, rectangle.Top);
			Assert.AreEqual(3, rectangle.Width);
			Assert.AreEqual(4, rectangle.Height);
		}

		[Test]
		public void TestEquals()
		{
			Assert.IsTrue(new Rectangle(1, 2, 3, 4).Equals(new Rectangle(1, 2, 3, 4)));
			Assert.IsFalse(new Rectangle(1, 2, 3, 4).Equals(new Rectangle(1, 2, 4, 3)));
			Assert.IsFalse(new Rectangle(1, 2, 3, 4).Equals(new Rectangle(2, 1, 3, 4)));
			Assert.IsTrue(new Rectangle(1, 2, 3, 4) == new Rectangle(1, 2, 3, 4));
			Assert.IsTrue(new Rectangle(1, 2, 3, 4) != new Rectangle(1, 2, 4, 3));
		}

		[Test]
		public void TestGetHashCode()
		{
			Assert.AreEqual((((((1 << 4) ^ 2) << 4) ^ 3) << 4) ^ 4, new Rectangle(1, 2, 3, 4).GetHashCode());
		}

		[Test]
		public void TestRightExclusive()
		{
			Assert.AreEqual(4, new Rectangle(1, 2, 3, 4).RightExclusive);
			Assert.AreEqual(8, new Rectangle(3, 4, 5, 6).RightExclusive);
			Assert.AreEqual(1, new Rectangle(1, 2, 0, 0).RightExclusive);
		}

		[Test]
		public void TestBottomExclusive()
		{
			Assert.AreEqual(6, new Rectangle(1, 2, 3, 4).BottomExclusive);
			Assert.AreEqual(10, new Rectangle(3, 4, 5, 6).BottomExclusive);
			Assert.AreEqual(2, new Rectangle(1, 2, 0, 0).BottomExclusive);
		}

		[Test]
		public void TestContainsPoint()
		{
			Assert.IsTrue(new Rectangle(10, 20, 30, 40).ContainsPoint(new Point(10, 20)));
			Assert.IsFalse(new Rectangle(10, 20, 30, 40).ContainsPoint(new Point(10, 10)));
			Assert.IsTrue(new Rectangle(10, 20, 30, 40).ContainsPoint(new Point(25, 40)));
			Assert.IsFalse(new Rectangle(10, 20, 30, 40).ContainsPoint(new Point(0, 0)));
			Assert.IsFalse(new Rectangle(10, 20, 30, 40).ContainsPoint(new Point(100, 100)));
			Assert.IsFalse(new Rectangle(10, 20, 30, 40).ContainsPoint(new Point(30, 60)));
			Assert.IsTrue(new Rectangle(10, 20, 30, 40).ContainsPoint(new Point(29, 59)));
		}

		[Test]
		public void TestAMinusB()
		{
			AssertAMinuB(new Rectangle(10, 10, 30, 20), new Rectangle(10, 10, 30, 20));
			AssertAMinuB(new Rectangle(10, 10, 30, 20), new Rectangle(50, 50, 30, 20), new Rectangle(10, 10, 30, 20));
			AssertAMinuB(new Rectangle(50, 50, 30, 20), new Rectangle(10, 10, 30, 20), new Rectangle(50, 50, 30, 20));
			AssertAMinuB(new Rectangle(10, 10, 30, 20), new Rectangle(15, 15, 20, 10), new Rectangle(10, 10, 5, 20), new Rectangle(35, 10, 5, 20), new Rectangle(15, 10, 20, 5), new Rectangle(15, 25, 20, 5));
			AssertAMinuB(new Rectangle(15, 15, 20, 10), new Rectangle(10, 10, 30, 20));
			AssertAMinuB(new Rectangle(10, 10, 30, 20), new Rectangle(20, 10, 20, 20), new Rectangle(10, 10, 10, 20));
			AssertAMinuB(new Rectangle(10, 10, 30, 20), new Rectangle(10, 10, 10, 20), new Rectangle(20, 10, 20, 20));
			AssertAMinuB(new Rectangle(10, 10, 30, 20), new Rectangle(10, 20, 30, 10), new Rectangle(10, 10, 30, 10));
			AssertAMinuB(new Rectangle(10, 10, 30, 20), new Rectangle(10, 10, 30, 10), new Rectangle(10, 20, 30, 10));
			AssertAMinuB(new Rectangle(10, 10, 30, 20), new Rectangle(20, 20, 30, 20), new Rectangle(10, 10, 10, 20), new Rectangle(20, 10, 20, 10));
			AssertAMinuB(new Rectangle(10, 10, 30, 20), new Rectangle(20, 5, 10, 20), new Rectangle(10, 10, 10, 20), new Rectangle(30, 10, 10, 20), new Rectangle(20, 25, 10, 5));
		}

		[Test]
		public void TestXor()
		{
			AssertAXorB(new Rectangle(10, 10, 30, 20), new Rectangle(20, 20, 30, 20), new Rectangle(10, 10, 10, 20), new Rectangle(20, 10, 20, 10), new Rectangle(40, 20, 10, 20), new Rectangle(20, 30, 20, 10));
			AssertAXorB(new Rectangle(10, 10, 30, 20), new Rectangle(10, 10, 30, 20));
			AssertAXorB(new Rectangle(10, 10, 30, 20), new Rectangle(50, 50, 30, 20), new Rectangle(10, 10, 30, 20), new Rectangle(50, 50, 30, 20));
		}

		void AssertAMinuB(Rectangle a, Rectangle b, params Rectangle[] expected)
		{
			AsserAreEqual(expected, Rectangle.AMinusB(a, b), $"{a} - {b}");
		}

		void AssertAXorB(Rectangle a, Rectangle b, params Rectangle[] expected)
		{
			AsserAreEqual(expected, a.Xor(b), $"{a} XOR {b}");
		}

		void AsserAreEqual(IEnumerable<Rectangle> expected, IEnumerable<Rectangle> actual, string message)
		{
			var actualRectsAsString = string.Join(Environment.NewLine, actual);
			var expectedRectsAsString = string.Join(Environment.NewLine, expected);
			Assert.AreEqual(expectedRectsAsString, actualRectsAsString, message);
		}
	}
}

