using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using Ravlyk.SAE.Drawing;
using Ravlyk.SAE.Drawing.Serialization;

namespace PaletteFileEditor
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		void buttonLoad_Click(object sender, EventArgs e)
		{
			using (var openDialog = new OpenFileDialog { Filter = "Thread kits (*.thread)|*.thread|Comma separated values file (*.csv)|*.csv" })
			{
				if (openDialog.ShowDialog(this) == DialogResult.OK)
				{
					var extension = Path.GetExtension(openDialog.FileName);

					if (!string.IsNullOrEmpty(extension) && extension.Equals(".csv", StringComparison.OrdinalIgnoreCase))
					{
						Palette = PaletteCsvImporter.ImportFromCsv(File.ReadAllLines(openDialog.FileName), name: Path.GetFileNameWithoutExtension(openDialog.FileName), indexName: -1);
						Colors = Palette.OrderByCode().ToList();
						listViewPalette.VirtualListSize = Colors.Count;
						listViewPalette.Invalidate();
						listViewPalette.Update();
					}
					else
					{
						using (var stream = new FileStream(openDialog.FileName, FileMode.Open))
						using (var reader = XmlReader.Create(stream))
						{
							Palette = PaletteSerializer.ReadFromXml(reader);
							Colors = Palette.OrderByCode().ToList();
							listViewPalette.VirtualListSize = Colors.Count;
							listViewPalette.Invalidate();
							listViewPalette.Update();
						}
					}
				}
			}
		}

		CodedPalette Palette { get; set; }
		List<CodedColor> Colors { get; set; }

		void listViewPalette_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
		{
			var color = Colors[e.ItemIndex];

			e.Item =
				new ListViewItem(
					new[]
					{
						color.ColorCode,
						color.ColorName,
						"",
						color.R.ToString(),
						color.G.ToString(),
						color.B.ToString()
					},
					e.ItemIndex)
				{
					UseItemStyleForSubItems = false
				};
			e.Item.SubItems[2].BackColor = System.Drawing.Color.FromArgb(color.Argb);
		}

		void buttonSave_Click(object sender, EventArgs e)
		{
			if (Palette == null)
			{
				return;
			}

			using (var saveDialog = new SaveFileDialog { Filter = "Thread kits (*.thread)|*.thread|Comma separated values file (*.csv)|*.csv" })
			{
				if (saveDialog.ShowDialog(this) == DialogResult.OK)
				{
					var extension = Path.GetExtension(saveDialog.FileName);

					if (!string.IsNullOrEmpty(extension) && extension.Equals(".csv", StringComparison.OrdinalIgnoreCase))
					{
						File.WriteAllLines(saveDialog.FileName, Palette.ExportToCsv(true));
					}
					else
					{
						using (var stream = new FileStream(saveDialog.FileName, FileMode.Create))
						using (var writer = XmlWriter.Create(stream))
						{
							Palette.WriteToXml(writer);
						}
					}

					MessageBox.Show("Palette saved to " + saveDialog.FileName);
				}
			}
		}
	}
}
