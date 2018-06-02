using System;
using System.Windows.Forms;
using Ravlyk.Common;
using Ravlyk.SAE.Drawing.Processor;

namespace Ravlyk.Drawing.WinForms
{
	/// <summary>
	/// Image box with zoom control.
	/// </summary>
	public partial class ZoomImageBox : UserControl
	{
		/// <summary>
		/// Initializes new ZoomImageBox instance.
		/// </summary>
		public ZoomImageBox()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Returns or sets image controller to be shown in this image box.
		/// </summary>
		public VisualZoomController Controller
		{
			get { return VisualControl.Controller as VisualZoomController; }
			set
			{
				var controller = Controller;
				if (value == controller)
				{
					return;
				}

				if (controller != null)
				{
					controller.VisualImageChanged -= VisualImageChanged;
				}

				VisualControl.Controller = value;
				if (value != null)
				{
					value.FitImage();
					zoomSlider.ZoomValue = value.ZoomPercent;
					value.VisualImageChanged += VisualImageChanged;
				}
			}
		}

		void VisualImageChanged(object sender, EventArgs e)
		{
			var controller = Controller;
			if (controller != null && zoomSlider.ZoomValue != controller.ZoomPercent)
			{
				zoomSlider.ZoomValue = controller.ZoomPercent;
			}
		}

		void zoomSlider_ZoomChanged(object sender, EventArgs e)
		{
			var controller = Controller;
			if (controller != null)
			{
				controller.ZoomPercent = zoomSlider.ZoomValue;
			}
		}

		void VisualControl_ControllerTouched(object sender, VisualControl.ControllerTouchedEventArgs e)
		{
			var controller = Controller;
			if (controller != null)
			{
				var zoomedImagePoint = controller.GetImagePoint(e.TouchPoint);
				ImageTouched?.Invoke(this, new VisualControl.ControllerTouchedEventArgs(new Point(zoomedImagePoint.X * 100 / controller.ZoomPercent, zoomedImagePoint.Y * 100 / controller.ZoomPercent)));
			}
		}

		/// <summary>
		/// Occurs when left mouse button is pressed down over image.
		/// </summary>
		public event EventHandler<VisualControl.ControllerTouchedEventArgs> ImageTouched;
	}
}
