using System;
using System.Windows.Forms;

namespace Ravlyk.Drawing.WinForms
{
	/// <summary>
	/// Split container with collapsible first panel and Collapse button.
	/// </summary>
	public class CollapsibleSplitContainer : SplitContainer
	{
		/// <summary>
		/// Initializes class instance.
		/// </summary>
		public CollapsibleSplitContainer()
		{
			InitializeComponent();
		}

		void InitializeComponent()
		{
			collapseButton = new PaddinglessButton();
			collapseButton.Click += CollapseButton_Click;

			SplitterMoved += CollapsibleSplitContainer_SplitterMoved;
		}

		/// <summary>
		/// Collapse symbol.
		/// </summary>
		public string CollapseText { get; set; } = "<";
		/// <summary>
		/// Expand symbol.
		/// </summary>
		public string ExpandText { get; set; } = ">";

		PaddinglessButton collapseButton;

		void CollapseButton_Click(object sender, EventArgs e)
		{
			Panel1Collapsed = !Panel1Collapsed;
			UpdateCollapseButton();
		}

		/// <summary>
		/// Updates text, size, and position of collapse button.
		/// </summary>
		public void UpdateCollapseButton()
		{
			if (collapseButton == null)
			{
				return;
			}

			collapseButton.Text = Panel1Collapsed ? ExpandText : CollapseText;

			if (Orientation == Orientation.Vertical)
			{
				collapseButton.Width = SplitterWidth;
				collapseButton.Left = Left + (Panel1Collapsed ? 0 : SplitterDistance);
				collapseButton.Height = 80;
				collapseButton.Top = Top + (Height - collapseButton.Height) / 2;

				Panel2.Padding = new Padding(Panel1Collapsed ? SplitterWidth : 0, 0, 0, 0);
			}
			else
			{
				collapseButton.Width = 80;
				collapseButton.Left = Left + (Width - collapseButton.Width) / 2;
				collapseButton.Height = SplitterWidth;
				collapseButton.Top = Top + (Panel1Collapsed ? 0 : SplitterDistance);

				Panel2.Padding = new Padding(0, Panel1Collapsed ? SplitterWidth : 0, 0, 0);
			}
		}

		/// <summary>
		/// Processes control size change.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			UpdateCollapseButton();
		}

		void CollapsibleSplitContainer_SplitterMoved(object sender, SplitterEventArgs e)
		{
			UpdateCollapseButton();
		}

		/// <summary>
		/// Parent control has changed.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnParentChanged(EventArgs e)
		{
			base.OnParentChanged(e);
			if (Parent != null && collapseButton != null)
			{
				Parent.Controls.Add(collapseButton);
				collapseButton.BringToFront();
				UpdateCollapseButton();
			}
		}

		/// <summary>
		/// Dispose controls resources.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (collapseButton != null)
				{
					collapseButton.Click -= CollapseButton_Click;
					collapseButton.Dispose();
					collapseButton = null;
				}
			}

			base.Dispose(disposing);
		}
	}
}
