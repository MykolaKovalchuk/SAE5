using System;
using System.Windows.Forms;
using Ravlyk.SAE.Drawing.Processor;

namespace Ravlyk.Drawing.WinForms
{
	/// <summary>
	/// Visual control with scroll bars.
	/// </summary>
	public partial class ScrollControl : UserControl
	{
		/// <summary>
		/// Initializes ScrollControl instance.
		/// </summary>
		public ScrollControl()
		{
			InitializeComponent();
			SetStyle(ControlStyles.Selectable, true);
			TabStop = true;
		}

		/// <summary>
		/// Additional initialization after control is loaded first time.
		/// </summary>
		/// <param name="e">Empty.</param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			BeginInvoke(new MethodInvoker(Initialize));
		}

		void Initialize()
		{
			panelH.Height = hScrollBar.Height = 17;
			panelV.Width = vScrollBar.Width = 17;
			panelSpacer.Width = vScrollBar.Width;
		}

		/// <summary>
		/// Visual controller.
		/// </summary>
		public VisualScrollableController Controller
		{
			get { return VisualControl.Controller as VisualScrollableController; }
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
					value.VisualImageChanged += VisualImageChanged;
					UpdateScrollers();
				}
			}
		}

		void VisualImageChanged(object sender, EventArgs e)
		{
			UpdateScrollers();
		}

		void UpdateScrollers()
		{
			var controller = Controller;
			if (controller != null)
			{
				inUpdatingScrollers = true;
				try
				{
					panelV.Visible = vScrollBar.Enabled = controller.ShowVScroll;
					if (panelV.Visible)
					{
						vScrollBar.Maximum = controller.MaxVSteps + 1;
						vScrollBar.LargeChange = controller.BigVStep;
						vScrollBar.SmallChange = 1; //Math.Max(1, controller.BigVStep / 50);
						vScrollBar.Value = controller.VPosition;
					}

					panelH.Visible = hScrollBar.Enabled = controller.ShowHScroll;
					if (panelH.Visible)
					{
						hScrollBar.Maximum = controller.MaxHSteps + 1;
						hScrollBar.LargeChange = controller.BigHStep;
						hScrollBar.SmallChange = 1; //Math.Max(1, controller.BigHStep / 50);
						hScrollBar.Value = controller.HPosition;
					}
				}
				finally
				{
					inUpdatingScrollers = false;
				}
			}
		}

		bool inUpdatingScrollers;

		void vScrollBar_ValueChanged(object sender, EventArgs e)
		{
			VisualScrollableController controller;
			if (!inUpdatingScrollers && (controller = Controller) != null)
			{
				controller.VPosition = vScrollBar.Value;
			}
		}

		void hScrollBar_ValueChanged(object sender, EventArgs e)
		{
			VisualScrollableController controller;
			if (!inUpdatingScrollers && (controller = Controller) != null)
			{
				controller.HPosition = hScrollBar.Value;
			}
		}
	}
}
