using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Ravlyk.Drawing.ImageProcessor.Utilities;
using Ravlyk.SAE.Drawing;
using Ravlyk.SAE.Drawing.Processor;
using Ravlyk.SAE.Resources;
using Ravlyk.SAE5.WinForms.Properties;
using Color = Ravlyk.Drawing.Color;

namespace Ravlyk.SAE5.WinForms.Dialogs
{
	public partial class ThreadsDialog : Form
	{
		public ThreadsDialog(string paletteName, ICollection<CodedColor> targetColors, ICollection<CodedColor> usedColors)
		{
			InitializeComponent();
			this.paletteName = paletteName;
			this.targetColors = targetColors;
			this.usedColors = usedColors;
		}

		string paletteName;
		readonly ICollection<CodedColor> targetColors;
		readonly ICollection<CodedColor> usedColors;

		public CodedColor SelectedColor { get; private set; }

		public void ChangeHint(string hint)
		{
			labelHint.Text = hint;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			BeginInvoke(new MethodInvoker(Initialize));
		}

		void Initialize()
		{
			foreach (var palette in AllPalettes)
			{
				comboBoxKit.Items.Add(palette.Name);
			}
			comboBoxKit.TextChanged += ComboBoxKit_TextChanged;
			comboBoxKit.Text = paletteName;

			if (targetColors == null)
			{
				pictureBoxReplacedThread.Visible = labelReplacedThread.Visible = false;
				pictureBoxDifference.Visible = labelDifference.Visible = false;
				groupBoxColorComparisonMode.Visible = false;
				paletteListView.Columns[ColumnIndexDifference].Width = 0;
				paletteListView.Columns[ColumnIndexDifference].DisplayIndex = 1;
			}

			paletteListView.Columns[ColumnIndexName].Width = -2;
		}

		IEnumerable<CodedPalette> AllPalettes => allPalettes ?? (allPalettes = SAEResources.GetAllPalettes(Settings.Default.UserPalettesLocationSafe).OrderBy(p => p.Name).ToList());
		IList<CodedPalette> allPalettes;

		CodedPalette SelectedPalette
		{
			get { return selectedPalette ?? (SelectedPalette = AllPalettes.FirstOrDefault(palette => palette.Name == paletteName)); }
			set
			{
				if (value != selectedPalette)
				{
					selectedPalette = value;
					foreach (var color in selectedPalette)
					{
						selectedPalette.ChangeColorVisibility(color, (targetColors?.Any(c => c.Equals(color)) ?? false) || (usedColors?.Any(c => c.Equals(color)) ?? false));
					}
					controller = null;
					UpdateListView();
				}
			}
		}
		CodedPalette selectedPalette;

		PaletteController Controller
		{
			get
			{
				if (controller == null)
				{
					controller = new PaletteController(SelectedPalette, null)
					{
						UseVisualColorDiff = radioButtonVisual.Checked,
						TargetColor = TargetColor,
						OrderMode = PaletteController.ColorsOrderMode.Distance,
					};
					controller.PaletteSettingsChanged += Controller_PaletteSettingsChanged;
				}

				return controller;
			}
		}
		PaletteController controller;

		Color TargetColor
		{
			get
			{
				if (targetColor == null)
				{
					if (targetColors == null || targetColors.Count == 0)
					{
						targetColor = null;
					}
					else if (targetColors.Count == 1)
					{
						targetColor = targetColors.First();
					}
					else
					{
						int r = 0, g = 0, b = 0, occurrences = 0;
						foreach (var color in targetColors)
						{
							r += color.R * color.OccurrencesCount;
							g += color.G * color.OccurrencesCount;
							b += color.B * color.OccurrencesCount;
							occurrences += color.OccurrencesCount;
						}
						if (occurrences > 0)
						{
							r /= occurrences;
							if (r > byte.MaxValue) r = byte.MaxValue;

							g /= occurrences;
							if (g > byte.MaxValue) g = byte.MaxValue;

							b /= occurrences;
							if (b > byte.MaxValue) b = byte.MaxValue;

							targetColor = new Color((byte)r, (byte)g, (byte)b);
						}
					}
				}
				return targetColor;
			}
		}
		Color targetColor;

		void Controller_PaletteSettingsChanged(object sender, EventArgs e)
		{
			UpdateListView();
		}

		void UpdateListView()
		{
			if (updatingListView)
			{
				return;
			}
			updatingListView = true;
			try
			{
				paletteListView.VirtualListSize = SelectedPalette?.Count ?? 0;
				paletteListView.Invalidate();
				paletteListView.Update();
			}
			finally
			{
				updatingListView = false;
			}
		}

		bool updatingListView;

		void ComboBoxKit_TextChanged(object sender, EventArgs e)
		{
			paletteName = comboBoxKit.Text;
			selectedPalette = null;
			UpdateListView();
		}

		void paletteListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
		{
			var colorInfo = Controller.GetOrderedColorInfo(e.ItemIndex);

			e.Item =
				new ListViewItem(
					new[]
					{
						"",
						"",
						colorInfo.Code,
						TargetColor == null ? "" : radioButtonVisual.Checked ? TargetColor.GetVisualDistance(colorInfo.Color).ToString() : TargetColor?.GetSquareDistance(colorInfo.Color).ToString(),
						colorInfo.Name
					},
					e.ItemIndex)
				{
					Checked = colorInfo.IsSelected,
					ToolTipText = colorInfo.Name,
					UseItemStyleForSubItems = false
				};
			e.Item.SubItems[ColumnIndexColor].BackColor = System.Drawing.Color.FromArgb(colorInfo.Rgb);
			e.Item.StateImageIndex =
				targetColors != null && targetColors.Contains(colorInfo.Color)
					? 1
					: usedColors != null && usedColors.Contains(colorInfo.Color)
						? 0
						: -1;
		}

		const int ColumnIndexState = 0;
		const int ColumnIndexColor = 1;
		const int ColumnIndexCode = 2;
		const int ColumnIndexDifference = 3;
		const int ColumnIndexName = 4;

		void paletteListView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
		{
			e.DrawDefault = true;
		}

		void paletteListView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
		{
			if (e.ColumnIndex == ColumnIndexColor)
			{
				e.Graphics.FillRectangle(GetBrush(e.SubItem.BackColor.ToArgb()), e.Bounds);
			}
			else
			{
				e.DrawDefault = true;
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

		void paletteListView_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			switch (e.Column)
			{
				case ColumnIndexState:
					Controller.ChangeOrderMode(PaletteController.ColorsOrderMode.Selected);
					break;
				case ColumnIndexColor:
					Controller.ChangeOrderMode(PaletteController.ColorsOrderMode.Darkness);
					break;
				case ColumnIndexCode:
					Controller.ChangeOrderMode(PaletteController.ColorsOrderMode.Code);
					break;
				case ColumnIndexDifference:
					Controller.ChangeOrderMode(PaletteController.ColorsOrderMode.Distance);
					break;
				case ColumnIndexName:
					Controller.ChangeOrderMode(PaletteController.ColorsOrderMode.Name);
					break;
			}
		}

		void paletteListView_SelectedIndexChanged(object sender, EventArgs e)
		{
			var colorInfo = paletteListView.SelectedIndices.Count == 1 ? Controller.GetOrderedColorInfo(paletteListView.SelectedIndices[0]) : null;
			SelectedColor = colorInfo?.Color;
			buttonOk.Enabled = SelectedColor != null && targetColors != null || usedColors == null || !usedColors.Contains(SelectedColor);
		}

		void radioButtonVisual_CheckedChanged(object sender, EventArgs e)
		{
			controller = null;
			UpdateListView();
		}
	}
}
