using System;
using NUnit.Framework;

namespace Ravlyk.Common.Test.Points
{
	[TestFixture]
	public class SizeTest
	{
		[Test]
		public void TestSize()
		{
			var size = new Size(1, 2);
			Assert.That(size.Width, Is.EqualTo(1));
			Assert.That(size.Height, Is.EqualTo(2));
		}

		[Test]
		public void TestEquals()
		{
			Assert.That(new Size(1, 2).Equals(new Size(1, 2)), Is.True);
			Assert.That(new Size(1, 2).Equals(new Size(2, 1)), Is.False);
			Assert.That(new Size(1, 2).Equals(new Size(1, 1)), Is.False);
			Assert.That(new Size(1, 2).Equals(new Size(2, 2)), Is.False);
			Assert.That(new Size(1, 2).Equals(null), Is.False);
		}

		[Test]
		public void TestGetHashCode()
		{
			Assert.That(new Size(1, 2).GetHashCode(), Is.EqualTo((1 << 16) ^ 2));
			Assert.That(new Size(2, 1).GetHashCode(), Is.EqualTo((2 << 16) ^ 1));
			Assert.That(new Size(0, 100).GetHashCode(), Is.EqualTo(100));
			Assert.That(new Size(100, 200).GetHashCode(), Is.EqualTo((100 << 16) ^ 200));
		}
	}
}
