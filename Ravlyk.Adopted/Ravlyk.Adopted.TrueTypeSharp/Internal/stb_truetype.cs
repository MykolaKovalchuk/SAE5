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

// ----------------------------------------------------------------------------
// stb_truetype, ported to C# by James Bellinger             Aug 2010, May 2012
// This file kept rather similar to the original, to make merging fixes easier.
// To avoid exposing the greatness of FakePtr<T> and the ugliness of my port,
// the public API is separate.
// ----------------------------------------------------------------------------

/*
 * Adopted by Mykola Kovalchuk Oct 2014
 * Removed font baking as not needed.
 * Partly updated to stb_truetype v0.7.
*/

#pragma warning disable 0219, 0660, 0661

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Ravlyk.Adopted.TrueTypeSharp
{
	static class stb_truetype
	{
		// stb_truetype.h - v0.7 - public domain
		// authored from 2009-2013 by Sean Barrett / RAD Game Tools
		//
		//   This library processes TrueType files:
		//        parse files
		//        extract glyph metrics
		//        extract glyph shapes
		//        render glyphs to one-channel bitmaps with antialiasing (box filter)
		//
		//   Todo:
		//        non-MS cmaps
		//        crashproof on bad data
		//        hinting? (no longer patented)
		//        cleartype-style AA?
		//        optimize: use simple memory allocator for intermediates
		//        optimize: build edge-list directly from curves
		//        optimize: rasterize directly from curves?
		//
		// ADDITIONAL CONTRIBUTORS
		//
		//   Mikko Mononen: compound shape support, more cmap formats
		//   Tor Andersson: kerning, subpixel rendering
		//
		//   Bug/warning reports:
		//       "Zer" on mollyrocket (with fix)
		//       Cass Everitt
		//       stoiko (Haemimont Games)
		//       Brian Hook 
		//       Walter van Niftrik
		//
		// VERSION HISTORY
		//
		//   0.7  (2013-09-25) bugfix: subpixel glyph bug fixed in 0.5 had come back
		//   0.6c (2012-07-24) improve documentation
		//   0.6b (2012-07-20) fix a few more warnings
		//   0.6  (2012-07-17) fix warnings; added stbtt_ScaleForMappingEmToPixels,
		//                        stbtt_GetFontBoundingBox, stbtt_IsGlyphEmpty
		//   0.5  (2011-12-09) bugfixes:
		//                        subpixel glyph renderer computed wrong bounding box
		//                        first vertex of shape can be off-curve (FreeSans)
		//   0.4b (2011-12-03) fixed an error in the font baking example
		//   0.4  (2011-12-01) kerning, subpixel rendering (tor)
		//                    bugfixes for:
		//                        codepoint-to-glyph conversion using table fmt=12
		//                        codepoint-to-glyph conversion using table fmt=4
		//                        stbtt_GetBakedQuad with non-square texture (Zer)
		//                    updated Hello World! sample to use kerning and subpixel
		//                    fixed some warnings
		//   0.3  (2009-06-24) cmap fmt=12, compound shapes (MM)
		//                    userdata, malloc-from-userdata, non-zero fill (STB)
		//   0.2  (2009-03-11) Fix unsigned/signed char warnings
		//   0.1  (2009-03-09) First public release
		//
		// LICENSE
		//
		//   This software is in the public domain. Where that dedication is not
		//   recognized, you are granted a perpetual, irrevokable license to copy
		//   and modify this file as you see fit.
		//
		// USAGE
		//
		//   Include this file in whatever places neeed to refer to it. In ONE C/C++
		//   file, write:
		//      #define STB_TRUETYPE_IMPLEMENTATION
		//   before the #include of this file. This expands out the actual
		//   implementation into that C/C++ file.
		//
		//   Simple 3D API (don't ship this, but it's fine for tools and quick start,
		//                  and you can cut and paste from it to move to more advanced)
		//           stbtt_BakeFontBitmap()               -- bake a font to a bitmap for use as texture
		//           stbtt_GetBakedQuad()                 -- compute quad to draw for a given char
		//
		//   "Load" a font file from a memory buffer (you have to keep the buffer loaded)
		//           stbtt_InitFont()
		//           stbtt_GetFontOffsetForIndex()        -- use for TTC font collections
		//
		//   Render a unicode codepoint to a bitmap
		//           stbtt_GetCodepointBitmap()           -- allocates and returns a bitmap
		//           stbtt_MakeCodepointBitmap()          -- renders into bitmap you provide
		//           stbtt_GetCodepointBitmapBox()        -- how big the bitmap must be
		//
		//   Character advance/positioning
		//           stbtt_GetCodepointHMetrics()
		//           stbtt_GetFontVMetrics()
		//           stbtt_GetCodepointKernAdvance()
		//
		// ADDITIONAL DOCUMENTATION
		//
		//   Immediately after this block comment are a series of sample programs.
		//
		//   After the sample programs is the "header file" section. This section
		//   includes documentation for each API function.
		//
		//   Some important concepts to understand to use this library:
		//
		//      Codepoint
		//         Characters are defined by unicode codepoints, e.g. 65 is
		//         uppercase A, 231 is lowercase c with a cedilla, 0x7e30 is
		//         the hiragana for "ma".
		//
		//      Glyph
		//         A visual character shape (every codepoint is rendered as
		//         some glyph)
		//
		//      Glyph index
		//         A font-specific integer ID representing a glyph
		//
		//      Baseline
		//         Glyph shapes are defined relative to a baseline, which is the
		//         bottom of uppercase characters. Characters extend both above
		//         and below the baseline.
		//
		//      Current Point
		//         As you draw text to the screen, you keep track of a "current point"
		//         which is the origin of each character. The current point's vertical
		//         position is the baseline. Even "baked fonts" use this model.
		//
		//      Vertical Font Metrics
		//         The vertical qualities of the font, used to vertically position
		//         and space the characters. See docs for stbtt_GetFontVMetrics.
		//
		//      Font Size in Pixels or Points
		//         The preferred interface for specifying font sizes in stb_truetype
		//         is to specify how tall the font's vertical extent should be in pixels.
		//         If that sounds good enough, skip the next paragraph.
		//
		//         Most font APIs instead use "points", which are a common typographic
		//         measurement for describing font size, defined as 72 points per inch.
		//         stb_truetype provides a point API for compatibility. However, true
		//         "per inch" conventions don't make much sense on computer displays
		//         since they different monitors have different number of pixels per
		//         inch. For example, Windows traditionally uses a convention that
		//         there are 96 pixels per inch, thus making 'inch' measurements have
		//         nothing to do with inches, and thus effectively defining a point to
		//         be 1.333 pixels. Additionally, the TrueType font data provides
		//         an explicit scale factor to scale a given font's glyphs to points,
		//         but the author has observed that this scale factor is often wrong
		//         for non-commercial fonts, thus making fonts scaled in points
		//         according to the TrueType spec incoherently sized in practice.
		//
		// ADVANCED USAGE
		//
		//   Quality:
		//
		//    - Use the functions with Subpixel at the end to allow your characters
		//      to have subpixel positioning. Since the font is anti-aliased, not
		//      hinted, this is very import for quality. (This is not possible with
		//      baked fonts.)
		//
		//    - Kerning is now supported, and if you're supporting subpixel rendering
		//      then kerning is worth using to give your text a polished look.
		//
		//   Performance:
		//
		//    - Convert Unicode codepoints to glyph indexes and operate on the glyphs;
		//      if you don't do this, stb_truetype is forced to do the conversion on
		//      every call.
		//
		//    - There are a lot of memory allocations. We should modify it to take
		//      a temp buffer and allocate from the temp buffer (without freeing),
		//      should help performance a lot.
		//
		// NOTES
		//
		//   The system uses the raw data found in the .ttf file without changing it
		//   and without building auxiliary data structures. This is a bit inefficient
		//   on little-endian systems (the data is big-endian), but assuming you're
		//   caching the bitmaps or glyph shapes this shouldn't be a big deal.
		//
		//   It appears to be very hard to programmatically determine what font a
		//   given file is in a general way. I provide an API for this, but I don't
		//   recommend it.
		//
		//
		// SOURCE STATISTICS (based on v0.6c, 2050 LOC)
		//
		//   Documentation & header file        520 LOC  \___ 660 LOC documentation
		//   Sample code                        140 LOC  /
		//   Truetype parsing                   620 LOC  ---- 620 LOC TrueType
		//   Software rasterization             240 LOC  \
		//   Curve tesselation                  120 LOC   \__ 550 LOC Bitmap creation
		//   Bitmap management                  100 LOC   /
		//   Baked bitmap interface              70 LOC  /
		//   Font name matching & access        150 LOC  ---- 150 
		//   C runtime library abstraction       60 LOC  ----  60
		//
		//
		// ///////////////////////////////////////////////////////////////////////////

		static int STBTT_ifloor(float x)
		{
			return (int)Math.Floor(x);
		}

		static int STBTT_iceil(float x)
		{
			return (int)Math.Ceiling(x);
		}

		class DelegateComparer<T> : IComparer<T>
		{
			Comparison<T> _comparison;

			public DelegateComparer(Comparison<T> comparison)
			{
				_comparison = comparison;
			}

			public int Compare(T item1, T item2)
			{
				return _comparison(item1, item2);
			}
		}

		static void STBTT_sort<T>(ref FakePtr<T> p, int n, Comparison<T> comparer)
		{
			Array.Sort<T>(p.Array, p.Offset, n, new DelegateComparer<T>(comparer));
		}

		static void STBTT_memcpy<T>(FakePtr<T> target, FakePtr<T> source, int count)
		{
			for (int i = 0; i < count; i ++)
			{
				target[i] = source[i];
			}
		}

		static void STBTT_memset<T>(FakePtr<T> target, int count)
		{
			for (int i = 0; i < count; i ++)
			{
				target[i] = default(T);
			}
		}

		static T STBTT_malloc<T>() where T : class, new()
		{
			try
			{
				return new T();
			}
			catch (OutOfMemoryException)
			{
				return null;
			}
		}

		static FakePtr<T> STBTT_malloc<T>(int count)
		{
			try
			{
				return new FakePtr<T>() { Array = new T[count] };
			}
			catch (OutOfMemoryException)
			{
				return new FakePtr<T>();
			}
		}

		static void STBTT_free<T>(FakePtr<T> p) { } // no-op

		static void STBTT_assert(bool condition)
		{
			Debug.Assert(condition);
		}

		static void STBTT_assert(int condition)
		{
			STBTT_assert(condition != 0);
		}

		public struct stbtt_fontinfo
		{
			public FakePtr<byte> data; // pointer to .ttf file
			public int fontstart; // offset of start of font

			public uint numGlyphs; // number of glyphs, needed for range checking

			public uint loca, head, glyf, hhea, hmtx, kern; // table locations as offset from start of .ttf
			public uint index_map; // a cmap mapping for our chosen character encoding
			public int indexToLocFormat; // format needed to map from glyph index to glyph
		}

		// @TODO: don't expose this structure
		internal struct stbtt__bitmap
		{
			public int w, h, stride;
			public FakePtr<byte> pixels;
		}

		enum STBTT_MACSTYLE
		{
			STBTT_MACSTYLE_DONTCARE = 0,
			STBTT_MACSTYLE_BOLD = 1,
			STBTT_MACSTYLE_ITALIC = 2,
			STBTT_MACSTYLE_UNDERSCORE = 4,
			STBTT_MACSTYLE_NONE = 8
		}; // <= not same as 0, this makes us check the bitfield is 0

		enum STBTT_PLATFORM_ID
		{
			// platformID
			STBTT_PLATFORM_ID_UNICODE = 0,
			STBTT_PLATFORM_ID_MAC = 1,
			STBTT_PLATFORM_ID_ISO = 2,
			STBTT_PLATFORM_ID_MICROSOFT = 3
		}

		enum STBTT_UNICODE_EID
		{
			// encodingID for STBTT_PLATFORM_ID_UNICODE
			STBTT_UNICODE_EID_UNICODE_1_0 = 0,
			STBTT_UNICODE_EID_UNICODE_1_1 = 1,
			STBTT_UNICODE_EID_ISO_10646 = 2,
			STBTT_UNICODE_EID_UNICODE_2_0_BMP = 3,
			STBTT_UNICODE_EID_UNICODE_2_0_FULL = 4
		}

		enum STBTT_MS_EID
		{
			// encodingID for STBTT_PLATFORM_ID_MICROSOFT
			STBTT_MS_EID_SYMBOL = 0,
			STBTT_MS_EID_UNICODE_BMP = 1,
			STBTT_MS_EID_SHIFTJIS = 2,
			STBTT_MS_EID_UNICODE_FULL = 10
		}

		enum STBTT_MAC_EID
		{
			// encodingID for STBTT_PLATFORM_ID_MAC; same as Script Manager codes
			STBTT_MAC_EID_ROMAN = 0,
			STBTT_MAC_EID_ARABIC = 4,
			STBTT_MAC_EID_JAPANESE = 1,
			STBTT_MAC_EID_HEBREW = 5,
			STBTT_MAC_EID_CHINESE_TRAD = 2,
			STBTT_MAC_EID_GREEK = 6,
			STBTT_MAC_EID_KOREAN = 3,
			STBTT_MAC_EID_RUSSIAN = 7
		}

		enum STBTT_MS_LANG
		{
			// languageID for STBTT_PLATFORM_ID_MICROSOFT; same as LCID...
			// problematic because there are e.g. 16 english LCIDs and 16 arabic LCIDs
			STBTT_MS_LANG_ENGLISH = 0x0409,
			STBTT_MS_LANG_ITALIAN = 0x0410,
			STBTT_MS_LANG_CHINESE = 0x0804,
			STBTT_MS_LANG_JAPANESE = 0x0411,
			STBTT_MS_LANG_DUTCH = 0x0413,
			STBTT_MS_LANG_KOREAN = 0x0412,
			STBTT_MS_LANG_FRENCH = 0x040c,
			STBTT_MS_LANG_RUSSIAN = 0x0419,
			STBTT_MS_LANG_GERMAN = 0x0407,
			STBTT_MS_LANG_SPANISH = 0x0409,
			STBTT_MS_LANG_HEBREW = 0x040d,
			STBTT_MS_LANG_SWEDISH = 0x041D
		}

		enum STBTT_MAC_LANG
		{
			// languageID for STBTT_PLATFORM_ID_MAC
			STBTT_MAC_LANG_ENGLISH = 0,
			STBTT_MAC_LANG_JAPANESE = 11,
			STBTT_MAC_LANG_ARABIC = 12,
			STBTT_MAC_LANG_KOREAN = 23,
			STBTT_MAC_LANG_DUTCH = 4,
			STBTT_MAC_LANG_RUSSIAN = 32,
			STBTT_MAC_LANG_FRENCH = 1,
			STBTT_MAC_LANG_SPANISH = 6,
			STBTT_MAC_LANG_GERMAN = 2,
			STBTT_MAC_LANG_SWEDISH = 5,
			STBTT_MAC_LANG_HEBREW = 10,
			STBTT_MAC_LANG_CHINESE_SIMPLIFIED = 33,
			STBTT_MAC_LANG_ITALIAN = 3,
			STBTT_MAC_LANG_CHINESE_TRAD = 19
		}

		static byte ttBYTE(FakePtr<byte> p)
		{
			return p.Value;
		}

		static sbyte ttCHAR(FakePtr<byte> p)
		{
			return (sbyte)p.Value;
		}

		static int ttFixed(FakePtr<byte> p)
		{
			return ttLONG(p);
		}

		static ushort ttUSHORT(FakePtr<byte> p)
		{
			return (ushort)(p[0] * 256 + p[1]);
		}

		static short ttSHORT(FakePtr<byte> p)
		{
			return (short)(p[0] * 256 + p[1]);
		}

		static uint ttULONG(FakePtr<byte> p)
		{
			return ((uint)p[0] << 24) + ((uint)p[1] << 16) + ((uint)p[2] << 8) + p[3];
		}

		static int ttLONG(FakePtr<byte> p)
		{
			return (p[0] << 24) + (p[1] << 16) + (p[2] << 8) + p[3];
		}

		static bool stbtt_tag4(FakePtr<byte> p, byte c0, byte c1, byte c2, byte c3)
		{
			return ((p)[0] == (c0) && (p)[1] == (c1) && (p)[2] == (c2) && (p)[3] == (c3));
		}

		static bool stbtt_tag(FakePtr<byte> p, string str)
		{
			return stbtt_tag4(p,
				(byte)(str.Length >= 1 ? str[0] : 0),
				(byte)(str.Length >= 2 ? str[1] : 0),
				(byte)(str.Length >= 3 ? str[2] : 0),
				(byte)(str.Length >= 4 ? str[3] : 0));
		}

		static int stbtt__isfont(FakePtr<byte> font)
		{
			// check the version number
			if (stbtt_tag4(font, (byte)'1', 0, 0, 0)) return 1; // TrueType 1
			if (stbtt_tag(font, "typ1")) return 1; // TrueType with type 1 font -- we don't support this!
			if (stbtt_tag(font, "OTTO")) return 1; // OpenType with CFF
			if (stbtt_tag4(font, 0, 1, 0, 0)) return 1; // OpenType 1.0
			return 0;
		}

		// @OPTIMIZE: binary search
		static uint stbtt__find_table(FakePtr<byte> data, int fontstart, string tag)
		{
			int num_tables = ttUSHORT(data + fontstart + 4);
			int tabledir = fontstart + 12;
			for (int i = 0; i < num_tables; ++i)
			{
				int loc = tabledir + 16 * i;
				if (stbtt_tag(data + loc + 0, tag))
					return ttULONG(data + loc + 8);
			}
			return 0;
		}

		public static int stbtt_GetFontOffsetForIndex(FakePtr<byte> font_collection, int index)
		{
			// if it's just a font, there's only one valid index
			if (stbtt__isfont(font_collection) != 0)
				return index == 0 ? 0 : -1;

			// check if it's a TTC
			if (stbtt_tag(font_collection, "ttcf"))
			{
				// version 1?
				if (ttULONG(font_collection + 4) == 0x00010000 || ttULONG(font_collection + 4) == 0x00020000)
				{
					int n = ttLONG(font_collection + 8);
					if (index >= n)
						return -1;
					return (int)ttULONG(font_collection + 12 + index * 14);
				}
			}
			return -1;
		}

		public static int stbtt_InitFont(ref stbtt_fontinfo info, FakePtr<byte> data2, int fontstart)
		{
			FakePtr<byte> data = (FakePtr<byte>)data2;
			uint cmap, t;
			uint i, numTables;

			info.data = data;
			info.fontstart = fontstart;

			cmap = stbtt__find_table(data, fontstart, "cmap"); // required
			info.loca = stbtt__find_table(data, fontstart, "loca"); // required
			info.head = stbtt__find_table(data, fontstart, "head"); // required
			info.glyf = stbtt__find_table(data, fontstart, "glyf"); // required
			info.hhea = stbtt__find_table(data, fontstart, "hhea"); // required
			info.hmtx = stbtt__find_table(data, fontstart, "hmtx"); // required
			info.kern = stbtt__find_table(data, fontstart, "kern"); // not required
			if (cmap == 0 || info.loca == 0 || info.head == 0 || info.glyf == 0 || info.hhea == 0 || info.hmtx == 0)
				return 0;

			t = stbtt__find_table(data, fontstart, "maxp");
			if (t != 0)
				info.numGlyphs = ttUSHORT(data + t + 4);
			else
				info.numGlyphs = 0xffff;

			// find a cmap encoding table we understand *now* to avoid searching
			// later. (todo: could make this installable)
			// the same regardless of glyph.
			numTables = ttUSHORT(data + cmap + 2);
			info.index_map = 0;
			for (i = 0; i < numTables; ++i)
			{
				uint encoding_record = cmap + 4 + 8 * i;
				// find an encoding we understand:
				switch ((STBTT_PLATFORM_ID)ttUSHORT(data + encoding_record))
				{
					case STBTT_PLATFORM_ID.STBTT_PLATFORM_ID_MICROSOFT:
						switch ((STBTT_MS_EID)ttUSHORT(data + encoding_record + 2))
						{
							case STBTT_MS_EID.STBTT_MS_EID_UNICODE_BMP:
							case STBTT_MS_EID.STBTT_MS_EID_UNICODE_FULL:
								// MS/Unicode
								info.index_map = cmap + ttULONG(data + encoding_record + 4);
								break;
						}
						break;
				}
			}
			if (info.index_map == 0)
				return 0;

			info.indexToLocFormat = ttUSHORT(data + info.head + 50);
			return 1;
		}

		public static uint stbtt_FindGlyphIndex(ref stbtt_fontinfo info, uint unicode_codepoint)
		{
			return stbtt_FindGlyphIndexOrNull(ref info, unicode_codepoint) ?? 0;
		}

		public static uint? stbtt_FindGlyphIndexOrNull(ref stbtt_fontinfo info, uint unicode_codepoint)
		{
			FakePtr<byte> data = info.data;
			uint index_map = info.index_map;

			ushort format = ttUSHORT(data + index_map + 0);
			if (format == 0)
			{
				// apple byte encoding
				int bytes = ttUSHORT(data + index_map + 2);
				if (unicode_codepoint < bytes - 6)
					return ttBYTE(data + index_map + 6 + unicode_codepoint);
				return null;
			}
			else if (format == 6)
			{
				uint first = ttUSHORT(data + index_map + 6);
				uint count = ttUSHORT(data + index_map + 8);
				if ((uint)unicode_codepoint >= first && (uint)unicode_codepoint < first + count)
					return ttUSHORT(data + index_map + 10 + ((uint)unicode_codepoint - first) * 2);
				return null;
			}
			else if (format == 2)
			{
				STBTT_assert(0); // @TODO: high-byte mapping for japanese/chinese/korean
				return null;
			}
			else if (format == 4)
			{
				// standard mapping for windows fonts: binary search collection of ranges
				ushort segcount = (ushort)(ttUSHORT(data + index_map + 6) >> 1);
				ushort searchRange = (ushort)(ttUSHORT(data + index_map + 8) >> 1);
				ushort entrySelector = ttUSHORT(data + index_map + 10);
				ushort rangeShift = (ushort)(ttUSHORT(data + index_map + 12) >> 1);
				ushort item, offset, start, end;

				// do a binary search of the segments
				uint endCount = index_map + 14;
				uint search = endCount;

				if (unicode_codepoint > 0xffff)
					return null;

				// they lie from endCount .. endCount + segCount
				// but searchRange is the nearest power of two, so...
				if (unicode_codepoint >= ttUSHORT(data + search + rangeShift * 2))
					search += (uint)rangeShift * 2;

				// now decrement to bias correctly to find smallest
				search -= 2;
				while (entrySelector != 0)
				{
					ushort start_, end_;
					searchRange >>= 1;
					start_ = ttUSHORT(data + search + 2 + segcount * 2 + 2);
					end_ = ttUSHORT(data + search + 2);
					start_ = ttUSHORT(data + search + searchRange * 2 + segcount * 2 + 2);
					end_ = ttUSHORT(data + search + searchRange * 2);
					if (unicode_codepoint > end_)
						search += (uint)searchRange * 2;
					--entrySelector;
				}
				search += 2;

				item = (ushort)((search - endCount) >> 1);

				STBTT_assert(unicode_codepoint <= ttUSHORT(data + endCount + 2 * item));
				start = ttUSHORT(data + index_map + 14 + segcount * 2 + 2 + 2 * item);
				end = ttUSHORT(data + index_map + 14 + 2 + 2 * item);
				if (unicode_codepoint < start)
					return null;

				offset = ttUSHORT(data + index_map + 14 + segcount * 6 + 2 + 2 * item);
				if (offset == 0)
					return (ushort)(unicode_codepoint + ttSHORT(data + index_map + 14 + segcount * 4 + 2 + 2 * item));

				return ttUSHORT(data + offset + (unicode_codepoint - start) * 2 + index_map + 14 + segcount * 6 + 2 + 2 * item);
			}
			else if (format == 12 || format == 13)
			{
				uint ngroups = ttULONG(data + index_map + 12);
				int low, high;
				low = 0;
				high = (int)ngroups;
				// Binary search the right group.
				while (low < high)
				{
					int mid = low + ((high - low) >> 1); // rounds down, so low <= mid < high
					uint start_char = ttULONG(data + index_map + 16 + mid * 12);
					uint end_char = ttULONG(data + index_map + 16 + mid * 12 + 4);
					if (unicode_codepoint < start_char)
						high = mid;
					else if (unicode_codepoint > end_char)
						low = mid + 1;
					else
					{
						uint start_glyph = ttULONG(data + index_map + 16 + mid * 12 + 8);
						if (format == 12)
							return start_glyph + unicode_codepoint - start_char;
						else // format == 13
							return start_glyph;
					}
				}
				return null; // not found
			}
			// @TODO
			STBTT_assert(0);
			return null;
		}

		static void stbtt__setvertex(FakePtr<GlyphVertex> v, GlyphVertexType type, int x, int y, int cx, int cy)
		{
			var _ = v.Value;
			_.Type = type;
			_.X = (short)x;
			_.Y = (short)y;
			_.CX = (short)cx;
			_.CY = (short)cy;
			v.Value = _;
		}

		static int stbtt__GetGlyfOffset(ref stbtt_fontinfo info, uint glyph_index)
		{
			int g1, g2;

			if (glyph_index >= info.numGlyphs) return -1; // glyph index out of range
			if (info.indexToLocFormat >= 2) return -1; // unknown index.glyph map format

			if (info.indexToLocFormat == 0)
			{
				g1 = (int)(info.glyf + ttUSHORT(info.data + info.loca + glyph_index * 2) * 2);
				g2 = (int)(info.glyf + ttUSHORT(info.data + info.loca + glyph_index * 2 + 2) * 2);
			}
			else
			{
				g1 = (int)(info.glyf + ttULONG(info.data + info.loca + glyph_index * 4));
				g2 = (int)(info.glyf + ttULONG(info.data + info.loca + glyph_index * 4 + 4));
			}

			return g1 == g2 ? -1 : g1; // if length is 0, return -1
		}

		public static int stbtt_GetGlyphBox(ref stbtt_fontinfo info, uint glyph_index, out int x0, out int y0, out int x1, out int y1)
		{
			int g = stbtt__GetGlyfOffset(ref info, glyph_index);
			if (g < 0)
			{
				x0 = y0 = x1 = y1 = 0;
				return 0;
			}

			x0 = ttSHORT(info.data + g + 2);
			y0 = ttSHORT(info.data + g + 4);
			x1 = ttSHORT(info.data + g + 6);
			y1 = ttSHORT(info.data + g + 8);
			return 1;
		}

		public static int stbtt_GetCodepointBox(ref stbtt_fontinfo info, uint codepoint, out int x0, out int y0, out int x1, out int y1)
		{
			return stbtt_GetGlyphBox(ref info, stbtt_FindGlyphIndex(ref info, codepoint), out x0, out y0, out x1, out y1);
		}

		public static bool stbtt_IsGlyphEmpty(ref stbtt_fontinfo info, uint glyph_index)
		{
			short numberOfContours;
			int g = stbtt__GetGlyfOffset(ref info, glyph_index);
			if (g < 0) return true;
			numberOfContours = ttSHORT(info.data + g);
			return numberOfContours == 0;
		}

		static int stbtt__close_shape(FakePtr<GlyphVertex> vertices, int num_vertices, int was_off, int start_off, int sx, int sy, int scx, int scy, int cx, int cy)
		{
			if (start_off != 0)
			{
				if (was_off != 0)
					stbtt__setvertex(vertices + num_vertices++, GlyphVertexType.Curve, (cx + scx) >> 1, (cy + scy) >> 1, cx, cy);
				stbtt__setvertex(vertices + num_vertices++, GlyphVertexType.Curve, sx, sy, scx, scy);
			}
			else
			{
				if (was_off != 0)
					stbtt__setvertex(vertices + num_vertices++, GlyphVertexType.Curve, sx, sy, cx, cy);
				else
					stbtt__setvertex(vertices + num_vertices++, GlyphVertexType.Move, sx, sy, 0, 0);
			}
			return num_vertices;
		}

		public static int stbtt_GetGlyphShape(ref stbtt_fontinfo info, uint glyph_index, out FakePtr<GlyphVertex> pvertices)
		{
			short numberOfContours;
			FakePtr<byte> endPtsOfContours;
			FakePtr<byte> data = info.data;
			FakePtr<GlyphVertex> vertices = new FakePtr<GlyphVertex>();
			int num_vertices = 0;
			int g = stbtt__GetGlyfOffset(ref info, glyph_index);

			pvertices = new FakePtr<GlyphVertex>();
			if (g < 0) return 0;

			numberOfContours = ttSHORT(data + g);

			if (numberOfContours > 0)
			{
				byte flags = 0, flagcount;
				int ins, i, j = 0, m, n, next_move, was_off = 0, off, start_off = 0;
				int x, y, cx, cy, sx, sy, scx = 0, scy = 0;
				FakePtr<byte> points;
				endPtsOfContours = (data + g + 10);
				ins = ttUSHORT(data + g + 10 + numberOfContours * 2);
				points = data + g + 10 + numberOfContours * 2 + 2 + ins;

				n = 1 + ttUSHORT(endPtsOfContours + numberOfContours * 2 - 2);

				m = n + 2 * numberOfContours; // a loose bound on how many vertices we might need
				vertices = STBTT_malloc<GlyphVertex>(m);
				if (vertices.IsNull) return 0;

				next_move = 0;
				flagcount = 0;

				// in first pass, we load uninterpreted data into the allocated array
				// above, shifted to the end of the array so we won't overwrite it when
				// we create our final data starting from the front

				off = m - n; // starting offset for uninterpreted data, regardless of how m ends up being calculated

				// first load flags

				for (i = 0; i < n; ++i)
				{
					if (flagcount == 0)
					{
						flags = (points++).Value;
						if ((flags & 8) != 0)
							flagcount = (points++).Value;
					}
					else
						--flagcount;

					var _ = vertices[off + i];
					_.Type = (GlyphVertexType)flags;
					vertices[off + i] = _;
				}

				// now load x coordinates
				x = 0;
				for (i = 0; i < n; ++i)
				{
					flags = (byte)vertices[off + i].Type;
					if ((flags & 2) != 0)
					{
						short dx = (points++).Value;
						x += (short)((flags & 16) != 0 ? dx : -dx); // ???
					}
					else
					{
						if ((flags & 16) == 0)
						{
							x += (short)(points[0] * 256 + points[1]);
							points += 2;
						}
					}
					var _ = vertices[off + i];
					_.X = (short)x;
					vertices[off + i] = _;
				}

				// now load y coordinates
				y = 0;
				for (i = 0; i < n; ++i)
				{
					flags = (byte)vertices[off + i].Type;
					if ((flags & 4) != 0)
					{
						short dy = (points++).Value;
						y += (short)((flags & 32) != 0 ? dy : -dy); // ???
					}
					else
					{
						if ((flags & 32) == 0)
						{
							y += (short)(points[0] * 256 + points[1]);
							points += 2;
						}
					}
					var _ = vertices[off + i];
					_.Y = (short)y;
					vertices[off + i] = _;
				}

				// now convert them to our format
				num_vertices = 0;
				sx = sy = cx = cy = scx = scy = 0;
				for (i = 0; i < n; ++i)
				{
					flags = (byte)vertices[off + i].Type;
					x = (short)vertices[off + i].X;
					y = (short)vertices[off + i].Y;
					if (next_move == i)
					{
						if (i != 0)
							num_vertices = stbtt__close_shape(vertices, num_vertices, was_off, start_off, sx, sy, scx, scy, cx, cy);

						// now start the new one               
						start_off = 0 == (flags & 1) ? 1 : 0;
						if (start_off != 0)
						{
							// if we start off with an off-curve point, then when we need to find a point on the curve
							// where we can start, and we need to save some state for when we wraparound.
							scx = x;
							scy = y;
							if (0 == ((int)vertices[off + i + 1].Type & 1))
							{
								// next point is also a curve point, so interpolate an on-point curve
								sx = (x + (int)vertices[off + i + 1].X) >> 1;
								sy = (y + (int)vertices[off + i + 1].Y) >> 1;
							}
							else
							{
								// otherwise just use the next point as our start point
								sx = (int)vertices[off + i + 1].X;
								sy = (int)vertices[off + i + 1].Y;
								++i; // we're using point i+1 as the starting point, so skip it
							}
						}
						else
						{
							sx = x;
							sy = y;
						}
						stbtt__setvertex(vertices + num_vertices++, GlyphVertexType.Move, sx, sy, 0, 0);
						was_off = 0;
						next_move = 1 + ttUSHORT(endPtsOfContours + j * 2);
						++j;
					}
					else
					{
						if (0 == (flags & 1))
						{
							// if it's a curve
							if (was_off != 0) // two off-curve control points in a row means interpolate an on-curve midpoint
								stbtt__setvertex(vertices + num_vertices++, GlyphVertexType.Curve, (cx + x) >> 1, (cy + y) >> 1, cx, cy);
							cx = x;
							cy = y;
							was_off = 1;
						}
						else
						{
							if (was_off != 0)
								stbtt__setvertex(vertices + num_vertices++, GlyphVertexType.Curve, x, y, cx, cy);
							else
								stbtt__setvertex(vertices + num_vertices++, GlyphVertexType.Line, x, y, 0, 0);
							was_off = 0;
						}
					}
				}
				num_vertices = stbtt__close_shape(vertices, num_vertices, was_off, start_off, sx, sy, scx, scy, cx, cy);
			}
			else if (numberOfContours == -1)
			{
				// Compound shapes.
				int more = 1;
				FakePtr<byte> comp = data + g + 10;
				num_vertices = 0;
				vertices.MakeNull();
				while (more != 0)
				{
					ushort flags, gidx;
					int comp_num_verts = 0, i;
					FakePtr<GlyphVertex> comp_verts = new FakePtr<GlyphVertex>(), tmp = new FakePtr<GlyphVertex>();
					float mtx0 = 1, mtx1 = 0, mtx2 = 0, mtx3 = 1, mtx4 = 0, mtx5 = 0, m, n;

					flags = ttUSHORT(comp);
					comp += 2;
					gidx = ttUSHORT(comp);
					comp += 2;

					if (0 != (flags & 2))
					{
						// XY values
						if (0 != (flags & 1))
						{
							// shorts
							mtx4 = ttSHORT(comp);
							comp += 2;
							mtx5 = ttSHORT(comp);
							comp += 2;
						}
						else
						{
							mtx4 = ttCHAR(comp);
							comp += 1;
							mtx5 = ttCHAR(comp);
							comp += 1;
						}
					}
					else
					{
						// @TODO handle matching point
						STBTT_assert(0);
					}
					if (0 != (flags & (1 << 3)))
					{
						// WE_HAVE_A_SCALE
						mtx0 = mtx3 = ttSHORT(comp) / 16384.0f;
						comp += 2;
						mtx1 = mtx2 = 0;
					}
					else if (0 != (flags & (1 << 6)))
					{
						// WE_HAVE_AN_X_AND_YSCALE
						mtx0 = ttSHORT(comp) / 16384.0f;
						comp += 2;
						mtx1 = mtx2 = 0;
						mtx3 = ttSHORT(comp) / 16384.0f;
						comp += 2;
					}
					else if (0 != (flags & (1 << 7)))
					{
						// WE_HAVE_A_TWO_BY_TWO
						mtx0 = ttSHORT(comp) / 16384.0f;
						comp += 2;
						mtx1 = ttSHORT(comp) / 16384.0f;
						comp += 2;
						mtx2 = ttSHORT(comp) / 16384.0f;
						comp += 2;
						mtx3 = ttSHORT(comp) / 16384.0f;
						comp += 2;
					}

					// Find transformation scales.
					m = (float)Math.Sqrt(mtx0 * mtx0 + mtx1 * mtx1);
					n = (float)Math.Sqrt(mtx2 * mtx2 + mtx3 * mtx3);

					// Get indexed glyph.
					comp_num_verts = stbtt_GetGlyphShape(ref info, gidx, out comp_verts);
					if (comp_num_verts > 0)
					{
						// Transform vertices.
						for (i = 0; i < comp_num_verts; ++i)
						{
							FakePtr<GlyphVertex> v = comp_verts + i;
							short x, y;

							var _ = v.Value;
							x = _.X;
							y = _.Y;
							_.X = (short)(m * (mtx0 * x + mtx2 * y + mtx4));
							_.Y = (short)(n * (mtx1 * x + mtx3 * y + mtx5));
							x = _.CX;
							y = _.CY;
							_.CX = (short)(m * (mtx0 * x + mtx2 * y + mtx4));
							_.CY = (short)(n * (mtx1 * x + mtx3 * y + mtx5));
							v.Value = _;
						}
						// Append vertices.
						tmp = STBTT_malloc<GlyphVertex>(num_vertices + comp_num_verts);
						if (tmp.IsNull)
						{
							if (!vertices.IsNull) STBTT_free(vertices);
							if (!comp_verts.IsNull) STBTT_free(comp_verts);
							return 0;
						}
						if (num_vertices > 0) STBTT_memcpy(tmp, vertices, num_vertices);
						STBTT_memcpy(tmp + num_vertices, comp_verts, comp_num_verts);
						if (!vertices.IsNull) STBTT_free(vertices);
						vertices = tmp;
						STBTT_free(comp_verts);
						num_vertices += comp_num_verts;
					}
					// More components ?
					more = flags & (1 << 5);
				}
			}
			else if (numberOfContours < 0)
			{
				// @TODO other compound variations?
				STBTT_assert(0);
			}
			else
			{
				// numberOfCounters == 0, do nothing
			}

			pvertices = vertices;
			return num_vertices;
		}

		public static void stbtt_GetGlyphHMetrics(ref stbtt_fontinfo info, uint glyph_index, out int advanceWidth, out int leftSideBearing)
		{
			ushort numOfLongHorMetrics = ttUSHORT(info.data + info.hhea + 34);
			if (glyph_index < numOfLongHorMetrics)
			{
				advanceWidth = ttSHORT(info.data + info.hmtx + 4 * glyph_index);
				leftSideBearing = ttSHORT(info.data + info.hmtx + 4 * glyph_index + 2);
			}
			else
			{
				advanceWidth = ttSHORT(info.data + info.hmtx + 4 * (numOfLongHorMetrics - 1));
				leftSideBearing = ttSHORT(info.data + info.hmtx + 4 * numOfLongHorMetrics + 2 * (glyph_index - numOfLongHorMetrics));
			}
		}

		public static int stbtt_GetGlyphKernAdvance(ref stbtt_fontinfo info, uint glyph1, uint glyph2)
		{
			FakePtr<byte> data = info.data + info.kern;
			uint needle, straw;
			int l, r, m;

			// we only look at the first table. it must be 'horizontal' and format 0.
			if (0 == info.kern)
				return 0;
			if (ttUSHORT(data + 2) < 1) // number of tables, need at least 1
				return 0;
			if (ttUSHORT(data + 8) != 1) // horizontal flag must be set in format
				return 0;

			l = 0;
			r = ttUSHORT(data + 10) - 1;
			needle = glyph1 << 16 | glyph2;
			while (l <= r)
			{
				m = (l + r) >> 1;
				straw = ttULONG(data + 18 + (m * 6)); // note: unaligned read
				if (needle < straw)
					r = m - 1;
				else if (needle > straw)
					l = m + 1;
				else
					return ttSHORT(data + 22 + (m * 6));
			}
			return 0;
		}

		public static int stbtt_GetCodepointKernAdvance(ref stbtt_fontinfo info, uint ch1, uint ch2)
		{
			if (0 == info.kern) // if no kerning table, don't waste time looking up both codepoint->glyphs
				return 0;
			return stbtt_GetGlyphKernAdvance(ref info, stbtt_FindGlyphIndex(ref info, ch1), stbtt_FindGlyphIndex(ref info, ch2));
		}

		public static void stbtt_GetCodepointHMetrics(ref stbtt_fontinfo info, uint codepoint, out int advanceWidth, out int leftSideBearing)
		{
			stbtt_GetGlyphHMetrics(ref info, stbtt_FindGlyphIndex(ref info, codepoint), out advanceWidth, out leftSideBearing);
		}

		public static void stbtt_GetFontVMetrics(ref stbtt_fontinfo info, out int ascent, out int descent, out int lineGap)
		{
			ascent = ttSHORT(info.data + info.hhea + 4);
			descent = ttSHORT(info.data + info.hhea + 6);
			lineGap = ttSHORT(info.data + info.hhea + 8);
		}

		public static float stbtt_ScaleForPixelHeight(ref stbtt_fontinfo info, float height)
		{
			int fheight = ttSHORT(info.data + info.hhea + 4) - ttSHORT(info.data + info.hhea + 6);
			return (float)height / fheight;
		}

		public static void stbtt_FreeShape(ref stbtt_fontinfo info, FakePtr<GlyphVertex> v)
		{
			STBTT_free(v);
		}

//////////////////////////////////////////////////////////////////////////////
//
// antialiasing software rasterizer
//

		public static void stbtt_GetGlyphBitmapBoxSubpixel(ref stbtt_fontinfo font, uint glyph, float scale_x, float scale_y, float shift_x, float shift_y, out int ix0, out int iy0, out int ix1, out int iy1)
		{
			int x0, y0, x1, y1;
			if (0 == stbtt_GetGlyphBox(ref font, glyph, out x0, out y0, out x1, out y1))
				x0 = y0 = x1 = y1 = 0; // e.g. space character
			// now move to integral bboxes (treating pixels as little squares, what pixels get touched)?
			ix0 = STBTT_ifloor(x0 * scale_x + shift_x);
			iy0 = -STBTT_iceil(y1 * scale_y + shift_y);
			ix1 = STBTT_iceil(x1 * scale_x + shift_x);
			iy1 = -STBTT_ifloor(y0 * scale_y + shift_y);
		}

		public static void stbtt_GetGlyphBitmapBox(ref stbtt_fontinfo font, uint glyph, float scale_x, float scale_y, out int ix0, out int iy0, out int ix1, out int iy1)
		{
			stbtt_GetGlyphBitmapBoxSubpixel(ref font, glyph, scale_x, scale_y, 0, 0, out ix0, out iy0, out ix1, out iy1);
		}

		public static void stbtt_GetCodepointBitmapBoxSubpixel(ref stbtt_fontinfo font, uint codepoint,
			float scale_x, float scale_y, float shift_x, float shift_y,
			out int ix0, out int iy0, out int ix1, out int iy1)
		{
			stbtt_GetCodepointBitmapBoxSubpixel(ref font, stbtt_FindGlyphIndex(ref font, codepoint),
				scale_x, scale_y, shift_x, shift_y, out ix0, out iy0, out ix1, out iy1);
		}

		public static void stbtt_GetCodepointBitmapBox(ref stbtt_fontinfo font, uint codepoint,
			float scale_x, float scale_y, out int ix0, out int iy0, out int ix1, out int iy1)
		{
			stbtt_GetCodepointBitmapBoxSubpixel(ref font, codepoint, scale_x, scale_y, 0, 0, out ix0, out iy0, out ix1, out iy1);
		}

		struct stbtt__edge
		{
			public float x0, y0, x1, y1;
			public int invert;
		}

		class stbtt__active_edge
		{
			public int x, dx;
			public float ey;
			public stbtt__active_edge next;
			public int valid;
		}

		const int FIXSHIFT = 10;
		const int FIX = (1 << FIXSHIFT);
		const int FIXMASK = (FIX - 1);

		static stbtt__active_edge new_active(stbtt__edge e, int off_x, float start_point)
		{
			stbtt__active_edge z = STBTT_malloc<stbtt__active_edge>(); // @TODO: make a pool of these!!!
			float dxdy = (e.x1 - e.x0) / (e.y1 - e.y0);
			STBTT_assert(e.y0 <= start_point);
			if (null == z) return z;
			// round dx down to avoid going too far
			if (dxdy < 0)
				z.dx = -STBTT_ifloor(FIX * -dxdy);
			else
				z.dx = STBTT_ifloor(FIX * dxdy);
			z.x = STBTT_ifloor(FIX * (e.x0 + dxdy * (start_point - e.y0)));
			z.x -= off_x * FIX;
			z.ey = e.y1;
			z.next = null;
			z.valid = e.invert != 0 ? 1 : -1;
			return z;
		}

		// note: this routine clips fills that extend off the edges... ideally this
		// wouldn't happen, but it could happen if the truetype glyph bounding boxes
		// are wrong, or if the user supplies a too-small bitmap
		static void stbtt__fill_active_edges(FakePtr<byte> scanline, int len, stbtt__active_edge e, int max_weight)
		{
			// non-zero winding fill
			int x0 = 0, w = 0;

			while (e != null)
			{
				if (w == 0)
				{
					// if we're currently at zero, we need to record the edge start point
					x0 = e.x;
					w += e.valid;
				}
				else
				{
					int x1 = e.x;
					w += e.valid;
					// if we went to zero, we need to draw
					if (w == 0)
					{
						int i = x0 >> FIXSHIFT;
						int j = x1 >> FIXSHIFT;

						if (i < len && j >= 0)
						{
							if (i == j)
							{
								// x0,x1 are the same pixel, so compute combined coverage
								scanline[i] = (byte)(scanline[i] + (byte)((x1 - x0) * max_weight >> FIXSHIFT));
							}
							else
							{
								if (i >= 0) // add antialiasing for x0
									scanline[i] = (byte)(scanline[i] + (byte)(((FIX - (x0 & FIXMASK)) * max_weight) >> FIXSHIFT));
								else
									i = -1; // clip

								if (j < len) // add antialiasing for x1
									scanline[j] = (byte)(scanline[j] + (byte)(((x1 & FIXMASK) * max_weight) >> FIXSHIFT));
								else
									j = len; // clip

								for (++i; i < j; ++i) // fill pixels between x0 and x1
									scanline[i] = (byte)(scanline[i] + (byte)max_weight);
							}
						}
					}
				}

				e = e.next;
			}
		}

		static void stbtt__rasterize_sorted_edges(ref stbtt__bitmap result, FakePtr<stbtt__edge> e, int n, int vsubsample, int off_x, int off_y)
		{
			stbtt__active_edge activeIsNext = new stbtt__active_edge(); // jfb: If only I could do pointers to stack...*sigh* Oh C#, why so arbitrarily limited...
			int y, j = 0;
			int max_weight = (255 / vsubsample); // weight per vertical scanline
			int s; // vertical subsample index
			FakePtr<byte> scanline_data = STBTT_malloc<byte>(512), scanline;

			if (result.w > 512)
				scanline = STBTT_malloc<byte>(result.w);
			else
				scanline = scanline_data;

			y = off_y * vsubsample;
			{
				var _ = e[n];
				_.y0 = (off_y + result.h) * (float)vsubsample + 1;
				e[n] = _;
			}

			while (j < result.h)
			{
				STBTT_memset(scanline, result.w);
				for (s = 0; s < vsubsample; ++s)
				{
					// find center of pixel for this scanline
					float scan_y = y + 0.5f;
					stbtt__active_edge stepIsNext = activeIsNext;

					// update all active edges;
					// remove all active edges that terminate before the center of this scanline
					while (stepIsNext.next != null)
					{
						stbtt__active_edge z = stepIsNext.next;
						if (z.ey <= scan_y)
						{
							stepIsNext.next = z.next; // delete from list
							STBTT_assert(z.valid);
							z.valid = 0;
							//STBTT_free(z);
						}
						else
						{
							z.x += z.dx; // advance to position for current scanline
							stepIsNext = stepIsNext.next; // advance through list
						}
					}

					// resort the list if needed
					for (;;)
					{
						int changed = 0;
						stepIsNext = activeIsNext;
						while (stepIsNext.next != null && stepIsNext.next.next != null)
						{
							if (stepIsNext.next.x > stepIsNext.next.next.x)
							{
								stbtt__active_edge t = stepIsNext.next;
								stbtt__active_edge q = t.next;

								t.next = q.next;
								q.next = t;
								stepIsNext.next = q;
								changed = 1;
							}
							stepIsNext = stepIsNext.next;
						}
						if (0 == changed) break;
					}

					// insert all edges that start before the center of this scanline -- omit ones that also end on this scanline
					while (e[0].y0 <= scan_y)
					{
						if (e[0].y1 > scan_y)
						{
							stbtt__active_edge z = new_active(e[0], off_x, scan_y);
							// find insertion point
							if (activeIsNext.next == null)
								activeIsNext.next = z;
							else if (z.x < activeIsNext.next.x)
							{
								// insert at front
								z.next = activeIsNext.next;
								activeIsNext.next = z;
							}
							else
							{
								// find thing to insert AFTER
								stbtt__active_edge p = activeIsNext.next;
								while (p.next != null && p.next.x < z.x)
									p = p.next;
								// at this point, p->next->x is NOT < z->x
								z.next = p.next;
								p.next = z;
							}
						}
						++e;
					}

					// now process all active edges in XOR fashion
					if (activeIsNext.next != null)
						stbtt__fill_active_edges(scanline, result.w, activeIsNext.next, max_weight);

					++y;
				}
				STBTT_memcpy(result.pixels + j * result.stride, scanline, result.w);
				++j;
			}

			if (scanline != scanline_data)
				STBTT_free(scanline);
		}

		static int stbtt__edge_compare(stbtt__edge a, stbtt__edge b)
		{
			if (a.y0 < b.y0) return -1;
			if (a.y0 > b.y0) return 1;
			return 0;
		}

		static void stbtt__rasterize(ref stbtt__bitmap result, FakePtr<ContourPoint> pts, FakePtr<int> wcount, int windings, float scale_x, float scale_y, float shift_x, float shift_y, int off_x, int off_y, int invert)
		{
			float y_scale_inv = invert != 0 ? -scale_y : scale_y;
			FakePtr<stbtt__edge> e;
			int n, i, j, k, m;
			int vsubsample = result.h < 8 ? 15 : 5;
			// vsubsample should divide 255 evenly; otherwise we won't reach full opacity

			// now we have to blow out the windings into explicit edge lists
			n = 0;
			for (i = 0; i < windings; ++i)
				n += wcount[i];

			e = STBTT_malloc<stbtt__edge>(n + 1); // add an extra one as a sentinel
			if (e.IsNull) return;
			n = 0;

			m = 0;
			for (i = 0; i < windings; ++i)
			{
				FakePtr<ContourPoint> p = pts + m;
				m += wcount[i];
				j = wcount[i] - 1;
				for (k = 0; k < wcount[i]; j = k++)
				{
					int a = k, b = j;
					// skip the edge if horizontal
					if (p[j].Y == p[k].Y)
						continue;
					// add edge from j to k to the list
					{
						var _ = e[n];
						_.invert = 0;
						if (invert != 0 ? p[j].Y > p[k].Y : p[j].Y < p[k].Y)
						{
							_.invert = 1;
							a = j;
							b = k;
						}
						_.x0 = p[a].X * scale_x + shift_x;
						_.y0 = p[a].Y * y_scale_inv * vsubsample + shift_y;
						_.x1 = p[b].X * scale_x + shift_x;
						_.y1 = p[b].Y * y_scale_inv * vsubsample + shift_y;
						e[n] = _;
					}
					++n;
				}
			}

			// now sort the edges by their highest point (should snap to integer, and then by x)
			STBTT_sort(ref e, n, stbtt__edge_compare);

			// now, traverse the scanlines and find the intersections on each scanline, use xor winding rule
			stbtt__rasterize_sorted_edges(ref result, e, n, vsubsample, off_x, off_y);

			STBTT_free(e);
		}

		static void stbtt__add_point(FakePtr<ContourPoint> points, int n, float x, float y)
		{
			if (points.IsNull) return; // during first pass, it's unallocated
			ContourPoint p;
			p.X = x;
			p.Y = y;
			points[n] = p;
		}

		// tesselate until threshhold p is happy... @TODO warped to compensate for non-linear stretching
		static int stbtt__tesselate_curve(FakePtr<ContourPoint> points, ref int num_points, float x0, float y0, float x1, float y1, float x2, float y2, float objspace_flatness_squared, int n)
		{
			// midpoint
			float mx = (x0 + 2 * x1 + x2) / 4;
			float my = (y0 + 2 * y1 + y2) / 4;
			// versus directly drawn line
			float dx = (x0 + x2) / 2 - mx;
			float dy = (y0 + y2) / 2 - my;
			if (n > 16) // 65536 segments on one curve better be enough!
				return 1;
			if (dx * dx + dy * dy > objspace_flatness_squared)
			{
				// half-pixel error allowed... need to be smaller if AA
				stbtt__tesselate_curve(points, ref num_points, x0, y0, (x0 + x1) / 2.0f, (y0 + y1) / 2.0f, mx, my, objspace_flatness_squared, n + 1);
				stbtt__tesselate_curve(points, ref num_points, mx, my, (x1 + x2) / 2.0f, (y1 + y2) / 2.0f, x2, y2, objspace_flatness_squared, n + 1);
			}
			else
			{
				stbtt__add_point(points, num_points, x2, y2);
				num_points = num_points + 1;
			}
			return 1;
		}

		// returns number of contours
		public static FakePtr<ContourPoint> stbtt_FlattenCurves(FakePtr<GlyphVertex> vertices, int num_verts, float objspace_flatness, out FakePtr<int> contour_lengths, out int num_contours)
		{
			FakePtr<ContourPoint> points = new FakePtr<ContourPoint>();
			int num_points = 0;

			float objspace_flatness_squared = objspace_flatness * objspace_flatness;
			int i, n = 0, start = 0, pass;

			// count how many "moves" there are to get the contour count
			for (i = 0; i < num_verts; ++i)
				if (vertices[i].Type == GlyphVertexType.Move)
					++n;

			num_contours = n;
			if (n == 0)
			{
				contour_lengths = new FakePtr<int>();
				return new FakePtr<ContourPoint>();
			}

			contour_lengths = STBTT_malloc<int>(n);

			if (contour_lengths.IsNull)
			{
				num_contours = 0;
				return new FakePtr<ContourPoint>();
			}

			// make two passes through the points so we don't need to realloc
			for (pass = 0; pass < 2; ++pass)
			{
				float x = 0, y = 0;
				if (pass == 1)
				{
					points = STBTT_malloc<ContourPoint>(num_points);
					if (points.IsNull) goto error;
				}
				num_points = 0;
				n = -1;
				for (i = 0; i < num_verts; ++i)
				{
					switch (vertices[i].Type)
					{
						case GlyphVertexType.Move:
							// start the next contour
							if (n >= 0)
								contour_lengths[n] = num_points - start;
							++n;
							start = num_points;

							x = vertices[i].X;
							y = vertices[i].Y;
							stbtt__add_point(points, num_points++, x, y);
							break;
						case GlyphVertexType.Line:
							x = vertices[i].X;
							y = vertices[i].Y;
							stbtt__add_point(points, num_points++, x, y);
							break;
						case GlyphVertexType.Curve:
							stbtt__tesselate_curve(points, ref num_points, x, y,
								vertices[i].CX, vertices[i].CY,
								vertices[i].X, vertices[i].Y,
								objspace_flatness_squared, 0);
							x = vertices[i].X;
							y = vertices[i].Y;
							break;
					}
				}
				contour_lengths[n] = num_points - start;
			}

			return points;
			error:
			STBTT_free(points);
			STBTT_free(contour_lengths);
			contour_lengths.MakeNull();
			num_contours = 0;
			return new FakePtr<ContourPoint>();
		}

		public static void stbtt_Rasterize(ref stbtt__bitmap result, float flatness_in_pixels, FakePtr<GlyphVertex> vertices, int num_verts, float scale_x, float scale_y, float shift_x, float shift_y, int x_off, int y_off, int invert)
		{
			float scale = scale_x > scale_y ? scale_y : scale_x;
			int winding_count;
			FakePtr<int> winding_lengths;
			FakePtr<ContourPoint> windings = stbtt_FlattenCurves(vertices, num_verts, flatness_in_pixels / scale, out winding_lengths, out winding_count);
			if (!windings.IsNull)
			{
				stbtt__rasterize(ref result, windings, winding_lengths, winding_count, scale_x, scale_y, shift_x, shift_y, x_off, y_off, invert);
				STBTT_free(winding_lengths);
				STBTT_free(windings);
			}
		}

		public static void stbtt_FreeBitmap(FakePtr<byte> bitmap)
		{
			STBTT_free(bitmap);
		}

		public static FakePtr<byte> stbtt_GetGlyphBitmapSubpixel(ref stbtt_fontinfo info, float scale_x, float scale_y, float shift_x, float shift_y, uint glyph, out int width, out int height, out int xoff, out int yoff)
		{
			int ix0 = 0, iy0 = 0, ix1 = 0, iy1 = 0;
			stbtt__bitmap gbm;
			FakePtr<GlyphVertex> vertices;
			int num_verts = stbtt_GetGlyphShape(ref info, glyph, out vertices);

			if (scale_x == 0) scale_x = scale_y;
			if (scale_y == 0)
			{
				if (scale_x == 0)
				{
					width = 0;
					height = 0;
					xoff = 0;
					yoff = 0;
					return new FakePtr<byte>();
				}
				scale_y = scale_x;
			}

			stbtt_GetGlyphBitmapBoxSubpixel(ref info, glyph, scale_x, scale_y, shift_x, shift_y, out ix0, out iy0, out ix1, out iy1);

			// now we get the size
			gbm.w = (ix1 - ix0);
			gbm.h = (iy1 - iy0);
			gbm.pixels = new FakePtr<byte>(); // in case we error

			width = gbm.w;
			height = gbm.h;
			xoff = ix0;
			yoff = iy0;

			if (gbm.w > 0 && gbm.h > 0)
			{
				gbm.pixels = STBTT_malloc<byte>(gbm.w * gbm.h);
				if (!gbm.pixels.IsNull)
				{
					gbm.stride = gbm.w;

					stbtt_Rasterize(ref gbm, 0.35f, vertices, num_verts, scale_x, scale_y, shift_x, shift_y, ix0, iy0, 1);
				}
			}
			STBTT_free(vertices);
			return gbm.pixels;
		}

		public static FakePtr<byte> stbtt_GetGlyphBitmap(ref stbtt_fontinfo info, float scale_x, float scale_y, uint glyph, out int width, out int height, out int xoff, out int yoff)
		{
			return stbtt_GetGlyphBitmapSubpixel(ref info, scale_x, scale_y, 0, 0, glyph, out width, out height, out xoff, out yoff);
		}

		public static FakePtr<byte> stbtt_GetCodepointBitmapSubpixel(ref stbtt_fontinfo info, float scale_x, float scale_y, float shift_x, float shift_y, uint codepoint, out int width, out int height, out int xoff, out int yoff)
		{
			return stbtt_GetGlyphBitmapSubpixel(ref info, scale_x, scale_y, shift_x, shift_y, stbtt_FindGlyphIndex(ref info, codepoint), out width, out height, out xoff, out yoff);
		}

		public static FakePtr<byte> stbtt_GetCodepointBitmap(ref stbtt_fontinfo info, float scale_x, float scale_y, uint codepoint, out int width, out int height, out int xoff, out int yoff)
		{
			return stbtt_GetCodepointBitmapSubpixel(ref info, scale_x, scale_y, 0, 0, codepoint, out width, out height, out xoff, out yoff);
		}
	}
}