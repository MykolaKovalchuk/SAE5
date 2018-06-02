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
			Assert.AreEqual(1, size.Width);
			Assert.AreEqual(2, size.Height);
		}

		[Test]
		public void TestEquals()
		{
			Assert.IsTrue(new Size(1, 2).Equals(new Size(1, 2)));
			Assert.IsFalse(new Size(1, 2).Equals(new Size(2, 1)));
			Assert.IsFalse(new Size(1, 2).Equals(new Size(1, 1)));
			Assert.IsFalse(new Size(1, 2).Equals(new Size(2, 2)));
			Assert.IsFalse(new Size(1, 2).Equals(null));
		}

		[Test]
		public void TestGetHashCode()
		{
			Assert.AreEqual((1 << 16) ^ 2, new Size(1, 2).GetHashCode());
			Assert.AreEqual((2 << 16) ^ 1, new Size(2, 1).GetHashCode());
			Assert.AreEqual(100, new Size(0, 100).GetHashCode());
			Assert.AreEqual((100 << 16) ^ 200, new Size(100, 200).GetHashCode());
		}
	}
}
