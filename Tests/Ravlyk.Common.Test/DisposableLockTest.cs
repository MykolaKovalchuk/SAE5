using System;
using NUnit.Framework;

namespace Ravlyk.Common.Test
{
	[TestFixture]
	public class DisposableLockTest
	{
		[Test]
		public void TestDisposableSuspender()
		{
			DisposableLock testLock1 = null;
			DisposableLock testLock2 = null;
			Assert.That(testLock1.IsLocked(), Is.False);
			Assert.That(testLock2.IsLocked(), Is.False);

			int unlockCounter = 0;
			Action unlockAction = () => unlockCounter++;
			Assert.That(unlockCounter, Is.EqualTo(0));

			using (DisposableLock.Lock(ref testLock1, unlockAction))
			{
				Assert.That(testLock1.IsLocked(), Is.True);
				Assert.That(testLock2.IsLocked(), Is.False);
				Assert.That(unlockCounter, Is.EqualTo(0));

				using (DisposableLock.Lock(ref testLock1, unlockAction))
				{
					Assert.That(testLock1.IsLocked(), Is.True);
					Assert.That(testLock2.IsLocked(), Is.False);
					Assert.That(unlockCounter, Is.EqualTo(0));

					using (DisposableLock.Lock(ref testLock2, unlockAction))
					{
						Assert.That(testLock1.IsLocked(), Is.True);
						Assert.That(testLock2.IsLocked(), Is.True);
						Assert.That(unlockCounter, Is.EqualTo(0));
					}

					Assert.That(testLock1.IsLocked(), Is.True);
					Assert.That(testLock2.IsLocked(), Is.False);
					Assert.That(unlockCounter, Is.EqualTo(1), "Action for counter 2 should be fired");
				}

				Assert.That(testLock1.IsLocked(), Is.True);
				Assert.That(testLock2.IsLocked(), Is.False);
				Assert.That(unlockCounter, Is.EqualTo(1));
			}

			Assert.That(testLock1.IsLocked(), Is.False);
			Assert.That(testLock2.IsLocked(), Is.False);
			Assert.That(unlockCounter, Is.EqualTo(2), "Action for counter 1 should be fired");
		}
	}
}

