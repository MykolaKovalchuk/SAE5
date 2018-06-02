using System;
using System.Linq;
using System.Windows.Forms;
using Ravlyk.Drawing.WinForms.Properties;

namespace Ravlyk.Drawing.WinForms
{
	/// <summary>
	/// Container control with Collapse button, which changes control's height.
	/// </summary>
	public partial class CollapsiblePanel : UserControl
	{
		/// <summary>
		/// Initializes instance.
		/// </summary>
		public CollapsiblePanel()
		{
			InitializeComponent();

			buttonCollapse.FlatAppearance.BorderSize = 0;
		}

		/// <summary>
		/// Container's caption, appears on Collapse button.
		/// </summary>
		public string Caption
		{
			get { return buttonCollapse.Text; }
			set { buttonCollapse.Text = value; }
		}

		/// <summary>
		/// Specifies collapsed state of controller.
		/// </summary>
		public bool IsCollapsed
		{
			get { return isCollapsed; }
			set
			{
				if (value != isCollapsed)
				{
					isCollapsed = value;
					UpdateState();
				}
			}
		}
		bool isCollapsed;

		void buttonCollapse_Click(object sender, EventArgs e)
		{
			IsCollapsed = !IsCollapsed;
		}

		void UpdateState()
		{
			Height = IsCollapsed ? buttonCollapse.Height : Controls.Cast<Control>().Select(c => c.Bottom).Max() + 6;
			buttonCollapse.Image = IsCollapsed ? Resources.Right16 : Resources.Down16;
		}
	}
}
