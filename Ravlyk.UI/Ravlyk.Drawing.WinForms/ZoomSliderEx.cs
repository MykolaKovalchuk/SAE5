using System;
using System.Windows.Forms;

namespace Ravlyk.Drawing.WinForms
{
	/// <summary>
	/// Extended zoom slider with zoom value label.
	/// </summary>
	public partial class ZoomSliderEx : UserControl
	{
		/// <summary>
		/// Public constructor.
		/// </summary>
		public ZoomSliderEx()
		{
			InitializeComponent();
		}

		/// <summary>
		/// List of zoom points for zoom slider.
		/// </summary>
		public int[] FixedZoomValues
		{
			get { return ZoomSlider.FixedZoomValues; }
			set { ZoomSlider.FixedZoomValues = value; }
		}

		/// <summary>
		/// Sets or returns current zoom value.
		/// </summary>
		public int ZoomValue
		{
			get { return ZoomSlider.ZoomValue; }
			set { ZoomSlider.ZoomValue = value; }
		}

		/// <summary>
		/// Zoom changed event.
		/// </summary>
		public event EventHandler ZoomChanged
		{
			add { ZoomSlider.ZoomChanged += value; }
			remove { ZoomSlider.ZoomChanged -= value; }
		}

		void ZoomSlider_ZoomChanged(object sender, EventArgs e)
		{
			labelZoom.Text = ZoomSlider.ZoomValue + "%";
		}
	}
}
