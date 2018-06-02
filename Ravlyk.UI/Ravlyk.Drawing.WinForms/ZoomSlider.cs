using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Ravlyk.Drawing.WinForms
{
	/// <summary>
	/// Zoom slider control.
	/// </summary>
	public partial class ZoomSlider : UserControl
	{
		/// <summary>
		/// Public constructor.
		/// </summary>
		public ZoomSlider()
		{
			SetStyle(ControlStyles.Selectable, false);
			InitializeComponent();

			FixedZoomValues = new[] { 50, 100, 200 };
		}

		#region Zoom controls

		class ZoomButton : FlatButton
		{
			public ZoomButton()
			{
				FlatAppearance.BorderSize = 0;
			}
		}

		class ZoomTrackBar : TrackBar
		{
			public ZoomTrackBar()
			{
				SetStyle(ControlStyles.Selectable, false);
			}
		}

		#endregion

		#region Zoom Value

		/// <summary>
		/// List of zoom points for zoom slider.
		/// </summary>
		public int[] FixedZoomValues
		{
			get { return fixedZoomValues; }
			set
			{
				fixedZoomValues = value;
				lastFixedZoomValueIndex = fixedZoomValues.Length - 1;
				minZoom = fixedZoomValues[0];
				maxZoom = fixedZoomValues[lastFixedZoomValueIndex];
				trackBarZoom.Maximum = lastFixedZoomValueIndex;
				trackBarZoom.Value = lastFixedZoomValueIndex / 2;
			}
		}

		int[] fixedZoomValues;
		int lastFixedZoomValueIndex;
		int minZoom;
		int maxZoom;

		/// <summary>
		/// Sets or returns current zoom value.
		/// </summary>
		[DefaultValue(100)]
		public int ZoomValue
		{
			get { return zoomValue; }
			set
			{
				if (changingZoom)
				{
					return;
				}

				if (value < minZoom)
				{
					value = minZoom;
				}
				if (value > maxZoom)
				{
					value = maxZoom;
				}

				if (value != zoomValue)
				{
					changingZoom = true;
					try
					{
						int tickIndex = 0;
						for (int i = 0; i <= lastFixedZoomValueIndex; i++)
						{
							if (value == FixedZoomValues[i])
							{
								tickIndex = i;
								break;
							}
							if (value < FixedZoomValues[i])
							{
								var range = (FixedZoomValues[i] - FixedZoomValues[i - 1]) / 2;
								var right = FixedZoomValues[i] - value;
								tickIndex = right < range ? i : i - 1;
								break;
							}
						}
						trackBarZoom.Value = tickIndex;

						zoomValue = value;
						OnZoomChanged();
					}
					finally
					{
						changingZoom = false;
					}
				}
			}
		}
		int zoomValue = 100;
		bool changingZoom;

		#endregion

		#region Event

		void OnZoomChanged()
		{
			ZoomChanged?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// Zoom changed event.
		/// </summary>
		public event EventHandler ZoomChanged;

		#endregion

		#region Control actions

		void buttonZoomOut_Click(object sender, EventArgs e)
		{
			if (trackBarZoom.Value > trackBarZoom.Minimum)
			{
				trackBarZoom.Value--;
			}
		}

		void buttonZoomIn_Click(object sender, EventArgs e)
		{
			if (trackBarZoom.Value < trackBarZoom.Maximum)
			{
				trackBarZoom.Value++;
			}
		}

		void trackBarZoom_ValueChanged(object sender, EventArgs e)
		{
			if (!changingZoom)
			{
				ZoomValue = fixedZoomValues[trackBarZoom.Value];
			}
		}

		#endregion
	}
}
