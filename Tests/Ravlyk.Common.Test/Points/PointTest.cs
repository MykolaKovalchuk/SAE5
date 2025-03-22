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
			Assert.That(point.X, Is.EqualTo(1));
			Assert.That(point.Y, Is.EqualTo(2));
		}

		[Test]
		public void TestEquals()
		{
			Assert.That(new Point(1, 2).Equals(new Point(1, 2)), Is.True);
			Assert.That(new Point(1, 2).Equals(new Point(2, 1)), Is.False);
			Assert.That(new Point(1, 2).Equals(new Point(1, 1)), Is.False);
			Assert.That(new Point(1, 2).Equals(new Point(2, 2)), Is.False);
			Assert.That(new Point(1, 2).Equals(null), Is.False);
			Assert.That(new Point(1, 2) == new Point(1, 2), Is.True);
			Assert.That(new Point(1, 2) != new Point(2, 1), Is.True);
		}

		[Test]
		public void TestGetHashCode()
		{
			Assert.That(new Point(1, 2).GetHashCode(), Is.EqualTo((1 << 16) ^ 2));
			Assert.That(new Point(2, 1).GetHashCode(), Is.EqualTo((2 << 16) ^ 1));
			Assert.That(new Point(0, 100).GetHashCode(), Is.EqualTo(100));
			Assert.That(new Point(100, 200).GetHashCode(), Is.EqualTo((100 << 16) ^ 200));
		}
	}
}

