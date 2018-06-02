using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ravlyk.Common;
using Ravlyk.SAE.Drawing.Properties;

namespace Ravlyk.SAE.Drawing.Processor.Test
{
	[TestFixture]
	public class ImageColorsControllerTest
	{
		[Test]
		public void TestDoesntCallManipulationsWithoutChanges()
		{
			var image = new CodedImage { Size = new Size(5, 5) };
			var colorer = new ImageColorsController4Test(new ImageColorsManipulator(image));

			Assert.AreEqual(0, colorer.DitherLevel, "Default value");
			Assert.IsFalse(colorer.EnsureBlackAndWhiteColors, "Default value");
			Assert.AreEqual(30, colorer.MaxColorsCount, "Default value");
			Assert.IsTrue(colorer.CallManipulationsCoreFired, "Manipulations should have been already executed for default options.");

			colorer.CallManipulationsCoreFired = false;
			colorer.DitherLevel = 0;
			Assert.IsFalse(colorer.CallManipulationsCoreFired, "Should not call manipulations when there are no changes.");
			colorer.DitherLevel = 1;
			Assert.IsTrue(colorer.CallManipulationsCoreFired, "Should call manipulations for changed dither level.");

			colorer.CallManipulationsCoreFired = false;
			colorer.EnsureBlackAndWhiteColors = false;
			Assert.IsFalse(colorer.CallManipulationsCoreFired, "Should not call manipulations when there are no changes.");
			colorer.EnsureBlackAndWhiteColors = true;
			Assert.IsTrue(colorer.CallManipulationsCoreFired, "Should call manipulations for changed ensure special colors.");

			colorer.CallManipulationsCoreFired = false;
			colorer.MaxColorsCount = 30;
			Assert.IsFalse(colorer.CallManipulationsCoreFired, "Should not call manipulations when there are no changes.");
			colorer.MaxColorsCount = 50;
			Assert.IsTrue(colorer.CallManipulationsCoreFired, "Should call manipulations for changed max colors count.");
		}

		[Test]
		public void TestCallManipulators()
		{
			var image = new CodedImage { Size = new Size(50, 50) };
			image.CompletePalette();
			for (int x = 0; x < 50; x += 5)
			{
				for (int y = 0; y < 50; y++)
				{
					image[x, y] = new CodedColor(0);
					image[x + 1, y] = new CodedColor(255, 255, 255);
					image[x + 2, y] = new CodedColor(255, 0, 0);
					image[x + 3, y] = new CodedColor(127, 0, 0);
					image[x + 4, y] = new CodedColor(0, 255, 0);
				}
			}

			image.CompletePalette();
			Assert.AreEqual(5, image.Palette.Count, "Precondition: there are 5 different colors in source image.");

			var palette1 = new CodedPalette { Name = "Palette 1" };
			palette1.Add(new CodedColor(0));
			palette1.Add(new CodedColor(255, 255, 255));
			palette1.Add(new CodedColor(255, 0, 0));

			var palette2 = new CodedPalette { Name = "Palette 2" };
			palette2.Add(new CodedColor(0));
			palette2.Add(new CodedColor(255, 255, 255));

			var colorer = new ImageColorsController(new ImageColorsManipulator(image), new[] { palette1, palette2 }, palette1);
			colorer.Manipulator.ManipulatedImage.CompletePalette();
			Assert.AreEqual(3, colorer.Manipulator.ManipulatedImage.Palette.Count, "Colors reducing manipulation should be applied.");

			colorer.PaletteName = "Palette 2";
			colorer.Manipulator.ManipulatedImage.CompletePalette();
			Assert.AreEqual(2, colorer.Manipulator.ManipulatedImage.Palette.Count, "Colors reducing manipulation should be applied with new maximum available colors number.");
		}

		[Test]
		public void TestDitherLevelInCorrectLimits()
		{
			var image = new CodedImage { Size = new Size(5, 5) };
			var colorer = new ImageColorsController4Test(new ImageColorsManipulator(image));

			for (int i = -5; i < 20; i++)
			{
				colorer.DitherLevel = i;
				Assert.AreEqual(i < 0 ? 0 : i > 10 ? 10 : i, colorer.DitherLevel, "DitherLevel should be within valid range.");
			}
		}

		[Test]
		public void TestAvailablePaletteNames()
		{
			var image = new CodedImage { Size = new Size(5, 5) };
			var palette1 = new CodedPalette { Name = "Palette 1" };
			var palette2 = new CodedPalette { Name = "Palette 2" };
			var colorer = new ImageColorsController4Test(new ImageColorsManipulator(image), new[] { palette2, palette1 });

			var availablePalettes = colorer.AvailablePalettes.ToArray();

			Assert.AreEqual("Palette 2", availablePalettes[0].Name, "Should return all names in original order.");
			Assert.AreEqual("Palette 1", availablePalettes[1].Name, "Should return all names in original order.");
		}

		[Test]
		public void TestPaletteName()
		{
			var image = new CodedImage { Size = new Size(5, 5) };
			var palette1 = new CodedPalette { Name = "Palette 1" };
			var palette2 = new CodedPalette { Name = "Palette 2" };
			var colorer = new ImageColorsController4Test(new ImageColorsManipulator(image), new[] { palette1, palette2 });

			Assert.AreSame(palette1, colorer.SelectedPalette, "First palette should be selected by default.");
			Assert.AreEqual("Palette 1", colorer.PaletteName, "Name of current palette should be returned.");

			colorer.PaletteName = "Palette 2";
			Assert.AreSame(palette2, colorer.SelectedPalette, "Second palette should be selected.");
			Assert.AreEqual("Palette 2", colorer.PaletteName, "Name of current palette should be returned.");

			colorer.PaletteName = "Palette 3";
			Assert.AreSame(palette2, colorer.SelectedPalette, "Previous palette should remain selected.");
			Assert.AreEqual("Palette 2", colorer.PaletteName, "Name of current palette should be returned.");

			colorer.PaletteName = "pAlEtTe 1";
			Assert.AreSame(palette1, colorer.SelectedPalette, "First palette should be selected ignoring characters case.");
			Assert.AreEqual("Palette 1", colorer.PaletteName, "Name of current palette should be returned.");
		}

		[Test]
		public void TestAddColorPalettes()
		{
			var image = new CodedImage { Size = new Size(5, 5) };
			var colorer = new ImageColorsController4Test(new ImageColorsManipulator(image));

			Assert.IsNull(colorer.AvailablePalettes);
			Assert.IsNull(colorer.AvailablePalettesNames);
			Assert.IsNull(colorer.SelectedPalette);

			var palette1 = new CodedPalette { Name = "Palette 1" };
			var palette2 = new CodedPalette { Name = "Palette 2" };

			colorer.AddColorPalettes(new[] { palette1, palette2 });

			Assert.AreEqual(2, colorer.AvailablePalettes.Count());
			Assert.AreEqual(2, colorer.AvailablePalettesNames.Count());
			Assert.AreSame(palette1, colorer.SelectedPalette);
			Assert.AreEqual("Palette 1", colorer.PaletteName);

			var palette3 = new CodedPalette { Name = "Palette 3" };

			colorer.AddColorPalettes(new[] { palette3 });

			Assert.AreEqual(3, colorer.AvailablePalettes.Count());
			Assert.AreEqual(3, colorer.AvailablePalettesNames.Count());
			Assert.AreSame(palette1, colorer.SelectedPalette, "Should keep already selected palette.");
			Assert.AreEqual("Palette 1", colorer.PaletteName, "Should keep already selected palette.");
		}

		[Test]
		public void TestMaxColorsCountInCorrectLimits()
		{
			var image = new CodedImage { Size = new Size(5, 5) };
			var colorer = new ImageColorsController4Test(new ImageColorsManipulator(image));
			Assert.AreEqual(ImageColorsController.MaximumAvailableColorsCountWithoutPalette, colorer.MaxAvailableColorsCount, "Maximum available colors count without palette.");

			colorer.MaxColorsCount = 1;
			Assert.AreEqual(2, colorer.MaxColorsCount, "Minimum valid max colors count is 2.");
			colorer.MaxColorsCount = 1000;
			Assert.AreEqual(ImageColorsController.MaximumAvailableColorsCountWithoutPalette, colorer.MaxColorsCount, "Max colors count should not go above maximum value without palette.");

			var palette = new CodedPalette { Name = "Palette 1" };
			for (int i = 0; i < 10; i++)
			{
				palette.Add(new CodedColor(i));
			}
			colorer = new ImageColorsController4Test(new ImageColorsManipulator(image), new[] { palette });
			Assert.AreEqual(10, colorer.MaxAvailableColorsCount, "Maximum available colors count should equal colors count in palette.");

			colorer.MaxColorsCount = 1000;
			Assert.AreEqual(10, colorer.MaxColorsCount, "Max colors count should not go above colors count in palette.");
		}

		[Test]
		public void TestDefaultSettings()
		{
			var image = new CodedImage { Size = new Size(5, 5) };
			var colorer = new ImageColorsController4Test(new ImageColorsManipulator(image));

			var palette1 = new CodedPalette { Name = "Palette 1" };
			var palette2 = new CodedPalette { Name = "Palette 2" };
			palette2.Add(1);
			palette2.Add(2);
			palette2.Add(3);

			using (colorer.SuspendCallManipulations())
			{
				colorer.AddColorPalettes(new[] { palette1, palette2 });

				colorer.PaletteName = "Palette 2";
				colorer.MaxColorsCount = 2;
				colorer.DitherLevel = 5;
				colorer.ColorComparisonType = ImageColorsController.ColorComparisonTypes.Exact;
				colorer.EnsureBlackAndWhiteColors = true;
			}

			var newColorer = new ImageColorsController4Test(new ImageColorsManipulator(image), new[] { palette1, palette2 }, palette1); // Pass different initial palette

			// Precondition checks
			Assert.AreNotEqual("Palette 2", newColorer.PaletteName);
			Assert.AreNotSame(palette2, newColorer.SelectedPalette);
			Assert.AreNotEqual(2, newColorer.MaxColorsCount);
			Assert.AreNotEqual(5, newColorer.DitherLevel);
			Assert.AreNotEqual(ImageColorsController.ColorComparisonTypes.Exact, newColorer.ColorComparisonType);
			Assert.IsFalse(newColorer.EnsureBlackAndWhiteColors);

			colorer.SaveDefaults();

			newColorer = new ImageColorsController4Test(new ImageColorsManipulator(image), new[] { palette1, palette2 }, palette1); // Pass different initial palette

			Assert.AreEqual("Palette 2", newColorer.PaletteName);
			Assert.AreSame(palette2, newColorer.SelectedPalette);
			Assert.AreEqual(2, newColorer.MaxColorsCount);
			Assert.AreEqual(5, newColorer.DitherLevel);
			Assert.AreEqual(ImageColorsController.ColorComparisonTypes.Exact, newColorer.ColorComparisonType);
			Assert.IsTrue(newColorer.EnsureBlackAndWhiteColors);

			SAEWizardSettings.Default.Reset(); // Restore defaults
		}

		#region Test class

		class ImageColorsController4Test : ImageColorsController
		{
			public ImageColorsController4Test(ImageColorsManipulator manipulator, IEnumerable<CodedPalette> availablePalettes = null, CodedPalette selectedPalette = null) : base(manipulator, availablePalettes, selectedPalette) { }

			protected override void CallManipulationsCore()
			{
				CallManipulationsCoreFired = true;
			}

			public bool CallManipulationsCoreFired { get; set; }
		}

		#endregion
	}
}

