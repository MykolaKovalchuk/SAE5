using System;
using NUnit.Framework;

namespace Ravlyk.SAE.Drawing.Processor.Test
{
	[TestFixture]
	public abstract class ImageManipulatorTestCase
	{
		[Test]
		public abstract void TestRestoreManipulationsCore();

		protected void AssertActionCallsOnImageChanged(ImageManipulatorTest.DummyManipulator childManipulator, Action action)
		{
			Assert.NotNull(childManipulator.ManipulatedImage);
			childManipulator.RestoreManipulationsCoreFired = false;
			Assert.IsFalse(childManipulator.RestoreManipulationsCoreFired, "Precondition");
			action();
			Assert.IsTrue(childManipulator.RestoreManipulationsCoreFired);
		}
	}
}

