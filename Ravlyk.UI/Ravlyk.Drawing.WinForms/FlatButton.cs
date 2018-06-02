using System;
using System.Windows.Forms;

namespace Ravlyk.Drawing.WinForms
{
	/// <summary>
	/// Flat button control.
	/// </summary>
	public class FlatButton : Button
	{
		/// <summary>
		/// Initializes instance.
		/// </summary>
		public FlatButton()
		{
			if (!IsSelectable)
			{
				SetStyle(ControlStyles.Selectable, false);
			}
			FlatStyle = FlatStyle.Flat;
			//FlatAppearance.BorderSize = 0;
			FlatAppearance.MouseOverBackColor = System.Drawing.Color.CornflowerBlue;
			FlatAppearance.MouseDownBackColor = System.Drawing.Color.LightSlateGray;
			FlatAppearance.CheckedBackColor = System.Drawing.Color.Coral;
			TextImageRelation = TextImageRelation.ImageAboveText;
		}

		/// <summary>
		/// Specifies if this button class controls is selectable in UI.
		/// </summary>
		protected virtual bool IsSelectable => false;

		/// <summary>
		/// Gets or sets button's Selected state.
		/// </summary>
		public bool IsSelected
		{
			get { return isSelected; }
			set
			{
				isSelected = value;
				//BackColor = isSelected ? System.Drawing.Color.FromArgb(0, 224, 112, 32) : System.Drawing.Color.White;
				BackColor = isSelected ? System.Drawing.Color.Coral : System.Drawing.Color.White;
				FlatAppearance.MouseOverBackColor = isSelected ? System.Drawing.Color.OrangeRed : System.Drawing.Color.CornflowerBlue;
			}
		}
		bool isSelected;
	}

	/// <summary>
	/// Flat dialog (selectable) button control.
	/// </summary>
	public class FlatDialogButton : FlatButton
	{
		/// <summary>
		/// Specifies if this button class controls is selectable in UI.
		/// </summary>
		protected override bool IsSelectable => true;
	}

	/// <summary>
	/// Flat button with 0 padding for text.
	/// </summary>
	class PaddinglessButton : FlatButton
	{
		public PaddinglessButton()
		{
			FlatAppearance.BorderSize = 0;
			BackColor = System.Drawing.Color.LightSteelBlue;
			base.Text = "";
		}

		/// <summary>
		/// New Text property for manual painting.
		/// </summary>
		public new string Text { get; set; }

		protected override void OnPaint(PaintEventArgs pevent)
		{
			base.OnPaint(pevent);

			if (!string.IsNullOrEmpty(Text))
			{
				TextRenderer.DrawText(pevent.Graphics, Text, Font, ClientRectangle, ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
			}
		}
	}
}
