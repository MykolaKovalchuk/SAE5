using System;
using Ravlyk.Common;
using Ravlyk.Drawing;

namespace Ravlyk.SAE.Drawing
{
	/// <summary>
	/// Extension to <see cref="Color"/> class with additional color description properties.
	/// </summary>
	public class CodedColor : Color
	{
		#region Constructors

		/// <summary>
		/// Creates <see cref="Ravlyk.Drawing.Color"/> structure and inits its internal <see cref="Color.Argb"/> by combined in argb (Red, Green Blue) color components.
		/// </summary>
		/// <param name="argb">32-bit (Alpha, Red, Green, Blue) color value.</param>
		public CodedColor(int argb) : base(argb) { }

		/// <summary>
		/// Creates <see cref="Ravlyk.Drawing.Color"/> structure and inits its internal <see cref="Color.Argb"/> by (Red, Green, Blue) color components.
		/// </summary>
		/// <param name="r">Red color component.</param>
		/// <param name="g">Green color component.</param>
		/// <param name="b">Blue color component.</param>
		public CodedColor(byte r, byte g, byte b) : base(r, g, b) { }

		/// <summary>
		/// Creates <see cref="Ravlyk.Drawing.Color"/> structure and inits its internal <see cref="Color.Argb"/> by (Red, Green, Blue) color components.
		/// </summary>
		/// <param name="a">Alpha color component.</param>
		/// <param name="r">Red color component.</param>
		/// <param name="g">Green color component.</param>
		/// <param name="b">Blue color component.</param>
		public CodedColor(byte a, byte r, byte g, byte b) : base(a, r, g, b) { }

		/// <summary>
		/// Makes a copy of color instance of <see cref="Ravlyk.Drawing.Color"/> object.
		/// </summary>
		/// <param name="color">Source color of <see cref="Ravlyk.Drawing.Color"/> type.</param>
		public CodedColor(Color color) : base(color) { }

		/// <summary>
		/// Makes a copy of color instance of <see cref="CodedColor"/> object.
		/// </summary>
		/// <param name="color">Source color of <see cref="CodedColor"/> type.</param>
		public CodedColor(CodedColor color) : base(color)
		{
			ColorName = color.ColorName;
			ColorCode = color.ColorCode;
			SymbolChar = color.SymbolChar;
			Hidden = color.Hidden;
		}

		#endregion

		#region Color description

		/// <summary>
		/// Code/short name/number of color.
		/// </summary>
		[SortWithProperty("ColorCodeForSort")]
		public string ColorCode { get; set; }

		/// <summary>
		/// Color code adjusted for proper sorting.
		/// </summary>
		public string ColorCodeForSort
		{
			get
			{
				int number;
				if (int.TryParse(ColorCode, out number))
				{
					return ColorCode.PadLeft(8, '0');
				}
				return ColorCode;
			}
		}

		/// <summary>
		/// Name of color.
		/// </summary>
		public string ColorName { get; set; }

		/// <summary>
		/// Symbol for schematic representation of color.
		/// </summary>
		public char SymbolChar { get; set; } = ' ';

		/// <summary>
		/// Specifies if this color is hidden.
		/// </summary>
		public bool Hidden { get; internal set; }

		#endregion

		#region ICloneable

		public new CodedColor Clone()
		{
			return (CodedColor)base.Clone();
		}

		#endregion
	}
}
