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
			Assert.IsFalse(testLock1.IsLocked());
			Assert.IsFalse(testLock2.IsLocked());

			int unlockCounter = 0;
			Action unlockAction = () => unlockCounter++;
			Assert.AreEqual(0, unlockCounter);

			using (DisposableLock.Lock(ref testLock1, unlockAction))
			{
				Assert.IsTrue(testLock1.IsLocked());
				Assert.IsFalse(testLock2.IsLocked());
				Assert.AreEqual(0, unlockCounter);

				using (DisposableLock.Lock(ref testLock1, unlockAction))
				{
					Assert.IsTrue(testLock1.IsLocked());
					Assert.IsFalse(testLock2.IsLocked());
					Assert.AreEqual(0, unlockCounter);

					using (DisposableLock.Lock(ref testLock2, unlockAction))
					{
						Assert.IsTrue(testLock1.IsLocked());
						Assert.IsTrue(testLock2.IsLocked());
						Assert.AreEqual(0, unlockCounter);
					}

					Assert.IsTrue(testLock1.IsLocked());
					Assert.IsFalse(testLock2.IsLocked());
					Assert.AreEqual(1, unlockCounter, "Action for counter 2 should be fired");
				}

				Assert.IsTrue(testLock1.IsLocked());
				Assert.IsFalse(testLock2.IsLocked());
				Assert.AreEqual(1, unlockCounter);
			}

			Assert.IsFalse(testLock1.IsLocked());
			Assert.IsFalse(testLock2.IsLocked());
			Assert.AreEqual(2, unlockCounter, "Action for counter 1 should be fired");
		}
	}
}

