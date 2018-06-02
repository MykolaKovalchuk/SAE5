using System;
using NUnit.Framework;
using Ravlyk.Common;

namespace Ravlyk.SAE.Drawing.Processor.Test
{
	[TestFixture]
	public class ImageControllerTest
	{
		[Test]
		public void TestSuspendCallManipulations()
		{
			var controller = new DummyController(new ImageManipulatorTest.DummyManipulator(new CodedImage { Size = new Size(1, 1) }));
			Assert.IsFalse(controller.CallManipulationsCoreFired, "Precondition.");

			controller.CallManipulations();
			Assert.IsTrue(controller.CallManipulationsCoreFired, "CallManipulationsCore should be fired.");

			controller.CallManipulationsCoreFired = false;
			using (controller.SuspendCallManipulations())
			{
				controller.CallManipulations();
				Assert.IsFalse(controller.CallManipulationsCoreFired, "CallManipulationsCore should not be fired while it is suspended.");
			}
			Assert.IsTrue(controller.CallManipulationsCoreFired, "CallManipulationsCore should be automatically fired after suspension was released.");
		}

		[Test]
		public void TestSuspendDoesntCallManipulationIfNotNeeded()
		{
			var controller = new DummyController(new ImageManipulatorTest.DummyManipulator(new CodedImage { Size = new Size(1, 1) }));
			Assert.IsFalse(controller.CallManipulationsCoreFired, "Precondition.");

			using (controller.SuspendCallManipulations())
			{
				Assert.IsFalse(controller.CallManipulationsCoreFired, "CallManipulationsCore should not be fired while it is suspended.");
			}
			Assert.IsFalse(controller.CallManipulationsCoreFired, "CallManipulationsCore should not be fired if CallManipulations method was not not called during suspension.");
		}

		#region Test class

		public class DummyController : ImageController<ImageManipulatorTest.DummyManipulator>
		{
			public DummyController(ImageManipulatorTest.DummyManipulator manipulator) : base(manipulator) { }

			protected override void CallManipulationsCore()
			{
				CallManipulationsCoreFired = true;
			}

			public bool CallManipulationsCoreFired { get; set; }

			#region Defaults

			protected override void RestoreDefaultsCore() { }

			protected override void SaveDefaultsCore() { }

			#endregion
		}

		#endregion
	}
}

