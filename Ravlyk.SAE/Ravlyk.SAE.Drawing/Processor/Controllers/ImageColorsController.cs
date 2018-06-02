using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Ravlyk.Drawing.ImageProcessor.Utilities;
using Ravlyk.SAE.Drawing.Properties;

namespace Ravlyk.SAE.Drawing.Processor
{
	public class ImageColorsController : ImageController<ImageColorsManipulator>
	{
		public ImageColorsController(ImageColorsManipulator manipulator, IEnumerable<CodedPalette> availablePalettes = null, CodedPalette selectedPalette = null) : base(manipulator)
		{
			Debug.Assert(selectedPalette == null || availablePalettes != null && availablePalettes.Contains(selectedPalette), "selectedPalette should be included into availablePalettes collection.");

			using (SuspendCallManipulations())
			{
				AvailablePalettes = availablePalettes?.ToList();
				SelectedPalette = selectedPalette ?? AvailablePalettes?.FirstOrDefault();

				RestoreDefaults();
				CallManipulations();
			}
		}

		public int DitherLevel
		{
			get { return ditherLevel; }
			set
			{
				if (value < 0) { value = 0; }
				if (value > 10) { value = 10; }
				if (ditherLevel == value)
				{
					return;
				}

				ditherLevel = value;

				CallManipulations();

				OnPropertyChanged(nameof(DitherLevel));
			}
		}
		int ditherLevel = 0;

		public bool EnsureBlackAndWhiteColors
		{
			get { return ensureBlackAndWhiteColors; }
			set
			{
				if (ensureBlackAndWhiteColors == value)
				{
					return;
				}

				ensureBlackAndWhiteColors = value;

				CallManipulations();

				OnPropertyChanged(nameof(EnsureBlackAndWhiteColors));
			}
		}
		bool ensureBlackAndWhiteColors = false;

		public enum ColorComparisonTypes
		{
			Visual,
			Exact
		}

		public ColorComparisonTypes ColorComparisonType
		{
			get { return colorComparisonType; }
			set
			{
				if (colorComparisonType == value)
				{
					return;
				}

				colorComparisonType = value;

				switch (colorComparisonType)
				{
					case ColorComparisonTypes.Visual:
						ColorDistance.SetCoeffs(30, 59, 11);
						break;
					case ColorComparisonTypes.Exact:
						ColorDistance.SetCoeffs(1, 1, 1);
						break;
				}

				CallManipulations();

				OnPropertyChanged(nameof(ColorComparisonType));
			}
		}
		ColorComparisonTypes colorComparisonType = ColorComparisonTypes.Visual;

		public IList<CodedPalette> AvailablePalettes { get; private set; }

		public IEnumerable<string> AvailablePalettesNames
		{
			get { return AvailablePalettes?.Select(palette => palette.Name); }
		}

		public void AddColorPalettes(IEnumerable<CodedPalette> newPalettes, bool replace = false)
		{
			Debug.Assert(newPalettes != null, "New palettes collection should not be null.");

			if (replace)
			{
				AvailablePalettes = null;
			}

			if (AvailablePalettes == null)
			{
				AvailablePalettes = newPalettes.ToList();
			}
			else
			{
				foreach (var newPalette in newPalettes)
				{
					if (!AvailablePalettes.Any(palette => palette.Name.Equals(newPalette.Name, StringComparison.OrdinalIgnoreCase)))
					{
						AvailablePalettes.Add(newPalette);
					}
				}
			}

			if (SelectedPalette == null || replace)
			{
				var defaultPalette = (SelectedPalette != null ? AvailablePalettes.FirstOrDefault(p => p.Name.Equals(SelectedPalette.Name, StringComparison.OrdinalIgnoreCase)) : null)
					?? AvailablePalettes.FirstOrDefault();
				selectedPalette = null;
				if (defaultPalette != null)
				{
					PaletteName = defaultPalette.Name;
				}
			}
		}

		public CodedPalette SelectedPalette
		{
			get { return selectedPalette; }
			private set
			{
				selectedPalette = value;

				if (maxColorsCount > MaxAvailableColorsCount)
				{
					maxColorsCount = MaxAvailableColorsCount;
				}

				CallManipulations();

				OnPropertyChanged(nameof(PaletteName));
				OnPropertyChanged(nameof(MaxColorsCount));
				OnPropertyChanged(nameof(MaxAvailableColorsCount));
			}
		}
		CodedPalette selectedPalette;

		public string PaletteName
		{
			get { return SelectedPalette?.Name ?? string.Empty; }
			set
			{
				if (PaletteName.Equals(value, StringComparison.OrdinalIgnoreCase))
				{
					return;
				}

				var newPalette = AvailablePalettes?.FirstOrDefault(palette => palette.Name.Equals(value, StringComparison.OrdinalIgnoreCase));
				if (newPalette == null)
				{
					return;
				}

				SelectedPalette = newPalette;
			}
		}

		public int MaxColorsCount
		{
			get { return maxColorsCount; }
			set
			{
				if (value < 2) { value = 2; }
				if (value > MaxAvailableColorsCount) { value = MaxAvailableColorsCount; }
				if (maxColorsCount == value)
				{
					return;
				}

				maxColorsCount = value;

				CallManipulations();

				OnPropertyChanged(nameof(MaxColorsCount));
			}
		}
		int maxColorsCount = 30;

		public int MaxAvailableColorsCount => SelectedPalette?.Count ?? MaximumAvailableColorsCountWithoutPalette;

		public const int MaximumAvailableColorsCountWithoutPalette = 200;

		protected override void CallManipulationsCore()
		{
			Manipulator.QuantizeColors(MaxColorsCount, SelectedPalette, ditherLevel, ensureBlackAndWhiteColors);
		}

		#region Defaults

		protected override void RestoreDefaultsCore()
		{
			using (SuspendCallManipulations())
			{
				PaletteName = SAEWizardSettings.Default.PaletteName;
				DitherLevel = SAEWizardSettings.Default.DitherLevel;
				EnsureBlackAndWhiteColors = SAEWizardSettings.Default.EnsureBlackAndWhiteColors;
				MaxColorsCount = SAEWizardSettings.Default.MaxColors;

				ColorComparisonTypes defaultComparisonType;
				if (Enum.TryParse(SAEWizardSettings.Default.ColorComparisonType, out defaultComparisonType))
				{
					ColorComparisonType = defaultComparisonType;
				}
			}
		}

		protected override void SaveDefaultsCore()
		{
			SAEWizardSettings.Default.PaletteName = PaletteName;
			SAEWizardSettings.Default.DitherLevel = DitherLevel;
			SAEWizardSettings.Default.EnsureBlackAndWhiteColors = EnsureBlackAndWhiteColors;
			SAEWizardSettings.Default.MaxColors = MaxColorsCount;
			SAEWizardSettings.Default.ColorComparisonType = ColorComparisonType.ToString();
		}

		protected override void SaveImageSettingsCore(CodedImage image)
		{
			base.SaveImageSettingsCore(image);
			image.AdditionalData[nameof(MaxColorsCount)] = MaxColorsCount.ToString();
			image.AdditionalData[nameof(ColorComparisonType)] = ColorComparisonType.ToString();
			image.AdditionalData[nameof(EnsureBlackAndWhiteColors)] = EnsureBlackAndWhiteColors.ToString();
			image.AdditionalData[nameof(DitherLevel)] = DitherLevel.ToString();
		}

		protected override void RestoreImageSettingsCore(CodedImage image)
		{
			base.RestoreImageSettingsCore(image);

			using (SuspendCallManipulations())
			{
				PaletteName = image.Palette.Name;

				int maxColorsValue;
				MaxColorsCount = image.TryGetAdditionalValueAsInt(nameof(MaxColorsCount), out maxColorsValue) ? maxColorsValue : image.Palette.Count;

				string comparisonTypeValue;
				if (image.TryGetAdditionalDataAsString(nameof(ColorComparisonType), out comparisonTypeValue))
				{
					ColorComparisonTypes comparisonType;
					if (Enum.TryParse(comparisonTypeValue, out comparisonType))
					{
						ColorComparisonType = comparisonType;
					}
				}

				bool ensureBlackAndWhiteValue;
				if (image.TryGetAdditionalValueAsBool(nameof(EnsureBlackAndWhiteColors), out ensureBlackAndWhiteValue))
				{
					EnsureBlackAndWhiteColors = ensureBlackAndWhiteValue;
				}

				int ditherLevelValue;
				if (image.TryGetAdditionalValueAsInt(nameof(DitherLevel), out ditherLevelValue))
				{
					DitherLevel = ditherLevelValue;
				}
			}
		}

		#endregion
	}
}

