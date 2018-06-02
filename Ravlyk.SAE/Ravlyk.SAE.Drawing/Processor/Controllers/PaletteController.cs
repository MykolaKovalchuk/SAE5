using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Ravlyk.Adopted.TrueTypeSharp;
using Ravlyk.Common;
using Ravlyk.Drawing;
using Ravlyk.Drawing.ImageProcessor.Utilities;
using Ravlyk.SAE.Drawing.Properties;

namespace Ravlyk.SAE.Drawing.Processor
{
	public class PaletteController
	{
		public PaletteController(CodedPalette palette, TrueTypeFont symbolsFont)
		{
			Palette = palette;
			SymbolsFont = symbolsFont;

			ColorsOrderMode setOrderMode;
			OrderMode = Enum.TryParse(GridPainterSettings.Default.PaletteOrderMode, out setOrderMode) ? setOrderMode : ColorsOrderMode.Code;

			Palette.ColorAttributesChanged += Palette_ColorAttributesChanged;
			Palette.ColorAdded += Palette_ColorChanged;
			Palette.ColorRemoved += Palette_ColorChanged;
			Palette.OccurrencesChanged += Palette_ColorChanged;
		}

		public CodedPalette Palette { get; }
		TrueTypeFont SymbolsFont { get; }

		public void SaveSettings()
		{
			GridPainterSettings.Default.PaletteOrderMode = OrderMode.ToString();
			GridPainterSettings.Default.Save();
		}

		#region Order colors

		public IList<CodedColor> OrderedColors => orderedColors ?? (orderedColors = GetOrderedColors().ToList());
		IList<CodedColor> orderedColors;

		public Color TargetColor { get; set; }

		public enum ColorsOrderMode
		{
			None,
			Symbol,
			Darkness,
			Code,
			Name,
			Count,
			SymbolDesc,
			DarknessDesc,
			CodeDesc,
			NameDesc,
			CountDesc,
			Selected,
			SelectedDesc,
			Distance,
			DistanceDesc
		}

		public ColorsOrderMode OrderMode
		{
			get { return orderMode; }
			set
			{
				orderMode = value;
				OnPaletteSettingsChanged();
			}
		}
		ColorsOrderMode orderMode = ColorsOrderMode.None;

		public void ChangeOrderMode(ColorsOrderMode mode)
		{
			if (mode == OrderMode)
			{
				switch (orderMode)
				{
					case ColorsOrderMode.Symbol:
						mode = ColorsOrderMode.SymbolDesc;
						break;
					case ColorsOrderMode.Darkness:
						mode = ColorsOrderMode.DarknessDesc;
						break;
					case ColorsOrderMode.Code:
						mode = ColorsOrderMode.CodeDesc;
						break;
					case ColorsOrderMode.Name:
						mode = ColorsOrderMode.NameDesc;
						break;
					case ColorsOrderMode.Count:
						mode = ColorsOrderMode.CountDesc;
						break;
					case ColorsOrderMode.Selected:
						mode = ColorsOrderMode.SelectedDesc;
						break;
					case ColorsOrderMode.SymbolDesc:
						mode = ColorsOrderMode.Symbol;
						break;
					case ColorsOrderMode.DarknessDesc:
						mode = ColorsOrderMode.Darkness;
						break;
					case ColorsOrderMode.CountDesc:
						mode = ColorsOrderMode.Code;
						break;
					case ColorsOrderMode.CodeDesc:
						mode = ColorsOrderMode.Count;
						break;
					case ColorsOrderMode.NameDesc:
						mode = ColorsOrderMode.Name;
						break;
					case ColorsOrderMode.SelectedDesc:
						mode = ColorsOrderMode.Selected;
						break;
					case ColorsOrderMode.Distance:
						mode = ColorsOrderMode.DistanceDesc;
						break;
					case ColorsOrderMode.DistanceDesc:
						mode = ColorsOrderMode.Distance;
						break;
				}
			}

			OrderMode = mode;
		}

		IEnumerable<CodedColor> GetOrderedColors()
		{
			switch (orderMode)
			{
				case ColorsOrderMode.Symbol:
					return Palette.OrderBySymbol();
				case ColorsOrderMode.Darkness:
					return Palette.OrderByDarknes().Cast<CodedColor>();
				case ColorsOrderMode.Code:
					return Palette.OrderByCode();
				case ColorsOrderMode.Name:
					return Palette.Cast<CodedColor>().OrderBy(c => c.ColorName);
				case ColorsOrderMode.Count:
					return Palette.OrderByCount().Cast<CodedColor>();
				case ColorsOrderMode.SymbolDesc:
					return Palette.OrderBySymbol(true);
				case ColorsOrderMode.DarknessDesc:
					return Palette.OrderByDarknes(true).Cast<CodedColor>();
				case ColorsOrderMode.CountDesc:
					return Palette.OrderByCount(true).Cast<CodedColor>();
				case ColorsOrderMode.CodeDesc:
					return Palette.OrderByCode(true);
				case ColorsOrderMode.NameDesc:
					return Palette.Cast<CodedColor>().OrderByDescending(c => c.ColorName);
				case ColorsOrderMode.Selected:
					return Palette.Cast<CodedColor>().OrderBy(c => c.Hidden);
				case ColorsOrderMode.SelectedDesc:
					return Palette.Cast<CodedColor>().OrderBy(c => !c.Hidden);
				case ColorsOrderMode.Distance:
					return TargetColor != null ? Palette.OrderByDiff(TargetColor, UseVisualColorDiff).Cast<CodedColor>() : Palette;
				case ColorsOrderMode.DistanceDesc:
					return TargetColor != null ? Palette.OrderByDiff(TargetColor, UseVisualColorDiff, true).Cast<CodedColor>() : Palette;
				default:
					return Palette;
			}
		}

		public bool UseVisualColorDiff { get; set; } = true;

		#endregion

		#region Ordered color info

		public ColorInfo GetOrderedColorInfo(int index)
		{
			if (index < 0 || index >= OrderedColors.Count)
			{
				return null;
			}

			return new ColorInfo(OrderedColors[index], this);
		}

		public class ColorInfo
		{
			internal ColorInfo(CodedColor color, PaletteController controller)
			{
				Color = color;
				Controller = controller;
			}

			public CodedColor Color { get; }
			public PaletteController Controller { get; }

			public char Symbol => Color.SymbolChar;
			public string Code => Color.ColorCode;
			public string Name => Color.ColorName;
			public int Rgb => Color.Argb;
			public int Count => Color.UsageOccurrences.Count;

			public bool IsSelected
			{
				get { return !Color.Hidden; }
				set
				{
					Color.Hidden = !value;
					Controller.OnPaletteSettingsChanged();
				}
			}
		}

		#endregion

		#region Symbols painters

		public IndexedImage GetSymbolImage(char symbol, int imageSize, int fontRgb = 0, int backgroundRgb = 0x00ffffff)
		{
			symbolImageSize = imageSize;
			var symbolImage = new IndexedImage { Size = new Size(imageSize, imageSize) };
			SymbolsPainter.PaintSymbol(symbol, symbolImage.Pixels, symbolImage.Size, default(Point), fontRgb: fontRgb, backgroundRgb: backgroundRgb);
			return symbolImage;
		}

		CachedSymbolPainter SymbolsPainter
		{
			get
			{
				if (symbolsPainter == null || symbolsPainter.Font != SymbolsFont || symbolsPainter.PixelHeight != symbolImageSize)
				{
					symbolsPainter = new CachedSymbolPainter(SymbolsFont, symbolImageSize);
				}
				return symbolsPainter;
			}
		}
		CachedSymbolPainter symbolsPainter;
		int symbolImageSize;

		#endregion

		#region Replace color

		public void ReplaceColor(CodedImage image, CodedColor oldColor, CodedColor newColor)
		{
			Debug.Assert(ReferenceEquals(Palette, image.Palette), "Image should have same palette.");

			if (oldColor != newColor)
			{
				foreach (var occurrence in oldColor.UsageOccurrences.ToArray())
				{
					var location = image.GetPixelLocationFromIndex(occurrence);
					image[location.X, location.Y] = newColor;
				}
				Palette.Remove(oldColor.GetHashCode());
			}
		}

		#endregion

		#region OnChanged event

		void OnPaletteSettingsChanged()
		{
			orderedColors = null;
			PaletteSettingsChanged?.Invoke(this, EventArgs.Empty);
		}

		public event EventHandler PaletteSettingsChanged;

		void Palette_ColorChanged(object sender, Palette.ColorChangedEventArgs e)
		{
			OnPaletteSettingsChanged();
		}

		void Palette_ColorAttributesChanged(object sender, CodedPalette.ColorAttributesChangedEventArgs e)
		{
			OnPaletteSettingsChanged();
		}

		#endregion
	}
}
