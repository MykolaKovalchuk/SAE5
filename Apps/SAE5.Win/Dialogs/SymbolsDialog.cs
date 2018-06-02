using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Ravlyk.Adopted.TrueTypeSharp;
using Ravlyk.Common;
using Ravlyk.SAE.Drawing;
using Ravlyk.SAE.Drawing.Processor;

namespace Ravlyk.SAE5.WinForms.Dialogs
{
	public partial class SymbolsDialog : Form
	{
		public SymbolsDialog(TrueTypeFont symbolsFont, ICollection<char> usedSymbols)
		{
			InitializeComponent();
			SymbolsFont = symbolsFont;
			this.usedSymbols = usedSymbols;
		}

		TrueTypeFont SymbolsFont { get; }
		readonly ICollection<char> usedSymbols;

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			BeginInvoke(new MethodInvoker(Initialize));
		}

		void Initialize()
		{
			var image = new CodedImage { Size = new Size(1, 1) };
			image.CompletePalette();
			image.Palette.SymbolsFont = SymbolsFont.Name;
			var manipulator = new ImageSymbolsManipulator(image);

			symboler = new ImageSymbolsController(new ImageSymbolsManipulator(manipulator), new[] { SymbolsFont }, SymbolsFont);
			symboler.IncludeLetters = true;
			symboler.IncludeNumbers = true;
			symboler.IncludeSymbols = true;
			symboler.MaxSelectedSymbols = 1;

			symboler.AvailableSymbols.RemoveAll(pair => usedSymbols.Contains(pair.Key));
			symboler.ClearAllSelection();

			scrollControlSymbols.Controller = new VisualSymbolsController(symboler, new Size(scrollControlSymbols.Width, scrollControlSymbols.Height));
			scrollControlSymbols.Controller.VisualImageChanged += Controller_VisualImageChanged;
		}

		ImageSymbolsController symboler;

		void Controller_VisualImageChanged(object sender, EventArgs e)
		{
			buttonOk.Enabled = symboler.SelectedCount == 1;
		}

		public char SelectedSymbol => symboler.SelectedCount > 0 ? symboler.SelectedSymbols.First() : ' ';
	}
}
