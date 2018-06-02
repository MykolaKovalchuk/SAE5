using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Ravlyk.Drawing.WinForms;
using Ravlyk.SAE.Drawing;
using Ravlyk.SAE.Drawing.Processor;

namespace Ravlyk.SAE5.WinForms.UserControls
{
	public partial class PaletteUserControl : UserControl
	{
		public PaletteUserControl()
		{
			InitializeComponent();
		}

		#region Controller

		public PaletteController Controller
		{
			get { return controller; }
			set
			{
				if (value == controller)
				{
					return;
				}

				if (controller != null)
				{
					controller.PaletteSettingsChanged -= Controller_PaletteSettingsChanged;
				}

				controller = value;
				listViewPalette.VirtualListSize = controller?.Palette.Count ?? 0;
				listViewPalette.Invalidate();
				listViewPalette.Update();

				if (controller != null)
				{
					controller.PaletteSettingsChanged += Controller_PaletteSettingsChanged;
				}
			}
		}
		PaletteController controller;

		void Controller_PaletteSettingsChanged(object sender, EventArgs e)
		{
			UpdatePalette();
		}

		public void UpdatePalette()
		{
			if (!pendingUpdate)
			{
				pendingUpdate = true;
				BeginInvoke(new MethodInvoker(Repaint));
			}
		}

		bool pendingUpdate;

		void Repaint()
		{
			listViewPalette.VirtualListSize = Controller.Palette.Count;
			listViewPalette.Invalidate();
			listViewPalette.Update();
			pendingUpdate = false;
		}

		#endregion

		#region Properties

		public bool CompactMode
		{
			get { return compactMode; }
			set
			{
				// TODO: Implement compact mode view
				if (value)
				{
					throw new NotImplementedException();
				}
				compactMode = value;
			}
		}
		bool compactMode;

		#endregion

		#region Prepare list view

		const int ColumnIndexSymbol = 1;
		const int ColumnIndexColor = 3;
		const int ColumnIndexCount = 4;
		const int ColumnIndexCode = 2;
		const int ColumnIndexChecked = 0;

		void listViewPalette_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
		{
			var colorInfo = Controller.GetOrderedColorInfo(e.ItemIndex);

			e.Item =
				new ListViewItem(
					new[]
					{
						colorInfo.IsSelected ? "+" : "-",
						colorInfo.Symbol.ToString(),
						colorInfo.Code,
						"",
						colorInfo.Count.ToString()
					},
					e.ItemIndex)
					{
						Checked = colorInfo.IsSelected,
						ToolTipText = colorInfo.Name,
						UseItemStyleForSubItems = false
					};
			e.Item.SubItems[ColumnIndexColor].BackColor = System.Drawing.Color.FromArgb(colorInfo.Rgb);
		}

		#endregion

		#region Paint

		void listViewPalette_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
		{
			int fontColor, backColor;
			if (!listViewPalette.SelectedIndices.Contains(e.ItemIndex))
			{
				fontColor = e.SubItem.ForeColor.ToArgb();
				backColor = e.SubItem.BackColor.ToArgb();
			}
			else
			{
				fontColor = SystemColors.HighlightText.ToArgb();
				backColor = SystemColors.Highlight.ToArgb();
			}

			switch (e.ColumnIndex)
			{
				case ColumnIndexChecked:
					e.Graphics.FillRectangle(GetBrush(backColor), e.Bounds);

					var rect = e.Bounds;
					rect.Width = Math.Min(15, e.Bounds.Width);
					rect.Height = Math.Min(15, e.Bounds.Height);
					rect.X += (e.Bounds.Width - rect.Width) / 2;
					rect.Y += (e.Bounds.Height - rect.Height) / 2;

					ControlPaint.DrawCheckBox(e.Graphics, rect, (e.Item.Checked ? ButtonState.Checked : ButtonState.Normal) | ButtonState.Flat);

					break;
				case ColumnIndexSymbol:
					e.Graphics.FillRectangle(GetBrush(backColor), e.Bounds);

					var size = Math.Min(e.Bounds.Height, e.Bounds.Width);
					var image = size >= 2 ? Controller.GetSymbolImage(e.SubItem.Text[0], size, fontColor, backColor) : null;
					if (image != null)
					{
						var bitmap = image.ToBitmap();

						var dx = e.Bounds.Left + (e.Bounds.Width - bitmap.Size.Width) / 2;
						var dy = e.Bounds.Top + (e.Bounds.Height - bitmap.Size.Height) / 2;
						e.Graphics.DrawImage(bitmap, dx, dy);

						bitmap.Dispose();
					}
					break;
				case ColumnIndexCode:
					e.DrawDefault = true;
					break;
				case ColumnIndexColor:
					e.Graphics.FillRectangle(GetBrush(e.SubItem.BackColor.ToArgb()), e.Bounds);
					break;
				case ColumnIndexCount:
					e.DrawDefault = true;
					break;
			}
		}

		protected Brush GetBrush(int color)
		{
			color = unchecked((int)((uint)color | 0xff000000));

			Brush brush;
			if (!brushes.TryGetValue(color, out brush))
			{
				brush = new SolidBrush(System.Drawing.Color.FromArgb(color));
				brushes.Add(color, brush);
			}

			return brush;
		}
		readonly Dictionary<int, Brush> brushes = new Dictionary<int, Brush>();

		void listViewPalette_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
		{
			e.DrawDefault = true;
		}

		#endregion

		#region Mouse interaction

		void listViewPalette_MouseClick(object sender, MouseEventArgs e)
		{
			var item = listViewPalette.GetItemAt(e.X, e.Y);
			if (item != null && e.X < listViewPalette.Columns[ColumnIndexChecked].Width)
			{
				var color = Controller.Palette[item.SubItems[ColumnIndexColor].BackColor.ToArgb()];
				if (color != null)
				{
					Controller.Palette.ChangeColorVisibility(color);
					item.SubItems[ColumnIndexChecked].Text = color.Hidden ? "-" : "+";

					var rect = listViewPalette.GetItemRect(item.Index);
					rect.Width = listViewPalette.Columns[ColumnIndexChecked].Width;

					listViewPalette.Invalidate(rect);
					listViewPalette.Update();
				}
			}
		}

		void listViewPalette_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			switch (e.Column)
			{
				case ColumnIndexChecked:
					Controller.ChangeOrderMode(PaletteController.ColorsOrderMode.Selected);
					break;
				case ColumnIndexSymbol:
					Controller.ChangeOrderMode(PaletteController.ColorsOrderMode.Symbol);
					break;
				case ColumnIndexCode:
					Controller.ChangeOrderMode(PaletteController.ColorsOrderMode.Code);
					break;
				case ColumnIndexColor:
					Controller.ChangeOrderMode(PaletteController.ColorsOrderMode.Darkness);
					break;
				case ColumnIndexCount:
					Controller.ChangeOrderMode(PaletteController.ColorsOrderMode.Count);
					break;
			}
		}

		#endregion

		#region Selection

		void listViewPalette_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!pendingSelectedColorChanged)
			{
				pendingSelectedColorChanged = true;
				BeginInvoke(new MethodInvoker(OnSelectedColorChanged));
			}
		}

		bool pendingSelectedColorChanged;

		void OnSelectedColorChanged()
		{
			var colorInfo = listViewPalette.SelectedIndices.Count == 1 ? Controller.GetOrderedColorInfo(listViewPalette.SelectedIndices[0]) : null;
			SelectedColorChanged?.Invoke(this, new ColorEventArgs(colorInfo?.Color));
			pendingSelectedColorChanged = false;
		}

		public event EventHandler<ColorEventArgs> SelectedColorChanged;

		public class ColorEventArgs : EventArgs
		{
			public ColorEventArgs(CodedColor color)
			{
				Color = color;
			}

			public CodedColor Color { get; }
		}

		public IEnumerable<CodedColor> SelectedColors => listViewPalette.SelectedIndices.Cast<int>().Select(i => Controller.GetOrderedColorInfo(i).Color);

		public void ResetSelection()
		{
			if (pendingUpdate)
			{
				Repaint();
			}
			listViewPalette.SelectedIndices.Clear();
		}

		#endregion
	}
}
