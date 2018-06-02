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

using System.Drawing.Drawing2D;

namespace Ravlyk.Adopted.PdfSharp.Drawing
{
    /// <summary>
    /// Converts XGraphics enums to GDI+ enums.
    /// </summary>
    internal static class XConvert
    {
#if GDI
//#if UseGdiObjects
        /// <summary>
        /// Converts XLineJoin to LineJoin.
        /// </summary>
        public static LineJoin ToLineJoin(XLineJoin lineJoin)
        {
            return GdiLineJoin[(int)lineJoin];
        }
        static readonly LineJoin[] GdiLineJoin = new LineJoin[] { LineJoin.Miter, LineJoin.Round, LineJoin.Bevel };
//#endif
#endif

#if GDI
//#if UseGdiObjects
        /// <summary>
        /// Converts XLineCap to LineCap.
        /// </summary>
        public static LineCap ToLineCap(XLineCap lineCap)
        {
            return _gdiLineCap[(int)lineCap];
        }
        static readonly LineCap[] _gdiLineCap = new LineCap[] { LineCap.Flat, LineCap.Round, LineCap.Square };
        //#endif
#endif
    }
}