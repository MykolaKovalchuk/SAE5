using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using Ravlyk.Drawing;
using Ravlyk.Drawing.WinForms;
using Ravlyk.SAE.Drawing;
using Ravlyk.SAE.Drawing.Serialization;
using Ravlyk.SAE.Resources;
using Ravlyk.SAE5.WinForms.Properties;

namespace Ravlyk.SAE5.WinForms.Dialogs
{
	public partial class ThreadsManagementDialog : Form
	{
		public ThreadsManagementDialog()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			BeginInvoke(new MethodInvoker(Initialize));
		}

		void Initialize()
		{
			ribbonThreads.Height = 120;

			gridViewPalettes.AutoGenerateColumns = false;
			gridViewThreads.AutoGenerateColumns = false;

			gridViewPalettes.DataSource = AllPalettes;
			gridViewPalettes.Select();
		}

		#region PalettesData

		public class PaletteData
		{
			internal PaletteData(CodedPalette palette)
			{
				Palette = palette;
			}

			public CodedPalette Palette { get; }

			public string PaletteName
			{
				get { return Palette.Name; }
				set { Palette.Name = value; }
			}

			public bool IsSystem => Palette.IsSystem;
		}

		SortableBindingList<PaletteData> AllPalettes => allPalettes ??
			(allPalettes =
				new SortableBindingList<PaletteData>(SAEResources.GetAllPalettes(Settings.Default.UserPalettesLocationSafe).OrderBy(palette => palette.Name).Select(palette => new PaletteData(palette)).ToList())
				{
					AllowNew = false,
					AllowEdit = true,
					AllowRemove = false,
					RaiseListChangedEvents = true
				});
		SortableBindingList<PaletteData> allPalettes;

		#endregion

		#region Palettes grid

		void gridViewPalettes_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
		{
			foreach (DataGridViewRow row in gridViewPalettes.Rows)
			{
				if (((PaletteData)row.DataBoundItem).IsSystem)
				{
					row.ReadOnly = true;
				}
			}
		}

		void gridViewPalettes_CurrentCellChanged(object sender, EventArgs e)
		{
			var selectedPalette = (gridViewPalettes.CurrentRow?.DataBoundItem as PaletteData)?.Palette;
			buttonDeletePalette.Enabled = buttonCopyThread.Enabled = !selectedPalette?.IsSystem ?? false;
			BindThreadsGridView(selectedPalette);
		}

		#endregion

		#region Threads grid

		void BindThreadsGridView(CodedPalette selectedPalette)
		{
			if (selectedPalette != currentPalette || selectedPalette == null)
			{
				SaveChangesInCurrentPalette();

				var oldDataSource = gridViewThreads.DataSource as BindingList<CodedColor>;
				if (oldDataSource != null)
				{
					oldDataSource.AddingNew -= ThreadsDataSource_AddingNew;
				}

				try
				{
					var editable = !selectedPalette?.IsSystem ?? false;
					currentPalette = selectedPalette;
					if (currentPalette != null)
					{
						var newDataSource = new SortableBindingList<CodedColor>(currentPalette.ToList<CodedColor>())
						{
							AllowNew = editable,
							AllowEdit = editable,
							AllowRemove = editable,
							RaiseListChangedEvents = editable
						};
						newDataSource.AddingNew += ThreadsDataSource_AddingNew;

						gridViewThreads.DataSource = newDataSource;
					}
					else
					{
						gridViewThreads.DataSource = null;
					}
					gridViewThreads.ReadOnly = !editable;
					gridViewThreads.SelectionMode = gridViewThreads.ReadOnly ? DataGridViewSelectionMode.FullRowSelect : DataGridViewSelectionMode.RowHeaderSelect;
				}
				catch
				{
					currentPalette = null;
					gridViewThreads.DataSource = null;
				}
			}
		}

		void ThreadsDataSource_AddingNew(object sender, AddingNewEventArgs e)
		{
			var dataSource = gridViewThreads.DataSource as ICollection;
			e.NewObject = new CodedColor((dataSource?.Count ?? 0) | 0x01000000);
		}

		CodedPalette currentPalette;
		bool HasChanges { get; set; }

		void SaveChangesInCurrentPalette()
		{
			if (currentPalette != null && !currentPalette.IsSystem)
			{
				var editedColors = gridViewThreads.DataSource as ICollection<CodedColor>;

				Debug.Assert(editedColors != null || gridViewThreads.DataSource == null, "gridViewThreads.DataSource has value of unsupported type.");

				if (editedColors != null)
				{
					if (currentPalette.Count != editedColors.Count)
					{
						HasChanges = true;
					}
					if (!HasChanges)
					{
						foreach (var newColor in editedColors)
						{
							var oldColor = currentPalette[newColor.GetHashCode()];
							if (oldColor == null || oldColor.Argb != newColor.Argb || oldColor.ColorCode != newColor.ColorCode || oldColor.ColorName != newColor.ColorName)
							{
								HasChanges = true;
								break;
							}
						}
					}

					currentPalette.Clear();
					foreach (var color in editedColors)
					{
						currentPalette.Add(color);
					}
				}
			}
		}

		void gridViewThreads_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
		{
			if (e.ColumnIndex == ColumnColor.Index && e.RowIndex >= 0)
			{
				var row = gridViewThreads.Rows[e.RowIndex];
				var color = row.DataBoundItem as CodedColor;
				if (color != null)
				{
					e.Graphics.FillRectangle(GetBrush(color.Argb), e.CellBounds);
					e.Handled = true;
				}
			}
		}

		void gridViewThreads_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex == ColumnSelectColor.Index && e.RowIndex >= 0)
			{
				var row = gridViewThreads.Rows[e.RowIndex];
				var color = row.DataBoundItem as CodedColor;
				if (color != null)
				{
					using (var colorDialog = new ColorDialog())
					{
						colorDialog.Color = System.Drawing.Color.FromArgb(color.R, color.G, color.B);
						colorDialog.FullOpen = true;

						if (colorDialog.ShowDialog(this) == DialogResult.OK && !gridViewThreads.ReadOnly)
						{
							var newArgb = colorDialog.Color.ToArgb();
							if (newArgb != color.Argb)
							{
								var newColor = new CodedColor(newArgb.Red(), newArgb.Green(), newArgb.Blue()) { ColorCode = color.ColorCode, ColorName = color.ColorName, SymbolChar = color.SymbolChar };
								var colorsList = gridViewThreads.DataSource as IList<CodedColor>;
								Debug.Assert(colorsList != null, "gridViewThreads has some data with DataSource null or unsupported type.");
								if (colorsList != null)
								{
									colorsList[e.RowIndex] = newColor;
									gridViewThreads.UpdateCellValue(ColumnColor.Index, e.RowIndex);

									HasChanges = true;
								}
							}
						}
					}
				}
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

		#endregion

		#region Dispose

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (brushes != null)
				{
					foreach (var brush in brushes.Values)
					{
						brush.Dispose();
					}
				}
			}
			base.Dispose(disposing);
		}

		#endregion

		#region Save

		void buttonSaveAll_Click(object sender, EventArgs e)
		{
			SaveChangesInCurrentPalette();
			SaveAll();
		}

		bool SaveAll()
		{
			HasChanges = false;

			foreach (var paletteData in AllPalettes.Where(p => !p.IsSystem))
			{
				var palette = paletteData.Palette;
				if (string.IsNullOrEmpty(palette.FileName))
				{
					continue;
				}

				try
				{
					using (var stream = new FileStream(palette.FileName, FileMode.Create))
					using (var writer = XmlWriter.Create(stream))
					{
						palette.WriteToXml(writer);
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(Resources.ErrorCannotSaveFile + Environment.NewLine + ex.Message);
					HasChanges = true;
				}
			}

			return !HasChanges;
		}

		void ThreadsManagementDialog_FormClosing(object sender, FormClosingEventArgs e)
		{
			SaveChangesInCurrentPalette();

			if (HasChanges)
			{
				var answer = MessageBox.Show(Resources.QuestionSavePalettesBeforeClosing, AppInfo.AppDescription, MessageBoxButtons.YesNoCancel);
				if (answer == DialogResult.Cancel)
				{
					e.Cancel = true;
				}
				else if (answer == DialogResult.Yes)
				{
					e.Cancel = !SaveAll();
				}
			}
		}

		void buttonCancelAll_Click(object sender, EventArgs e)
		{
			allPalettes = null;
			currentPalette = null;
			BindThreadsGridView(null);
			HasChanges = false;
		}

		void buttonUserPalettesLocation_Click(object sender, EventArgs e)
		{
			using (var optionsDialog = new OpitonsDialog())
			{
				optionsDialog.tabControlOptinos.SelectedTab = optionsDialog.tabPageThreads;
				if (optionsDialog.ShowDialog(this) == DialogResult.OK)
				{
					buttonCancelAll_Click(sender, e);
				}
			}
		}

		#endregion

		#region Add/remove palette

		void buttonAddPalette_Click(object sender, EventArgs e)
		{
			using (var inputDialog = new InputDialog(Resources.QueryEnterNewPaletteName))
			{
				if (inputDialog.ShowDialog(this) == DialogResult.OK)
				{
					var newName = inputDialog.Answer.Trim();

					if (string.IsNullOrEmpty(newName))
					{
						return;
					}

					if (AllPalettes.Any(p => p.PaletteName.Equals(newName, StringComparison.CurrentCultureIgnoreCase)))
					{
						MessageBox.Show(Resources.ErrorSelectedPaletteNameAlreadyUsed);
						return;
					}

					var palette = new CodedPalette { Name = newName, IsSystem = false, FileName = Path.Combine(Settings.Default.UserPalettesLocationSafe, newName + SAEResources.ThreadFileExtension) };
					AddNewPalette(palette);
				}
			}
		}

		void AddNewPalette(CodedPalette newPalette)
		{
			var paletteData = new PaletteData(newPalette);
			AllPalettes.Add(paletteData);
			gridViewPalettes.CurrentCell = gridViewPalettes.Rows[AllPalettes.IndexOf(paletteData)].Cells[0];

			HasChanges = true;
		}

		void buttonDeletePalette_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(Resources.QuestionDeletePalette, Resources.CaptionDeletePalette, MessageBoxButtons.YesNo) != DialogResult.Yes)
			{
				return;
			}

			var paletteData = gridViewPalettes.CurrentRow?.DataBoundItem as PaletteData;
			var selectedPalette = paletteData?.Palette;
			if (selectedPalette != null && !selectedPalette.IsSystem)
			{
				if (File.Exists(selectedPalette.FileName))
				{
					try
					{
						File.Delete(selectedPalette.FileName);
					}
					catch (Exception ex)
					{
						MessageBox.Show(Resources.ErrorCannotDeletePaletteFile + Environment.NewLine + ex.Message);
						return;
					}
				}

				AllPalettes.AllowRemove = true;
				AllPalettes.Remove(paletteData);
				AllPalettes.AllowRemove = false;

				HasChanges = true;
			}
		}

		void buttonCopyPalette_Click(object sender, EventArgs e)
		{
			var paletteData = gridViewPalettes.CurrentRow?.DataBoundItem as PaletteData;
			var selectedPalette = paletteData?.Palette;
			if (selectedPalette == null)
			{
				return;
			}

			using (var inputDialog = new InputDialog(Resources.QueryEnterNewPaletteName))
			{
				if (inputDialog.ShowDialog(this) == DialogResult.OK)
				{
					var newName = inputDialog.Answer.Trim();

					if (string.IsNullOrEmpty(newName))
					{
						return;
					}

					if (AllPalettes.Any(p => p.PaletteName.Equals(newName, StringComparison.CurrentCultureIgnoreCase)))
					{
						MessageBox.Show(Resources.ErrorSelectedPaletteNameAlreadyUsed);
						return;
					}

					var newPalette = selectedPalette.Clone();
					newPalette.Name = newName;
					newPalette.IsSystem = false;
					newPalette.FileName = Path.Combine(Settings.Default.UserPalettesLocationSafe, newName + SAEResources.ThreadFileExtension);

					AddNewPalette(newPalette);
				}
			}
		}

		#endregion

		#region Import

		void buttonImportPalette_Click(object sender, EventArgs e)
		{
			using (var openDialog = new OpenFileDialog { Filter = Resources.FileFilterAllPalettes })
			{
				if (openDialog.ShowDialog(this) == DialogResult.OK)
				{
					try
					{
						CodedPalette newPalette;

						var extension = Path.GetExtension(openDialog.FileName);

						if (!string.IsNullOrEmpty(extension) && extension.Equals(".csv", StringComparison.OrdinalIgnoreCase))
						{
							var newName = Path.GetFileNameWithoutExtension(openDialog.FileName);
							newPalette = PaletteCsvImporter.ImportFromCsv(File.ReadAllLines(openDialog.FileName), name: newName);
							newPalette.IsSystem = false;
							newPalette.FileName = newPalette.FileName = Path.Combine(Settings.Default.UserPalettesLocationSafe, newName + SAEResources.ThreadFileExtension);
						}
						else
						{
							using (var stream = new FileStream(openDialog.FileName, FileMode.Open))
							using (var reader = XmlReader.Create(stream))
							{
								var newName = Path.GetFileNameWithoutExtension(openDialog.FileName);
								newPalette = PaletteSerializer.ReadFromXml(reader);
								newPalette.Name = newName;
								newPalette.IsSystem = false;
								newPalette.FileName = Path.Combine(Settings.Default.UserPalettesLocationSafe, newName + SAEResources.ThreadFileExtension);
							}
						}

						AddNewPalette(newPalette);
					}
					catch (Exception ex)
					{
						MessageBox.Show(Resources.ErrorCannotOpenFile + Environment.NewLine + ex.Message);
					}
				}
			}
		}

		void buttonExportPalette_Click(object sender, EventArgs e)
		{
			var paletteData = gridViewPalettes.CurrentRow?.DataBoundItem as PaletteData;
			var selectedPalette = paletteData?.Palette;
			if (selectedPalette == null)
			{
				return;
			}

			using (var saveDialog = new SaveFileDialog { Filter = Resources.FileFilterPalette })
			{
				if (saveDialog.ShowDialog(this) == DialogResult.OK)
				{
					var extension = Path.GetExtension(saveDialog.FileName);

					try
					{
						if (!string.IsNullOrEmpty(extension) && extension.Equals(".csv", StringComparison.OrdinalIgnoreCase))
						{
							File.WriteAllLines(saveDialog.FileName, selectedPalette.ExportToCsv(true));
						}
						else
						{
							using (var stream = new FileStream(saveDialog.FileName, FileMode.Create))
							using (var writer = XmlWriter.Create(stream))
							{
								selectedPalette.WriteToXml(writer);
							}
						}

						MessageBox.Show(string.Format(Resources.MessagePaletteSavedTo, saveDialog.FileName));
					}
					catch (Exception ex)
					{
						MessageBox.Show(Resources.ErrorCannotSaveFile + Environment.NewLine + ex.Message);
					}
				}
			}
		}

		#endregion

		#region Add thread

		void buttonCopyThread_Click(object sender, EventArgs e)
		{
			var paletteData = gridViewPalettes.CurrentRow?.DataBoundItem as PaletteData;
			var selectedPalette = paletteData?.Palette;
			if (selectedPalette == null)
			{
				return;
			}

			using (var threadsDialog = new ThreadsDialog(selectedPalette.Name, null, null))
			{
				threadsDialog.ChangeHint(Resources.DialogHintAddNewThread);
				if (threadsDialog.ShowDialog(this) == DialogResult.OK && threadsDialog.SelectedColor != null)
				{
					var colorsList = gridViewThreads.DataSource as BindingList<CodedColor>;
					if (colorsList != null)
					{
						var newColor = threadsDialog.SelectedColor.Clone();
						colorsList.Add(newColor);
						gridViewThreads.CurrentCell = gridViewThreads.Rows[colorsList.IndexOf(newColor)].Cells[0];

						HasChanges = true;
					}
				}
			}
		}

		#endregion
	}
}
