#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Stefan Lange
//
// Copyright (c) 2005-2016 empira Software GmbH, Cologne Area (Germany)
//
// http://www.pdfsharp.com
// http://sourceforge.net/projects/pdfsharp
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion

using System.Diagnostics;
using System.Globalization;
using Ravlyk.Adopted.PdfSharp.Internal;
using GdiFontFamily = System.Drawing.FontFamily;

namespace Ravlyk.Adopted.PdfSharp.Drawing
{
    /// <summary>
    /// Internal implementation class of XFontFamily.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay}")]
    class FontFamilyInternal
    {
        // Implementation Notes
        // FontFamilyInternal implements an XFontFamily.
        //
        // * Each XFontFamily object is just a handle to its FontFamilyInternal singleton.
        //
        // * A FontFamilyInternal is is uniquely identified by its name. It
        //    is not possible to use two different fonts that have the same
        //    family name.

        FontFamilyInternal(string familyName, bool createPlatformObjects)
        {
            _sourceName = _name = familyName;
#if CORE || GDI
            if (createPlatformObjects)
            {
                _gdiFontFamily = new GdiFontFamily(familyName);
                _name = _gdiFontFamily.Name;
            }
#endif
        }

#if CORE || GDI
        FontFamilyInternal(GdiFontFamily gdiFontFamily)
        {
            _sourceName = _name = gdiFontFamily.Name;
            _gdiFontFamily = gdiFontFamily;
        }
#endif

        internal static FontFamilyInternal GetOrCreateFromName(string familyName, bool createPlatformObject)
        {
            try
            {
                Lock.EnterFontFactory();
                FontFamilyInternal family = FontFamilyCache.GetFamilyByName(familyName);
                if (family == null)
                {
                    family = new FontFamilyInternal(familyName, createPlatformObject);
                    family = FontFamilyCache.CacheOrGetFontFamily(family);
                }
                return family;
            }
            finally { Lock.ExitFontFactory(); }
        }

#if CORE || GDI
        internal static FontFamilyInternal GetOrCreateFromGdi(GdiFontFamily gdiFontFamily)
        {
            try
            {
                Lock.EnterFontFactory();
                FontFamilyInternal fontFamily = new FontFamilyInternal(gdiFontFamily);
                fontFamily = FontFamilyCache.CacheOrGetFontFamily(fontFamily);
                return fontFamily;
            }
            finally { Lock.ExitFontFactory(); }
        }
#endif

        /// <summary>
        /// Gets the family name this family was originally created with.
        /// </summary>
        public string SourceName
        {
            get { return _sourceName; }
        }
        readonly string _sourceName;

        /// <summary>
        /// Gets the name that uniquely identifies this font family.
        /// </summary>
        public string Name
        {
            // In WPF this is the Win32FamilyName, not the WPF family name.
            get { return _name; }
        }
        readonly string _name;

#if CORE || GDI
        /// <summary>
        /// Gets the underlying GDI+ font family object.
        /// Is null if the font was created by a font resolver.
        /// </summary>
        public GdiFontFamily GdiFamily
        {
            get { return _gdiFontFamily; }
        }
        readonly GdiFontFamily _gdiFontFamily;
#endif

        /// <summary>
        /// Gets the DebuggerDisplayAttribute text.
        /// </summary>
        // ReSha rper disable UnusedMember.Local
        internal string DebuggerDisplay
        // ReShar per restore UnusedMember.Local
        {
            get { return string.Format(CultureInfo.InvariantCulture, "FontFamiliy: '{0}'", Name); }
        }
    }
}
