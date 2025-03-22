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
			Assert.That(rectangle.Left, Is.EqualTo(1));
			Assert.That(rectangle.Top, Is.EqualTo(2));
			Assert.That(rectangle.Width, Is.EqualTo(3));
			Assert.That(rectangle.Height, Is.EqualTo(4));
		}

		[Test]
		public void TestEquals()
		{
			Assert.That(new Rectangle(1, 2, 3, 4).Equals(new Rectangle(1, 2, 3, 4)), Is.True);
			Assert.That(new Rectangle(1, 2, 3, 4).Equals(new Rectangle(1, 2, 4, 3)), Is.False);
			Assert.That(new Rectangle(1, 2, 3, 4).Equals(new Rectangle(2, 1, 3, 4)), Is.False);
			Assert.That(new Rectangle(1, 2, 3, 4) == new Rectangle(1, 2, 3, 4), Is.True);
			Assert.That(new Rectangle(1, 2, 3, 4) != new Rectangle(1, 2, 4, 3), Is.True);
		}

		[Test]
		public void TestGetHashCode()
		{
			Assert.That(new Rectangle(1, 2, 3, 4).GetHashCode(), Is.EqualTo((((((1 << 4) ^ 2) << 4) ^ 3) << 4) ^ 4));
		}

		[Test]
		public void TestRightExclusive()
		{
			Assert.That(new Rectangle(1, 2, 3, 4).RightExclusive, Is.EqualTo(4));
			Assert.That(new Rectangle(3, 4, 5, 6).RightExclusive, Is.EqualTo(8));
			Assert.That(new Rectangle(1, 2, 0, 0).RightExclusive, Is.EqualTo(1));
		}

		[Test]
		public void TestBottomExclusive()
		{
			Assert.That(new Rectangle(1, 2, 3, 4).BottomExclusive, Is.EqualTo(6));
			Assert.That(new Rectangle(3, 4, 5, 6).BottomExclusive, Is.EqualTo(10));
			Assert.That(new Rectangle(1, 2, 0, 0).BottomExclusive, Is.EqualTo(2));
		}

		[Test]
		public void TestContainsPoint()
		{
			Assert.That(new Rectangle(10, 20, 30, 40).ContainsPoint(new Point(10, 20)), Is.True);
			Assert.That(new Rectangle(10, 20, 30, 40).ContainsPoint(new Point(10, 10)), Is.False);
			Assert.That(new Rectangle(10, 20, 30, 40).ContainsPoint(new Point(25, 40)), Is.True);
			Assert.That(new Rectangle(10, 20, 30, 40).ContainsPoint(new Point(0, 0)), Is.False);
			Assert.That(new Rectangle(10, 20, 30, 40).ContainsPoint(new Point(100, 100)), Is.False);
			Assert.That(new Rectangle(10, 20, 30, 40).ContainsPoint(new Point(30, 60)), Is.False);
			Assert.That(new Rectangle(10, 20, 30, 40).ContainsPoint(new Point(29, 59)), Is.True);
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
			Assert.That(actualRectsAsString, Is.EqualTo(expectedRectsAsString), message);
		}
	}
}

