#region License
/* TrueTypeSharp
   Copyright (c) 2010, 2012 Illusory Studios LLC

   TrueTypeSharp is available at zer7.com. It is a C# port of Sean Barrett's
   C library stb_truetype, which was placed in the public domain and is
   available at nothings.org.

   Permission to use, copy, modify, and/or distribute this software for any
   purpose with or without fee is hereby granted, provided that the above
   copyright notice and this permission notice appear in all copies.

   THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
   WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
   MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
   ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
   WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
   ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF
   OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
*/
#endregion

/*
 * Adopted by Mykola Kovalchuk Oct 2014
 * Removed font baking as not needed.
 * Partly updated to stb_truetype v0.7.
*/

using System;
using System.Collections.Generic;
using System.IO;

namespace Ravlyk.Adopted.TrueTypeSharp
{
	public class TrueTypeFont
	{
		#region Initialization

		stb_truetype.stbtt_fontinfo _info;

		public TrueTypeFont(string name, byte[] data, int offset)
		{
			CheckFontData(data, offset);

			if (0 == stb_truetype.stbtt_InitFont(ref _info, new FakePtr<byte> { Array = data }, offset))
			{
				throw new BadImageFormatException("Couldn't load TrueType file.");
			}

			Name = name;
		}

		public TrueTypeFont(string filename) : this(Path.GetFileNameWithoutExtension(filename), File.ReadAllBytes(filename), 0) { }

		static void CheckFontData(byte[] data, int offset)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (offset < 0 || offset > data.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
		}

		#endregion

		#region Characters info

		public string Name { get; private set; }

		public uint FindGlyphIndex(char codepoint)
		{
			return stb_truetype.stbtt_FindGlyphIndex(ref _info, codepoint);
		}

		public float GetScaleForPixelHeight(float pixelHeight)
		{
			return stb_truetype.stbtt_ScaleForPixelHeight(ref _info, pixelHeight);
		}

		public IEnumerable<char> GetAvailableCodepoints()
		{
			for (var c = char.MinValue; c < char.MaxValue; c++)
			{
				var g = stb_truetype.stbtt_FindGlyphIndexOrNull(ref _info, c);
				if (g != null)
				{
					yield return c;
				}
			}
		}

		public IEnumerable<char> GetNonEmptyCodepoints()
		{
			for (var c = char.MinValue; c < char.MaxValue; c++)
			{
				var g = stb_truetype.stbtt_FindGlyphIndexOrNull(ref _info, c);
				if (g != null && !stb_truetype.stbtt_IsGlyphEmpty(ref _info, g.Value))
				{
					yield return c;
				}
			}
		}

		public int GetCodepintWeight(char codepoint)
		{
			var scale = GetScaleForPixelHeight(32);
			int width, height, xOffset, yOffset;
			var bitmap = GetCodepointBitmap(codepoint, scale, scale, out width, out height, out xOffset, out yOffset);

			int weight = 0;
			for (int i = 0; i < bitmap.Length; i++) // Use loop for performance
			{
				weight += bitmap[i];
			}
			return weight;
		}

		#endregion

		#region Character bitmap

		public byte[] GetGlyphBitmap(uint glyphIndex, float xScale, float yScale, out int width, out int height, out int xOffset, out int yOffset, float xShift = 0f, float yShift = 0f)
		{
			var data = stb_truetype.stbtt_GetGlyphBitmapSubpixel(ref _info, xScale, yScale, xShift, yShift, glyphIndex, out width, out height, out xOffset, out yOffset);
			if (data.IsNull)
			{
				width = 0;
				height = 0;
				xOffset = 0;
				yOffset = 0;
				return data.GetData(0);
			}
			return data.GetData(width * height);
		}

		public byte[] GetCodepointBitmap(char codepoint, float xScale, float yScale, out int width, out int height, out int xOffset, out int yOffset, float xShift = 0f, float yShift = 0f)
		{
			return GetGlyphBitmap(FindGlyphIndex(codepoint), xScale, yScale, out width, out height, out xOffset, out yOffset, xShift, yShift);
		}

		public void GetGlyphBitmapBox(uint glyphIndex, float xScale, float yScale, out int x0, out int y0, out int x1, out int y1, float xShift = 0f, float yShift = 0f)
		{
			stb_truetype.stbtt_GetGlyphBitmapBoxSubpixel(ref _info, glyphIndex, xScale, yScale, xShift, yShift, out x0, out y0, out x1, out y1);
		}

		public void GetCodepointBitmapBox(char codepoint, float xScale, float yScale, out int x0, out int y0, out int x1, out int y1, float xShift = 0f, float yShift = 0f)
		{
			GetGlyphBitmapBox(FindGlyphIndex(codepoint), xScale, yScale, out x0, out y0, out x1, out y1, xShift, yShift);
		}

		public void GetGlyphMeasures(uint glyphIndex, float xScale, float yScale, out int width, out int height, out int xOffset, out int yOffset, float xShift = 0f, float yShift = 0f)
		{
			var data = stb_truetype.stbtt_GetGlyphBitmapSubpixel(ref _info, xScale, yScale, xShift, yShift, glyphIndex, out width, out height, out xOffset, out yOffset);
			if (data.IsNull)
			{
				width = 0;
				height = 0;
				xOffset = 0;
				yOffset = 0;
			}
		}

		public void GetCodePointMeasures(char codepoint, float xScale, float yScale, out int width, out int height, out int xOffset, out int yOffset, float xShift = 0f, float yShift = 0f)
		{
			GetGlyphMeasures(FindGlyphIndex(codepoint), xScale, yScale, out width, out height, out xOffset, out yOffset, xShift, yShift);
		}

		#endregion

		#region Test stuff
		#if DEBUG

		public void ChangeNameForTest(string newName)
		{
			Name = newName;
		}

		#endif
		#endregion
	}
}
