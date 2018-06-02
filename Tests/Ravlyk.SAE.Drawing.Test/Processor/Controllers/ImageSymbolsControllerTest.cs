using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Ravlyk.Adopted.TrueTypeSharp;
using Ravlyk.Common;
using Ravlyk.SAE.Resources;
using Ravlyk.SAE.Drawing.Properties;

namespace Ravlyk.SAE.Drawing.Processor.Test
{
	[TestFixture]
	class ImageSymbolsControllerTest
	{
		[Test]
		public void TestDoesntCallManipulationsWithoutChanges()
		{
			var image = new CodedImage { Size = new Size(5, 5) };
			var symboler = new ImageSymbolsController4Test(new ImageSymbolsManipulator(image));

			Assert.IsFalse(symboler.IncludeNumbers, "Default value");
			Assert.IsFalse(symboler.IncludeLetters, "Default value");
			Assert.IsTrue(symboler.IncludeSymbols, "Default value");
			Assert.IsTrue(symboler.CallManipulationsCoreFired, "Manipulations should have been already executed for default options.");

			symboler.CallManipulationsCoreFired = false;
			symboler.IncludeNumbers = false;
			Assert.IsFalse(symboler.CallManipulationsCoreFired, "Should not call manipulations when there are no changes.");
			symboler.IncludeNumbers = true;
			Assert.IsTrue(symboler.CallManipulationsCoreFired, "Should call manipulations for changed included symbols range.");

			symboler.CallManipulationsCoreFired = false;
			symboler.IncludeLetters = false;
			Assert.IsFalse(symboler.CallManipulationsCoreFired, "Should not call manipulations when there are no changes.");
			symboler.IncludeLetters = true;
			Assert.IsTrue(symboler.CallManipulationsCoreFired, "Should call manipulations for changed included symbols range.");

			symboler.CallManipulationsCoreFired = false;
			symboler.IncludeSymbols = true;
			Assert.IsFalse(symboler.CallManipulationsCoreFired, "Should not call manipulations when there are no changes.");
			symboler.IncludeSymbols = false;
			Assert.IsTrue(symboler.CallManipulationsCoreFired, "Should call manipulations for changed included symbols range.");
		}

		[Test]
		public void TestSymbolsFont()
		{
			var font = SAEResources.GetAllFonts().First();
			var image = new CodedImage { Size = new Size(5, 5) };
			var symboler = new ImageSymbolsController4Test(new ImageSymbolsManipulator(image), new[] { font }, font);

			Assert.AreEqual(1, symboler.AvailableFonts.Count());
			Assert.AreSame(font, symboler.SymbolsFont);
			Assert.AreEqual(font.Name, symboler.SymbolsFontName);

			symboler.CallManipulationsCoreFired = false;

			var otherFont = SAEResources.GetAllFonts().First();
			otherFont.ChangeNameForTest("Other font");
			symboler.AddSymbolsFonts(new[] { otherFont });

			Assert.AreEqual(2, symboler.AvailableFonts.Count());
			Assert.AreSame(font, symboler.SymbolsFont, "Selected font should not be changed.");
			Assert.IsFalse(symboler.CallManipulationsCoreFired, "Should not call manipulations when there are no changes.");

			symboler.SymbolsFontName = "Xyz";

			Assert.AreSame(font, symboler.SymbolsFont, "Selected font should remain if new name is incorrect.");
			Assert.AreEqual(font.Name, symboler.SymbolsFontName);
			Assert.IsFalse(symboler.CallManipulationsCoreFired, "Should not call manipulations when there are no changes.");

			symboler.SymbolsFontName = "Other font";

			Assert.AreSame(otherFont, symboler.SymbolsFont, "New font should be selected");
			Assert.AreEqual("Other font", symboler.SymbolsFontName);
			Assert.IsTrue(symboler.CallManipulationsCoreFired, "Should call manipulations when symbols font is changed.");
		}

		[Test]
		public void TestAvailableSymbols()
		{
			var font = SAEResources.GetAllFonts().First();
			var image = new CodedImage { Size = new Size(5, 5) };
			var symboler = new ImageSymbolsController4Test(new ImageSymbolsManipulator(image), new[] { font }, font);

			symboler.IncludeNumbers = false;
			symboler.IncludeLetters = false;
			symboler.IncludeSymbols = false;
			Assert.AreEqual(1, symboler.AvailableSymbols.Count);
			Assert.AreEqual(' ', symboler.AvailableSymbols[0].Key);

			symboler.IncludeNumbers = true;
			Assert.AreEqual(11, symboler.AvailableSymbols.Count, "10 digits and 1 space.");

			symboler.IncludeNumbers = false;
			symboler.IncludeLetters = true;
			Assert.AreEqual(117, symboler.AvailableSymbols.Count, "26 latin letters in both cases + 32 cyrillic letters in both cases + 1 space.");

			symboler.IncludeNumbers = true;
			Assert.AreEqual(127, symboler.AvailableSymbols.Count, "10+116+1");

			symboler.IncludeSymbols = true;
			Assert.IsTrue(symboler.AvailableSymbols.Count > 127, "10+116+1+more");
		}

		[Test]
		public void TestSelectedSymbols()
		{
			var image = new CodedImage { Size = new Size(5, 5) };
			image.CompletePalette();
			image[1, 1] = new CodedColor(100);
			image[2, 2] = new CodedColor(200);
			image.CompletePalette();
			Assert.AreEqual(3, image.Palette.Count, "Precondition");

			var font = SAEResources.GetAllFonts().First();
			var symboler = new ImageSymbolsController4Test(new ImageSymbolsManipulator(image), new[] { font }, font);

			symboler.IncludeNumbers = false;
			symboler.IncludeLetters = true;
			symboler.IncludeSymbols = false;

			Assert.AreEqual(117, symboler.AvailableSymbols.Count);
			Assert.AreEqual(3, symboler.SelectedSymbols.Count(), "Should select enough symbols for available colors.");
			Assert.AreEqual(3, symboler.SelectedCount);

			symboler.CallManipulationsCoreFired = false;
			symboler.SwitchSelection(symboler.SelectedSymbols.First());

			Assert.AreEqual(2, symboler.SelectedSymbols.Count());
			Assert.AreEqual(2, symboler.SelectedCount);
			Assert.IsTrue(symboler.CallManipulationsCoreFired, "Should call manipulations when selected symbols have changed.");

			symboler.CallManipulationsCoreFired = false;
			symboler.AddRandomSymbols();

			Assert.AreEqual(3, symboler.SelectedSymbols.Count(), "Should select enough symbols for available colors.");
			Assert.AreEqual(3, symboler.SelectedCount);
			Assert.IsTrue(symboler.CallManipulationsCoreFired, "Should call manipulations when selected symbols have changed.");

			symboler.IncludeNumbers = true;

			Assert.AreEqual(3, symboler.SelectedSymbols.Count(), "Should not reset selected symbols after adding more available symbols.");
			Assert.AreEqual(3, symboler.SelectedCount);

			symboler.SwitchSelection('0');

			Assert.AreEqual(4, symboler.SelectedSymbols.Count(), "Should select enough symbols for available colors.");
			Assert.AreEqual(4, symboler.SelectedCount);

			symboler.CallManipulationsCoreFired = false;
			symboler.AddRandomSymbols();

			Assert.AreEqual(4, symboler.SelectedSymbols.Count(), "Should not select extra symbols nor remove existing.");
			Assert.AreEqual(4, symboler.SelectedCount);
			Assert.IsFalse(symboler.CallManipulationsCoreFired, "Should not call manipulations if nothing changed.");
		}

		[Test]
		public void TestDefaultSettings()
		{
			var font1 = SAEResources.GetAllFonts().First();
			font1.ChangeNameForTest("111");
			var font2 = SAEResources.GetAllFonts().First();
			font2.ChangeNameForTest("222");

			var image = new CodedImage { Size = new Size(5, 5) };
			var symboler = new ImageSymbolsController4Test(new ImageSymbolsManipulator(image), new[] { font1, font2 }, font1);

			using (symboler.SuspendCallManipulations())
			{
				symboler.IncludeNumbers = true;
				symboler.IncludeLetters = true;
				symboler.IncludeSymbols = false;
				symboler.SymbolsFontName = "222";
			}

			var newSymboler = new ImageSymbolsController4Test(new ImageSymbolsManipulator(image), new[] { font1, font2 }, font1);

			Assert.IsFalse(newSymboler.IncludeNumbers, "Precondition check.");
			Assert.IsFalse(newSymboler.IncludeLetters, "Precondition check.");
			Assert.IsTrue(newSymboler.IncludeSymbols, "Precondition check.");
			Assert.AreEqual("111", newSymboler.SymbolsFontName, "Precondition check.");

			symboler.SaveDefaults();

			newSymboler = new ImageSymbolsController4Test(new ImageSymbolsManipulator(image), new[] { font1, font2 }, font1);

			Assert.IsTrue(newSymboler.IncludeNumbers);
			Assert.IsTrue(newSymboler.IncludeLetters);
			Assert.IsFalse(newSymboler.IncludeSymbols);
			Assert.AreEqual("222", newSymboler.SymbolsFontName);

			SAEWizardSettings.Default.Reset(); // Restore defaults
		}

		#region Test class

		class ImageSymbolsController4Test : ImageSymbolsController
		{
			public ImageSymbolsController4Test(ImageSymbolsManipulator manipulator, IEnumerable<TrueTypeFont> availableFonts = null, TrueTypeFont selectedFont = null) : base(manipulator, availableFonts, selectedFont) { }

			protected override void CallManipulationsCore()
			{
				CallManipulationsCoreFired = true;
			}

			public bool CallManipulationsCoreFired { get; set; }
		}

		#endregion
	}
}
