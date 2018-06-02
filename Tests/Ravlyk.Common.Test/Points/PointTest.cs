using System;
using NUnit.Framework;

namespace Ravlyk.Common.Test
{
	[TestFixture]
	public class PointTest
	{
		[Test]
		public void TestPoint()
		{
			var point = new Point(1, 2);
			Assert.AreEqual(1, point.X);
			Assert.AreEqual(2, point.Y);
		}

		[Test]
		public void TestEquals()
		{
			Assert.IsTrue(new Point(1, 2).Equals(new Point(1, 2)));
			Assert.IsFalse(new Point(1, 2).Equals(new Point(2, 1)));
			Assert.IsFalse(new Point(1, 2).Equals(new Point(1, 1)));
			Assert.IsFalse(new Point(1, 2).Equals(new Point(2, 2)));
			Assert.IsFalse(new Point(1, 2).Equals(null));
			Assert.IsTrue(new Point(1, 2) == new Point(1, 2));
			Assert.IsTrue(new Point(1, 2) != new Point(2, 1));
		}

		[Test]
		public void TestGetHashCode()
		{
			Assert.AreEqual((1 << 16) ^ 2, new Point(1, 2).GetHashCode());
			Assert.AreEqual((2 << 16) ^ 1, new Point(2, 1).GetHashCode());
			Assert.AreEqual(100, new Point(0, 100).GetHashCode());
			Assert.AreEqual((100 << 16) ^ 200, new Point(100, 200).GetHashCode());
		}
	}
}

