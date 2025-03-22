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
				Assert.That(i, Is.EqualTo(1));
				i = 2;
				Assert.That(i, Is.EqualTo(2));
			}
			Assert.That(i, Is.EqualTo(0));
		}
	}
}

