using System;
using Ravlyk.Common;

namespace Ravlyk.SAE.Drawing.Processor
{
	public abstract class VisualScrollableController : VisualBoxedController
	{
		protected VisualScrollableController(IImageProvider imageProvider, Size imageBoxSize) : base(imageProvider, imageBoxSize) { }

		public abstract bool ShowVScroll { get; }
		public abstract int MaxVSteps { get; }
		public abstract int BigVStep { get; }
		public abstract int VPosition { get; set; }

		public abstract bool ShowHScroll { get; }
		public abstract int MaxHSteps { get; }
		public abstract int BigHStep { get; }
		public abstract int HPosition { get; set; }
	}
}
