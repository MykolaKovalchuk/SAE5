using System;
using System.Linq;

using NUnit.Framework;

using Ravlyk.Common;
using Ravlyk.SAE.Resources;

namespace Ravlyk.SAE.Drawing.Processor.Test
{
	[TestFixture]
	public class VisualSymbolsControllerTest
	{
		[Test]
		public void TestVisualSymbolsController()
		{
			var font = SAEResources.GetAllFonts(string.Empty).First();
			var image = new CodedImage { Size = new Size(5, 5) };
			var symboler = new ImageSymbolsController(new ImageSymbolsManipulator(image), new[] { font }, font);
			using (symboler.SuspendCallManipulations())
			{
				symboler.IncludeLetters = true;
				symboler.IncludeNumbers = false;
				symboler.IncludeSymbols = false;
			}
			Assert.AreEqual(117, symboler.AvailableSymbols.Count, "Precondition");

			var visualSymbolsController = new VisualSymbolsController(symboler, new Size(100, 150));

			Assert.AreSame(symboler, visualSymbolsController.Controller);
			Assert.AreEqual(3, visualSymbolsController.ColumnsCount);
			Assert.AreEqual(39, visualSymbolsController.RowsCount);
			Assert.AreEqual(5, visualSymbolsController.VisibleRowsCount);
			Assert.IsTrue(visualSymbolsController.RequiresShift);

			Assert.AreEqual(new Size(96, 150), visualSymbolsController.VisualImage.Size);
		}

		[Test]
		public void TestHighlightedCell()
		{
			var font = SAEResources.GetAllFonts(string.Empty).First();
			var image = new CodedImage { Size = new Size(5, 5) };
			var symboler = new ImageSymbolsController(new ImageSymbolsManipulator(image), new[] { font }, font);
			using (symboler.SuspendCallManipulations())
			{
				symboler.IncludeLetters = true;
				symboler.IncludeNumbers = false;
				symboler.IncludeSymbols = false;
			}
			var visualSymbolsController = new VisualSymbolsController(symboler, new Size(100, 150));

			visualSymbolsController.OnShift(new Point(50, 20));
			Assert.AreEqual(new Point(1, 0), visualSymbolsController.HighlightedCell);

			visualSymbolsController.OnShift(new Point(50, 100));
			Assert.AreEqual(new Point(1, 3), visualSymbolsController.HighlightedCell);
		}

		[Test]
		public void TestShift()
		{
			var font = SAEResources.GetAllFonts(string.Empty).First();
			var image = new CodedImage { Size = new Size(5, 5) };
			var symboler = new ImageSymbolsController(new ImageSymbolsManipulator(image), new[] { font }, font);
			using (symboler.SuspendCallManipulations())
			{
				symboler.IncludeLetters = true;
				symboler.IncludeNumbers = false;
				symboler.IncludeSymbols = false;
			}
			var visualSymbolsController = new VisualSymbolsController(symboler, new Size(100, 150));

			Assert.AreEqual(0, visualSymbolsController.VerticalShift);

			using (visualSymbolsController.SuspendUpdateVisualImage())
			{
				visualSymbolsController.OnTouched(new Point(50, 50));

				visualSymbolsController.OnShift(new Point(50, 60));
				Assert.AreEqual(0, visualSymbolsController.VerticalShift, "Should not shift beyond top row.");

				visualSymbolsController.OnShift(new Point(50, 40));
				Assert.AreEqual(20, visualSymbolsController.VerticalShift);

				visualSymbolsController.OnShift(new Point(50, 45));
				Assert.AreEqual(15, visualSymbolsController.VerticalShift);

				visualSymbolsController.OnShift(new Point(50, -2000));
				Assert.AreEqual(1098, visualSymbolsController.VerticalShift, "Should not shift beyond bottom row.");

				visualSymbolsController.OnUntouched(new Point(0, 0));
			}
		}

		[Test]
		public void TestMouseWheel()
		{
			var font = SAEResources.GetAllFonts(string.Empty).First();
			var image = new CodedImage { Size = new Size(5, 5) };
			var symboler = new ImageSymbolsController(new ImageSymbolsManipulator(image), new[] { font }, font);
			using (symboler.SuspendCallManipulations())
			{
				symboler.IncludeLetters = true;
				symboler.IncludeNumbers = false;
				symboler.IncludeSymbols = false;
			}
			var visualSymbolsController = new VisualSymbolsController(symboler, new Size(100, 150));

			Assert.AreEqual(0, visualSymbolsController.VerticalShift);

			using (visualSymbolsController.SuspendUpdateVisualImage())
			{
				visualSymbolsController.OnMouseWheel(100);
				Assert.AreEqual(0, visualSymbolsController.VerticalShift, "Should not shift beyond top row.");

				visualSymbolsController.OnMouseWheel(-100);
				Assert.AreEqual(10, visualSymbolsController.VerticalShift);

				visualSymbolsController.OnMouseWheel(30);
				Assert.AreEqual(7, visualSymbolsController.VerticalShift);

				visualSymbolsController.OnMouseWheel(-30000);
				Assert.AreEqual(1098, visualSymbolsController.VerticalShift, "Should not shift beyond bottom row.");
			}
		}

		[Test]
		public void TestClick()
		{
			var font = SAEResources.GetAllFonts(string.Empty).First();
			var image = new CodedImage { Size = new Size(5, 5) };
			var symboler = new ImageSymbolsController(new ImageSymbolsManipulator(image), new[] { font }, font);
			using (symboler.SuspendCallManipulations())
			{
				symboler.IncludeLetters = true;
				symboler.IncludeNumbers = false;
				symboler.IncludeSymbols = false;
			}
			var visualSymbolsController = new VisualSymbolsController(symboler, new Size(100, 150));
			Assert.NotNull(visualSymbolsController.VisualImage);

			using (visualSymbolsController.SuspendUpdateVisualImage())
			{
				foreach (var c in symboler.SelectedSymbols.ToArray())
				{
					symboler.SwitchSelection(c);
				}
				Assert.AreEqual(0, symboler.SelectedCount);

				visualSymbolsController.OnTouched(new Point(50, 20)); // (1, 0) = #1 = A (#0 = ' ')
				visualSymbolsController.OnUntouched(new Point(50, 20));
				Assert.AreEqual(1, symboler.SelectedCount);
				Assert.Contains('A', symboler.SelectedSymbols.ToArray());

				visualSymbolsController.OnTouched(new Point(50, 100)); // (1, 3) = #1 = J
				visualSymbolsController.OnUntouched(new Point(50, 100));
				Assert.AreEqual(2, symboler.SelectedCount);
				Assert.Contains('A', symboler.SelectedSymbols.ToArray());
				Assert.Contains('J', symboler.SelectedSymbols.ToArray());

				visualSymbolsController.OnTouched(new Point(50, 20)); // (1, 0) = #1 = A
				visualSymbolsController.OnUntouched(new Point(50, 20));
				Assert.AreEqual(1, symboler.SelectedCount);
				Assert.Contains('J', symboler.SelectedSymbols.ToArray());
			}
		}
	}
}

