using System;
using System.Collections.Generic;
using System.Linq;

using Ravlyk.Adopted.TrueTypeSharp;
using Ravlyk.Drawing.ImageProcessor.Utilities;

namespace Ravlyk.SAE.Drawing.Processor
{
	public class ImageSymbolsManipulator : ImageInplaceManipulator
	{
		public ImageSymbolsManipulator(CodedImage sourceImage) : base(sourceImage) { }
		public ImageSymbolsManipulator(ImageManipulator parentManipulator) : base(parentManipulator) { }

		public void ApplySymbols(IEnumerable<char> symbols, TrueTypeFont font)
		{
			if (font != null)
			{
				lastUsedCharacters = symbols.OrderBy(font.GetCodepintWeight).ToArray();
				lastFontName = font.Name;
			}
			else
			{
				lastUsedCharacters = symbols.ToArray();
			}

			if (IsManipulatedImageInitialized)
			{
				ManipulatedImage.Palette.SymbolsFont = lastFontName;

				var symbolsEnumerator = lastUsedCharacters.GetEnumerator();
				foreach (var color in ManipulatedImage.Palette.OrderByDarknes(true).OfType<CodedColor>().TakeWhile(color => symbolsEnumerator.MoveNext()))
				{
					color.SymbolChar = (char)symbolsEnumerator.Current;
				}
				OnImageChanged();
			}
		}

		char[] lastUsedCharacters;
		string lastFontName;

		protected override void RestoreManipulationsCore()
		{
			base.RestoreManipulationsCore();

			if (lastUsedCharacters != null)
			{
				ApplySymbols(lastUsedCharacters, null);
			}
		}
	}
}
