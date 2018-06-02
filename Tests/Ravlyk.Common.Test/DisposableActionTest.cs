using System;
using NUnit.Framework;

namespace Ravlyk.Common.Test
{
	[TestFixture]
	public class DisposableActionTest
	{
		[Test]
		public void TestDisposableAction()
		{
			int i = 1;
			using (new DisposableAction(() => i = 0))
			{
				Assert.AreEqual(1, i);
				i = 2;
				Assert.AreEqual(2, i);
			}
			Assert.AreEqual(0, i);
		}
	}
}

