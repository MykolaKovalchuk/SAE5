using System;
using System.Collections.Generic;
using System.Linq;

using Ravlyk.Drawing;

namespace Ravlyk.SAE.Drawing
{
	public class CodedPalette : Palette, IEnumerable<CodedColor>
	{
		#region Palette description

		/// <summary>
		/// Palette name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Name of symbols font for color representation.
		/// </summary>
		public string SymbolsFont { get; set; }

		/// <summary>
		/// Specifies if it is sysetm palette, if false - it is user palette.
		/// </summary>
		public bool IsSystem { get; set; }

		/// <summary>
		/// Palette storage file name.
		/// </summary>
		public string FileName { get; set; }

		#endregion

		/// <summary>
		/// Sets new attributes of a color and fires ColorAttributesChanged event.
		/// </summary>
		/// <param name="color">Color object to change.</param>
		/// <param name="newSymbol">New color symbol.</param>
		/// <param name="newCode">New color code.</param>
		/// <param name="newName">New color name.</param>
		public void ChangeColorAttributes(CodedColor color, char newSymbol, string newCode, string newName)
		{
			char oldSymbol = color.SymbolChar;
			string oldCode = color.ColorCode;
			string oldName = color.ColorName;

			color.SymbolChar = newSymbol;
			color.ColorCode = newCode;
			color.ColorName = newName;

			OnColorAttributesChanged(color, oldSymbol, oldCode, oldName, newSymbol, newCode, newName);
		}

		/// <summary>
		/// Switches color between visible and hidden states.
		/// </summary>
		/// <param name="color">Color object to change.</param>
		public void ChangeColorVisibility(CodedColor color)
		{
			color.Hidden = !color.Hidden;
			OnColorVisibilityChanged(color);
		}

		/// <summary>
		/// Switches color between visible and hidden states.
		/// </summary>
		/// <param name="color">Color object to change.</param>
		/// <param name="visible">New visibility state.</param>
		public void ChangeColorVisibility(CodedColor color, bool visible)
		{
			color.Hidden = !visible;
			OnColorVisibilityChanged(color);
		}

		#region Overrides

		/// <summary>
		/// Overridden CodedColor indexer.
		/// </summary>
		/// <param name="colorHash">Color hash.</param>
		/// <returns>Found color casted to <see cref="CodedColor"/>.</returns>
		public new CodedColor this[int colorHash] => (CodedColor)base[colorHash];

		protected override Color NewColor(int argb)
		{
			return new CodedColor(argb);
		}

		protected override Color NewColor(Color color)
		{
			CodedColor codedColor = color as CodedColor;
			return codedColor != null ? codedColor.Clone() : new CodedColor(color);
		}

		#endregion

		#region Events

		void OnColorAttributesChanged(Color color, char oldSymbol, string oldCode, string oldName, char newSymbol, string newCode, string newName)
		{
			ColorAttributesChanged?.Invoke(this, new ColorAttributesChangedEventArgs(color, oldSymbol, oldCode, oldName, newSymbol, newCode, newName));
		}

		/// <summary>
		/// Occurs when <see cref="ChangeColorAttributes"/> method is called.
		/// </summary>
		public event EventHandler<ColorAttributesChangedEventArgs> ColorAttributesChanged;

		void OnColorVisibilityChanged(Color color)
		{
			ColorVisibilityChanged?.Invoke(this, new ColorChangedEventArgs(color));
		}

		/// <summary>
		/// Occurs when color is made visible or hidden.
		/// </summary>
		public event EventHandler<ColorChangedEventArgs> ColorVisibilityChanged;

		#region Events arguments

		/// <summary>
		/// Color attributes event arguments.
		/// </summary>
		public class ColorAttributesChangedEventArgs : ColorChangedEventArgs
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="ColorAttributesChangedEventArgs"/> class. 
			/// </summary>
			/// <param name="color">Changed color object.</param>
			/// <param name="oldSymbol">Old Color symbol.</param>
			/// <param name="oldCode">Old Color code.</param>
			/// <param name="oldName">Old Color name.</param>
			/// <param name="newSymbol">New Color symbol.</param>
			/// <param name="newCode">New Color code.</param>
			/// <param name="newName">New Color name.</param>
			public ColorAttributesChangedEventArgs(Color color, char oldSymbol, string oldCode, string oldName, char newSymbol, string newCode, string newName)
				: base(color)
			{
				OldSymbol = oldSymbol;
				OldCode = oldCode;
				OldName = oldName;
				NewSymbol = newSymbol;
				NewCode = newCode;
				NewName = newName;
			}

			/// <summary>
			/// Old Color symbol.
			/// </summary>
			public char OldSymbol { get; }

			/// <summary>
			/// Old Color code.
			/// </summary>
			public string OldCode { get; }

			/// <summary>
			/// Old Color name.
			/// </summary>
			public string OldName { get; }

			/// <summary>
			/// New Color symbol.
			/// </summary>
			public char NewSymbol { get; }

			/// <summary>
			/// New Color code.
			/// </summary>
			public string NewCode { get; }

			/// <summary>
			/// New Color name.
			/// </summary>
			public string NewName { get; }
		}

		#endregion

		#endregion

		#region ICloneable

		public new CodedPalette Clone(bool forImage = false)
		{
			return (CodedPalette)base.Clone(forImage);
		}

		protected override Palette CloneCore(bool forImage)
		{
			var palette = (CodedPalette)base.CloneCore(forImage);
			palette.ColorAttributesChanged = null;
			palette.ColorVisibilityChanged = null;

			palette.Name = Name;
			palette.SymbolsFont = SymbolsFont;

			return palette;
		}

		#endregion

		#region IEnumerable members

		public IEnumerator<CodedColor> GetEnumerator()
		{
			return GetColorsCore().Cast<CodedColor>().GetEnumerator();
		}

		#endregion
	}
}
